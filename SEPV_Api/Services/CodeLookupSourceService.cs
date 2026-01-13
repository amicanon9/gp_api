using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using SEPV_Api.Models.PMS;
using System;

namespace Gp_Api.Services
{
    public class CodeLookupSourceService: ICodeLookupSourceService
    {
        private readonly PMSContext _PMSContext;
        public CodeLookupSourceService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }
        public List<CodeLookupSourceViewModel> GetAllData()
        {
            return _PMSContext.CodeLookupSource.Select(t => new CodeLookupSourceViewModel
            {
                Source = t.SourceTable,
                Description = t.Description
            }).ToList();
        }

        public List<CodeLookupSourceViewModel> GetData(string source)
        {
            return _PMSContext.CodeLookupSource.Select(t => new CodeLookupSourceViewModel
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

            _PMSContext.CodeLookupSource.Add(data);
            _PMSContext.SaveChanges();
        }

        public string DeleteData(string source)
        {
            var data = _PMSContext.CodeLookupSource.Find(source);
            
            _PMSContext.CodeLookupSource.Remove(data);
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

        public string EditData(string code, CodeLookupSourceViewModel viewModel)
        {
            var data = _PMSContext.CodeLookupSource.Find(code);
            
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
