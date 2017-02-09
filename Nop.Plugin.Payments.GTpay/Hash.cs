using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


public class Hash
{
    public Hash() { }

    public enum HashType : int
    {
        MD5,
        SHA1,
        SHA256,
        SHA512
    }

    public static string GetHashMini(string text, HashType hashType)
    {
        SHA512 shaM = new SHA512Managed();
        byte[] hash = shaM.ComputeHash(Encoding.ASCII.GetBytes(Regex.Replace(text, @"[^\x20-\x7F]", "")));

        StringBuilder stringBuilder = new StringBuilder();
        foreach (byte b in hash)
        {
            stringBuilder.AppendFormat("{0:x2}", b);
        }
        return stringBuilder.ToString();
    }

    public static string CreateSHA256Signature(string secretkey, string stringtohash)
    {
        // Hex Decode the Secure Secret for use in using the HMACSHA256 hasher
        // hex decoding eliminates this source of error as it is independent of the character encoding
        // hex decoding is precise in converting to a byte array and is the preferred form for representing binary values as hex strings. 
        byte[] convertedHash = new byte[secretkey.Length / 2];
        for (int i = 0; i < secretkey.Length / 2; i++)
        {
            convertedHash[i] = (byte)Int32.Parse(secretkey.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
        }

        // Create secureHash on string
        string hexHash = "";
        using (HMACSHA256 hasher = new HMACSHA256(convertedHash))
        {
            byte[] hashValue = hasher.ComputeHash(Encoding.UTF8.GetBytes(stringtohash.ToString()));
            foreach (byte b in hashValue)
            {
                hexHash += b.ToString("X2");
            }
        }
        return hexHash;
    }


    public static string GetHash(string vsalt)
    {
        SHA256 sha256 = SHA256.Create();
        byte[] result = sha256.ComputeHash(Encoding.Default.GetBytes(vsalt));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            sb.Append(result[i].ToString("X2"));
        }
        return sb.ToString().ToUpper();
    }

}


    
    
    
    
    
    

