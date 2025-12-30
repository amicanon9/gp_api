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
    public interface IServiceNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<ServiceNoInfoView> GetAllData();
        public void InsertData(ServiceNoInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, ServiceNoInfoView data);
     
    }

    public partial class ServiceNoInfoView
    {

        public int id { get; set; }
        public short book_id { get; set; }
        public string service_no { get; set; }
        public decimal? ps_total_kwp { get; set; }
        public decimal? pp_rate { get; set; }
        public string description { get; set; }
        public string book_name { get; set; }
    }
    public class ServiceNoInfoService : IServiceNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public ServiceNoInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<ServiceNoInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.ServiceNoInfo.Include(t => t.Book).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Book.BookId == check.BookId);
            }
            var data = query.Select(t => new ServiceNoInfoView
            {
                id = t.Id,
                book_id = t.BookId,
                service_no = t.ServiceNo,
                ps_total_kwp = t.PsTotalKwp,
                pp_rate = t.PpRate,
                description = t.Description,
                book_name=t.Book.Name,
            });
            var allData = data.ToList();
            return allData;
        }

    

        public void InsertData(ServiceNoInfoView viewModel)
        {
            var data = new ServiceNoInfo();
            data.BookId = viewModel.book_id;
            data.ServiceNo = viewModel.service_no;
            data.PsTotalKwp = viewModel.ps_total_kwp;
            data.PpRate = viewModel.pp_rate;
            data.Description = viewModel.description;
            _GreenPowerContext.ServiceNoInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.ServiceNoInfo.Find(id);

            _GreenPowerContext.ServiceNoInfo.Remove(data);
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

        public string EditData(int id, ServiceNoInfoView viewModel)
        {
            var data = _GreenPowerContext.ServiceNoInfo.Find(id);
            
            data.BookId = viewModel.book_id;
            data.ServiceNo = viewModel.service_no;
            data.PsTotalKwp = viewModel.ps_total_kwp;
            data.PpRate = viewModel.pp_rate;
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
