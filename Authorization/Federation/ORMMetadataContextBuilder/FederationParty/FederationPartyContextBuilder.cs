﻿using System;
using System.Linq;
using Kernel.Cache;
using Kernel.Data.ORM;
using Kernel.Federation.FederationPartner;
using MemoryCacheProvider;
using ORMMetadataContextProvider.Models;

namespace ORMMetadataContextProvider.FederationParty
{
    internal class FederationPartyContextBuilder : IAssertionPartyContextBuilder
    {
        private readonly IDbContext _dbContext;
        private readonly ICacheProvider _cacheProvider;

        public FederationPartyContextBuilder(IDbContext dbContext, ICacheProvider cacheProvider)
        {
            this._dbContext = dbContext;
            this._cacheProvider = cacheProvider;
        }
        public FederationPartyConfiguration BuildContext(string federationPartyId)
        {
            if (this._cacheProvider.Contains(federationPartyId))
                return this._cacheProvider.Get<FederationPartyConfiguration>(federationPartyId);

            var federationPartyContext = this._dbContext.Set<FederationPartySettings>()
                .FirstOrDefault(x => x.FederationPartyId == federationPartyId);
            if (federationPartyContext == null)
                throw new InvalidOperationException(String.Format("No federation patty settings found for id: {0}", federationPartyId));

            var context = new FederationPartyConfiguration(federationPartyId, federationPartyContext.MetadataPath);
            var federationPartyAuthnRequestConfiguration = this.BuildFederationPartyAuthnRequestConfiguration(federationPartyContext.AutnRequestSettings, federationPartyContext.MetadataSettings.SPDescriptorSettings.EntityId);
            context.FederationPartyAuthnRequestConfiguration = federationPartyAuthnRequestConfiguration;
            
            context.RefreshInterval = MetadataHelper.TimeSpanFromDatapartEntry(federationPartyContext.RefreshInterval);
            context.AutomaticRefreshInterval = MetadataHelper.TimeSpanFromDatapartEntry(federationPartyContext.AutoRefreshInterval);
            this.BuildMetadataContext(context, federationPartyContext.MetadataSettings);
            object policy = new MemoryCacheItemPolicy();
            ((ICacheItemPolicy)policy).SlidingExpiration = TimeSpan.FromDays(1);
            this._cacheProvider.Put(federationPartyId, context,  (ICacheItemPolicy)policy);
            return context;
        }

        private void BuildMetadataContext(FederationPartyConfiguration federationPartyContext, MetadataSettings metadataSettings)
        {
            var metadataContextBuilder = new MetadataContextBuilder(this._dbContext, this._cacheProvider);
            var metadata = metadataContextBuilder.BuildFromDbSettings(metadataSettings);
            federationPartyContext.MetadataContext = metadata;
        }

        private FederationPartyAuthnRequestConfiguration BuildFederationPartyAuthnRequestConfiguration(AutnRequestSettings autnRequestSettings, string entityId)
        {
            if (autnRequestSettings == null)
                throw new ArgumentNullException("autnRequestSettings");
            
            RequestedAuthnContextConfiguration requestedAuthnContextConfiguration = null;
            if (autnRequestSettings.RequitedAutnContext != null)
            {
                requestedAuthnContextConfiguration = new RequestedAuthnContextConfiguration(autnRequestSettings.RequitedAutnContext.Comparison.ToString());
                autnRequestSettings.RequitedAutnContext.RequitedAuthnContexts.Aggregate(requestedAuthnContextConfiguration.RequestedAuthnContexts, (t, next) =>
                {
                    t.Add(new Kernel.Federation.Protocols.AuthnContext(next.RefType.ToString(), new Uri(next.Value)));
                    return t;
                });
            }
            if (autnRequestSettings.NameIdConfiguration == null)
                throw new ArgumentNullException("nameIdConfiguration");

            var defaultNameId = new DefaultNameId(new Uri(autnRequestSettings.NameIdConfiguration.DefaultNameIdFormat.Uri))
            {
                AllowCreate = autnRequestSettings.NameIdConfiguration.AllowCreate,
                EncryptNameId = autnRequestSettings.NameIdConfiguration.EncryptNameId
            };
            var scopingConfiguration = autnRequestSettings.Scoping == null ? new ScopingConfiguration()
                    : new ScopingConfiguration(entityId) { PoxyCount = autnRequestSettings.Scoping.MaxProxyCount };

            var configuration = new FederationPartyAuthnRequestConfiguration(requestedAuthnContextConfiguration, defaultNameId, scopingConfiguration)
            {
                ForceAuthn = autnRequestSettings.ForceAuthn,
                IsPassive = autnRequestSettings.IsPassive,
                Version = autnRequestSettings.Version ?? "2.0"
            };
            return configuration;
        }
        public void Dispose()
        {
            if(this._dbContext != null)
                this._dbContext.Dispose();
        }
    }
}