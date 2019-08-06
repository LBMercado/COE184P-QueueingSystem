using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


internal class HashResult
{
    /// <summary>
    /// hashed string of text
    /// </summary>
    public string Digest { get; set; }

    public HashResult(string digest)
    {
        Digest = digest;
    }
}

internal class HashWithSaltResult
{
    /// <summary>
    /// hashed string of text
    /// </summary>
    public string Digest { get; private set; }
    public string Salt { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="digest">
    /// hashed string of text
    /// </param>
    /// <param name="salt"></param>
    public HashWithSaltResult(string digest, string salt)
    {
        Digest = digest;
        Salt = salt;
    }

    public void ChangeHashResult(string digest, string salt)
    {
        Digest = digest;
        Salt = salt;
    }
}

internal class RandNumGen
{
    public string GenerateRandomCryptographicKey(int keyLength)
    {
        return Convert.ToBase64String(GenerateRandomCryptographicBytes(keyLength));
    }

    public byte[] GenerateRandomCryptographicBytes(int keyLength)
    {
        RNGCryptoServiceProvider rngCryptoServiceProvider = new RNGCryptoServiceProvider();
        byte[] randomBytes = new byte[keyLength];
        rngCryptoServiceProvider.GetBytes(randomBytes);
        return randomBytes;
    }
}

internal class HasherWithSalt
{
    /// <summary>
    /// Hash a given string and generate a new salt given the salt length (typically 64),
    /// and hash algorithm(SHA256.Create() by default) 
    /// (see HashAlgorithm class for specific algos to use)
    /// </summary>
    /// <param name="password"></param>
    /// <param name="saltLength"></param>
    /// <param name="hashAlgo"></param>
    /// <returns></returns>
    static internal HashWithSaltResult HashWithSalt(string password, int saltLength = 64, HashAlgorithm hashAlgo = null)
    {
        if (hashAlgo == null)
            hashAlgo = SHA256.Create();
        RandNumGen rng = new RandNumGen();
        byte[] saltBytes = rng.GenerateRandomCryptographicBytes(saltLength);
        byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
        List<byte> passwordWithSaltBytes = new List<byte>();
        passwordWithSaltBytes.AddRange(passwordAsBytes);
        passwordWithSaltBytes.AddRange(saltBytes);
        byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
        return new HashWithSaltResult(Convert.ToBase64String(digestBytes), Convert.ToBase64String(saltBytes));
    }
    /// <summary>
    /// Hash a given string with a given salt,
    /// and hash algorithm(use SHA256.Create() if you do not know) 
    /// (see HashAlgorithm class for specific algos to use)
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <param name="hashAlgo"></param>
    /// <returns></returns>
    static internal HashWithSaltResult HashWithSalt(string password, string salt, HashAlgorithm hashAlgo)
    {
        byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = Convert.FromBase64String(salt);
        List<byte> passwordWithSaltBytes = new List<byte>();
        passwordWithSaltBytes.AddRange(passwordAsBytes);
        passwordWithSaltBytes.AddRange(saltBytes);
        byte[] digestBytes = hashAlgo.ComputeHash(passwordWithSaltBytes.ToArray());
        return new HashWithSaltResult(Convert.ToBase64String(digestBytes), salt);
    }


    /// <summary>
    /// Hash a given string given the hash algorithm(use SHA256.Create() if you do not know) (see HashAlgorithm class for specific algos to use)
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashAlgo"></param>
    /// <returns></returns>
    static public HashResult HashOnly(string password, HashAlgorithm hashAlgo)
    {
        byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
        byte[] digestBytes = hashAlgo.ComputeHash(passwordAsBytes);
        return new HashResult(Convert.ToBase64String(digestBytes));
    }
}

static internal class Hasher
{
    /// <summary>
    /// Hash a given string given the hash algorithm(use SHA512.Create() if you do not know) (see HashAlgorithm class for specific algos to use)
    /// </summary>
    /// <param name="password"></param>
    /// <param name="hashAlgo"></param>
    /// <returns></returns>
    static public HashResult HashSHA512(string password)
    {
        var hashAlgo = SHA512.Create();

        byte[] passwordAsBytes = Encoding.UTF8.GetBytes(password);
        byte[] digestBytes = hashAlgo.ComputeHash(passwordAsBytes);
        return new HashResult(Convert.ToBase64String(digestBytes));
    }
}