using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Kernel.Cryptography.DataProtection;
using Kernel.Federation.Protocols;
using Kernel.Logging;
using Kernel.Serialisation;
using Serialisation.JSON;

namespace Federation.Protocols.RelayState
{
    internal class RelaystateSerialiser : IRelayStateSerialiser
    {
        private readonly IMessageEncoding _encoding;
        private readonly IJsonSerialiser _jsonSerialiser;
        private readonly ILogProvider _logProvider;
        public RelaystateSerialiser(IJsonSerialiser jsonSerialiser, IMessageEncoding encoding, ILogProvider logProvider)
        {
            this._jsonSerialiser = jsonSerialiser;
            this._encoding = encoding;
            this._logProvider = logProvider;
            this.DataProtector = new Kernel.Cryptography.DataProtection.DpapiDataProtector("SSO", "saml", "relaystate");
        }
        public IDataProtector DataProtector { private get; set; }
        public object[] Deserialize(Stream stream, IList<Type> messageTypes)
        {
            throw new NotImplementedException();
        }
        
        public object Deserialize(string data)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(string data)
        {
            throw new NotImplementedException();
        }

        public async Task<string> Serialize(object data)
        {
            var jsonString = this._jsonSerialiser.Serialize(data);
            this._logProvider.LogMessage(String.Format("Relay state serialised:\r\n{0}", jsonString));
            if (this.DataProtector != null)
            {
                var buffer = Encoding.UTF8.GetBytes(jsonString);
                var dataProtected = this.DataProtector.Protect(buffer);
                this._logProvider.LogMessage("Relay state data protected");
                return await this._encoding.EncodeMessage(dataProtected);
            }
            var encoded = await this._encoding.EncodeMessage(jsonString);
            return encoded;
        }

        public void Serialize(Stream stream, object[] o)
        {
            throw new NotImplementedException();
        }
        
        async Task<object> IRelayStateSerialiser.Deserialize(string data)
        {
            string decoded;
            if (this.DataProtector != null)
            {
                var buffer = await this._encoding.Decode(data);
                var dataProtected = this.DataProtector.Unprotect(buffer);
                decoded = Encoding.UTF8.GetString(dataProtected);
            }
            else
            {
                decoded = await this._encoding.DecodeMessage(data);
            }
            var deserialised = this._jsonSerialiser.Deserialize(decoded);
            return deserialised;
        }

        string ISerializer.Serialize(object o)
        {
            throw new NotImplementedException();
        }
    }
}