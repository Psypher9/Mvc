// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation
{
    internal class RequiresValidationMetadataProvider : IValidationMetadataProvider
    {
        private readonly bool _usesDefaultValidatorProviders;

        public RequiresValidationMetadataProvider(IList<IModelValidatorProvider> modelValidatorProviders)
            : this(modelValidatorProviders.All(p => p is IDefaultModelValidatorProvider))
        {
        }

        public RequiresValidationMetadataProvider(bool usesDefaultValidatorProviders)
        {
            _usesDefaultValidatorProviders = usesDefaultValidatorProviders;
        }

        public void CreateValidationMetadata(ValidationMetadataProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (!_usesDefaultValidatorProviders)
            {
                return;
            }

            if (!context.ValidationMetadata.IsRequired != true &&
                context.ValidationMetadata.ValidatorMetadata.Count == 0 &&
                !typeof(IValidatableObject).IsAssignableFrom(context.Key.ModelType))
            {
                context.ValidationMetadata.IsValidationRequired = false;
            }
        }
    }
}
