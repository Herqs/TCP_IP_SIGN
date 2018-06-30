using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TCPIPCLIENT
{
    static class RSACLASS
    {

        public static byte[] Sign(byte[] data, RSAParameters key)
        {
            RSACryptoServiceProvider rsacl = new RSACryptoServiceProvider();
            rsacl.ImportParameters(key);
            return rsacl.SignData(data, CryptoConfig.MapNameToOID("SHA512"));
        }

        public static bool verify(byte[] data, string xmlstr, byte[] signed)
        {
            RSACryptoServiceProvider rsacl = new RSACryptoServiceProvider();
            rsacl.FromXmlString(xmlstr);

            return rsacl.VerifyData(data, CryptoConfig.MapNameToOID("SHA512"), signed);
        }

        public static RSAParameters GenerateKeys()
        {
            RSACryptoServiceProvider rsacl = new RSACryptoServiceProvider();
            return rsacl.ExportParameters(true);
        }

        public static RSAParameters exportpublickey(RSAParameters key)
        {
            RSACryptoServiceProvider rsacl = new RSACryptoServiceProvider();
            rsacl.ImportParameters(key);
            return rsacl.ExportParameters(false);
        }
    }
}
