using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.IServices
{
    public interface ICodeLookupSourceService
    {
        public List<CodeLookupSourceViewModel> GetAllData();
        public List<CodeLookupSourceViewModel> GetData(string source);
        public void InsertData(CodeLookupSourceViewModel viewModel);
        public string DeleteData(string code);
        public string EditData(string code, CodeLookupSourceViewModel viewModel);
    }
}