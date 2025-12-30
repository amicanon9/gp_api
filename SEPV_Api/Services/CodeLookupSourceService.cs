using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using SEPV_Api.Models.GreenPower;
using System;

namespace Gp_Api.Services
{
    public class CodeLookupSourceService: ICodeLookupSourceService
    {
        private readonly GreenPowerContext _GreenPowerContext;
        public CodeLookupSourceService(GreenPowerContext GreenPowerContext)
        {
            _GreenPowerContext = GreenPowerContext;
        }
        public List<CodeLookupSourceViewModel> GetAllData()
        {
            return _GreenPowerContext.CodeLookupSource.Select(t => new CodeLookupSourceViewModel
            {
                Source = t.SourceTable,
                Description = t.Description
            }).ToList();
        }

        public List<CodeLookupSourceViewModel> GetData(string source)
        {
            return _GreenPowerContext.CodeLookupSource.Select(t => new CodeLookupSourceViewModel
            {
                Source = t.SourceTable,
                Description = t.Description
            }).Where(a => a.Source == source).ToList();
        }

        public void InsertData(CodeLookupSourceViewModel viewModel)
        {
            var data = new CodeLookupSource
            {
                SourceTable = viewModel.Source,
                Description = viewModel.Description
            };

            _GreenPowerContext.CodeLookupSource.Add(data);
            _GreenPowerContext.SaveChanges();
        }

        public string DeleteData(string source)
        {
            var data = _GreenPowerContext.CodeLookupSource.Find(source);
            
            _GreenPowerContext.CodeLookupSource.Remove(data);
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

        public string EditData(string code, CodeLookupSourceViewModel viewModel)
        {
            var data = _GreenPowerContext.CodeLookupSource.Find(code);
            
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
