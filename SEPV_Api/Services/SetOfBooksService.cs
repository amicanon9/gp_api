
using SEPV_Api.Models.GreenPower;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System;


namespace Gp_Api.Services
{
    public class SetOfBooksService : ISetOfBooksService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        private readonly GreenPowerContext _GreenPowerContext;
        public SetOfBooksService (GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
        }

        public List<SetOfBooksViewModel> GetAllData()
        {
            var check = _GreenPowerContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _GreenPowerContext.SetOfBooks.AsQueryable();

            // 如果不是 admin，就加上 BookId 條件
            if (!check.IsAdmin)
            {
                query = query.Where(a => a.BookId == check.BookId);
            }

            return query.Select(t => new SetOfBooksViewModel
            {
                Book_id = t.BookId,
                Name = t.Name,
                Description = t.Description
            }).ToList();
        }

        public SetOfBooksViewModel GetData(short id)
        {
            return _GreenPowerContext.SetOfBooks.Select(t => new SetOfBooksViewModel
            {
                Book_id = t.BookId,
                Name = t.Name,
                Description = t.Description
            }).Where(a => a.Book_id == id).FirstOrDefault();
        }

        public void InsertData(SetOfBooksViewModel viewModel)
        {
            var data = new SetOfBooks
            {
                BookId = viewModel.Book_id,
                Name = viewModel.Name,
                Description = viewModel.Description
            };

            _GreenPowerContext.SetOfBooks.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _GreenPowerContext.SetOfBooks.Find(id);

            _GreenPowerContext.SetOfBooks.Remove(data);
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

        public string EditData(short id, SetOfBooksViewModel viewModel)
        {
            var data = _GreenPowerContext.SetOfBooks.Find(id);
            
            data.Name = viewModel.Name ?? data.Name;
            data.Description = viewModel.Description ?? data.Description;
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
