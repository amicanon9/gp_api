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
    public interface IBankBranchInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<BankBranchInfoView> GetAllData();
        public void InsertData(BankBranchInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, BankBranchInfoView data);
    }

    public partial class BankBranchInfoView
    {

        public int id { get; set; }
        public string branch_no { get; set; }
        public string branch_name { get; set; }
        public string description { get; set; }
        public bool disable { get; set; }
        public int bank_id { get; set; }
        public string bank_name { get; set; }
    }
    public class BankBranchInfoService : IBankBranchInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        public BankBranchInfoService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
        }

        public List<BankBranchInfoView> GetAllData()
        {
            var query = _GreenPowerContext.BankBranchInfo.Include(r=>r.Bank).AsQueryable();
            var data = query.Select(t => new BankBranchInfoView
            {
                id = t.Id,
                branch_no = t.BranchNo,
                branch_name = t.BranchName,
                description = t.Description,
                disable=t.Disable,
                bank_id = t.BankId,
                bank_name=t.Bank.BankName
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(BankBranchInfoView viewModel)
        {
            var data = new BankBranchInfo();
            data.BranchNo = viewModel.branch_no;
            data.BranchName = viewModel.branch_name;
            data.Description = viewModel.description;
            data.Disable = viewModel.disable;
            data.BankId = viewModel.bank_id;
            _GreenPowerContext.BankBranchInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.BankBranchInfo.Find(id);
            
            _GreenPowerContext.BankBranchInfo.Remove(data);
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

        public string EditData(int id, BankBranchInfoView viewModel)
        {
            var data = _GreenPowerContext.BankBranchInfo.Find(id);
            
            data.BranchNo = viewModel.branch_no;
            data.BranchName = viewModel.branch_name;
            data.Description = viewModel.description;
            data.Disable = viewModel.disable;
            data.BankId = viewModel.bank_id;
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
