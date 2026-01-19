using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS; // 確保包含您產生的 CheckinLogs Entity
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface ICheckinLogsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        List<CheckinLogsView> GetDataById(int id);
        void InsertData(CheckinLogsView data);
        string DeleteData(int id);
        string EditData(int id, CheckinLogsView data);
        public List<CheckinLogsView> GetAllData();
    }

    public class CheckinLogsView
    {
        public int id { get; set; }
        // 新增時前端會傳一組 ID 陣列
        public List<int> project_ids { get; set; }
        public int project_id { get; set; }
        public string project_name { get; set; } // 補回這個欄位
        public int? user_id { get; set; }
        public DateTime checkin_time { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public DateTime? created_at { get; set; }
    }
    public class CheckinLogsService : ICheckinLogsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly PMSContext _PMSContext;

        public CheckinLogsService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<CheckinLogsView> GetAllData()
        {
            var query = _PMSContext.CheckinLogs
                 .OrderByDescending(t => t.CheckinTime)
                 .Select(t => new CheckinLogsView
                 {
                     id = t.Id,
                     project_id = t.ProjectId,
                     user_id = t.UserId,
                     checkin_time = t.CheckinTime,
                     mode = t.Mode,
                     status = t.Status,
                     created_at = t.CreatedAt
                 });

            return query.ToList();
        }

        public List<CheckinLogsView> GetDataById(int id)
        {
            // 權限檢查邏輯統一
            var role = _PMSContext.LoginInfoRoles.Where(a => a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var project = _PMSContext.ProjectPlm.FirstOrDefault(p => p.Id == id);

            if (project == null) return new List<CheckinLogsView>();

            if (role != null && !role.IsAdmin && project.RoleId != RoleId)
            {
                return new List<CheckinLogsView>();
            }

            var query = _PMSContext.CheckinLogs
                .Where(t => t.UserId == id)
                .OrderByDescending(t => t.CheckinTime)
               .Select(t => new CheckinLogsView
               {
                   id = t.Id,
                   project_id = t.ProjectId,
                   project_name = _PMSContext.ProjectPlm
                                .Where(p => p.Id == t.ProjectId)
                                .Select(p => p.Customer.Name)
                                .FirstOrDefault(),
                   user_id = t.UserId,
                   checkin_time = t.CheckinTime,
                   mode = t.Mode,
                   status = t.Status,
                   created_at = t.CreatedAt
               });

            return query.ToList();
        }

        public void InsertData(CheckinLogsView viewModel)
        {
            // 1. 取得當前要判斷的日期（不含時間）
            // 取得今天的起始點 (00:00:00)
            DateTime startOfToday = viewModel.checkin_time.Date;
            // 取得明天的起始點 (00:00:00)
            DateTime startOfNextDay = startOfToday.AddDays(1);

            string finalStatus = viewModel.status;

            if (viewModel.mode == "normal")
            {
                // 檢查當天是否有紀錄： >= 今天 00:00 AND < 明天 00:00
                bool hasRecordToday = _PMSContext.CheckinLogs
                    .Any(t => t.UserId == UserId &&
                              t.CheckinTime >= startOfToday &&
                              t.CheckinTime < startOfNextDay);

                finalStatus = hasRecordToday ? "下班" : "上班";
            }

            // 3. 批次新增
            if (viewModel.project_ids != null && viewModel.project_ids.Any())
            {
                foreach (var pid in viewModel.project_ids)
                {
                    var data = new CheckinLogs
                    {
                        UserId = UserId,
                        ProjectId = pid,
                        CheckinTime = viewModel.checkin_time,
                        Mode = viewModel.mode,
                        Status = finalStatus, // 使用判斷後的狀態
                        CreatedAt = DateTime.Now
                    };
                    _PMSContext.CheckinLogs.Add(data);
                }
            }
            _PMSContext.SaveChanges();
        }
        public string DeleteData(int id)
        {
            var data = _PMSContext.CheckinLogs.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.CheckinLogs.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        public string EditData(int id, CheckinLogsView viewModel)
        {
            var data = _PMSContext.CheckinLogs.Find(id);
            if (data == null) return "NotFound";

            data.ProjectId = viewModel.project_id;
            data.CheckinTime = viewModel.checkin_time;
            data.Mode = viewModel.mode;
            data.Status = viewModel.status;

            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }
    }
}