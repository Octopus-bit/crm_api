using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using sigma_api.classes;
using sigma_api.Security;

namespace sigma_api.Classes
{
    public class clsRequestBuys
    {
        public string MessageError = "";
        DateTime dtdate;
        SqlCommand command;
        SqlCommand Command2;
        public int scaleNew = 0; // شناسه پیش فاکتور
        public int scaler = 0; // شناسه پیش فاکتور
        public int number = 0;
        private cls_public cp = new cls_public();
        bool checktrn2 = false;
        bool checkinsert = false;
        private SqlTransaction transaction;
        public bool Etebar(DataRow Header,DataTable listItem) // اعتبار سنجی مشتری
        {
            if (clsProperties.GetPropertise("checkEtebarPortal").ToInt() == 1)
                return true;
            else
            {
                ArrayList Arraycreditlimit = new ArrayList();
                foreach (DataRow dr in listItem.Rows)
                {
                    bool chk = false;
                    foreach (string str in Arraycreditlimit)
                    {
                        if (str == dr["stock_id"].ToString())
                        {
                            chk = true;
                            break;
                        }
                    }
                    if (!chk)
                        Arraycreditlimit.Add(dr["stock_id"].ToString());
                    string gg = dr["stock_id"].ToString();
                }
                bool CreditLimit = false;
                foreach (string str in Arraycreditlimit)
                {
                    decimal MetrajJari = 0;
                    decimal MoneyJari = 0;
                    foreach (DataRow dr in listItem.Rows)
                    {
                        MetrajJari = MetrajJari + decimal.Parse(dr["value"].ToString());
                        MoneyJari = 0;
                    }
                    CreditLimit = true;//clsGlobal.creditlimit(2, Header["CustomerId"].ToInt(), str, clsProperties.GetPropertise("Kiosk_SaleType").ToInt(), Header["TypeSale"].ToInt(), MetrajJari, MoneyJari, true);
                    if (!CreditLimit)
                        break;
                }
                if (CreditLimit)
                {
                    return true;
                }
                else
                {
                    MessageError = "حد اعتبار مشتری رعایت نشده است";
                    clsDB.checkinseretstatic = true;
                    return false;
                    string error = "";
                    foreach (DataRow dr in classes.clsGlobal.DTSEllLimited.Rows)
                    {
                        error = error + "\n\r" + dr[0].ToString();
                    }
                }
            }
        }
        public bool inventory(DataRow Header, DataTable listItem) //گرفتن موجودی کالاهای اقلام  دیتا گرید
        {
            if (clsProperties.GetPropertise("checkinventorystockPortal").ToInt() == 1)
                return true;
            else
            {
                int idref = 0;
                bool res = clsDB.BeforeGetInventoryItem(Header["CustomerId"].ToInt().ToString(), "itm_id", "stock_id", "RBI_id", "PRG_RequestBuyItem", "value", "item_code", listItem, idref);
                if (res == false)
                {
                    MessageError = "موجودی کالاهای (" + clsDB.CheckInventory + ") کافی نمیباشد";
                    clsDB.CheckInventory = "";
                }
                return res;
            }
        }
        int ScalerQu = 0;
        DataTable listItemQ;
        public int Acsept(DataRow Header, DataTable listItem) // تابع ذخیره سازی و ثبت پیش فاکتور و گرفتن تسویه از مشتری
        {
            int insertbuyPortalType = clsProperties.GetPropertise("insertbuyPortalType").ToInt();
            int insertbuyQuentionPortalType = clsProperties.GetPropertise("insertbuyQuentionPortalType").ToInt();
            string DataMalistr = clsProperties.GetPropertise("DataMali").ToString();
            string saletrypeCrm = clsProperties.GetPropertise("DefualtTasfiePortal").ToString();

            listItem.Columns.Add("Discount", typeof(decimal));
            listItem.Columns.Add("DiscountPercent", typeof(int));
            listItem.Columns.Add("tax", typeof(decimal));
            listItem.Columns.Add("duty", typeof(decimal));
            listItem.Columns.Add("NetPrice", typeof(decimal));
            listItem.Columns.Add("Fee", typeof(decimal));
            if (insertbuyQuentionPortalType == 1)
            {
                listItemQ = listItem.Clone();
                foreach (DataRow dr in listItem.Rows)
                {
                    bool chkbalance = false;
                    if (clsProperties.GetPropertise("chkbalanceQ").ToBoolean())
                        chkbalance = true;
                    if (chkbalance)//موجودی  چک شود در تنظطیمات
                    {
                        DataTable dtmcheckojodi = cp.select_command(@"select case ST_Inventorycontrol when 1 then 1 
else IS_Inventorycontrol end as checkInventorycontrol from dbo.PRG_ItemStock
join dbo.PRG_Stock on ST_stockid = IS_stock where IS_stock=" + dr["stock_id"].ToString() + " AND IS_itm_id=" + dr["itm_id"].ToString());

                        if (dtmcheckojodi.Rows[0][0].ToString() == "1")
                        {
                            decimal sumvalues = 0;
                            DataTable dtmojodi = new DataTable();

                            sumvalues = decimal.Parse(dr["value"].ToString());
                            if (clsProperties.GetPropertise("reservesanad").ToInt() == 1)//درخواست خرید
                            {
                                dtmojodi = cp.select_command("select INS_Out from [dbo].[PRG_InventoryStock] where INS_Type=0 and INS_state =1 and INS_RequestBuyItem =(select RBI_id from PRG_RequestBuyItem where RBI_it_id=" + dr["itm_id"].ToString() + " and RBI_stock=" + dr["stock_id"].ToString() + " and RBI_RB_id=" + scaler.ToString() + ")");
                                if (dtmojodi != null)
                                    if (dtmojodi.Rows.Count > 0)
                                        sumvalues = sumvalues - decimal.Parse(dtmojodi.Rows[0][0].ToString());
                            }
                            string CheckInventory0 = "";
                            DataTable dtcode = cp.select_command("select itm_code from [dbo].[prg_item] where ITM_id=" + dr["itm_id"].ToString());
                            string Resualt = "";
                            DataTable dtfiscalyear1 = classes.clsDB.GetFiscalYearNow(classes.clsDB.GetFiscalYearNowStr());
                            string FiscalYearId1 = dtfiscalyear1.Rows[0]["FiscalYearId"].ToString();

                            DataTable dtmojodi2 = cp.select_command(classes.clsDB.SelectItem("", FiscalYearId1, " AND istk.IS_id = " + dr["is_id"].ToString(), Header["CustomerId"].ToString()));
                            if (dtmojodi2.Rows.Count > 0)
                            {
                                string hhh = dtmojodi2.Rows[0]["InventoryStock"].ToString();

                                if (dtmojodi2.Rows[0]["InventoryStock"].ToString() != "")
                                {
                                    if ((decimal.Parse(dtmojodi2.Rows[0]["InventoryStock"].ToString()) - sumvalues) >= 0)
                                    {
                                        listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8]);
                                    }
                                    else if (decimal.Parse(dtmojodi2.Rows[0]["InventoryStock"].ToString()) > 0)
                                    {
                                        listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], decimal.Parse(dtmojodi2.Rows[0]["InventoryStock"].ToString())
                                            , dr[6], dr[7], dr[8]);
                                    }
                                }
                                else
                                    listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8]);
                            }
                            else
                                listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8]);
                        }
                        else
                            listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8]);
                    }
                    else
                        listItemQ.Rows.Add(dr[0], dr[1], dr[2], dr[3], dr[4], dr[5], dr[6], dr[7], dr[8]);
                }
                foreach (DataRow dr in listItemQ.Rows)
                {
                    DataTable DtPricefee = cp.select_command(@"
select isnull((SELECT [Fee] FROM " + DataMalistr + @".SLS.vwPriceNoteItem    WITH (NOLOCK) 
   WHERE SaleTypeRef =(select SaleTypeId from Sepidar01.sls.SaleType where dbo.ArabicPersion( Title)=(select dbo.ArabicPersion(LKP_name) from PRG_Loockup where LKP_id=" + saletrypeCrm + ") )   AND CurrencyRef = 1 AND ItemRef =(select ITM_Item_id from PRG_Item where ITM_id=" + dr["itm_id"].ToString() + @") 
AND CustomerGroupingRef IS NULL),0) ");

                    DataTable Dtdiscount = cp.select_command(@"select dbo.GetDiscountItem(" + Header["CustomerId"].ToString() + ", " + dr["itm_id"].ToString() + ") AS DistinctCustomersBrand");
                    DataTable DtTaxPercent = cp.select_command(@"select ITM_tax from prg_item where ITM_id = " + dr["itm_id"].ToString());
                    //decimal PPrice = DtPricefee.Rows[0][0].ToDecimal();
                    decimal PDiscount = 0;
                    decimal Ptax = 0;
                    decimal PNetPrice = 0;
                    decimal discountPercent = 0;
                    decimal Fee = 0;
                    int PTaxPercent = DtTaxPercent.Rows[0][0].ToInt();
                    //PTaxPercent = 10;

                    if (DtPricefee.Rows.Count > 0)
                    {
                        Fee = DtPricefee.Rows[0][0].ToDecimal();
                        discountPercent = Dtdiscount.Rows[0][0].ToDecimal();
                        if (discountPercent != 0)
                            PDiscount = (discountPercent * (Fee * dr["value"].ToDecimal())) / 100;
                        Fee = DtPricefee.Rows[0][0].ToDecimal();
                        if (PTaxPercent != 0)
                            Ptax = (((Fee * dr["value"].ToDecimal()) - PDiscount) * PTaxPercent) / 100;
                        PNetPrice = (Fee * dr["value"].ToDecimal()) + Ptax - PDiscount;
                    }
                    dr["Fee"] = Fee;
                    dr["Discount"] = PDiscount;
                    dr["tax"] = Ptax;
                    dr["NetPrice"] = PNetPrice;
                    dr["DiscountPercent"] = discountPercent;
                    // (PPrice* dr["value"].ToDecimal())+ Ptax- PDiscount;
                }
            }


            DatePro date = new DatePro();
            bool CheckPishnevis;
            CheckPishnevis = clsDB.CheckPishnevis;
            clsDB.CheckPishnevis = false;
            dtdate = cp.GetDateTime();
            string miladishamsi = ShamsiDate.MiladiToShamsi(dtdate);
            int year = ShamsiDate.MiladiToShamsi(dtdate).Split('/')[0].ToInt();
            DataTable dtfiscalyear = clsDB.GetFiscalYearNow(year.ToString());
            string FiscalYearId = dtfiscalyear.Rows[0]["FiscalYearId"].ToString();
            string id = "";
            command = new SqlCommand("", cp.connect());
            try
            {
                transaction = cp.Transaction("SampleTransaction2");
            }
            catch
            {
            }
            command.CommandType = CommandType.Text;
            command.Transaction = transaction;
            command.Parameters.Clear();
            id = "";
            string RequestBuyInsQuINS = "";
            string RequestBuyIns = @"    declare @FiscalYearRef int
               set @FiscalYearRef=" + FiscalYearId + @" 
    DECLARE @strout int
            EXEC dbo.GetNextId 'PRG_RequestBuy',0, @strout OUTPUT
    declare @RBno int
               set @RBno=(select isnull(max(RB_no) + 1, 1) from PRG_RequestBuy WHERE RB_FiscalYearRef=@FiscalYearRef)
insert into PRG_RequestBuy (RB_id,RB_Portal,RB_Creditperiod,RB_stock,RB_Nemone,RB_nemonechk,RB_creator,RB_date, RB_cst_id, RB_no, RB_typebuy, RB_typecar, RB_Weight, RB_address, RB_DescNo,RB_typeSale,RB_modificationdate,RB_SalesExpert,RB_FiscalYearRef,changestatePrs,RB_Note,RB_City,RB_CustomerName,RB_Modifier,RB_type,RB_confirmer,RB_confirmdate) values 
    (@strout,1,(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='DefualtTasfiePortal'),null,null,0,@Creator,@date,@cst,@RBno,@typebuy,null,null,@RB_address,@DescNo,1,getdate(),@SalesExpert,@FiscalYearRef,@changestatePrs,@Note,@RB_City,(select ISNULL(cst_fname,'')+' '+ISNULL(cst_laname,'') from PRG_Customers where cst_id=@cst),@creator,@typee,@confirmer,@confirmdate);

   ";
            if (listItemQ != null)
            {
                if (listItemQ.Rows.Count > 0 & insertbuyQuentionPortalType == 1)
                {
                    RequestBuyInsQuINS = @" DECLARE @stroutQU int
            EXEC dbo.GetNextId 'PRG_Quotation',0, @stroutQU OUTPUT
    declare @QUno int
               set @QUno=(select isnull(max(QU_no) + 1, 1) from PRG_Quotation WHERE QU_FiscalYearRef=@FiscalYearRef)
    insert into dbo.PRG_Quotation 
                (QU_id, QU_Portal,QU_rb_id, QU_typesale,QU_Reserved,QU_TypeTasfie, QU_no, QU_date, QU_dateexpired, QU_modificationdate, 
QU_cst_id, QU_stock, QU_add_id, QU_desc, QU_type, QU_creator, QU_confirmer, QU_confirmdate, QU_Discount, QU_addition, QU_tax, QU_duty, QU_NetPrice,QU_Price, changestatePrs,QU_typeMoneySale,QU_Tamin,QU_SalesExpert,QU_FiscalYearRef,QU_deliverylocation,Qu_Note,QU_address,QU_Checkinventory,QU_Modifier,QU_CustomerName)
                values(@stroutQU,1,@strout,@typebuy,0,(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='DefualtTasfiePortal'),@QUno,@date,
DATEADD(day,(select cast([TNZ_sharh] as int) from [dbo].[PRG_Propertise] where TNZ_name='expireddayQuotation'),GETDATE()),GETDATE(),
@cst,null,@add,@DescNo,@typee,@creator,@confirmer,@confirmdate,@Discount,@addition,@tax,@duty,@NetPrice,@Price,@changestatePrs,@typeMoneySale,@Tamin,@SalesExpert,@FiscalYearRef,@RB_City,@NoteQU,@QU_address,0,@creator,(select ISNULL(cst_fname,'')+' '+ISNULL(cst_laname,'') from PRG_Customers where cst_id=@cst));  ";
                }
            }
            RequestBuyIns = RequestBuyIns + RequestBuyInsQuINS;
            RequestBuyIns = RequestBuyIns + " select cast(@strout as nvarchar(20)) +'*'+(select cast(@RBno as nvarchar(100)))";
            if (listItemQ != null)
            {
                if (listItemQ.Rows.Count > 0)
                {
                    RequestBuyIns = RequestBuyIns + " +'*'+ (SELECT cast(@stroutQU as nvarchar(20))) +'*'+(select cast(@QUno as nvarchar(100)))";
                }
            }
            command.CommandText = RequestBuyIns;

            if (listItemQ != null)
            {
                if (listItemQ.Rows.Count > 0)
                {
                    decimal totalNetPrice = listItemQ.AsEnumerable()
                              .Sum(row => row.Field<decimal>("NetPrice"));
                    decimal totaltax = listItemQ.AsEnumerable()
                                  .Sum(row => row.Field<decimal>("tax"));
                    decimal totalDiscount = listItemQ.AsEnumerable()
                                  .Sum(row => row.Field<decimal>("Discount"));
                    decimal totalPrice = listItemQ.AsEnumerable()
                                  .Sum(row => row.Field<decimal>("Fee"));
                    command.Parameters.Add("@QU_address", SqlDbType.NVarChar, 4000).Value = Header["address"].ToString();
                    command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = totalDiscount;
                    command.Parameters.Add("@tax", SqlDbType.Decimal).Value = totaltax;
                    command.Parameters.Add("@duty", SqlDbType.Decimal).Value = 0;
                    command.Parameters.Add("@NetPrice", SqlDbType.Decimal).Value = totalNetPrice;
                    command.Parameters.Add("@Price", SqlDbType.Decimal).Value = totalPrice;
                    command.Parameters.Add("@Tamin", SqlDbType.Int).Value = DBNull.Value;
                    command.Parameters.Add("@typeMoneySale", SqlDbType.Bit).Value = 0;
                    command.Parameters.Add("@addition", SqlDbType.Decimal).Value = 0;
                    command.Parameters.Add("@NoteQU", SqlDbType.NVarChar, 4000).Value = "پیش فاکتور اتوماتیک بر مبنای درخواست خرید دریافتی از پورتال مشتریان";

                    string gg = Header["CustomerId"].ToString();
                    DataTable dt = cp.select_command(
                    "SELECT add_id FROM dbo.PRG_Address WHERE add_cst_id = @cst_id AND add_type = 1",
                    new SqlParameter[] { new SqlParameter("@cst_id", Header["CustomerId"].ToInt()) },
                    transaction);
                    if (dt.Rows.Count > 0)
                    {
                        command.Parameters.Add("@add", SqlDbType.Int).Value = dt.Rows[0]["add_id"].ToInt();
                    }
                    else
                    {
                        transaction?.Rollback();
                        try { cp.connect().Close(); }
                        catch { }

                        MessageError = " هیچ آدرسی برای شما تعریف نشده است لطفا با کارشناس مربوطه در ارتباط باشید ";
                        return 0;
                    }
                }
            }
            if (insertbuyPortalType == 0)
            {
                command.Parameters.Add("@typee", SqlDbType.TinyInt).Value = 0;
                command.Parameters.Add("@confirmdate", SqlDbType.DateTime).Value = DBNull.Value;
                command.Parameters.Add("@confirmer", SqlDbType.Int).Value = DBNull.Value;
                command.Parameters.Add("@changestatePrs", SqlDbType.Int).Value = 0;
            }
            if (insertbuyPortalType == 1)
            {
                command.Parameters.Add("@typee", SqlDbType.TinyInt).Value = 1;
                command.Parameters.Add("@confirmdate", SqlDbType.DateTime).Value = dtdate;
                command.Parameters.Add("@confirmer", SqlDbType.Int).Value = Header["SalesExpert"].ToInt();
                command.Parameters.Add("@changestatePrs", SqlDbType.Int).Value = 1;
            }

            command.Parameters.Add("@cst", SqlDbType.Int).Value = Header["CustomerId"].ToInt();
            int TSALE = -1;
            if (Header["TypeSale"].ToInt() == 1)
                TSALE = 0;
            if (Header["TypeSale"].ToInt() == 2)
                TSALE = 1;
            if (Header["TypeSale"].ToInt() == 3)
                TSALE = 2;
            command.Parameters.Add("@typebuy", SqlDbType.Int).Value = TSALE;
            command.Parameters.Add("@Creator", SqlDbType.Int).Value = Header["SalesExpert"].ToInt();
            command.Parameters.Add("@RB_City", SqlDbType.Int).Value = Header["Location"].ToInt();
            command.Parameters.Add("@Note", SqlDbType.NVarChar, 4000).Value = "درخواست خرید دریافتی از پورتال مشتریان";
            command.Parameters.Add("@RB_address", SqlDbType.NVarChar, 4000).Value = Header["address"].ToString();
            command.Parameters.Add("@SalesExpert", SqlDbType.Int).Value = Header["SalesExpert"].ToInt(); //user id

            decimal sumdescount = 0;
            decimal sumAddition = 0;
            decimal sumprice = 0;
            decimal sumtax = 0;
            decimal sumduty = 0;
            decimal txtfinal = 0;

            command.Parameters.Add("@DescNo", SqlDbType.NVarChar, 4000).Value = Header["Description"].ToString();
            command.Parameters.Add("@date", SqlDbType.DateTime).Value = Convert.ToDateTime(Header["Date"]).Date;

            try
            {
                string[] str = { };
                try
                {
                    try { cp.connect().Open(); }
                    catch { }
                    string scalerstr = Convert.ToString((command.ExecuteScalar()));
                    str = scalerstr.Split('*');
                    scaleNew = scaler = Int32.Parse(str[0].ToString());
                    number = Int32.Parse(str[1].ToString());

                    if (insertbuyPortalType == 1)
                        cp.SendSmsToDatabase("1", "0", "1", Header["CustomerId"].ToString(), scaler.ToString(), " تایید درخواست خرید ");
                    if (insertbuyPortalType == 0)
                        cp.SendSmsToDatabase("1", "0", "1", Header["CustomerId"].ToString(), scaler.ToString(), " ثبت درخواست خرید ");
                    if (listItemQ != null)
                    {
                        if (listItemQ.Rows.Count > 0 & insertbuyQuentionPortalType == 1)
                            ScalerQu = Int32.Parse(str[2].ToString());
                    }

                    if (scaler == 0)
                    {
                        try
                        {
                            transaction?.Rollback();
                            try { cp.connect().Close(); }
                            catch { }
                        }
                        catch (Exception ex)
                        {
                        }

                        MessageError = " شناسه درخواست یافت نشد ";
                        return 0;
                    }
                }
                catch (Exception ex)
                {
                    checktrn2 = true;
                    try
                    {
                        checkinsert = false;
                        transaction?.Rollback();
                        try { cp.connect().Close(); }
                        catch { }

                    }
                    catch
                    {
                    }

                    MessageError = " ثبت درخواست با مشکل مواجه شد " + "( " + ex.Message + " )";
                    return 0;
                }
                if (str.Length == 2 || str.Length == 4)
                {
                    checkinsert = true;
                    if (listItemQ.Rows.Count > 0)
                    {
                        if (!checktrn2)
                        {
                            if (scaler != 0)
                            {
                                DataTable dtaghlamdatabase = new DataTable();
                                dtaghlamdatabase.Columns.Add("QUI", typeof(int));
                                dtaghlamdatabase.Columns.Add("QU", typeof(int));
                                dtaghlamdatabase.Columns.Add("it", typeof(int));
                                dtaghlamdatabase.Columns.Add("value", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("valuesec", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("QUI_fee", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("QUI_Discount", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("DistinctPercent", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("QUI_NetPrice", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("typeGh", typeof(int));
                                dtaghlamdatabase.Columns.Add("descc", typeof(string));
                                dtaghlamdatabase.Columns.Add("Tax", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("Duty", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("Addition", typeof(decimal));
                                dtaghlamdatabase.Columns.Add("stock", typeof(int));
                                dtaghlamdatabase.Columns.Add("GroupStock", typeof(string));
                                dtaghlamdatabase.Columns.Add("stockid2", typeof(int));
                                dtaghlamdatabase.Columns.Add("QUI_RBI_id", typeof(int));
                                dtaghlamdatabase.Columns.Add("QUI_TracingRef", typeof(int));
                                dtaghlamdatabase.Columns.Add("QUI_Reserved", typeof(bool));

                                int rownumber = 0;
                                bool checkaghlamValuesec = false;
                                if (listItemQ != null)
                                {
                                    foreach (DataRow dr in listItemQ.Rows)
                                    {
                                        decimal value = decimal.Parse(dr["value"].ToString());
                                        Nullable<decimal> valuesec = 0;
                                        Nullable<Int32> mabna = 0;
                                        if (dr["nesbat"].ToInt() > 0 && clsProperties.GetPropertise("SubunitQuotation").ToBoolean())
                                        {
                                            if (dr["nesbatValue"].ToDecimal() != 0)
                                                valuesec = value / dr["nesbatValue"].ToDecimal();
                                            else
                                                valuesec = null;
                                        }
                                        else
                                        {
                                            valuesec = null;
                                        }

                                        mabna = null;
                                        rownumber++;
                                        int stockitem = 0;
                                        stockitem = Int32.Parse(dr["stock_id"].ToString());

                                        string stk2 = "";

                                        if (valuesec != 0)
                                        {
                                            dtaghlamdatabase.Rows.Add(0, ScalerQu, Int32.Parse(dr["itm_id"].ToString()), value, valuesec, decimal.Parse(dr["Fee"].ToString()),
                                                decimal.Parse(dr["Discount"].ToString()), double.Parse(dr["DiscountPercent"].ToString()), decimal.Parse(dr["NetPrice"].ToString()).ToString("F4").ToDecimal(), 0, "", decimal.Parse(dr["tax"].ToString()), 0, 0, stockitem, stk2, DBNull.Value, mabna, DBNull.Value);
                                        }
                                        else
                                        {
                                            dtaghlamdatabase.Rows.Add(0, ScalerQu, Int32.Parse(dr["itm_id"].ToString()), value, DBNull.Value, decimal.Parse(dr["Fee"].ToString()),
                                                decimal.Parse(dr["Discount"].ToString()), double.Parse(dr["DiscountPercent"].ToString()), decimal.Parse(dr["NetPrice"].ToString()).ToString("F4").ToDecimal(), 0, "", decimal.Parse(dr["tax"].ToString()), 0, 0, stockitem, stk2, DBNull.Value, mabna, DBNull.Value);
                                        }

                                    }
                                    if (checktrn2)
                                    {
                                        transaction?.Rollback();
                                        try { cp.connect().Close(); }
                                        catch { }
                                        return 0;
                                    }
                                    command.CommandType = CommandType.StoredProcedure;
                                    command.Transaction = transaction;
                                    command.CommandText = "SPInQuotationItem";
                                    command.Parameters.Clear();
                                    command.Parameters.AddWithValue("@TableQuotationItem", dtaghlamdatabase);
                                    command.Parameters.Add("@Oknext", SqlDbType.Bit).Value = 0;
                                    try
                                    {
                                        string text = Convert.ToString(command.ExecuteScalar());

                                        //cp.close();
                                        //return number;
                                    }
                                    catch (Exception ex)
                                    {
                                        try
                                        {
                                            transaction?.Rollback();
                                            try { cp.connect().Close(); }
                                            catch { }
                                        }
                                        catch
                                        {
                                        }
                                        MessageError = " ثبت پیش فاکتور با مشکل مواجه شد " + "( " + ex.Message + " )";
                                        return 0;

                                    }
                                }
                            }
                        }
                    }

                    if (!checktrn2)
                    {
                        if (scaler != 0)
                        {
                            DataTable dtaghlamdatabase = new DataTable();
                            dtaghlamdatabase.Columns.Add("RBI", typeof(int));
                            dtaghlamdatabase.Columns.Add("RB", typeof(int));
                            dtaghlamdatabase.Columns.Add("it", typeof(int));
                            dtaghlamdatabase.Columns.Add("metraj", typeof(decimal));
                            dtaghlamdatabase.Columns.Add("palet", typeof(decimal));
                            dtaghlamdatabase.Columns.Add("Weight", typeof(int));
                            dtaghlamdatabase.Columns.Add("typeGh", typeof(int));
                            dtaghlamdatabase.Columns.Add("carton", typeof(decimal));
                            dtaghlamdatabase.Columns.Add("stock", typeof(int)); 
                            int rownumber = 0;
                            bool checkaghlamValuesec = false;
                            foreach (DataRow dr in listItem.Rows)
                            {
                                decimal value = decimal.Parse(dr["value"].ToString());
                                Nullable<decimal> valuesec = 0;
                                Nullable<Int32> mabna = 0;
                                if (dr["nesbat"].ToInt() > 0 && clsProperties.GetPropertise("SubunitQuotation").ToBoolean())
                                {
                                    if (dr["nesbatValue"].ToDecimal() != 0)
                                        valuesec = value / dr["nesbatValue"].ToDecimal();
                                    else
                                        valuesec = null;
                                }
                                else
                                {
                                    valuesec = null;
                                }
                                mabna = null;
                                rownumber++;
                                int stockitem = 0;
                                stockitem = Int32.Parse(dr["stock_id"].ToString());

                                string stk2 = "";


                                if (valuesec != 0)
                                {
                                    dtaghlamdatabase.Rows.Add(0, scaler, Int32.Parse(dr["itm_id"].ToString()), value, DBNull.Value, DBNull.Value, 0, valuesec, stockitem);
                                }
                                else
                                {
                                    dtaghlamdatabase.Rows.Add(0, scaler, Int32.Parse(dr["itm_id"].ToString()), value, DBNull.Value, DBNull.Value, 0, DBNull.Value, stockitem);
                                }

                            }

                            if (checktrn2)
                            {
                                transaction?.Rollback();
                                try { cp.connect().Close(); }
                                catch { }
                                return 0;
                            }
                            //if (!checkaghlamValuesec)
                            // {QU_deliverylocation
                            command.CommandType = CommandType.StoredProcedure;
                            command.Transaction = transaction;
                            command.CommandText = "SPInRequestBuyItem";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@TableRequestBuyItem", dtaghlamdatabase);
                            try
                            {
                                string text = Convert.ToString(command.ExecuteScalar());
                                transaction?.Commit();
                                cp.close();
                                try
                                {
                                    cp.SendSmsToDatabase("2", "2", "1", Header["CustomerId"].ToString(), ScalerQu.ToString(), "تایید پیش فاکتور ");
                                }
                                catch { }
                                return number;
                            }
                            catch (Exception ex)
                            {
                                try
                                {
                                    transaction?.Rollback();
                                    try { cp.connect().Close(); }
                                    catch { }
                                }
                                catch
                                {
                                }
                                MessageError = " ثبت پیش فاکتور با مشکل مواجه شد " + "( " + ex.Message + " )";
                                return 0;

                            }
                        }
                    }
                }
                else
                {
                    checktrn2 = true;
                    try
                    {
                        checkinsert = false;
                        transaction?.Rollback();
                        try { cp.connect().Close(); }
                        catch { }

                    }
                    catch
                    {
                    }

                    MessageError = " ثبت درخواست با مشکل مواجه شد " ;
                    return 0;
                }
            }
            catch { }
            return 0;
        }

    }
}
