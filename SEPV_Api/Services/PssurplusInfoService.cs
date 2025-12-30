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
    public interface IPssurplusInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PssurplusInfoView> GetAllData();
        public void InsertData(PssurplusInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PssurplusInfoView data);
     
    }

    public partial class PssurplusInfoView
    {

        public int id { get; set; }
        public string ps_site_name {  get; set; }
        public decimal surplus_rate { get; set; }
        public string description { get; set; }
        public decimal? total_kwp { get; set; }
        public int ps_id { get; set; }
        public string ps_name { get; set; }
        public string power_no { get; set; }

    }
    public class PssurplusInfoService : IPssurplusInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PssurplusInfoService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PssurplusInfoView> GetAllData()
        {
            var query = _GreenPowerContext.PssurplusInfo.Include(e => e.Ps).AsQueryable();
            var data = query.Select(t => new PssurplusInfoView
            {
                id = t.Id,
                ps_site_name = t.PsSiteName,
                description = t.Description,
                surplus_rate = t.SurplusRate,
                total_kwp = t.TotalKwp,
                ps_id = t.PsId,
                ps_name = t.Ps.Info.PsName,
                power_no = t.Ps.PowerNo,
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(PssurplusInfoView viewModel)
        {
            var data = new PssurplusInfo();
            data.PsSiteName= viewModel.ps_site_name;
            data.Description=viewModel.description;
            data.SurplusRate=viewModel.surplus_rate;
            data.TotalKwp=viewModel.total_kwp;
            data.PsId=viewModel.ps_id;
            _GreenPowerContext.PssurplusInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PssurplusInfo.Find(id);
            
            _GreenPowerContext.PssurplusInfo.Remove(data);
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

        public string EditData(int id, PssurplusInfoView viewModel)
        {
            var data = _GreenPowerContext.PssurplusInfo.Find(id);
            data.PsSiteName = viewModel.ps_site_name;
            data.Description = viewModel.description;
            data.SurplusRate = viewModel.surplus_rate;
            data.TotalKwp = viewModel.total_kwp;
            data.PsId = viewModel.ps_id;
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
