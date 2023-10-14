// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAesCryptor.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   Defines the IAesCryptor type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace NetCoreMQTTExampleJsonConfig;

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
