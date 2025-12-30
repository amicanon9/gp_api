using SEPV_Api.Models.GreenPower;

using Gp_Api.IServices;
using Gp_Api.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;

namespace Gp_Api.Services
{
    public interface IImportService
    {
        public short BookId { get; set; }
        public short RoleId { get; set; }
        public short UserId { get; set; }
        public res ImportData(ImportObj data);
        public res ImportServiceData(List<Service> data);
    }
    public class Service
    {
        public string service_no { get; set; }
        public string ps_power_no { get; set; }
        public string ps_meter_no { get; set; }
        public string pp_power_no { get; set; }
        public string pp_meter_no { get; set; }
        public int bill_year { get; set; }
        public int bill_month { get; set; }
        public decimal dispatching_rate { get; set; }
        public decimal distribution_rate { get; set; }
        public decimal fee { get; set; }
        public decimal transmission_rate { get; set; }
        public decimal kwh_usage { get; set; }
        public decimal ancillary_services_rate { get; set; }
    }
    public class ImportObj
    {
        public List<PsListItem> pslist { get; set; }
        public List<PpListItem> pplist { get; set; }
        public List<ServiceListItem> servicelist { get; set; }  // 新增
    }


    public class PsListItem
    {
        public int id { get; set; }
        public string ps_no { get; set; }
        public string ps_name { get; set; }
        public int? tax_id_no { get; set; }
        public string description { get; set; }
        public string email {  get; set; }
        public string contact { get; set; }
        public string telephone { get; set; }
        public List<PspList> pspowerNoInfo { get; set; }
    }

    public class PspList
    {
        public string power_no { get; set; }
        public string meter_no { get; set; }
        public string pp_meter_no { get; set; }
        public int id { get; set; }
        public int? info_id { get; set; }
        public string ps_bank_name { get; set; }
        public string bank_account_number { get; set; }
        public string trust_bank_name { get; set; }
        public string description { get; set; }
        public string etype { get; set; }
        public string address { get; set; }
        public string site_name { get; set; }

    }

    public class PpListItem
    {
        public int id { get; set; }
        public string pp_no { get; set; }
        public string pp_name { get; set; }
        public int? tax_id_no { get; set; }
        public string description { get; set; }
        public string ctype { get; set; }
        public List<PppList> pppowerNoInfo { get; set; }
    }

    public class PppList
    {
        public string power_no { get; set; }
        public int id { get; set; }
        public int? info_id { get; set; }
        public string description { get; set; }
    }

    public class ServiceListItem
    {
        public int id { get; set; }
        public string service_no { get; set; }
        public decimal? ps_total_kwp { get; set; }
        public decimal? pp_rate { get; set; }
        public string description { get; set; }
        public List<ServiceDetailInfo> servicenodetailinfo { get; set; }
    }

    public class ServiceDetailInfo
    {
        public int id { get; set; }
        public decimal? ps_total_kwp { get; set; }
        public string ps_meter_no { get; set; }
        public string pp_meter_no { get; set; }
        public string describetion { get; set; }  // 你前端拼錯了，是 description？
        public decimal? ps_pp_percent { get; set; }
        public decimal? ps_rate { get; set; }
        public string type { get; set; }
    }



    public class res
    {
        public List<string> success { get; set; }
        public List<string> failure { get; set; }

        public class ImportService : IImportService
        {
            public short RoleId { get; set; }
            public short UserId { get; set; }
            public short BookId { get; set; }
            private readonly GreenPowerContext _GreenPowerContext;
            
            public ImportService(GreenPowerContext GreenPowerContext)
            {
                _GreenPowerContext = GreenPowerContext;
                
            }
            public string getPsno()
            {
                var lastPsNo = _GreenPowerContext.PsbasicInfo
                .Where(p => p.PsNo.StartsWith("I"))
                .OrderByDescending(p => p.PsNo)
                .Select(p => p.PsNo)
                .FirstOrDefault();

                string newPsNo;

                if (!string.IsNullOrEmpty(lastPsNo) && lastPsNo.Length == 3)
                {
                    var prefix = lastPsNo.Substring(0, 1); // "I"
                    var number = int.Parse(lastPsNo.Substring(1, 2)); // 01, 02...
                    number++;
                    newPsNo = $"{prefix}{number.ToString("D2")}";
                }
                else
                {
                    // 沒有資料或格式不正確時，從 I01 開始
                    newPsNo = "I01";
                }
                return newPsNo;
            }
            public res ImportData(ImportObj data)
            {
                var res = new res
                {
                    success = new List<string>(),
                    failure = new List<string>()
                };
                foreach (var item in data.pslist) {
                    var ps = new PsbasicInfo();
                  
                    try
                    {
                        ps.BookId = BookId;
                        ps.PsNo = getPsno();
                        ps.PsName = item.ps_name;
                        ps.TaxIdNo = item.tax_id_no;
                        ps.Description = item.description;
                        ps.Email = item.email;
                        ps.Contact = item.contact;
                        ps.Telephone = item.telephone;
                        var exists = _GreenPowerContext.PsbasicInfo.AsNoTracking()
                            .FirstOrDefault(p => p.PsName == item.ps_name&&p.BookId == BookId);

                        if (exists!=null)
                        {
                            res.failure.Add("匯入基本資料略過（已存在）:" + item.ps_name);
                            ps.Id= exists.Id;
                            //exists.TaxIdNo= ps.TaxIdNo;
                            //_GreenPowerContext.PsbasicInfo.Update(exists);
                            //_GreenPowerContext.SaveChanges();
                        }
                        else
                        {
                            _GreenPowerContext.PsbasicInfo.Add(ps);
                            _GreenPowerContext.SaveChanges();
                            res.success.Add("匯入基本資料成功:" + ps.PsName);
                        }

                        
                    }
                    catch (Exception ex)
                    {
                        res.failure.Add("匯入基本資料失敗:" + ps.PsName + " 失敗原因:"+ (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                    }

                    if (item.pspowerNoInfo != null)
                    {
                        foreach (var subitem in item.pspowerNoInfo)
                        {
                            var etype = _GreenPowerContext.CodeLookup.Where(e=>e.SourceTable =="etype").FirstOrDefault(e=>e.Description==subitem.etype)?.Code;
                            var trustBankId = _GreenPowerContext.BankInfo.FirstOrDefault(b => b.BankName == subitem.trust_bank_name)?.Id ?? null;
                            var psBankId = _GreenPowerContext.PsbankData.FirstOrDefault(e => e.Branch.BranchNo == subitem.ps_bank_name && e.BankAccountNumber == subitem.bank_account_number && e.InfoId == ps.Id)?.Id ?? null;
                            if (psBankId == null)
                            {
                                var psb = new PsbankData();
                                psb.BankAccountNumber = subitem.bank_account_number;
                                psb.InfoId=ps.Id;
                                var branchId = _GreenPowerContext.BankBranchInfo.FirstOrDefault(e => e.BranchNo == subitem.ps_bank_name)?.Id??null ;
                                if (branchId != null)
                                {
                                    psb.BranchId = branchId.Value;
                                }
                                else
                                {
                                    res.failure.Add($"找不到分行代碼: {subitem.ps_bank_name}，無法建立購電業銀行資料");
                                    continue; // 或 throw/return 視情況中斷流程
                                }

                                try
                                {
                                    _GreenPowerContext.Add(psb);
                                    _GreenPowerContext.SaveChanges();
                                    res.success.Add("匯入銀行資料成功:" + psb.BankAccountNumber);
                                    psBankId = psb.Id;


                                }
                                catch (Exception ex)
                                {
                                    res.failure.Add("匯入銀行資料失敗:" + psb.BankAccountNumber + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                                }
                               
                            }
                            else
                            {
                                //var pspp = _GreenPowerContext.PspowerNoInfo
                                //.Include(p => p.PsBank)
                                //    .ThenInclude(b => b.Branch)
                                //.FirstOrDefault(p => p.PowerNo == subitem.power_no && p.InfoId == ps.Id);
                                //    if (pspp != null)
                                //    {
                                //    pspp.PsBankId = psBankId;
                                //    _GreenPowerContext.PspowerNoInfo.Update(pspp);
                                //    _GreenPowerContext.SaveChanges();
                                //    }
                            }
                            var psp = new PspowerNoInfo();
                            var psm = new PsmeterNoInfo();
                        
                            try
                            {
                                psm.MeterNo = subitem.meter_no;
                                psp.PowerNo = subitem.power_no;
                                psp.PsBankId = psBankId;
                                psp.TrustBankId = trustBankId;
                                psp.Description = subitem.description;
                                psp.InfoId = ps.Id;
                                psp.Etype = etype;
                                psp.Address = subitem.address;
                                psp.SiteName = subitem.site_name;
                                var exists = _GreenPowerContext.PspowerNoInfo.AsNoTracking()
                                    .FirstOrDefault(p => p.PowerNo == subitem.power_no);

                                if (exists!=null)
                                {
                                    res.failure.Add("匯入電號略過（已存在）:" + subitem.power_no);
                                    psm.PsId = exists.Id;
                                    //exists.Etype = psp.Etype;
                                    //_GreenPowerContext.PspowerNoInfo.Update(exists);
                                    //_GreenPowerContext.SaveChanges();
                                }
                                else
                                {
                                    _GreenPowerContext.Add(psp);
                                    _GreenPowerContext.SaveChanges();
                                    res.success.Add("匯入電號成功:" + psp.PowerNo);
                                    psm.PsId = psp.Id;
                                    
                                }
                                if (subitem.meter_no != null)
                                {
                                    try
                                    {
                                        var test = _GreenPowerContext.PsmeterNoInfo.AsNoTracking()
                                         .Any(p => p.MeterNo == psm.MeterNo);

                                        if (test)
                                        {
                                            res.failure.Add("匯入表號略過（已存在）:" + psm.MeterNo);
                                        }
                                        else
                                        {
                                            _GreenPowerContext.Add(psm);
                                            _GreenPowerContext.SaveChanges();
                                            res.success.Add("匯入表號成功:" + psm.MeterNo);

                                        }

                                    }
                                    catch (Exception ex)
                                    {
                                        res.failure.Add("匯入表號失敗:" + psm.MeterNo + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                                    }

                                }


                            }
                            catch (Exception ex)
                            {
                                res.failure.Add("匯入電號失敗:" + psp.PowerNo + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                            }
                        }
                    }

                }
                foreach (var item in data.servicelist)
                {
                    if(item.service_no != null)
                    {
                        var sv = new ServiceNoInfo();
                        sv.ServiceNo = item.service_no;
                        sv.BookId = BookId;
                        try
                        {
                            var exists = _GreenPowerContext.ServiceNoInfo.AsNoTracking()
                                .FirstOrDefault(p => p.ServiceNo == item.service_no && p.BookId == BookId);

                            if (exists != null)
                            {
                                res.failure.Add("匯入基本資料略過（已存在）:" + item.service_no);
                                sv.Id = exists.Id;
                            }
                            else
                            {
                                _GreenPowerContext.ServiceNoInfo.Add(sv);
                                _GreenPowerContext.SaveChanges();
                                res.success.Add("匯入基本資料成功:" + sv.ServiceNo);
                            }


                        }
                        catch (Exception ex)
                        {
                            res.failure.Add("匯入基本資料失敗:" + sv.ServiceNo + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                        }
                        if (item.servicenodetailinfo != null)
                        {
                            foreach(var subitem in item.servicenodetailinfo)
                            {
                               
                                try
                                {
                                    var svd = new ServiceNoDetailInfo();
                                    var ps_meter = _GreenPowerContext.PsmeterNoInfo.FirstOrDefault(p => p.MeterNo == subitem.ps_meter_no);
                                    var pp_meter = _GreenPowerContext.PpmeterNoInfo.FirstOrDefault(p => p.MeterNo == subitem.pp_meter_no);
                                    if(ps_meter == null)
                                    {
                                        res.failure.Add("查無購電戶表號:" + subitem.ps_meter_no);
                                        continue;
                                    }
                                    if (pp_meter == null)
                                    {
                                        res.failure.Add("查無用電表號:" + subitem.pp_meter_no);
                                        continue;
                                    }
                                    svd.PsTotalKwp = subitem.ps_total_kwp;
                                    svd.PsMeterId = ps_meter.Id;
                                    svd.PpMeterId = pp_meter.Id;
                                    svd.ServiceNoId = sv.Id;
                                    svd.PsPpPercent = subitem.ps_pp_percent;
                                    svd.PsRate = subitem.ps_rate;
                                    svd.Type = subitem.type;
                                    var exists = _GreenPowerContext.ServiceNoDetailInfo.AsNoTracking()
                                     .FirstOrDefault(p => p.ServiceNoId == svd.ServiceNoId && p.PpMeterId == pp_meter.Id && p.PsMeterId ==ps_meter.Id);

                                    if (exists != null)
                                    {
                                        res.failure.Add("匯入服務編號明細略過（已存在）:" + item.service_no);
                                        svd.Id = exists.Id;
                                    }
                                    else
                                    {
                                        _GreenPowerContext.ServiceNoDetailInfo.Add(svd);
                                        _GreenPowerContext.SaveChanges();
                                        res.success.Add("匯入成功:" + item.service_no);
                                    }


                                }
                                catch (Exception ex)
                                {
                                    res.failure.Add("匯入失敗:" + item.service_no + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                                }
                            }
                        }

                    }
                  
                }
                return res;
            }
            public res ImportServiceData(List<Service> data)
            {
                var res = new res
                {
                    success = new List<string>(),
                    failure = new List<string>()
                };
                foreach (var item in data)
                {
                    var s = new ServiceNoDetailData();
                    var detail = _GreenPowerContext.ServiceNoDetailInfo.Where(e => e.PsMeter.MeterNo == item.ps_meter_no && e.PpMeter.MeterNo == item.pp_meter_no && e.ServiceNo.ServiceNo ==item.service_no && e.ServiceNo.BookId == BookId).FirstOrDefault();
                    if (detail == null)
                    {
                        res.failure.Add("查無服務編號明細:"  + item.service_no+"," + item.ps_meter_no + "," + item.pp_meter_no );
                        //detail = new ServiceNoDetailInfo();

                        //var psmeter = _GreenPowerContext.PsmeterNoInfo.FirstOrDefault(e => e.MeterNo == item.ps_meter_no);
                        //var ppmeter = _GreenPowerContext.PpmeterNoInfo.FirstOrDefault(e => e.MeterNo == item.pp_meter_no);
                        //var service = _GreenPowerContext.ServiceNoInfo.FirstOrDefault(e => e.ServiceNo == item.service_no && e.BookId==BookId);
                        //if (service == null)
                        //{
                        //    res.failure.Add("查無服務編號");
                        //    continue;
                        //}
                        //detail.ServiceNoId = service.Id;
                        //if (psmeter == null && item.ps_power_no != null)
                        //{
                        //    var rawNo = item.ps_power_no.Replace("-", "");
                        //    var pspower = _GreenPowerContext.PspowerNoInfo
                        //        .Where(e => e.PowerNo != null && e.PowerNo.Replace("-", "").Contains(rawNo.Substring(0, 5))) // 初步篩
                        //        .AsEnumerable()
                        //        .FirstOrDefault(e => e.PowerNo.Replace("-", "") == rawNo);
                        //    if (pspower != null)
                        //    {
                        //        psmeter = new PsmeterNoInfo();
                        //        psmeter.MeterNo = item.ps_meter_no;
                        //        psmeter.PsId = pspower.Id;
                        //        var Meter = _GreenPowerContext.PsmeterNoInfo.AsNoTracking()
                        //                .FirstOrDefault(p => p.MeterNo == psmeter.MeterNo);

                        //        if (Meter!=null)
                        //        {
                        //            res.failure.Add("匯入表號略過（已存在）:" + psmeter.MeterNo);
                        //            psmeter.Id= Meter.Id;
                        //        }
                        //        else
                        //        {
                        //            try
                        //            {
                        //                _GreenPowerContext.PsmeterNoInfo.Add(psmeter);
                        //                _GreenPowerContext.SaveChanges();
                        //                res.success.Add("匯入購電戶表號成功:" + item.ps_meter_no);

                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                res.failure.Add("匯入購電戶表號失敗:" + item.ps_meter_no + " 失敗原因:"  +(ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        res.failure.Add("查無購電戶電號:" + item.ps_power_no);
                        //        continue;
                        //    }



                        //}
                        //if(ppmeter == null && item.pp_power_no!=null)
                        //{
                        //    var rawNo = item.pp_power_no.Replace("-", "");
                        //    var pppower = _GreenPowerContext.PppowerNoInfo
                        //        .Where(e => e.PowerNo != null && e.PowerNo.Replace("-", "").Contains(rawNo.Substring(0, 5))) // 初步篩
                        //        .AsEnumerable()
                        //        .FirstOrDefault(e => e.PowerNo.Replace("-", "") == rawNo);
                        //    if (pppower != null)
                        //    {
                        //        ppmeter = new PpmeterNoInfo();
                        //        ppmeter.MeterNo = item.pp_meter_no;
                        //        ppmeter.PpId = pppower.Id;
                        //        var pp = _GreenPowerContext.PpmeterNoInfo.AsNoTracking()
                        //                .FirstOrDefault(p => p.MeterNo == ppmeter.MeterNo);

                        //        if (pp!=null)
                        //        {
                        //            res.failure.Add("用電戶表號略過（已存在）:" + ppmeter.MeterNo);
                        //            ppmeter.Id= pp.Id;
                        //        }
                        //        else
                        //        {
                        //            try
                        //            {
                        //                _GreenPowerContext.PpmeterNoInfo.Add(ppmeter);
                        //                _GreenPowerContext.SaveChanges();
                        //                res.success.Add("匯入用電戶表號成功:" + item.pp_meter_no);

                        //            }
                        //            catch (Exception ex)
                        //            {
                        //                res.failure.Add("匯入用電戶表號失敗:" + item.pp_meter_no + " 失敗原因:"  +(ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        res.failure.Add("查無用電戶電號:" + item.pp_power_no);
                        //        continue;
                        //    }


                        //}
                        //detail.PsMeterId = psmeter.Id;
                        //detail.PpMeterId = ppmeter.Id;
                        //var dd = _GreenPowerContext.ServiceNoDetailInfo.AsNoTracking()
                        //            .Any(p => p.PsMeterId == detail.PsMeterId && p.PpMeterId == detail.PpMeterId && p.ServiceNoId == detail.ServiceNoId);

                        //if (dd)
                        //{
                        //    res.failure.Add("明細略過（已存在）:" + item.service_no);
                        //}
                        //else
                        //{
                        //    try
                        //    {
                        //        _GreenPowerContext.ServiceNoDetailInfo.Add(detail);
                        //        _GreenPowerContext.SaveChanges();
                        //        res.success.Add("匯入明細成功:" + item.service_no);

                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        res.failure.Add("匯入明細失敗:" + item.service_no + " 失敗原因:"  +(ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                        //    }
                        //}

                    }
                    else
                    {
                        s.ServiceNoDetailId = detail.Id;
                        s.AncillaryServicesRate = item.ancillary_services_rate;
                        s.BillMonth = (byte)item.bill_month;
                        s.BillYear = (short)item.bill_year;
                        s.DistributionRate = item.distribution_rate;
                        s.DispatchingRate = item.dispatching_rate;
                        s.Fee = (int)item.fee;
                        s.KwhUsage = (int)item.kwh_usage;
                        s.TransmissionRate = item.transmission_rate;
                        var test = _GreenPowerContext.ServiceNoDetailData.AsNoTracking()
                                        .Any(p => p.ServiceNoDetailId == s.ServiceNoDetailId && p.BillMonth == s.BillMonth && p.BillYear == s.BillYear);

                        if (test)
                        {
                            res.failure.Add("明細資料略過（已存在）:" + item.ps_power_no);
                        }
                        else
                        {
                            try
                            {
                                _GreenPowerContext.ServiceNoDetailData.Add(s);
                                _GreenPowerContext.SaveChanges();
                                res.success.Add("匯入資料成功:" + item.ps_power_no);

                            }
                            catch (Exception ex)
                            {
                                res.failure.Add("匯入資料失敗:" + item.ps_power_no + " 失敗原因:" + (ex.InnerException != null ? $"{ex}\nInnerException:{ex.InnerException}" : ex.ToString()));
                            }
                        }
                    }
                 
                   


                }

                return res;
            }
        }
    }
}
