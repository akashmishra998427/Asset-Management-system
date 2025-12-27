using AssetManagement_DataAccess;
using AssetManagement_EntityClass.Assets;
using Assets_Management.Utilities;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using QRCoder;
using System.Data;
using System.Drawing;
using Rectangle = iTextSharp.text.Rectangle;

namespace Assets_Management.Controllers
{
    public class ProductionMachineController : Controller
    {
        private readonly ApiConnect apiConnect;
        private readonly ProductionMachine productionMachineDataAccess;
        private readonly SQL_DB _SQL_DB;
        private DataTable Result = new DataTable();

        public ProductionMachineController(ApiConnect apiConnect, ProductionMachine productionMachineDataAccess, SQL_DB sQL_DB)
        {
            this.apiConnect = apiConnect;
            this.productionMachineDataAccess = productionMachineDataAccess;
            _SQL_DB = sQL_DB;
        }

        public IActionResult MachineBarcodePrint()
        {
            ViewBag.ApiBasurl = apiConnect.CoreApiUrl();
            return View();
        }

        public IActionResult TransferMachine()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> TransferMachineList([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.GetmachineTransferList(Entity.CompCode);
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
        public async Task<IActionResult> TransferMachine([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                int Result = await productionMachineDataAccess.TransferMachine(Entity);
                if (Result == 0)
                {
                    return NotFound("There is Problem To transfer machine");
                }
                else
                {
                    return Ok("Machine Transffred Successfully");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> BindModelNo([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                var Result = await productionMachineDataAccess.BindModelNo(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Model No Not Available ");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetMachineList([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.GetMachineList(Entity);
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
        public async Task<IActionResult> ViewBarcodePdf([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                if (Entity.MachineID == null || !Entity.MachineID.Any())
                {
                    return BadRequest("No machine IDs provided");
                }
                var machineData = await productionMachineDataAccess.GetMachineDataForBarcode(Entity);
                if (machineData.Rows.Count > 0)
                {
                    byte[] pdfBytes = GenerateBarcodePdf(machineData);
                    return File(pdfBytes, "application/pdf", "MachineBarcode.pdf");
                }
                else
                {
                    return NotFound("No Record Found To Genrate QR Code");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private byte[] GenerateBarcodePdf(DataTable machineData)
        {
            using (var memoryStream = new MemoryStream())
            {
                Rectangle cardSize = new Rectangle(150f, 150f);
                Document document = new Document(cardSize, 0, 0, 0, 0);
                PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);
                document.Open();
                var FontStyle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                foreach (DataRow row in machineData.Rows)
                {
                    if (machineData.Rows.IndexOf(row) > 0)
                    {
                        document.NewPage();
                    }
                    PdfPTable MainTable = new PdfPTable(1);
                    PdfPCell ImageContentCell = new PdfPCell();
                    ImageContentCell.Border = Rectangle.NO_BORDER;
                    ImageContentCell.HorizontalAlignment = Element.ALIGN_CENTER;
                    ImageContentCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    Paragraph serialNo = new Paragraph($"{row["MachineSrNo"]}", FontStyle);
                    serialNo.Alignment = Element.ALIGN_CENTER;
                    ImageContentCell.AddElement(serialNo);
                    string responseText = $"https://support.richagroup.com/CallLog/AddCallLogs?AssetCode={row["MachineCode"]}&assetType=S";
                    byte[] qrCodeBytes = GenerateQRCodeBytes(responseText);
                    if (qrCodeBytes != null)
                    {
                        iTextSharp.text.Image qrImage = iTextSharp.text.Image.GetInstance(qrCodeBytes);
                        qrImage.ScaleAbsolute(130f, 130f);
                        qrImage.Alignment = Element.ALIGN_CENTER;
                        ImageContentCell.PaddingTop = 0f;
                        ImageContentCell.AddElement(qrImage);
                    }
                    else
                    {
                        Paragraph qrError = new Paragraph("QR Code could not be generated", FontStyle);
                        qrError.Alignment = Element.ALIGN_CENTER;
                        ImageContentCell.AddElement(qrError);
                    }
                    Chunk machineCodeChunk = new Chunk($"{row["MachineCode"]}", FontStyle);
                    Paragraph machineCodeParagraph = new Paragraph(machineCodeChunk);
                    machineCodeParagraph.Alignment = Element.ALIGN_CENTER;
                    machineCodeParagraph.SetLeading(0f, 0);
                    ImageContentCell.AddElement(machineCodeParagraph);
                    MainTable.AddCell(ImageContentCell);
                    document.Add(MainTable);
                }
                document.Close();
                return memoryStream.ToArray();
            }
        }


        public byte[] GenerateQRCodeBytes(string text)
        {
            try
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeGenerator.QRCode qrCode = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (Bitmap qrBitmap = qrCode.GetGraphic(30))
                {
                    return BitmapToByteArray(qrBitmap);
                }
            }
            catch
            {
                return null;
            }
        }

        private byte[] BitmapToByteArray(Bitmap bitmap)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    return stream.ToArray();
                }
            }
            catch (PlatformNotSupportedException ex)
            {
                _SQL_DB.ExceptionLogs($"Platform not supported: {ex.Message}");
                return Array.Empty<byte>();
            }
        }

        public IActionResult ViewMachineMaster()
        {
            return View();
        }

        public IActionResult MachineMaster()
        {
            ViewBag.ApiBasurl = "http://192.168.41.148/Vgems_WebApi/api";
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMachineMasterList([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.GetMachineMaster(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Machine detail not available");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPremises()
        {
            try
            {
                Result = await productionMachineDataAccess.GetPremises();
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

        [HttpGet]
        public async Task<IActionResult> GetCompCode()
        {
            try
            {
                Result = await productionMachineDataAccess.GetCompCode();
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

        [HttpGet]
        public async Task<IActionResult> DownloadExcel([FromServices] IWebHostEnvironment env)
        {
            string filePath = Path.Combine(env.WebRootPath, "ExcelTemplate", "ExcelTemplate.xlsx");

            if (System.IO.File.Exists(filePath))
            {
                byte[] fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                string fileName = Path.GetFileName(filePath);
                return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            else
            {
                return NotFound("File not found");
            }
        }

        [HttpPost]
        public async Task<IActionResult> downloadExcelData([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.downloadExcelData(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("No data available to Dowenload");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCode()
        {
            try
            {
                Result = await productionMachineDataAccess.GetCode();
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Code not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> SetMachineData([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.SetMachineData(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Code not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetAutoCompleteData([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.GetAutoCompleteData(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Code not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetLineNo([FromBody] ProductionMachineEntity Entity)
        {
            try
            {
                Result = await productionMachineDataAccess.GetLineNo(Entity);
                if (Result.Rows.Count > 0)
                {
                    var Response = JsonConvert.SerializeObject(Result);
                    return Ok(Response);
                }
                else
                {
                    return NotFound("Code not found");
                }
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageMachineData([FromBody] ProductionMachineEntity entity)
        {
            if (entity == null)
            {
                return BadRequest(new { success = false, message = "Invalid request: entity is null." });
            }
            if (string.IsNullOrWhiteSpace(entity.MachineSrNo))
            {
                return BadRequest(new { success = false, message = "Machine Serial Number is required." });
            }
            try
            {
                string result = await productionMachineDataAccess.ManageMachineData(entity);
                if (!string.IsNullOrWhiteSpace(result))
                {
                    if (result.Contains("SuccessFully", StringComparison.OrdinalIgnoreCase))
                    {
                        return Ok(new { success = true, message = "Operation completed successfully.", result = result });
                    }
                    else if (result.Contains("Already Exists", StringComparison.OrdinalIgnoreCase))
                    {
                        return Ok(new { success = false, message = "Machine already exists.", result = result });
                    }
                    else
                    {
                        return BadRequest(new { success = false, message = "Operation failed.", result = result });
                    }
                }
                return StatusCode(500, new { success = false, message = "No response received from the database procedure." });
            }
            catch (ApplicationException appEx) when (appEx.InnerException is SqlException)
            {
                _SQL_DB.ExceptionLogs($"SQL Error in ManageMachineData: {appEx.InnerException.Message}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "Database error occurred while processing the request."
                });
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs($"Unexpected error in ManageMachineData: {ex.Message}, StackTrace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    success = false,
                    message = "An unexpected error occurred while processing the request."
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ManageExcelData([FromBody] List<ProductionMachineEntity> ManageExcelData, [FromQuery] string status, [FromQuery] string username)
        {
            try
            {
                if (ManageExcelData == null || ManageExcelData.Count == 0)
                {
                    return Json(new { success = false, message = "No data received" });
                }

                string result = await productionMachineDataAccess.ManageExcelData(ManageExcelData, status, username);
                return Json(new { success = true, message = result });
            }
            catch (Exception ex)
            {
                _SQL_DB.ExceptionLogs(ex.Message.ToString());
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
