using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmItemListResponseModel // اطلاعات کالا
    {
        public int IS_id { get; set; }
        public int StockID { get; set; }
        public string StockTitle { get; set; }
        public int ITM_id { get; set; }
        public string ITM_Code { get; set; }
        public int Sepidar_ItemId { get; set; }
        public string FullName { get; set; }
        public string Brand { get; set; }
        public double DistinctCustomersBrand { get; set; } // درصد تخفیف مشتری روی برند
        public int UnitId { get; set; }
        public string Unit { get; set; }
        public int UnitId2 { get; set; }
        public string Unit2 { get; set; }
        public decimal PricingQ { get; set; }
        public bool ITM_nesabt { get; set; }
        public double ITM_nesbatValue { get; set; }
        public int duty { get; set; }
        public int tax { get; set; }
        public int SupplierId { get; set; }
        public string Supplier { get; set; }
        public int IS_GroupStock { get; set; }
        public string GRS_GroupName { get; set; }
        public string Image { get; set; }
    }
}
