
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
        private readonly PMSContext _PMSContext;
        public SetOfBooksService (PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        public List<SetOfBooksViewModel> GetAllData()
        {
            var check = _PMSContext.LoginInfoRoles.Where(a => a.InfoId == UserId && a.RoleId == RoleId).Select(b => b.Role).FirstOrDefault();
            var query = _PMSContext.SetOfBooks.AsQueryable();

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
            return _PMSContext.SetOfBooks.Select(t => new SetOfBooksViewModel
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

            _PMSContext.SetOfBooks.Add(data);
            _PMSContext.SaveChanges();
        }

        public string DeleteData(int id)
        {
            var data = _PMSContext.SetOfBooks.Find(id);

            _PMSContext.SetOfBooks.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }

        public string EditData(short id, SetOfBooksViewModel viewModel)
        {
            var data = _PMSContext.SetOfBooks.Find(id);
            
            data.Name = viewModel.Name ?? data.Name;
            data.Description = viewModel.Description ?? data.Description;
            try
            {
                _PMSContext.SaveChanges();
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}": ex.ToString();

            }

            return "OK";
        }
    }
}
