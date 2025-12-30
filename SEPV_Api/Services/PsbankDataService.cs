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
    public interface IPsbankDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PsbankDataView> GetAllData();
        public void InsertData(PsbankDataView data);
        public string DeleteData(int id);
        public string EditData(int id, PsbankDataView data);
     
    }

    public partial class PsbankDataView
    {

        public int id { get; set; }
        public int info_id { get; set; }
        public int branch_id { get; set; }
        public string bank_account_number { get; set; }
        public string description { get; set; }
        public string ps_name { get; set; }
        public string bank_name { get; set; }
        public string branch_name { get; set; }

    }
    public class PsbankDataService : IPsbankDataService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PsbankDataService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PsbankDataView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PsbankData.Include(t => t.Info).Include(t => t.Branch)
                        .ThenInclude(bb => bb.Bank).AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Info.BookId == check.BookId);
            }
            var data = query.Select(t => new PsbankDataView
            {
                id = t.Id,
                info_id = t.InfoId,
                branch_id = t.BranchId,
                bank_account_number = t.BankAccountNumber,
                description = t.Description,
                bank_name=t.Branch.Bank.BankName,
                branch_name=t.Branch.BranchName,
                ps_name = t.Info.PsName,
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(PsbankDataView viewModel)
        {
            var data = new PsbankData();
            data.InfoId = viewModel.info_id;
            data.BranchId = viewModel.branch_id;
            data.BankAccountNumber = viewModel.bank_account_number;
            data.Description = viewModel.description;
            _GreenPowerContext.PsbankData.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PsbankData.Find(id);

            _GreenPowerContext.PsbankData.Remove(data);
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

        public string EditData(int id, PsbankDataView viewModel)
        {
            var data = _GreenPowerContext.PsbankData.Find(id);
            
            data.InfoId = viewModel.info_id;
            data.BranchId = viewModel.branch_id;
            data.BankAccountNumber = viewModel.bank_account_number;
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
