// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Microsoft.AspNetCore.Mvc.Performance
{
    public class ValidationVisitorModelWithValidatedProperties : ValidationVisitorBenchmarkBase
    {
        private class Person
        {
            [Required]
            public int Id { get; set; }

            [Required]
            [StringLength(20)]
            public string Name { get; set; }

            public string Description { get; set; }
        }

        public override object Model { get; } = new Person
        {
            Id = 10,
            Name = "Test",
        };

        [Benchmark(Baseline = true, Description = "validation for a model with some validated properties - baseline", OperationsPerInvoke = Iterations)]
        public void Visit_TypeWithSomeValidatedProperties_Baseline()
        {
            var validationVisitor = new ValidationVisitor(
                ActionContext,
                CompositeModelValidatorProvider,
                ValidatorCache,
                BaselineModelMetadataProvider,
                new ValidationStateDictionary());

            validationVisitor.Validate(BaselineModelMetadata, "key", Model);
        }

        [Benchmark(Description = "validation for a model with some validated properties", OperationsPerInvoke = Iterations)]
        public void Visit_TypeWithSomeValidatedProperties()
        {
            var validationVisitor = new ValidationVisitor(
                ActionContext,
                CompositeModelValidatorProvider,
                ValidatorCache,
                ModelMetadataProvider,
                new ValidationStateDictionary());

            validationVisitor.Validate(ModelMetadata, "key", Model);
        }
    }
}
