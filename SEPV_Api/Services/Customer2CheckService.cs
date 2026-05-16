using SEPV_Api.Models.PMS;
using Gp_Api.IServices;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    public interface ICustomer2CheckService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<Customer2CheckView> GetAllData();
        public void InsertData(Customer2CheckView data);
        public string DeleteData(int id);
        public string EditData(int id, Customer2CheckView data);
    }

    public partial class Customer2CheckView
    {
        public int id { get; set; }
        public string name { get; set; }
        public int? tax_id_no { get; set; }

        public string contact { get; set; }
        public string telephone { get; set; }
        public string email { get; set; }

        public string contact2 { get; set; }
        public string telephone2 { get; set; }
        public string email2 { get; set; }

        public string contact3 { get; set; }
        public string telephone3 { get; set; }
        public string email3 { get; set; }

        public string contact4 { get; set; }
        public string telephone4 { get; set; }
        public string email4 { get; set; }

        public string contact5 { get; set; }
        public string telephone5 { get; set; }
        public string email5 { get; set; }

        public string decision_level { get; set; }
        public string description { get; set; }
    }

    public class Customer2CheckService : ICustomer2CheckService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly PMSContext _PMSContext;

        public Customer2CheckService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<Customer2CheckView> GetAllData()
        {
            return _PMSContext.Customer2Check.Select(t => new Customer2CheckView
            {
                id = t.Id,
                name = t.Name,
                tax_id_no = t.TaxIdNo,
                contact = t.Contact,
                telephone = t.Telephone,
                email = t.Email,
                contact2 = t.Contact2,
                telephone2 = t.Telephone2,
                email2 = t.Email2,
                contact3 = t.Contact3,
                telephone3 = t.Telephone3,
                email3 = t.Email3,
                contact4 = t.Contact4,
                telephone4 = t.Telephone4,
                email4 = t.Email4,
                contact5 = t.Contact5,
                telephone5 = t.Telephone5,
                email5 = t.Email5,
                decision_level = t.DecisionLevel,
                description = t.Description
            }).ToList();
        }

        public void InsertData(Customer2CheckView data)
        {
            var item = new Customer2Check
            {
                Name = data.name,
                TaxIdNo = data.tax_id_no,
                Contact = data.contact,
                Telephone = data.telephone,
                Email = data.email,
                Contact2 = data.contact2,
                Telephone2 = data.telephone2,
                Email2 = data.email2,
                Contact3 = data.contact3,
                Telephone3 = data.telephone3,
                Email3 = data.email3,
                Contact4 = data.contact4,
                Telephone4 = data.telephone4,
                Email4 = data.email4,
                Contact5 = data.contact5,
                Telephone5 = data.telephone5,
                Email5 = data.email5,
                DecisionLevel = data.decision_level,
                Description = data.description
            };
            _PMSContext.Customer2Check.Add(item);
            _PMSContext.SaveChanges();
        }

        public string EditData(int id, Customer2CheckView data)
        {
            var item = _PMSContext.Customer2Check.Find(id);
            if (item == null) return "NotFound";

            item.Name = data.name;
            item.TaxIdNo = data.tax_id_no;
            item.Contact = data.contact;
            item.Telephone = data.telephone;
            item.Email = data.email;
            item.Contact2 = data.contact2;
            item.Telephone2 = data.telephone2;
            item.Email2 = data.email2;
            item.Contact3 = data.contact3;
            item.Telephone3 = data.telephone3;
            item.Email3 = data.email3;
            item.Contact4 = data.contact4;
            item.Telephone4 = data.telephone4;
            item.Email4 = data.email4;
            item.Contact5 = data.contact5;
            item.Telephone5 = data.telephone5;
            item.Email5 = data.email5;
            item.DecisionLevel = data.decision_level;
            item.Description = data.description;

            _PMSContext.SaveChanges();
            return "OK";
        }

        public string DeleteData(int id)
        {
            var item = _PMSContext.Customer2Check.Find(id);
            if (item == null) return "NotFound";
            try
            {
                _PMSContext.Customer2Check.Remove(item);
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (System.Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
