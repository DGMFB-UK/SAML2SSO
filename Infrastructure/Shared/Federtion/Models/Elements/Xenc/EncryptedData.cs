using System;
using System.Xml.Serialization;
using Shared.Federtion.Constants;

namespace Shared.Federtion.Models
{
    /// <summary>
    /// Encrypted data class
    /// </summary>
    [Serializable]
    [XmlType(Namespace = Saml20Constants.Xenc)]
    [XmlRoot(ElementName, Namespace = Saml20Constants.Xenc, IsNullable = false)]
    public class EncryptedData : Encrypted
    {
        /// <summary>
        /// The XML Element name of this class
        /// </summary>
        public const string ElementName = "EncryptedData";
    }
}