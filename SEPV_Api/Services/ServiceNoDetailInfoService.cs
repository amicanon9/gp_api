using SEPV_Api.Models.GreenPower;

using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Gp_Api.Services
{
    public interface IServiceNoDetailInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<ServiceNoDetailInfoView> GetAllData();
        public void InsertData(ServiceNoDetailInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, ServiceNoDetailInfoView data);
     
    }

    public class ServiceNoDetailInfoView
    {
        public int id { get; set; }
        public int ps_meter_id { get; set; }
        public int pp_meter_id { get; set; }
        public string description { get; set; }
        public int service_no_id { get; set; }
        public decimal? ps_total_kwp { get; set; }
        public decimal? ps_pp_percent { get; set; }
        public string ps_meter_no { get; set; }
        public decimal? ps_rate { get; set; }
        public string pp_meter_no { get; set; }
        public string service_no { get; set; }
        public string pp_name { get; set; }
        public string ps_name { get; set; }
        public string ps_power_no { get; set; }
        public string pp_power_no { get; set; }
        public string type { get; set; }
        public string branch_name { get; set; }

    }
    public class ServiceNoDetailInfoService : IServiceNoDetailInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public ServiceNoDetailInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<ServiceNoDetailInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.ServiceNoDetailInfo.Include(t=>t.PsMeter).Include(t=>t.PpMeter).Include(t=>t.ServiceNo).Include(t=>t.ServiceNoDetailData).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.ServiceNo.BookId == check.BookId);
            }
            var data = query.Select(t => new ServiceNoDetailInfoView
            {
                id = t.Id,
                ps_meter_id = t.PsMeterId,
                pp_meter_id = t.PpMeterId,
                description = t.Description,
                service_no_id = t.ServiceNoId,
                ps_pp_percent = t.PsPpPercent,
                ps_rate = t.PsRate,
                ps_total_kwp=t.PsTotalKwp,
                ps_meter_no = t.PsMeter.MeterNo,
                pp_meter_no=t.PpMeter.MeterNo,
                service_no=t.ServiceNo.ServiceNo,
                pp_name = t.PpMeter.Pp.Info.PpName,
                ps_name = t.PsMeter.Ps.Info.PsName,
                ps_power_no = t.PsMeter.Ps.PowerNo,
                pp_power_no = t.PpMeter.Pp.PowerNo,
                branch_name = t.PsMeter.Ps.PsBank.Branch.BranchName,
                type =t.Type,
            });
            var allData = data.ToList();
            return allData;
        }

    

        public void InsertData(ServiceNoDetailInfoView viewModel)
        {
            var data = new ServiceNoDetailInfo();
            data.PsMeterId = viewModel.ps_meter_id;
            data.PpMeterId = viewModel.pp_meter_id;
            data.Description = viewModel.description;
            data.ServiceNoId = viewModel.service_no_id;
            data.PsPpPercent = viewModel.ps_pp_percent;
            data.PsTotalKwp = viewModel.ps_total_kwp;
            data.PsRate = viewModel.ps_rate;
            data.Type = viewModel.type;
            _GreenPowerContext.ServiceNoDetailInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.ServiceNoDetailInfo.Find(id);

            _GreenPowerContext.ServiceNoDetailInfo.Remove(data);
            try
            {
                _GreenPowerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }

        public string EditData(int id, ServiceNoDetailInfoView viewModel)
        {
            var data = _GreenPowerContext.ServiceNoDetailInfo.Find(id);
            
            data.PsMeterId = viewModel.ps_meter_id;
            data.PpMeterId = viewModel.pp_meter_id;
            data.Description = viewModel.description;
            data.ServiceNoId = viewModel.service_no_id;
            data.PsPpPercent = viewModel.ps_pp_percent;
            data.PsTotalKwp = viewModel.ps_total_kwp;
            data.PsRate = viewModel.ps_rate;
            data.Type = viewModel.type;
            try
            {
                _GreenPowerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }

  

      
    }
}
