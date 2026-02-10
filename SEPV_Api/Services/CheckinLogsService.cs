using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface ICheckinLogsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        List<CheckinLogsView> GetDataById(int id);
        void InsertData(List<CheckinLogsView> viewModelList);
        string DeleteData(int id);
        string EditData(int id, CheckinLogsView data);
        public List<CheckinLogsView> GetAllData();
    }

    public class CheckinLogsView
    {
        public int id { get; set; }
        public int book_id { get; set; }
        public int? project_id { get; set; }
        public string type { get; set; }           // 讓前端根據 type 決定去哪張 List 找名稱
        public int? user_id { get; set; }
        public DateTime checkin_time { get; set; }
        public DateTime? fake_time { get; set; }
        public string mode { get; set; }
        public string status { get; set; }
        public DateTime? created_at { get; set; }
        public int? work_percentage { get; set; }
    }

    public class CheckinLogsService : ICheckinLogsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        private readonly PMSContext _PMSContext;

        public CheckinLogsService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<CheckinLogsView> GetAllData()
        {
            return _PMSContext.CheckinLogs
                 .OrderByDescending(t => t.CheckinTime)
                 .Select(t => new CheckinLogsView
                 {
                     id = t.Id,
                     project_id = t.ProjectId,
                     type = t.Type,
                     user_id = t.UserId,
                     checkin_time = t.CheckinTime,
                     mode = t.Mode,
                     status = t.Status,
                     created_at = t.CreatedAt
                 }).ToList();
        }

        public List<CheckinLogsView> GetDataById(int id)
        {
            DateTime oneMonthAgo = DateTime.Now.AddMonths(-1);

            return _PMSContext.CheckinLogs
                .Where(t => t.UserId == UserId && t.BookId == BookId && t.CheckinTime >= oneMonthAgo)
                .OrderByDescending(t => t.CheckinTime)
                .Select(t => new CheckinLogsView
                {
                    id = t.Id,
                    project_id = t.ProjectId,
                    type = t.Type,
                    book_id = t.BookId,
                    user_id = t.UserId,
                    checkin_time = t.CheckinTime,
                    fake_time = t.FakeTime,
                    mode = t.Mode,
                    status = t.Status,
                    created_at = t.CreatedAt,
                    work_percentage = t.WorkPercentage
                }).Take(60).ToList();
        }

        public void InsertData(List<CheckinLogsView> viewModelList)
        {
            if (viewModelList == null || !viewModelList.Any()) return;

            var firstItem = viewModelList.First();
            DateTime startOfToday = firstItem.checkin_time.Date;
            DateTime startOfNextDay = startOfToday.AddDays(1);

            string finalStatus = firstItem.status;

            if (firstItem.mode == "normal")
            {
                bool hasRecordToday = _PMSContext.CheckinLogs
                    .Any(t => t.UserId == UserId && t.BookId == BookId &&
                              t.CheckinTime >= startOfToday &&
                              t.CheckinTime < startOfNextDay);

                finalStatus = hasRecordToday ? "下班" : "上班";
            }

            foreach (var item in viewModelList)
            {
                var data = new CheckinLogs
                {
                    UserId = UserId,
                    BookId = BookId,
                    ProjectId = item.project_id,
                    Type = item.type, // 這裡存入前端傳過來的專案類型
                    CheckinTime = item.checkin_time,
                    FakeTime = item.fake_time,
                    Mode = item.mode,
                    Status = finalStatus,
                    CreatedAt = DateTime.Now,
                    WorkPercentage = item.work_percentage,
                };
                _PMSContext.CheckinLogs.Add(data);
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
            data.Type = viewModel.type;
            data.CheckinTime = viewModel.checkin_time;
            data.Mode = viewModel.mode;
            data.Status = viewModel.status;
            data.WorkPercentage = viewModel.work_percentage;

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