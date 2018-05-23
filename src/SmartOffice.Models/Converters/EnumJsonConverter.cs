// -----------------------------------------------------------------------
// <copyright file="EnumJsonConverter.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.Partner.SmartOffice.Models.Converters
{
    using System;
    using System.Globalization;
    using System.Text;
    using Newtonsoft.Json;

    public class EnumJsonConverter : JsonConverter
    {
        /// <summary>
        ///  Determines whether this instance can convert the specified object type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns><c>true</c> if this instance can convert the specified object type; otherwise, <c>false.</c></returns>
        public override bool CanConvert(Type objectType)
        {
            if (objectType != null)
            {
                return objectType.IsEnum;
            }

            return false; 
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return Enum.Parse(objectType, JScriptToPascalCase(reader.Value.ToString()));
            }
            else if (reader.TokenType == JsonToken.Integer)
            {
                return Enum.ToObject(objectType, reader.Value);
            }

            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        private static string JScriptToPascalCase(string jsonValue)
        {
            if (jsonValue == null)
            {
                throw new ArgumentNullException(nameof(jsonValue));

            }

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToUpper(jsonValue[0], CultureInfo.InvariantCulture));

            for (int index = 1; index < jsonValue.Length; ++index)
            {
                stringBuilder.Append(jsonValue[index] == '_' ? char.ToUpper(jsonValue[++index], CultureInfo.InvariantCulture) : jsonValue[index]);
            }

            return stringBuilder.ToString();
        }
    }
}