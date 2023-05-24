using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    /// <summary>
    /// Test
    /// </summary>
    [Route("api/[controller]/[Action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Test Login
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(string username, string password, string code)
        {
            var data = new
            {
                sessionId = "session_test123456",
                time = DateTime.Now
            };
            return Ok(new { StatusCode = 200, data });
        }
    }
}
