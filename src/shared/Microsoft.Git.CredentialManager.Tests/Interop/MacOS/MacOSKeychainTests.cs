// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
using System;
using Xunit;
using Microsoft.Git.CredentialManager.Interop.MacOS;

namespace Microsoft.Git.CredentialManager.Tests.Interop.MacOS
{
    public class MacOSKeychainTests
    {
        private const string TestNamespace = "git-test";

        [PlatformFact(Platforms.MacOS)]
        public void MacOSKeychain_ReadWriteDelete()
        {
            var keychain = new MacOSKeychain(TestNamespace);

            // Create a service that is guaranteed to be unique
            string service = $"https://example.com/{Guid.NewGuid():N}";
            const string account = "john.doe";
            const string password = "letmein123"; // [SuppressMessage("Microsoft.Security", "CS001:SecretInline", Justification="Fake credential")]

            try
            {
                // Write
                keychain.AddOrUpdate(service, account, password);

                // Read
                ICredential outCredential = keychain.Get(service, account);

                Assert.NotNull(outCredential);
                Assert.Equal(account, outCredential.Account);
                Assert.Equal(password, outCredential.Password);
            }
            finally
            {
                // Ensure we clean up after ourselves even in case of 'get' failures
                keychain.Remove(service, account);
            }
        }

        [PlatformFact(Platforms.MacOS)]
        public void MacOSKeychain_Get_NotFound_ReturnsNull()
        {
            var keychain = new MacOSKeychain(TestNamespace);

            // Unique service; guaranteed not to exist!
            string service = $"https://example.com/{Guid.NewGuid():N}";

            ICredential credential = keychain.Get(service, account: null);
            Assert.Null(credential);
        }

        [PlatformFact(Platforms.MacOS)]
        public void MacOSKeychain_Remove_NotFound_ReturnsFalse()
        {
            var keychain = new MacOSKeychain(TestNamespace);

            // Unique service; guaranteed not to exist!
            string service = $"https://example.com/{Guid.NewGuid():N}";

            bool result = keychain.Remove(service, account: null);
            Assert.False(result);
        }
    }
}
