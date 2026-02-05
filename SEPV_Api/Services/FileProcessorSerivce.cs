using Microsoft.AspNetCore.Http;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMS_api.Services
{
    public interface IFileProcessorService
    {
        public FileProcessorResult UploadData(List<IFormFile> files, int item_id, string category);
        public FileProcessorResult DeleteData(string file_name, int item_id, string category);
        public FileDownloadFormat DownloadData(string file_name, int item_id, string category);
        public List<FileInfoView> GetDataNameList(int item_id, string category);
    }

    #region View Models
    public class FileProcessorResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class FileDownloadFormat
    {
        public byte[] File_byte { get; set; }
        public string Option { get; set; }
        public string File_name { get; set; }
    }

    public class FileInfoView
    {
        public string name { get; set; }
        public long size { get; set; }
    }
    #endregion

    public class FileProcessorService : IFileProcessorService
    {
        private readonly PMSContext _pmsContext;
        private readonly string basePath = @"C:\pms_files\";

        public FileProcessorService(PMSContext pmsContext)
        {
            _pmsContext = pmsContext;
        }

        /// <summary>
        /// 檢查資料庫中是否存在該項目，避免孤兒檔案
        /// </summary>
        private bool IsTargetExist(int item_id, string category)
        {
            return category.ToLower() switch
            {
                "task" => _pmsContext.TaskMaster.Any(a => a.Id == item_id),
                _ => false
            };
        }

        public FileProcessorResult UploadData(List<IFormFile> files, int item_id, string category)
        {
            // 1. 驗證資料合法性
            if (!IsTargetExist(item_id, category))
                return new FileProcessorResult { Status = "NotFound", Message = $"無法在 {category} 中找到 ID 為 {item_id} 的資料物件" };

            // 2. 確定目錄結構：C:\pms_files\{category}\{item_id}
            string targetFolder = Path.Combine(basePath, category, item_id.ToString());

            if (!Directory.Exists(targetFolder))
                Directory.CreateDirectory(targetFolder);

            // 3. 限制單一項目檔案數量
            var dirInfo = new DirectoryInfo(targetFolder);
            if (dirInfo.GetFiles().Length >= 10)
                return new FileProcessorResult { Status = "BadRequest", Message = "該項目檔案數量已達上限(10)，請刪除舊檔後再上傳" };

            // 4. 過濾副檔名
            string[] allowExtension = { ".png", ".jpg", ".jpeg", ".gif", ".pdf", ".xlsx", ".docx" };
            var readyToSave = files.Where(f => allowExtension.Contains(Path.GetExtension(f.FileName).ToLower())).ToList();

            if (!readyToSave.Any())
                return new FileProcessorResult { Status = "BadRequest", Message = "未偵測到支援的檔案格式" };

            // 5. 儲存檔案
            foreach (var file in readyToSave)
            {
                if (file.Length > 0)
                {
                    // 使用原檔名（或視需求加時間戳），這裡保持原名以便前端對應，但建議加 GUID 預防重複
                    string fileName = file.FileName;
                    string filePath = Path.Combine(targetFolder, fileName);

                    // 如果檔案已存在，則先刪除或覆蓋
                    if (File.Exists(filePath)) File.Delete(filePath);

                    using var fileStream = new FileStream(filePath, FileMode.Create);
                    file.CopyTo(fileStream);
                }
            }

            return new FileProcessorResult { Status = "Ok", Message = $"成功上傳 {readyToSave.Count} 個檔案" };
        }

        public FileProcessorResult DeleteData(string file_name, int item_id, string category)
        {
            string targetFile = Path.Combine(basePath, category, item_id.ToString(), file_name);

            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
                return new FileProcessorResult { Status = "Ok", Message = "檔案已成功刪除" };
            }

            return new FileProcessorResult { Status = "NotFound", Message = "找不到欲刪除的檔案" };
        }

        public FileDownloadFormat DownloadData(string file_name, int item_id, string category)
        {
            string targetFile = Path.Combine(basePath, category, item_id.ToString(), file_name);

            if (!File.Exists(targetFile)) return null;

            byte[] fileBytes = File.ReadAllBytes(targetFile);
            return new FileDownloadFormat
            {
                File_byte = fileBytes,
                Option = "application/octet-stream",
                File_name = file_name
            };
        }

        public List<FileInfoView> GetDataNameList(int item_id, string category)
        {
            string targetFolder = Path.Combine(basePath, category, item_id.ToString());
            var result = new List<FileInfoView>();

            if (!Directory.Exists(targetFolder)) return result;

            var dirInfo = new DirectoryInfo(targetFolder);
            return dirInfo.GetFiles().Select(b => new FileInfoView
            {
                name = b.Name,
                size = b.Length
            }).ToList();
        }
    }
}