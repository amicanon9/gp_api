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
    public interface IPsbasicInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<PsbasicInfoView> GetAllData();
        public void InsertData(PsbasicInfoView data);
        public string DeleteData(int id);
        public string EditData(int id, PsbasicInfoView data);
     
    }

    public partial class PsbasicInfoView
    {
        
        public int id { get; set; }
        public short book_id { get; set; }
        public string ps_no { get; set; }
        public string ps_name { get; set; }
        public int? tax_id_no { get; set; }
        public string telephone { get; set; }
        public string contact { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public string book_name { get; set; }

    }
    public class PsbasicInfoService : IPsbasicInfoService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        
        public PsbasicInfoService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
            
        }

        public List<PsbasicInfoView> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.PsbasicInfo.Include(t => t.Book).Select(t => new PsbasicInfoView
            {
                id = t.Id,
                book_id = t.BookId,
                ps_no =t.PsNo,
                ps_name =t.PsName,
                contact = t.Contact,
                description = t.Description,
                tax_id_no =t.TaxIdNo,
                telephone = t.Telephone,
                email = t.Email,
                book_name=t.Book.Name,
            });
            // 如果不是 admin 才加上條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.book_id == check.BookId);
            }
            var allData = query.ToList();
            return allData;
        }

    

        public void InsertData(PsbasicInfoView viewModel)
        {
            var data = new PsbasicInfo();
            data.BookId = viewModel.book_id;
            data.PsNo = viewModel.ps_no;
            data.PsName = viewModel.ps_name;
            data.TaxIdNo = viewModel.tax_id_no;
            data.Telephone = viewModel.telephone;
            data.Contact = viewModel.contact;
            data.Description = viewModel.description;
            data.Email = viewModel.email;
            _GreenPowerContext.PsbasicInfo.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.PsbasicInfo.Find(id);

            _GreenPowerContext.PsbasicInfo.Remove(data);
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

        public string EditData(int id, PsbasicInfoView viewModel)
        {
            var data = _GreenPowerContext.PsbasicInfo.Find(id);
            
            data.BookId = viewModel.book_id;
            data.PsNo = viewModel.ps_no;
            data.PsName = viewModel.ps_name;
            data.TaxIdNo = viewModel.tax_id_no;
            data.Telephone = viewModel.telephone;
            data.Contact = viewModel.contact;
            data.Description = viewModel.description;
            data.Email= viewModel.email;
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
