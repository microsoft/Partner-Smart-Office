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
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Azure.Documents.Client;
    using CsvHelper;
    using Models;
    using Newtonsoft.Json;

    internal class Program
    {
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
                    throw new Exception("Please specify the path to the Secure Score control CSV file.");
                }

                if (!File.Exists(args[0]))
                {
                    throw new FileNotFoundException($"Unable to locate {args[0]}. Please check the path and try again.");
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

        private static async Task RunAsync(string path)
        {
            CsvReader reader = null;
            GraphService graphService;
            List<Customer> customers;
            DocumentClient client;
            List<SecureScore> scores;
            List<ControlListEntry> records;
            PartnerCenter partnerCenter;

            try
            {
                graphService = new GraphService();
                partnerCenter = new PartnerCenter();
                customers = await partnerCenter.GetCustomersAsync(Guid.NewGuid()).ConfigureAwait(false);

                client = new DocumentClient(
                    new Uri(ConfigurationManager.AppSettings["CosmosDbEndpoint"]),
                    ConfigurationManager.AppSettings["CosmosDbAccessKey"]);

                using (TextReader textReader = File.OpenText(path))
                {
                    reader = new CsvReader(textReader);
                    reader.Configuration.RegisterClassMap<ControlListEntryMap>();

                    records = reader.GetRecords<ControlListEntry>().ToList();

                    foreach (ControlListEntry entry in records)
                    {
                        await client.UpsertDocumentAsync(
                             "dbs/SmartOffice/colls/SecureScoreControls",
                             entry).ConfigureAwait(false);
                    }
                }

                foreach (Customer customer in customers)
                {
                    await client.UpsertDocumentAsync(
                        "dbs/SmartOffice/colls/Customers",
                        customer).ConfigureAwait(false);

                    try
                    {
                        scores = await graphService.GetSecureScoreAsync(customer.Id, 30).ConfigureAwait(false);

                        if (scores != null)
                        {
                            foreach (SecureScore score in scores)
                            {
                                await client.UpsertDocumentAsync(
                                  "dbs/SmartOffice/colls/SecureScore",
                                  score).ConfigureAwait(false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        /* Ignoring errors right now because I am not querying subscriptions. */

                        Console.WriteLine(ex.ToString());
                    }
                }
            }
            finally
            {
                customers = null;
            }
        }
    }
}