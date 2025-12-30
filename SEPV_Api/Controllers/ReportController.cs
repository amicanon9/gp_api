using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Linq;
namespace Gp_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ReportController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("Download")]
        public async Task<IActionResult> DownloadReport([FromBody] Dictionary<string, string> reportRequest)
        {
            if (reportRequest == null || !reportRequest.ContainsKey("reportName"))
                return BadRequest("請提供報表名稱（reportName）。");

            //string reportServerUrl = "http://10.1.3.16/ReportServer";
            string reportServerUrl = "http://192.168.1.80/ReportServer";
            string username = "demo";
            string password = "Spv0901&12";
            string domain = "";

            // 拿出報表名稱並移除它，剩下的都是參數
            string reportName = reportRequest["reportName"];
            reportRequest.Remove("reportName");

            string format = "PDF"; // 你也可以從 reportRequest 抓 format
            if (reportRequest.ContainsKey("format"))
            {
                format = reportRequest["format"];
                reportRequest.Remove("format");
            }

            string reportPath = $"/網頁用報表/{reportName}";

            // 組合查詢參數
            var paramString = string.Join("&", reportRequest.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            string reportUrl = $"{reportServerUrl}?{WebUtility.UrlEncode(reportPath)}";

            if (!string.IsNullOrEmpty(paramString))
                reportUrl += $"&{paramString}";
            reportUrl += $"&rs:Format={format}";

            var handler = new HttpClientHandler
            {
                Credentials = new NetworkCredential(username, password, domain)
            };

            using var client = new HttpClient(handler);
            var response = await client.GetAsync(reportUrl);

            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                return StatusCode((int)response.StatusCode, $"下載報表失敗：{msg}");
            }

            var data = await response.Content.ReadAsByteArrayAsync();

            var formatUpper = format.ToUpper();
            var contentType = formatUpper switch
            {
                "PDF" => "application/pdf",
                "EXCEL" => "application/vnd.ms-excel",
                "EXCELOPENXML" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            var ext = formatUpper switch
            {
                "PDF" => "pdf",
                "EXCEL" => "xls",
                "EXCELOPENXML" => "xlsx",
                _ => "bin"
            };

            var fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.{ext}";
            return File(data, contentType, fileName);
        }


    }
}
