// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.AspNetCore.Mvc.Formatters.Xml
{
    internal class WrapperProviderFactory : IWrapperProviderFactory
    {
        internal static WrapperProviderFactory[] ProblemDetailsFactories = new[]
        {
            new WrapperProviderFactory(
                typeof(ProblemDetails),
                typeof(ProblemDetailsWrapper),
                value => new ProblemDetailsWrapper((ProblemDetails)value)),

            new WrapperProviderFactory(
                typeof(ValidationProblemDetails),
                typeof(ValidationProblemDetailsWrapper),
                value => new ValidationProblemDetailsWrapper((ValidationProblemDetails)value)),
        };

#pragma warning disable CS0618 // Type or member is obsolete
        internal static WrapperProviderFactory[] ProblemDetails21Factories = new[]
        {
            new WrapperProviderFactory(
                typeof(ProblemDetails),
                typeof(ProblemDetails21Wrapper),
                value => new ProblemDetails21Wrapper((ProblemDetails)value)),

            new WrapperProviderFactory(
                typeof(ValidationProblemDetails),
                typeof(ValidationProblemDetails21Wrapper),
                value => new ValidationProblemDetails21Wrapper((ValidationProblemDetails)value)),
        };
#pragma warning restore CS0618 // Type or member is obsolete

        public WrapperProviderFactory(Type declaredType, Type wrappingType, Func<object, object> wrapper)
        {
            DeclaredType = declaredType;
            WrappingType = wrappingType;
            Wrapper = wrapper;
        }

        public Type DeclaredType { get; }

        public Type WrappingType { get; }

        public Func<object, object> Wrapper { get; }

        public IWrapperProvider GetProvider(WrapperProviderContext context)
        {
            if (context.DeclaredType == DeclaredType)
            {
                return new WrapperProvider(this);
            }

            return null;
        }

        private class WrapperProvider : IWrapperProvider
        {
            private readonly WrapperProviderFactory _wrapperFactory;

            public WrapperProvider(WrapperProviderFactory wrapperFactory)
            {
                _wrapperFactory = wrapperFactory;
            }

            public Type WrappingType => _wrapperFactory.WrappingType;

            public object Wrap(object original)
            {
                return _wrapperFactory.Wrapper(original);
            }
        }
    }
}