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
    public interface IPssurplusDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PssurplusDataView> GetAllData();
        public void InsertData(PssurplusDataView data);
        public string DeleteData(int id);
        public string EditData(int id, PssurplusDataView data);
     
    }

    public partial class PssurplusDataView
    {

        public int id { get; set; }
        public short bill_year {  get; set; }
        public byte bill_month { get; set; }
        public int? surplus_kwh { get; set; }
        public int surplus_amount { get; set; }
        public string description { get; set; }
        public int surplus_id { get; set; }
        public string ps_site_name { get; set; }
        public string power_no { get; set; }

    }
    public class PssurplusDataService : IPssurplusDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PssurplusDataService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PssurplusDataView> GetAllData()
        {
            var query = _GreenPowerContext.PssurplusData.AsQueryable();
            var data = query.Select(t => new PssurplusDataView
            {
                id=t.Id,
                bill_year=t.BillYear,
                bill_month=t.BillMonth,
                surplus_amount=t.SurplusAmount,
                surplus_kwh=t.SurplusKwh,
                description=t.Description,
                surplus_id=t.SurplusId,
                ps_site_name=t.Surplus.PsSiteName,
                power_no = t.Surplus.Ps.PowerNo,
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(PssurplusDataView viewModel)
        {
            var data = new PssurplusData();
            data.BillYear=viewModel.bill_year;
            data.BillMonth=viewModel.bill_month;
            data.SurplusAmount=viewModel.surplus_amount;
            data.SurplusKwh=viewModel.surplus_kwh;
            data.Description=viewModel.description;
            data.SurplusId=viewModel.surplus_id;
            _GreenPowerContext.PssurplusData.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PssurplusData.Find(id);
            
            _GreenPowerContext.PssurplusData.Remove(data);
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

        public string EditData(int id, PssurplusDataView viewModel)
        {
            var data = _GreenPowerContext.PssurplusData.Find(id);
            data.BillYear = viewModel.bill_year;
            data.BillMonth = viewModel.bill_month;
            data.SurplusAmount = viewModel.surplus_amount;
            data.SurplusKwh = viewModel.surplus_kwh;
            data.Description = viewModel.description;
            data.SurplusId = viewModel.surplus_id;
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
