// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    /// A default <see cref="IModelValidatorProvider"/>.
    /// </summary>
    /// <remarks>
    /// The <see cref="DefaultModelValidatorProvider"/> provides validators from <see cref="IModelValidator"/>
    /// instances in <see cref="ModelBinding.ModelMetadata.ValidatorMetadata"/>.
    /// </remarks>
    internal sealed class DefaultModelValidatorProvider : IDefaultModelValidatorProvider
    {
        /// <inheritdoc />
        public void CreateValidators(ModelValidatorProviderContext context)
        {
            //Perf: Avoid allocations here
            for (var i = 0; i < context.Results.Count; i++)
            {
                var validatorItem = context.Results[i];

                // Don't overwrite anything that was done by a previous provider.
                if (validatorItem.Validator != null)
                {
                    continue;
                }

                if (validatorItem.ValidatorMetadata is IModelValidator validator)
                {
                    validatorItem.Validator = validator;
                    validatorItem.IsReusable = true;
                }
            }
        }
    }
}