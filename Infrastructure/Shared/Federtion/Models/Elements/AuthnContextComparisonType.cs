using System;
using System.Xml.Serialization;
using Kernel.Federation.Constants;

namespace Shared.Federtion.Models
{
    /// <summary>
    /// <c>AuthContext</c> comparison type enumeration.
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Protocol)]
    public enum AuthnContextComparisonType
    {
        /// <summary>
        /// Exact comparison type.
        /// </summary>
        [XmlEnum("exact")]
        Exact,

        /// <summary>
        /// Minimum comparison type.
        /// </summary>
        [XmlEnum("minimum")]
        Minimum,

        /// <summary>
        /// Maximum comparison type.
        /// </summary>
        [XmlEnum("maximum")]
        Maximum,

        /// <summary>
        /// Better comparison type.
        /// </summary>
        [XmlEnum("better")]
        Better
    }
}