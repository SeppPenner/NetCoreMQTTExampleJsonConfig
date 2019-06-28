NetCoreMQTTExampleJsonConfig
====================================

NetCoreMQTTExampleJsonConfig is a project to check user credentials and topic restrictions from [MQTTnet](https://github.com/chkr1011/MQTTnet) from a json config file. The project was written and tested in .NetCore 2.2.

[![Build status](https://ci.appveyor.com/api/projects/status/fiew5qifetygw02w?svg=true)](https://ci.appveyor.com/project/SeppPenner/netcoremqttexamplejsonconfig)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/NetCoreMQTTExampleJsonConfig.svg)](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/stargazers)
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/SeppPenner/NetCoreMQTTExampleJsonConfig/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleJsonConfig/badge.svg)](https://snyk.io/test/github/SeppPenner/NetCoreMQTTExampleJsonConfig)

## JSON configuration (Adjust this to your needs):
```json
{
  "Port": 8883,
  "Users": [
    {
      "UserName": "Hans",
      "ClientId": "Hans",
      "Password": "Test",
      "SubscriptionTopicLists": {
        "BlacklistTopics": [
          "a",
          "b/+",
          "c/#"
        ],
        "WhitelistTopics": [
          "d",
          "e/+",
          "f/#"
        ]
      },
      "PublishTopicLists": {
        "BlacklistTopics": [
          "a",
          "b/+",
          "c/#"
        ],
        "WhitelistTopics": [
          "d",
          "e/+",
          "f/#"
        ]
      }
    }
  ]
}
```

## Attention:
* The project only works properly when the ClientId is properly set in the clients (and in the config.json, of course).

## Further information:
The function `TopicMatch` in the [TopicChecker](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/blob/master/NetCoreMQTTExampleJsonConfig/TopicChecker.cs)
class is the translation of https://github.com/eclipse/mosquitto/blob/master/lib/util_topic.c#L138 from the Eclipse Mosquitto project to C#.
This function is dual licensed under the Eclipse Public License 1.0 and the Eclipse Distribution License 1.0.
Check the [epl-v10](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/blob/master/epl-v10.txt) and
[edl-v10](https://github.com/SeppPenner/NetCoreMQTTExampleJsonConfig/blob/master/edl-v10.txt) files for further license information.

## Create an openssl certificate:
```bash
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365
openssl pkcs12 -export -out certificate.pfx -inkey key.pem -in cert.pem
```

An example certificate is in the folder. Password for all is `test`.

Change history
--------------

* **Version 1.0.0.0 (2019-06-28)** : 1.0 release.