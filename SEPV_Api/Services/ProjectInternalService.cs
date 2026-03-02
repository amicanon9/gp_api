using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Gp_Api.Services
{
    public interface IProjectInternalService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        public List<ProjectInternalView> GetAllData();
        public void InsertData(ProjectInternalView data);
        public string DeleteData(int id);
        public string EditData(int id, ProjectInternalView data);
    }

    public class ProjectInternalView
    {
        public int id { get; set; }
        public short book_id { get; set; }
        public string name { get; set; }         // 專案名稱
        public string description { get; set; }  // 專案描述
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string Company_name { get; set; }
    }

    public class ProjectInternalService : IProjectInternalService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }

        private readonly PMSContext _PMSContext;

        public ProjectInternalService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<ProjectInternalView> GetAllData()
        {
            // 內部專案通常根據 BookId 過濾即可
            var check = _PMSContext.LoginInfoRoles
               .Where(a => a.RoleId == RoleId)
               .Select(b => b.Role)
               .FirstOrDefault();

            var query = _PMSContext.ProjectInternal
                .Select(t => new ProjectInternalView
                {
                    id = t.Id,
                    book_id = t.BookId,
                    name = t.Name,
                    description = t.Description,
                    created_at = t.CreatedAt,
                    updated_at = t.UpdatedAt,
                    Company_name = t.Book.Name,
                });


            if (check != null && !check.IsAdmin)
            {
                BookId = check.BookId;
                query = query.Where(a => a.book_id == BookId);
            }

            return query.ToList();
        }

        public void InsertData(ProjectInternalView viewModel)
        {
            var data = new ProjectInternal
            {
                BookId = viewModel.book_id,
                Name = viewModel.name,
                Description = viewModel.description,
                CreatedAt = DateTime.Now
            };

            try
            {
                _PMSContext.ProjectInternal.Add(data);
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
                // 這裡可以加入 Log 記錄 msg
            }
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.ProjectInternal.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.ProjectInternal.Remove(data);
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

        public string EditData(int id, ProjectInternalView viewModel)
        {
            var data = _PMSContext.ProjectInternal.Find(id);
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
    }
}