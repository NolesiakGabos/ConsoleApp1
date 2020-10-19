using System;
using System.IO;
using System.Security.Cryptography;

namespace ConsoleApp1
{
    class Program
    {


        public static void DecryptFile(string fileToEncrypt, string destinitionFilename, byte[] key)
        {
            using (var sourceStream = File.OpenRead(fileToEncrypt))
            using (var destinationStream = File.Create(destinitionFilename))
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.BlockSize = 128;
                provider.KeySize = 256;
                provider.GenerateIV();
                provider.GenerateKey();
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                var IV = new byte[provider.IV.Length];
                sourceStream.Read(IV, 0, IV.Length);
                using (var cryptoTransform = provider.CreateDecryptor(key, IV))
                using (var cryptoStream = new CryptoStream(sourceStream, cryptoTransform, CryptoStreamMode.Read))
                {
                    cryptoStream.CopyTo(destinationStream);
                }

            }
            File.Delete(fileToEncrypt);
        }

        public static void EncryptFile(string fileToDecrypt, string destinitionFile)
        {
            using (var sourceStream = File.OpenRead(fileToDecrypt))
            using (var destinationStream = File.Create(destinitionFile))
            using (var provider = new AesCryptoServiceProvider())
            {
                provider.BlockSize = 128;
                provider.KeySize = 256;
                provider.GenerateIV();
                provider.GenerateKey();
                provider.Mode = CipherMode.CBC;
                provider.Padding = PaddingMode.PKCS7;
                using (var cryptoTransform = provider.CreateEncryptor())
                using (var cryptoStream = new CryptoStream(destinationStream, cryptoTransform, CryptoStreamMode.Write))
                {
                    destinationStream.Write(provider.IV, 0, provider.IV.Length);
                    sourceStream.CopyTo(cryptoStream);
                    Console.WriteLine(System.Convert.ToBase64String(provider.Key));
                    File.WriteAllText("key.txt", Convert.ToBase64String(provider.Key));

                }

            }
            File.Delete(fileToDecrypt);
        }



        static void Main(string[] args)
        {

            Console.WriteLine("Podaj nazwe pliku");
            var fileName = Console.ReadLine();
            Console.WriteLine("Podaj nazwe zaszyfrowanego pliku");
            var destinitionFilename = Console.ReadLine();
            if (File.Exists("key.txt") == true)
            {
                var key = Convert.FromBase64String(File.ReadAllText("key.txt"));

                DecryptFile(fileName, destinitionFilename, key);
            }
            else
            {
                EncryptFile(fileName, destinitionFilename);

            }





            Console.WriteLine("End of program....");
            Console.ReadLine();
        }
    }
}
