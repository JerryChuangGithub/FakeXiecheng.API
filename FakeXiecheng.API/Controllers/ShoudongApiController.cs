using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace FakeXiecheng.API.Controllers
{
    [Route("api/ShoudongApi")]
    // [Controller]
    // public class ShoudongApi
    // public class ShoudongApiController
    public class ShoudongApiController : Controller
    {
        // GET api/<controller>
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
    }
}