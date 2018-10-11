// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.AspNetCore.Mvc.Formatters.Xml.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Mvc.Formatters.Xml
{
    /// <summary>
    /// A <see cref="IConfigureOptions{TOptions}"/> implementation which will add the
    /// XML serializer formatters to <see cref="MvcOptions"/>.
    /// </summary>
    internal sealed class XmlSerializerMvcOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of <see cref="XmlSerializerMvcOptionsSetup"/>.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/>.</param>
        public XmlSerializerMvcOptionsSetup(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Adds the XML serializer formatters to <see cref="MvcOptions"/>.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions"/>.</param>
        public void Configure(MvcOptions options)
        {
            // Do not override any user mapping
            var key = "xml";
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(key);
            if (string.IsNullOrEmpty(mapping))
            {
                options.FormatterMappings.SetMediaTypeMappingForFormat(
                    key,
                    MediaTypeHeaderValues.ApplicationXml);
            }

            var inputFormatter = new XmlSerializerInputFormatter(options);
            var outputFormatter = new XmlSerializerOutputFormatter(_loggerFactory);

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

            options.InputFormatters.Add(inputFormatter);
            options.OutputFormatters.Add(outputFormatter);
        }
    }
}
