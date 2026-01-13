using SEPV_Api.Models.PMS; // 假設這是你的 Entity Framework Models 命名空間
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore; // 確保有引用此項以支援 Include

namespace Gp_Api.Services
{
    public interface ICustomerPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<CustomerPlmView> GetAllData();
        public void InsertData(CustomerPlmView data);
        public string DeleteData(int id);
        public string EditData(int id, CustomerPlmView data);
    }

    public partial class CustomerPlmView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? tax_id_no { get; set; }

        // 聯絡人資訊 1 ~ 5
        public string contact { get; set; }
        public string telephone { get; set; }
        public string contact2 { get; set; }
        public string telephone2 { get; set; }
        public string contact3 { get; set; }
        public string telephone3 { get; set; }
        public string contact4 { get; set; }
        public string telephone4 { get; set; }
        public string contact5 { get; set; }
        public string telephone5 { get; set; }

        public string decision_level { get; set; }
        public string description { get; set; }
        public string industry_crm { get; set; }
        public string existing_plm { get; set; }
        public string existing_cad { get; set; }
    }

    public class CustomerPlmService : ICustomerPlmService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly PMSContext _PMSContext;

        public CustomerPlmService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<CustomerPlmView> GetAllData()
        {
            return _PMSContext.CustomerPlm.Select(t => new CustomerPlmView
            {
                id = t.Id,
                name = t.Name,
                tax_id_no = t.TaxIdNo,
                contact = t.Contact,
                telephone = t.Telephone,
                contact2 = t.Contact2,
                telephone2 = t.Telephone2,
                contact3 = t.Contact3,
                telephone3 = t.Telephone3,
                contact4 = t.Contact4,
                telephone4 = t.Telephone4,
                contact5 = t.Contact5,
                telephone5 = t.Telephone5,
                decision_level = t.DecisionLevel,
                description = t.Description,
                industry_crm = t.IndustryCrm,
                existing_plm = t.ExistingPlm,
                existing_cad = t.ExistingCad
            }).ToList();
        }

        public void InsertData(CustomerPlmView viewModel)
        {
            var data = new CustomerPlm();
            MapViewModelToEntity(viewModel, data);

            _PMSContext.CustomerPlm.Add(data);
            _PMSContext.SaveChanges();
        }

        public string EditData(int id, CustomerPlmView viewModel)
        {
            var data = _PMSContext.CustomerPlm.Find(id);
            if (data == null) return "NotFound";

            MapViewModelToEntity(viewModel, data);

            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.CustomerPlm.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.CustomerPlm.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException?.Message ?? ex.Message;
            }
        }

        // 抽取出來的 Mapping 邏輯，保持代碼乾淨
        private void MapViewModelToEntity(CustomerPlmView vm, CustomerPlm entity)
        {
            entity.Name = vm.name;
            entity.TaxIdNo = vm.tax_id_no;
            entity.Contact = vm.contact;
            entity.Telephone = vm.telephone;
            entity.Contact2 = vm.contact2;
            entity.Telephone2 = vm.telephone2;
            entity.Contact3 = vm.contact3;
            entity.Telephone3 = vm.telephone3;
            entity.Contact4 = vm.contact4;
            entity.Telephone4 = vm.telephone4;
            entity.Contact5 = vm.contact5;
            entity.Telephone5 = vm.telephone5;
            entity.DecisionLevel = vm.decision_level;
            entity.Description = vm.description;
            entity.IndustryCrm = vm.industry_crm;
            entity.ExistingPlm = vm.existing_plm;
            entity.ExistingCad = vm.existing_cad;
        }
    }
}