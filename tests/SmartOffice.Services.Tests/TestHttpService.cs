// -----------------------------------------------------------------------
// <copyright file="TestHttpService.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Services.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;
    using Newtonsoft.Json;

    internal class TestHttpService : IHttpService
    {
        private Dictionary<Type, Func<string>> functions;

        public TestHttpService()
        {
            functions = new Dictionary<Type, Func<string>>
            {
                { typeof(Resources<Customer>), GetCustomers },
                { typeof(List<SecureScore>), GetSecureScore }
            };
        }

        public async Task<TResponse> GetAsync<TResponse>(Uri requestUri, Dictionary<string, string> headersToAdd = null)
        {
            await Task.FromResult(0).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResponse>(functions[typeof(TResponse)].Invoke());
        }

        public async Task<TResponse> GetAsync<TResponse>(Uri requestUri, IRequestContext requestContext)
        {
            await Task.FromResult(0).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TResponse>(functions[typeof(TResponse)].Invoke());
        }

        private static string GetCustomers()
        {
            return JsonConvert.SerializeObject(new Resources<Customer>
            {
                Items = new List<Customer> 
                {
                    new Customer
                    {
                        CompanyProfile = new CompanyProfile
                        {
                            CompanyName = "Fabrikam Unlimited",
                            Domain = "fabrikameunlimited.onmicrosoft.com"
                        },
                        Id = Guid.NewGuid().ToString()
                    }
                }
            });
        }

        private static string GetSecureScore()
        {
            return JsonConvert.SerializeObject(new List<SecureScore>
            {
                {
                    new SecureScore
                    {
                        AccountScore = 10,
                        ActiveUserCount = 10,
                        AverageAccountScore = 10,
                        AverageDataScore = 10,
                        AverageDeviceScore = 10,
                        AverageMaxSecureScore = 10,
                        AverageSecureScore = 10,
                        ControlScores = new List<ControlScore>
                        {
                            {
                                new ControlScore
                                {
                                     ControlDetails = new ControlDetails
                                     {
                                          Count = 10,
                                          Total = 10
                                     }
                                }
                            }
                        },
                        CreatedDate = new CreatedDate
                        {
                             Day = DateTime.Now.Day,
                             Month = DateTime.Now.Month,
                             Year = DateTime.Now.Year
                        },
                        DataScore = 10,
                        DeviceScore = 10,
                        EnabledServices = new List<string>
                        {
                            { "Exchange" },
                            { "OD4B" }
                        },
                        LicensedUserCount = 10,
                        MaxSecureScore = 10,
                        Score = 10,
                        TenantId = Guid.NewGuid().ToString()
                    }
                }
            });
        }
    }
}