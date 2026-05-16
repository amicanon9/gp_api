using Gp_Api.IServices;
using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface IProject2CheckService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }
        public List<Project2CheckView> GetAllData();
        public void InsertData(Project2CheckView data);
        public string DeleteData(int id);
        public string EditData(int id, Project2CheckView data);
    }

    public partial class Project2CheckView
    {
        public int id { get; set; }
        public short book_id { get; set; }
        public int role_id { get; set; }
        public int? year { get; set; }
        public string quarter { get; set; }
        public int? month { get; set; }
        public DateTime? close_date { get; set; }
        public int? customer_id { get; set; }
        public string status { get; set; }
        public string customer_name { get; set; }
    }

    public class Project2CheckService : IProject2CheckService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public short BookId { get; set; }

        private readonly PMSContext _PMSContext;

        public Project2CheckService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<Project2CheckView> GetAllData()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.Project2Check.Select(t => new Project2CheckView
            {
                id = t.Id,
                book_id = t.BookId,
                role_id = t.RoleId,
                year = t.Year,
                quarter = t.Quarter,
                month = t.Month,
                close_date = t.CloseDate,
                customer_id = t.CustomerId,
                status = t.Status,
                customer_name = t.Customer != null ? t.Customer.Name : "",
            });

            if (check != null && !check.IsAdmin)
            {
                query = query.Where(t => t.role_id == RoleId);
            }

            return query.ToList();
        }

        public void InsertData(Project2CheckView data)
        {
            var item = new Project2Check
            {
                BookId = BookId,
                RoleId = RoleId,
                Year = data.year,
                Quarter = data.quarter,
                Month = data.month,
                CloseDate = data.close_date,
                CustomerId = data.customer_id,
                Status = data.status,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            _PMSContext.Project2Check.Add(item);
            _PMSContext.SaveChanges();
        }

        public string EditData(int id, Project2CheckView data)
        {
            var item = _PMSContext.Project2Check.Find(id);
            if (item == null) return "NotFound";

            item.Year = data.year;
            item.Quarter = data.quarter;
            item.Month = data.month;
            item.CloseDate = data.close_date;
            item.CustomerId = data.customer_id;
            item.Status = data.status;
            item.UpdatedAt = DateTime.Now;

            _PMSContext.SaveChanges();
            return "OK";
        }

        public string DeleteData(int id)
        {
            var item = _PMSContext.Project2Check.Find(id);
            if (item == null) return "NotFound";
            try
            {
                _PMSContext.Project2Check.Remove(item);
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
