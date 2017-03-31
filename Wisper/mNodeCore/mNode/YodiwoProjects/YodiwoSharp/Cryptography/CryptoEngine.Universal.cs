using System;
using System.Collections.Generic;
using System.Text;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace Yodiwo.Cryptography
{
    public static class CryptoEngine
    {
        //Source : https://msdn.microsoft.com/en-us/library/windows/apps/windows.security.cryptography.core.symmetrickeyalgorithmprovider.aspx

        static readonly string DefaultAlgorithm = SymmetricAlgorithmNames.AesCbcPkcs7;
        //static readonly string DefaultAlgorithm = SymmetricAlgorithmNames.AesEcbPkcs7;


        public static CryptographicKey CreateKey(String strPassword,
                                                 byte[] strSalt,
                                                 UInt32 iterationCount = 10000,
                                                 String AlgorithmName = null)
        {
            //declares
            var strAlgName = AlgorithmName != null ? AlgorithmName : DefaultAlgorithm;
            var keygenAlgorithmName = KeyDerivationAlgorithmNames.Pbkdf2Sha512;
            uint keySize = 64; //must match algo

            //sanity checks
            if (strSalt == null)
                strSalt = new byte[keySize];
            if (strSalt.Length != keySize)
                Array.Resize(ref strSalt, (int)keySize);

            // Open the specified algorithm.
            var objKdfProv = KeyDerivationAlgorithmProvider.OpenAlgorithm(keygenAlgorithmName);

            // Create a buffer that contains the secret used during derivation.
            var buffSecret = CryptographicBuffer.ConvertStringToBinary(strPassword, BinaryStringEncoding.Utf8);

            // Create a random salt value.            
            var buffSalt = CryptographicBuffer.CreateFromByteArray(strSalt);

            // Create the derivation parameters.
            var pbkdf2Params = KeyDerivationParameters.BuildForPbkdf2(buffSalt, iterationCount);

            // Create a key from the secret value.
            var keyOriginal = objKdfProv.CreateKey(buffSecret);

            // Derive a key based on the original key and the derivation parameters.
            var keyMaterial = CryptographicEngine.DeriveKeyMaterial(keyOriginal,
                                                                    pbkdf2Params,
                                                                    keySize);

            // Demonstrate checking the iteration count.
            var iterationCountOut = pbkdf2Params.IterationCount;

            // Create a key by using the KDF parameters.
            var objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(strAlgName);
            var key = objAlg.CreateSymmetricKey(keyMaterial);

            //return key
            return key;
        }


        public static IBuffer EncryptString(String strMsg,
                                            CryptographicKey key,
                                            out IBuffer iv,
                                            String AlgorithmName = null)
        {
            // Create a buffer that contains the encoded message to be encrypted. 
            var buffMsg = CryptographicBuffer.ConvertStringToBinary(strMsg, BinaryStringEncoding.Utf8);
            //encrypt
            return Encrypt(buffMsg, key, out iv, AlgorithmName: AlgorithmName);
        }

        public static IBuffer Encrypt(IBuffer data,
                                            CryptographicKey key,
                                            out IBuffer iv,
                                            String AlgorithmName = null)
        {
            //declares
            var strAlgName = AlgorithmName != null ? AlgorithmName : DefaultAlgorithm;
            iv = null;

            // Open a symmetric algorithm provider for the specified algorithm. 
            var objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Determine whether the message length is a multiple of the block length.
            // This is not necessary for PKCS #7 algorithms which automatically pad the
            // message to an appropriate length.
            if (!strAlgName.Contains("PKCS7"))
            {
                if ((data.Length % objAlg.BlockLength) != 0)
                    throw new Exception("Message buffer length must be multiple of block length.");
            }

            // CBC algorithms require an initialization vector. Here, a random
            // number is used for the vector.
            if (strAlgName.Contains("CBC"))
                iv = CryptographicBuffer.GenerateRandom(objAlg.BlockLength);

            // Encrypt the data and return.
            return CryptographicEngine.Encrypt(key, data, iv);
        }



        public static string DecryptString(IBuffer encBuffer,
                                            CryptographicKey key,
                                            IBuffer iv,
                                            String AlgorithmName = null)
        {
            //decrypt buffer
            var buffDecrypted = Decrypt(encBuffer, key, iv, AlgorithmName: AlgorithmName);
            // Convert the decrypted buffer to a string.
            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, buffDecrypted);
        }

        public static IBuffer Decrypt(IBuffer encBuffer,
                                        CryptographicKey key,
                                        IBuffer iv,
                                        String AlgorithmName = null)
        {
            //declares
            var strAlgName = AlgorithmName != null ? AlgorithmName : DefaultAlgorithm;

            // Open an symmetric algorithm provider for the specified algorithm. 
            var objAlg = SymmetricKeyAlgorithmProvider.OpenAlgorithm(strAlgName);

            // Decrypt data
            return CryptographicEngine.Decrypt(key, encBuffer, iv);
        }

        public static byte[] ComputeHash(byte[] data)
        {
            var hap = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hap.CreateHash();
            hash.Append(data.AsBuffer());
            return hash.GetValueAndReset().ToArray();
        }

        public static byte[] ComputeHash(IBuffer data)
        {
            var hap = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hap.CreateHash();
            hash.Append(data);
            return hash.GetValueAndReset().ToArray();
        }

        public static byte[] ComputeHash(string data)
        {
            var dataBuffer = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);
            return ComputeHash(dataBuffer);
        }

    }
}
