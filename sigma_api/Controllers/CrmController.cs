using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.ModelView;
using sigma_api.Models;
using Newtonsoft.Json;
using sigma_api.Classes;
using sigma_api.Security;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace sigma_api.Controllers
{
    public class CrmController : Controller
    {
        cls_public cp = new cls_public();
        crmLoginResponseModelView response_login;
        crmRegistryCodeResponseModelView response_RegistryCode;
        crmItemListResponseModelView response_items;
        crmQuotationsResponseModelView response_quotations;
        generalInfoCustomerResponseModelView response_InfoCustomer;

        [HttpPost]
        [Route("api/[controller]/login")]
        [AllowAnonymous]
        public string login(crmLoginRequestModelView request)
        {
            string username, password;
            username = request.username;
            password = coding.SigmaEncryptString(/*coding.SafeFarsiStr(*/request.password/*)*/);
            SqlParameter[] parameters =
            {
                new SqlParameter("username",username),
                new SqlParameter("password",password),
                new SqlParameter("Type",1),
            };
            DataTable dt = cp.select_row("API_CheckUser", parameters);
            if (cls_public.sqlError.success == false)
            {
                response_login = new crmLoginResponseModelView(ErrorCode.DBO_Unavailable.GetHashCode(), cls_public.sqlError.message);
            }
            else
            {
                if (dt.Rows.Count == 1)
                {
                    int ID = dt.Rows[0]["ID"].ToInt();
                    string FirstName = dt.Rows[0]["FirstName"].ToString();
                    string LastName = dt.Rows[0]["LastName"].ToString();
                    string Birthdate = dt.Rows[0]["Birthdate"].ToString();
                    string MeliCode = dt.Rows[0]["MeliCode"].ToString();
                    string Email = dt.Rows[0]["Email"].ToString();
                    string Type = dt.Rows[0]["Type"].ToString();
                    bool Status = dt.Rows[0]["Status"].ToBoolean();
                    if (Status == false)
                    {
                        response_login = new crmLoginResponseModelView(ErrorCode.NoAccess.GetHashCode(), "حساب کاربری غیرفعال میباشد");
                    }
                    else
                    {
                        string token = TokenManagement.WriteToken(ID, TypeToken.customer.GetHashCode());
                        if (TokenManagement.IsCreated)
                        {
                            crmLoginResponseModel data = new crmLoginResponseModel();
                            data.id = token;
                            data.Type = Type;
                            data.FirstName = FirstName;
                            data.LastName = LastName;
                            data.MeliCode = MeliCode;
                            data.Email = Email;
                            data.Birthdate = Birthdate;
                            response_login = new crmLoginResponseModelView(data);
                            HttpContext.Response.Headers.Add("Token", token);
                        }
                        else
                        {
                            response_login =
                                new crmLoginResponseModelView(ErrorCode.Authentication.GetHashCode(),
                                    "احراز هویت با مشکل مواجه شد");
                        }

                    }
                }
                else
                {
                    response_login = new crmLoginResponseModelView(ErrorCode.InvalidUser.GetHashCode(), "نام کاربری یا کلمه عبور اشتباه میباشد");
                }
            }
            return JsonConvert.SerializeObject(response_login);
        }

        [HttpPost("api/[controller]/create-account")]
        [AllowAnonymous]
        public string signup()
        {
            return "";
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/get-info-customer")]
        public string getInfoCustomer()
        {
            messagebase mes = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters2 =
            {
                new SqlParameter("CustomerId", UserId),
            };
            DataTable dt_InfoCustomer = cp.select_row("API_GetCustomerInfoById", parameters2);
            if (dt_InfoCustomer.Rows.Count > 0)
            {
                generalInfoCustomerResponseModel data = new generalInfoCustomerResponseModel();
                data.Id = dt_InfoCustomer.Rows[0]["ID"].ToInt();
                data.FirstName = dt_InfoCustomer.Rows[0]["FirstName"].ToString();
                data.LastName = dt_InfoCustomer.Rows[0]["LastName"].ToString();
                data.FullName = dt_InfoCustomer.Rows[0]["fullname"].ToString(); ;
                data.Birthdate = dt_InfoCustomer.Rows[0]["Birthdate"].ToString();
                data.MeliCode = dt_InfoCustomer.Rows[0]["MeliCode"].ToString();
                data.Email = dt_InfoCustomer.Rows[0]["Email"].ToString();
                data.Type = dt_InfoCustomer.Rows[0]["Type"].ToString();
                data.TypeId = dt_InfoCustomer.Rows[0]["TypeId"].ToInt();
                data.Mobile = dt_InfoCustomer.Rows[0]["Mobile"].ToString();
                data.Address = dt_InfoCustomer.Rows[0]["Address"].ToString();
                data.Status = dt_InfoCustomer.Rows[0]["Status"].ToInt();
                data.SalesExpert = dt_InfoCustomer.Rows[0]["SalesExpert"].ToInt();
                data.SalesExpert_FullName = dt_InfoCustomer.Rows[0]["SalesExpert_name"].ToString();
                response_InfoCustomer = new generalInfoCustomerResponseModelView(data);
            }
            else
            {
                response_InfoCustomer = new generalInfoCustomerResponseModelView(null);
            }
            return JsonConvert.SerializeObject(response_InfoCustomer);
        }

        [HttpPost]
        [Route("api/[controller]/forgot-password")]
        [AllowAnonymous]
        public string RegistryCode(crmRegistryCodeRequestModelView request)
        {
            string mobile;
            string code;
            mobile = request.mobile;
            try
            {
                code = request.code.ToString();
            }
            catch
            {
                code = null;
            }

            if (mobile.StartsWith("98"))
            {
                mobile = "+" + mobile;
                mobile = mobile.Replace("+98", "0");
            }

            if (mobile.StartsWith("+98"))
            {
                mobile = mobile.Replace("+98", "0");
            }

            SqlParameter[] parameters2 =
            {
                new SqlParameter("Mobile", mobile),
                new SqlParameter("RegistryCode", string.IsNullOrEmpty(code)?DBNull.Value:code),
            };
            string res_code = cp.insert_getid("API_RegistryCodeUsers", parameters2).ToString();
            if (res_code.StartsWith("1-"))
            {
                int RandCode = res_code.Split('-')[1].ToInt();
                clsSms.send(mobile, $"کد تایید شرکت سیگما : {RandCode}");
                response_RegistryCode = new crmRegistryCodeResponseModelView(true, ErrorCode.Send.GetHashCode(), "code generated");
            }
            else
            {
                if (res_code.ToInt() == 0)
                {
                    response_RegistryCode = new crmRegistryCodeResponseModelView(false, ErrorCode.Conflict.GetHashCode(),
                        "تایید حساب کاربری با مشکل مواجه شده است");
                }
                else if (res_code.ToInt() > 1)
                {

                    string token = TokenManagement.WriteToken(res_code.ToInt(), TypeToken.customer.GetHashCode());
                    if (TokenManagement.IsCreated)
                    {
                        crmLoginResponseModel data = new crmLoginResponseModel();
                        data.id = token;
                        response_login = new crmLoginResponseModelView(data);
                        response_RegistryCode = new crmRegistryCodeResponseModelView(true, ErrorCode.Created.GetHashCode(), "registered", data);
                    }
                    else
                    {
                        response_RegistryCode = new crmRegistryCodeResponseModelView(false, ErrorCode.Conflict.GetHashCode(),
                            "تایید حساب کاربری با مشکل مواجه شده است");
                    }
                }
            }

            return JsonConvert.SerializeObject(response_RegistryCode);
        }

        [Authorize]
        [HttpPut]
        [Route("api/[controller]/reset-password")]
        public string CheangPassword([Required] string NewPassword)
        {
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            if (UserId == 0)
            {

                m.code = ErrorCode.Authentication.GetHashCode();
                m.success = false;
                m.message = "عدم احراز هویت";

                return JsonConvert.SerializeObject(m);
            }
            SqlParameter[] parameters =
            {
                new SqlParameter("UserId", UserId),
                new SqlParameter("Password", coding.SigmaEncryptString(NewPassword)),
            };
            if (cp.update("API_CheangPasswordUser", parameters))
            {
                m.code = ErrorCode.Created.GetHashCode();
                m.success = true;
                m.message = "کلمه عبور با موفقیت تغییر کرد";
            }
            else
            {
                m.code = ErrorCode.Conflict.GetHashCode();
                m.success = false;
                m.message = "تغییر کلمه عبور با مشکل مواجه شد";
            }
            return JsonConvert.SerializeObject(m);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Items")]
        public string ItemList(int page_number = 1)
        {
            if (page_number < 1)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            }
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmItemListResponseModel> ItemList = new List<crmItemListResponseModel>();
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            DataTable dt_items = new DataTable();
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("item_id",0),
                new SqlParameter("CustomerId",UserId)
            };
            dt_items = cp.select("API_ListItems");
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_items.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {

                Pages = dt_items.Rows.Count / 10;
                dt_items = dt_items.AsEnumerable().Skip((page_number - 1) * 50).CopyToDataTable();
                dt_items = dt_items.AsEnumerable().Take(50).CopyToDataTable();
                foreach (DataRow row in dt_items.Rows)
                {
                    ItemList.Add(new crmItemListResponseModel()
                    {
                        IS_id = row["IS_id"].ToInt(),
                        StockID = row["StockID"].ToInt(),
                        StockTitle = row["StockTitle"].ToString(),
                        ITM_id = row["ITM_id"].ToInt(),
                        ITM_Code = row["ITM_Code"].ToString(),
                        Sepidar_ItemId = row["Sepidar_ItemId"].ToInt(),
                        FullName = row["FullName"].ToString(),
                        Brand = row["Brand"].ToString(),
                        DistinctCustomersBrand = row["DistinctCustomersBrand"].ToDouble(),
                        UnitId = row["UnitId"].ToInt(),
                        Unit = row["Unit"].ToString(),
                        UnitId2 = row["UnitId"].ToInt(),
                        Unit2 = row["Unit2"].ToString(),
                        PricingQ = row["PricingQ"].ToDecimal(),
                        ITM_nesabt = row["ITM_nesabt"].ToBoolean(),
                        ITM_nesbatValue = row["ITM_nesbatValue"].ToDouble(),
                        duty = row["duty"].ToInt(),
                        tax = row["tax"].ToInt(),
                        SupplierId = row["SupplierId"].ToInt(),
                        Supplier = row["Supplier"].ToString(),
                        IS_GroupStock = row["IS_GroupStock"].ToInt(),
                        GRS_GroupName = row["GRS_GroupName"].ToString(),
                        Image = row["Image"].ToString(),
                    });
                }

                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = page_number,
                    results = ItemList
                };

            }
            response_items = new crmItemListResponseModelView(ItemList.Count <= 0 ? null : list);
            return JsonConvert.SerializeObject(response_items);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Item/{ItemId?}")]
        public string GetItem(int? ItemId)
        {
            if (ItemId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "ItemId ارائه نشده است.");
            }
            crmItemListResponseModel ItemInfo = null;
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("item_id",ItemId),
                    new SqlParameter("CustomerId",UserId)
            };
            DataTable dt_items = new DataTable();
            dt_items = cp.select_row("API_ListItems", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_items.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                ItemInfo = new crmItemListResponseModel()
                {
                    IS_id = dt_items.Rows[0]["IS_id"].ToInt(),
                    StockID = dt_items.Rows[0]["StockID"].ToInt(),
                    StockTitle = dt_items.Rows[0]["StockTitle"].ToString(),
                    ITM_id = dt_items.Rows[0]["ITM_id"].ToInt(),
                    ITM_Code = dt_items.Rows[0]["ITM_Code"].ToString(),
                    Sepidar_ItemId = dt_items.Rows[0]["Sepidar_ItemId"].ToInt(),
                    FullName = dt_items.Rows[0]["FullName"].ToString(),
                    Brand = dt_items.Rows[0]["Brand"].ToString(),
                    DistinctCustomersBrand = dt_items.Rows[0]["DistinctCustomersBrand"].ToDouble(),
                    UnitId = dt_items.Rows[0]["UnitId"].ToInt(),
                    Unit = dt_items.Rows[0]["Unit"].ToString(),
                    UnitId2 = dt_items.Rows[0]["UnitId"].ToInt(),
                    Unit2 = dt_items.Rows[0]["Unit2"].ToString(),
                    PricingQ = dt_items.Rows[0]["PricingQ"].ToDecimal(),
                    ITM_nesabt = dt_items.Rows[0]["ITM_nesabt"].ToBoolean(),
                    ITM_nesbatValue = dt_items.Rows[0]["ITM_nesbatValue"].ToDouble(),
                    duty = dt_items.Rows[0]["duty"].ToInt(),
                    tax = dt_items.Rows[0]["tax"].ToInt(),
                    SupplierId = dt_items.Rows[0]["SupplierId"].ToInt(),
                    Supplier = dt_items.Rows[0]["Supplier"].ToString(),
                    IS_GroupStock = dt_items.Rows[0]["IS_GroupStock"].ToInt(),
                    GRS_GroupName = dt_items.Rows[0]["GRS_GroupName"].ToString(),
                    Image = dt_items.Rows[0]["Image"].ToString(),
                };
            }
            response_items = new crmItemListResponseModelView(ItemInfo);
            return JsonConvert.SerializeObject(response_items);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Quotations")]
        public string GetQuotations(int page_number = 1)
        {
            if (page_number < 1)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            }
            int Pages = 1;
            ListResponse list = new ListResponse();
            List<crmQuotationsResponseModel> Quotations = new List<crmQuotationsResponseModel>();
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("Customer_id",UserId)
            };
            DataTable dt_Quotations = new DataTable();
            dt_Quotations = cp.select_row("API_ListQuotations", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0 & cls_public.sqlError.message != null)
            {
                return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                Pages = dt_Quotations.Rows.Count / 10;
                dt_Quotations = dt_Quotations.AsEnumerable().Skip((page_number - 1) * 10).CopyToDataTable();
                dt_Quotations = dt_Quotations.AsEnumerable().Take(10).CopyToDataTable();
                foreach (DataRow row in dt_Quotations.Rows)
                {
                    Quotations.Add(new crmQuotationsResponseModel()
                    {
                        QU_id = row["QU_id"].ToInt(),
                        QU_no = row["QU_no"].ToInt(),
                        RB_no = row["RB_no"].ToInt(), // چون nullable هست
                        QU_date = row["dateQ"].ToString(),
                        Customer = row["CustomerName"].ToString(),
                        Supplier = row["TaminKonandeh"].ToString(),
                        TypeSale = row["TypeSale"].ToString(),
                        TypePayment = row["how_pay"].ToString(),
                        NetPrice = row["QU_NetPrice"].ToDecimal(),
                        City = row["city_name"].ToString(),
                        value = row["QUIvalue"].ToDecimal(),
                        valueSecondary = row["QUIvalueSecondary"].ToDecimal(),
                        SalesExpert = row["Karshenas"].ToString(),
                        Status = row["QUTypeTitle"].ToString(),
                        StatusId = row["QUType"].ToInt(),
                        TypeSaleId = row["QU_typesale"].ToInt(),
                        DLCode = row["DLCode"].ToInt(),
                        SalesExpertId = row["QU_SalesExpert"].ToInt(),
                        CustomerId = row["QU_cst_id"].ToInt(),
                        Tell = row["tel_tell"].ToString(),
                        Details = Array.Empty<crmQuotationsResponseModel.Detail>() // فعلاً خالی
                    });
                }
                list = new ListResponse()
                {
                    Pages = Pages,
                    Page_row = page_number,
                    results = Quotations
                };

            }
            response_quotations = new crmQuotationsResponseModelView(Quotations.Count <= 0 ? null : list);
            return JsonConvert.SerializeObject(response_quotations);
        }



        [Authorize]
        [HttpGet]
        [Route("api/[controller]/Quotation/{QuotationId?}")]
        public string GetQuotations(int? QuotationId)
        {
            if (QuotationId == null)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "QuotationId ارائه نشده است.");
            }
            crmQuotationsResponseModel Quotations = null;
            messagebase m = new messagebase();
            int UserId = HttpContext.Items["UserId"].ToInt();
            SqlParameter[] parameters = new SqlParameter[]
            {
                    new SqlParameter("Customer_id",UserId)
            };
            DataTable dt_Quotations = new DataTable();
            dt_Quotations = cp.select_row("API_ListQuotations", parameters);
            if (cls_public.sqlError.success == false)
            {
                return messagebase.messages(false, ErrorCode.DBO_Unavailable.GetHashCode(), "DBO_Unavailable");
            }
            if (dt_Quotations.Rows.Count <= 0)
            {
                if (cls_public.sqlError.message != null)
                    return messagebase.messages(false, ErrorCode.DBO_Error.GetHashCode(), "DBO_Error");
            }
            else
            {
                DataView dv = new DataView(dt_Quotations);
                dv.RowFilter = $"QU_id = {QuotationId}";
                dt_Quotations = dv.ToTable();
                if (dt_Quotations.Rows.Count > 0)
                {
                    DataTable dt_QuItems = cp.select_row("API_QuotationItems",
                        new SqlParameter[] { new SqlParameter("QuotationId", QuotationId) });
                    List<crmQuotationsResponseModel.Detail> details = new List<crmQuotationsResponseModel.Detail>();
                    foreach (DataRow row in dt_QuItems.Rows)
                    {
                        details.Add(new crmQuotationsResponseModel.Detail
                        {
                            id = row["QUI_id"].ToInt(),
                            Code = row["Code"].ToString(),
                            StockTitle = row["stocktitle"].ToString(),
                            Title = row["Title"].ToString(),
                            SupplierId = row["ITM_Supplier"].ToString(),
                            SupplierName = row["Supplier"].ToString(),
                            Unit1 = row["vahed"].ToString(),
                            value = row["QUI_value"].ToDouble(),
                            valueSecondary = row["QUI_valueSecondary"].ToDouble(),
                            fee = row["QUI_fee"].ToDouble(),
                            Price = row["QUI_Price"].ToDouble(),
                            Discount = row["QUI_Discount"].ToDouble(),
                            DistinctPercent = row["DistinctPercent"].ToDouble(),
                            Tax = row["tax"].ToDouble(),
                            Duty = row["duty"].ToDouble(),
                            Addition = row["addition"].ToDouble(),
                            NetPrice = row["QUI_NetPrice"].ToDouble(),
                            Unit2 = row["unit2"].ToString(),
                            Nesabt = row["nesabt"].ToString(),
                            NesbatValue = row["nesbatValue"].ToDouble(),
                            Item_tax = row["ITM_tax"].ToDouble(),
                            Item_duty = row["ITM_duty"].ToDouble(),
                            Item_id = row["QUI_it_id"].ToInt(),
                            StockId = row["QUI_stock"].ToString(),
                            DistinctCustomersBrand = row["DistinctCustomersBrand"].ToString(),
                            ItemStock_Id = row["IS_id"].ToString(),
                            ItemBrand = row["itembrand"].ToString()
                        });
                    }

                    Quotations = new crmQuotationsResponseModel()
                    {
                        QU_id = dt_Quotations.Rows[0]["QU_id"].ToInt(),
                        QU_no = dt_Quotations.Rows[0]["QU_no"].ToInt(),
                        RB_no = dt_Quotations.Rows[0]["RB_no"].ToInt(), // چون nullable هست
                        QU_date = dt_Quotations.Rows[0]["dateQ"].ToString(),
                        Customer = dt_Quotations.Rows[0]["CustomerName"].ToString(),
                        Supplier = dt_Quotations.Rows[0]["TaminKonandeh"].ToString(),
                        TypeSale = dt_Quotations.Rows[0]["TypeSale"].ToString(),
                        TypePayment = dt_Quotations.Rows[0]["how_pay"].ToString(),
                        NetPrice = dt_Quotations.Rows[0]["QU_NetPrice"].ToDecimal(),
                        City = dt_Quotations.Rows[0]["city_name"].ToString(),
                        value = dt_Quotations.Rows[0]["QUIvalue"].ToDecimal(),
                        valueSecondary = dt_Quotations.Rows[0]["QUIvalueSecondary"].ToDecimal(),
                        SalesExpert = dt_Quotations.Rows[0]["Karshenas"].ToString(),
                        Status = dt_Quotations.Rows[0]["QUTypeTitle"].ToString(),
                        StatusId = dt_Quotations.Rows[0]["QUType"].ToInt(),
                        TypeSaleId = dt_Quotations.Rows[0]["QU_typesale"].ToInt(),
                        DLCode = dt_Quotations.Rows[0]["DLCode"].ToInt(),
                        SalesExpertId = dt_Quotations.Rows[0]["QU_SalesExpert"].ToInt(),
                        CustomerId = dt_Quotations.Rows[0]["QU_cst_id"].ToInt(),
                        Tell = dt_Quotations.Rows[0]["tel_tell"].ToString(),
                        Details = details.ToArray() // فعلاً خالی
                    };
                }

            }
            response_quotations = new crmQuotationsResponseModelView(Quotations);

            return JsonConvert.SerializeObject(response_quotations);
        }

        [Authorize]
        [HttpGet]
        [Route("api/[controller]/bill")]
        public string GetBill(int page_number = 1)
        {
            if (page_number < 1)
            {
                return messagebase.messages(false, ErrorCode.BadRequest.GetHashCode(), "شماره صفحه وارد شده یافت نشد");
            }
            int UserId = HttpContext.Items["UserId"].ToInt();
            int Pages = 1;
            ListResponse list = new ListResponse();
            string DataMali = clsProperties.GetPropertise("DataMali").ToString();
            DateTime DTNOW = cp.GetDateTime().Date;
            DateTime DTNOW2 = DTNOW;
            DataTable customerkoli2;

            string DTNOWStr = ShamsiDate.MiladiToShamsi(DTNOW);
            DTNOWStr = DTNOWStr.Substring(0, 4) + "/01/01";
            DTNOW = ShamsiDate.Shamsi2Miladi(DTNOWStr);
            string DTNOWStr2 = ShamsiDate.MiladiToShamsi(DTNOW2.Date);
            try
            {
                string DTNOWStr3 = DTNOWStr2.Substring(0, 4) + "/12/30";
                DTNOW2 = ShamsiDate.Shamsi2Miladi(DTNOWStr3);
            }
            catch
            {
                string DTNOWStr3 = DTNOWStr2.Substring(0, 4) + "/12/29";
                DTNOW2 = ShamsiDate.Shamsi2Miladi(DTNOWStr3);
            }
            SqlDataAdapter _DA;
            DataSet _DS = new DataSet();
            _DA = new SqlDataAdapter();
            string text = @"SELECT DLRef, DLCode, DLTitle,  SumDebit, SumCredit,DLType,
    CASE WHEN Balance > 0 THEN Balance ELSE 0 END DebitBalance,
    CASE WHEN Balance < 0 THEN ABS(Balance)	ELSE 0 END CreditBalance
FROM
    (
    SELECT DLRef, DLCode, DLTitle, DLTitle_En,DLType,
        ISNULL(SUM(VI.Debit), 0) SumDebit, ISNULL(SUM(VI.Credit), 0) SumCredit,
        ISNULL(SUM(VI.Debit), 0) - ISNULL(SUM(VI.Credit), 0) Balance,
        IsCustomer, IsVendor
    FROM " + DataMali + @".ACC.vwVoucherItem VI
    WHERE 
			(VI.DLRef IS NOT NULL)
        AND (VI.VoucherDate            >=  @FromDate      OR -1 = @FromDate)
        AND (VI.VoucherDate            <=  @ToDate        OR -1 = @ToDate)
        AND ((VI.VoucherType IN (1, 2, 6)) --General, OtherSystems, CurrencyConversion
            OR  (VoucherType =  N'5'       AND VoucherDate <= @LastDayOfFirstYear)
            OR  (VoucherType =  -1       AND VoucherDate >= @FirstDayOfLastYear)
            OR  (VoucherType =  -1)
            )
    GROUP BY DLRef, DLCode, DLTitle, DLTitle_En,DLType, IsCustomer, IsVendor   
--having DLRef=(SELECT csT_dl FROM dbo.PRG_Customers WHERE cst_id = " + UserId + @")
having DLRef=5967
) AS Moror";
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@FromDate",ShamsiDate.Shamsi2Miladi("1403/01/01")/*DTNOW*/ ),
                new SqlParameter("@ToDate", ShamsiDate.Shamsi2Miladi("1403/12/29")/*DTNOW2*/),
                new SqlParameter("@LastDayOfFirstYear",ShamsiDate.Shamsi2Miladi( "1403/01/01")/*DTNOW*/),
                new SqlParameter("@FirstDayOfLastYear",ShamsiDate.Shamsi2Miladi("1403/12/29")/*DTNOW2*/ ),
            };
            DataTable dtmoror = cp.select_command(text, parameters);


            text = @"SELECT   [RowNumber] ,[VoucherDate] ,[Description] ,[Debit] ,[Credit]
FROM " + DataMali + @".ACC.[vwVoucherItem] vi
LEFT JOIN (SELECT    g.VoucherRef   ,g1.Number   ,g1.FromVoucherNumber   ,g1.ToVoucherNumber
  FROM " + DataMali + @".ACC.vwGLVoucher g1
  INNER JOIN " + DataMali + @".ACC.vwGLVoucherItem g
    ON g1.GLVoucherId = g.GLVoucherRef) GLVoucher
  ON GLVoucher.VoucherRef = vi.VoucherRef
WHERE 
 (VoucherDate >= @FromDate )
AND (VoucherDate <= @ToDate)
AND ((VoucherType IN (1, 2, 6)) OR (VoucherType = N'5' AND VoucherDate <= @LastDayOfFirstYear) OR (VoucherType = N'-1' AND VoucherDate >= @FirstDayOfLastYear) OR (VoucherType = N'-1'))
AND DLRef=5967";
            SqlParameter[] parameters2 = new SqlParameter[]
            {
                new SqlParameter("@FromDate",ShamsiDate.Shamsi2Miladi("1403/01/01")/*DTNOW*/ ),
                new SqlParameter("@ToDate", ShamsiDate.Shamsi2Miladi("1403/12/29")/*DTNOW2*/),
                new SqlParameter("@LastDayOfFirstYear",ShamsiDate.Shamsi2Miladi( "1403/01/01")/*DTNOW*/),
                new SqlParameter("@FirstDayOfLastYear",ShamsiDate.Shamsi2Miladi("1403/12/29")/*DTNOW2*/ ),
            };
            DataTable dt_ditales = cp.select_command(text, parameters2);
            return JsonConvert.SerializeObject(list);
        }
    }
}
