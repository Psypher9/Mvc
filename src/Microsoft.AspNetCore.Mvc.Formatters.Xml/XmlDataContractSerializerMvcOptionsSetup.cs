// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Formatters.Xml.Internal;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Formatters.Xml
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> implementation which will add the
    /// data contract serializer formatters to <see cref="MvcOptions"/>.
    /// </summary>
    internal sealed class XmlDataContractSerializerMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlDataContractSerializerMvcOptionsSetup"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public XmlDataContractSerializerMvcOptionsSetup(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Adds the data contract serializer formatters to <see cref="MvcOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        public void Configure(MvcOptions options)
        {
            var outputFormatter = new XmlDataContractSerializerOutputFormatter(_loggerFactory);
            var inputFormatter = new XmlDataContractSerializerInputFormatter(options);

            options.OnAfterPostConfigure += option =>
            {
                if (options.AllowRfc7807CompliantProblemDetailsFormat)
                {
                    var problemDetailsFactories = WrapperProviderFactory.ProblemDetailsFactories;
                    foreach (var factory in problemDetailsFactories)
                    {
                        inputFormatter.WrapperProviderFactories.Add(factory);
                        outputFormatter.WrapperProviderFactories.Add(factory);
                    }
                }
                else
                {
                    var problemDetailsFactories = WrapperProviderFactory.ProblemDetails21Factories;
                    foreach (var factory in problemDetailsFactories)
                    {
                        inputFormatter.WrapperProviderFactories.Add(factory);
                        outputFormatter.WrapperProviderFactories.Add(factory);
                    }
                }
            };

            options.ModelMetadataDetailsProviders.Add(new DataMemberRequiredBindingMetadataProvider());

            options.OutputFormatters.Add(outputFormatter);
            options.InputFormatters.Add(inputFormatter);

            // Do not override any user mapping
            var key = "xml";
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
            if (string.IsNullOrEmpty(mapping))
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    key,
                    MediaTypeHeaderValues.ApplicationXml);
            }

            options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider("System.Xml.Linq.XObject"));
            options.ModelMetadataDetailsProviders.Add(new SuppressChildValidationMetadataProvider("System.Xml.XmlNode"));
        }
    }
}
