using System;
using System.IO;
using System.Security.Cryptography;

namespace ConsoleApp1
{
    class Program
    {


        public static void DecryptFile(string fileToDecrypt, string destinitionFilename, byte[] key)
        {
            //fileToDncrypt - Plik do odszyfrowania
            using (var sourceStream = File.OpenRead(fileToDecrypt))
            //destinitionFilename -lokacja i nazwa pod jaką ma być zapisany odszyfrowany
            using (var destinationStream = File.Create(destinitionFilename))
            //definicja metody szyfrowania
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
            //opcjonalne kasowanie pliku źródłowego
            File.Delete(fileToDecrypt);
        }

        public static void EncryptFile(string fileToEncrypt, string destinitionFile)
        {
            //fileToEncrypt plik do zaszyfrowania
            using (var sourceStream = File.OpenRead(fileToEncrypt))
            //destinitionFile - nazwa i lokalizacja gdzie zapisać plik zaszyfrowany
            using (var destinationStream = File.Create(destinitionFile))
            //definicja metod szyfrowania
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
                    //Zapisanie klucza szyfrowania do pliku
                    File.WriteAllText("key.txt", Convert.ToBase64String(provider.Key));

                }

            }
            //opcjonalne kasowanie pliku źróddłowego
            File.Delete(fileToEncrypt);
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
