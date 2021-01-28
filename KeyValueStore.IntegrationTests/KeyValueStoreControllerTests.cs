using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using KeyValueStore.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KeyValueStore.IntegrationTests
{
    [TestClass]
    public class KeyValueStoreControllerTests
    {
        private const string _mediaType = "application/json";
        private TestServer _server;
        private HttpClient _client;

        [TestInitialize]
        public void Init()
        {
            var webHostBuilder = new WebHostBuilder()
                .UseStartup<Startup>();

            _server = new TestServer(webHostBuilder);
            _client = _server.CreateClient();
            _client.BaseAddress = new Uri(_client.BaseAddress, "/api/store/");
        }

        [TestMethod]
        public async Task SetValue()
        {
            var key = "fruit";
            var value = "\"apple\"";

            var response = await _client.PostAsync($"{key}", new StringContent(value, Encoding.UTF8, _mediaType));
            Assert.IsTrue(response.IsSuccessStatusCode);

            response = await _client.GetAsync($"{key}");
            Assert.IsTrue(response.IsSuccessStatusCode);

            var actualValue = await response.Content.ReadAsStringAsync();
            Assert.AreEqual(value, actualValue);
        }

        [TestMethod]
        public async Task DeleteKey()
        {
            var key = "fruit";
            var value = "\"apple\"";

            var response = await _client.PostAsync($"{key}", new StringContent(value, Encoding.UTF8, _mediaType));
            Assert.IsTrue(response.IsSuccessStatusCode);

            response = await _client.DeleteAsync($"{key}");
            Assert.IsTrue(response.IsSuccessStatusCode);

            response = await _client.GetAsync($"{key}");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetKeys()
        {
            var key = "fruit";
            var value = "\"apple\"";

            var response = await _client.PostAsync($"{key}", new StringContent(value, Encoding.UTF8, _mediaType));
            Assert.IsTrue(response.IsSuccessStatusCode);

            response = await _client.GetAsync("");
            Assert.IsTrue(response.IsSuccessStatusCode);

            var keysListJson = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("[\"fruit\"]", keysListJson);
        }

        [TestMethod]
        public async Task MultiClientAccess()
        {
            // Arrange
            var keys = Enumerable.Range(1, 1000).Select(k => k.ToString()).ToList();

            // Act
            var tasks = new List<Task>();

            foreach (var key in keys)
            {
                var task = _client.PostAsync($"{key}", new StringContent($"\"{key}\"", Encoding.UTF8, _mediaType));
                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());

            // Assert
            foreach (var key in keys)
            {
                var response = await _client.GetAsync($"{key}");
                Assert.IsTrue(response.IsSuccessStatusCode, $"Expected OK for key {key}, but got {response.StatusCode})");

                var actualValue = await response.Content.ReadAsStringAsync();
                Assert.AreEqual($"\"{key}\"", actualValue);
            }
        }
    }
}
