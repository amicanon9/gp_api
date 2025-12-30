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
    public interface IPspowerNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PspowerNoInfoView> GetAllData();
        public void InsertData(PspowerNoInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PspowerNoInfoView data);
    }

    public partial class PspowerNoInfoView
    {
        
        public int id { get; set; }
        public string power_no { get; set; }
        public int? ps_bank_id { get; set; }
        public int? trust_bank_id { get; set; }
        public string description { get; set; }
        public int info_id { get; set; }
        public string etype { get; set; }
        public string site_name { get; set; }
        public string address { get; set; }

        public string ps_name { get; set; }
        public string bank_name { get; set; }
        public string bank_account_number { get; set; }
        public string branch_name { get; set; }
        public string trust_bank_name { get; set; }

    }
    public class PspowerNoInfoService : IPspowerNoInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PspowerNoInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PspowerNoInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles
                .Where(a => a.InfoId == UserId && a.RoleId == RoleId)
                .Select(b => b.Role)
                .FirstOrDefault();

            var query = _GreenPowerContext.PspowerNoInfo
                .Include(t => t.Info)
                .Include(t => t.TrustBank)
                .Include(t => t.PsBank)
                    .ThenInclude(b => b.Branch)
                        .ThenInclude(bb => bb.Bank)
                .AsQueryable();
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.Info.BookId == check.BookId);
            }
            var data  = query.Select(t => new PspowerNoInfoView
            {
                id = t.Id,
                power_no = t.PowerNo,
                ps_bank_id =t.PsBankId,
                trust_bank_id =t.TrustBankId,
                description = t.Description,
                info_id = (int)t.InfoId,
                etype=t.Etype,
                site_name = t.SiteName,
                address =t.Address,
                ps_name=t.Info.PsName,
                bank_name=t.PsBank.Branch.Bank.BankName,
                bank_account_number = t.PsBank.BankAccountNumber,
                trust_bank_name = t.TrustBank.BankName,
                branch_name=t.PsBank.Branch.BranchName
            });
            var allData = data.ToList();
            return allData;
        }



        public void InsertData(PspowerNoInfoView viewModel)
        {
            var data = new PspowerNoInfo();
            data.PowerNo = viewModel.power_no;
            data.PsBankId= viewModel.ps_bank_id;
            data.TrustBankId = viewModel.trust_bank_id;
            data.Description = viewModel.description;
            data.InfoId = viewModel.info_id;
            data.Etype=viewModel.etype;
            data.Address=viewModel.address;
            data.SiteName=viewModel.site_name;
            _GreenPowerContext.PspowerNoInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PspowerNoInfo.Find(id);

            _GreenPowerContext.PspowerNoInfo.Remove(data);
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

        public string EditData(int id, PspowerNoInfoView viewModel)
        {
            var data = _GreenPowerContext.PspowerNoInfo.Find(id);
            
            data.PowerNo = viewModel.power_no;
            data.PsBankId = viewModel.ps_bank_id;
            data.TrustBankId = viewModel.trust_bank_id;
            data.Description = viewModel.description;
            data.InfoId = viewModel.info_id;
            data.Etype = viewModel.etype;
            data.Etype= viewModel.etype;
            data.Address = viewModel.address;
            data.SiteName = viewModel.site_name;
            try
            {
                _GreenPowerContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null
                    ? $"{ex}\nInnerException: {ex.InnerException}"
                    : ex.ToString();
            }

            return "OK";
        }

  

     
    }
}
