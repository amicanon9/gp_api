using Microsoft.EntityFrameworkCore;
using SEPV_Api.Models.PMS; // 請確保此命名空間包含你的 PMSContext 與 ExpenseClaims Entity
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gp_Api.Services
{
    #region View Models
    public class ExpenseClaimsView
    {
        public int id { get; set; }
        public int user_id { get; set; }
        public string username { get; set; }
        public int book_id { get; set; }
        public string project_type { get; set; }
        public int project_id { get; set; }
        public string category_main { get; set; }
        public string category_item { get; set; }
        public DateTime expense_date { get; set; }
        public string item_name { get; set; }
        public string description { get; set; }

        // 交通費專屬欄位
        public string location_from_to { get; set; }
        public decimal? mileage { get; set; }
        public decimal? subsidy_unit_price { get; set; }
        public decimal? toll_fee { get; set; }
        public decimal? parking_fee { get; set; }

        // 金額欄位
        public decimal? manual_amount { get; set; }
        public decimal total_amount { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public string transportation { get; set; }
    }
    #endregion

    #region Service Interface
    public interface IExpenseClaimsService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        List<ExpenseClaimsView> GetAllData();
        List<ExpenseClaimsView> GetDataByUserId(int id);
        int InsertData(ExpenseClaimsView viewModelList);
        string EditData(int id, ExpenseClaimsView data);
        string DeleteData(int id);
    }
    #endregion

    #region Service Implementation
    public class ExpenseClaimsService : IExpenseClaimsService
    {
        public short UserId { get; set; }
        public short BookId { get; set; }
        private readonly PMSContext _PMSContext;

        public ExpenseClaimsService(PMSContext PMSContext)
        {
            _PMSContext = PMSContext;
        }

        /// <summary>
        /// 取得所有報支紀錄 (管理員或財務視角)
        /// </summary>
        public List<ExpenseClaimsView> GetAllData()
        {
            return _PMSContext.ExpenseClaims
                .Include(t => t.User)
                .OrderByDescending(t => t.ExpenseDate)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => MapToView(t))
                .ToList();
        }

        /// <summary>
        /// 根據當前 UserID 與 BookID 取得個人報支紀錄
        /// </summary>
        public List<ExpenseClaimsView> GetDataByUserId(int id)
        {
            return _PMSContext.ExpenseClaims
                .Include(t => t.User)
                .Where(t => t.UserId == id && t.BookId == BookId)
                .OrderByDescending(t => t.ExpenseDate)
                .Select(t => MapToView(t))
                .ToList();
        }

        /// <summary>
        /// 批次新增報支資料
        /// </summary>
        public int InsertData(ExpenseClaimsView viewModel)
        {
            var data = new ExpenseClaims
            {
                UserId = UserId,
                BookId = BookId,
                ProjectType = viewModel.project_type,
                ProjectId = viewModel.project_id,
                CategoryMain = viewModel.category_main,
                CategoryItem = viewModel.category_item,
                ExpenseDate = viewModel.expense_date,
                ItemName = viewModel.item_name,
                Description = viewModel.description,
                LocationFromTo = viewModel.location_from_to,
                Mileage = viewModel.mileage,
                SubsidyUnitPrice = viewModel.subsidy_unit_price ?? 7,
                TollFee = viewModel.toll_fee ?? 0,
                ParkingFee = viewModel.parking_fee ?? 0,
                ManualAmount = viewModel.manual_amount,
                TotalAmount = viewModel.total_amount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                Transportation = viewModel.transportation,
            };

            _PMSContext.ExpenseClaims.Add(data);
            _PMSContext.SaveChanges(); // 執行完後，data.Id 會自動帶入自增值

            return data.Id; // 把 ID 傳回去
        }

        /// <summary>
        /// 編輯報支資料
        /// </summary>
        public string EditData(int id, ExpenseClaimsView viewModel)
        {
            var data = _PMSContext.ExpenseClaims.Find(id);
            if (data == null) return "NotFound";

            data.ProjectType = viewModel.project_type;
            data.ProjectId = viewModel.project_id;
            data.CategoryMain = viewModel.category_main;
            data.CategoryItem = viewModel.category_item;
            data.ExpenseDate = viewModel.expense_date;
            data.ItemName = viewModel.item_name;
            data.Description = viewModel.description;
            data.LocationFromTo = viewModel.location_from_to;
            data.Mileage = viewModel.mileage;
            data.SubsidyUnitPrice = viewModel.subsidy_unit_price ?? 7;
            data.TollFee = viewModel.toll_fee ?? 0;
            data.ParkingFee = viewModel.parking_fee ?? 0;
            data.ManualAmount = viewModel.manual_amount;

            // 重新計算總金額
            data.TotalAmount = viewModel.total_amount;
            data.UpdatedAt = DateTime.Now;
            data.Transportation = viewModel.transportation;
            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        /// <summary>
        /// 刪除報支資料
        /// </summary>
        public string DeleteData(int id)
        {
            var data = _PMSContext.ExpenseClaims.Find(id);
            if (data == null) return "NotFound";

            _PMSContext.ExpenseClaims.Remove(data);
            try
            {
                _PMSContext.SaveChanges();
                return "OK";
            }
            catch (Exception ex)
            {
                return ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            }
        }

        #region Private Helpers
        // 統一計算金額邏輯
        private decimal CalculateTotal(ExpenseClaimsView item)
        {
            if (item.category_item == "交通費")
            {
                decimal unitPrice = item.subsidy_unit_price ?? 7;
                return ((item.mileage ?? 0) * unitPrice) + (item.toll_fee ?? 0) + (item.parking_fee ?? 0);
            }
            return item.manual_amount ?? 0;
        }

        // Entity 轉 View 模型
        private static ExpenseClaimsView MapToView(ExpenseClaims t)
        {
            return new ExpenseClaimsView
            {
                id = t.Id,
                user_id = t.UserId,
                username=t.User.Username,
                book_id = t.BookId,
                project_type = t.ProjectType,
                project_id = t.ProjectId,
                category_main = t.CategoryMain,
                category_item = t.CategoryItem,
                expense_date = t.ExpenseDate,
                item_name = t.ItemName,
                description = t.Description,
                location_from_to = t.LocationFromTo,
                mileage = t.Mileage,
                subsidy_unit_price = t.SubsidyUnitPrice,
                toll_fee = t.TollFee,
                parking_fee = t.ParkingFee,
                manual_amount = t.ManualAmount,
                total_amount = t.TotalAmount,
                created_at = t.CreatedAt,
                updated_at = t.UpdatedAt,
                transportation = t.Transportation,
            };
        }
        #endregion
    }
    #endregion
}