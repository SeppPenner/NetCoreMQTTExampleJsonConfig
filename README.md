NetCoreMQTTExampleJsonConfig
====================================

NetCoreMQTTExampleJsonConfig is a project to check user credentials and topic restrictions from [MQTTnet](https://github.com/chkr1011/MQTTnet) from a json config file. The project was written and tested in .NetCore 2.2.

[![Build status](https://ci.appveyor.com/api/projects/status/fiew5qifetygw02w?svg=true)](https://ci.appveyor.com/project/SeppPenner/netcoremqttexamplejsonconfig)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/stargazers)
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/SeppPenner/NetCoreMQTTExampleJsonConfig/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleJsonConfig/badge.svg)](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleJsonConfig)

## Main code:
```csharp
namespace NetCoreMQTTExampleJsonConfig
{
    using System;

    using MQTTnet;
    using MQTTnet.Protocol;
    using MQTTnet.Server;

    using Newtonsoft.Json;

    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;

    /// <summary>
    ///     The main program.
    /// </summary>
    public class Program
    {
        private static readonly IAesCryptor aesCryptor = new AesCryptor();

        private const string Password = "somePassword";

        /// <summary>
        ///     The main method that starts the service.
        /// </summary>
        /// <param name="args">Some arguments. Currently unused.</param>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1650:ElementDocumentationMustBeSpelledCorrectly",
            Justification = "Reviewed. Suppression is OK here.")]
        public static void Main(string[] args)
        {
            var currentPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var certificate = new X509Certificate2(
                Path.Combine(currentPath, "certificate.pfx"),
                "test",
                X509KeyStorageFlags.Exportable);

            var config = ReadConfiguration(currentPath);

            var optionsBuilder = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1883)
                .WithEncryptedEndpoint()
                .WithEncryptedEndpointPort(config.Port)
                .WithEncryptionCertificate(certificate.Export(X509ContentType.Pfx))
                .WithEncryptionSslProtocol(SslProtocols.Tls12)
                .WithConnectionValidator(
                    c =>
                        {
                            var currentUser = config.Users.FirstOrDefault(u => u.UserName == c.Username);

                            if (currentUser == null)
                            {
                                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                                return;
                            }

                            if (c.Username != currentUser.UserName)
                            {
                                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                                return;
                            }

                            if (c.Password != currentUser.Password)
                            {
                                c.ReturnCode = MqttConnectReturnCode.ConnectionRefusedBadUsernameOrPassword;
                                return;
                            }

                            c.ReturnCode = MqttConnectReturnCode.ConnectionAccepted;
                        })
                .WithSubscriptionInterceptor(
                    c =>
                        {
                            var currentUser = config.Users.FirstOrDefault(u => u.ClientId == c.ClientId);

                            if (currentUser == null)
                            {
                                c.AcceptSubscription = false;
                                c.CloseConnection = true;
                                return;
                            }

                            var topic = c.TopicFilter.Topic;

                            if (currentUser.AllowedTopics.Contains(topic))
                            {
                                c.AcceptSubscription = true;
                                return;
                            }

                            c.AcceptSubscription = false;
                            c.CloseConnection = true;
                        });

            var mqttServer = new MqttFactory().CreateMqttServer();
            mqttServer.StartAsync(optionsBuilder.Build());
            Console.ReadLine();
        }

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        /// <param name="currentPath">The current path.</param>
        /// <returns>A <see cref="Config"/> object.</returns>
        private static Config ReadConfiguration(string currentPath)
        {
            Config config;

            var filePath = $"{currentPath}\\config.json";

            if (File.Exists(filePath))
            {
                using (var r = new StreamReader(filePath))
                {
                    var json = r.ReadToEnd();
                    config = JsonConvert.DeserializeObject<Config>(json);
                }

                if (!string.IsNullOrWhiteSpace(Password))
                {
                    aesCryptor.EncryptFile(filePath, Password);
                }

                return config;
            }
            else
            {
                var decrypted = aesCryptor.DecryptFile(filePath, Password);
                return JsonConvert.DeserializeObject<Config>(decrypted);
            }
        }
    }
}
```

## Attention:
* The project currently only matches topics exactly. I want to provide a regex later, check: https://github.com/eclipse/mosquitto/issues/1317.
* The project only works properly when the ClientId is properly set in the clients (and in the config.json, of course).

## Create an openssl certificate:
```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365
openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
```

An example certificate is in the folder. Password for all is `test`.

Change history
--------------

* **Version 1.0.0.0 (2019-06-21)** : 1.0 release.