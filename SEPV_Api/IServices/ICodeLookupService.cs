using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.IServices
{
    public interface ICodeLookupService
    {
        public List<CodeLookupViewModel> GetAllData();
        public List<CodeLookupViewModel> GetData(string source);
        public void InsertData(CodeLookupViewModel viewModel);
        public string DeleteData(string code);
        public string EditData(string code, CodeLookupViewModel viewModel);
        public List<CodeLookupSelectorViewModel> GetSelector(string source);
    }
}
