// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Formatters.Json
{
    internal class ValidationProblemDetailsConverter : ProblemDetailsConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ValidationProblemDetails);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            if (!(existingValue is ValidationProblemDetails problemDetails))
            {
                problemDetails = new ValidationProblemDetails();
            }

            while (reader.Read() && (reader.TokenType == JsonToken.PropertyName))
            {
                if (string.Equals("errors", (string)reader.Value, StringComparison.Ordinal))
                {
                    if (!reader.Read())
                    {
                        throw new JsonSerializationException();
                    }

                    serializer.Populate(reader, problemDetails.Errors);
                }
                else
                {
                    ReadValue(reader, problemDetails, serializer);
                }

            }

            return problemDetails;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var problemDetails = (ValidationProblemDetails)value;

            writer.WriteStartObject();
            WriteProperties(writer, problemDetails, serializer);
            
            if (problemDetails.Errors.Count > 0)
            {
                writer.WritePropertyName("errors");
                serializer.Serialize(writer, problemDetails.Errors);
            }

            writer.WriteEndObject();
        }
    }
}
