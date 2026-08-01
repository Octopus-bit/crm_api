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
        public int Acsept(DataRow Header, DataTable listItem) // تابع ذخیره سازی و ثبت پیش فاکتور و گرفتن تسویه از مشتری
        {
            int insertbuyPortalType = clsProperties.GetPropertise("insertbuyPortalType").ToInt();

            DatePro date = new DatePro();
            bool CheckPishnevis;
            CheckPishnevis = clsDB.CheckPishnevis;
            clsDB.CheckPishnevis = false;
            //DateTime time = cp.GetDateTime();
            dtdate = cp.GetDateTime();
            //dtdate = DateTime.Now;
            string miladishamsi = ShamsiDate.MiladiToShamsi(dtdate);
            int year = ShamsiDate.MiladiToShamsi(dtdate).Split('/')[0].ToInt();
            DataTable dtfiscalyear = clsDB.GetFiscalYearNow(year.ToString());
            string FiscalYearId = dtfiscalyear.Rows[0]["FiscalYearId"].ToString();
            string id = "";

            //-------------------- انقضا
            //int etebar_Date = clsProperties.expireddayQuotation.ToInt();
            //DateTime dtexpired = classes.ShamsiDate.Shamsi2Miladi(date.DateSumDay(date.GetDatePertion(dtdate.ToShortDateString()), etebar_Date));
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

            command.CommandText = @"
    declare @FiscalYearRef int
               set @FiscalYearRef=" + FiscalYearId + @" 
    DECLARE @strout int
            EXEC dbo.GetNextId 'PRG_RequestBuy',0, @strout OUTPUT
    declare @RBno int
               set @RBno=(select isnull(max(RB_no) + 1, 1) from PRG_RequestBuy WHERE RB_FiscalYearRef=@FiscalYearRef)
insert into PRG_RequestBuy (RB_id,RB_Portal,RB_Creditperiod,RB_stock,RB_Nemone,RB_nemonechk,RB_creator,RB_date, RB_cst_id, RB_no, RB_typebuy, RB_typecar, RB_Weight, RB_address, RB_DescNo,RB_typeSale,RB_modificationdate,RB_SalesExpert,RB_FiscalYearRef,changestatePrs,RB_Note,RB_City,RB_CustomerName,RB_Modifier,RB_type,RB_confirmer,RB_confirmdate) values 
    (@strout,1,(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='DefualtTasfiePortal'),null,null,0,@Creator,@date,@cst,@RBno,@typebuy,null,null,@RB_address,@DescNo,1,getdate(),@SalesExpert,@FiscalYearRef,@changestatePrs,@Note,@RB_City,(select ISNULL(cst_fname,'')+' '+ISNULL(cst_laname,'') from PRG_Customers where cst_id=@cst),@creator,@typee,@confirmer,@confirmdate);
    select cast(@strout as nvarchar(20)) +'*'+(select cast(@RBno as nvarchar(100)))";

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

            //DataTable dt = cp.select_command($"SELECT add_id FROM dbo.PRG_Address WHERE add_cst_id = {Header["CustomerId"].ToInt()} AND add_type = 1", null, transaction);
            //if (dt.Rows.Count > 0)
            //{
            //    command.Parameters.Add("@RB_City", SqlDbType.Int).Value = dt.Rows[0]["add_id"].ToInt();
            //}
            //else
            //{
            //    transaction.Rollback();
            //    try { cp.connect().Close(); }
            //    catch { }

            //    MessageError = " هیچ آدرسی برای شما تعریف نشده است لطفا با کارشناس مربوطه در ارتباط باشید ";
            //    return 0;
            //}
            //            }
            command.Parameters.Add("@Note", SqlDbType.NVarChar, 4000).Value = "درخواست خرید دریافتی از پورتال مشتریان";


            //command.Parameters.Add("@TypeTasfie", SqlDbType.Int).Value = typePayment(Header["TypeSale"].ToInt());

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
                    //  MessageBox.Show("6");
                    string scalerstr = Convert.ToString((command.ExecuteScalar()));

                    str = scalerstr.Split('*');
                    scaleNew = scaler = Int32.Parse(str[0].ToString());
                    number = Int32.Parse(str[1].ToString());
                    //classes.clsDB.scalerReq = scaler;
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

                        MessageError = " شناسه پیش فاکتور یافت نشد ";
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

                    MessageError = " ثبت پیش فاکتور با مشکل مواجه شد " + "( " + ex.Message + " )";
                    return 0;
                }
                if (str.Length == 2)
                {
                    checkinsert = true;
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
            }
            catch { }
            return 0;
        }

    }
}
