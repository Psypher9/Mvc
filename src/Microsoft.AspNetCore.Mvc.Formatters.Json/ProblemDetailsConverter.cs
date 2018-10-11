// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Newtonsoft.Json;

namespace Microsoft.AspNetCore.Mvc.Formatters.Json
{
    internal class ProblemDetailsConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ProblemDetails);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            if (!(existingValue is ProblemDetails problemDetails))
            {
                problemDetails = new ProblemDetails();
            }

            while (reader.Read() && (reader.TokenType == JsonToken.PropertyName))
            {
                ReadValue(reader, problemDetails, serializer);
            }

            return problemDetails;
        }

        protected static void ReadValue(JsonReader reader, ProblemDetails problemDetails, JsonSerializer serializer)
        {
            var key = (string)reader.Value;
            if (!reader.Read())
            {
                throw new JsonSerializationException();
            }
            var value = reader.Value;

            switch (key)
            {
                case "detail":
                    problemDetails.Detail = (string)value;
                    break;

                case "instance":
                    problemDetails.Instance = (string)value;
                    break;

                case "status":
                    var longValue = (long)value;
                    problemDetails.Status = (int)longValue;
                    break;

                case "title":
                    problemDetails.Title = (string)value;
                    break;

                case "type":
                    problemDetails.Type = (string)value;
                    break;

                default:
                    problemDetails.Extensions.Add(key, serializer.Deserialize(reader));
                    break;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var problemDetails = (ProblemDetails)value;

            writer.WriteStartObject();
            WriteProperties(writer, problemDetails, serializer);
            writer.WriteEndObject();
        }

        protected static void WriteProperties(JsonWriter writer, ProblemDetails problemDetails, JsonSerializer serializer)
        {
            if (!string.IsNullOrEmpty(problemDetails.Type))
            {
                writer.WritePropertyName("type");
                writer.WriteValue(problemDetails.Type);
            }

            if (!string.IsNullOrEmpty(problemDetails.Title))
            {
                writer.WritePropertyName("title");
                writer.WriteValue(problemDetails.Title);
            }

            if (problemDetails.Status.HasValue)
            {
                writer.WritePropertyName("status");
                writer.WriteValue(problemDetails.Status);
            }

            if (!string.IsNullOrEmpty(problemDetails.Detail))
            {
                writer.WritePropertyName("detail");
                writer.WriteValue(problemDetails.Detail);
            }

            if (!string.IsNullOrEmpty(problemDetails.Instance))
            {
                writer.WritePropertyName("instance");
                writer.WriteValue(problemDetails.Instance);
            }

            foreach (var keyValuePair in problemDetails.Extensions)
            {
                writer.WritePropertyName(keyValuePair.Key);
                serializer.Serialize(writer, keyValuePair.Value);
            }
        }
    }
}
