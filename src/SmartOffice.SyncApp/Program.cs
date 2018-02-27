// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.SyncApp
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Azure.Documents;
    using Azure.Documents.Client;
    using CsvHelper;
    using IdentityModel.Clients.ActiveDirectory;
    using Models;
    using Newtonsoft.Json;

    internal class Program
    {
        /// <summary>
        /// Name of the bulk import stored procedure.
        /// </summary>
        private const string BulkImportProcedureName = "BulkImport";

        /// <summary>
        /// Name of the customers collection.
        /// </summary>
        private const string CustomersCollection = "Customers";

        /// <summary>
        /// Name of the database.
        /// </summary>
        private const string DatabaseName = "SmartOffice";

        /// <summary>
        /// Name of the Secure Score collection.
        /// </summary>
        private const string SecureScoreCollection = "SecureScore";

        /// <summary>
        /// Name of the Secure Score controls collection.
        /// </summary>
        private const string SecureScoreControlsCollection = "SecureScoreControls";

        /// <summary>
        /// Entry point for the console application.
        /// </summary>
        /// <param name="args">Arguments provided invoking the application</param>
        internal static void Main(string[] args)
        {
            try
            {
                if (args.Length != 1)
                {
                    throw new Exception(Resources.PathNotSpecifiedException);
                }

                if (!File.Exists(args[0]))
                {
                    throw new FileNotFoundException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.PathNotFoundException, args[0]));
                }

                JsonConvert.DefaultSettings = () =>
                {
                    return new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                };

                Task.Run(() => RunAsync(args[0])).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        /// <summary>
        /// Creates the required resources within the instance of Azure Cosmos DB.
        /// </summary>
        /// <returns>
        /// An instance of the <see cref="Task"/> class that represents the asynchronous operation.
        /// </returns>
        private static async Task CreateResourcesAsync()
        {
            DocumentCollection collection;
            bool createStoredProc; 
            string[] collections;

            try
            {
                collections = new string[] { CustomersCollection, SecureScoreCollection, SecureScoreControlsCollection };

                using (DocumentClient client = new DocumentClient(
                    new Uri(ConfigurationManager.AppSettings["CosmosDbEndpoint"]),
                    ConfigurationManager.AppSettings["CosmosDbAccessKey"],
                    new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    }))
                {
                    await client.CreateDatabaseIfNotExistsAsync(new Database { Id = DatabaseName }).ConfigureAwait(false);

                    foreach (string c in collections)
                    {
                        collection = new DocumentCollection
                        {
                            Id = c
                        };

                        await client.CreateDocumentCollectionIfNotExistsAsync(
                            UriFactory.CreateDatabaseUri(DatabaseName),
                            collection,
                            new RequestOptions { OfferThroughput = 400 }).ConfigureAwait(false);

                        try
                        {
                            createStoredProc = false;

                            await client.ReadStoredProcedureAsync(
                                UriFactory.CreateStoredProcedureUri(
                                    DatabaseName,
                                    c,
                                    BulkImportProcedureName)).ConfigureAwait(false);
                        }
                        catch (DocumentClientException ex)
                        {
                            if (ex.StatusCode != HttpStatusCode.NotFound)
                            {
                                throw;
                            }

                            createStoredProc = true;
                        }

                        if (createStoredProc)
                        {
                            await client.CreateStoredProcedureAsync(
                                UriFactory.CreateDocumentCollectionUri(
                                    DatabaseName,
                                    c),
                                new StoredProcedure
                                {
                                    Id = BulkImportProcedureName,
                                    Body = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}\\Scripts\\{BulkImportProcedureName}.js")
                                });
                        }
                    }
                }
            }
            finally
            {
                collection = null;
                collections = null;
            }
        }

        /// <summary>
        /// Synchronizes Secure Score resources into the instance of Azure Cosmos DB.
        /// </summary>
        /// <param name="path">The file to be opened for reading.</param>
        /// <returns>
        /// An instance of the <see cref="Task"/> class that represents the asynchronous operation.
        /// </returns>
        private static async Task RunAsync(string path)
        {
            CsvReader reader = null;
            List<Customer> customers;
            List<SecureScore> scores;
            List<ControlListEntry> records;

            try
            {
                await CreateResourcesAsync().ConfigureAwait(false);

                using (DocumentClient client = new DocumentClient(
                    new Uri(ConfigurationManager.AppSettings["CosmosDbEndpoint"]),
                    ConfigurationManager.AppSettings["CosmosDbAccessKey"],
                    new ConnectionPolicy
                    {
                        ConnectionMode = ConnectionMode.Direct,
                        ConnectionProtocol = Protocol.Tcp
                    }))
                {

                    using (TextReader textReader = File.OpenText(path))
                    {
                        reader = new CsvReader(textReader);
                        reader.Configuration.RegisterClassMap<ControlListEntryMap>();

                        records = reader.GetRecords<ControlListEntry>().ToList();

                        await client.ExecuteStoredProcedureAsync<int>(
                            UriFactory.CreateStoredProcedureUri(
                                DatabaseName,
                                SecureScoreControlsCollection,
                                BulkImportProcedureName),
                            records).ConfigureAwait(false);
                    }

                    customers = await new Services.PartnerService(
                        ConfigurationManager.AppSettings["PartnerCenterEndpoint"]).GetCustomersAsync(
                        new Services.RequestContext
                        {
                            AccessToken = await GetAccessTokenAsync().ConfigureAwait(false),
                            CorrelationId = Guid.NewGuid(),
                            Locale = "en-US"
                        }).ConfigureAwait(false);

                    await client.ExecuteStoredProcedureAsync<int>(
                        UriFactory.CreateStoredProcedureUri(
                            DatabaseName,
                            CustomersCollection,
                            BulkImportProcedureName),
                        customers).ConfigureAwait(false);

                    foreach (Customer customer in customers)
                    {
                        try
                        {
                            scores = await new Services.GraphService(ConfigurationManager.AppSettings["GraphEndpoint"])
                                .GetSecureScoreAsync(new Services.RequestContext
                                {
                                    AccessToken = await GetAccessTokenAsync(customer.Id).ConfigureAwait(false),
                                    CorrelationId = Guid.NewGuid(),
                                    Locale = "en-US",
                                },
                                30).ConfigureAwait(false);

                            if (scores != null)
                            {
                                await client.ExecuteStoredProcedureAsync<int>(
                                    UriFactory.CreateStoredProcedureUri(
                                        DatabaseName,
                                        SecureScoreCollection,
                                        BulkImportProcedureName),
                                    scores).ConfigureAwait(false);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"{customer.CompanyProfile.CompanyName} -- {customer.Id} -- {ex.Message}");
                        }
                    }
                }
            }
            finally
            {
                customers = null;
                reader = null;
                records = null;
                scores = null;
            }
        }

        private static async Task<string> GetAccessTokenAsync()
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                authContext = new AuthenticationContext(
                    $"{ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"]}/{ConfigurationManager.AppSettings["PartnerCenter.AccountId"]}");

                authResult = await authContext.AcquireTokenAsync(
                    "https://graph.windows.net",
                    new ClientCredential(
                        ConfigurationManager.AppSettings["PartnerCenter.ApplicationId"],
                        ConfigurationManager.AppSettings["PartnerCenter.ApplicationSecret"])).ConfigureAwait(false);

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
            }
        }

        private static async Task<string> GetAccessTokenAsync(string customerId)
        {
            AuthenticationContext authContext;
            AuthenticationResult authResult;

            try
            {
                authContext = new AuthenticationContext(
                    $"{ConfigurationManager.AppSettings["ActiveDirectoryEndpoint"]}/{customerId}");

                authResult = await authContext.AcquireTokenAsync(
                    ConfigurationManager.AppSettings["GraphEndpoint"],
                    new ClientCredential(
                        ConfigurationManager.AppSettings["ApplicationId"],
                        ConfigurationManager.AppSettings["ApplicationSecret"])).ConfigureAwait(false);

                return authResult.AccessToken;
            }
            finally
            {
                authContext = null;
            }
        }
    }
}