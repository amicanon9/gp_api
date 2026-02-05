using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS; // 確保這是你 Entity 的命名空間
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface ILeaveApplicationsService
    {
        public short UserId { get; set; }
        public short RoleId { get; set; }
        List<LeaveApplicationsView> GetDataById(int userId);
        string InsertData(LeaveApplicationsView viewModel);
        string UpdateStatus(int id, string status, string remark);
        List<LeaveApplicationsView> GetAllData(); // 主管審核用
    }

    public class LeaveApplicationsView
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public int? dept_id { get; set; }
        public string dept_name { get; set; }
        public string leave_type { get; set; }
        public DateTime start_time { get; set; }
        public DateTime end_time { get; set; }
        public decimal? total_hours { get; set; }
        public string reason { get; set; }
        public string status { get; set; }
        public string manager_name { get; set; }
        public string manager_remark { get; set; }
        public DateTime? created_at { get; set; }
    }

    public class LeaveApplicationsService : ILeaveApplicationsService
    {
        private readonly PMSContext _PMSContext;
        public short UserId { get; set; }
        public short RoleId { get; set; }
        public LeaveApplicationsService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        // 1. 取得個人請假紀錄 (含部門與主管名稱)
        public List<LeaveApplicationsView> GetDataById(int userId)
        {
            return _PMSContext.LeaveApplications
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.StartTime)
                .Select(l => new LeaveApplicationsView
                {
                    id = l.Id,
                    user_id = l.UserId,
                    leave_type = l.LeaveType,
                    start_time = l.StartTime,
                    end_time = l.EndTime,
                    total_hours = l.TotalHours,
                    reason = l.Reason,
                    status = l.Status,
                    manager_remark = l.ManagerRemark,
                    created_at = l.CreatedAt,
                    // 聯集部門名稱
                    dept_name = l.Dept.DeptName,
                    // 聯集主管名稱 (從 Department 關聯過來)
                    manager_name = l.Dept.Manager.Username
                }).ToList();
        }

        // 2. 提交請假申請
        public string InsertData(LeaveApplicationsView viewModel)
        {
            // 找出申請人的部門資訊
          

            var data = new LeaveApplications
            {
                UserId = (int)UserId,
                DeptId = viewModel.dept_id, // 自動帶入部門 ID
                LeaveType = viewModel.leave_type,
                StartTime = viewModel.start_time,
                EndTime = viewModel.end_time,
                TotalHours = viewModel.total_hours,
                Reason = viewModel.reason,
                Status = "Pending", // 預設狀態：待審核
                CreatedAt = DateTime.Now
            };

            _PMSContext.LeaveApplications.Add(data);

            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        // 3. 主管審核更新狀態 (Approved/Rejected)
        public string UpdateStatus(int id, string status, string remark)
        {
            var data = _PMSContext.LeaveApplications.Find(id);
            if (data == null) return "NotFound";

            data.Status = status;
            data.ManagerRemark = remark;
            data.UpdatedAt = DateTime.Now;

            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        // 4. 取得所有資料 (主管管理介面用)
        public List<LeaveApplicationsView> GetAllData()
        {
            // 1. 先確認該角色的權限
            var currentRole = _PMSContext.LoginInfoRoles
                .Where(a => a.RoleId == RoleId)
                .Select(b => b.Role)
                .FirstOrDefault();

            // 2. 建立基礎 Query (先把 Include 和 Select 寫好)
            var query = _PMSContext.LeaveApplications
                .Include(l => l.User)
                .Include(l => l.Dept)
                .ThenInclude(d => d.Manager) // 確保能抓到主管名稱
                .AsQueryable();

            // 3. 權限判斷：若非 Admin，則增加 ManagerId 的過濾條件
            // 假設 check.IsAdmin 是判斷是否為管理員的欄位
            if (currentRole == null || !currentRole.IsAdmin)
            {
                query = query.Where(e => e.Dept.ManagerId == UserId);
            }

            // 4. 最後執行排序與投影轉型
            return query
                .OrderByDescending(l => l.CreatedAt)
                .Select(l => new LeaveApplicationsView
                {
                    id = l.Id,
                    user_id = l.UserId,
                    username = l.User.Username,
                    leave_type = l.LeaveType,
                    start_time = l.StartTime,
                    end_time = l.EndTime,
                    total_hours = l.TotalHours,
                    reason = l.Reason,
                    status = l.Status,
                    manager_remark = l.ManagerRemark,
                    created_at = l.CreatedAt,
                    dept_name = l.Dept.DeptName,
                    manager_name = l.Dept.Manager != null ? l.Dept.Manager.Username : "無主管"
                })
                .ToList();
        }
    }
}