using SEPV_Api.Models.GreenPower;
using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gp_Api.Services
{
    public class CodeLookupService : ICodeLookupService
    {
        private readonly GreenPowerContext _GreenPowerContext;
        public CodeLookupService(GreenPowerContext sEPVContext)
        {
            _GreenPowerContext = sEPVContext;
        }
        public List<CodeLookupViewModel> GetAllData()
        {
            return _GreenPowerContext.CodeLookup.Select(t => new CodeLookupViewModel
            {
                Source = t.SourceTable,
                Code = t.Code,
                Description = t.Description
            }).ToList();
        }

        public List<CodeLookupViewModel> GetData(string source)
        {
            return _GreenPowerContext.CodeLookup.Select(t => new CodeLookupViewModel
            {
                Source = t.SourceTable,
                Code = t.Code,
                Description = t.Description
            }).Where(a => a.Source == source).ToList();
        }

        public void InsertData(CodeLookupViewModel viewModel)
        {
            var data = new CodeLookup
            {
                SourceTable = viewModel.Source,
                Code = viewModel.Code,
                Description = viewModel.Description
            };

            _GreenPowerContext.CodeLookup.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(string code)
        {
            var data = _GreenPowerContext.CodeLookup.Find(code);
            
            _GreenPowerContext.CodeLookup.Remove(data);
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

        public string EditData(string code, CodeLookupViewModel viewModel)
        {
            var data = _GreenPowerContext.CodeLookup.Find(code);
            
            data.SourceTable = viewModel.Source ?? data.SourceTable;
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

        public List<CodeLookupSelectorViewModel> GetSelector(string source)
        {
            return _GreenPowerContext.CodeLookup.Where(a => a.SourceTable == source)
            .Select(t => new CodeLookupSelectorViewModel
            {
                Code = t.Code,
                Description = t.Description
            }).ToList();
        }
    }
}
