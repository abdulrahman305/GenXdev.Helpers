using Microsoft.Win32;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.X509;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace GenXdev.Helpers
{
    public static class Pki
  { 
        public static Pkcs12Store CreatePkcs12Store()
    {
        // Generate the key pair
        var random = new SecureRandom();
        var keyGenerationParameter = new KeyGenerationParameters(random, 4096);
        var keyPairGenerator = new RsaKeyPairGenerator();
        keyPairGenerator.Init(keyGenerationParameter);
        var keyPair = keyPairGenerator.GenerateKeyPair();

            // Generate the certificate
            var generator = new X509V3CertificateGenerator();

            string signatureAlgorithm = "SHA256WithRSA"; // Change this as needed
            ISignatureFactory signatureFactory = new Asn1SignatureFactory(signatureAlgorithm, keyPair.Private);

            var cert = generator.Generate(signatureFactory);
            
            generator.SetPublicKey(keyPair.Public);
        
        // Create the PKCS12 store
        var builder = new Pkcs12StoreBuilder();
        builder.SetUseDerEncoding(true);
        var store = builder.Build();

        // Add a certificate entry to the store
        var certEntry = new X509CertificateEntry(cert);
        store.SetCertificateEntry("mycertificates.net", certEntry);

        // Add a private key entry to the store
        var keyEntry = new AsymmetricKeyEntry(keyPair.Private);
        store.SetKeyEntry("mycertificates.net", keyEntry, new[] { certEntry });

        return store;
    }
        static int bitStrength = 2048;

        /// <summary>
        /// Export a certificate to a PEM format string
        /// </summary>
        /// <param name="cert">The certificate to export</param>
        /// <returns>A PEM encoded string</returns>
        public static string ExportToPEM(System.Security.Cryptography.X509Certificates.X509Certificate cert)
        {
            StringBuilder builder = new StringBuilder();

            builder.AppendLine("-----BEGIN CERTIFICATE-----");
            builder.AppendLine(Convert.ToBase64String(cert.Export(X509ContentType.Cert), Base64FormattingOptions.InsertLineBreaks));
            builder.AppendLine("-----END CERTIFICATE-----");

            return builder.ToString();
        }

        public static X509Certificate2 LoadCertificate(string issuerFileName, string password = null)
        {
            try
            {
                // We need to pass 'Exportable', otherwise we can't get the public key.
                return new X509Certificate2(issuerFileName, password, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }
            catch (System.Security.Cryptography.CryptographicException)
            {
                // We need to pass 'Exportable', otherwise we can't get the public key.
                return new X509Certificate2(issuerFileName, default(String), X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            }
        }

        public static X509Certificate2 IssueCertificate(string subjectName, X509Certificate2 issuerCertificate, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            // It's self-signed, so these are the same.
            var issuerName = issuerCertificate.Subject;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, bitStrength);

            var issuerKeyPair = DotNetUtilities.GetKeyPair(issuerCertificate.GetRSAPrivateKey());

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = new Org.BouncyCastle.Math.BigInteger(issuerCertificate.GetSerialNumber());

            const bool isCertificateAuthority = false;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        public static X509Certificate2 CreateCertificateAuthorityCertificate(string subjectName, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            // It's self-signed, so these are the same.
            var issuerName = subjectName;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, bitStrength);

            // It's self-signed, so these are the same.
            var issuerKeyPair = subjectKeyPair;

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber; // Self-signed, so it's the same serial number.

            const bool isCertificateAuthority = true;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages, 3650 * 2);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        public static X509Certificate2 CreateSelfSignedCertificate(string subjectName, string[] subjectAlternativeNames, KeyPurposeID[] usages)
        {
            // It's self-signed, so these are the same.
            var issuerName = subjectName;

            var random = GetSecureRandom();
            var subjectKeyPair = GenerateKeyPair(random, bitStrength);

            // It's self-signed, so these are the same.
            var issuerKeyPair = subjectKeyPair;

            var serialNumber = GenerateSerialNumber(random);
            var issuerSerialNumber = serialNumber; // Self-signed, so it's the same serial number.

            const bool isCertificateAuthority = false;
            var certificate = GenerateCertificate(random, subjectName, subjectKeyPair, serialNumber,
                                                  subjectAlternativeNames, issuerName, issuerKeyPair,
                                                  issuerSerialNumber, isCertificateAuthority,
                                                  usages);
            return ConvertCertificate(certificate, subjectKeyPair, random);
        }

        public static SecureRandom GetSecureRandom()
        {
            // Since we're on Windows, we'll use the CryptoAPI one (on the assumption
            // that it might have access to better sources of entropy than the built-in
            // Bouncy Castle ones):
            var randomGenerator = new CryptoApiRandomGenerator();
            var random = new SecureRandom(randomGenerator);
            return random;
        }

        public static Org.BouncyCastle.X509.X509Certificate GenerateCertificate(SecureRandom random,
                                                           string subjectName,
                                                           AsymmetricCipherKeyPair subjectKeyPair,
                                                           Org.BouncyCastle.Math.BigInteger subjectSerialNumber,
                                                           string[] subjectAlternativeNames,
                                                           string issuerName,
                                                           AsymmetricCipherKeyPair issuerKeyPair,
                                                           Org.BouncyCastle.Math.BigInteger issuerSerialNumber,
                                                           bool isCertificateAuthority,
                                                           KeyPurposeID[] usages,
                                                           uint LifeSpanDays = 365 * 2
                                                           )
        {
            var certificateGenerator = new X509V3CertificateGenerator();

            certificateGenerator.SetSerialNumber(subjectSerialNumber);

            var issuerDN = new X509Name(issuerName);
            certificateGenerator.SetIssuerDN(issuerDN);

            // Note: The subject can be omitted if you specify a subject alternative name (SAN).
            var subjectDN = new X509Name(subjectName);
            certificateGenerator.SetSubjectDN(subjectDN);

            // Our certificate needs valid from/to values.
            var notBefore = System.DateTime.UtcNow.Date.AddDays(-2);
            var notAfter = notBefore.AddDays(LifeSpanDays);

            certificateGenerator.SetNotBefore(notBefore);
            certificateGenerator.SetNotAfter(notAfter);

            // The subject's public key goes in the certificate.
            certificateGenerator.SetPublicKey(subjectKeyPair.Public);

            AddAuthorityKeyIdentifier(certificateGenerator, issuerDN, issuerKeyPair, issuerSerialNumber);
            AddSubjectKeyIdentifier(certificateGenerator, subjectKeyPair);
            AddBasicConstraints(certificateGenerator, isCertificateAuthority);

            if (usages != null && usages.Any())
                AddExtendedKeyUsage(certificateGenerator, usages);

            if (subjectAlternativeNames != null && subjectAlternativeNames.Any())
                AddSubjectAlternativeNames(certificateGenerator, subjectAlternativeNames);

            // Set the signature algorithm. 
            ISignatureFactory signatureFactory = new Asn1SignatureFactory("SHA512WITHRSA", issuerKeyPair.Private, random);

            // The certificate is signed with the issuer's public key.
            var certificate = certificateGenerator.Generate(signatureFactory);
            return certificate;
        }

        /// <summary>
        /// The certificate needs a serial number. This is used for revocation,
        /// and usually should be an incrementing index (which makes it easier to revoke a range of certificates).
        /// Since we don't have anywhere to store the incrementing index, we can just use a random number.
        /// </summary>
        /// <param name="random"></param>
        /// <returns></returns>
        public static Org.BouncyCastle.Math.BigInteger GenerateSerialNumber(SecureRandom random)
        {
            var serialNumber =
                BigIntegers.CreateRandomInRange(
                    Org.BouncyCastle.Math.BigInteger.One, Org.BouncyCastle.Math.BigInteger.ValueOf(Int64.MaxValue), random);
            return serialNumber;
        }

        /// <summary>
        /// Generate a key pair.
        /// </summary>
        /// <param name="random">The random number generator.</param>
        /// <param name="strength">The key length in bits. For RSA, 2048 bits should be considered the minimum acceptable these days.</param>
        /// <returns></returns>
        public static AsymmetricCipherKeyPair GenerateKeyPair(SecureRandom random, int strength)
        {
            var keyGenerationParameters = new KeyGenerationParameters(random, strength);

            var keyPairGenerator = new RsaKeyPairGenerator();
            keyPairGenerator.Init(keyGenerationParameters);
            var subjectKeyPair = keyPairGenerator.GenerateKeyPair();
            return subjectKeyPair;
        }

        /// <summary>
        /// Add the Authority Key Identifier. According to http://www.alvestrand.no/objectid/2.5.29.35.html, this
        /// identifies the public key to be used to verify the signature on this certificate.
        /// In a certificate chain, this corresponds to the "Subject Key Identifier" on the *issuer* certificate.
        /// The Bouncy Castle documentation, at http://www.bouncycastle.org/wiki/display/JA1/X.509+Public+Key+Certificate+and+Certification+Request+Generation,
        /// shows how to create this from the issuing certificate. Since we're creating a self-signed certificate, we have to do this slightly differently.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="issuerDN"></param>
        /// <param name="issuerKeyPair"></param>
        /// <param name="issuerSerialNumber"></param>
        public static void AddAuthorityKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                      X509Name issuerDN,
                                                      AsymmetricCipherKeyPair issuerKeyPair,
                                                      Org.BouncyCastle.Math.BigInteger issuerSerialNumber)
        {
            var authorityKeyIdentifierExtension =
                new AuthorityKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(issuerKeyPair.Public),
                    new GeneralNames(new GeneralName(issuerDN)),
                    issuerSerialNumber);
            certificateGenerator.AddExtension(
                X509Extensions.AuthorityKeyIdentifier.Id, false, authorityKeyIdentifierExtension);
        }

        /// <summary>
        /// Add the "Subject Alternative Names" extension. Note that you have to repeat
        /// the value from the "Subject Name" property.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectAlternativeNames"></param>
        public static void AddSubjectAlternativeNames(X509V3CertificateGenerator certificateGenerator,
                                                       IEnumerable<string> subjectAlternativeNames)
        {
            var subjectAlternativeNamesExtension =
                new DerSequence(
                    subjectAlternativeNames.Select(name => new GeneralName(GeneralName.DnsName, name))
                                           .ToArray<Asn1Encodable>());

            certificateGenerator.AddExtension(
                X509Extensions.SubjectAlternativeName.Id, false, subjectAlternativeNamesExtension);
        }

        /// <summary>
        /// Add the "Extended Key Usage" extension, specifying (for example) "server authentication".
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="usages"></param>
        public static void AddExtendedKeyUsage(X509V3CertificateGenerator certificateGenerator, KeyPurposeID[] usages)
        {
            certificateGenerator.AddExtension(
                X509Extensions.ExtendedKeyUsage.Id, false, new ExtendedKeyUsage(usages));
        }

        /// <summary>
        /// Add the "Basic Constraints" extension.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="isCertificateAuthority"></param>
        public static void AddBasicConstraints(X509V3CertificateGenerator certificateGenerator,
                                                bool isCertificateAuthority)
        {
            certificateGenerator.AddExtension(
                X509Extensions.BasicConstraints.Id, true, new BasicConstraints(isCertificateAuthority));
        }

        /// <summary>
        /// Add the Subject Key Identifier.
        /// </summary>
        /// <param name="certificateGenerator"></param>
        /// <param name="subjectKeyPair"></param>
        public static void AddSubjectKeyIdentifier(X509V3CertificateGenerator certificateGenerator,
                                                    AsymmetricCipherKeyPair subjectKeyPair)
        {
            var subjectKeyIdentifierExtension =
                new SubjectKeyIdentifier(
                    SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(subjectKeyPair.Public));
            certificateGenerator.AddExtension(
                X509Extensions.SubjectKeyIdentifier.Id, false, subjectKeyIdentifierExtension);
        }

        public static X509Certificate2 ConvertCertificate(Org.BouncyCastle.X509.X509Certificate certificate,
                                                           AsymmetricCipherKeyPair subjectKeyPair,
                                                           SecureRandom random,
                                                           string password = null)
        {
            // Now to convert the Bouncy Castle certificate to a .NET certificate.
            // See http://web.archive.org/web/20100504192226/http://www.fkollmann.de/v2/post/Creating-certificates-using-BouncyCastle.aspx
            // ...but, basically, we create a PKCS12 store (a .PFX file) in memory, and add the public and public key to that.

            var store = CreatePkcs12Store();

            // What Bouncy Castle calls "alias" is the same as what Windows terms the "friendly name".
            string friendlyName = certificate.SubjectDN.ToString();

            // Add the certificate.
            var certificateEntry = new X509CertificateEntry(certificate);
            store.SetCertificateEntry(friendlyName, certificateEntry);

            // Add the public key.
            store.SetKeyEntry(friendlyName, new AsymmetricKeyEntry(subjectKeyPair.Private), new[] { certificateEntry });

            // Convert it to an X509Certificate2 object by saving/loading it from a MemoryStream.
            // It needs a password. Since we'll remove this later, it doesn't particularly matter what we use.
            var stream = new MemoryStream();
            store.Save(stream, "password".ToCharArray(), random);

            var convertedCertificate =
                new X509Certificate2(stream.ToArray(),
                                     "password",
                                     X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
            return convertedCertificate;
        }

        public static void WriteCertificate(X509Certificate2 certificate, string targetFilePath, string password = null)
        {
            var bytes = certificate.Export(X509ContentType.Pfx, password);
            FileSystem.ForciblyPrepareTargetFilePath(targetFilePath);
            File.WriteAllBytes(targetFilePath, bytes);
        }

        public static void WriteCertificateToPemFormat(X509Certificate2 certificate, string targetFilePath)
        {
            var text = ExportToPEM(certificate);
            FileSystem.ForciblyPrepareTargetFilePath(targetFilePath);
            File.WriteAllText(targetFilePath, text, new UTF8Encoding(false));
        }

        public static void EnableTlsProtocols()
        {
            // https://docs.microsoft.com/en-us/dotnet/framework/configure-apps/file-schema/runtime/appcontextswitchoverrides-element

            try
            {
                const string DisableCachingName = @"TestSwitch.LocalAppContext.DisableCaching";
                AppContext.SetSwitch(DisableCachingName, true);
            }
            catch { }
            try
            {
                const string DontEnableSchUseStrongCrypto = @"Switch.System.Net.DontEnableSchUseStrongCrypto";
                AppContext.SetSwitch(DontEnableSchUseStrongCrypto, true);
            }
            catch { }
            try
            {
                const string DontEnableSystemDefaultTlsVersions = @"TestSwitch.LocalAppContext.DontEnableSystemDefaultTlsVersions";
                AppContext.SetSwitch(DontEnableSystemDefaultTlsVersions, true);
            }
            catch { }
            try
            {
                const string DontEnableTlsAlerts = @"Switch.System.Net.DontEnableTlsAlerts";
                AppContext.SetSwitch(DontEnableTlsAlerts, true);
            }
            catch { }
            try
            {
                const string DontCheckCertificateEKUs = @"TestSwitch.LocalAppContext.DontCheckCertificateEKUs";
                AppContext.SetSwitch(DontCheckCertificateEKUs, true);
            }
            catch { }
            try
            {
                const string DontEnableSchSendAuxRecord = @"Switch.System.Net.DontEnableSchSendAuxRecord";
                AppContext.SetSwitch(DontEnableSchSendAuxRecord, true);
            }
            catch { }
            try
            {
                const string DisableUsingServicePointManagerSecurityProtocols = @"TestSwitch.LocalAppContext.DisableUsingServicePointManagerSecurityProtocols";
                AppContext.SetSwitch(DisableUsingServicePointManagerSecurityProtocols, false);
            }
            catch { }
            try
            {
                const string DontEnableSchUseStrongCrypto = @"TestSwitch.LocalAppContext.DontEnableSchUseStrongCrypto";
                AppContext.SetSwitch(DontEnableSchUseStrongCrypto, true);
            }
            catch { }
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Client", true))
                {
                    key.SetValue("Enabled", "1", RegistryValueKind.DWord);
                }
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.2\Server", true))
                {
                    key.SetValue("Enabled", "1", RegistryValueKind.DWord);
                }
            }
            catch { }
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS 1.3\Client", true))
                {
                    key.SetValue("Enabled", "1", RegistryValueKind.DWord);
                }
            }
            catch { }
            try
            {
                using (var key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Cryptography\Configuration\Local\SSL\00010003", true))
                {
                    var currentValue = ((string[])key.GetValue("Functions")).ToList<String>();

                    if (currentValue != null && !currentValue.Contains("RSA/SHA512"))
                    {
                        currentValue.Add(System.Environment.NewLine + "RSA/SHA512");

                        key.SetValue("Function", currentValue.ToArray<string>(), RegistryValueKind.MultiString);
                    }
                }
            }
            catch { }

        }

        public static void OptimizeAndReorganizeDNSNames(ref string certSubjectName, ref string[] certSubjectAltNames)
        {
            if ((certSubjectAltNames == null) || (certSubjectAltNames.Length == 0))
            {
                return;
            }

            var dnsName = Dns.GetHostName().ToLowerInvariant();

            var list = (from q in certSubjectAltNames where q.Trim().ToLowerInvariant() != dnsName select q.Trim().ToLowerInvariant()).ToList<string>();

            if ((list.Count > 0) && (string.IsNullOrWhiteSpace(certSubjectName) || (certSubjectName == dnsName)))
            {
                certSubjectName = list[0];
                list.Add(dnsName);
            }

            certSubjectAltNames = list.ToArray<string>();
        }

        public static string GetPrimaryDnsName(string defaultValue, string[] certSubjectAltNames)
        {
            if ((certSubjectAltNames == null) || (certSubjectAltNames.Length == 0))
            {
                return defaultValue;
            }

            var dnsName = Dns.GetHostName().ToLowerInvariant();

            var list = (from q in certSubjectAltNames where q.Trim().ToLowerInvariant() != dnsName select q.Trim().ToLowerInvariant());

            if (list.Any())
            {
                return list.First<string>();
            }

            return defaultValue;
        }
    }
}
