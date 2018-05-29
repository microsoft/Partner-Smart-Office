// -----------------------------------------------------------------------
// <copyright file="ResourceConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Functions.Converters
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
    using Services.PartnerCenter;

    public class ResourceConverter
    {
        /// <summary>
        /// Provides the ability to interact with Partner Center.
        /// </summary>
        private readonly IPartnerServiceClient client;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConverter{TInput, TOutput}" /> class.
        /// </summary>
        public ResourceConverter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceConverter{TInput, TOutput}" /> class.
        /// </summary>
        /// <param name="client">Provides the ability to interact with Partner Center.</param>
        public ResourceConverter(IPartnerServiceClient client)
        {
            this.client = client;
        }

        /// <summary>
        /// Converts the resource to the specified output object.
        /// </summary>
        /// <param name="resource">The base resource to be used for the conversion.</param>
        /// <param name="additionalValues">Additional values to be added to the converted object.</param>
        /// <returns>An object with a type that matches the specified output.</returns>
        /// <remarks>
        /// This process will only clone property values between the input and output. Additional values will 
        /// only be added if there is a property with the same key value. All other key vaule pairs will be ignored.
        /// </remarks>
        public static TOutput Convert<TInput, TOutput>(TInput resource, Dictionary<string, string> additionalValues = null) where TOutput : class, new()
        {
            TOutput newResource = new TOutput();

            foreach (PropertyInfo prop in newResource.GetType().GetRuntimeProperties())
            {
                if (resource.GetType().GetRuntimeProperty(prop.Name) != null)
                {
                    prop.SetValue(newResource, resource.GetType().GetRuntimeProperty(prop.Name).GetValue(resource));
                }

                if (additionalValues != null && additionalValues.ContainsKey(prop.Name))
                {
                    prop.SetValue(newResource, additionalValues[prop.Name]);
                }
            }

            return newResource;
        }
    }
}