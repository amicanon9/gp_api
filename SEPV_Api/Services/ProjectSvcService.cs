using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gp_Api.Services
{
    #region Interface
    public interface IProjectSvcService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        public int RoleId { get; set; }
        public List<ProjectSvcView> GetAllData();
        public void InsertData(ProjectSvcView data);
        public string EditData(int id, ProjectSvcView data);
        public string DeleteData(int id);
    }
    #endregion

    #region View Models
    public class ProjectSvcView
    {
        public int id { get; set; }
        public short book_id { get; set; }
        public string project_name { get; set; }
        public decimal? contract_amount { get; set; }
        public int? project_manager_id { get; set; }
        public string project_manager_name { get; set; }

        public decimal? planned_days { get; set; }
        public decimal? actual_days { get; set; }
        public decimal? margin_percentage { get; set; }

        // 8 大里程碑
        public DateTime? sow_signed_date { get; set; }
        public DateTime? kickoff_date { get; set; }
        public DateTime? access_date { get; set; }
        public DateTime? define_date { get; set; }
        public DateTime? design_date { get; set; }
        public DateTime? uat_date { get; set; }
        public DateTime? go_live_date { get; set; }
        public DateTime? rollout_date { get; set; }

        // 統一出入格式：只使用 team_members
        public List<TeamMemberView> team_members { get; set; } = new List<TeamMemberView>();
    }

    public class TeamMemberView
    {
        public int id { get; set; }
        public string username { get; set; }
    }
    #endregion

    #region Service Implementation
    public class ProjectSvcService : IProjectSvcService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        public int RoleId { get; set; }
        private readonly PMSContext _PMSContext;

        public ProjectSvcService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<ProjectSvcView> GetAllData()
        {
            // 透過角色關聯表確認 Admin 權限
            var currentRole = _PMSContext.LoginInfoRoles
                .Include(r => r.Role)
                .FirstOrDefault(r => r.Id == RoleId);

            var query = _PMSContext.ProjectSvc
                .Include(p => p.ProjectManager)
                .Include(p => p.ProjectSvcTeam)
                    .ThenInclude(t => t.User)
                .AsQueryable();

            // 權限過濾：非管理員只能看自己 BookId 的專案
            if (currentRole == null || !currentRole.Role.IsAdmin)
            {
                query = query.Where(p => p.BookId == BookId);
            }

            return query.Select(t => new ProjectSvcView
            {
                id = t.Id,
                book_id = t.BookId,
                project_name = t.ProjectName,
                contract_amount = t.ContractAmount,
                project_manager_id = t.ProjectManagerId,
                project_manager_name = t.ProjectManager != null ? t.ProjectManager.Username : "",
                planned_days = t.PlannedDays,
                actual_days = t.ActualDays,
                margin_percentage = t.MarginPercentage,
                sow_signed_date = t.SowSignedDate,
                kickoff_date = t.KickoffDate,
                access_date = t.AccessDate,
                define_date = t.DefineDate,
                design_date = t.DesignDate,
                uat_date = t.UatDate,
                go_live_date = t.GoLiveDate,
                rollout_date = t.RolloutDate,

                // 傳出格式：將資料庫的 Username 對應到 View 的 name
                team_members = t.ProjectSvcTeam.Select(mt => new TeamMemberView
                {
                    id = mt.UserId,
                    username = mt.User != null ? mt.User.Username : "Unknown"
                }).ToList()
            }).ToList();
        }

        public void InsertData(ProjectSvcView viewModel)
        {
            using (var transaction = _PMSContext.Database.BeginTransaction())
            {
                try
                {
                    var newData = new ProjectSvc
                    {
                        BookId = BookId,
                        ProjectName = viewModel.project_name,
                        ContractAmount = viewModel.contract_amount,
                        ProjectManagerId = viewModel.project_manager_id,
                        PlannedDays = viewModel.planned_days,
                        ActualDays = viewModel.actual_days,
                        MarginPercentage = viewModel.margin_percentage,
                        SowSignedDate = viewModel.sow_signed_date,
                        KickoffDate = viewModel.kickoff_date,
                        AccessDate = viewModel.access_date,
                        DefineDate = viewModel.define_date,
                        DesignDate = viewModel.design_date,
                        UatDate = viewModel.uat_date,
                        GoLiveDate = viewModel.go_live_date,
                        RolloutDate = viewModel.rollout_date,
                        CreatedAt = DateTime.Now,
                    };

                    _PMSContext.ProjectSvc.Add(newData);
                    _PMSContext.SaveChanges();

                    // 處理成員同步
                    SyncTeamMembers(newData.Id, viewModel.team_members);

                    _PMSContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public string EditData(int id, ProjectSvcView viewModel)
        {
            var data = _PMSContext.ProjectSvc
                .Include(p => p.ProjectSvcTeam)
                .FirstOrDefault(p => p.Id == id);

            if (data == null) return "NotFound";

            using (var transaction = _PMSContext.Database.BeginTransaction())
            {
                try
                {
                    data.ProjectName = viewModel.project_name;
                    data.ContractAmount = viewModel.contract_amount;
                    data.ProjectManagerId = viewModel.project_manager_id;
                    data.PlannedDays = viewModel.planned_days;
                    data.ActualDays = viewModel.actual_days;
                    data.MarginPercentage = viewModel.margin_percentage;
                    data.SowSignedDate = viewModel.sow_signed_date;
                    data.KickoffDate = viewModel.kickoff_date;
                    data.AccessDate = viewModel.access_date;
                    data.DefineDate = viewModel.define_date;
                    data.DesignDate = viewModel.design_date;
                    data.UatDate = viewModel.uat_date;
                    data.GoLiveDate = viewModel.go_live_date;
                    data.RolloutDate = viewModel.rollout_date;
                    data.UpdatedAt = DateTime.Now;

                    // 多對多更新：先清空舊的，再重新同步
                    _PMSContext.ProjectSvcTeam.RemoveRange(data.ProjectSvcTeam);
                    SyncTeamMembers(id, viewModel.team_members);

                    _PMSContext.SaveChanges();
                    transaction.Commit();
                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return ex.Message;
                }
            }
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.ProjectSvc.Find(id);
            if (data == null) return "NotFound";
            try
            {
                _PMSContext.ProjectSvc.Remove(data);
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private void SyncTeamMembers(int projectId, List<TeamMemberView> teamInput)
        {
            if (teamInput == null || !teamInput.Any()) return;

            foreach (var member in teamInput)
            {
                // 因為你傳入的是 List<TeamMemberView>，Model Binder 會自動將 JSON 中的 id 填入 member.id
                if (member.id > 0)
                {
                    _PMSContext.ProjectSvcTeam.Add(new ProjectSvcTeam
                    {
                        ProjectSvcId = projectId,
                        UserId = member.id
                    });
                }
            }
        }
    }
    #endregion
}