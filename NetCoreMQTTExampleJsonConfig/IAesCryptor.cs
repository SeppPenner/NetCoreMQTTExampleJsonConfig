﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAesCryptor.cs" company="Haemmer Electronics">
//   Copyright (c) 2020 All rights reserved.
// </copyright>
// <summary>
//   Defines the IAesCryptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleJsonConfig
{
    using System;

    /// <inheritdoc cref="IDisposable" />
    /// <summary>
    ///     A service to encrypt files and decrypt file data to a string.
    /// </summary>
    public interface IAesCryptor : IDisposable
    {
        /// <summary>
        ///     Encrypts the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="password">The password.</param>
        void EncryptFile(string fileName, string password);

        /// <summary>
        ///     Decrypts the file.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="password">The password.</param>
        /// <returns>The decrypted file contents as <see cref="string"/>.</returns>
        string DecryptFile(string fileName, string password);
    }
}