// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.Formatters.Json
{
    public class ProblemDetailsConverterTest
    {
        private const string ProblemDetailsJson =
@"{
  ""type"": ""https://example.com/probs/out-of-credit"",
  ""title"": ""You do not have enough credit."",
  ""status"": 400,
  ""detail"": ""Your current balance is 30, but that costs 50."",
  ""instance"": ""/account/12345/msgs/abc"",
  ""balance"": 30,
  ""accounts"": [
    ""/account/12345"",
    ""/account/67890""
  ]
}";

        [Fact]
        public void ReadJson_ReturnsNull_IfReaderIsAtNullToken()
        {
            // Arrange
            var reader = new StreamReader(new MemoryStream());
            var jsonReader = new JsonTextReader(reader);
            var converter = new ProblemDetailsConverter();

            // Act
            jsonReader.Read();
            var result = converter.ReadJson(jsonReader, typeof(ProblemDetails), null, new JsonSerializer());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ReadJson_ReturnsValue()
        {
            // Arrange
            var content = Encoding.UTF8.GetBytes(ProblemDetailsJson);
            var reader = new StreamReader(new MemoryStream(content));
            var jsonReader = new JsonTextReader(reader);
            var converter = new ProblemDetailsConverter();

            // Act
            jsonReader.Read();
            var result = converter.ReadJson(jsonReader, typeof(ProblemDetails), null, new JsonSerializer());

            // Assert
            Assert.NotNull(result);
            var problemDetails = Assert.IsType<ProblemDetails>(result);
            Assert.Equal("https://example.com/probs/out-of-credit", problemDetails.Type);
            Assert.Equal("You do not have enough credit.", problemDetails.Title);
            Assert.Equal(400, problemDetails.Status);
            Assert.Equal("Your current balance is 30, but that costs 50.", problemDetails.Detail);
            Assert.Equal("/account/12345/msgs/abc", problemDetails.Instance);
            Assert.Equal(30L, problemDetails.Extensions["balance"]);
            var accounts = Assert.IsType<JArray>(problemDetails.Extensions["accounts"]);
            Assert.Equal(new[] { "/account/12345", "/account/67890" }, accounts.Values<string>());
        }

        [Fact]
        public void WriteJson_WritesEmpty_IfValueIsNull()
        {
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(writer);
            var converter = new ProblemDetailsConverter();

            // Act
            converter.WriteJson(jsonWriter, null, new JsonSerializer());
            jsonWriter.Flush();

            // Assert
            var content = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal("null", content);
        }

        [Fact]
        public void WriteJson_WritesValue()
        {
            // Arrange
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            var jsonWriter = new JsonTextWriter(writer)
            {
                Formatting = Formatting.Indented,
            };
            var converter = new ProblemDetailsConverter();
            var problemDetails = new ProblemDetails
            {
                Type = "https://example.com/probs/out-of-credit",
                Title = "You do not have enough credit.",
                Status = 400,
                Detail = "Your current balance is 30, but that costs 50.",
                Instance = "/account/12345/msgs/abc",
                Extensions =
                {
                    ["balance"] = 30,
                    ["accounts"] = new[] { "/account/12345", "/account/67890" },
                }
            };

            // Act
            converter.WriteJson(jsonWriter, problemDetails, new JsonSerializer());
            jsonWriter.Flush();

            // Assert
            var content = Encoding.UTF8.GetString(stream.ToArray());
            Assert.Equal(ProblemDetailsJson, content);
        }
    }
}
