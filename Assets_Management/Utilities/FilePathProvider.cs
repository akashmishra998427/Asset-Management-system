using AssetManagement_EntityClass;

namespace Assets_Management.Utilities
{
    public class FilePathProvider
    {
        /// <summary>
        /// Saves the uploaded file and returns file name + relative path.
        /// </summary>
        /// <param name="file">The file to upload.</param>
        /// <param name="uploadFolder">Folder inside wwwroot (e.g. "Uploads").</param>
        /// <returns>FileUploadResult with FileName & FilePath</returns>
        public async Task<FileUploadResultEntity> SaveFileAsync(IFormFile file, string uploadFolder)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file. Please upload a valid file.");
            }
            string rootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", uploadFolder);
            if (!Directory.Exists(rootPath))
            {
                Directory.CreateDirectory(rootPath);
            }

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(rootPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return new FileUploadResultEntity
            {
                FileName = uniqueFileName,
                FilePath = $"/{uploadFolder}/{uniqueFileName}"
            };
        }

        /// <summary>
        /// Fetch file name present in the folder
        /// </summary>
        /// <param name="filePath">Full relative path of the folder</param>
        /// <returns>Returns an array of a string containing the file name inside the provided folder path</returns>
        public string[] UploadFolder(string filePath)
        {
            if (Directory.Exists(filePath))
            {
                string[] files = Directory.GetFiles(filePath);
                string[] fileNames = files.Select(Path.GetFileNameWithoutExtension).ToArray();
                foreach (var name in fileNames)
                {
                    Console.WriteLine($"- {name}");
                }
                return fileNames;
            }
            else
            {
                return Array.Empty<string>();
            }
        }
    }
}
