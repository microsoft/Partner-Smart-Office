// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SmartOffice.Aggregator.Converters
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Provides the ability to quickly convert an object into a different type.
    /// </summary>
    public static class ResourceConverter
    {
        /// <summary>
        /// Converts the resource to the specified output object.
        /// </summary>
        /// <param name="resource">The base resource to be used for the conversion.</param>
        /// <param name="additionalValues">Additional values to be added to the converted object.</param>
        /// <returns>An object with a type that matches the specified output.</returns>
        /// <remarks>
        /// This process will only clone property values between the input and output. Additional values will 
        /// only be added if there is a property with the same key value. All other key value pairs will be ignored.
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