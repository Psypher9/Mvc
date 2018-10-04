// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace Microsoft.AspNetCore.Mvc.ModelBinding.Validation
{
    /// <summary>
    /// An <see cref="IModelValidatorProvider" /> that is implemented by MVC.
    /// Used to denote that the provider behaves in predictable ways such as relying on <see cref="ModelMetadata.ValidatorMetadata"/> to
    /// construct validators.
    /// </summary>
    internal interface IDefaultModelValidatorProvider : IModelValidatorProvider
    {
    }
}
