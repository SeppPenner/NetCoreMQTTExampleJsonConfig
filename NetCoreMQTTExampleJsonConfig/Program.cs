﻿namespace NetCoreMQTTExampleJsonConfig
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
        /// <summary>
        /// The <see cref="AesCryptor"></see>.
        /// </summary>
        private static readonly IAesCryptor aesCryptor = new AesCryptor();

        /// <summary>
        /// The password.
        /// </summary>
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
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (c.Username != currentUser.UserName)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (c.Password != currentUser.Password)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            c.ReasonCode = MqttConnectReasonCode.Success;
                        })
                .WithSubscriptionInterceptor(
                    c =>
                        {
                            var currentUser = config.Users.FirstOrDefault(u => u.ClientId == c.ClientId);

                            if (currentUser == null)
                            {
                                c.AcceptSubscription = false;
                                return;
                            }

                            var topic = c.TopicFilter.Topic;

                            if (currentUser.SubscriptionTopicLists.BlacklistTopics.Contains(topic))
                            {
                                c.AcceptSubscription = false;
                                return;
                            }

                            if (currentUser.SubscriptionTopicLists.WhitelistTopics.Contains(topic))
                            {
                                c.AcceptSubscription = true;
                                return;
                            }

                            foreach (var forbiddenTopic in currentUser.SubscriptionTopicLists.BlacklistTopics)
                            {
                                var doesTopicMatch = TopicChecker.TopicMatch(forbiddenTopic, topic);
                                if (doesTopicMatch)
                                {
                                    c.AcceptSubscription = false;
                                    return;
                                }
                            }

                            foreach (var allowedTopic in currentUser.SubscriptionTopicLists.WhitelistTopics)
                            {
                                var doesTopicMatch = TopicChecker.TopicMatch(allowedTopic, topic);
                                if (doesTopicMatch)
                                {
                                    c.AcceptSubscription = true;
                                    return;
                                }
                            }

                            c.AcceptSubscription = false;
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