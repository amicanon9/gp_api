using SEPV_Api.Models.GreenPower;

using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Gp_Api.Services
{
    public interface IPppowerNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PppowerNoInfoView> GetAllData();
        public void InsertData(PppowerNoInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PppowerNoInfoView data);
     
    }

    public partial class PppowerNoInfoView
    {

        public int id { get; set; }
        public string power_no { get; set; }
        public string description { get; set; }
        public int? info_id { get; set; }

    }
    public class PppowerNoInfoService : IPppowerNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PppowerNoInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PppowerNoInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PppowerNoInfo.AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Info.BookId == check.BookId);
            }
            var data = query.Select(t => new PppowerNoInfoView
            {
                id = t.Id,
                power_no = t.PowerNo,
                description = t.Description,
                info_id = (int)t.InfoId,
            });
            var allData = data.ToList();
            return allData;
        }

    

        public void InsertData(PppowerNoInfoView viewModel)
        {
            var data = new PppowerNoInfo();
            data.PowerNo = viewModel.power_no;
            data.Description = viewModel.description;
            data.InfoId = viewModel.info_id;
            _GreenPowerContext.PppowerNoInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PppowerNoInfo.Find(id);

            _GreenPowerContext.PppowerNoInfo.Remove(data);
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

        public string EditData(int id, PppowerNoInfoView viewModel)
        {
            var data = _GreenPowerContext.PppowerNoInfo.Find(id);
            
            data.PowerNo = viewModel.power_no;
            data.Description = viewModel.description;
            data.InfoId = viewModel.info_id;
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
