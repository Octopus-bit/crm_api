/*
  Recommended indexes for sigma_api hot paths.
  Review on staging before applying to production.
  All statements use IF NOT EXISTS for safe re-run.
*/

-- Token auth (API_GetUserByToken)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_API_Tokens_Token' AND object_id = OBJECT_ID('dbo.API_Tokens'))
    CREATE NONCLUSTERED INDEX IX_API_Tokens_Token
        ON dbo.API_Tokens (tk_Token)
        INCLUDE (tk_UserIdRef, tk_AtCreate, tk_typeToken);

-- Customer lookup by mobile (API_GetCustomerInfoByMobile, android call sync)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_Tell_tel_tell' AND object_id = OBJECT_ID('dbo.PRG_Tell'))
    CREATE NONCLUSTERED INDEX IX_PRG_Tell_tel_tell
        ON dbo.PRG_Tell (tel_tell)
        INCLUDE (tel_cst_id);

-- Address lookup for quotations (add_cst_id + add_type)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_Address_cst_type' AND object_id = OBJECT_ID('dbo.PRG_Address'))
    CREATE NONCLUSTERED INDEX IX_PRG_Address_cst_type
        ON dbo.PRG_Address (add_cst_id, add_type)
        INCLUDE (add_id);

-- Quotation numbering per fiscal year
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_Quotation_FiscalYear_no' AND object_id = OBJECT_ID('dbo.PRG_Quotation'))
    CREATE NONCLUSTERED INDEX IX_PRG_Quotation_FiscalYear_no
        ON dbo.PRG_Quotation (QU_FiscalYearRef, QU_no DESC)
        INCLUDE (QU_id, QU_cst_id, QU_date);

-- Request buy numbering per fiscal year
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_RequestBuy_FiscalYear_no' AND object_id = OBJECT_ID('dbo.PRG_RequestBuy'))
    CREATE NONCLUSTERED INDEX IX_PRG_RequestBuy_FiscalYear_no
        ON dbo.PRG_RequestBuy (RB_FiscalYearRef, RB_no DESC)
        INCLUDE (RB_id, RB_cst_id, RB_date);

-- Inventory checks (BeforeGetInventoryItem)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_ItemStock_stock_item' AND object_id = OBJECT_ID('dbo.PRG_ItemStock'))
    CREATE NONCLUSTERED INDEX IX_PRG_ItemStock_stock_item
        ON dbo.PRG_ItemStock (IS_stock, IS_itm_id)
        INCLUDE (IS_Inventorycontrol);

-- Properties lookup (clsProperties cache warm query)
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_Propertise_name_usr' AND object_id = OBJECT_ID('dbo.PRG_Propertise'))
    CREATE NONCLUSTERED INDEX IX_PRG_Propertise_name_usr
        ON dbo.PRG_Propertise (TNZ_name, TNZ_usr_id)
        INCLUDE (TNZ_sharh, TNZ_image);

-- Lookup tables used in list endpoints
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_PRG_Loockup_type' AND object_id = OBJECT_ID('dbo.PRG_Loockup'))
    CREATE NONCLUSTERED INDEX IX_PRG_Loockup_type
        ON dbo.PRG_Loockup (LK_type)
        INCLUDE (LK_id, LK_name, LK_desc);

/*
  Stored procedure optimization notes (apply in SSMS, not in repo):
  - Replace CURSOR loops in SPInPartCustomers-style procs with set-based INSERT...SELECT.
  - Merge API_Sales6Month + API_ComparisonSales6Month + API_GetStatusPortal into one proc with multiple result sets.
  - Add SET NOCOUNT ON to all API_* procedures to reduce network chatter.
  - Use READ COMMITTED SNAPSHOT at database level to reduce reader/writer blocking under batch call sync.
*/
