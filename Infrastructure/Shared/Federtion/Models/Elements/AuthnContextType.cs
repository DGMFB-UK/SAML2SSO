using System;
using System.Xml.Serialization;
using Kernel.Federation.Constants;

namespace Shared.Federtion.Models
{
    /// <summary>
    /// <c>AuthContext</c> type enumeration.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol, IncludeInSchema = false)]
    public enum AuthnContextType
    {
        /// <summary>
        /// <c>AuthnContextClassRef</c> type.
        /// </summary>
        [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextClassRef")]
        AuthnContextClassRef,

        /// <summary>
        /// <c>AuthnContextDeclRef</c> type.
        /// </summary>
        [XmlEnum("urn:oasis:names:tc:SAML:2.0:assertion:AuthnContextDeclRef")]
        AuthnContextDeclRef
    }
}