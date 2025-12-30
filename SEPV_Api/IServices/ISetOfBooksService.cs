
using Gp_Api.Models.ViewModels;
using System.Collections.Generic;


namespace Gp_Api.IServices
{
    public interface ISetOfBooksService
    {
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public List<SetOfBooksViewModel> GetAllData();
        public SetOfBooksViewModel GetData(short id);
        public void InsertData(SetOfBooksViewModel viewModel);
        public string DeleteData(int id);
        public string EditData(short id, SetOfBooksViewModel viewModel);
    }
}
