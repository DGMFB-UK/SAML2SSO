﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Shared.Federtion.Constants;

namespace Federation.Protocols.Response.Validation.ValidationRules
{
    internal class IssuerKnownRule : ResponseValidationRule
    {
        private readonly IdpInitDiscoveryService _service;

        public IssuerKnownRule(IdpInitDiscoveryService service, ILogProvider logProvider)
            : base(logProvider)
        {
            this._service = service;
        }

        internal override RuleScope Scope
        {
            get
            {
                return RuleScope.IdpInitiated;
            }
        }

        protected override async Task<bool> ValidateInternal(SamlResponseValidationContext context)
        {
            try
            {

                var federationParnerId = this._service.ResolveParnerId(context.Response);
                if (String.IsNullOrWhiteSpace(federationParnerId))
                    throw new InvalidOperationException(String.Format("Unsolicited Web SSO initiated by unknow issuer. Issuer: {0}", context.Response.Issuer));

                context.Response.RelayState = new Dictionary<string, object> { { RelayStateContstants.FederationPartyId, federationParnerId } };

                return true;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}