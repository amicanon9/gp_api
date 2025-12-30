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
    public interface IPpmeterNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PpmeterNoInfoView> GetAllData();
        public void InsertData(PpmeterNoInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PpmeterNoInfoView data);
     
    }

    public partial class PpmeterNoInfoView
    {

        public int id { get; set; }
        public string meter_no { get; set; }
        public string description { get; set; }
        public string power_no { get; set; }
        public int? pp_id { get; set; }
        public string pp_name { get; set; }
    }
    public class PpmeterNoInfoService : IPpmeterNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PpmeterNoInfoService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PpmeterNoInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PpmeterNoInfo.Include(t=>t.Pp).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Pp.Info.BookId == check.BookId);
            }
            var data = query.Select(t => new PpmeterNoInfoView
            {
                id = t.Id,
                meter_no = t.MeterNo,
                description = t.Description,
                pp_id = (int)t.PpId,
                power_no =t.Pp.PowerNo,
                pp_name = t.Pp.Info.PpName,
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(PpmeterNoInfoView viewModel)
        {
            var data = new PpmeterNoInfo();
            data.MeterNo = viewModel.meter_no;
            data.Description = viewModel.description;
            data.PpId = (int)viewModel.pp_id;
            _GreenPowerContext.PpmeterNoInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PpmeterNoInfo.Find(id);

            _GreenPowerContext.PpmeterNoInfo.Remove(data);
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

        public string EditData(int id, PpmeterNoInfoView viewModel)
        {
            var data = _GreenPowerContext.PpmeterNoInfo.Find(id);
            
            data.MeterNo = viewModel.meter_no;
            data.Description = viewModel.description;
            data.PpId = (int)viewModel.pp_id;
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
