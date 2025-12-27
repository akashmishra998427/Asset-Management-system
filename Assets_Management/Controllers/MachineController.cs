using AssetManagement_DataAccess;
using AssetManagement_DataAccess.Reports;
using AssetManagement_EntityClass.Reports;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace Assets_Management.Controllers
{
    public class MachineController : Controller
    {
        private readonly SQL_DB _SQL_DB;
        private readonly MachineReport M_Rpt;
        DataTable Result = new DataTable();

        public MachineController(SQL_DB _Db, MachineReport Machine)
        {
            _SQL_DB = _Db;
            M_Rpt = Machine;
        }

        public IActionResult MachineReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMachineReport([FromBody] MachineReportEntity Entity)
        {
            try
            {
                Result = await M_Rpt.GetMachineDetails(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Res = JsonConvert.SerializeObject(Result);
                    return Ok(Res);
                }
                else
                {
                    return NotFound(new { message = "No Machine Details Found", success = false });
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { message = "No Machine Details Found", success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindPremises()
        {
            DataTable Result = await M_Rpt.BindPremises();
            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(Response);
            }
            else
            {
                return NotFound("No Unit Found");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindLineID()
        {
            DataTable Result = await M_Rpt.BindLineID();
            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(Response);
            }
            else
            {
                return NotFound("No Unit Found");
            }
        }
    }
}
