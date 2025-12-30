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
    public interface IServiceNoDetailDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<ServiceNoDetailDataView> GetAllData();
        public void InsertData(ServiceNoDetailDataView data);
        public string DeleteData(int id);
        public string EditData(int id, ServiceNoDetailDataView data);
     
    }

    public class ServiceNoDetailDataView
    {
        public int id { get; set; }
        public int service_no_detail_id { get; set; }
        public short bill_year { get; set; }
        public byte bill_month { get; set; }
        public int kwh_usage { get; set; }
        public decimal transmission_rate { get; set; }
        public decimal distribution_rate { get; set; }
        public decimal dispatching_rate { get; set; }
        public decimal ancillary_services_rate { get; set; }
        public int fee { get; set; }
        public string description { get; set; }
        public string service_no { get; set; }
        public string ps_meter_no { get; set; }
        public string pp_meter_no { get; set; }
        public string pp_name { get; set; }
        public string ps_name { get; set; }
        public string ps_power_no { get; set; }
        public string pp_power_no { get; set; }

    }
    public class ServiceNoDetailDataService : IServiceNoDetailDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public ServiceNoDetailDataService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<ServiceNoDetailDataView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.ServiceNoDetailData.Include(t=>t.ServiceNoDetail).ThenInclude(tt=>tt.ServiceNo).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.ServiceNoDetail.ServiceNo.BookId == check.BookId);
            }
            var data = query.Select(t => new ServiceNoDetailDataView
            {
                id = t.Id,
                service_no_detail_id = t.ServiceNoDetailId,
                bill_year = t.BillYear,
                bill_month = t.BillMonth,
                kwh_usage = t.KwhUsage,
                transmission_rate = t.TransmissionRate,
                distribution_rate = t.DistributionRate,
                dispatching_rate = t.DispatchingRate,
                ancillary_services_rate = t.AncillaryServicesRate,
                fee = t.Fee,
                description = t.Description,
                service_no = t.ServiceNoDetail.ServiceNo.ServiceNo,
                pp_meter_no = t.ServiceNoDetail.PpMeter.MeterNo,
                pp_name = t.ServiceNoDetail.PpMeter.Pp.Info.PpName,
                ps_meter_no = t.ServiceNoDetail.PsMeter.MeterNo,
                ps_name = t.ServiceNoDetail.PsMeter.Ps.Info.PsName,
                ps_power_no = t.ServiceNoDetail.PsMeter.Ps.PowerNo,
                pp_power_no = t.ServiceNoDetail.PpMeter.Pp.PowerNo,
            });
            var allData = data.ToList();
            return allData;
        }

    

        public void InsertData(ServiceNoDetailDataView viewModel)
        {
            var data = new ServiceNoDetailData();
            data.ServiceNoDetailId = viewModel.service_no_detail_id;
            data.BillYear = viewModel.bill_year;
            data.BillMonth = viewModel.bill_month;
            data.KwhUsage = viewModel.kwh_usage;
            data.TransmissionRate = viewModel.transmission_rate;
            data.DistributionRate = viewModel.distribution_rate;
            data.DispatchingRate = viewModel.dispatching_rate;
            data.AncillaryServicesRate = viewModel.ancillary_services_rate;
            data.Fee = viewModel.fee;
            data.Description = viewModel.description;
            _GreenPowerContext.ServiceNoDetailData.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.ServiceNoDetailData.Find(id);

            _GreenPowerContext.ServiceNoDetailData.Remove(data);
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

        public string EditData(int id, ServiceNoDetailDataView viewModel)
        {
            var data = _GreenPowerContext.ServiceNoDetailData.Find(id);
            
            data.ServiceNoDetailId = viewModel.service_no_detail_id;
            data.BillYear = viewModel.bill_year;
            data.BillMonth = viewModel.bill_month;
            data.KwhUsage = viewModel.kwh_usage;
            data.TransmissionRate = viewModel.transmission_rate;
            data.DistributionRate = viewModel.distribution_rate;
            data.DispatchingRate = viewModel.dispatching_rate;
            data.AncillaryServicesRate = viewModel.ancillary_services_rate;
            data.Fee = viewModel.fee;
            data.Description = viewModel.description;
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
