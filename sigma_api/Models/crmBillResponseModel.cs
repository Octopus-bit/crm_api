using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace sigma_api.Models
{
    public class crmBillResponseModel
    {
        public int Id { get; set; } // dlref
        public int Code { get; set; } // dlcode
        public int Type { get; set; } // DLType
        public string Title { get; set; } // عنوان
        public decimal SumDebit { get; set; } // جمع بدهی
        public decimal SumCredit { get; set; } // جمع بستان کاری
        public decimal DebitBalance { get; set; } // مانده بدهی
        public decimal CreditBalance { get; set; } // مانده بستانکاری
        public ListResponse Turnovers { get; set; }

        public class turnover
        {
            public int Row { get; set; } // ردیف
            public decimal Remaining { get; set; } // باقی مانده 
            public string Date { get; set; } // تاریخ سند
            public string Description { get; set; } // توضیحات
            public decimal Debit { get; set; } // بدهکاری
            public decimal Credit { get; set; } // بستانکاری
        }
    }
}
