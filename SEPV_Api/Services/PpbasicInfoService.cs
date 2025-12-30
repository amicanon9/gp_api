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
    public interface IPpbasicInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PpbasicInfoView> GetAllData();
        public void InsertData(PpbasicInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PpbasicInfoView data);
     
    }

    public partial class PpbasicInfoView
    {
        
        public int id { get; set; }
        public short book_id { get; set; }
        public string pp_no { get; set; }
        public string pp_name { get; set; }
        public int? tax_id_no { get; set; }
        public string telephone { get; set; }
        public string contact { get; set; }
        public string description { get; set; }
        public string ctype { get; set; }

    }
    public class PpbasicInfoService : IPpbasicInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PpbasicInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PpbasicInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PpbasicInfo.Select(t => new PpbasicInfoView
            {
                id = t.Id,
                book_id = t.BookId,
                pp_no =t.PpNo,
                pp_name =t.PpName,
                contact = t.Contact,
                description = t.Description,
                tax_id_no =t.TaxIdNo,
                telephone = t.Telephone,
                ctype = t.Ctype
            });
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.book_id == check.BookId);
            }
            var allData = query.ToList();
            return allData;
        }

    

        public void InsertData(PpbasicInfoView viewModel)
        {
            var data = new PpbasicInfo();
            data.BookId = viewModel.book_id;
            data.PpNo = viewModel.pp_no;
            data.PpName = viewModel.pp_name;
            data.TaxIdNo = viewModel.tax_id_no;
            data.Telephone = viewModel.telephone;
            data.Contact = viewModel.contact;
            data.Description = viewModel.description;
            data.Ctype=viewModel.ctype;
            _GreenPowerContext.PpbasicInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PpbasicInfo.Find(id);

            _GreenPowerContext.PpbasicInfo.Remove(data);
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

        public string EditData(int id, PpbasicInfoView viewModel)
        {
            var data = _GreenPowerContext.PpbasicInfo.Find(id);
            
            data.BookId = viewModel.book_id;
            data.PpNo = viewModel.pp_no;
            data.PpName = viewModel.pp_name;
            data.TaxIdNo = viewModel.tax_id_no;
            data.Telephone = viewModel.telephone;
            data.Contact = viewModel.contact;
            data.Description = viewModel.description;
            data.Ctype = viewModel.ctype;
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
