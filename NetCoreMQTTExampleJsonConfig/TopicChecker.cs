
namespace NetCoreMQTTExampleJsonConfig
{
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A class to test the topics accoridng to the regex provided.
    /// </summary>
    public static class TopicChecker
    {
        /// <summary>
        /// Does a regex check on the topics.
        /// </summary>
        /// <param name="allowedTopic">The allowed topic.</param>
        /// <param name="topic">The topic.</param>
        /// <returns><c>true</c> if the topic is valid, <c>false</c> if not.</returns>
        public static bool Test(string allowedTopic, string topic)
        {
            var realTopicRegex = allowedTopic.Replace(@"/", @"\/").Replace("+", "§").Replace("#", @"[a-zA-Z0-9 \/_#+.-]*").Replace("§", @"[a-zA-Z0-9 _.-]*");
            var regex = new Regex(realTopicRegex);
            var matches = regex.Matches(topic);

            foreach (var match in matches.ToList())
            {
                if (match.Value == topic)
                {
                    return true;
                }
            }

            return false;
        }
    }
}