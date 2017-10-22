﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Owin.Security;

namespace Microsoft.Owin.CertificateValidators.Tests
{
    internal class Utility
    {
        internal static byte[] GetSubjectPublicKeyInfo(X509Certificate2 cert)
        {
            var minfo = typeof(CertificateSubjectPublicKeyInfoValidator).GetMethod("ExtractSpkiBlob", BindingFlags.Static | BindingFlags.NonPublic);
            var expPar = Expression.Parameter(typeof(X509Certificate2));
            var call = Expression.Call(minfo, expPar);
            var lambda = Expression.Lambda<Func<X509Certificate2, byte[]>>(call, expPar)
                .Compile();
            return lambda(cert);
        }

        internal static string HashSpki(byte[] data)
        {
            var hash = new SHA256CryptoServiceProvider();
            var hashed = hash.ComputeHash(data);
            return Convert.ToBase64String(hashed);
        }

        internal static void GetNextSequence(byte[] cert, ICollection<byte[]> sequences)
        {
            for(var j = 0; j < cert.Length; j++)
            {
                if (cert[j] != 0x30)
                {
                    continue;
                }
                var nn = cert.Skip(j).ToArray();
                var offset = 0;
                var inner = GetSequence(nn, sequences, out offset);
                
                GetNextSequence(inner, sequences);
                var source = nn.Skip(inner.Length + offset).ToArray();
                GetNextSequence(source, sequences);
                break;
            }
            
        }

        internal static byte[] GetSequence(byte[] import, ICollection<byte[]> seq, out int offset)
        {
            offset = 0;
            byte[] lenth = null;
            if ((import[1] & 0x80) == 0x80)
            {
                var foo = import[1] & 01111111;
                lenth = import.Skip(2).Take(foo).Reverse().ToArray();
                offset = +2 + foo;
            }
            else
            {
                lenth = import.Skip(1).Take(1).ToArray();
                offset = +2;
            }

            var i = (lenth.Length == 1) ? (short)lenth[0] : BitConverter.ToInt16(lenth, 0);

            var result = import.Skip(offset).Take(i).ToArray();
            var result1 = import.Take(i + offset).ToArray();
            seq.Add(result1);
            return result;
        }
    }
}