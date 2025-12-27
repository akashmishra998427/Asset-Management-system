using System.Data;
using AssetManagement_DataAccess;
using AssetManagement_EntityClass;
using Assets_Management.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Assets_Management.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApiConnect _apiConnect;
        private readonly IConfiguration _configuration;
        private readonly SQL_DB _SQL_DB;
        private readonly Login _Login;
        private DataTable Result = new DataTable();

        public LoginController(IConfiguration configuration, ApiConnect apiConnect, SQL_DB _DB, Login log)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
            _SQL_DB = _DB;
            _Login = log;
        }

        public IActionResult Login()
        {
            ViewBag.ApiBasurl = _apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult Master()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> BindMenu([FromBody] LoginEntity Entity)
        {
            try
            {
                Result = await _Login.BindMenu(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No asset details found.");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs($"Unexpected Error Occured While Processing{ex.Message} \n {ex.StackTrace}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
