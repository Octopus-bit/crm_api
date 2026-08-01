using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using sigma_api.Classes;
using System.Linq;

namespace sigma_api.classes
{
    class clsGlobal
    {
        [ThreadStatic]
        private static cls_public _threadCp;

        private static cls_public cp
        {
            get
            {
                if (_threadCp == null)
                    _threadCp = new cls_public();
                return _threadCp;
            }
        }

        private static string DataMali;
        private static int reservesanad;
        private static DateTime DTPeriodType;
        private static DateTime DTPeriodType2;
        private static ArrayList Arraycreditlimit;
        private static string stock2 = "";
        public static DataTable DTSEllLimited;
        //private static string FiscalYearNowStr = cp.GetDateTime().Year.ToString();
        public clsGlobal()
        {
            DataMali = clsProperties.GetPropertise("DataMali").ToString();
            reservesanad = clsProperties.GetPropertise("reservesanad").ToInt();
        }

        public static string StringSplitOptionsRemoveEmptyEntries(string Input)
        {
            string Reualt = "";
            string Reualt2 = "";
            string Reualt3 = "";
            string[] words = Input.Split(
                            new char[] { ' ' },
                            StringSplitOptions.RemoveEmptyEntries
                        );
            foreach(string str in words)
            {
                Reualt2 = Reualt2 + " AND " + " dbo.ArabicPersion(itm.ITM_fullname) LIKE dbo.ArabicPersion(N'%" + str + "%')";
                Reualt3 = Reualt3 + " AND " + " itm.ITM_Code LIKE N'%" + str + "%'";
            }
            if (!string.IsNullOrEmpty(Reualt2))
                Reualt2 = Reualt2.Substring(4, Reualt2.Length - 4);
            if (!string.IsNullOrEmpty(Reualt3))
                Reualt3 = Reualt3.Substring(4, Reualt3.Length - 4);

            //Reualt2 = string.Join(" AND ",
            //                words.Select(x => $" dbo.ArabicPersion(itm.ITM_fullname) LIKE dbo.ArabicPersion(N'%{x}%')"));
            //Reualt3 = string.Join(" AND ",
            //                words.Select(x => $" itm.ITM_Code LIKE N'%{x}%'"));
            Reualt = "(" + Reualt2 + ")" + " OR " + "(" + Reualt3 + ")";

            return Reualt;
        }
        public static bool creditlimit(Int16 formLimited, int customer, string stock, int noeforosh, int nahvetasfie, decimal MetrajJari, decimal MoneyJari, bool TypeJari)
        {
            DTSEllLimited = new DataTable();
            DTSEllLimited.Columns.Add("cc_desc", typeof(string));

            DataTable dtResult0;
            DataTable dtshow;
            bool Result = true;
            DataSet _DS = new DataSet();
            DataTable dtexists = cp.select_command(@"SELECT DISTINCT PRG_Creditcondition.cc_id
FROM         PRG_Creditcondition left JOIN
                      PRG_CreditconditionCustomers ccu ON PRG_Creditcondition.cc_id = ccu.ccu_cc_id INNER JOIN
                      PRG_CreditconditionCup ON PRG_Creditcondition.cc_id = PRG_CreditconditionCup.CCM_cc_id
WHERE (cc_formlimited=0 OR cc_formlimited=" + formLimited.ToString() + @") AND cc_state=1 AND (ccu.ccu_cst_id is null or ccu.ccu_cst_id=" + customer.ToString() + @")");
            string lpkiddd = "";
            if (dtexists.Rows.Count > 0)
            {
                dtshow = clsDB.MEdalCustomers();
                // MessageBox.Show(dtshow.Rows.Count.ToString()); 
                foreach (DataRow dr in dtshow.Rows)
                {
                    if (dr["CST"].ToString() == customer.ToString())
                    {
                        // MessageBox.Show(dr["CST"].ToString());
                        lpkiddd = dr["lkpid"].ToString();
                        break;
                    }
                }
            }
            else
            {
                lpkiddd = "0";
            }
            string tt = "";
            string ttAND = "";
            if (lpkiddd != "")
            {
                tt = " OR CCM_lkp_Cup=" + lpkiddd;
                ttAND = " AND CCM_lkp_Cup=" + lpkiddd;
            }
            else
            {
                tt = " OR 1=1 ";
                ttAND = " AND 1=1 ";
            }

            //وقتی غیر مجاز باشند
            _DS = new DataSet();
            dtResult0 = cp.select_command(@"SELECT distinct cc.cc_id,cc.cc_no,ccd.ccd_id,ccs.ccs_stock_id,cccType0.ccc_lkp_clear ccc_lkp_clear0,cccType1.ccc_lkp_clear ccc_lkp_clear1
,ccp.CCM_lkp_Cup FROM PRG_Creditcondition cc LEFT JOIN
PRG_CreditconditionDetail ccd ON cc.cc_id = ccd.ccd_cc_id LEFT JOIN
PRG_CreditconditionClear cccType0 ON cc.cc_id = cccType0.ccc_cc_id AND cccType0.ccc_type=0 LEFT JOIN
PRG_CreditconditionClear cccType1 ON cc.cc_id = cccType1.ccc_cc_id AND cccType1.ccc_type=1 LEFT JOIN
PRG_CreditconditionCustomers ccu ON cc.cc_id = ccu.ccu_cc_id LEFT JOIN
PRG_CreditconditionCup ccp ON cc.cc_id = ccp.CCM_cc_id LEFT JOIN
PRG_CreditconditionStock ccs ON cc.cc_id = ccs.ccs_cc_id
where (cc_formlimited=0 OR cc_formlimited=" + formLimited.ToString() + @") AND cc_state=1 AND cc_statecredit=0 AND (ccu.ccu_cst_id is null or ccu.ccu_cst_id=" + customer.ToString() + @")
 AND (((select count(*) from dbo.PRG_Loockup where LKP_enownerName='CustomerMedal') =0) " + tt + @") AND
(ccs_stock_id is null or ccs_stock_id=" + stock + @")
AND cccType0.ccc_lkp_clear = " + noeforosh.ToString() + " AND cccType1.ccc_lkp_clear = " + nahvetasfie.ToString() + " order by ccd.ccd_id");


            //رکوردی در حد اعتبار وجود نداشه باشد برای این مشتری
            //  MessageBox.Show("2");
            if (dtResult0.Rows.Count == 0)
            {
                Result = true;
            }
            else
            {
                DataRow[] drfil = dtResult0.Select(" ccd_id is null ");
                if (drfil.Length != 0)
                {
                    Result = false;
                    DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil[0]["cc_no"].ToString() + " : " + " مدال ، نوع فروش ، نحوه تسویه ، انبار ");
                }
                else
                {
                    //اقلام شرط
                    string ccd_id = "";
                    foreach (DataRow dr in dtResult0.Rows)
                    {
                        ccd_id = ccd_id + ',' + dr["ccd_id"].ToString();
                    }
                    ccd_id = "(" + ccd_id.Substring(1, ccd_id.Length - 1) + ")";

                    Result = creditlimitCondition(0, ccd_id, customer, stock, MetrajJari, MoneyJari, TypeJari);
                }
            }
            if (Result)
            {
                //وقتی مجاز باشند
                _DS = new DataSet();
                dtResult0 = cp.select_command(@"SELECT distinct cc.cc_id,cc.cc_no,ccd.ccd_id,ccs.ccs_stock_id,cccType0.ccc_lkp_clear ccc_lkp_clear0,cccType1.ccc_lkp_clear ccc_lkp_clear1
,ccp.CCM_lkp_Cup FROM PRG_Creditcondition cc LEFT JOIN
PRG_CreditconditionDetail ccd ON cc.cc_id = ccd.ccd_cc_id LEFT JOIN
PRG_CreditconditionClear cccType0 ON cc.cc_id = cccType0.ccc_cc_id AND cccType0.ccc_type=0 LEFT JOIN
PRG_CreditconditionClear cccType1 ON cc.cc_id = cccType1.ccc_cc_id AND cccType1.ccc_type=1  LEFT JOIN
PRG_CreditconditionCup ccp ON cc.cc_id = ccp.CCM_cc_id LEFT JOIN
PRG_CreditconditionCustomers ccu ON cc.cc_id = ccu.ccu_cc_id LEFT JOIN
PRG_CreditconditionStock ccs ON cc.cc_id = ccs.ccs_cc_id
where  (cc_formlimited=0 OR cc_formlimited=" + formLimited.ToString() + @") AND cc_state=1 AND (ccs_stock_id is null or ccs_stock_id=" + stock + @") AND cc_statecredit=1 
 AND (ccu.ccu_cst_id is null or ccu.ccu_cst_id=" + customer.ToString() + @")");

                //رکوردی در حد اعتبار وجود نداشه باشد برای این مشتری
                // MessageBox.Show("3");
                if (dtResult0.Rows.Count == 0)
                {
                    Result = true;
                }
                else
                {
                    _DS = new DataSet();
                    DataTable dtcount = cp.select_command(@"select count(*) from dbo.PRG_Loockup where LKP_enownerName='CustomerMedal'");
                    string filter = "";
                    DataRow[] drfil = dtResult0.Select(" ccd_id is null");
                    if (dtcount.Rows[0][0].ToString() == "0")
                        filter = " ccc_lkp_clear0 = " + noeforosh.ToString() + " AND ccc_lkp_clear1 = " + nahvetasfie.ToString();
                    else
                        filter = " ccc_lkp_clear0 = " + noeforosh.ToString() + " AND ccc_lkp_clear1 = " + nahvetasfie.ToString() + ttAND;

                    DataRow[] drfil0 = dtResult0.Select(filter);

                    // MessageBox.Show("4");
                    if (drfil0.Length != 0)
                    {
                        if (drfil.Length != 0)
                        {
                            Result = true;
                        }
                        else
                        {
                            //اقلام شرط
                            string ccd_id = "";
                            foreach (DataRow dr in dtResult0.Rows)
                            {
                                if (!ccd_id.Contains("," + dr["ccd_id"].ToString()))
                                    ccd_id = ccd_id + "," + dr["ccd_id"].ToString();
                            }
                            ccd_id = "(" + ccd_id.Substring(1, ccd_id.Length - 1) + ")";

                            Result = creditlimitCondition(1, ccd_id, customer, stock, MetrajJari, MoneyJari, TypeJari);
                        }
                    }
                    else
                    {
                        Result = false;
                        DTSEllLimited.Rows.Add(" محدودیت شماره " + dtResult0.Rows[0]["cc_no"].ToString() + " : " + " نوع فروش ، نحوه تسویه ، انبار ، مدال");
                    }
                    if (drfil.Length != 0)
                    {

                    }
                }
            }
            //foreach(DataRow dr in DTSEllLimited.Rows)
            //{
            //}
            return Result;
        }


        public static bool creditlimitCondition(int type, string ccd_id, int customer, string stock, decimal MetrajJari, decimal MoneyJari, bool TypeJari)
        {
            bool Result0 = true;
            //   bool Result = false;
            DataTable dtResult0;
            DataTable dtResultkol0;
            DataSet _DS = new DataSet();
            SqlDataAdapter _DA = new SqlDataAdapter();
            SqlCommand Command = new SqlCommand("", cp.connect());
            _DA.SelectCommand = Command;
            Command.CommandType = CommandType.Text;
            Command.CommandText = @"SELECT distinct ccd_cc_id
    FROM PRG_CreditconditionDetail
    where ccd_id in " + ccd_id;
            //            Command.CommandText = @"SELECT ccd_id, ccd_cc_id, ccd_type, ccd_itemtype, ccd_condition, ccd_period, ccd_periodtype, ccd_countF, ccd_countT, ccd_moneyF, ccd_moneyT
            //FROM PRG_CreditconditionDetail
            //where ccd_id in" + ccd_id + " order by ccd_id";
            _DA.Fill(_DS);
            dtResult0 = _DS.Tables[0];

            _DS = new DataSet();
            _DA = new SqlDataAdapter();
            Command = new SqlCommand("", cp.connect());
            _DA.SelectCommand = Command;
            Command.CommandType = CommandType.Text;
            Command.CommandText = @"SELECT ccd_id, ccd_cc_id, ccd_type, ccd_itemtype, ccd_condition, ccd_period, ccd_periodtype, ccd_countF, ccd_countT, ccd_moneyF, ccd_moneyT
    ,(select cc_no from PRG_Creditcondition where cc_id =ccd_cc_id) as cc_no FROM PRG_CreditconditionDetail
    where ccd_id in" + ccd_id + " order by ccd_id";
            _DA.Fill(_DS);
            dtResultkol0 = _DS.Tables[0];
            ArrayList Arraycreditlimit0 = new ArrayList();

            foreach (DataRow dr in dtResult0.Rows)
            {
                _DS = new DataSet();
                _DA = new SqlDataAdapter();
                Command = new SqlCommand("", cp.connect());
                _DA.SelectCommand = Command;
                Command.CommandType = CommandType.Text;
                Command.CommandText = @"SELECT ccs_stock_id
        FROM PRG_CreditconditionStock
        where ccs_cc_id =" + dr["ccd_cc_id"].ToString();
                _DA.Fill(_DS);
                DataTable dtAnbar0 = _DS.Tables[0];
                stock2 = "";
                if (dtAnbar0.Rows.Count > 0)
                {
                    foreach (DataRow drAnbar in dtAnbar0.Rows)
                    {
                        if (drAnbar["ccs_stock_id"].ToString() == stock)
                        {
                            stock2 = drAnbar["ccs_stock_id"].ToString();
                            break;
                        }
                    }
                }

                Result0 = false;
                Arraycreditlimit = new ArrayList();

                DataRow[] drfilter = dtResultkol0.Select(" ccd_cc_id =" + dr["ccd_cc_id"].ToString());
                foreach (DataRow drfil in drfilter)
                {
                    string ccd_type = drfil["ccd_type"].ToString();
                    string ccd_itemtype = drfil["ccd_itemtype"].ToString();
                    string conditionStr = "";
                    string conditionStrFA = "";
                    string conditionStr2 = "";
                    string ccd_condition = drfil["ccd_condition"].ToString();
                    if (ccd_condition == "0")
                    {
                        conditionStr = " > ";
                        conditionStrFA = " بزرگتر از ";
                    }
                    if (ccd_condition == "1")
                    {
                        conditionStr = " <  ";
                        conditionStrFA = " کوچکتر از ";
                    }
                    if (ccd_condition == "2")
                    {
                        conditionStr = " = ";
                        conditionStrFA = " مساوی ";
                    }
                    if (ccd_condition == "3")
                    {
                        conditionStr = " != ";
                        conditionStrFA = " مخالف ";
                    }
                    if (ccd_condition == "4")
                    {
                        conditionStr = " >= ";
                        conditionStrFA = "بزرگتر مساوی";
                    }
                    if (ccd_condition == "5")
                    {
                        conditionStr = " <= ";
                        conditionStrFA = "کوچکتر مساوی";
                    }

                    //شرط بین باشد
                    if (ccd_condition == "6")
                    {
                        conditionStr = " >= ";
                        conditionStr2 = " <= ";
                        conditionStrFA = " بین ";
                    }

                    DataTable DtResualt;
                    #region  اگر نوع معین باشد
                    if (drfil["ccd_type"].ToString() == "0")
                    {
                        DtResualt = TypleSellLimitccd_type0(0, type, ccd_id, customer, stock, conditionStr, conditionStr2, ccd_itemtype, drfil["ccd_moneyF"].ToString(), drfil["ccd_moneyT"].ToString(), drfil["ccd_countF"].ToString(), drfil["ccd_countT"].ToString(),
                        drfil["ccd_period"].ToString(), drfil["ccd_periodtype"].ToString(), MetrajJari, MoneyJari, TypeJari);
                        //اگر فرم جاری فرم مدنظر نباشد مثلا درخواست بوده ولی داره پیش فاکتور جاری رو میخونه
                        if (DtResualt.Rows[0][0].ToString() != "-1")
                        {
                            string natijr = "";
                            if (DtResualt.Rows[0][2].ToString() != "")
                                natijr = Convert.ToDouble(DtResualt.Rows[0][2].ToString()).ToString();
                            else
                                natijr = "مقداری یافت نشد";

                            //متغییر معین
                            //مقایسه شرط و
                            if (type == 0)
                            {
                                if (DtResualt.Rows[0][1].ToString() == "True")
                                    Arraycreditlimit.Add("1");
                                if (DtResualt.Rows[0][1].ToString() == "False")
                                {
                                    Arraycreditlimit.Add("0");
                                    if (conditionStr2 == "")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " " + conditionStrFA + " " + drfil["ccd_moneyF"].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " بین " + drfil["ccd_moneyF"].ToString() + " و " + drfil["ccd_moneyT"].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");
                                }
                            }
                            if (type == 1)
                            {
                                if (DtResualt.Rows[0][1].ToString() == "True")
                                    Arraycreditlimit.Add("1");
                                if (DtResualt.Rows[0][1].ToString() == "False")
                                {
                                    Arraycreditlimit.Add("0");
                                    if (conditionStr2 == "")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " " + conditionStrFA + " " + drfil["ccd_moneyF"].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " بین " + drfil["ccd_moneyF"].ToString() + " و " + drfil["ccd_moneyT"].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                }
                            }
                            //}
                            //else
                            //{
                            //    if (type == 0) 
                            //        Arraycreditlimit.Add("1");
                            //    if (type == 1)
                            //    {
                            //        if (conditionStr2 == "")
                            //            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " " + conditionStrFA + " " + drfil["ccd_moneyF"].ToString() + " نمی باشد ");
                            //        else
                            //            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مبلغ معین با کد " + ccd_itemtype + " بین " + drfil["ccd_moneyF"].ToString() + " و " + drfil["ccd_moneyT"].ToString() + " نمی باشد ");
                            //        Arraycreditlimit.Add("0");
                            //    }
                            //}
                        }
                    }
                    #endregion

                    #region  اگر نوع فروش باشد
                    if (drfil["ccd_type"].ToString() == "1")
                    {
                        DtResualt = TypleSellLimitccd_type0(1, type, ccd_id, customer, stock, conditionStr, conditionStr2, ccd_itemtype, drfil["ccd_moneyF"].ToString(), drfil["ccd_moneyT"].ToString(), drfil["ccd_countF"].ToString(), drfil["ccd_countT"].ToString(), drfil["ccd_period"].ToString(), drfil["ccd_periodtype"].ToString(), MetrajJari, MoneyJari, TypeJari);
                        ////مقایسه شرط و
                        if (DtResualt.Rows[0][0].ToString() != "-1")
                        {
                            //if (DtResualt.Rows[0][0].ToString()!="0")
                            //{                        
                            string Str = "";
                            string MoCo = "";
                            string MoCoT = "";
                            if (ccd_itemtype == "0")
                            {
                                Str = " تعداد درخواست خرید ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "1")
                            {
                                Str = " متراژ درخواست خرید ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "2")
                            {
                                Str = " تعداد پیش فاکتور ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "3")
                            {
                                Str = " مبلغ پیش فاکتور ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "4")
                            {
                                Str = " تعداد بارگیری ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "5")
                            {
                                Str = " مبلغ بارگیری ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "6")
                            {
                                Str = " تعداد فاکتور ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "7")
                            {
                                Str = " مبلغ فاکتور ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "8")
                            {
                                Str = " متراژ پیش فاکتور ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "9")
                            {
                                Str = " متراژ بارگیری ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "10")
                            {
                                Str = " متراژ فاکتور ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "11")
                            {
                                Str = " صورت حساب مشتری ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "12")
                            {
                                Str = " مبلغ پیش فاکتور جاری ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "13")
                            {
                                Str = " متراژ پیش فاکتور جاری ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "14")
                            {
                                Str = " متراژ درخواست خرید جاری ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            string natijr = "";
                            if (DtResualt.Rows[0][2].ToString() != "")
                                natijr = Convert.ToDouble(DtResualt.Rows[0][2].ToString()).ToString();
                            else
                                natijr = "مقداری یافت نشد";

                            if (type == 0)
                            {
                                if (DtResualt.Rows[0][1].ToString() == "True")
                                    Arraycreditlimit.Add("1");
                                if (DtResualt.Rows[0][1].ToString() == "False")
                                {
                                    Arraycreditlimit.Add("0");
                                    if (ccd_condition != "6")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " " + conditionStrFA + " " + drfil[MoCo].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " بین " + drfil[MoCo].ToString() + " و " + drfil[MoCoT].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");
                                }
                            }
                            if (type == 1)
                            {

                                if (DtResualt.Rows[0][1].ToString() == "True")
                                    Arraycreditlimit.Add("1");
                                if (DtResualt.Rows[0][1].ToString() == "False")
                                {
                                    Arraycreditlimit.Add("0");
                                    if (ccd_condition != "6")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " " + conditionStrFA + " " + drfil[MoCo].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " بین " + drfil[MoCo].ToString() + " و " + drfil[MoCoT].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                }
                            }
                            //}
                            //else
                            //{
                            //    if (type == 0)
                            //    {
                            //        Arraycreditlimit.Add("1");
                            //    }
                            //    if (type == 1)
                            //    {
                            //        if (ccd_itemtype == "0")
                            //        {

                            //            if (ccd_condition != "6")
                            //                DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " تعداد درخواست خرید " + " " + conditionStrFA + " " + drfil["ccd_countF"].ToString() + " نمی باشد ");
                            //            else
                            //                DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " تعداد درخواست خرید " + " بین " + drfil["ccd_countF"].ToString() + " و " + drfil["ccd_countT"].ToString() + " نمی باشد ");
                            //        }
                            //        Arraycreditlimit.Add("0");
                            //    }
                            //}
                            { }
                        }
                    }
                    #endregion

                    #region اگر نوع سایر باشد
                    if (drfil["ccd_type"].ToString() == "2")
                    {
                        //if (ccd_condition == "6")
                        //{
                        //    conditionStr = " >= ";
                        //   // conditionStr2 = " AND SUM(i.debit-i.credit) <= " + drfil["ccd_moneyT"].ToString();
                        //}
                        DtResualt = TypleSellLimitccd_type0(2, type, ccd_id, customer, stock, conditionStr, conditionStr2, ccd_itemtype, drfil["ccd_moneyF"].ToString(), drfil["ccd_moneyT"].ToString(), drfil["ccd_countF"].ToString(), drfil["ccd_countT"].ToString(), drfil["ccd_period"].ToString(), drfil["ccd_periodtype"].ToString(), MetrajJari, MoneyJari, TypeJari);
                        if (DtResualt.Rows[0][0].ToString() != "-1")
                        {
                            //مقایسه شرط و
                            string Str = "";
                            string MoCo = "";
                            string MoCoT = "";
                            if (ccd_itemtype == "50")
                            {
                                Str = " اعتبار سنجی ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "51")
                            {
                                Str = " تعداد چک برگشتی سی ار ام ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "52")
                            {
                                Str = " مبلغ چک برگشتی سی ار ام ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "53")
                            {
                                Str = " تعداد چک های نقد شده ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "54")
                            {
                                Str = " تعداد چک های نقد شده ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "55")
                            {
                                Str = " تعداد چک های واخواست شده ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "56")
                            {
                                Str = " مبلغ چک های واخواست شده ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "57")
                            {
                                Str = " تعداد چک های واگذار به بانک ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "58")
                            {
                                Str = " مبلغ چک های واگذار به بانک ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "59")
                            {
                                Str = " تعداد چک های وصول شده ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "60")
                            {
                                Str = " مبلغ چک های وصول شده ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "61")
                            {
                                Str = " تعداد چک های نزد صندوق ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "62")
                            {
                                Str = " مبلغ چک های نزد صندوق ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "63")
                            {
                                Str = " تعداد چک های استرداد شده ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countT";
                            }
                            if (ccd_itemtype == "64")
                            {
                                Str = " مبلغ چک های استرداد شده ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (ccd_itemtype == "65")
                            {
                                Str = " تعداد چک های خرج شده ";
                                MoCo = "ccd_countF";
                                MoCoT = "ccd_countF";
                            }
                            if (ccd_itemtype == "66")
                            {
                                Str = " مبلغ چک های خرج شده ";
                                MoCo = "ccd_moneyF";
                                MoCoT = "ccd_moneyT";
                            }
                            if (DtResualt.Rows.Count > 0)
                            {
                                string natijr = "";
                                if (DtResualt.Rows[0][2].ToString() != "")
                                    natijr = Convert.ToDouble(DtResualt.Rows[0][2].ToString()).ToString();
                                else
                                    natijr = "مقداری یافت نشد";
                                string gg = DtResualt.Rows[0][1].ToString();
                                if (DtResualt.Rows[0][1].ToString() == "True")
                                    Arraycreditlimit.Add("1");
                                if (DtResualt.Rows[0][1].ToString() == "False")
                                {
                                    Arraycreditlimit.Add("0");

                                    if (type == 0)
                                    {
                                        if (ccd_condition != "6")
                                            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " " + conditionStrFA + " " + drfil[MoCo].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");
                                        else
                                            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " بین " + drfil[MoCo].ToString() + " و " + drfil[MoCoT].ToString() + " می باشد " + " ( نتیجه : " + natijr + " ) ");

                                    }
                                    if (type == 1)
                                    {
                                        if (ccd_condition != "6")
                                            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " " + conditionStrFA + " " + drfil[MoCo].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                        else
                                            DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " بین " + drfil[MoCo].ToString() + " و " + drfil[MoCoT].ToString() + " نمی باشد " + " ( نتیجه : " + natijr + " ) ");
                                    }
                                }
                            }
                            else
                            {
                                if (type == 0)
                                    Arraycreditlimit.Add("1");
                                if (type == 1)
                                {
                                    if (ccd_condition != "6")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " " + conditionStrFA + " " + drfil[MoCo].ToString() + " نمی باشد " + " ( نتیجه : " + " مقداری یافت نشد " + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + Str + " بین " + drfil[MoCo].ToString() + " و " + drfil[MoCoT].ToString() + " نمی باشد " + " ( نتیجه : " + " مقداری یافت نشد " + " ) ");

                                    Arraycreditlimit.Add("0");
                                }
                            }
                        }
                    }
                    #endregion

                    #region اگر نوع ترکیبی باشد
                    if (drfil["ccd_type"].ToString() == "3")
                    {
                        //string msg = "[gggg]+[6]+[6666]-[5555]/[555554]";
                        // var data2 = msg.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        try
                        {
                            string msg = drfil["ccd_itemtype"].ToString();// "[gggg]+[6]+[6666]-([5555]/[555554])";
                            string[] copy = new string[100];
                            //Array copyu = new Array;
                            msg.Split(new string[] { "[", "]" }, StringSplitOptions.RemoveEmptyEntries).CopyTo(copy, 0);
                            int number = 0;
                            foreach (string str in copy)
                            {
                                if (str != null)
                                {
                                    try
                                    {
                                        int number0 = Int32.Parse(str);
                                        if (number0 < 50) { number = 1; }
                                        if (number0 >= 50 & number0 < 100) { number = 2; }
                                        if (number0 >= 100) { number = 0; }
                                        string CommandTextResualt = "";
                                        DataTable Dtresult = new DataTable();
                                        _DS = new DataSet();
                                        _DA = new SqlDataAdapter();
                                        Command = new SqlCommand("", cp.connect());
                                        _DA.SelectCommand = Command;
                                        Command.CommandType = CommandType.Text;

                                        DataTable dtSale;
                                        CommandTextResualt = CommandTextR(number, type, ccd_id, customer, stock, str, true, drfil["ccd_period"].ToString(), drfil["ccd_periodtype"].ToString(), MetrajJari, MoneyJari, TypeJari);
                                        if (str == "11" || (Int32.Parse(str) >= 53 & Int32.Parse(str) <= 66))
                                            Command.Parameters.Add("@date2", SqlDbType.DateTime).Value = DTPeriodType2;
                                        else if (str != "12" & str != "13" & str != "14")
                                            Command.Parameters.Add("@date", SqlDbType.DateTime).Value = DTPeriodType;
                                        Command.CommandText = CommandTextResualt;
                                        _DA.Fill(_DS);
                                        dtSale = _DS.Tables[0];
                                        Dtresult = dtSale;
                                        //int ggg = Dtresult.Rows.Count;
                                        string Res = "";

                                        foreach (DataRow drh in dtSale.Rows) { }
                                        if (Dtresult.Rows.Count == 0)
                                            Res = "0";
                                        else
                                            Res = Dtresult.Rows[0][0].ToString();
                                        if (Res == "")
                                            Res = "0";
                                        if (Res.Contains("/"))
                                        {
                                            Res = Res.Replace("/", ".");
                                        }
                                        msg = msg.Replace("[" + str + "]", Res);
                                    }
                                    catch (Exception ex) { }
                                }
                            }
                            decimal FeeListt2 = 0;
                            try
                            {
                                DataTable dt = new DataTable();
                                object result = dt.Compute(msg, "");
                                FeeListt2 = decimal.Parse(result.ToString());
                            }
                            catch { }
                            decimal fiii = 0;
                            decimal fiii2 = 0;
                            if (drfil["ccd_moneyF"].ToString() != "")
                            {
                                decimal fiiie = decimal.Parse("2000000000");
                                fiii = decimal.Parse(drfil["ccd_moneyF"].ToString());
                                if (drfil["ccd_moneyT"].ToString() != "")
                                    fiii2 = decimal.Parse(drfil["ccd_moneyT"].ToString());
                            }
                            else if (drfil["ccd_countF"].ToString() != "")
                            {
                                fiii = decimal.Parse(drfil["ccd_countF"].ToString());
                                if (drfil["ccd_countT"].ToString() != "")
                                    fiii2 = decimal.Parse(drfil["ccd_countT"].ToString());
                            }
                            bool checktype = false;
                            switch (drfil["ccd_condition"].ToString())
                            {
                                case "0":
                                    if (FeeListt2 > fiii)
                                        checktype = true;
                                    break;
                                case "1":
                                    if (FeeListt2 < fiii)
                                        checktype = true;
                                    break;
                                case "2":
                                    if (FeeListt2 == fiii)
                                        checktype = true;
                                    break;
                                case "3":
                                    if (FeeListt2 != fiii)
                                        checktype = true;
                                    break;
                                case "4":
                                    if (FeeListt2 >= fiii)
                                        checktype = true;
                                    break;
                                case "5":
                                    if (FeeListt2 <= fiii)
                                        checktype = true;
                                    break;
                                case "6":
                                    if (FeeListt2 >= fiii & FeeListt2 <= fiii2)
                                        checktype = true;
                                    break;
                                    //case "1": return left.CompareTo(right) > 0;
                                    //case "2": return left.CompareTo(right) <= 0;
                                    //case "3": return left.CompareTo(right) >= 0;
                                    //case "4": return left.Equals(right);
                                    //case "5": return !left.Equals(right);
                                    //  default: throw new ArgumentException("Invalid comparison operator: {0}", op);
                            }
                            if (ccd_condition == "6")
                            {
                                conditionStr = " >= ";
                            }

                            if (checktype)
                            {
                                if (type == 0)
                                {
                                    if (conditionStr2 == "")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مقدار فرمول ترکیبی " + " " + conditionStrFA + " " + fiii + " می باشد " + " ( نتیجه : " + Convert.ToDouble(FeeListt2.ToString()).ToString() + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مقدار فرمول ترکیبی " + " بین " + fiii + " و " + fiii2 + " می باشد " + " ( نتیجه : " + Convert.ToDouble(FeeListt2.ToString()).ToString() + " ) ");
                                    Arraycreditlimit.Add("0");
                                }
                                if (type == 1)
                                    Arraycreditlimit.Add("1");
                            }
                            else
                            {
                                if (type == 0)
                                    Arraycreditlimit.Add("1");
                                if (type == 1)
                                {
                                    if (conditionStr2 == "")
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مقدار فرمول ترکیبی " + " " + conditionStrFA + " " + fiii + " نمی باشد " + " ( نتیجه : " + Convert.ToDouble(FeeListt2.ToString()).ToString() + " ) ");
                                    else
                                        DTSEllLimited.Rows.Add(" محدودیت شماره " + drfil["cc_no"].ToString() + " : " + " مقدار فرمول ترکیبی " + " بین " + fiii + " و " + fiii2 + " نمی باشد " + " ( نتیجه : " + Convert.ToDouble(FeeListt2.ToString()).ToString() + " ) ");

                                    Arraycreditlimit.Add("0");
                                }
                            }

                        }
                        catch (Exception ex) { }
                    }
                    #endregion

                    //چک میشود در زمان نامعتبری اگر حداقل یکی شرط بر طبق نامعتبری نبود 1 ارسال شود و خروجی 1 شود
                    //چک میشود در زمان معتبری اگر حداقل یکی شرط بر طبق معتبری نبود 0 ارسال شود و خروجی 0 شود
                    //در این جالت شرط (و) بررسی میشود
                    if (type == 0)
                        Result0 = false;
                    if (type == 1)
                        Result0 = true;

                    foreach (string str in Arraycreditlimit)
                    {
                        if (type == 0)
                            if (str == "1")
                                Result0 = true;
                        if (type == 1)
                            if (str == "0")
                                Result0 = false;
                    }
                    // دراین شرط (یا) در لیست ریخته میشود نتیجه شروط (و) در لیست ریخته میشود تا (یا) ها را تشکیل دهد
                    if (Result0)
                        Arraycreditlimit0.Add("1");
                    if (!Result0)
                        Arraycreditlimit0.Add("0");
                }
                // دراین حالت شرط (یا) چک میشود اگر نامعتبری باشد اگر حداقل یکی 0 بود خروجی 0 نمایش داده میشود
                // دراین حالت شرط (یا) چک میشود اگر معتبری باشد اگر حداقل یکی 1 بود خروجی 1 نمایش داده میشود
                foreach (string str in Arraycreditlimit0)
                {
                    if (type == 0)
                        if (str == "0")
                            Result0 = false;
                    if (type == 1)
                        if (str == "1")
                            Result0 = true;
                }
            }
            return Result0;
        }

        public static DataTable TypleSellLimitccd_type0(int number, int type, string ccd_id, int customer, string stock, string conditionStr, string conditionStr2, string ccd_itemtype, string drfilmoneyF, string drfilmoneyT, string drfilcountF, string drfilcountT, string ccd_period, string ccd_periodtype, decimal MetrajJari, decimal MoneyJari, bool TypeJari)
        {
            DTPeriodType = DateTime.Now;
            DTPeriodType2 = DateTime.Now;
            string CommandTextResualt = "";
            DataTable Dtresult = new DataTable();
            DataSet _DS = new DataSet();
            SqlDataAdapter _DA = new SqlDataAdapter();
            SqlCommand Command = new SqlCommand("", cp.connect());
            _DA.SelectCommand = Command;
            Command.CommandType = CommandType.Text;

            DataTable dtSale;
            CommandTextResualt = CommandTextR(number, type, ccd_id, customer, stock, ccd_itemtype, false, ccd_period, ccd_periodtype, MetrajJari, MoneyJari, TypeJari);
            Command.CommandText = CommandTextResualt;
            if (ccd_itemtype == "11" || (Int32.Parse(ccd_itemtype) >= 53 & Int32.Parse(ccd_itemtype) <= 66))
                Command.Parameters.Add("@date2", SqlDbType.DateTime).Value = DTPeriodType2;
            else if (ccd_itemtype != "12" & ccd_itemtype != "13" & ccd_itemtype != "14")
                Command.Parameters.Add("@date", SqlDbType.DateTime).Value = DTPeriodType;
            _DA.Fill(_DS);
            dtSale = _DS.Tables[0];
            dtSale.Columns.Add("Check", typeof(bool));
            dtSale.Columns.Add("Resualt", typeof(decimal));
            if (dtSale.Rows.Count == 0)
                dtSale.Rows.Add(DBNull.Value, DBNull.Value, 0);

            if (dtSale.Rows[0][0].ToString() != "-1")
            {
                foreach (DataRow dr in dtSale.Rows)
                {
                    if (dr[0].ToString() == "")
                    {
                        dr.BeginEdit();
                        if (type == 0)//مجاز نمیباشد
                        {
                            if (conditionStr.Contains(">") || conditionStr.Contains(">=") || conditionStr == " = " || conditionStr == " != ")
                                dr[1] = 1;//مجاز
                            else dr[1] = 0;//غیر مجاز
                        }
                        if (type == 1)//مجاز می باشد
                        {
                            if (conditionStr.Contains(">") || conditionStr.Contains(">="))
                                dr[1] = 0;//غیر مجاز
                            else if (conditionStr.Contains("<") || conditionStr.Contains("<=") || conditionStr == " = " || conditionStr == " != ")
                                dr[1] = 1;//مجاز
                            else dr[0] = 0;//غیر مجاز
                        }
                        dr.EndEdit();
                        dr.AcceptChanges();
                    }
                }

                bool checktype = false;
                decimal countmoneyF = 0;
                decimal countmoneyT = 0;
                if (drfilmoneyF != "")
                    countmoneyF = decimal.Parse(drfilmoneyF);
                if (drfilcountF != "")
                    countmoneyF = decimal.Parse(drfilcountF);

                if (drfilmoneyT != "")
                    countmoneyT = decimal.Parse(drfilmoneyT);
                if (drfilcountT != "")
                    countmoneyT = decimal.Parse(drfilcountT);

                foreach (DataRow dr in dtSale.Rows)
                {
                    if (dr[1].ToString() == "")
                    {
                        dr.BeginEdit();
                        if (conditionStr.Contains(">="))
                        {
                            if (conditionStr2 != "")
                                if ((decimal.Parse(dr[0].ToString()) >= countmoneyF & decimal.Parse(dr[0].ToString()) <= countmoneyT))
                                {
                                    if (type == 0)//مجاز نمیباشد
                                        dr[1] = 0;
                                    if (type == 1)//مجاز می باشد
                                        dr[1] = 1;
                                }
                                else
                                {
                                    if (type == 0)//مجاز نمیباشد
                                        dr[1] = 1;
                                    if (type == 1)//مجاز می باشد
                                        dr[1] = 0;
                                }
                            if (conditionStr == "")
                                if ((decimal.Parse(dr[0].ToString()) >= countmoneyF) & dr[1].ToString() == "")
                                {
                                    if (type == 0)//مجاز نمیباشد
                                        dr[1] = 0;
                                    if (type == 1)//مجاز می باشد
                                        dr[1] = 1;
                                }
                                else
                                {
                                    if (type == 0)//مجاز نمیباشد
                                        dr[1] = 1;
                                    if (type == 1)//مجاز می باشد
                                        dr[1] = 0;
                                }
                        }
                        else if (conditionStr.Contains("<="))
                        {
                            if ((decimal.Parse(dr[0].ToString()) <= countmoneyF))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }
                        else if (conditionStr.Contains("!="))
                        {
                            if ((decimal.Parse(dr[0].ToString()) != countmoneyF))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }
                        else if (conditionStr.Contains("="))
                        {
                            if ((decimal.Parse(dr[0].ToString()) == countmoneyF))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }
                        else if (conditionStr.Contains(">"))
                        {
                            if ((decimal.Parse(dr[0].ToString()) > countmoneyF))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }
                        else if (conditionStr.Contains("<"))
                        {
                            if ((decimal.Parse(dr[0].ToString()) < countmoneyF))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }

                        else if (conditionStr.Contains("ب"))
                        {
                            if ((decimal.Parse(dr[0].ToString()) >= countmoneyF & decimal.Parse(dr[0].ToString()) <= countmoneyT))
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 0;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 1;
                            }
                            else
                            {
                                if (type == 0)//مجاز نمیباشد
                                    dr[1] = 1;
                                if (type == 1)//مجاز می باشد
                                    dr[1] = 0;
                            }
                        }
                        dr["Resualt"] = decimal.Parse(dr[0].ToString());
                        dr.EndEdit();
                        dr.AcceptChanges();
                    }
                }
            }
            Dtresult = dtSale;
            return Dtresult;
        }

        public static string CommandTextR(int number, int type, string ccd_id, int customer, string stock,
            string ccd_itemtype, bool state,
            string ccd_period, string ccd_periodtype, decimal MetrajJari, decimal MoneyJari, bool TypeJari)
        {
            DateTime DTNOW = cp.GetDateTime().Date;
            DateTime DTNOW2 = DTNOW;
            if (ccd_periodtype == "0")
            {
                DTNOW = DTNOW.AddYears(-(Int32.Parse(ccd_period) - 1));
                string DTNOWStr = ShamsiDate.MiladiToShamsi(DTNOW.Date);
                DTNOWStr = DTNOWStr.Substring(0, 4) + "/01/01";
                DTNOW = ShamsiDate.Shamsi2Miladi(DTNOWStr);
            }

            if (ccd_periodtype == "1")
                DTNOW = DTNOW.AddMonths(-(Int32.Parse(ccd_period)));
            string StrResualt = "";
            DTPeriodType = DTNOW;

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

            DTPeriodType2 = DTNOW2;

            if (number == 0)
            {
                string shart = "";
                //if (state)
                //    shart = "";
                //else
                //{
                //    conditionStr2 = " AND SUM(i.debit-i.credit) <= " + drfilmoneyT;
                //    shart = " having SUM(i.debit - i.credit) " + conditionStr + " " + drfilmoneyF + conditionStr2;
                //}
                StrResualt = @"select SUM(i.debit - i.credit) from " + DataMali + @".ACC.vwVoucherItem i
                    where Convert(date,i.VoucherDate)>=@date and DLRef=(select csT_dl from dbo.PRG_Customers where cst_id=" +
                             customer.ToString() + @")
                    and i.SLCode ='" + ccd_itemtype + "'"; // + @"' group by i.SLCode " + shart;
            }

            if (number == 1)
            {
                if (ccd_itemtype == "0")
                {

                    // شرط تعداد درخواست خرید
                    string shart = "";

                    string stock3 = "";
                    if (stock2 == "")
                        stock3 = "";
                    else
                        stock3 = " AND RBI_stock =" + stock.ToString() + " AND ";

                    StrResualt = @"SELECT     count(RB_cst_id)
                    FROM         PRG_RequestBuy INNER JOIN
                                          PRG_RequestBuyItem ON PRG_RequestBuy.RB_id = PRG_RequestBuyItem.RBI_RB_id
                                          where (RB_type = 1) " + stock3 + @" RB_cst_id=" + customer.ToString() +
                                 @" and Convert(date,RB_date)>=@date "; //group by RB_cst_id                     " + shart;

                }

                if (ccd_itemtype == "1")
                {
                    // شرط متراژ درخواست خرید
                    string shart = "";

                    string stock3 = "";
                    if (stock2 == "")
                        stock3 = "";
                    else
                        stock3 = " AND RBI_stock =" + stock.ToString() + " AND ";

                    StrResualt = @"SELECT isnull(Sum(RBI_metraj),0)+" + MetrajJari.ToString().Replace("/", ".") + @"
                    FROM         PRG_RequestBuy INNER JOIN
                                          PRG_RequestBuyItem ON PRG_RequestBuy.RB_id = PRG_RequestBuyItem.RBI_RB_id
                    where (RB_type = 1) " + stock3 + @" RB_cst_id=" + customer.ToString() +
                                 @" and Convert(date,RB_date)>=@date "; //group by RB_cst_id                     " + shart;
                }

                if (ccd_itemtype == "2")
                {
                    // شرط تعداد پیش فاکتور
                    string shart = "";

                    string stock3 = "";
                    if (stock2 == "")
                        stock3 = "";
                    else
                        stock3 = " AND QUI_stock =" + stock.ToString() + " AND ";

                    StrResualt = @"SELECT count(QU_cst_id) 
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id
                    where (QU_type = 1) " + stock3 + @" QU_cst_id=" + customer.ToString() +
                                 @" and Convert(date,QU_date)>=@date "; //group by QU_cst_id                     " + shart;
                }

                if (ccd_itemtype == "3")
                {
                    // شرط مبلغ پیش فاکتور
                    string shart = "";

                    StrResualt = @"SELECT Sum(QUI_NetPrice) 
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id
                    where (QU_type = 1) AND QU_cst_id=" + customer.ToString() +
                                 @" and Convert(date,QU_date)>=@date "; //group by QU_cst_id                         " + shart;
                    //  where(QU_type = 1) " + stock3 + @" QU_cst_id = " + customer.ToString() + @" and Convert(date, QU_date)>= @date ";//group by QU_cst_id                         " + shart;
                }

                if (ccd_itemtype == "8")
                {
                    // شرط متراژ پیش فاکتور
                    string shart = "";

                    StrResualt = @"SELECT Sum(QUI_value)
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id
                    where (QU_type = 1) AND QU_cst_id=" + customer.ToString() +
                                 @" and Convert(date,QU_date)>=@date "; //group by QU_cst_id                     " + shart;

                }

                if (ccd_itemtype == "4")
                {
                    // شرط تعداد بارگیری
                    string shart = "";

                    StrResualt = @"SELECT count(LD_cst_id) 
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id INNER JOIN
                      PRG_Lading ON PRG_Quotation.QU_id = PRG_Lading.LD_Qu_id
                    where (LD_state = 1) AND LD_cst_id=" + customer.ToString() +
                                 @" and Convert(date,LD_datebargiri)>=@date"; // group by LD_cst_id                         " + shart;
                }

                if (ccd_itemtype == "5")
                {
                    // شرط مبلغ بارگیری
                    string shart = "";

                    StrResualt = @"SELECT Sum(QUI_NetPrice)
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id INNER JOIN
                      PRG_Lading ON PRG_Quotation.QU_id = PRG_Lading.LD_Qu_id
                    where (LD_state = 1) AND LD_cst_id=" + customer.ToString() +
                                 @" and Convert(date,LD_datebargiri)>=@date "; //group by LD_cst_id                     " + shart;
                }

                if (ccd_itemtype == "9")
                {
                    // شرط متراژ بارگیری
                    string shart = "";

                    StrResualt = @"SELECT Sum(QUI_value)
                    FROM         PRG_Quotation INNER JOIN
                                          PRG_QuotationItem ON PRG_Quotation.QU_id = PRG_QuotationItem.QUI_QU_id INNER JOIN
                      PRG_Lading ON PRG_Quotation.QU_id = PRG_Lading.LD_Qu_id
                    where (LD_state = 1) AND LD_cst_id=" + customer.ToString() +
                                 @" and Convert(date,LD_datebargiri)>=@date"; //group by LD_cst_id                         " + shart;
                }

                if (ccd_itemtype == "6")
                {
                    // شرط تعداد فاکتور
                    string shart = "";
                    //string stock3 = "";
                    //if (stock2 == "")
                    //    stock3 = "";
                    //else
                    //    stock3 = " AND StockRef =" + stock.ToString() + " AND ";

                    StrResualt = @"SELECT count(INV.CustomerPartyRef) 
FROM " + DataMali + @".SLS.Invoice INV INNER JOIN
                      " + DataMali + @".SLS.InvoiceItem INVITM ON INV.InvoiceId = INVITM.InvoiceRef
                    where (INV.State = 1) AND INV.CustomerPartyRef=(SELECT PartyId FROM " + DataMali + @".GNR.Party
WHERE DLRef = (SELECT csT_dl FROM PRG_Customers WHERE cst_id = " + customer.ToString() +
                                 @")) and INV.QuotationRef is not null AND Convert(date,INV.Date)>=@date "; //group by INV.CustomerPartyRef                     " + shart;
                }

                if (ccd_itemtype == "7")
                {
                    // شرط مبلغ فاکتور
                    string shart = "";
                    //string stock3 = "";
                    //if (stock2 == "")
                    //    stock3 = "";
                    //else
                    //    stock3 = " AND StockRef =" + stock.ToString() + " AND ";

                    StrResualt = @"SELECT SUM(INVITM.NetPriceInBaseCurrency)
FROM " + DataMali + @".SLS.Invoice INV INNER JOIN
                      " + DataMali + @".SLS.InvoiceItem INVITM ON INV.InvoiceId = INVITM.InvoiceRef
                    where (INV.State = 1) AND INV.CustomerPartyRef=(SELECT PartyId FROM " + DataMali + @".GNR.Party
WHERE DLRef = (SELECT csT_dl FROM PRG_Customers WHERE cst_id = " + customer.ToString() +
                                 @")) and INV.QuotationRef is not null AND Convert(date,INV.Date)>=@date "; //group by INV.CustomerPartyRef                     " + shart;
                }

                if (ccd_itemtype == "12")
                {
                    // مبلغ پیش فاکتور جاری
                    if (TypeJari)
                        StrResualt = @"SELECT " + MoneyJari;
                    else
                        StrResualt = @"SELECT -1";
                }

                if (ccd_itemtype == "13")
                {
                    // متراژ پیش فاکتور جاری
                    if (TypeJari)
                        StrResualt = @"SELECT " + MetrajJari;
                    else
                        StrResualt = @"SELECT -1";
                }

                if (ccd_itemtype == "14")
                {
                    // متراژ درخواست خرید جاری 
                    if (!TypeJari)
                        StrResualt = @"SELECT " + MetrajJari;
                    else
                        StrResualt = @"SELECT -1";
                }

                if (ccd_itemtype == "11")
                {
                    DataTable dtfiscalyear = classes.clsDB.GetFiscalYearNow(classes.clsDB.GetFiscalYearNowStr());
                    string FiscalYearId = dtfiscalyear.Rows[0]["FiscalYearId"].ToString();
                    // صورت حساب مشتری
                    StrResualt = @"declare @year int
set @year=" + FiscalYearId + @"
IF OBJECT_ID('tempdb..#t') IS NOT NULL
    DROP TABLE #t 
create table #t
    (
        RecordType           int,
        EntityID             int,
        EntityItemID         int,
        FiscalYearRef        int,
        Number               int,
        PartyRef             int,
        VoucherRef           int,
        Description          nvarchar(max) COLLATE DATABASE_DEFAULT,
        Amount               [decimal](19, 4),
        CurrencyRef          int,
        AmountInBaseCurrency [decimal](19, 4),
        Date                 datetime,
        PartyRoleType        int,
        CustomerRemainFactor int,
        VendorRemainFactor   int,
        BrokerFactor         int,
        OtherFactor          int,
        PettyCashFactor      int
    ) 
CREATE  CLUSTERED INDEX IX_#t_Date ON #t ([Date]); 

declare @dlref int
set @dlref=(SELECT csT_dl FROM PRG_Customers WHERE cst_id = " + customer.ToString() + @")
INSERT                                                      
INTO #t EXEC " + DataMali + @".GNR.spReviewPartyRemainder @ContainsCheque = True, 
                                          @SystemCurrency = 1,
                                          @FiscalYearRef = @year, 
                                          @FilterFiscalYear = @year,
                                          @PartyRef = @dlref,
                                          @PettyCashInstallFiscalYear = @year
SELECT	distinct Remain = ISNULL((SELECT SUM(CustomerRemainFactor * ISNULL(AmountInBaseCurrency,0))FROM #t WHERE FiscalYearRef = @year AND PartyRef = @dlref AND CustomerRemainFactor * 1 <> 0 AND (Date < @date2 OR Date IS NULL)),0) + 
                  ISNULL((SELECT SUM(VendorRemainFactor * ISNULL(AmountInBaseCurrency,0)) FROM #t WHERE FiscalYearRef = @year AND PartyRef = @dlref AND VendorRemainFactor * 1 <> 0 AND (Date < @date2 OR Date IS NULL)),0) +
                  ISNULL((SELECT SUM(BrokerFactor * ISNULL(AmountInBaseCurrency,0)) FROM #t WHERE FiscalYearRef = @year AND PartyRef = @dlref AND BrokerFactor * 1 <> 0 AND (Date < @date2 OR Date IS NULL)),0) +
                  ISNULL((SELECT SUM (OtherFactor * ISNULL(AmountInBaseCurrency,0))FROM #t WHERE PartyRef = @dlref AND OtherFactor * 1 <> 0 AND Date < @date2),0) +
                  ISNULL((SELECT SUM(PettyCashFactor * ISNULL(AmountInBaseCurrency, 0))FROM #t WHERE PartyRef = @dlref AND PettyCashFactor * 1 <> 0 AND (Date < @date2 OR Date IS NULL) AND FiscalYearRef = @year),0)
FROM #t t
WHERE (t.Date < @date2 OR t.Date IS NULL ) ";
                    //string shart = "";
                    //if (state)
                    //    shart = "";
                    //else
                    //{
                    //    conditionStr2 = " AND SUM([Debit]-[Credit]) <= " + drfilmoneyT;
                    //    shart = " HAVING SUM([Debit]-[Credit]) " + conditionStr + " " + drfilmoneyF + conditionStr2;
                    //}
                    //StrResualt = @"SELECT SUM([Debit]-[Credit]) FROM " + DataMali + @".ACC.vwVoucherItem WHERE DLRef = (SELECT csT_dl FROM PRG_Customers WHERE cst_id = " + customer.ToString() + @") AND SLCode ='111201' AND Convert(date,VoucherDate)>=@date";// group by DLRef                     " + shart;
                }

                if (ccd_itemtype == "10")
                {
                    string shart = "";
                    // شرط متراژ فاکتور
                    //string stock3 = "";
                    //if (stock2 == "")
                    //    stock3 = "";
                    //else
                    //{
                    //    //  conditionStr2 = " AND SUM(INVITM.Quantity) <= " + drfilcountT;
                    //    stock3 = " AND StockRef =" + stock.ToString() + " AND ";
                    //}

                    StrResualt = @"SELECT isnull(SUM(INVITM.Quantity),0)+" + MoneyJari.ToString("F0") + @" 
FROM " + DataMali + @".SLS.Invoice INV INNER JOIN
                      " + DataMali + @".SLS.InvoiceItem INVITM ON INV.InvoiceId = INVITM.InvoiceRef
                    where (INV.State = 1) AND INV.CustomerPartyRef=(SELECT PartyId FROM " + DataMali + @".GNR.Party
WHERE DLRef = (SELECT csT_dl FROM PRG_Customers WHERE cst_id = " + customer.ToString() +
                                 @")) and INV.QuotationRef is not null AND Convert(date,INV.Date)>=@date "; //group by INV.CustomerPartyRef                     " + shart;
                }
            }

            if (number == 2)
            {
                if (ccd_itemtype == "50")
                {

                    // شرط اعتبار سنجی
                    string shart = "";
                    StrResualt = @"SELECT  VL_finalscore
FROM         PRG_ValidateCustomer Where VL_state=1 AND VL_CST_id=" + customer.ToString();
                }

                if (ccd_itemtype == "51")
                {

                    // شرط تعداد چک برگشتی سی ار ام
                    string shart = "";
                    StrResualt = @"SELECT  COUNT(CB_cst_id)
                    FROM         PRG_ChequeBack
                    WHERE   CB_cst_id=" + customer.ToString() +
                                 @" and Convert(date,CB_date)>=@date "; //group by CB_cst_id                    " + shart;
                }

                if (ccd_itemtype == "52")
                {
                    // شرط مبلغ مبلغ چک برگشتی سی ار ام
                    string shart = "";
                    StrResualt = @"SELECT Sum(CB_money)
                    FROM         PRG_ChequeBack
                    WHERE   CB_cst_id=" + customer.ToString() +
                                 @" and Convert(date,CB_date)>=@date "; //group by CB_cst_id                   " + shart;
                }

                if (Int32.Parse(ccd_itemtype) >= 53 & Int32.Parse(ccd_itemtype) <= 66)
                {
                    string yearsStr = "";
                    string yearsStr2 = "";
                    if (ccd_itemtype != "61" & ccd_itemtype != "62" & ccd_itemtype != "57" & ccd_itemtype != "58")
                    {
                        yearsStr = "-5";
                        yearsStr2 = "";
                    }
                    else
                    {
                        yearsStr = "-1";
                        yearsStr2 = " AND FiscalYearRef = @outCount ";
                    }

                    string datefinal = "";
                    string Str1 = "";
                    string StrState = "";
                    if (ccd_itemtype == "53")
                    {
                        Str1 = " Count(*) ";
                        StrState = "16";
                    }

                    if (ccd_itemtype == "54")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "16";
                    }

                    if (ccd_itemtype == "55")
                    {
                        Str1 = " Count(*) ";
                        StrState = "8";
                    }

                    if (ccd_itemtype == "56")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "8";
                    }

                    if (ccd_itemtype == "57")
                    {
                        Str1 = " Count(*) ";
                        StrState = "2";
                    }

                    if (ccd_itemtype == "58")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "2";
                    }

                    if (ccd_itemtype == "59")
                    {
                        Str1 = " Count(*) ";
                        StrState = "4";
                    }

                    if (ccd_itemtype == "60")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "4";
                    }

                    if (ccd_itemtype == "61")
                    {
                        Str1 = " Count(*) ";
                        StrState = "1";
                        datefinal = "  and Type!=4";
                    }

                    if (ccd_itemtype == "62")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "1";
                        datefinal = " and Type!=4";
                    }

                    if (ccd_itemtype == "63")
                    {
                        Str1 = " Count(*) ";
                        StrState = "64";
                        datefinal = " and State!=4";
                    }

                    if (ccd_itemtype == "64")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "64";
                        datefinal = " and State!=4";
                    }

                    //65و66 چک های خرج شده هستند که تاریخ سررسیدشان باید بزرگتر از تاریخ روز باشد و از نوع چک ضمانت هم نباشند
                    // برای بقیههم نباید چک ضمانت باشند
                    if (ccd_itemtype == "65")
                    {
                        Str1 = " Count(*) ";
                        StrState = "32";
                        datefinal = " and Date>getdate() and Type!=4";
                        // datefinal = " and Type!=4 ";
                    }

                    if (ccd_itemtype == "66")
                    {
                        Str1 = " SUM([AmountInBaseCurrency]) ";
                        StrState = "32";
                        datefinal = " and Date>getdate() and Type!=4";
                        // datefinal = " and Type!=4 ";
                    }

                    StrResualt = @" declare @sqlstring nvarchar(MAX)
    declare @datamali nvarchar(500)
    set @datamali = (select TNZ_sharh from dbo.PRG_Propertise where TNZ_name = 'DataMali')
		DECLARE @outCount INT;
                DECLARE @params NVARCHAR(255) = '@Count INT OUTPUT'
    set @sqlstring = 'SELECT @Count =[FiscalYearId]  
                    FROM '+@datamali+'.FMK.[FiscalYear]
                    where Title = SUBSTRING(dbo.mtsh(Getdate()), 1, 4) and Status = 1'
    EXEC sp_executeSQL @sqlstring,@params,  @Count = @outCount OUTPUT;
                SELECT " + Str1 + @" FROM(
              SELECT  ReceiptChq.AmountInBaseCurrency, ReceiptChq.DlRef, ReceiptChq.Date, ReceiptChq.State, ReceiptChq.ReceiptHeaderRef, ReceiptChq.ReceiptChequeId
            ,ReceiptChq.type   FROM  " + DataMali + @".RPA.ReceiptCheque AS ReceiptChq LEFT OUTER JOIN
                    (
                      SELECT Submit2.*, History.STATE hState
                      FROM " + DataMali + @".RPA.vwReceiptChequeBankingItem AS Submit2
                          INNER JOIN
                          (
                              SELECT ROW_NUMBER() OVER(PARTITION BY rh.ReceiptChequeref ORDER BY receiptChequeHistoryId DESC) RN , *
                              FROM " + DataMali + @".rpa.ReceiptChequeHistory RH
                          )History ON History.ReceiptChequeRef = Submit2.ReceiptChequeRef AND History.RN = 1
                              AND History.ReceiptChequeBankingItemRef = Submit2.ReceiptChequeBankingItemId
                  ) AS  Submit ON Submit.ReceiptChequeRef = ReceiptChq.ReceiptChequeId AND Submit.hState = 2)  AS ReceiptCheque
   WHERE((EXISTS(SELECT 1 FROM " + DataMali + @".Rpa.ReceiptHeader H Where H.ReceiptHeaderID = ReceiptHeaderRef " +
                                 yearsStr2 + @")OR exists(select * from " + DataMali + @".rpa.ReceiptChequeHistory HH
                     where Convert(date,Date) >= dateadd(year, " + yearsStr + @",@date2)  and
                           Convert(date,Date) <= @date2  and type in (33, 53, 54) and
                           exists(select * from " + DataMali + @".rpa.ReceiptChequeHistory hhh
                                   where Type in (53, 54) and hhh.ReceiptChequeHistoryID =
                                            (select top 1 ReceiptChequeHistoryID from " + DataMali +
                                 @".rpa.ReceiptChequeHistory HH2
                                             where Convert(date,Date) >= dateadd(year, " + yearsStr +
                                 @",@date2) and hh2.[ReceiptChequeRef] = hhh.[ReceiptChequeRef]
                                             order by ReceiptChequeHistoryId) and
                                          hh.ReceiptChequeRef = hhh.[ReceiptChequeRef]) and
                            [ReceiptChequeId] = hh.ReceiptChequeRef) ) ) and [State] = " + StrState +
                                 @" and DlRef = (SELECT csT_dl FROM PRG_Customers WHERE cst_id = " +
                                 customer.ToString() + @") " +
                                 datefinal;
                }

            }
            //string shart = "";
            //if (state)
            //    shart = "";
            //else
            //{
            //    conditionStr2 = " AND Sum(CB_money) <= " + drfilmoneyT;
            //    shart = " HAVING Sum(CB_money)  " + conditionStr + " " + drfilmoneyF + conditionStr2;
            //}

            return StrResualt;
        }

    }
}