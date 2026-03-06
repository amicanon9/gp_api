using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    #region Interface
    public interface IProjectFirmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        public List<ProjectFirmView> GetAllData();
        public void InsertData(ProjectFirmView data);
        public string DeleteData(int id);
        public string EditData(int id, ProjectFirmView data);
    }
    #endregion

    #region View Models
    public class ProjectFirmView
    {
        public int id { get; set; }
        public short book_id { get; set; }
        public string name { get; set; }          // 事務所名稱
        public string description { get; set; }   // 描述
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string Company_name { get; set; }  // 對應 Book.Name
    }
    #endregion

    #region Service Implementation
    public class ProjectFirmService : IProjectFirmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }

        private readonly PMSContext _PMSContext;

        public ProjectFirmService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<ProjectFirmView> GetAllData()
        {
            // 透過角色關聯表確認權限
            var check = _PMSContext.LoginInfoRoles
               .Where(a => a.RoleId == RoleId)
               .Select(b => b.Role)
               .FirstOrDefault();

            var query = _PMSContext.ProjectFirm
                .Select(t => new ProjectFirmView
                {
                    id = t.Id,
                    book_id = t.BookId,
                    name = t.Name,
                    description = t.Description,
                    created_at = t.CreatedAt,
                    updated_at = t.UpdatedAt,
                    Company_name = t.Book.Name, // 取得關聯的帳簿名稱
                });

            // 權限過濾：非管理員則根據 BookId 過濾
            if (check != null && !check.IsAdmin)
            {
                // 如果 Token 帶過來的 BookId 跟 Role 關聯的不同，以 Role 的為主
                var filterBookId = check.BookId;
                query = query.Where(a => a.book_id == filterBookId);
            }

            return query.ToList();
        }

        public void InsertData(ProjectFirmView viewModel)
        {
            var data = new ProjectFirm
            {
                // 優先使用傳入的 book_id，若無則使用 Service 全域的 BookId
                BookId = viewModel.book_id != 0 ? viewModel.book_id : BookId,
                Name = viewModel.name,
                Description = viewModel.description,
                CreatedAt = DateTime.Now
            };

            try
            {
                _PMSContext.ProjectFirm.Add(data);
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                // 錯誤處理邏輯
                var msg = ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
                throw new Exception(msg);
            }
        }

        public string EditData(int id, ProjectFirmView viewModel)
        {
            var data = _PMSContext.ProjectFirm.Find(id);
            if (data == null) return "NotFound";

            data.BookId = viewModel.book_id;
            data.Name = viewModel.name;
            data.Description = viewModel.description;
            data.UpdatedAt = DateTime.Now;

            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
            }
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.ProjectFirm.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.ProjectFirm.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
            }
        }
    }
    #endregion
}