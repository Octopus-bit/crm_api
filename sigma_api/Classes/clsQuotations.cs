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
    public class clsQuotations
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

        private int typePayment(int TypeSale)
        {
            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("TypeSale", TypeSale)
            };
            int id = cp.insert_getid("API_GetTypePaymentById", parameters, transaction).ToInt(); // برای دریافت شناسه نوع تسویه از جدول لوکاپ
            return id;
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
            EXEC dbo.GetNextId 'PRG_Quotation',0, @strout OUTPUT
    declare @QUno int
               set @QUno=(select isnull(max(QU_no) + 1, 1) from PRG_Quotation WHERE QU_FiscalYearRef=@FiscalYearRef)
    insert into dbo.PRG_Quotation 
                (QU_id, QU_Portal,QU_rb_id, QU_typesale,QU_Reserved,QU_TypeTasfie, QU_no, QU_date, QU_dateexpired, QU_modificationdate, QU_cst_id, QU_stock, QU_add_id, QU_desc, QU_type, QU_creator, QU_confirmer, QU_confirmdate, QU_Discount, QU_addition, QU_tax, QU_duty, QU_NetPrice,QU_Price, changestatePrs,QU_typeMoneySale,QU_Tamin,QU_SalesExpert,QU_FiscalYearRef,QU_deliverylocation,Qu_Note,QU_address,QU_Checkinventory,QU_Modifier,QU_CustomerName)
                values(@strout,1,@rb,@typesale,1,(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='DefualtTasfiePortal'),@QUno,@date,@dateexpired,GETDATE(),@cst,null,@add,@desc,@typee,@creator,@confirmer,@confirmdate,@Discount,@addition,@tax,@duty,@NetPrice,@Price,@changestatePrs,@typeMoneySale,@Tamin,@SalesExpert,@FiscalYearRef,@deliverylocation,@Note,@QU_address,0,@creator,(select ISNULL(cst_fname,'')+' '+ISNULL(cst_laname,'') from PRG_Customers where cst_id=@cst));  
   select (SELECT cast(@strout as nvarchar(20))) +'*'+(select cast(@QUno as nvarchar(100)))";
            command.Parameters.Add("@cst", SqlDbType.Int).Value = Header["CustomerId"].ToInt();

            //if (currentRowCustomersNostatic != null)
            //{
            //    command.Parameters.Add("@cst", SqlDbType.Int).Value = Convert.ToInt32(currentRowCustomersNostatic[0].ToString());
            //    classes.clsDB.CstForm = Convert.ToInt32(currentRowCustomersNostatic[0].ToString());
            //}
            //else
            //{
            //    command.Parameters.Add("@cst", SqlDbType.Int).Value = idcustomers;
            //    classes.clsDB.CstForm = idcustomers;
            //}
            command.Parameters.Add("@Creator", SqlDbType.Int).Value = Header["SalesExpert"].ToInt();

            // @confirmer,@confirmdate
            if (insertbuyPortalType == 0)
            {
                command.Parameters.Add("@confirmdate", SqlDbType.DateTime).Value = DBNull.Value;
                command.Parameters.Add("@confirmer", SqlDbType.Int).Value = DBNull.Value;
                command.Parameters.Add("@changestatePrs", SqlDbType.Int).Value = 0;
            }
            if (insertbuyPortalType == 1)
            {
                command.Parameters.Add("@confirmdate", SqlDbType.DateTime).Value = dtdate;
                command.Parameters.Add("@confirmer", SqlDbType.Int).Value = Header["SalesExpert"].ToInt();
                command.Parameters.Add("@changestatePrs", SqlDbType.Int).Value = 1;
            }


            command.Parameters.Add("@rb", SqlDbType.Int).Value = DBNull.Value;

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
            //            }
            int TSALE = -1;
            if (Header["TypeSale"].ToInt() == 1)
                TSALE = 0;
            if (Header["TypeSale"].ToInt() == 2)
                TSALE = 1;
            if (Header["TypeSale"].ToInt() == 3)
                TSALE = 2;
            command.Parameters.Add("@typesale", SqlDbType.Int).Value = TSALE;
            command.Parameters.Add("@Note", SqlDbType.NVarChar, 4000).Value = "پیش فاکتور دریافتی از پورتال مشتریان";
            //command.Parameters.Add("@TypeTasfie", SqlDbType.Int).Value = typePayment(Header["TypeSale"].ToInt());
            command.Parameters.Add("@deliverylocation", SqlDbType.Int).Value = Header["Location"].ToInt();
            command.Parameters.Add("@QU_address", SqlDbType.NVarChar,4000).Value = Header["address"].ToString();

            command.Parameters.Add("@SalesExpert", SqlDbType.Int).Value = Header["SalesExpert"].ToInt(); //user id

            if (insertbuyPortalType == 0)
                command.Parameters.Add("@typee", SqlDbType.TinyInt).Value = 0;
            if (insertbuyPortalType == 1)
                command.Parameters.Add("@typee", SqlDbType.TinyInt).Value = 1;
            decimal sumdescount = 0;
            decimal sumAddition = 0;
            decimal sumprice = 0;
            decimal sumtax = 0;
            decimal sumduty = 0;
            decimal txtfinal = 0;
            foreach (DataRow dr in listItem.Rows)
            {

                sumprice = sumprice + decimal.Parse(dr["price"].ToString());
                decimal priceDescount = ((decimal.Parse(dr["price"].ToString()) * decimal.Parse(dr["discountCustomer"].ToString())) / 100);
                sumdescount = sumdescount + priceDescount;
                sumtax = sumtax + (((decimal.Parse(dr["price"].ToString()) - priceDescount) * decimal.Parse(dr["tax"].ToString())) / 100);
                txtfinal = txtfinal + (sumprice - sumdescount) + sumtax;
            }

            command.Parameters.Add("@Discount", SqlDbType.Decimal).Value = sumdescount;

            command.Parameters.Add("@addition", SqlDbType.Decimal).Value = 0;
            //txttax.Text = sumtax.ToString();
            //txtduty.Text = sumduty.ToString();

            command.Parameters.Add("@tax", SqlDbType.Decimal).Value = sumtax;
            command.Parameters.Add("@duty", SqlDbType.Decimal).Value = 0;
            command.Parameters.Add("@NetPrice", SqlDbType.Decimal).Value = decimal.Parse(txtfinal.ToString("F4"));
            command.Parameters.Add("@Price", SqlDbType.Decimal).Value = decimal.Parse(sumprice.ToString("F4"));

            command.Parameters.Add("@typeMoneySale", SqlDbType.Bit).Value = 0;

            command.Parameters.Add("@desc", SqlDbType.NVarChar, 4000).Value = Header["Description"].ToString();


            command.Parameters.Add("@date", SqlDbType.DateTime).Value = Convert.ToDateTime(Header["Date"]).Date;

            command.Parameters.Add("@dateexpired", SqlDbType.DateTime).Value = Convert.ToDateTime(Header["expire_date"]).Date; // تاریخ انقضا

            command.Parameters.Add("@Tamin", SqlDbType.Int).Value = DBNull.Value;



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

                            //bool nextok = false;
                            int rownumber = 0;
                            bool checkaghlamValuesec = false;
                            foreach (DataRow dr in listItem.Rows)
                            {
                                decimal value = decimal.Parse(dr["value"].ToString());
                                Nullable<decimal> valuesec = 0;
                                Nullable<Int32> mabna = 0;
                                if (dr["nesbat"].ToInt() > 0 && clsProperties.GetPropertise("SubunitQuotation").ToBoolean())
                                {
                                    if(dr["nesbatValue"].ToDecimal()!=0)
                                        valuesec = value / dr["nesbatValue"].ToDecimal();
                                    else
                                        valuesec = null;
                                }
                                else
                                {
                                    valuesec = null;
                                }
                                string gg = dr["price"].ToString();
                                decimal descount = ((decimal.Parse(dr["price"].ToString()) * decimal.Parse(dr["discountCustomer"].ToString())) / 100);
                                decimal tax = (((decimal.Parse(dr["price"].ToString()) - descount) * decimal.Parse(dr["tax"].ToString())) / 100);
                                decimal netPrice = (decimal.Parse(dr["price"].ToString()) - descount) + tax;
                                if (netPrice != dr["netPrice"].ToDecimal())
                                {
                                    MessageError = "مبلغ خالص اقلام به درستی محاصبه نشده است";
                                    checktrn2 = true;
                                    break;
                                }
                                mabna = null;
                                rownumber++;
                                int stockitem = 0;
                                stockitem = Int32.Parse(dr["stock_id"].ToString());

                                string stk2 = "";


                                if (valuesec != 0)
                                {
                                    dtaghlamdatabase.Rows.Add(0, scaler, Int32.Parse(dr["itm_id"].ToString()), value, valuesec, decimal.Parse(dr["fee"].ToString()), descount, double.Parse(dr["discountCustomer"].ToString()), netPrice.ToString("F4").ToDecimal(), 0, "", tax, 0, 0, stockitem, stk2, DBNull.Value, mabna, DBNull.Value);
                                }
                                else
                                {
                                    dtaghlamdatabase.Rows.Add(0, scaler, Int32.Parse(dr["itm_id"].ToString()), value, DBNull.Value, decimal.Parse(dr["fee"].ToString()), descount, double.Parse(dr["discountCustomer"].ToString()), netPrice.ToString("F4").ToDecimal(), 0, "", tax, 0, 0, stockitem, stk2, DBNull.Value, mabna, DBNull.Value);
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
                            command.CommandText = "SPInQuotationItem";
                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("@TableQuotationItem", dtaghlamdatabase);
                            command.Parameters.Add("@Oknext", SqlDbType.Bit).Value = 0;
                            try
                            {
                                string text = Convert.ToString(command.ExecuteScalar());
                                transaction?.Commit();
                                cp.close();
                                return number;
                                /*if (chkTypeTasvie.SelectedValue.ToBoolean())
                                {
                                    FrmPayment fp = new FrmPayment(clsProperties.Kiosk_TerminalId, sumprice);
                                    fp.ShowDialog();
                                    if (fp.state_payment == 0)
                                    {
                                        return ((int)acsept.canceledPayment);
                                    }
                                    else if (fp.state_payment == 1)
                                    {
                                        clsGlobal.Alarm(". . . پرداخت شما با موفقیت انجام شد لطفا منتظر بمانید", clsGlobal.AlarmIcon.Success, 8, DevComponents.DotNetBar.Controls.eAlertPosition.TopRight);
                                    }
                                    else if (fp.state_payment == 2)
                                    {
                                        return ((int)acsept.canceledPayment);
                                    }
                                    SqlParameter[] parameters_insertRec =
                                    {
                                        new SqlParameter("RC_Qu_id",scaler),
                                        new SqlParameter("RC_Terminal_id",clsProperties.Kiosk_TerminalId),
                                        new SqlParameter("RC_Money",sumprice),
                                        new SqlParameter("RC_Created",clsProperties.Kiosk_SaleUserId),
                                        new SqlParameter("RC_number",fp.RRN),
                                    };
                                    cp.insert("SP_Kiosk_RecievedCheque_insert", parameters_insertRec);
                                    SqlTransaction transactionclsdb;
                                    int scalerReqIDFull = 0;
                                    DateTime dtNow = CRM.classes.clsDB.GetDateTime();
                                    try { CRM.classes.clsDB.connection.Open(); }
                                    catch { }
                                    //   transactionclsdb = null;
                                    // transactionclsdb = new SqlTransaction();
                                    transactionclsdb = CRM.classes.clsDB.connection.BeginTransaction("SampleTransaction");
                                    bool check = true;
                                    CRM.classes.clsDB.Stringreplaceclsdb();
                                    DataSet _DS_sendTomali = new DataSet();
                                    SqlDataAdapter _DA_select = new SqlDataAdapter();
                                    SqlCommand Command2 = new SqlCommand(CRM.classes.clsDB.TextSendMali + @" where PRG_Quotation.QU_id=" + scaler, CRM.classes.clsDB.connection);
                                    try { CRM.classes.clsDB.connection.Open(); }
                                    catch { }

                                    Command2.CommandType = CommandType.Text;
                                    Command2.Transaction = transactionclsdb;
                                    _DA_select.SelectCommand = Command2;
                                    _DA_select.Fill(_DS_sendTomali);
                                    DataTable dtTextSendMali = _DS_sendTomali.Tables[0];
                                    if (dtTextSendMali.Rows.Count > 0)
                                    {
                                        check = CRM.classes.clsDB.SendQuentionToMali(dtTextSendMali.Rows[0], dtTextSendMali, transactionclsdb, "", dtNow);
                                        if (!check && clsProperties.SepidarApi_Access == false)
                                        {
                                            DataColumn[] dc = new DataColumn[1];
                                            SqlCommand command = new SqlCommand("", CRM.classes.clsDB.connection);
                                            command.Transaction = transactionclsdb;
                                            command.CommandType = CommandType.Text;
                                            command.CommandText = @"declare @type int
                    set @type =(select QU_type from dbo.PRG_Quotation where  QU_id = " + scaler + @")
                    if(@type=2 or @type=0)
                    begin
                                    update dbo.PRG_Quotation set QU_type=1 ,changestatePrs=1,QU_modificationdate=GETDATE(),QU_confirmer=" + clsProperties.Kiosk_SaleUserId + " ,QU_confirmdate=GETDATE() where QU_id=" + scaler + @";
                                    update PRG_RefferingForm set RFF_Valid=0  where RFF_Formid=" + scaler + @" and RFF_Formname='Quotation'
                                       select 1
                    end
                    else
                   begin
                             select @type
                    end";
                                            try
                                            {
                                                try { CRM.classes.clsDB.connection.Open(); }
                                                catch { }

                                                string scalp = Convert.ToString(command.ExecuteScalar());
                                                if (scalp == "1")
                                                {
                                                    transactionclsdb.Commit();
                                                    try { CRM.classes.clsDB.connection.Close(); }
                                                    catch { }
                                                    return ((int)acsept.success_full);
                                                }
                                                else if (scalp != "1")
                                                {
                                                    try
                                                    {
                                                        transactionclsdb.Rollback();
                                                        try { CRM.classes.clsDB.connection.Close(); }
                                                        catch { }
                                                    }
                                                    catch
                                                    {
                                                    }
                                                    classes.clsFarsiMessageBox.ShowMessage(0, "پیش فاکتور در وضعیتی غیر از وضعیت تایید مالی میباشد. امکان تایید نهایی پیش فاکتور وجود ندارد", "توجه", "تایید", "تایید", "", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                                                    return ((int)acsept.success_paid);
                                                }
                                            }
                                            catch { }
                                        }
                                        if (!check && clsProperties.SepidarApi_Access == true)
                                        {
                                            return ((int)acsept.success_full);
                                        }
                                        else
                                        {
                                            transactionclsdb.Rollback();
                                            try { CRM.classes.clsDB.connection.Close(); }
                                            catch { }
                                            return ((int)acsept.success_pishfactor);
                                        }

                                    }
                                    else
                                    {
                                        check = true;
                                        //classes.clsFarsiMessageBox.ShowMessage(0, "مشتری بدون ادرس اصلی می باشد", "توجه", "تاييد", "تاييد", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                                        return ((int)acsept.success_paid);
                                    }
                                }
                                else
                                {
                                    return ((int)acsept.success_pishfactor);
                                }*/
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
            catch{ }
            return 0;
        }

        void maliatavarez()
        {
            //try
            //{
            //    foreach (DataRow dr in dtaghlam.Rows)
            //    {
            //        dr.BeginEdit();
            //        decimal addition = Convert.ToDecimal(dr["addition"].ToString());
            //        decimal kol = Convert.ToDecimal(dr["kol"].ToString());
            //        decimal Discount = Convert.ToDecimal(dr["INVI_Discount"].ToString());
            //        decimal duty = Int32.Parse(dr["ITM_duty"].ToString());
            //        decimal tax = Int32.Parse(dr["ITM_tax"].ToString());
            //        decimal dutydec = ((kol - Discount + addition) * duty) / 100;
            //        decimal taxdec = ((kol - Discount + addition) * tax) / 100;
            //        if (FrmMain.TaxQuotation)
            //            dr["tax"] = (decimal.Parse(taxdec.ToString())).ToString("F0");
            //        else dr["tax"] = 0;

            //        if (FrmMain.DutyQuotation)
            //            dr["duty"] = (decimal.Parse(dutydec.ToString())).ToString("F0");
            //        else dr["duty"] = 0;

            //        dr["INVI_NetPrice"] = (decimal.Parse(dr["kol"].ToString()) - decimal.Parse(dr["INVI_Discount"].ToString()) + decimal.Parse(dr["tax"].ToString()) + decimal.Parse(dr["duty"].ToString()) + decimal.Parse(dr["addition"].ToString())).ToString("F0");
            //        string g4g = dr["typeGh"].ToString();
            //        if (dr["typeGh"].ToString() != "0")
            //            dr["typeGh"] = "2";

            //        dr.EndEdit();
            //        dr.AcceptChanges();
            //    }
            //    dgritemQ.UpdateData();
            //    //refreshdgr();
            //}
            //catch { }
        }
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
                        MoneyJari = MoneyJari + decimal.Parse(dr["Price"].ToString());
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
            int idref = 0;
            bool res = clsDB.BeforeGetInventoryItem(Header["CustomerId"].ToInt().ToString(), "itm_id", "stock_id", "QUI_id", "PRG_QuotationItem", "value", "item_code", listItem, idref);
            if (res == false)
            {
                MessageError = "موجودی کالاهای ("+clsDB.CheckInventory+") کافی نمیباشد";
                clsDB.CheckInventory = "";
            }
            return res;
        }
    }
}
