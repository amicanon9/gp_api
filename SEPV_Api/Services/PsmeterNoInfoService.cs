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
    public interface IPsmeterNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PsmeterNoInfoView> GetAllData();
        public void InsertData(PsmeterNoInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PsmeterNoInfoView data);
     
    }

    public partial class PsmeterNoInfoView
    {

        public int id { get; set; }
        public string meter_no { get; set; }
        public string description { get; set; }
        public int? ps_id { get; set; }
        public string power_no { get; set; }
        public string ps_name { get; set; }
    }
    public class PsmeterNoInfoService : IPsmeterNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PsmeterNoInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PsmeterNoInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PsmeterNoInfo.Include(e=>e.Ps).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Ps.Info.BookId == check.BookId);
            }
            var data = query.Select(t => new PsmeterNoInfoView
            {
                id = t.Id,
                meter_no = t.MeterNo,
                description = t.Description,
                ps_id = (int)t.PsId,
                power_no = t.Ps.PowerNo,
                ps_name = t.Ps.Info.PsName,
            });
            var allData = data.ToList();
            return allData;
        }

    

        public void InsertData(PsmeterNoInfoView viewModel)
        {
            var data = new PsmeterNoInfo();
            data.MeterNo = viewModel.meter_no;
            data.Description = viewModel.description;
            data.PsId = (int)viewModel.ps_id;
            _GreenPowerContext.PsmeterNoInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PsmeterNoInfo.Find(id);
            
            _GreenPowerContext.PsmeterNoInfo.Remove(data);
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

        public string EditData(int id, PsmeterNoInfoView viewModel)
        {
            var data = _GreenPowerContext.PsmeterNoInfo.Find(id);
            
            data.MeterNo = viewModel.meter_no;
            data.Description = viewModel.description;
            data.PsId = (int)viewModel.ps_id;
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
