using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace KeyValueStore.BusinessLayer.Tests
{
    [TestFixture]
    public class InMemoryKeyValueStoreTests
    {
        private IKeyValueStore _store;

        [SetUp]
        public void Setup()
        {
            _store = new InMemoryKeyValueStore();
            _store.SetValue("fruit", "apple");
        }

        [Test]
        public void TryGetValue_WhenMissingKey_ShouldReturnFalse()
        {
            Assert.IsFalse(_store.TryGetValue("missing-key", out string actualValue));
        }

        [Test]
        public void TryGetValue_WhenKeyExists_ShouldGetValue()
        {
            _store.TryGetValue("fruit", out string actualValue);
            Assert.AreEqual("apple", actualValue);
        }

        [Test]
        public void TryGetValue_WhenKeyExists_ShouldReturnTrue()
        {
            Assert.IsTrue(_store.TryGetValue("fruit", out string actualValue));
        }

        [Test]
        public void DeleteKey_ShouldRemoveKey()
        {
            _store.DeleteKey("fruit");
            Assert.IsFalse(_store.TryGetValue("fruit", out string actualValue));
        }

        [Test]
        public void GetKeys_WhenNotEmpty_ShouldReturnAllExistingKeys()
        {
            var keys = _store.GetKeys().ToList();
            Assert.AreEqual(1, keys.Count);
            Assert.AreEqual("fruit", keys[0]);
        }

        [Test]
        public void GetKeys_WhenEmpty_ShouldReturnEmptyCollection()
        {
            _store.DeleteKey("fruit");
            Assert.IsEmpty(_store.GetKeys());
        }

        [Test]
        public void MultiThreadedUpdates_ShouldProcessAllKeysAndValues()
        {
            // Arrange
            var keys = Enumerable.Range(1, 100000).Select(k => k.ToString()).ToList();

            // Act
            var tasks = new List<Task>();

            foreach (var key in keys)
            {
                var task = Task
                    .Run(() => _store.SetValue(key, key))
                    .ContinueWith(_ => _store.DeleteKey(key))
                    .ContinueWith(_ => _store.SetValue(key, key));

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            foreach (var key in keys)
            {
                if (!_store.TryGetValue(key, out string v))
                {
                    Assert.Fail($"missing key: {key}");
                }

                Assert.AreEqual(key, v);
            }
        }
    }
}
