using ClosedXML.Excel;
using System.Data;


namespace Assets_Management.Utilities
{
    public class ExcelHelper
    {

        private readonly FilePathProvider _fileUploadHelper;
        public ExcelHelper(FilePathProvider fileUploadHelper)
        {
            _fileUploadHelper = fileUploadHelper;
        }
        /// <summary>
        /// Uploads the Excel file using FileUploadHelper, 
        /// then reads it into a DataTable.
        /// </summary>
        public async Task<DataTable> ProcessExcelFile(IFormFile excelFile, string uploadFolder)
        {
            if (excelFile == null || excelFile.Length == 0)
                throw new ArgumentException("Invalid file. Please upload a valid Excel file.");

            var uploadResult = await _fileUploadHelper.SaveFileAsync(excelFile, uploadFolder);

            DataTable dt = new DataTable();
            string fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadResult.FilePath.TrimStart('/'));

            using (var workbook = new XLWorkbook(fullPath))
            {
                var worksheet = workbook.Worksheet(1);
                bool isFirstRow = true;

                foreach (var row in worksheet.RowsUsed())
                {
                    if (isFirstRow)
                    {
                        foreach (var cell in row.Cells())
                            dt.Columns.Add(cell.Value.ToString());
                        isFirstRow = false;
                    }
                    else
                    {
                        var dataRow = dt.NewRow();
                        int i = 0;
                        foreach (var cell in row.Cells())
                        {
                            dataRow[i] = cell.Value.ToString();
                            i++;
                        }
                        dt.Rows.Add(dataRow);
                    }
                }
            }

            return dt;
        }
    }
}
