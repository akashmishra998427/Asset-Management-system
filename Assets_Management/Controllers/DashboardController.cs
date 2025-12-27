using AssetManagement_DataAccess;
using AssetManagement_EntityClass.Reports;
using Assets_Management.Utilities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;

namespace Assets_Management.Controllers
{
    public class DashboardController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ApiConnect _apiConnect;
        private readonly DashboardReports _dashboardReports;
        private readonly SQL_DB _SQL_DB;
        private readonly FilePathProvider _Provider;
        private readonly IWebHostEnvironment _env;
        private DataTable Result;
        public DashboardController(IConfiguration configuration, ApiConnect apiConnect, DashboardReports dashboardReports, SQL_DB sQL_DB, FilePathProvider Provider,
            IWebHostEnvironment env)
        {
            _configuration = configuration;
            _apiConnect = apiConnect;
            _dashboardReports = dashboardReports;
            this._SQL_DB = sQL_DB;
            _Provider = Provider;
            Result = new DataTable();
            _env = env;
        }

        public IActionResult Dashboard(string? Action, string? AssetType)
        {
            try
            {
                ViewBag.ApiBasurl = _apiConnect?.WebApiUrl() ?? "DefaultUrl";
                ViewBag.ImageUrl = _configuration["Apisettings:ImageUrl"];
                return View();
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        public IActionResult Production()
        {
            ViewBag.ApiBasurl = _apiConnect.WebApiUrl();
            return View();
        }

        public IActionResult AttachInvoice(string? AssetType, string? MenuID)
        {
            return View();
        }

        public IActionResult DetailPage(string AssetCode)
        {
            ViewBag.ImageUrl = _configuration["Apisettings:ImageUrl"];
            return View();
        }

        public IActionResult AttachAll_TypeInvoice()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetAssetCounts_AndDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.GetAssetDetailsAsync(Entity);
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

        [HttpPost]
        public async Task<IActionResult> DownloadToExcel([FromBody] DashboardReportsEntity Entity)
        {
            Result = await _dashboardReports.GetAssetDetailsAsync(Entity);
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Asset Details");
                worksheet.Cell(1, 1).InsertTable(Result);
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(), $"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Entity.Action}.xlsx");
                }
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindColumns()
        {
            try
            {
                Result = await _dashboardReports.BindColumnNames();
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No column names  found.");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindProcessor([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindProcessor(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Processor not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindMake([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindMake(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Make not Fount");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindDepartment([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindDepartment(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Department found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindUnit([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindUnit(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Unit Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindFilteredData([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindFilteredDatas(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindPurchaseYear([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindPurchaseYear(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BinModel(DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BinModel(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindCounting([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindCounting(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindRowDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindDetailsRow(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindInvoiceDetails([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                Result = await _dashboardReports.BindInvoiceDetails(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Data Not Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        //[HttpPost]
        //public async Task<IActionResult> SaveAttachedInvoice([FromForm] DashboardReportsEntity Entity, [FromForm] IFormFile ImageFile, [FromForm] List<string> SelectedAssetCodes)
        //{
        //    try
        //    {
        //        if (ImageFile == null || ImageFile.Length == 0)
        //        {
        //            return BadRequest("Please upload asset invoice.");
        //        }

        //        if (SelectedAssetCodes == null || SelectedAssetCodes.Count == 0)
        //        {
        //            return BadRequest("Please select at least one asset.");
        //        }
        //        if (Entity.FileName == null && Entity.FileName == "")
        //        {
        //            return BadRequest("FileName Is Required");
        //        }
        //        //@"~/GIMS_AttachedDocument/Assets/BillImage"
        //        var uploadLocation = Path.Combine(_env.WebRootPath, "GIMS_AttachedDocument", "Assets", "BillImage");
        //        if (!Directory.Exists(uploadLocation))
        //        {
        //            Directory.CreateDirectory(uploadLocation);
        //        }
        //        var fileExtension = Path.GetExtension(ImageFile.FileName);

        //        var File = $"{Entity.FileName}{fileExtension}";

        //        var fileLocation = Path.Combine(uploadLocation, File);

        //        var FileExist = _Provider.UploadFolder(uploadLocation);

        //        if (FileExist.Length > 0)
        //        {
        //            foreach (var item in FileExist)
        //            {
        //                if (Entity.FileName == item)
        //                {
        //                    return StatusCode(400, new { message = "Invice using this name is already uploaded try different name 😊", success = false });
        //                }
        //            }
        //        }

        //        using (var stream = new FileStream(fileLocation, FileMode.Create))
        //        {
        //            await ImageFile.CopyToAsync(stream);
        //        }

        //        int Result = await _dashboardReports.SaveAttachedInvoice(Entity, SelectedAssetCodes);

        //        if (Result > 0)
        //        {
        //            return Ok($"Invoice attached successfully for {SelectedAssetCodes} asset(s).");
        //        }
        //        else
        //        {
        //            return BadRequest("Failed to save invoice.");
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        _SQL_DB.ExceptionLogs(ex.Message.ToString());
        //        return StatusCode(500, new { success = false, message = ex.Message });
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> SaveAttachedInvoice([FromForm] DashboardReportsEntity Entity, [FromForm] IFormFile ImageFile, [FromForm] List<string> SelectedAssetCodes)
        {
            try
            {
                if (SelectedAssetCodes == null || SelectedAssetCodes.Count == 0)
                {
                    return BadRequest("Please select at least one asset.");
                }

                if (string.IsNullOrWhiteSpace(Entity.FileName))
                {
                    return BadRequest("FileName is required");
                }

                var uploadLocation = Path.Combine(_env.WebRootPath, "GIMS_AttachedDocument", "Assets", "BillImage");
                if (!Directory.Exists(uploadLocation))
                {
                    Directory.CreateDirectory(uploadLocation);
                }

                var fileExtension = Path.GetExtension(ImageFile?.FileName ?? "");
                var finalFileName = $"{Entity.FileName}{fileExtension}";
                var fileLocation = Path.Combine(uploadLocation, finalFileName);

                _SQL_DB.ExceptionLogs($"file location is {fileLocation}  \n with the filename {finalFileName} and \t the extension is {fileExtension}");


                bool fileAlreadyExists = System.IO.File.Exists(fileLocation);

                if (fileAlreadyExists)
                {
                    int result = await _dashboardReports.SaveAttachedInvoice(Entity, SelectedAssetCodes);
                    return Ok(new { success = true, message = "File already exists. Existing file successfully tagged to selected assets.", file = finalFileName });
                }

                if (ImageFile == null || ImageFile.Length == 0)
                {
                    return BadRequest("Please upload asset invoice.");
                }

                using (var stream = new FileStream(fileLocation, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(stream);
                }

                int uploadResult = await _dashboardReports.SaveAttachedInvoice(Entity, SelectedAssetCodes);

                if (uploadResult > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = $"Invoice uploaded & attached to {SelectedAssetCodes.Count} asset(s).",
                        file = finalFileName
                    });
                }

                return BadRequest("Failed to save invoice.");
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message);
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveAllAttachedInvoice([FromForm] string AssetCode, [FromForm] string InvoiceType, [FromForm] IFormFile ImageFile)
        {
            try
            {
                if (ImageFile == null || ImageFile.Length == 0)
                {
                    return BadRequest(new { success = false, message = "Please upload asset invoice." });
                }
                if (string.IsNullOrEmpty(AssetCode))
                {
                    return BadRequest(new { success = false, message = "Asset Code is required." });
                }
                if (string.IsNullOrEmpty(InvoiceType))
                {
                    return BadRequest(new { success = false, message = "Asset Type is required." });
                }
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest(new { success = false, message = "Invalid file type. Only PDF, JPG, PNG, DOC, and DOCX files are allowed." });
                }
                var baseLocation = @"D:\Invoice_Type";
                if (!Directory.Exists(baseLocation))
                {
                    try
                    {
                        Directory.CreateDirectory(baseLocation);
                    }
                    catch (Exception ex)
                    {
                        _SQL_DB.ExceptionLogs($"Failed to create directory: {ex.Message}");
                        return StatusCode(500, new { success = false, message = "Failed to create directory for saving invoices." });
                    }
                }
                var uniqueFileName = $"{AssetCode.Replace("/", "-")}_{InvoiceType}_{Guid.NewGuid().ToString("N")[..8]}{fileExtension}";
                var fileLocation = Path.Combine(baseLocation, uniqueFileName);
                try
                {
                    using (var stream = new FileStream(fileLocation, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }
                }
                catch (Exception ex)
                {
                    _SQL_DB.ExceptionLogs($"Failed to save file: {ex.Message}");
                    return StatusCode(500, new { success = false, message = "Failed to save the uploaded file." });
                }
                var invoiceEntity = new DashboardReportsEntity
                {
                    AssetCoode = AssetCode,
                    InvoiceType = InvoiceType,
                    ImageURL = fileLocation,
                };
                var result = await _dashboardReports.SaveInvoiceTypeWithIdAsync(invoiceEntity);
                if (result > 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Invoice uploaded and saved successfully.",
                        recordsAffected = result,
                        fileName = uniqueFileName
                    });
                }
                else
                {
                    if (System.IO.File.Exists(fileLocation))
                    {
                        System.IO.File.Delete(fileLocation);
                    }
                    return StatusCode(500, new { success = false, message = "Failed to save invoice details to database." });
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = "An error occurred while processing your request." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DowenloadInvoiceData([FromBody] DashboardReportsEntity Entity)
        {
            try
            {
                DataTable Result = await _dashboardReports.BindInvoiceDetails(Entity);
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Asset Details");
                    worksheet.Cell(1, 1).InsertTable(Result);
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        stream.Position = 0;
                        return File(stream.ToArray(), $"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"{Entity.AssetType}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = "An error occurred while fetching Report Details" });
            }
        }
    }
}


