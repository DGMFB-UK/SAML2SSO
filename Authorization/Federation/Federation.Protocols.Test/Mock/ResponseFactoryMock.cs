﻿using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;
using Serialisation.Xml;
using Shared.Federtion.Constants;
using Shared.Federtion.Models;
using Shared.Federtion.Response;

namespace Federation.Protocols.Test.Mock
{
    internal class ResponseFactoryMock
    {
        public static TokenResponse GetTokenResponseSuccess(string inResponseTo, string statusCode)
        {
            var response = new TokenResponse
            {
                ID = Guid.NewGuid().ToString(),
                Destination = "http://localhost:59611/",
                IssueInstant = DateTime.UtcNow,
                InResponseTo = inResponseTo,
                Status = ResponseFactoryMock.BuildStatus(statusCode, null),
                Issuer = new NameId { Value = "https://dg-mfb/idp/shibboleth", Format = NameIdentifierFormats.Entity }
            };
            var assertion = AssertionFactroryMock.BuildAssertion();
            var token = AssertionFactroryMock.GetToken(assertion);
            var assertionElement = AssertionFactroryMock.SerialiseToken(token);
            response.Assertions = new XmlElement[] { assertionElement };
            return response;
        }

        public static Status BuildStatus(string code, string message = null)
        {
            return new Status
            {
                StatusMessage = message,
                StatusCode = ResponseFactoryMock.GetStatusCode(code, null)
            };
        }

        public static StatusDetail BuildStatuseDetail(ICollection<XmlElement> details)
        {
            return new StatusDetail
            {
                Any = details.ToArray()
            };
        }

        public static StatusCode GetStatusCode(string code, StatusCode parent)
        {
            var statusCode = new StatusCode
            {
                Value = code
            };
            if (parent != null)
            {
                parent.SubStatusCode = statusCode;
                return parent;
            }
            return statusCode;
        }

        public static string Serialize(object o)
        {
            var xmlSerialiser = new XMLSerialiser();
            xmlSerialiser.XmlNamespaces.Add("samlp", Saml20Constants.Protocol);
            xmlSerialiser.XmlNamespaces.Add("saml", Saml20Constants.Assertion);

            using (var ms = new MemoryStream())
            {
                xmlSerialiser.Serialize(ms, new[] { o });
                ms.Position = 0;
                var streamReader = new StreamReader(ms);
                var xmlString = streamReader.ReadToEnd();
                return xmlString;
            }
        }
    }
}