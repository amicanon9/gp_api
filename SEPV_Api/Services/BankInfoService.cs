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
    public interface IBankInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<BankInfoView> GetAllData();
        public void InsertData(BankInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, BankInfoView data);
    }

    public partial class BankInfoView
    {

        public int id { get; set; }
        public string bank_no {  get; set; }
        public string bank_name { get; set; }
        public string description { get; set; }
        public bool disable { get; set; }

    }
    public class BankInfoService : IBankInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        public BankInfoService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
        }

        public List<BankInfoView> GetAllData()
        {
            var query = _GreenPowerContext.BankInfo.AsQueryable();
            var data = query.Select(t => new BankInfoView
            {
                id = t.Id,
                bank_name = t.BankName,
                description = t.Description,
                disable = t.Disable,
                bank_no = t.BankNo,
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(BankInfoView viewModel)
        {
            var data = new BankInfo();
            data.BankName = viewModel.bank_name;
            data.BankNo= viewModel.bank_no;
            data.Description = viewModel.description;
            data.Disable =viewModel.disable;
            _GreenPowerContext.BankInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.BankInfo.Find(id);
            
            _GreenPowerContext.BankInfo.Remove(data);
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

        public string EditData(int id, BankInfoView viewModel)
        {
            var data = _GreenPowerContext.BankInfo.Find(id);
            
            data.BankName = viewModel.bank_name;
            data.BankNo = viewModel.bank_no;
            data.Description = viewModel.description;
            data.Disable = viewModel.disable;
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
