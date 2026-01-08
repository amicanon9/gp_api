using SEPV_Api.Models.GreenPower; // 假設這是你的 Entity Framework Models 命名空間
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // 確保有引用此項以支援 Include

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
        public int? year { get; set; }
        public string quarter { get; set; }
        public int? month { get; set; }
        public DateTime? close_date { get; set; }
        public decimal? rfq_to_client_amount { get; set; }
        public decimal? net_to_ds_amount { get; set; }
        public string system_inquiry_channel { get; set; }
        public string is_system_checked { get; set; }
        public bool? is_ags_booking { get; set; }
        public string industry_crm { get; set; }
        public string existing_plm { get; set; }
        public string existing_cad { get; set; }
        public string ags_status { get; set; }
        public string under_control_longshot_year_q { get; set; }
        public string solution_mapping { get; set; }
        public string sales_owner { get; set; }
        public string service_owner { get; set; }

        // Customer 關聯資訊
        public int? customer_id { get; set; }
        public string customer_name { get; set; } // 來自 CustomerPLM
        public int? customer_tax_id { get; set; } // 來自 CustomerPLM
    }

    public class ProjectPlmService : IProjectPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;

        public ProjectPlmService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
        }

        public List<ProjectPlmView> GetAllData()
        {
            // 使用 Select 直接進行投影，這會自動轉換為 SQL Join
            var data = _GreenPowerContext.ProjectPlm.Select(t => new ProjectPlmView
            {
                id = t.Id,
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
                industry_crm = t.IndustryCrm,
                existing_plm = t.ExistingPlm,
                existing_cad = t.ExistingCad,
                ags_status = t.AgsStatus,
                under_control_longshot_year_q = t.UnderControlLongshotYearQ,
                solution_mapping = t.SolutionMapping,
                sales_owner = t.SalesOwner,
                service_owner = t.ServiceOwner,

                // 帶入客戶資訊
                customer_name = t.Customer != null ? t.Customer.Name : "",
                customer_tax_id = t.Customer != null ? t.Customer.TaxIdNo : null
            }).ToList();

            return data;
        }

        public void InsertData(ProjectPlmView viewModel)
        {
            var data = new ProjectPlm
            {
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
                IndustryCrm = viewModel.industry_crm,
                ExistingPlm = viewModel.existing_plm,
                ExistingCad = viewModel.existing_cad,
                AgsStatus = viewModel.ags_status,
                UnderControlLongshotYearQ = viewModel.under_control_longshot_year_q,
                SolutionMapping = viewModel.solution_mapping,
                SalesOwner = viewModel.sales_owner,
                ServiceOwner = viewModel.service_owner,
                CreatedAt = DateTime.Now // 雖然 DB 有 Default，但程式給值較安全
            };

            _GreenPowerContext.ProjectPlm.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.ProjectPlm.Find(id);
            if (data == null) return "NotFound";

            _GreenPowerContext.ProjectPlm.Remove(data);
            try
            {
                _GreenPowerContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
            }
        }

        public string EditData(int id, ProjectPlmView viewModel)
        {
            var data = _GreenPowerContext.ProjectPlm.Find(id);
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
            data.IndustryCrm = viewModel.industry_crm;
            data.ExistingPlm = viewModel.existing_plm;
            data.ExistingCad = viewModel.existing_cad;
            data.AgsStatus = viewModel.ags_status;
            data.UnderControlLongshotYearQ = viewModel.under_control_longshot_year_q;
            data.SolutionMapping = viewModel.solution_mapping;
            data.SalesOwner = viewModel.sales_owner;
            data.ServiceOwner = viewModel.service_owner;
            data.UpdatedAt = DateTime.Now;

            try
            {
                _GreenPowerContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex.Message}\nInner:{ex.InnerException.Message}" : ex.Message;
            }
        }
    }
}