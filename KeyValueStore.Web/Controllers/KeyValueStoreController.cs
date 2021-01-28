using System.Collections.Generic;
using KeyValueStore.BusinessLayer;
using Microsoft.AspNetCore.Mvc;

namespace KeyValueStore.Web.Controllers
{
    [ApiController]
    [Route("api/store")]
    public class KeyValueStoreController : ControllerBase
    {
        private readonly IKeyValueStore _store;

        public KeyValueStoreController(IKeyValueStore store)
        {
            _store = store;
        }

        [HttpGet]
        public ICollection<string> GetKeys()
        {
            return _store.GetKeys();
        }

        [HttpGet("{key}")]
        public IActionResult GetValue(string key)
        {
            if (!_store.TryGetValue(key, out string value))
            {
                return NotFound();
            }

            return new JsonResult(value);
        }

        [HttpPost("{key}")]
        public void SetValue(string key, [FromBody] string value)
        {
            _store.SetValue(key, value);
        }

        [HttpDelete("{key}")]
        public void DeleteKey(string key)
        {
            _store.DeleteKey(key);
        }
    }
}
