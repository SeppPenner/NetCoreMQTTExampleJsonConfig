using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetCoreMQTTExampleJsonConfig;

namespace TopicCheckerTest
{
    /// <summary>
    /// A test class to test the <see cref="TopicCheckerTest"/> with the # operator.
    /// </summary>
    [TestClass]
    public class TopicCheckerTestsCrossOperator
    {
        /// <summary>
        /// Checks the tester with a valid topic for the # operator.
        /// </summary>
        [TestMethod]
        public void CheckSingleValueCrossMatch()
        {
            var result = TopicChecker.Test("a/#", "a/b");
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks the tester with another valid topic for the # operator.
        /// </summary>
        [TestMethod]
        public void CheckSingleValueCrossMatch2()
        {
            var result = TopicChecker.Test("a/#", "a/b/c");
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks the tester with a valid topic with a # for the # operator.
        /// </summary>
        [TestMethod]
        public void CheckSingleValueCrossMatchWithCross()
        {
            var result = TopicChecker.Test("a/#", "a/#");
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks the tester with a valid topic with a + for the # operator.
        /// </summary>
        [TestMethod]
        public void CheckSingleValueCrossMatchWithPlus()
        {
            var result = TopicChecker.Test("a/#", "a/+");
            Assert.IsTrue(result);
        }

        /// <summary>
        /// Checks the tester with an invalid topic with an invalid char for the # operator.
        /// </summary>
        [TestMethod]
        public void CheckSingleValueCrossDontMatchInvalidChar()
        {
            var result = TopicChecker.Test("a/#", "a/?");
            Assert.IsFalse(result);
        }
    }
}
