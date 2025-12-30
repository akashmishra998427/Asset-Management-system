using AssetManagement_DataAccess;
using AssetManagement_DataAccess.Reports;
using AssetManagement_EntityClass;
using AssetManagement_EntityClass.Operations;
using Assets_Management.Utilities;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QRCoder;
using System.Data;
using System.Drawing;

namespace Assets_Management.Controllers
{
    public class AssetsController : Controller
    {
        private readonly ApiConnect apiConnect;
        private readonly IConfiguration configuration;
        private readonly AssetDetailsAndReports _Detail;
        private readonly SQL_DB _SQL_DB;
        private readonly AssetManagerFactory Factory;
        private readonly ExcelHelper Excel;
        private readonly FilePathProvider filePathProvider;

        public AssetsController(ApiConnect apiConnect, IConfiguration configuration, AssetDetailsAndReports Detail,
            SQL_DB sQL_DB, AssetManagerFactory factory, ExcelHelper helper, FilePathProvider Provider)
        {
            this.apiConnect = apiConnect;
            this.configuration = configuration;
            this._Detail = Detail;
            _SQL_DB = sQL_DB;
            Factory = factory;
            Excel = helper;
            filePathProvider = Provider;
        }

        public IActionResult genrateAssetQR()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult Audit_PC_Data()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult transferAssets()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult invoiceEntry()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult GENERATEQRPDF()
        {
            return View();
        }

        public IActionResult Update(string? ID)
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult ScrapAsset()
        {
            return View();
        }

        public IActionResult RegisterAsset()
        {
            return View();
        }

        public IActionResult AddMasters()
        {
            return View();
        }

        public IActionResult Licence()
        {
            return View();
        }

        [HttpPost]
        public IActionResult genrateAssetQR([FromBody] QR_CodesEntity CheckAassetData)
        {
            foreach (var item in CheckAassetData.GeneratedQRCodeList)
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                string code = $"https://support.richagroup.com/CallLog/AddCallLogs?AssetCode={item.Asset_Code.Replace("/", "-")}&assetType={item.Asset_Type}";
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap qrBitmap = qrCode.GetGraphic(20))
                {
                    string base64 = Convert.ToBase64String(BitmapToByteArray(qrBitmap));
                    item.ImageURL = "data:image/png;base64," + base64;
                }
            }
            var result = CheckAassetData.GeneratedQRCodeList.ToList();
            return Json(result);
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadScrapImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                return BadRequest("No File Selected");
            }
            var UploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ScrapedImage");
            if (!Directory.Exists(UploadPath))
            {
                Directory.CreateDirectory(UploadPath);
            }
            var FileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var FilePath = Path.Combine(UploadPath, FileName);
            using (var Stream = new FileStream(FilePath, FileMode.Create))
            {
                await image.CopyToAsync(Stream);
            }
            return Ok(new { Message = "Image Uploaded Successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> AssetUpdation()
        {
            DataTable Result = await _Detail.AssetUpdations();

            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(Response);
            }
            return NotFound(new { success = false, message = "Data Not found." });
        }

        [HttpPost]
        public async Task<IActionResult> AssetDetalUpdation([FromBody] AssetEntity Entity)
        {
            var parameters = new Dictionary<string, object>
            {
                { "@ID",Entity.ID}
            };

            DataTable Result = await _Detail.BindAssetDetals(parameters);
            if (Result.Rows.Count > 0)
            {
                var Response = JsonConvert.SerializeObject(Result);
                return Ok(new
                {
                    Data = Response,
                });
            }
            return NotFound(new { success = false, message = "Data Not found." });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssetDetails([FromBody] AssetEntity Entity)
        {
            try
            {
                int Result = await _Detail.UpdateAssetDetal(Entity);
                if (Result > 0)
                {
                    return Ok(new { message = "Asset Detail Updated Successfully" });
                }
                else
                {
                    return NotFound(new { message = "No Assets Found To Update" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindAssetType()
        {
            try
            {
                DataTable Result = await _Detail.BindDepartment();
                if (Result != null && Result.Rows.Count > 0)
                {
                    var response = JsonConvert.SerializeObject(Result);
                    return Ok(response);
                }
                else
                {
                    return NotFound("No Asset Type Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddMaster([FromBody] AssetManagerEntity entity)
        {
            try
            {
                if (entity == null)
                {
                    return BadRequest(new { Success = false, Message = "Invalid data provided" });
                }

                if (string.IsNullOrWhiteSpace(entity.Asset_Name))
                {
                    return BadRequest(new { Success = false, Message = "Asset name is required" });
                }

                if (_Detail == null)
                {
                    return StatusCode(500, new { Success = false, Message = "Service not initialized" });
                }

                int result = await _Detail.AddMaster(entity);

                if (result > 0)
                {
                    return Ok(new { Success = true, Message = "Asset registered successfully", Data = result });
                }
                else
                {
                    return BadRequest(new { Success = false, Message = "Failed to register master" });
                }
            }
            catch (ArgumentNullException ex)
            {
                _SQL_DB.ExceptionLogs($"Null argument: {ex.Message}");
                return BadRequest(new { Success = false, Message = "Required data is missing" });
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssetRegistration([FromBody] AssetManagerEntity Entity)
        {
            if (string.IsNullOrEmpty(Entity.CPU_ASSET_CODE))
            {
                return BadRequest("Please Enter AsetCode");
            }
            if (string.IsNullOrEmpty(Entity.Asset_Type))
            {
                return BadRequest("Please Enter Asset Type");
            }
            if (string.IsNullOrEmpty(Entity.MAKE))
            {
                return BadRequest("Please Enter Make");
            }

            var Result = Factory.GetManager(Entity);
            var Response = await Result.AddAsset(Entity);

            if (Response > 0)
            {
                return Ok(new { Success = true, Message = "Asset registered successfully", Data = Response });
            }
            else if (Response == -1)
            {
                return Ok(new { Success = false, Message = "Duplicate record found. Asset with this code already exists." });
            }
            else
            {
                return Ok(new { Success = false, Message = "Failed to register asset" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindPuchaseUnit()
        {
            DataTable Result = await _Detail.BindPuchaseUnit();
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

        [HttpPost]
        public async Task<IActionResult> BindMakeAndModel([FromBody] AssetManagerEntity Entity)
        {
            try
            {
                DataTable Result = await _Detail.BuindMakeAndModel(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Res = JsonConvert.SerializeObject(Result);
                    return Ok(Res);
                }
                else
                {
                    return Ok("[]");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAsset([FromBody] AssetManagerEntity Entity)
        {
            try
            {
                int Result = await _Detail.UpdateAssetAsync(Entity);
                if (Result > 0)
                {

                    return Ok("Asset Upadated Successfuly");
                }
                else
                {
                    return Ok("[]");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStorageData([FromBody] AssetManagerEntity Entity)
        {
            try
            {
                DataTable Result = await _Detail.GetStorageData(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Storage Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindDetailedData(string UserName, string ScrapTransfer, string Location)
        {
            try
            {
                DataTable Result = await _Detail.BindDetailedData(UserName, ScrapTransfer, Location);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No Storage Data Found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> TransferAsset([FromBody] AssetManagerEntity Entity)
        {
            try
            {
                int result = await _Detail.TransferAsset(Entity);
                if (result > 0)
                {
                    return Ok(new
                    {
                        Status = "SUCCESS",
                        Message = "Asset transferred successfully",
                        AssetCode = Entity.AssetCode
                    });
                }
                else
                {
                    DataTable Result = await _Detail.CheckAssetPresent(Entity.AssetCode);
                    if (Result?.Rows.Count == 0)
                    {
                        return NotFound(new
                        {
                            Status = "FAILURE",
                            Message = "Provided Asset Code is not Present in your System"
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            Status = "FAILURE",
                            Message = "Asset couldn't be transferred. Please check asset code or transfer criteria."
                        });
                    }
                }

            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.ToString());
                return StatusCode(500, new
                {
                    Status = "ERROR",
                    Message = "Internal server error occurred while transferring asset.",
                    Error = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadLicenceBulk(IFormFile ExcelSheet)
        {
            try
            {
                if (ExcelSheet == null || ExcelSheet.Length == 0)
                {
                    return StatusCode(400, new { success = false, message = "No Excel file uploaded. Please upload a file first." });
                }

                var dt = await Excel.ProcessExcelFile(ExcelSheet, "Uploads/InvoiceExcel");
                if (dt.Rows.Count > 0)
                {
                    bool IsSuccess = await _Detail.UploadLicenceBulk(dt);
                    if (IsSuccess)
                    {
                        return Ok(new
                        {
                            success = true,
                            message = "File uploaded and data processed successfully.",
                            fileName = "uniqueFileName",
                            filePath = $"/Uploads/{"uniqueFileName"}"
                        });
                    }
                    else
                    {
                        return StatusCode(500, new { success = false, message = "Failed to process data from the Excel file." });
                    }

                }
                else
                {
                    return StatusCode(400, new { success = false, message = "The uploaded file is empty or has an invalid format." });
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message);
                return StatusCode(500, new { success = false, message = "An error occurred while uploading the file." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RegisterLicense([FromForm] LicenseEntity Entity, IFormFile InvoiceFile)
        {
            try
            {
                if (InvoiceFile == null || InvoiceFile.Length == 0)
                {
                    return BadRequest(new { Success = false, Message = "Invoice file is required." });
                }

                var uploadResult = await filePathProvider.SaveFileAsync(InvoiceFile, "uploads/invoices");

                Entity.InvoiceFileName = uploadResult.FileName;
                Entity.InvoiceFilePath = uploadResult.FilePath;

                bool isSuccess = await _Detail.RegisterLicense(Entity);

                if (isSuccess)
                {
                    return Ok(new { Success = true, Message = "License registered successfully" });
                }
                else
                {
                    try
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Entity.InvoiceFilePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                    catch (Exception fileEx)
                    {
                        _SQL_DB.ExceptionLogs("File cleanup error: " + fileEx.Message);
                    }

                    return BadRequest(new { Success = false, Message = "Failed to register license" });
                }
            }
            catch (ArgumentNullException ex)
            {
                _SQL_DB.ExceptionLogs($"Null argument: {ex.Message}");
                return BadRequest(new { Success = false, Message = "Required data is missing" });
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message);

                try
                {
                    if (InvoiceFile != null && !string.IsNullOrEmpty(Entity?.InvoiceFilePath))
                    {
                        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", Entity.InvoiceFilePath.TrimStart('/'));
                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }
                }
                catch (Exception fileEx)
                {
                    _SQL_DB.ExceptionLogs("File cleanup error: " + fileEx.Message);
                }

                return StatusCode(500, new { Success = false, Message = "Internal server error", Error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AssetTracDetails(string AssetCode)
        {
            DataTable Result = await _Detail.AssetTracDetails(AssetCode);
            if (Result.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(Result);
                return Ok(res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "Data Not Found" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetCodeDetails(string AssetType)
        {
            DataTable Result = await _Detail.GetAssetCodeDetails(AssetType);
            if (Result.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(Result);
                return Ok(res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "Data Not Found" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAssetType()
        {
            DataTable Result = await _Detail.GetAssetType();
            if (Result.Rows.Count > 0)
            {
                var Res = JsonConvert.SerializeObject(Result);
                return Ok(Res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "Asset type not found" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindDetailsData(string AssetType)
        {
            DataTable dt = await _Detail.BindDetailsData(AssetType);
            if (dt.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(dt);
                return Ok(res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "No Asset found" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> RetriveDataFromAssetCode(string assetCode)
        {
            DataTable dt = await _Detail.RetriveDataFromAssetCode(assetCode);
            if (dt.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(dt);
                return Ok(new { success = true, Data = res, message = "Data retrived successfully" });
            }
            else
            {
                return StatusCode(404, new { success = false, message = "No Asset found" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindSoftwareCategory(string SoftwareType)
        {
            DataTable dt = await _Detail.BindSoftwareCategory(SoftwareType);
            if (dt.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(dt);
                return Ok(res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "No Asset found" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAssets([FromBody] AssetManagerEntity Entity)
        {
            var Result = Factory.GetManager(Entity);
            var Response = await Result.UpdateAssetDetails(Entity);

            if (Response > 0)
            {
                return Ok(new { Success = true, Message = "Asset details updated successfully", Data = Response });
            }
            else
            {
                return Ok(new { Success = false, Message = "Failed to update asset details" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> BindSuggestions(string Type)
        {
            DataTable dt = await _Detail.BindSuggestions(Type);
            if (dt.Rows.Count > 0)
            {
                var res = JsonConvert.SerializeObject(dt);
                return Ok(res);
            }
            else
            {
                return StatusCode(404, new { success = false, message = "No Asset found" });
            }
        }

         #region Upload Receiving start

 public IActionResult UploadReceiving()
 {
     return View();
 }

 [HttpPost]
 public async Task<IActionResult> UploadReceiving([FromForm] string AssetCode, [FromForm] IFormFile Images, [FromForm] string Category)
 {
     if (string.IsNullOrEmpty(AssetCode))
     {
         return BadRequest("To Save Receiving Asset Code Is Required");
     }

     if (Images == null || Images.Length == 0)
     {
         return BadRequest("Please Upload The Receiving Image");
     }

     var uploadLocation = Path.Combine(_env.WebRootPath, "GIMS_AttachedDocument", "Assets", Category, AssetCode.Replace("/", "-"));

     if (!Directory.Exists(uploadLocation))
     {
         Directory.CreateDirectory(uploadLocation);
     }

     var fileExtension = Path.GetExtension(Images.FileName);

     int nextFileNumber = Directory.GetFiles(uploadLocation).Length + 1;

     var finalFileName = $"{nextFileNumber}{fileExtension}";
     var filePath = Path.Combine(uploadLocation, finalFileName);

     using (var stream = new FileStream(filePath, FileMode.Create))
     {
         await Images.CopyToAsync(stream);
     }

     return Ok(new { Message = "Image uploaded successfully", Url = filePath, AssetCode = AssetCode });
 }

 [HttpGet]
 public IActionResult GetUploadedReceivingImage(string AssetCode, string Category)
 {
     if (string.IsNullOrWhiteSpace(AssetCode))
     {
         return BadRequest(new { success = false, message = "AssetCode is required" });
     }

     var folderPath = Path.Combine(_env.WebRootPath, "GIMS_AttachedDocument", "Assets", Category, AssetCode.Replace("/", "-"));

     if (!Directory.Exists(folderPath))
     {
         return Ok(new { success = true, data = new List<object>() });
     }

     var files = Directory.GetFiles(folderPath)
         .Select((file, index) =>
         {
             var fileName = Path.GetFileName(file);
             return new
             {
                 id = index + 1,
                 imageName = fileName,
                 imageUrl = $"{Request.Scheme}://{Request.Host}/GIMS_AttachedDocument/Assets/{Category}/{AssetCode.Replace("/", "-")}/{fileName}",
                 uploadDate = System.IO.File.GetCreationTime(file),
                 category = Category
             };
         }).ToList();

     return Ok(new { success = true, data = files });
 }

 #endregion Upload Receiving End
        
    }
}

