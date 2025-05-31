using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Text;
using System.Globalization;
using System.Collections;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using sigma_api.Classes;

namespace sigma_api.classes
{
    public class clsDB
    {
        private static cls_public cp = new cls_public();
        private static string DataMali;
        private static int reservesanad;
        private static string FiscalYearNowStr = cp.GetDateTime().Year.ToString();
        static List<DataPoint> ListDataPoint = new List<DataPoint>();

        static clsDB() 
        {
            DataMali = clsProperties.GetPropertise("DataMali").ToString();
            reservesanad = clsProperties.GetPropertise("reservesanad").ToInt();
        }
        public clsDB() 
        {
            DataMali = clsProperties.GetPropertise("DataMali").ToString();
            reservesanad = clsProperties.GetPropertise("reservesanad").ToInt();
        }

        public static string InventorySep(string itemid)
        {
            string InventorySepidar = @",(
SELECT
	--ItemID, 
   	TotalQuantity 
--, TotalSecondaryQuantity,
	--UnitTitle,
	--SecondaryUnitTitle
FROM (

SELECT
	I.ItemID, 
	ISNULL(OpeningQuantity,0) + ISNULL(TotalReceiptQuantity,0) - ISNULL(TotalReceiptReturnQuantity,0)
	- ISNULL(TotalDeliveryQuantity,0) + ISNULL(TotalDeliveryReturnQuantity,0) AS TotalQuantity,
	ISNULL(OpeningSecondaryQuantity,0) + ISNULL(TotalReceiptSecondaryQuantity,0) - ISNULL(TotalReceiptReturnSecondaryQuantity,0)
	- ISNULL(TotalDeliverySecondaryQuantity,0) + ISNULL(TotalDeliveryReturnSecondaryQuantity,0) AS TotalSecondaryQuantity,
	U.Title AS UnitTitle,
	U2.Title AS SecondaryUnitTitle
FROM " + DataMali + @".INV.vwItem AS I
	LEFT JOIN " + DataMali + @".INV.Unit AS U ON I.UnitRef = U.UnitID
	LEFT JOIN " + DataMali + @".INV.Unit AS U2 ON I.SecondaryUnitRef = U2.UnitID
    LEFT JOIN
    (
		SELECT
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] = 4 THEN IRI.Quantity ELSE 0 END) AS OpeningQuantity,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] = 4 THEN IRI.SecondaryQuantity ELSE 0 END) AS OpeningSecondaryQuantity,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] = 4 THEN ISNULL(IRI.Price,0) + ISNULL(IRI.TransportPrice,0) ELSE 0 END) AS OpeningPrice,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] <> 4 THEN IRI.Quantity ELSE 0 END) AS TotalReceiptQuantity,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] <> 4 THEN IRI.SecondaryQuantity ELSE 0 END) AS TotalReceiptSecondaryQuantity,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IR.[Type] <> 4 THEN ISNULL(IRI.Price,0) + ISNULL(IRI.TransportPrice,0) ELSE 0 END) AS TotalReceiptPrice,
			SUM(CASE WHEN IRI.IsReturn = 1 THEN IRI.Quantity ELSE 0 END) AS TotalReceiptReturnQuantity,
			SUM(CASE WHEN IRI.IsReturn = 1 THEN IRI.SecondaryQuantity ELSE 0 END) AS TotalReceiptReturnSecondaryQuantity,
			SUM(CASE WHEN IRI.IsReturn = 1 THEN ISNULL(IRI.Price,0) + ISNULL(IRI.TransportPrice,0) ELSE 0 END) AS TotalReceiptReturnPrice,
			SUM(CASE WHEN IRI.IsReturn = 0 AND IRI.Price IS NOT NULL THEN IRI.Quantity ELSE 0 END) AS TotalReceiptWithPriceQuantity,
			SUM(CASE WHEN IRI.IsReturn = 1 AND IRI.Price IS NOT NULL THEN IRI.Quantity ELSE 0 END) AS TotalReceiptReturnWithPriceQuantity,
			IRI.ItemRef
		FROM
			" + DataMali + @".INV.InventoryReceipt AS IR
			INNER JOIN " + DataMali + @".INV.InventoryReceiptItem AS IRI 
			ON IRI.InventoryReceiptRef = IR.InventoryReceiptID
		WHERE
            (IR.FiscalYearRef = 10) AND
            (IR.[InventoryReceiptId] = -1 OR -1 = -1) AND
            (-1 = 1 OR -1 = 2 OR -1 = -1) AND

            (IR.[StockRef] = -1 OR -1 = -1) AND
			(IR.Date >= @FromDate OR @FromDate = -1) AND
			(IR.Date <= @ToDate OR @ToDate = -1 ) AND
			(IRI.TracingRef = -1 OR -1 = -1) AND
			((IR.DelivererDLRef = -1 AND IR.[Type]=1) OR -1 = -1)AND
			((IR.DelivererDLRef = -1 AND IR.[Type]=2) OR -1 = -1) AND
			((IR.DelivererDLRef = -1 AND IR.[Type]=3) OR -1 = -1) AND
			(-1 = -1) AND
			(-1 = -1) AND
            (
			 IRI.WeighingRef=-1 OR -1=-1
            )

		GROUP BY [ItemRef]
	) AS IRI
	ON I.ItemID = IRI.ItemRef
	LEFT JOIN
	(
		SELECT
			SUM(CASE WHEN IDI.IsReturn = 0 THEN IDI.Quantity ELSE 0 END) AS TotalDeliveryQuantity,
			SUM(CASE WHEN IDI.IsReturn = 0 THEN IDI.SecondaryQuantity ELSE 0 END) AS TotalDeliverySecondaryQuantity,
			SUM(CASE WHEN IDI.IsReturn = 0 THEN IDI.Price ELSE 0 END) AS TotalDeliveryPrice,
			SUM(CASE WHEN IDI.IsReturn = 1 THEN IDI.Quantity ELSE 0 END) AS TotalDeliveryReturnQuantity,
			SUM(CASE WHEN IDI.IsReturn = 1 THEN IDI.SecondaryQuantity ELSE 0 END) AS TotalDeliveryReturnSecondaryQuantity,
			SUM(CASE WHEN IDI.IsReturn = 1 THEN IDI.Price ELSE 0 END) AS TotalDeliveryReturnPrice,
			SUM(CASE WHEN IDI.IsReturn = 0 AND IDI.Price IS NOT NULL THEN IDI.Quantity ELSE 0 END) AS TotalDeliveryWithPriceQuantity,
			SUM(CASE WHEN IDI.IsReturn = 1 AND IDI.Price IS NOT NULL THEN IDI.Quantity ELSE 0 END) AS TotalDeliveryReturnWithPriceQuantity,
			IDI.ItemRef
		FROM
			" + DataMali + @".INV.InventoryDelivery AS ID
			INNER JOIN " + DataMali + @".INV.InventoryDeliveryItem AS IDI
			ON (ID.InventoryDeliveryID = IDI.InventoryDeliveryRef)
		WHERE
            (ID.FiscalYearRef = 10) AND			

            (ID.[InventoryDeliveryId] = -1 OR -1 = -1) AND
            (-1 = 3 OR -1 = 4 OR -1 = -1) AND

            (ID.[StockRef] = -1 OR -1 = -1) AND
			(ID.Date >= @FromDate OR @FromDate = -1) AND
			(ID.Date <= @ToDate OR @ToDate = -1 ) AND
			(IDI.TracingRef = -1 OR -1 = -1) AND
			((ID.ReceiverDLRef = -1 AND ID.[Type]=1) OR -1 = -1) AND
			((ID.ReceiverDLRef = -1 AND ID.[Type]=2) OR -1 = -1 ) AND
			((ID.ReceiverDLRef = -1 AND ID.[Type]=3) OR -1 = -1) AND
			(-1 = -1) AND
			(-1 = -1) AND
			(
			 IDI.WeighingRef=-1 OR -1=-1
            )
		GROUP BY [ItemRef]
	) IDI
	ON I.[ItemID] = IDI.[ItemRef]
	LEFT JOIN
    (
		SELECT
			SUM(CASE WHEN IW.IsPrimaryUnit = 1 THEN IW.PureQuantity ELSE 0 END) AS WeighingInputQuantity,
			SUM(CASE WHEN IW.IsPrimaryUnit = 0 THEN IW.PureQuantity ELSE 0 END) AS WeighingInputSecondaryQuantity,
			IW.ItemRef
		FROM
			" + DataMali + @".INV.vwInventoryWeighing AS IW
		WHERE
		    (IW.InventoryWeighingID=-1 OR -1=-1) AND
			(IW.Date >= @FromDate OR @FromDate = -1) AND
			(IW.Date <= @ToDate OR @ToDate = -1 ) AND
			(IW.VendorDLRef = -1 OR -1 = -1)AND
			(IW.VendorDLRef = -1  OR -1 = -1) AND
			(IW.VendorDLRef = -1  OR -1 = -1) AND
			(IW.Type=1) 
			
		GROUP BY [ItemRef]
	) AS IWInput
	ON I.ItemID = IWInput.ItemRef
	LEFT JOIN
    (
		SELECT
			SUM(CASE WHEN IW.IsPrimaryUnit = 1 THEN IW.PureQuantity ELSE 0 END) AS WeighingOutputQuantity,
			SUM(CASE WHEN IW.IsPrimaryUnit = 0 THEN IW.PureQuantity ELSE 0 END) AS WeighingOutputSecondaryQuantity,
			IW.ItemRef
		FROM
			" + DataMali + @".INV.vwInventoryWeighing AS IW
		WHERE
		    (IW.InventoryWeighingID=-1 OR -1=-1) AND
			(IW.Date >= @FromDate OR @FromDate = -1) AND
			(IW.Date <= @ToDate OR @ToDate = -1 ) AND
			(IW.VendorDLRef = -1 OR -1 = -1)AND
			(IW.VendorDLRef = -1  OR -1 = -1) AND
			(IW.VendorDLRef = -1  OR -1 = -1) AND
			(IW.Type=2) 

		GROUP BY [ItemRef]
	) AS IWOutput
	ON I.ItemID = IWOutput.ItemRef
WHERE
-- كالاهايي كه گردشي دارند؛ خواه انباري انتخاب شده است، خواه نشده است
	(
		((OpeningQuantity <> 0 ) OR
 		(TotalReceiptQuantity <> 0 ) OR
 		(TotalReceiptReturnQuantity <> 0 ) OR
 		(TotalDeliveryQuantity <> 0) OR
 		(TotalDeliveryReturnQuantity <> 0) OR
		((-1=-1 AND -1=-1 AND -1=-1 ) AND
		((WeighingInputQuantity <> 0) OR 
		(WeighingInputSecondaryQuantity <> 0) OR
		(WeighingOutputQuantity <> 0) OR 
		(WeighingOutputSecondaryQuantity <> 0)))
		) AND -1 = -1
	) OR
	(
			-1 <> -1 AND
		(EXISTS
			(SELECT 1 FROM " + DataMali + @".INV.InventoryPurchaseInvoice IP
			 INNER JOIN " + DataMali + @".INV.InventoryPurchaseInvoiceItem IPI
			 ON IP.InventoryPurchaseInvoiceID = IPI.InventoryPurchaseInvoiceRef
			 WHERE IP.InventoryPurchaseInvoiceID = -1 AND
				IPI.ItemRef = I.ItemID))
  OR (EXISTS (SELECT 1 FROM " + DataMali + @".POM.PurchaseInvoice pi
  INNER JOIN " + DataMali + @".POM.PurchaseInvoiceItem pii ON pi.PurchaseInvoiceID = pii.PurchaseInvoiceRef
  WHERE pi.PurchaseInvoiceID = -1  AND pii.ItemRef = I.ItemID))
	)
) AS ItemSheet
where ItemID=" + itemid + ") AS InventoryStockSep";
            return InventorySepidar;
        }

        public static string listletterFull =
            @"SELECT     AUTO_Letter.LTT_id, CASE AUTO_Letter.LTT_type WHEN 2 THEN 'صادره' WHEN 1 THEN 'داخلي' WHEN 3 THEN 'وارده' END AS LTTtype, 
                      CASE AUTO_Letter.LTT_vaziat WHEN 0 THEN 'ثبت شده' WHEN 1 THEN 'ثبت انديکاتور شده' WHEN 2 THEN 'امضا شده' WHEN 3 THEN 'ابطال شده' END AS LTTvaziat, dbo.mtsh(CONVERT(date, 
                      AUTO_Letter.LTT_datecreate)) AS LTTdatecreate, PRG_Users.Name AS creator, AUTO_Letter.LTT_baseletterno, dbo.mtsh(CONVERT(date, AUTO_Letter.LTT_baseletterdate)) AS LTTbaseletterdate, 
                     CASE AUTO_Letter.LTT_type WHEN 2 THEN ISNULL(PRG_Customers.cst_fname, '') + ' ' + ISNULL(PRG_Customers.cst_laname, '')
                      WHEN 1 THEN PRG_Users_2.Name WHEN 3 THEN CASE LTT_Girande WHEN 0 THEN (SELECT TNZ_sharh FROM dbo.PRG_Propertise WHERE TNZ_name = 'txtnameforoshgah') ELSE PRG_Users_2.Name END END AS girande,
                       AUTO_Letter.LTT_desc, 
                      CASE AUTO_Letter.LTT_typelttr WHEN 0 THEN 'الگوي متن' WHEN 1 THEN 'پرونده' END AS LTTtypelttr, AUTO_Letter.LTT_Keyword, AUTO_Letter.LTT_letterno, dbo.mtsh(CONVERT(date, 
                      AUTO_Letter.LTT_letterdate)) AS LTTletterdate, AUTO_Letter.LTT_Tozih, dbo.mtsh(CONVERT(date, AUTO_Letter.LTT_signdate)) AS LTT_signdate, AUTO_Letter.LTT_vaziat, AUTO_Letter.LTT_type, 
                      AUTO_Letter.LTT_creator, PRG_Users_1.Name AS signer, 
                      CASE AUTO_Letter.LTT_type WHEN 2 THEN ISNULL(PRG_Customers_1.cst_fname, '') + ' ' + ISNULL(PRG_Customers_1.cst_laname, '')
                      WHEN 1 THEN PRG_Users_3.Name WHEN 3 THEN PRG_Users_3.Name  END AS roneshtG, 
                      CONVERT(VARCHAR(8), AUTO_Letter.LTT_datecreate, 108) AS times, CONVERT(VARCHAR(8), AUTO_Letter.LTT_signdate, 108) AS timessign, AUTO_Letter.LTT_Girande, AUTO_Letter.LTT_textname,
                          (SELECT     COUNT(*) AS Expr1
                            FROM          PRG_Documents
                            WHERE      (DOC_Lttr_id = AUTO_Letter.LTT_id)) AS attachment, AUTO_Letter.LTT_typelttr
FROM         AUTO_Letter INNER JOIN
                      PRG_Users ON AUTO_Letter.LTT_creator = PRG_Users.USR_id LEFT OUTER JOIN
                      PRG_Customers ON AUTO_Letter.LTT_Girande = PRG_Customers.cst_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_2 ON AUTO_Letter.LTT_Girande = PRG_Users_2.USR_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_3 ON AUTO_Letter.LTT_Ronevesht = PRG_Users_3.USR_id LEFT OUTER JOIN
                      PRG_Customers AS PRG_Customers_1 ON AUTO_Letter.LTT_Ronevesht = PRG_Customers_1.cst_id LEFT OUTER JOIN
                      PRG_Users AS PRG_Users_1 ON AUTO_Letter.LTT_signer = PRG_Users_1.USR_id";

        private static string currentrowReportPoya;

        public static decimal PricingSepidar(string itm, string saletype)
        {
            decimal PriceSep = 0;
            try
            {
                //string PricingTax = "";
                //if (FrmMain.ShowPricingTax)
                //    PricingTax = " Fee + ((Fee * (select ITM_tax from PRG_Item where ITM_Item_id=ItemRef)))/100";
                //else
                //    PricingTax = " Fee ";
                DataSet _DS = new DataSet();
                SqlDataAdapter _DA = new SqlDataAdapter();
                DataTable dt = cp.select_command(@"SELECT Fee FROM " + DataMali + @".SLS.[vwPriceNoteItem]   
   WHERE (([SaleTypeRef] = (select SaleTypeId from " + DataMali + ".SLS.SaleType where Title=REPLACE(REPLACE(CAST('" +
                                                     saletype +
                                                     @"' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) 
or Title=REPLACE(REPLACE(CAST('" + saletype +
                                                     "' AS nvarchar(MAX)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603))) ) AND ([CurrencyRef] = 1) AND ((( 1=2) OR ([TracingRef] IS NULL)) AND ([ItemRef] =" +
                                                     itm +
                                                     ")) AND (([CustomerGroupingRef] IS NULL)) )  ORDER BY [TracingRef] DESC");

                try
                {
                    PriceSep = decimal.Parse(dt.Rows[0][0].ToString());
                }
                catch
                {
                    PriceSep = 0;
                }
            }
            catch (Exception ex)
            {
                PriceSep = 0;
            }

            return PriceSep;
        }

        public static string saletype;

        public static string Stringreserved(string FiscalYearId)
        {
            string reserverd = "";
            if (clsProperties.GetPropertise("reservesanad").ToInt() == 0) //رزرو ندارد
            {
                reserverd = "ISNULL((select cast(Quantity  as decimal) Quantity  from  " + DataMali +
                            @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
ItemRef=itm.ITM_Item_id and UnitRef=itm.ITM_unit and FiscalYearRef=" + FiscalYearId + @"),0) InventoryStockSummary,
0 as InventoryStockInv, 
  0 as InventoryStock0
, ISNULL((select cast(cast(SUM(INS_Inside) as decimal(19,4))-cast(SUM(INS_Out) as decimal(19,4)) as float) from dbo.PRG_InventoryStock 
			where INS_Item=itm.ITM_id  and INS_Stock=istk.IS_stock  AND INS_state=1 and TNS_TypeStock!=1  AND TNS_fy_id=" +
                            FiscalYearId + @"),0) as InventoryStock1
,  NULL as InventoryStockReserved1,
NULL as InventoryStockReserved0";
            }

            if (clsProperties.GetPropertise("reservesanad").ToInt() == 1) //رزرو درخواست
            {
                reserverd = "ISNULL((select cast(cast(Quantity  as decimal) as float) Quantity  from  " + DataMali +
                            @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
ItemRef=itm.ITM_Item_id and UnitRef=itm.ITM_unit and FiscalYearRef=" + FiscalYearId + @"),0) InventoryStockSummary,
ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)),4) as float) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" +
                            FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0) InventoryStockInv, 
ISNULL((select cast(round(cast(SUM(IDI.Quantity) as decimal(19,4)),4) as float) from dbo.PRG_InventoryStock  INNER JOIN
PRG_RequestBuyItem RBI on RBI.RBI_id=INS_RequestBuyItem INNER JOIN
dbo.PRG_QuotationItem QUI on RBI.RBI_id=QUI.QUI_RBI_id INNER JOIN
dbo.PRG_Quotation QU on QU.QU_id=QUI.QUI_QU_id 
INNER JOIN " + DataMali + @".SLS.Quotation Q on Q.QuotationId=QU.QU_QuotationIdSP join  PRG_Item on ITM_id=INS_Item
INNER JOIN " + DataMali +
                            @".SLS.QuotationItem QI ON Q.QuotationId=QI.QuotationRef and QI.ItemRef=PRG_Item.ITM_Item_id and QI.StockRef=istk.IS_stock
 INNER JOIN " + DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID
 INNER JOIN " + DataMali + @".INV.InventoryDeliveryItem IDI ON IDI.BaseInvoiceItem = II.InvoiceItemID 
where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId +
                            @" and TNS_TypeStock=1 AND INS_state = 1),0)  as InventoryStock0

, ISNULL((select cast(cast(SUM(INS_Inside) as decimal(19,4))-cast(SUM(INS_Out) as decimal(19,4)) as float) from dbo.PRG_InventoryStock 
			where INS_Item=itm.ITM_id  and INS_Stock=istk.IS_stock  AND INS_state=1 and TNS_TypeStock!=1  AND TNS_fy_id=" +
                            FiscalYearId + @"),0) as InventoryStock1
,  ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)), 4) as float) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
			istk.IS_stock AND TNS_fy_id = " + FiscalYearId +
                            @" and TNS_TypeStock != 1 AND INS_state = 1 and INS_Type=0),0)  as InventoryStockReserved1,
ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)), 4) as float) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
			istk.IS_stock AND TNS_fy_id = " + FiscalYearId +
                            @" and TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0) as InventoryStockReserved0";

            }

            if (clsProperties.GetPropertise("reservesanad").ToInt() == 2 || clsProperties.GetPropertise("reservesanad").ToInt() == 3) //رزرو پیش فاکتور
            {
                reserverd = "ISNULL((select cast(Quantity  as decimal) Quantity  from  " + DataMali +
                            @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
ItemRef=itm.ITM_Item_id and UnitRef=itm.ITM_unit and FiscalYearRef=" + FiscalYearId + @"),0) InventoryStockSummary,
ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)),4) as float) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" +
                            FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0) InventoryStockInv, 
ISNULL((select cast(round(cast(SUM(IDI.Quantity) as decimal(19,4)),4) as float) from dbo.PRG_InventoryStock  
INNER JOIN dbo.PRG_QuotationItem QUI on INS_QuotationItem=QUI.QUI_id 
INNER JOIN dbo.PRG_Quotation QU on QU.QU_id=QUI.QUI_QU_id 
INNER JOIN " + DataMali + @".SLS.Quotation Q on Q.QuotationId=QU.QU_QuotationIdSP join  PRG_Item on ITM_id=INS_Item
INNER JOIN " + DataMali +
                            @".SLS.QuotationItem QI ON Q.QuotationId=QI.QuotationRef and QI.ItemRef=PRG_Item.ITM_Item_id and QI.StockRef=istk.IS_stock
 INNER JOIN " + DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID
 INNER JOIN " + DataMali + @".INV.InventoryDeliveryItem IDI ON IDI.BaseInvoiceItem = II.InvoiceItemID 
where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId +
                            @" and TNS_TypeStock=1 AND INS_state = 1),0)  as InventoryStock0
, ISNULL((select cast(cast(SUM(INS_Inside) as decimal(19,4))-cast(SUM(INS_Out) as decimal(19,4)) as float) from dbo.PRG_InventoryStock 
			where INS_Item=itm.ITM_id  and INS_Stock=istk.IS_stock  AND INS_state=1 and TNS_TypeStock!=1  AND TNS_fy_id=" +
                            FiscalYearId + @"),0) as InventoryStock1
,  ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)), 4) as float) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
			istk.IS_stock AND TNS_fy_id = " + FiscalYearId +
                            @" and TNS_TypeStock != 1 AND INS_state = 1 and INS_Type=0),0)  as InventoryStockReserved1,
ISNULL((select cast(round(cast(SUM(INS_Out) as decimal(19,4)), 4) as float) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
			istk.IS_stock AND TNS_fy_id = " + FiscalYearId +
                            @" and TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0) as InventoryStockReserved0";
            }

            return reserverd;
        }


        public static String GetTimestamp(System.DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }

        public static string StringSmsReaualt = "";
        public static string returnCode = "";
        public static bool checkreturncode = false;
        public static DataTable DtSendSmsClass;
        public static int i2 = 0;
        public static bool checkrefresh = false;
        public static bool checkrefreshTypee = false;

        public static int checkrefreshID = 0;

        //static string computerName = System.Windows.Forms.SystemInformation.ComputerName.ToString();
        static DataTable dt, dtda, dttest, dtkbd;

        public static string SafeArabiStr(string input)
        {
            return input.Replace("ی", "ي").Replace("ک", "ك");
        }

        // تبديل اعداد انگليسي به فارسي
        public static string GetPersianNumber(string englishNumber)
        {
            string persianNumber = "";
            foreach (char ch in englishNumber)
            {
                persianNumber += (char)(1776 + char.GetNumericValue(ch));
            }

            return persianNumber;
        }

        public static string SafeFarsiStr(string input)
        {
            return input.Replace("ي", "ی").Replace("ك", "ک");
        }

        public static string FiscalYear(System.DateTime DTNOW)
        {

            SqlParameter[] parameters = new SqlParameter[]
            {
                new SqlParameter("@date", DTNOW)
            };
            return cp.select_command(@"(select [FiscalYearId] FROM " + DataMali +
                                     @".FMK.[FiscalYear] WHERE Status=1 AND 
              title=SUBSTRING(dbo.mtsh(convert(date, @date)),1,4))
", parameters).Rows[0][0].ToString();

        }

        public static string formname = "";

        public static float InventoryStock(int stockid)
        {

            DataTable dt =
                cp.select_command(
                    @"select SUM(INS_Inside)-SUM(INS_Out)- @RBI_metraj from dbo.PRG_InventoryStock where INS_Item=" +
                    stockid.ToString());

            return float.Parse(dt.Rows[0][0].ToString());
        }

        public static string SelectItem(string tt, string FiscalYearId, string Filter, string cst = null)
        {
            string PricingTax = ",NULL ";
            string PricingQ = ",NULL ";
            if (string.IsNullOrEmpty(saletype))
                saletype = "0";
            if (clsProperties.GetPropertise("PricingQ").ToBoolean() & !string.IsNullOrEmpty(saletype))
            {
                //if (FrmMain.ShowPricingTax)
                //    PricingTax = @",1250.2583";
                //else
                //    PricingTax = " ,0 ";


                //if (FrmMain.ShowPricingTax)
                //                    PricingTax = @",(SELECT CAST(( Fee+ ((Fee * itm.ITM_tax))/100)AS DECIMAL(19,0))  FROM " + FrmMain.DataMali + @".SLS.[vwPriceNoteItem]   
                //   WHERE (([SaleTypeRef] = (select SaleTypeId from " + FrmMain.DataMali + ".SLS.SaleType where Title=REPLACE(REPLACE(CAST('" + saletype + @"' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) 
                //or Title=REPLACE(REPLACE(CAST('" + saletype + "' AS nvarchar(MAX)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603))) ) AND ([CurrencyRef] = 1) AND ((( 1=2) OR ([TracingRef] IS NULL)) AND ([ItemRef] =itm.ITM_Item_id)) AND (([CustomerGroupingRef] IS NULL)) )) ";
                //                else
                //                    PricingTax = " ,0 ";
                PricingTax = ",(SELECT CAST(( Fee+ ((Fee * itm.ITM_tax)/100))AS DECIMAL(19,0)) FROM " + DataMali +
                             @".SLS.[vwPriceNoteItem] " +
                             "WHERE (([SaleTypeRef] = (select SaleTypeId from " + DataMali +
                             ".SLS.SaleType where Title=REPLACE(REPLACE(CAST('" + saletype +
                             @"' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) or 
Title=REPLACE(REPLACE(CAST('" + saletype +
                             "' AS nvarchar(MAX)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603))) ) AND ([CurrencyRef] = 1) AND ((( 1=2) OR ([TracingRef] IS NULL)) AND ([ItemRef] =itm.ITM_Item_id)) AND (([CustomerGroupingRef] IS NULL)) ))";
                PricingQ = @",(SELECT [Fee] FROM " + DataMali + @".SLS.[vwPriceNoteItem]   
   WHERE (([SaleTypeRef] = (select SaleTypeId from " + DataMali + ".SLS.SaleType where Title=REPLACE(REPLACE(CAST('" +
                           saletype + @"' AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) 
or Title=REPLACE(REPLACE(CAST('" + saletype +
                           "' AS nvarchar(MAX)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603))) ) AND ([CurrencyRef] = 1) AND ((( 1=2) OR ([TracingRef] IS NULL)) AND ([ItemRef] =itm.ITM_Item_id)) AND (([CustomerGroupingRef] IS NULL)) )) ";
                saletype = "";
            }

            saletype = "";
            string TxtShowAdition1_1 = "";
            string TxtShowAdition1_2 = "";
            //if (FrmMain.ShowAdition1)
            //{
            TxtShowAdition1_1 = ",lkp1.LKP_name TitleItemAdition1,lkp1.LKP_value ValueAddition1 ";
            TxtShowAdition1_2 = " LEFT JOIN dbo.PRG_Loockup lkp1 on lkp1.LKP_id=itm.ITM_ItemAdition1 ";
            //}
            string TxtShowAdition2_1 = "";
            string TxtShowAdition2_2 = "";
            //if (FrmMain.ShowAdition2)
            //{
            TxtShowAdition2_1 = ",lkp2.LKP_name TitleItemAdition2,lkp1.LKP_value ValueAddition2 ";
            TxtShowAdition2_2 = " LEFT JOIN dbo.PRG_Loockup lkp2 on lkp2.LKP_id=itm.ITM_ItemAdition2 ";
            //}
            string TxtShowPurchaseItem_1 = "";
            string TxtShowPurchaseItem_2 = "";
            //if (FrmMain.ShowPurchaseItem)
            //{
            TxtShowPurchaseItem_1 =
                ",REPLACE(REPLACE(CAST(PurchaseGroup.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) TitlePurchaseGroup ";
            TxtShowPurchaseItem_2 = "  LEFT JOIN " + DataMali +
                                    @".GNR.Grouping PurchaseGroup ON itm.ITM_PurchaseGroup = PurchaseGroup.GroupingID ";
            //}
            string TxtShowSaleItem_1 = "";
            string TxtShowSaleItem_2 = "";
            //if (FrmMain.ShowSaleItem)
            //{
            TxtShowSaleItem_1 =
                ",REPLACE(REPLACE(CAST(SaleGroup.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) TitleSaleGroup ";
            TxtShowSaleItem_2 = "  LEFT JOIN " + DataMali +
                                @".GNR.Grouping SaleGroup ON itm.ITM_SaleGroup = SaleGroup.GroupingID ";
            //}

            string defualt = "";
            if (clsProperties.GetPropertise("ShowtemDefualt").ToBoolean())
            {
                defualt = " and istk.IS_Default = 1 ";
            }

            string reserverd = Stringreserved(FiscalYearId);
            string Resualt = @"select *,
case when ST_Inventorycontrol= 0 then (InventoryStockSummary-InventoryStockInv+InventoryStock0) else InventoryStock1 end as InventoryStock,
case when ST_Inventorycontrol= 0 then InventoryStockReserved0 else InventoryStockReserved1 end as InventoryStockReserved
from (SELECT   istk.IS_id  , CAST('false' AS bit) AS [...], Stocksep.Title stocktitle,itm.ITM_Code,  REPLACE(REPLACE(CAST(itm.ITM_fullname AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705))  AS ITM_fullname, 
                      REPLACE(REPLACE(CAST(unitsep.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS Title, itm.ITM_Item_id,  
                      PRG_Loockup_3.LKP_id GroupItemid" + TxtShowAdition1_1 + TxtShowAdition2_1 +
                             TxtShowPurchaseItem_1 + TxtShowSaleItem_1 + @"
" + PricingQ + @"as PricingQ " + PricingTax + @" as PricingTax,
" + reserverd +
                             @" ,itm.ITM_nesabt, itm.ITM_nesbatValue, unit2.Title AS TitleUnit2, itm.ITM_unit2,Stocksep.StockID ,itm.ITM_Supplier, isnull(PRG_Customers.cst_fname,'')+' '+isnull( 
                      PRG_Customers.cst_laname,'') Supplier,itm.ITM_Unit,stkcrm.ST_Inventorycontrol,itm.ITM_id,IS_GroupStock,GRS_GroupName,CAST(NULL AS decimal) AS FeeList,CAST(NULL AS decimal) AS PRC_FeeSell,ITM_duty,ITM_tax,istk.IS_Inventorycontrol,isnull (cast(cast(itm.ITM_Pointorderitem as decimal) as nvarchar(20)) ,0) ITM_Pointorderitem,ITM_Note
,(select LKP_id from [dbo].[PRG_ItemParameter]
join PRG_Loockup on LKP_id=[IP_lkp_id] where  LKP_enownerName in 
(select LKP_enownerName from PRG_Loockup where cast(LKP_id as nvarchar(10))=(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='cmd1Packing'))
and [IP_itm_id]=itm.ITM_id ) AS Id1Packing  ,
(select LKP_id from [dbo].[PRG_ItemParameter]
join PRG_Loockup on LKP_id=[IP_lkp_id] where  LKP_enownerName in 
(select LKP_enownerName from PRG_Loockup where cast(LKP_id as nvarchar(10))=(select cast(TNZ_sharh as nvarchar(10)) from PRG_Propertise where TNZ_name='cmd2Packing'))
and [IP_itm_id]=itm.ITM_id ) AS Id2Packing
 FROM PRG_Item itm  INNER JOIN PRG_ItemStock istk
on istk.IS_itm_id= itm.ITM_id  " + defualt + @" LEFT OUTER JOIN
            dbo.PRG_GroupStock on GRS_id=istk.IS_GroupStock 
 INNER JOIN
" + DataMali + @".INV.Stock Stocksep ON istk.IS_stock = Stocksep.StockID INNER JOIN 
" + DataMali + @".INV.Unit unitsep ON itm.ITM_Unit = unitsep.UnitID LEFT OUTER JOIN
                      " + DataMali + @".INV.Unit AS unit2 ON itm.ITM_unit2 = unit2.UnitID 
left JOIN PRG_Loockup AS PRG_Loockup_2 ON itm.ITM_Type = PRG_Loockup_2.LKP_id
left JOIN PRG_Loockup AS PRG_Loockup_3 ON itm.ITM_GroupItem = PRG_Loockup_3.LKP_id Left JOIN
                      PRG_Customers ON itm.ITM_Supplier = PRG_Customers.cst_id INNER JOIN
PRG_Stock stkcrm on stkcrm.ST_stockid=Stocksep.StockID" + TxtShowAdition1_2 + TxtShowAdition2_2 +
                             TxtShowPurchaseItem_2 + TxtShowSaleItem_2 + @"
WHERE (itm.ITM_TypeActive = 1) and Stocksep.IsActive=1 AND Stocksep.StockID not in (select [ST_stockid] from [dbo].[PRG_Stock] where [ST_Showitem]=0) " +
                             tt + " " + Filter + ") ListItems order by InventoryStock desc";
            return Resualt;
            //            string Resualt = @"
            //select * from (SELECT     itm.ITM_id, CAST('false' AS bit) AS [...], Stocksep.Title stocktitle,itm.ITM_Code, REPLACE(REPLACE(CAST(itm.ITM_fullname AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS ITM_fullname, 
            //                      REPLACE(REPLACE(CAST(unitsep.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS Title, itm.ITM_Item_id,  
            //                      PRG_Loockup_3.LKP_id GroupItemid" + TxtShowAdition1_1 + TxtShowAdition2_1 + TxtShowPurchaseItem_1 + TxtShowSaleItem_1 + @"
            //" + PricingQ + @",
            //case when stkcrm.ST_Inventorycon trol= 0 then 
            //(select cast(Quantity  as float) Quantity  from  " + FrmMain.DataMali + @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
            //ItemRef=itm.ITM_Item_id and UnitRef=itm.ITM_unit and FiscalYearRef=" + FiscalYearId + @")-
            //ISNULL((select SUM(INS_Out) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //+ISNULL((select SUM(IDI.Quantity) from dbo.PRG_InventoryStock INNER JOIN
            //dbo.PRG_Lading on INS_Lading=LD_id INNER JOIN
            //dbo.PRG_Quotation on LD_Qu_id=dbo.PRG_Quotation.Qu_id INNER JOIN
            //" + FrmMain.DataMali + @".SLS.QuotationItem QI ON QU_QuotationIdSP=QI.QuotationRef INNER JOIN
            //" + FrmMain.DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID INNER JOIN
            //" + FrmMain.DataMali + @".INV.InventoryDeliveryItem IDI ON  IDI.BaseInvoiceItem = II.InvoiceItemID INNER JOIN
            //" + FrmMain.DataMali + @".INV.InventoryDelivery ID ON IDI.InventoryDeliveryRef = ID.InventoryDeliveryID
            //where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //else (select SUM(INS_Inside)-SUM(INS_Out) from dbo.PRG_InventoryStock 
            //			where INS_Item=itm.ITM_id  and INS_Stock=istk.IS_stock  AND INS_state=1 and TNS_TypeStock!=1  AND TNS_fy_id=" + FiscalYearId + @") end as InventoryStock,
            //case when stkcrm.ST_Inventorycontrol= 0 then (select ISNULL((select round(SUM(INS_Out), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
            //			istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0) )
            //else (select ISNULL((select round(SUM(INS_Out), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
            //			istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and TNS_TypeStock != 1 AND INS_state = 1 and INS_Type=0),0) ) end as InventoryStockReserved,

            //case when stkcrm.ST_Inventorycontrol= 0 then 
            //(select round(((select cast(Quantity  as float) Quantity from  " + FrmMain.DataMali + @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
            //				ItemRef = itm.ITM_Item_id and UnitRef = itm.ITM_unit2 and FiscalYearRef = " + FiscalYearId + @")-
            //				ISNULL((select SUM(INS_Out2) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and
            //				 TNS_TypeStock=1 AND INS_state = 1),0)
            //				 +
            //				ISNULL((select SUM(IDI.SecondaryQuantity) from dbo.PRG_InventoryStock INNER JOIN
            //				dbo.PRG_Lading on INS_Lading=LD_id INNER JOIN
            //				dbo.PRG_Quotation on LD_Qu_id=dbo.PRG_Quotation.Qu_id INNER JOIN
            //				" + FrmMain.DataMali + @".SLS.QuotationItem QI ON QU_QuotationIdSP=QI.QuotationRef INNER JOIN
            //				" + FrmMain.DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID INNER JOIN
            //				" + FrmMain.DataMali + @".INV.InventoryDeliveryItem IDI ON  IDI.BaseInvoiceItem = II.InvoiceItemID INNER JOIN
            //				" + FrmMain.DataMali + @".INV.InventoryDelivery ID ON IDI.InventoryDeliveryRef = ID.InventoryDeliveryID
            //				where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //				),4))
            //else null end as InventoryStockSepSecondaryUnit,
            //case when stkcrm.ST_Inventorycontrol= 0 then (select ISNULL((select round(SUM(INS_Out2), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock = istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and
            //		    TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0)) else null end as InventoryStockSepSecondaryUnitRes,
            //itm.ITM_nesabt, itm.ITM_nesbatValue, unit2.Title AS TitleUnit2, itm.ITM_unit2,Stocksep.StockID ,itm.ITM_Supplier, isnull(PRG_Customers.cst_fname,'')+' '+isnull( 
            //                      PRG_Customers.cst_laname,'') Supplier,itm.ITM_Unit,stkcrm.ST_Inventorycontrol,istk.IS_id,IS_GroupStock,GRS_GroupName,CAST(NULL AS decimal) AS FeeList,CAST(NULL AS decimal) AS PRC_FeeSell,ITM_duty,ITM_tax
            //FROM PRG_Item itm  INNER JOIN PRG_ItemStock istk
            //on istk.IS_itm_id= itm.ITM_id  " + defualt + @" LEFT OUTER JOIN
            //            dbo.PRG_GroupStock on GRS_id=istk.IS_GroupStock 
            // INNER JOIN
            //" + FrmMain.DataMali + @".INV.Stock Stocksep ON istk.IS_stock = Stocksep.StockID INNER JOIN 
            //" + FrmMain.DataMali + @".INV.Unit unitsep ON itm.ITM_Unit = unitsep.UnitID LEFT OUTER JOIN
            //                      " + FrmMain.DataMali + @".INV.Unit AS unit2 ON itm.ITM_unit2 = unit2.UnitID 
            //left JOIN PRG_Loockup AS PRG_Loockup_2 ON itm.ITM_Type = PRG_Loockup_2.LKP_id
            //left JOIN PRG_Loockup AS PRG_Loockup_3 ON itm.ITM_GroupItem = PRG_Loockup_3.LKP_id Left JOIN
            //                      PRG_Customers ON itm.ITM_Supplier = PRG_Customers.cst_id INNER JOIN
            //PRG_Stock stkcrm on stkcrm.ST_stockid=Stocksep.StockID" + TxtShowAdition1_2 + TxtShowAdition2_2 + TxtShowPurchaseItem_2 + TxtShowSaleItem_2 + @"
            //WHERE (itm.ITM_TypeActive = 1) " + tt + ") ListItems order by ListItems.InventoryStock desc";
            //            return Resualt; string Resualt = @"
            //select * from (SELECT     itm.ITM_id, CAST('false' AS bit) AS [...], Stocksep.Title stocktitle,itm.ITM_Code, REPLACE(REPLACE(CAST(itm.ITM_fullname AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS ITM_fullname, 
            //                      REPLACE(REPLACE(CAST(unitsep.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS Title, itm.ITM_Item_id,  
            //                      PRG_Loockup_3.LKP_id GroupItemid" + TxtShowAdition1_1 + TxtShowAdition2_1 + TxtShowPurchaseItem_1 + TxtShowSaleItem_1 + @"
            //" + PricingQ + @",
            //case when stkcrm.ST_Inventorycontrol= 0 then 
            //(select cast(Quantity  as float) Quantity  from  " + FrmMain.DataMali + @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
            //ItemRef=itm.ITM_Item_id and UnitRef=itm.ITM_unit and FiscalYearRef=" + FiscalYearId + @")-
            //ISNULL((select SUM(INS_Out) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //+ISNULL((select SUM(IDI.Quantity) from dbo.PRG_InventoryStock INNER JOIN
            //dbo.PRG_Lading on INS_Lading=LD_id INNER JOIN
            //dbo.PRG_Quotation on LD_Qu_id=dbo.PRG_Quotation.Qu_id INNER JOIN
            //" + FrmMain.DataMali + @".SLS.QuotationItem QI ON QU_QuotationIdSP=QI.QuotationRef INNER JOIN
            //" + FrmMain.DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID INNER JOIN
            //" + FrmMain.DataMali + @".INV.InventoryDeliveryItem IDI ON  IDI.BaseInvoiceItem = II.InvoiceItemID INNER JOIN
            //" + FrmMain.DataMali + @".INV.InventoryDelivery ID ON IDI.InventoryDeliveryRef = ID.InventoryDeliveryID
            //where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //else (select SUM(INS_Inside)-SUM(INS_Out) from dbo.PRG_InventoryStock 
            //			where INS_Item=itm.ITM_id  and INS_Stock=istk.IS_stock  AND INS_state=1 and TNS_TypeStock!=1  AND TNS_fy_id=" + FiscalYearId + @") end as InventoryStock,
            //case when stkcrm.ST_Inventorycontrol= 0 then (select ISNULL((select round(SUM(INS_Out), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
            //			istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0) )
            //else (select ISNULL((select round(SUM(INS_Out), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock =
            //			istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and TNS_TypeStock != 1 AND INS_state = 1 and INS_Type=0),0) ) end as InventoryStockReserved,

            //case when stkcrm.ST_Inventorycontrol= 0 then 
            //(select round(((select cast(Quantity  as float) Quantity from  " + FrmMain.DataMali + @".INV.ItemStockSummary where StockRef=istk.IS_stock and 
            //				ItemRef = itm.ITM_Item_id and UnitRef = itm.ITM_unit2 and FiscalYearRef = " + FiscalYearId + @")-
            //				ISNULL((select SUM(INS_Out2) from dbo.PRG_InventoryStock where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and
            //				 TNS_TypeStock=1 AND INS_state = 1),0)
            //				 +
            //				ISNULL((select SUM(IDI.SecondaryQuantity) from dbo.PRG_InventoryStock INNER JOIN
            //				dbo.PRG_Lading on INS_Lading=LD_id INNER JOIN
            //				dbo.PRG_Quotation on LD_Qu_id=dbo.PRG_Quotation.Qu_id INNER JOIN
            //				" + FrmMain.DataMali + @".SLS.QuotationItem QI ON QU_QuotationIdSP=QI.QuotationRef INNER JOIN
            //				" + FrmMain.DataMali + @".SLS.InvoiceItem II ON II.QuotationItemRef = QI.QuotationItemID INNER JOIN
            //				" + FrmMain.DataMali + @".INV.InventoryDeliveryItem IDI ON  IDI.BaseInvoiceItem = II.InvoiceItemID INNER JOIN
            //				" + FrmMain.DataMali + @".INV.InventoryDelivery ID ON IDI.InventoryDeliveryRef = ID.InventoryDeliveryID
            //				where INS_Item=itm.ITM_id and INS_Stock=istk.IS_stock AND TNS_fy_id=" + FiscalYearId + @" and TNS_TypeStock=1 AND INS_state = 1),0)
            //				),4))
            //else null end as InveoryStockSepSecondaryUnit,
            //case when stkcrm.ST_Inventorycontrol= 0 then (select ISNULL((select round(SUM(INS_Out2), 4) from dbo.PRG_InventoryStock where INS_Item = itm.ITM_id and INS_Stock = istk.IS_stock AND TNS_fy_id = " + FiscalYearId + @" and
            //		    TNS_TypeStock = 1 AND INS_state = 1 and INS_Type=0),0)) else null end as InventoryStockSepSecondaryUnitRes,
            //itm.ITM_nesabt, itm.ITM_nesbatValue, unit2.Title AS TitleUnit2, itm.ITM_unit2,Stocksep.StockID ,itm.ITM_Supplier, isnull(PRG_Customers.cst_fname,'')+' '+isnull( 
            //                      PRG_Customers.cst_laname,'') Supplier,itm.ITM_Unit,stkcrm.ST_Inventorycontrol,istk.IS_id,IS_GroupStock,GRS_GroupName,CAST(NULL AS decimal) AS FeeList,CAST(NULL AS decimal) AS PRC_FeeSell,ITM_duty,ITM_tax
            //FROM PRG_Item itm  INNER JOIN PRG_ItemStock istk
            //on istk.IS_itm_id= itm.ITM_id  " + defualt + @" LEFT OUTER JOIN
            //            dbo.PRG_GroupStock on GRS_id=istk.IS_GroupStock 
            // INNER JOIN
            //" + FrmMain.DataMali + @".INV.Stock Stocksep ON istk.IS_stock = Stocksep.StockID INNER JOIN 
            //" + FrmMain.DataMali + @".INV.Unit unitsep ON itm.ITM_Unit = unitsep.UnitID LEFT OUTER JOIN
            //                      " + FrmMain.DataMali + @".INV.Unit AS unit2 ON itm.ITM_unit2 = unit2.UnitID 
            //left JOIN PRG_Loockup AS PRG_Loockup_2 ON itm.ITM_Type = PRG_Loockup_2.LKP_id
            //left JOIN PRG_Loockup AS PRG_Loockup_3 ON itm.ITM_GroupItem = PRG_Loockup_3.LKP_id Left JOIN
            //                      PRG_Customers ON itm.ITM_Supplier = PRG_Customers.cst_id INNER JOIN
            //PRG_Stock stkcrm on stkcrm.ST_stockid=Stocksep.StockID" + TxtShowAdition1_2 + TxtShowAdition2_2 + TxtShowPurchaseItem_2 + TxtShowSaleItem_2 + @"
            //WHERE (itm.ITM_TypeActive = 1) " + tt + ") ListItems order by ListItems.InventoryStock desc";
            //            return Resualt;
        }

        public static string GetInventoryItem(string Cstid, string IS_id, decimal Inventory, string Code)
        {
            string Resualt = "";
            DataTable dtfiscalyear = GetFiscalYearNow(FiscalYearNowStr);
            string FiscalYearId = dtfiscalyear.Rows[0]["FiscalYearId"].ToString();

            DataTable dtmojodi = cp.select_command(SelectItem("", FiscalYearId, " AND istk.IS_id = " + IS_id, Cstid));
            if (dtmojodi.Rows.Count > 0)
            {
                if (dtmojodi.Rows[0]["InventoryStock"].ToString() != "")
                {
                    if ((decimal.Parse(dtmojodi.Rows[0]["InventoryStock"].ToString()) - Inventory) < 0)
                    {
                        Resualt = Code;
                    }
                }
            }

            return Resualt;
        }

        public static string formnameEdit = "NOT";
        //        public static string SPInPartCustomers()
        //        public static string SPInPartCustomers()
        //        {
        //            string text = @"DECLARE @dataname nvarchar(100) 
        //    SET @dataname =(select TNZ_sharh from dbo.PRG_Propertise where TNZ_name='DataMali')
        //EXECUTE('create PROCEDURE [dbo].[SPInPartCustomers] 
        //	(
        //	@id int
        //	)
        //AS
        //SET NOCOUNT ON; 
        //BEGIN
        //BEGIN  TRANSACTION [Tran1]
        //  BEGIN TRY 
        //  declare @checked int
        //   declare @useridsep int
        //   set @useridsep=(select USR_us from PRG_Users where USR_id =(select cst_Creator from dbo.PRG_Customers where cst_id=@id))
        //  declare @checkdl bit
        //  set @checkdl=0
        //  if exists (select TNZ_sharh  from PRG_Propertise where TNZ_name='NotSetCityToCustomerCode')
        //  begin
        //	  if(select CAST(TNZ_sharh AS NVARCHAR(10))  from PRG_Propertise where TNZ_name='NotSetCityToCustomerCode')='1'
        //	  begin
        //		 set @checkdl=1
        //	  end
        //  end
        //  declare @messagetitle nvarchar(2000)
        //   if(( select cst_CustomerType from PRG_Customers where PRG_Customers.cst_id=@id)=0)
        //   begin
        //	  if(exists(SELECT top(1) PRG_Customers.cst_fname, PRG_Customers.cst_laname, PRG_Customers.cst_ostan, PRG_Customers.cst_city, 
        //						PRG_Address.add_add
        //	FROM         PRG_Customers INNER JOIN
        //						  PRG_Address ON PRG_Customers.cst_id = PRG_Address.add_cst_id AND PRG_Address.add_type = 1 
        //	  where PRG_Customers.cst_id=@id and PRG_Customers.cst_fname  is not null  and PRG_Customers.cst_fname !='' and PRG_Customers.cst_laname is not null  and PRG_Customers.cst_laname!='' and
        //	  PRG_Customers.cst_ostan is not null  and PRG_Customers.cst_city  is not null 
        //	   ))
        //	  begin
        //	     set @checked=1
        //	  end 
        //	  else
        //	  begin    
        //	     set @checked=0
        //	  end
        //   end
        //   else
        //   begin 

        //    if(exists(SELECT top(1) PRG_Customers.cst_fname, PRG_Customers.cst_laname, PRG_Customers.cst_ostan, PRG_Customers.cst_city, 
        //	PRG_Address.add_add
        //	FROM         PRG_Customers INNER JOIN
        //						  PRG_Address ON PRG_Customers.cst_id = PRG_Address.add_cst_id AND PRG_Address.add_type = 1
        //	  where PRG_Customers.cst_id=@id and PRG_Customers.cst_fname  is not null  and PRG_Customers.cst_fname !=''
        //	  and PRG_Customers.cst_ostan is not null  and PRG_Customers.cst_city  is not null))
        //      begin
        //	     set @checked=1
        //	  end 
        //	  else
        //	  begin
        //	     set @checked=0
        //	  end
        //   end
        //   if(@checked=1)
        //   begin
        //    declare @Partytell int,@Cityname nvarchar(100),@Partyadd int,@partyid int,@ostan int ,@LastName nvarchar(500),@Name nvarchar(500),@type tinyint,@maxdlid int ,@code int
        // 	set @LastName=(select cst_laname from dbo.PRG_Customers where cst_id=@id)
        //	set @Name=(select cst_fname from dbo.PRG_Customers where cst_id=@id)
        //	set @ostan=(select cst_city from dbo.PRG_Customers where cst_id=@id)
        //	set @Cityname=(SELECT  locsep.Title FROM  PRG_Customers INNER JOIN   " + DataMali + @".GNR.Location locsep ON PRG_Customers.cst_city =  locsep.LocationId WHERE  (PRG_Customers.cst_id = @id))

        //	set @type=(select case cst_type when 0 then 1 when 1 then 2 end from dbo.PRG_Customers where cst_id=@id)

        //	set @code=(select isnull(max(CAST(Code AS int)),1) from  " + DataMali + @".ACC.DL where Type=2)

        //	declare @cst_melicod nvarchar(50)
        //	set @cst_melicod=(select CAST(cst_melicod  as nvarchar(50)) from dbo.PRG_Customers where cst_id=@id)
        //	declare @cst_eghtesadi nvarchar(50)
        //	set @cst_eghtesadi=(select CAST(cst_eghtesadi as nvarchar(50))  from dbo.PRG_Customers where cst_id=@id)
        //declare @blacklist bit
        //	set @blacklist=(select cst_blacklist from dbo.PRG_Customers where cst_id=@id)
        //	declare @tamin bit
        //	set @tamin=(select cst_Tamin from dbo.PRG_Customers where cst_id=@id)
        //	declare @vaset bit
        //	set @vaset=(select cst_Vaset from dbo.PRG_Customers where cst_id=@id)
        //	declare @customer bit
        //	set @customer=(select cst_Customer from dbo.PRG_Customers where cst_id=@id)
        //	declare @cst_Email nvarchar(500)
        //	set @cst_Email=(select cst_Email from dbo.PRG_Customers where cst_id=@id)
        //declare @cst_birthdate datetime
        //	if(select cst_birthdate from dbo.PRG_Customers where cst_id=@id)!=''
        //	begin
        //	set @cst_birthdate=(select cst_birthdate from dbo.PRG_Customers where cst_id=@id)
        //	end
        //	else
        //	begin
        //	set @cst_birthdate=null
        //	end
        //	  if(@checkdl=1)
        //	  begin
        //		set @messagetitle= @Name+' '+@LastName
        //	  end
        //	  else
        //	  begin	
        //	    set @messagetitle= @Name+' '+@LastName+' ('+@Cityname+')'
        //	  end
        //    DECLARE @strout int
        //        EXEC dbo.GetNextId 'ACC.DL',1, @strout OUTPUT
        //insert into  " + DataMali + @".ACC.DL (DLId, Code, Title, Title_En, Type, Version, Creator, CreationDate, LastModifier, LastModificationDate, IsActive)
        //	values(@strout,@code+1,REPLACE(REPLACE(CAST(@messagetitle as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)),
        //REPLACE(REPLACE(CAST(@messagetitle as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)),2,1,@useridsep,getdate(),@useridsep,getdate(),(select cst_state from PRG_Customers where PRG_Customers.cst_id=@id))


        //        EXEC dbo.GetNextId 'GNR.Party',1, @partyid OUTPUT

        //insert into  " + DataMali + @".GNR.Party
        //	(PartyId, Type, SubType, Name, LastName, Name_En, LastName_En, EconomicCode, IdentificationCode, RegistrationCode, 
        //	Website, Email, DLRef, IsInBlacklist, IsVendor, VendorGroupingRef, IsBroker, CommissionRate, BrokerOpeningBalance, 
        //	BrokerOpeningBalanceType, IsEmployee, IsCustomer, SalespersonPartyRef, DiscountRate, MaximumCredit, CustomerGroupingRef,
        //	 CustomerCategoryForTax, Version, Creator, CreationDate, LastModifier, LastModificationDate, BirthDate, MarriageDate,
        //	  HasCredit, CreditCheckingType, CustomerRemaining)
        //	values(@partyid,@type,7,REPLACE(REPLACE(CAST(@Name as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)), 
        //REPLACE(REPLACE(CAST(@LastName as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)), 
        //REPLACE(REPLACE(CAST(@Name as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)),
        //REPLACE(REPLACE(CAST(@LastName as nvarchar(max)), NCHAR(1740), NCHAR(1610)), NCHAR(1705), NCHAR(1603)),@cst_eghtesadi,@cst_melicod,null,null,
        //	@cst_Email,@strout,@blacklist,@tamin,null,@vaset,0,null,2,0,@customer,null,0,null,null,2,1,@useridsep,getdate(),@useridsep,getdate(),@cst_birthdate,null,0,3,null)

        //    IF(EXISTS(SELECT * FROM  " + DataMali + @".GNR.Party WHERE PartyId=@partyid))
        //    BEGIN
        //        update dbo.PRG_Customers set csT_dl=@strout  ,cst_modificationdate=getdate(), cst_type=1 where cst_id=@id
        //		declare @typead bit,@addadd nvarchar(MAX),@codepadd nvarchar(50) ,@row int,@add_id int
        //     	---------------------------------------------------------------------
        //        declare @typeeet bit,@tellt nvarchar(500),@typetellt bit,@typet2 tinyint
        //		set @row=0
        //		--IF NOT EXISTS (SELECT 1 FROM dbo.PRG_Tell where tel_cst_id = @id AND tel_type = 1)
        //		--BEGIN
        //		--    RAISERROR('لطفا شماره اصلی مشتری را مشخص نمایید.',16,1);
        //		--	ROLLBACK TRANSACTION [Tran1]
        //		--	RETURN;
        //		--END
        //		DECLARE vendor_cursortell CURSOR FOR 
        //		SELECT tel_type, tel_tell, tel_typetell
        //		FROM dbo.PRG_Tell where tel_cst_id =@id
        //		OPEN vendor_cursortell

        //		FETCH NEXT FROM vendor_cursortell 
        //		INTO @typeeet ,@tellt ,@typetellt

        //		WHILE @@FETCH_STATUS = 0
        //		BEGIN
        //			if(@typetellt=1)
        //			begin
        //		   set @typet2=2
        //		   end
        //		   else
        //		   begin
        //		   set @typet2=4
        //		   end

        //        EXEC dbo.GetNextId 'GNR.PartyPhone',1, @Partytell OUTPUT

        //                INSERT INTO  " + DataMali + @".GNR.PartyPhone
        //							  (PartyPhoneId, PartyRef, IsMain, Type, Phone, Version)
        //				VALUES     (@Partytell,@partyid,@typeeet,@typet2, @tellt,1 )
        //			FETCH NEXT FROM vendor_cursortell 
        //			INTO @typeeet ,@tellt ,@typetellt
        //		END 
        //		CLOSE vendor_cursortell
        //		DEALLOCATE vendor_cursortell
        //		-----------------------------------------------------------------------------
        //    	SELECT * FROM dbo.PRG_Customers WHERE cst_id NOT IN (SELECT add_cst_id FROM dbo.PRG_Address)


        //		--IF NOT EXISTS (SELECT 1 FROM dbo.PRG_Address where add_cst_id =@id AND add_type = 1)
        //		--BEGIN
        //		--    RAISERROR('لطفا ادرس اصلی مشتری را مشخص نمایید.',16,1);
        //		--	ROLLBACK TRANSACTION [Tran1]
        //		--	RETURN;
        //		--END

        //		DECLARE vendor_cursor CURSOR FOR 
        //		SELECT add_id,add_type, add_add,CAST(add_CodeP as nvarchar(50)) 
        //		FROM dbo.PRG_Address where add_cst_id =@id
        //		OPEN vendor_cursor

        //		FETCH NEXT FROM vendor_cursor 
        //		INTO @add_id,@typead ,@addadd ,@codepadd

        //		WHILE @@FETCH_STATUS = 0
        //		BEGIN
        //        EXEC dbo.GetNextId 'GNR.PartyAddress',1, @Partyadd OUTPUT
        //   INSERT INTO  " + DataMali + @".GNR.PartyAddress
        //							  (PartyAddressId, PartyRef, IsMain, Type, Address, LocationRef, ZipCode, Adress_En, Version)
        //				VALUES     (@Partyadd,@partyid,@typead,6, @addadd,@ostan,@codepadd,@addadd,1 )
        //update dbo.PRG_Address set add_PartyAdd=@Partyadd where add_id=@add_id
        //			FETCH NEXT FROM vendor_cursor 
        //			INTO @add_id,@typead ,@addadd ,@codepadd
        //		END 
        //		CLOSE vendor_cursor
        //		DEALLOCATE vendor_cursor
        //		-----------------------------------------------------------------------------
        //		 COMMIT TRANSACTION [Tran1]
        //		 select @code+1
        //    END 
        //    else
        //    begin
        //        ROLLBACK TRANSACTION [Tran1]
        //        select 0
        //    end
        //   end
        //   else
        //   begin
        //        ROLLBACK TRANSACTION [Tran1]
        //        select 2
        //   end


        //  END TRY

        //  BEGIN CATCH

        //      ROLLBACK TRANSACTION [Tran1]

        //  END CATCH end ";
        //            return text;
        //        }
        public static DataTable GetFiscalYearNow(string textyear)
        {

            if (string.IsNullOrEmpty(textyear))
            {
                textyear = "1404";
            }

            DataTable dt = cp.select_command(
                @"SELECT convert(date,StartDate) startdate , convert(date,EndDate) EndDate ,Title,FiscalYearId
                FROM " + DataMali + @".FMK.[FiscalYear] where Title=" + textyear);
            //DataTable dtperiod = _DS1.Tables[0];

            return dt;
        }

        public static string formname4_2 = "";

        public static int typecharkhe = 0;
        private static bool checkaccept = false;
        private static DataRow currecurrentRowNostatic2;
        DataTable dtListForsat;
        private static DataTable dtleave;

        public static void TayidRequestbuy()
        {

        }

        private static bool SendSmsResualt = false;
        bool flglistRequestBuyNostatic = false;
        public static bool checkExixstsSms = true;
        public static DataRow drstocklclsdb;
        public static bool checkinseretstatic = false;
        //public static SqlTransaction transactionclsd


        public static bool closeform = false;
        public static string stateform2 = "";
        public static string ThisText2 = "";
        public static bool CheckCustomersALl = false;
        public static bool CheckCustomersALlResualt = false;
        public static bool CheckCustomersALlResualtInsertDatabase = false;


        public static DataRow[] drmulitiMaintorStock;
        public static DataRow currentRowDelete;
        public static bool CheckErsalFlowchartclsdb = false;
        public static string quid = "";



        public static int counttrue = 0;

        //public static bool checkinsert = false;
        public static bool checktrn2 = false;

        public static int ShowPrintStr = -1;
        public static bool CheckPishnevis = false;
        public static int scalerReq = 0;
        public static int IdForm = 0;
        public static int CstForm = 0;
        public static int count = 0;
        public static bool EditFlowChart = false;


        //    bool sendOknosT = false;
        public static string subjectsummery;
        public static string subjectsummeryDesc;
        public static int TypeReff;
        string user = "";
        string post = "";
        public static DataTable dtlistRecieveClsdb;

        bool sendOKK = false;
        public static int CFDid = 0;
        public static int CFDSid = 0;


        public static DataTable MEdalCustomers()
        {
            DataTable dtshow = new DataTable();
            cls_public cp = new cls_public();
            int MAXWeight = 10;
            int MINWeight = 0;

            float WeightL = 0;
            float WeightR = 0;
            float WeightF = 0;
            float WeightM = 0;
            string StrWeightL = "";
            string StrWeightR = "";
            string StrWeightF = "";
            string StrWeightM = "";

            MAXWeight = 10;
            MINWeight = 0;
            //clsProperties.GetPropertise("").ToString()
            if (clsProperties.GetPropertise("WeightL").ToString() != "")
                WeightL = float.Parse(clsProperties.GetPropertise("WeightL").ToString()) / 40;
            if (clsProperties.GetPropertise("WeightR").ToString() != "")
                WeightR = float.Parse(clsProperties.GetPropertise("WeightR").ToString()) / 40;
            if (clsProperties.GetPropertise("WeightF").ToString() != "")
                WeightF = float.Parse(clsProperties.GetPropertise("WeightF").ToString()) / 40;
            if (clsProperties.GetPropertise("WeightM").ToString() != "")
                WeightM = float.Parse(clsProperties.GetPropertise("WeightM").ToString()) / 40;
            StrWeightL = WeightL.ToString().Replace('/', '.');
            StrWeightR = WeightR.ToString().Replace('/', '.');
            StrWeightF = WeightF.ToString().Replace('/', '.');
            StrWeightM = WeightM.ToString().Replace('/', '.');

            ListDataPoint = new List<DataPoint>();
            DataSet _DS = new DataSet();
            SqlDataAdapter _DA = new SqlDataAdapter();
            DataTable dtmedal =
                cp.select_command(
                    " select LKP_id, LKP_enownerName, LKP_name from dbo.PRG_Loockup where LKP_state=1 AND LKP_enownerName='CustomerMedal'");

            int k = 0;
            k = dtmedal.Rows.Count;
            if (k != 0)
            {
                _DS = new DataSet();
                _DA = new SqlDataAdapter();
                string reshte = "";
                if (clsProperties.GetPropertise("CheckLRFM").ToInt() != 0)
                {
                    reshte = @"IF OBJECT_ID('tempdb..#LRFM') IS NOT NULL
    DROP TABLE #LRFM 
SELECT  dbo.PRG_Customers.cst_id CST,DATEDIFF(day, MIN(INV.Date), MAX(INV.Date)) L ,  DATEDIFF(day, MAX(INV.Date),GETDATE()) R, COUNT(INV.InvoiceId) F,
      SUM(INV.NetPriceInBaseCurrency) M  INTO #LRFM
FROM         " + DataMali + @".SLS.Invoice INV INNER JOIN
                      " + DataMali + @".GNR.Party PRT ON INV.CustomerPartyRef = PRT.PartyId
                      INNER JOIN dbo.PRG_Customers ON PRT.Dlref=dbo.PRG_Customers.csT_dl
                                            WHERE INV.VoucherRef IS NOT NULL AND cast(SUBSTRING(dbo.mtsh(CONVERT(date,GETDATE())),1,4) as int)-cast(SUBSTRING(dbo.mtsh(CONVERT(date,INV.Date)),1,4) as int)<=" +
                             clsProperties.GetPropertise("Medal") + @" 
                                            GROUP BY dbo.PRG_Customers.cst_id";
                }
                else
                {
                    reshte = @"IF OBJECT_ID('tempdb..#LRFM') IS NOT NULL
    DROP TABLE #LRFM
SELECT ld1.LD_cst_id CST,DATEDIFF(day, MIN(ld1.LD_datebargiri), MAX(ld1.LD_datebargiri)) L , DATEDIFF(day, MAX(ld1.LD_datebargiri),GETDATE()) R, COUNT(ld1.LD_id) F,
     SUM(q1.QU_NetPrice) M INTO #LRFM
FROM PRG_Lading ld1 INNER JOIN
                      PRG_Quotation q1 ON ld1.LD_Qu_id = q1.QU_id
WHERE (ld1.LD_state = 1) AND cast(SUBSTRING(dbo.mtsh(CONVERT(date,GETDATE())),1,4) as int)-cast(SUBSTRING(dbo.mtsh(CONVERT(date,ld1.LD_datebargiri)),1,4) as int)<=" +
                             clsProperties.GetPropertise("Medal").ToString() + @" 
GROUP BY ld1.LD_cst_id";
                }

                DataTable dtLRFMCAlc = new DataTable();
                dtLRFMCAlc = cp.select_command(@"IF OBJECT_ID('tempdb..#LRFM') IS NOT NULL
BEGIN
  Drop TABLE #LRFM
END
     " + reshte + @"
SELECT y.NormalizeL,y.NormalizeR,y.NormalizeF,y.NormalizeM,y.NormalizeL,SUM(y.NormalizeL) OVER ()/COUNT(y.CST) OVER () AS  LK 
,SUM(Y.NormalizeR) OVER ()/COUNT(Y.CST)  OVER () AS RK,SUM(Y.NormalizeF) OVER ()/COUNT(Y.CST) OVER () AS FK,SUM(Y.NormalizeM) OVER ()/COUNT(Y.CST) OVER () AS MK,
SUM(Y.CLV) OVER ()/COUNT(Y.CST) OVER () AS CLVK ,Y.L,y.R,y.F,cast(y.M as decimal) M,y.CST,y.Code,y.FullName,y.CLV,y.TitleOstan, y.TitleCity
FROM (SELECT
    (1.00*(L-MinL)/LRange)*" + StrWeightL + @" NormalizeL,
    (1.00*(MAXR - R)/RRange)*" + WeightL.ToString() + @" NormalizeR,
    (1.00*(F-MinF)/FRange)*" + WeightL.ToString() + @" NormalizeF,
    (1.00*(M-MinM)/MRange)*" + WeightL.ToString() + @" NormalizeM,
    --((1.00*(L-MinL)/LRange)*0.08333)+((1.00*(MAXR - R)/RRange)*0.1388)+((1.00*(F-MinF)/FRange)*0.3055)+((1.00*(M-MinM)/MRange)*0.4722) CLV,
    ((1.00*(L-MinL)/LRange)*" + StrWeightL + @")+((1.00*(MAXR - R)/RRange)*" + WeightR.ToString() +
                                               @")+((1.00*(F-MinF)/FRange)*" + WeightF.ToString() +
                                               @")+((1.00*(M-MinM)/MRange)*" +
                                               WeightM.ToString() + @") CLV,
    L,R,F,M,CST,FullName,Code,TitleOstan, TitleCity
FROM
    (
    SELECT
       lrfm.L,
       MIN(lrfm.L) OVER () AS MinL,
       MAX(lrfm.L) OVER () - MIN(lrfm.L) OVER () AS LRange,
       lrfm.R,
       MAX(lrfm.R) OVER () AS MAXR,
       MAX(lrfm.R) OVER () - MIN(lrfm.R) OVER () AS RRange,
       lrfm.F,
       MIN(lrfm.F) OVER () AS MinF,
       MAX(lrfm.F) OVER () - MIN(lrfm.F) OVER () AS FRange,
       lrfm.M,
       MIN(lrfm.M) OVER () AS MinM,
       MAX(lrfm.M) OVER () - MIN(lrfm.M) OVER () AS MRange,CST,ISNULL(PRGCst.cst_laname,'')+' '+ISNULL(PRGCst.cst_fname,'') FullName,dl.Code,REPLACE(REPLACE(CAST(loc.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS TitleOstan,REPLACE(REPLACE(CAST(loc2.Title AS nvarchar(MAX)), NCHAR(1610), NCHAR(1740)), NCHAR(1603), NCHAR(1705)) AS TitleCity
    FROM
       #LRFM lrfm join
       dbo.PRG_Customers PRGCst on lrfm.CST=PRGCst.cst_id join " + DataMali + @".acc.DL dl on dl.DLId=PRGCst.csT_dl
       join " + DataMali + @".GNR.Location loc on loc.LocationId=PRGCst.cst_ostan 
       join " + DataMali + @".GNR.Location loc2 on loc2.LocationId=PRGCst.cst_city 
    ) X
    ) Y  ORDER BY Y.CLV Desc");


                DataPoint dp = new DataPoint();
                float avefirst = 0;
                //float ff = float.Parse(dtLRFMCAlc.Rows[dtLRFMCAlc.Rows.Count - 1]["CLV"].ToString());
                // ff = float.Parse(dtLRFMCAlc.Rows[0]["CLV"].ToString());
                if (dtLRFMCAlc.Rows.Count > 0)
                {
                    avefirst = float.Parse(dtLRFMCAlc.Rows[dtLRFMCAlc.Rows.Count - 1]["CLV"].ToString()) -
                               float.Parse(dtLRFMCAlc.Rows[0]["CLV"].ToString());
                    avefirst = avefirst / k;
                    float firsitemcluster = float.Parse(dtLRFMCAlc.Rows[0]["CLV"].ToString());
                    for (int i = 1; i <= k; i++)
                    {
                        firsitemcluster = firsitemcluster + avefirst;
                        dp = new DataPoint();
                        dp.Cluster = i;
                        dp.CLV = 0;
                        dp.CST = 0;
                        dp.L = 0;
                        dp.R = 0;
                        dp.F = 0;
                        dp.M = 0;
                        dp.Ave = firsitemcluster;
                        dp.FullName = "";
                        dp.TitleCity = "";
                        dp.TitleOstan = "";
                        dp.Code = "";
                        ListDataPoint.Add(dp);
                    }
                }

                float First = 0;
                int CST = 0;
                int Cluster = 0;
                float L = 0;
                float CLV = 0;
                float R = 0;
                float F = 0;
                decimal M = 0;
                string Fullname = "";
                string TitleCity = "";
                string TitleOstan = "";
                string Code = "";

                while (dtLRFMCAlc.Rows.Count != 0)
                {
                    float FirstFinal = 0;
                    foreach (DataRow dr in dtLRFMCAlc.Rows)
                    {
                        foreach (DataPoint dataPoint in ListDataPoint)
                        {
                            //if (dataPoint.CST.ToString() != "0")
                            //{
                            First = Math.Abs(float.Parse(dr["CLV"].ToString()) - float.Parse(dataPoint.Ave.ToString()));
                            if (FirstFinal == 0)
                            {
                                FirstFinal = First;
                                CST = Int32.Parse(dr["CST"].ToString());
                                Cluster = Int32.Parse(dataPoint.Cluster.ToString());
                                L = float.Parse(dr["L"].ToString());
                                R = float.Parse(dr["R"].ToString());
                                F = float.Parse(dr["F"].ToString());
                                M = decimal.Parse(dr["M"].ToString());
                                CLV = float.Parse(dr["CLV"].ToString());
                                Fullname = dr["FullName"].ToString();
                                TitleCity = dr["TitleCity"].ToString();
                                TitleOstan = dr["TitleOstan"].ToString();
                                Code = dr["Code"].ToString();
                            }
                            else
                            {
                                if (First < FirstFinal)
                                {
                                    FirstFinal = First;
                                    CST = Int32.Parse(dr["CST"].ToString());
                                    Cluster = Int32.Parse(dataPoint.Cluster.ToString());
                                    L = float.Parse(dr["L"].ToString());
                                    R = float.Parse(dr["R"].ToString());
                                    F = float.Parse(dr["F"].ToString());
                                    M = decimal.Parse(dr["M"].ToString());
                                    CLV = float.Parse(dr["CLV"].ToString());
                                    Fullname = dr["FullName"].ToString();
                                    TitleCity = dr["TitleCity"].ToString();
                                    TitleOstan = dr["TitleOstan"].ToString();
                                    Code = dr["Code"].ToString();
                                }
                            }
                            //}
                        }

                        dp = new DataPoint();
                        dp.Cluster = Cluster;
                        dp.CLV = FirstFinal;
                        dp.CST = CST;
                        dp.Ave = FirstFinal;
                        dp.Code = Code;
                        dp.FullName = Fullname;
                        dp.TitleCity = TitleCity;
                        dp.TitleOstan = TitleOstan;
                        dp.M = M;
                        dp.F = F;
                        dp.R = R;
                        dp.L = L;
                        dp.CLV = CLV;
                        ListDataPoint.Add(dp);
                        dr.Delete();
                        dr.AcceptChanges();
                        float avecluster = 0;
                        float countcluster = 0;
                        foreach (DataPoint dataPoint in ListDataPoint)
                        {
                            if (Int32.Parse(dataPoint.Cluster.ToString()) == Cluster)
                            {
                                if (dataPoint.CST.ToString() != "0")
                                {
                                    avecluster = avecluster + float.Parse(dataPoint.CLV.ToString());
                                    countcluster++;
                                }
                            }
                        }

                        avecluster = avecluster / countcluster;
                        foreach (DataPoint dataPoint in ListDataPoint)
                        {
                            if (Int32.Parse(dataPoint.Cluster.ToString()) == Cluster)
                            {
                                dataPoint.Ave = avecluster;
                            }
                        }

                        break;
                    }
                }

                dtshow = new DataTable();
                dtshow.Columns.Add("Clustername", typeof(string));
                dtshow.Columns.Add("Cluster", typeof(string));
                dtshow.Columns.Add("CST", typeof(string));
                dtshow.Columns.Add("Code", typeof(string));
                dtshow.Columns.Add("FullName", typeof(string));
                dtshow.Columns.Add("L", typeof(string));
                dtshow.Columns.Add("R", typeof(string));
                dtshow.Columns.Add("F", typeof(string));
                dtshow.Columns.Add("M", typeof(decimal));
                dtshow.Columns.Add("CLV", typeof(string));
                dtshow.Columns.Add("TitleOstan", typeof(string));
                dtshow.Columns.Add("TitleCity", typeof(string));
                dtshow.Columns.Add("ClusterColor", typeof(int));
                dtshow.Columns.Add("lkpid", typeof(int));
                foreach (DataPoint dataPoint in ListDataPoint)
                {
                    if (dataPoint.CST.ToString() != "0")
                        dtshow.Rows.Add(DBNull.Value, dataPoint.Cluster.ToString(), dataPoint.CST.ToString(),
                            dataPoint.Code.ToString(), dataPoint.FullName.ToString(), dataPoint.L.ToString(),
                            dataPoint.R.ToString(),
                            dataPoint.F.ToString(), decimal.Parse(dataPoint.M.ToString()), dataPoint.CLV.ToString(),
                            dataPoint.TitleOstan.ToString(), dataPoint.TitleCity.ToString());
                }



                DataTable dtperiod = cp.select_command(
                    @"SELECT PRG_DetailMedalPeriod.ID_id, PRG_Loockup.LKP_name, PRG_DetailMedalPeriod.name_period,PRG_DetailMedalPeriod.Color, PRG_Loockup.LKP_id
FROM PRG_DetailMedalPeriod INNER JOIN
                      PRG_Loockup ON PRG_DetailMedalPeriod.ID_LKP_id = PRG_Loockup.LKP_id order by name_period");

                foreach (DataRow dr in dtshow.Rows)
                {
                    string medal = "";
                    int medalColor = 0;
                    int lkpid = 0;
                    foreach (DataRow drperiod in dtperiod.Rows)
                    {
                        if (dr["Cluster"].ToString() == drperiod["name_period"].ToString())
                        {
                            medal = drperiod["LKP_name"].ToString();
                            lkpid = Int32.Parse(drperiod["LKP_id"].ToString());
                            if (drperiod["Color"].ToString() != "")
                                medalColor = Int32.Parse(drperiod["Color"].ToString());
                        }
                    }

                    dr.BeginEdit();
                    dr["Clustername"] = medal;
                    dr["ClusterColor"] = medalColor;
                    dr["lkpid"] = lkpid;
                    dr.EndEdit();
                    dr.AcceptChanges();
                }
            }

            return dtshow;
        }

        public static bool BeforeGetInventoryItem(string Cstid, string Item, string Stock, string id, string formname, string values, string code, DataTable dtaghlam0, int Idref)
        {
            DataTable dtaghlam = dtaghlam0.Copy();
            string CheckInventory = "";
            bool CheckInventoryTrue = false;
            bool chkbalance = false;

            if (formname == "PRG_RequestBuyItem")
                if (clsProperties.GetPropertise("chkbalanceR").ToBoolean())
                    chkbalance = true;
            if (formname == "PRG_QuotationItem")
                if (clsProperties.GetPropertise("chkbalanceQ").ToBoolean())
                    chkbalance = true;
            if (formname == "PRG_InvoiceItem")
                if (clsProperties.GetPropertise("chkbalanceI").ToBoolean())
                    chkbalance = true;


            if (chkbalance)//موجودی  چک شود در تنظطیمات
            {
                foreach (DataRow dr in dtaghlam.Rows)
                {

                    DataTable dtmcheckojodi = cp.select_command(@"select case ST_Inventorycontrol when 1 then 1 
else IS_Inventorycontrol end as checkInventorycontrol from dbo.PRG_ItemStock
join dbo.PRG_Stock on ST_stockid = IS_stock where IS_stock=" + dr[Stock].ToString() + " AND IS_itm_id=" + dr[Item].ToString());

                    if (dtmcheckojodi.Rows[0][0].ToString() == "1")
                    {
                        if (string.IsNullOrEmpty(dr[values].ToString()))
                        {
                            CheckInventoryTrue = false;
                            checkinseretstatic = true;
                            break;
                        }
                        decimal sumvalues = 0;
                        DataTable dtmojodi = new DataTable();

                        sumvalues = decimal.Parse(dr[values].ToString());
                        if (Idref != 0)
                        {
                            if (clsProperties.GetPropertise("reservesanad").ToInt() == 1)//درخواست خرید
                            {
                                dtmojodi = cp.select_command("select INS_Out from [dbo].[PRG_InventoryStock] where INS_Type=0 and INS_state =1 and INS_RequestBuyItem =(select RBI_id from PRG_RequestBuyItem where RBI_it_id=" + dr["QUI_it_id"].ToString() + " and RBI_stock=" + dr["QUI_stock"].ToString() + " and RBI_RB_id=" + Idref.ToString() + ")");
                                if (dtmojodi != null)
                                    if (dtmojodi.Rows.Count > 0)
                                        sumvalues = sumvalues - decimal.Parse(dtmojodi.Rows[0][0].ToString());
                            }
                        }
                        if (clsProperties.GetPropertise("reservesanad").ToInt() == 3)//پیش فاکتور ثبت شده
                        {
                            if (!string.IsNullOrEmpty(dr["QUI_id"].ToString()))
                            {
                                dtmojodi = cp.select_command("select INS_Out from [dbo].[PRG_InventoryStock] where INS_Type=0 and INS_state =1 and INS_QuotationItem =" + dr["QUI_id"].ToString());

                                if (dtmojodi != null)
                                    if (dtmojodi.Rows.Count > 0)
                                        sumvalues = sumvalues - decimal.Parse(dtmojodi.Rows[0][0].ToString());
                            }
                        }
                        string CheckInventory0 = "";
                        CheckInventory0 = GetInventoryItem(Cstid, dr["IS_id"].ToString(), sumvalues, dr[code].ToString());
                        if (CheckInventory0 != "")
                        {
                            CheckInventory = CheckInventory + CheckInventory0 + ",";
                        }
                    }
                }

                if (CheckInventory != "")
                {
                    CheckInventoryTrue = false;
                    //CheckInventory = CheckInventory.Substring(0, CheckInventory.Length - 1);
                    //if (clsProperties.GetPropertise("chkNegativebalance").ToBoolean())//پیغام هشدار داده شود یانه در تنظیمات سیستمی موجودی کالا
                    //{
                    //    if (DialogResult.No == classes.clsFarsiMessageBox.MojodiManfi(" موجودی کالا با کدهای " + "(" + CheckInventory + ")" + " منفی میشود.آیا فرم با کالای دارای موجودی منفی ثبت شود؟ "))
                    //    {
                    //        CheckInventoryTrue = false;
                    //        checkinseretstatic = true;
                    //    }
                    //    else
                    //        CheckInventoryTrue = true;
                    //}
                    //else
                    //{
                    //    CheckInventoryTrue = false;
                    //    checkinseretstatic = true;
                    //    classes.clsFarsiMessageBox.ShowMessage(0, " موجودی کالا با کدهای " + "(" + CheckInventory + ")" + " منفی میشود ", "خطا", "تایید", "تایید", "", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, MessageBoxOptions.RightAlign);
                    //}
                }
                else
                {
                    CheckInventoryTrue = true;
                }
            }
            else
            {
                CheckInventoryTrue = true;
            }
            return CheckInventoryTrue;
        }
    }
}