using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS; // 確保包含您產生的 WeeklyReportPlm Entity
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface IWeeklyReportPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        List<WeeklyReportPlmView> GetDataById(int id);
        void InsertData(WeeklyReportPlmView data);
        string DeleteData(int id);
        string EditData(int id, WeeklyReportPlmView data);
        public List<WeeklyReportPlmView> GetAllData();
    }

    public class WeeklyReportPlmView
    {
        public int id { get; set; }
        public int project_id { get; set; }
        public string project_name { get; set; } // 關聯顯示用
        public int year { get; set; }
        public int week { get; set; }
        public string content { get; set; }
        public string content_detail { get; set; }
        public string ags_status { get; set; }
        public string ags_description { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class WeeklyReportPlmService : IWeeklyReportPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly PMSContext _PMSContext;

        public WeeklyReportPlmService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }
        public List<WeeklyReportPlmView> GetAllData()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.WeeklyReportPlm
                 .OrderByDescending(t => t.Year)
                 .ThenByDescending(t => t.Week)
                 .Select(t => new WeeklyReportPlmView
                 {
                     id = t.Id,
                     project_id = t.ProjectId,
                     year = t.Year,
                     week = t.Week,
                     content = t.Content,
                     content_detail = t.ContentDetail,
                     ags_status = t.AgsStatus,
                     created_at = t.CreatedAt
                 });

          
            var allData = query.ToList();
            return allData;
        }
        public List<WeeklyReportPlmView> GetDataById(int id)
        {
            // 1. 安全檢查：先確認該專案是否屬於該 Role (或是 Admin)
            var role = _PMSContext.LoginInfoRoles.Where(a => a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var project = _PMSContext.ProjectPlm.FirstOrDefault(p => p.Id == id);

            if (project == null) return new List<WeeklyReportPlmView>(); // 專案不存在

            // 如果不是 Admin，且專案的 RoleId 與當前 User 不同，則不給看
            if (role != null && !role.IsAdmin && project.RoleId != RoleId)
            {
                return new List<WeeklyReportPlmView>();
            }

            // 2. 抓取該專案的所有週報，並依照年份、週次降冪排序（最新的在前）
            var query = _PMSContext.WeeklyReportPlm
                .Where(t => t.ProjectId == id)
                .OrderByDescending(t => t.Year)
                .ThenByDescending(t => t.Week)
                .Select(t => new WeeklyReportPlmView
                {
                    id = t.Id,
                    project_id = t.ProjectId,
                    year = t.Year,
                    week = t.Week,
                    content = t.Content,
                    content_detail = t.ContentDetail,
                    ags_status = t.AgsStatus,
                    created_at = t.CreatedAt
                });

            return query.ToList();
        }

        public void InsertData(WeeklyReportPlmView viewModel)
        {
            var data = new WeeklyReportPlm
            {
                ProjectId = viewModel.project_id,
                Year = viewModel.year,
                Week = viewModel.week,
                Content = viewModel.content,
                ContentDetail = viewModel.content_detail,
                AgsStatus = viewModel.ags_status,
                CreatedAt = DateTime.Now
            };

            _PMSContext.WeeklyReportPlm.Add(data);
            _PMSContext.SaveChanges();

            // 新增後，同步更新 ProjectPlm.AgsStatus
            SyncProjectAgsStatus(viewModel.project_id);
        }

        public string EditData(int id, WeeklyReportPlmView viewModel)
        {
            var data = _PMSContext.WeeklyReportPlm.Find(id);
            if (data == null) return "NotFound";

            data.Year = viewModel.year;
            data.Week = viewModel.week;
            data.Content = viewModel.content;
            data.UpdatedAt = DateTime.Now;
            data.ContentDetail = viewModel.content_detail;
            data.AgsStatus = viewModel.ags_status;

            try
            {
                _PMSContext.SaveChanges();

                // 編輯後，同步更新 ProjectPlm.AgsStatus
                SyncProjectAgsStatus(data.ProjectId);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        // 抽出共用邏輯：找最新週報並同步至 ProjectPlm
        private void SyncProjectAgsStatus(int projectId)
        {
            // 取得該專案最新一筆週報（依 Year 降冪、Week 降冪）
            var latestReport = _PMSContext.WeeklyReportPlm
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.Year)
                .ThenByDescending(t => t.Week)
                .FirstOrDefault();
            if (latestReport == null) return;

            var project = _PMSContext.ProjectPlm.Find(projectId);
            if (project == null) return;

            // 只有當狀態有變動才更新
            if (project.AgsStatus != latestReport.AgsStatus)
            {
                project.AgsStatus = latestReport.AgsStatus;

                // 根據新狀態記錄對應時間點
                var now = DateTime.Now;
                switch (latestReport.AgsStatus)
                {
                    case "A2": // Commit
                        project.CommitDate = now;
                        break;
                    case "A4": // BCD
                        project.BcdDate = now;
                        break;
                    case "A5": // Longshot
                        project.LongshotDate = now;
                        break;
                }

                _PMSContext.SaveChanges();
            }
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.WeeklyReportPlm.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.WeeklyReportPlm.Remove(data);
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