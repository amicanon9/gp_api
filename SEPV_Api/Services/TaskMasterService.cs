using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using PMS_api.Services;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    #region Interface
    public interface ITaskMasterService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        public int RoleId { get; set; }
        public List<TaskMasterView> GetAllData();
        public void InsertData(TaskMasterView data);
        public string EditData(int id, TaskMasterView data);
        public string DeleteData(int id);
    }
    #endregion

    #region View Models
    public class TaskMasterView
    {
        public int id { get; set; }
        public short book_id { get; set; }
        public string task_name { get; set; }
        public string description { get; set; }
        public string category { get; set; } // 程式, 美工
        public string priority { get; set; } // 一般, 緊急
        public string status { get; set; }   // 待辦, 測試, 審核, 完成
        public DateTime? create_at { get; set; }
        public DateTime? close_date { get; set; }

        // 自動從 FileProcessorService 抓取的檔案清單
        public List<FileInfoView> task_images { get; set; } = new List<FileInfoView>();
    }
    #endregion

    #region Service Implementation
    public class TaskMasterService : ITaskMasterService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        public int RoleId { get; set; }

        private readonly PMSContext _PMSContext;
        private readonly IFileProcessorService _fileService;

        public TaskMasterService(PMSContext PMSContext, IFileProcessorService fileService)
        {
            _PMSContext = PMSContext;
            _fileService = fileService;
        }

        /// <summary>
        /// 取得所有任務，並整合實體路徑下的圖片清單
        /// </summary>
        public List<TaskMasterView> GetAllData()
        {
            // 權限過濾：僅抓取該帳簿 (BookId) 的任務
            var query = _PMSContext.TaskMaster
                .Where(p => p.BookId == BookId)
                .AsQueryable();

            var dataList = query.Select(t => new TaskMasterView
            {
                id = t.Id,
                book_id = t.BookId,
                task_name = t.TaskName,
                description = t.Description,
                category = t.Category,
                priority = t.Priority,
                status = t.Status,
                close_date = t.CloseDate,
                create_at = t.CreatedAt
            }).ToList();

            // 遍歷每一筆 Task，去實體資料夾掃描檔案
            foreach (var item in dataList)
            {
                // 這裡傳入 "task" 作為 category 名稱，對應 C:\pms_files\task\{id}\
                item.task_images = _fileService.GetDataNameList(item.id, "task");
            }

            return dataList;
        }

        /// <summary>
        /// 新增任務文字資料
        /// </summary>
        public void InsertData(TaskMasterView viewModel)
        {
            var newData = new TaskMaster
            {
                BookId = BookId,
                TaskName = viewModel.task_name,
                Description = viewModel.description,
                Category = viewModel.category,
                Priority = viewModel.priority,
                Status = viewModel.status,
                CloseDate = viewModel.close_date,
                CreatedAt = DateTime.Now
            };

            _PMSContext.TaskMaster.Add(newData);
            _PMSContext.SaveChanges();

            // 將資料庫生成的 ID 回填，供後續上傳圖片使用
            viewModel.id = newData.Id;
        }

        /// <summary>
        /// 修改任務文字資料
        /// </summary>
        public string EditData(int id, TaskMasterView viewModel)
        {
            var data = _PMSContext.TaskMaster.FirstOrDefault(p => p.Id == id);
            if (data == null) return "NotFound";

            try
            {
                data.TaskName = viewModel.task_name;
                data.Description = viewModel.description;
                data.Category = viewModel.category;
                data.Priority = viewModel.priority;
                data.Status = viewModel.status;
                data.CloseDate = viewModel.close_date;
                data.UpdatedAt = DateTime.Now;

                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// 刪除任務
        /// </summary>
        public string DeleteData(int id)
        {
            var data = _PMSContext.TaskMaster.Find(id);
            if (data == null) return "NotFound";

            try
            {
                _PMSContext.TaskMaster.Remove(data);
                _PMSContext.SaveChanges();

                // 註：資料夾內的實體檔案建議在 Controller 呼叫 _fileService 進行後續清理
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
    #endregion
}