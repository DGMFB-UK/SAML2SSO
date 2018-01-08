﻿using System;
using Kernel.Validation;
using Shared.Federtion.Response;

namespace Federation.Protocols.Response.Validation
{
    internal class SamlResponseValidationContext : ValidationContext
    {
        public SamlResponseContext ResponseContext { get { return (SamlResponseContext)base.Entry; } }
        public SamlResponseValidationContext(SamlResponseContext entry) : this((object)entry)
        {
        }

        protected SamlResponseValidationContext(object entry) : base(entry)
        {
        }

        protected override object GetServiceInternal(Type serviceType)
        {
            return base.GetServiceInternal(serviceType);
        }
    }
}