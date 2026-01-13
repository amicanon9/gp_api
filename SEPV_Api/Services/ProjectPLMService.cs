using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using Microsoft.EntityFrameworkCore; // 確保有引用此項以支援 Include
using SEPV_Api.Models.PMS; // 假設這是你的 Entity Framework Models 命名空間
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface IProjectPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<ProjectPlmView> GetAllData();
        public void InsertData(ProjectPlmView data);
        public string DeleteData(int id);
        public string EditData(int id, ProjectPlmView data);
    }

    public partial class ProjectPlmView
    {
        // Project 資訊
        public int id { get; set; }
        public int role_id { get; set; }
        public int? year { get; set; }
        public string quarter { get; set; }
        public int? month { get; set; }
        public DateTime? close_date { get; set; }
        public decimal? rfq_to_client_amount { get; set; }
        public decimal? net_to_ds_amount { get; set; }
        public string system_inquiry_channel { get; set; }
        public bool? is_system_checked { get; set; }
        public bool? is_ags_booking { get; set; }
      
        public string ags_status { get; set; }
        public string under_control_longshot_year_q { get; set; }
        public string solution_mapping { get; set; }
        public int? sales_owner { get; set; }
        public int? service_owner { get; set; }

        // Customer 關聯資訊
        public int? customer_id { get; set; }
        public string customer_name { get; set; } // 來自 CustomerPLM
    }

    public class ProjectPlmService : IProjectPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly PMSContext _PMSContext;

        public ProjectPlmService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<ProjectPlmView> GetAllData()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.ProjectPlm.Select(t => new ProjectPlmView
            {
                id = t.Id,
                role_id= t.RoleId,
                year = t.Year,
                quarter = t.Quarter,
                month = t.Month,
                close_date = t.CloseDate,
                rfq_to_client_amount = t.RfqToClientAmount,
                net_to_ds_amount = t.NetToDsAmount,
                system_inquiry_channel = t.SystemInquiryChannel,
                is_system_checked = t.IsSystemChecked,
                is_ags_booking = t.IsAgsBooking,
                customer_id = t.CustomerId,
              
                ags_status = t.AgsStatus,
                under_control_longshot_year_q = t.UnderControlLongshotYearQ,
                solution_mapping = t.SolutionMapping,
                sales_owner = t.SalesOwner,
                service_owner = t.ServiceOwner,

                // 帶入客戶資訊
                customer_name = t.Customer != null ? t.Customer.Name : "",
            });

            if (!check.IsAdmin)
            {
                query = query.Where(a => a.role_id == RoleId);
            }
            var allData = query.ToList();
            return allData;
        }

        public void InsertData(ProjectPlmView viewModel)
        {
            var data = new ProjectPlm
            {
                RoleId=RoleId,
                Year = viewModel.year,
                Quarter = viewModel.quarter,
                Month = viewModel.month,
                CloseDate = viewModel.close_date,
                RfqToClientAmount = viewModel.rfq_to_client_amount,
                NetToDsAmount = viewModel.net_to_ds_amount,
                SystemInquiryChannel = viewModel.system_inquiry_channel,
                IsSystemChecked = viewModel.is_system_checked,
                IsAgsBooking = viewModel.is_ags_booking,
                CustomerId = viewModel.customer_id,
                AgsStatus = viewModel.ags_status,
                UnderControlLongshotYearQ = viewModel.under_control_longshot_year_q,
                SolutionMapping = viewModel.solution_mapping,
                SalesOwner = viewModel.sales_owner,
                ServiceOwner = viewModel.service_owner,
                CreatedAt = DateTime.Now // 雖然 DB 有 Default，但程式給值較安全
            };
            try
            {
                _PMSContext.ProjectPlm.Add(data);
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                var a= ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
            }

        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.ProjectPlm.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.ProjectPlm.Remove(data);
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

        public string EditData(int id, ProjectPlmView viewModel)
        {
            var data = _PMSContext.ProjectPlm.Find(id);
            if (data == null) return "NotFound";

            data.Year = viewModel.year;
            data.Quarter = viewModel.quarter;
            data.Month = viewModel.month;
            data.CloseDate = viewModel.close_date;
            data.RfqToClientAmount = viewModel.rfq_to_client_amount;
            data.NetToDsAmount = viewModel.net_to_ds_amount;
            data.SystemInquiryChannel = viewModel.system_inquiry_channel;
            data.IsSystemChecked = viewModel.is_system_checked;
            data.IsAgsBooking = viewModel.is_ags_booking;
            data.CustomerId = viewModel.customer_id;
            data.AgsStatus = viewModel.ags_status;
            data.UnderControlLongshotYearQ = viewModel.under_control_longshot_year_q;
            data.SolutionMapping = viewModel.solution_mapping;
            data.SalesOwner = viewModel.sales_owner;
            data.ServiceOwner = viewModel.service_owner;
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