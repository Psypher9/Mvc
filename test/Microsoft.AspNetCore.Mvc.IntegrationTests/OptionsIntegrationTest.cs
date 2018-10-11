// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Formatters.Json;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Xunit;

namespace Microsoft.AspNetCore.Mvc.IntegrationTests
{
    public class OptionsIntegrationTest
    {
        [Fact]
        public void CompatibilityVersionLatest_AddsProblemDetailsConverters()
        {
            // Arrange
            var serviceProvider = GetServiceCollection()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .BuildServiceProvider();

            // Act
            var options = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
            var inputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().First();
            var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().First();

            Assert.Collection(
                inputFormatter.SerializerSettings.Converters,
                c => Assert.IsType<ProblemDetailsConverter>(c),
                c => Assert.IsType<ValidationProblemDetailsConverter>(c));

            Assert.Collection(
                outputFormatter.SerializerSettings.Converters,
                c => Assert.IsType<ProblemDetailsConverter>(c),
                c => Assert.IsType<ValidationProblemDetailsConverter>(c));
        }

        [Fact]
        public void CompatibilityVersion21_DoesNotAddProblemDetailsConverters()
        {
            // Arrange
            var serviceProvider = GetServiceCollection()
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .Services
                .BuildServiceProvider();

            // Act
            var options = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;
            var inputFormatter = options.InputFormatters.OfType<JsonInputFormatter>().First();
            var outputFormatter = options.OutputFormatters.OfType<JsonOutputFormatter>().First();

            // Assert
            Assert.Empty(inputFormatter.SerializerSettings.Converters);
            Assert.Empty(outputFormatter.SerializerSettings.Converters);
        }

        [Fact]
        public void CompatibilityVersionLatest_SetsUpWrapperProviderFactories_ForXmlSerializers()
        {
            // Arrange
            var serviceProvider = GetServiceCollection()
                .AddMvc()
                .AddXmlSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Latest)
                .Services
                .BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

            // Assert
            var inputFormatter = options.InputFormatters.OfType<XmlSerializerInputFormatter>().First();
            var outputFormatter = options.InputFormatters.OfType<XmlSerializerInputFormatter>().First();

            Assert.Collection(
                inputFormatter.WrapperProviderFactories,
                f => Assert.IsType<SerializableErrorWrapperProviderFactory>(f),
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ProblemDetailsWrapper), factory.WrappingType);
                },
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ValidationProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ValidationProblemDetailsWrapper), factory.WrappingType);
                });

            Assert.Collection(
                outputFormatter.WrapperProviderFactories,
                f => Assert.IsType<SerializableErrorWrapperProviderFactory>(f),
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ProblemDetailsWrapper), factory.WrappingType);
                },
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ValidationProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ValidationProblemDetailsWrapper), factory.WrappingType);
                });
        }

#pragma warning disable CS0618 // Type or member is obsolete
        [Fact]
        public void CompatibilityVersion21_SetsUp21WrapperProviderFactories_ForXmlSerializers()
        {
            // Arrange
            var serviceProvider = GetServiceCollection()
                .AddMvc()
                .AddXmlSerializerFormatters()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .Services
                .BuildServiceProvider();

            var options = serviceProvider.GetRequiredService<IOptions<MvcOptions>>().Value;

            // Assert
            var inputFormatter = options.InputFormatters.OfType<XmlSerializerInputFormatter>().First();
            var outputFormatter = options.InputFormatters.OfType<XmlSerializerInputFormatter>().First();

            Assert.Collection(
                inputFormatter.WrapperProviderFactories,
                f => Assert.IsType<SerializableErrorWrapperProviderFactory>(f),
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ProblemDetails21Wrapper), factory.WrappingType);
                },
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ValidationProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ValidationProblemDetails21Wrapper), factory.WrappingType);
                });

            Assert.Collection(
                outputFormatter.WrapperProviderFactories,
                f => Assert.IsType<SerializableErrorWrapperProviderFactory>(f),
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ProblemDetails21Wrapper), factory.WrappingType);
                },
                f =>
                {
                    var factory = Assert.IsType<WrapperProviderFactory>(f);
                    Assert.Equal(typeof(ValidationProblemDetails), factory.DeclaredType);
                    Assert.Equal(typeof(ValidationProblemDetails21Wrapper), factory.WrappingType);
                });
        }
#pragma warning restore CS0618 // Type or member is obsolete

        private static IServiceCollection GetServiceCollection()
        {
            return new ServiceCollection()
                .AddLogging()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
        }
    }
}
