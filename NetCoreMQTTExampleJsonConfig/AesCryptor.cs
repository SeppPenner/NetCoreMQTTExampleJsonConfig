// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AesCryptor.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   A service to encrypt files and decrypt file data to a string.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Text;

namespace NetCoreMQTTExampleJsonConfig
{
    using Crypt = SharpAESCrypt.SharpAESCrypt;

    /// <inheritdoc cref="IAesCryptor" />
    /// <summary>
    ///     A service to encrypt files and decrypt file data to a string.
    /// </summary>
    /// <seealso cref="IAesCryptor" />
    public class AesCryptor : IAesCryptor
    {
        /// <inheritdoc cref="IDisposable" />
        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <seealso cref="IDisposable" />
        public void Dispose()
        {
        }

        /// <inheritdoc cref="IAesCryptor" />
        /// <summary>
        ///     Encrypts the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="password">The password.</param>
        /// <seealso cref="IAesCryptor" />
        public void EncryptFile(string fileName, string password)
        {
            var encryptedFile = GetEncryptedFileName(fileName);
            DeleteFileIfExists(encryptedFile);
            Crypt.Encrypt(password, fileName, encryptedFile);
            DeleteFileIfExists(fileName);
        }

        /// <inheritdoc cref="IAesCryptor" />
        /// <summary>
        ///     Decrypts the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="password">The password.</param>
        /// <returns>The decrypted file contents as <see cref="string"/>.</returns>
        /// <seealso cref="IAesCryptor" />
        public string DecryptFile(string fileName, string password)
        {
            var encryptedFile = GetEncryptedFileName(fileName);
            var encryptedData = File.ReadAllBytes(encryptedFile);
            return DecryptData(encryptedData, password);
        }

        /// <summary>
        ///     Decrypts the data.
        /// </summary>
        /// <param name="encryptedData">The encrypted data.</param>
        /// <param name="password">The password.</param>
        /// <returns>A <see cref="string" /> containing the decrypted data.</returns>
        private static string DecryptData(byte[] encryptedData, string password)
        {
            using var encryptedStream = new MemoryStream(encryptedData);
            using var normalStream = new MemoryStream();
            Crypt.Decrypt(password, encryptedStream, normalStream);
            var normalBytes = normalStream.ToArray();
            var normalText = Encoding.UTF8.GetString(normalBytes);

            return normalText;
        }

        /// <summary>
        ///     Deletes the file if it exists.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        private static void DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName)) File.Delete(fileName);
        }

        /// <summary>
        ///     Gets the name of the encrypted file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>Gets the encrypted file name.</returns>
        private static string GetEncryptedFileName(string fileName)
        {
            return fileName + ".aes";
        }
    }
}