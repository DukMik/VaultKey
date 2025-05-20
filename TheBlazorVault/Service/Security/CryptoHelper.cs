using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

class CryptoHelper
{
    /// <summary>
    /// Chiffre un texte clair (string) et retourne le résultat chiffré (byte[]).
    /// </summary>
    public static byte[] Chiffrer(string texteClair, byte[] key, byte[] iv)
    {
        byte[] texteClairBytes = Encoding.UTF8.GetBytes(texteClair);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using MemoryStream ms = new();
        using CryptoStream cs = new(ms, encryptor, CryptoStreamMode.Write);
        {
            cs.Write(texteClairBytes, 0, texteClairBytes.Length);
            cs.FlushFinalBlock();
            return ms.ToArray();
        }
    }

    /// <summary>
    /// Déchiffre les données chiffrées (byte[]) et retourne le texte déchiffré (string UTF-8).
    /// </summary>
    public static string Dechiffrer(byte[] texteChiffre, byte[] key, byte[] iv)
    {
        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using MemoryStream ms = new(texteChiffre);
        using CryptoStream cs = new(ms, decryptor, CryptoStreamMode.Read);
        using StreamReader sr = new(cs, Encoding.UTF8);
        {
            return sr.ReadToEnd();
        }
    }

    public static class AesHelper
    {
        // Génère un tableau de bytes aléatoire de la taille spécifiée (par exemple 16 pour IV, 32 pour une clé AES-256)
        public static byte[] GenerateRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}