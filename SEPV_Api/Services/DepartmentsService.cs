using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS; // 確保包含您產生的 Departments Entity
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gp_Api.Services
{
    public interface IDepartmentsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        List<DepartmentsView> GetAllData();
        DepartmentsView GetDataById(int id);
        void InsertData(DepartmentsView data);
        string EditData(int id, DepartmentsView data);
        string DeleteData(int id);
    }

    public class DepartmentsView
    {
        public int? Id { get; set; }
        public short Book_id { get; set; }
        public string Dept_name { get; set; }
        public string Description { get; set; }
        public int? Manager_id { get; set; }      // 部門主管 ID
        public string Manager_name { get; set; }  // 關聯顯示用：主管姓名
        public string Company_name { get; set; }
    }

    public class DepartmentsService : IDepartmentsService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        private readonly PMSContext _PMSContext;

        public DepartmentsService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<DepartmentsView> GetAllData()
        {
            // 保持你的風格：先檢查權限 (如有需要)
            var check = _PMSContext.LoginInfoRoles
                .Where(a => a.RoleId == RoleId)
                .Select(b => b.Role)
                .FirstOrDefault();

            // 統一使用 .Select 與 Lambda 處理關聯
            var query = _PMSContext.Departments
                .Select(t => new DepartmentsView
                {
                    Id = t.Id,
                    Book_id = t.BookId,
                    Dept_name = t.DeptName,
                    Description = t.Description,
                    Manager_id = t.ManagerId,
                    Company_name = t.Book.Name,
                    Manager_name = _PMSContext.LoginInfo
                        .Where(u => u.Id == t.ManagerId)
                        .Select(u => u.Username)
                        .FirstOrDefault() ?? "未指定"
                });

            if (check != null && !check.IsAdmin)
            {
                BookId = check.BookId;
                query = query.Where(a => a.Book_id == BookId);
            }

            return query.ToList();
        }

        public DepartmentsView GetDataById(int id)
        {
            return _PMSContext.Departments
                .Where(t => t.Id == id)
                .Select(t => new DepartmentsView
                {
                    Id = t.Id,
                    Book_id=t.BookId,
                    Company_name = t.Book.Name,
                    Dept_name = t.DeptName,
                    Description = t.Description,
                    Manager_id = t.ManagerId
                })
                .FirstOrDefault();
        }

        public void InsertData(DepartmentsView viewModel)
        {
            var data = new Departments
            {
                BookId =viewModel.Book_id,
                DeptName = viewModel.Dept_name,
                Description = viewModel.Description,
                ManagerId = viewModel.Manager_id
            };

            _PMSContext.Departments.Add(data);
            _PMSContext.SaveChanges();
        }

        public string EditData(int id, DepartmentsView viewModel)
        {
            var data = _PMSContext.Departments.FirstOrDefault(t => t.Id == id);
            if (data == null) return "NotFound";
            data.BookId = viewModel.Book_id;
            data.DeptName = viewModel.Dept_name;
            data.Description = viewModel.Description;
            data.ManagerId = viewModel.Manager_id;

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

        public string DeleteData(int id)
        {
            var data = _PMSContext.Departments.FirstOrDefault(t => t.Id == id);
            if (data == null) return "NotFound";

            // 簡單防呆：確保沒有員工掛在該部門下
            if (_PMSContext.LoginInfo.Any(u => u.DeptId == id))
                return "此部門尚有員工，無法刪除";

            _PMSContext.Departments.Remove(data);
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