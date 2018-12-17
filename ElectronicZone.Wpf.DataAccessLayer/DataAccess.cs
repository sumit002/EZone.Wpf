using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace ElectronicZone.Wpf.DataAccessLayer
{
    public class DataAccess : IDisposable
    {
        #region Properties
        SQLHelper sQLHelper = null; 
        #endregion

        public DataAccess()
        {
            this.sQLHelper = new SQLHelper();
        }

        public void CommitTransaction()
        {
            sQLHelper.CommitTransaction();
        }

        public void RollbackTransaction()
        {
            sQLHelper.RollbackTransaction();
        }

        public void Dispose()
        {
            sQLHelper.Dispose();
        }

        #region User Validation
        /// <summary>
        /// Validate User Login by UserName and Password
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public DataTable ValidateUserLogin(string userName, string password)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@USERNAME", userName));
            dbParameterList.Add(new SQLiteParameter("@PASSWORD", password));
            string queryToUse = "SELECT * FROM tblUser WHERE Username=@USERNAME AND Password=@PASSWORD";
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public bool IfExistsValue(string tableName, string columnName, string value)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("Select * from {1} Where {0}='{2}' COLLATE NOCASE", columnName, tableName, value);

            DataTable dt = sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }

        public bool IfContactExists(string tableName, string column1, string column2, string value1, string value2)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            string queryToUse = string.Format("Select * from {0} Where {1}='{3}' COLLATE NOCASE and {2}='{4}'", tableName, column1, column2, value1, value2);
            DataTable dt = sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
            if (dt.Rows.Count > 0)
                return true;
            else
                return false;
        }
        #endregion User Validation

        #region masters
        /// <summary>
        /// Insert/Update Product
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int InsertOrUpdateProductMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            ////construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            string updateQuery = string.Format("UPDATE {0} SET Name=@Name, Description=@Description, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            return sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
        }

        public DataTable GetAllProducts()
        {
            // string queryToUse = string.Format("SELECT Id, Name, Description, IsActive, CreatedDate, ModifiedDate FROM tblProductMaster WHERE IsActive = 1");
            string queryToUse = string.Format("SELECT * FROM vw_ProductMaster WHERE IsActive = 1");
            return sQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteProduct(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblProductMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }
        /// <summary>
        /// Insert/Update Brand/Company
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int InsertOrUpdateBrandMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            ////construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            string updateQuery = string.Format("UPDATE {0} SET Name=@Name, Description=@Description, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            return sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
        }

        public DataTable GetAllBrands()
        {
            // string queryToUse = string.Format("SELECT Id, Name, Description, IsActive, CreatedDate, ModifiedDate FROM tblBrandMaster WHERE IsActive = 1");
            string queryToUse = string.Format("SELECT * FROM vw_BrandMaster WHERE IsActive = 1");
            return sQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteBrand(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblBrandMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }
        /// <summary>
        /// Insert/Update Stock
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int InsertOrUpdateStockMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            //string updateQuery = string.Format("UPDATE {0} SET ProductCode=@ProductCode, StockCode=@StockCode, ItemDesc=@ItemDesc, AvlQuantity=@AvlQuantity, PurchasePrice=@PurchasePrice, SalePrice=@SalePrice, ProductImage=@ProductImage, PurchaseDate=@PurchaseDate, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            string updateQuery = string.Format("UPDATE {0} SET ProductCode=@ProductCode, StockCode=@StockCode, ItemDesc=@ItemDesc, Quantity=@Quantity, AvlQuantity=@AvlQuantity, PurchasePrice=@PurchasePrice, SalePrice=@SalePrice, PurchaseDate=@PurchaseDate, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            int result = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = sQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public int UpdateStockQuantity(Dictionary<string, string> columns, string tableName, bool isAdd = false)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string updateQuery = $"UPDATE {tableName} SET AvlQuantity=AvlQuantity{(isAdd==true ? "+" : "-")}@AvlQuantity, ModifiedDate=@ModifiedDate WHERE ID=@ID";
            return sQLHelper.InsertOrUpdate(string.Empty, updateQuery, existanceTestQuery, dbParameterList);
        }

        public int UpdateStockImage(byte[] pImage, int pId, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@ID", pId));
            dbParameterList.Add(new SQLiteParameter("@ProductImage", pImage));
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string updateQuery = string.Format("UPDATE {0} SET ProductImage=@ProductImage WHERE ID=@ID", tableName);
            return sQLHelper.InsertOrUpdate(string.Empty, updateQuery, existanceTestQuery, dbParameterList);
        }

        //public DataTable GetAllStocks()
        //{
        //    List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
        //    //construct parameter
        //    //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
        //    string queryToUse = string.Format("SELECT * FROM vw_StockMaster_New");
        //    return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        //}

        public int DeleteStock(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblStockMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        /// <summary>
        /// Search Stocks / Purchase
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="brandId"></param>
        /// <param name="productCode"></param>
        /// <param name="stockCode"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="avlQty"></param>
        /// <returns></returns>
        public DataTable SearchStocks(string productId, string brandId, string productCode, string stockCode, double? priceMin, double? priceMax, string fromDate, string toDate, bool? avlQty = false, bool? outOfStock = false)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_StockMaster_New WHERE Quantity > 0 ");
            if (!string.IsNullOrEmpty(productId))
                queryToUse += $" AND ProductId = {productId}";
            if (!string.IsNullOrEmpty(brandId))
                queryToUse += string.Format(" AND BrandId = {0}", brandId);
            if (!string.IsNullOrEmpty(productCode))
                queryToUse += string.Format(" AND ProductCode like '%{0}%'", productCode);
            if (!string.IsNullOrEmpty(stockCode))
                queryToUse += string.Format(" AND StockCode like '%{0}%'", stockCode);
            if (priceMin.HasValue)
                queryToUse += string.Format(" AND SalePrice >= {0}", priceMin.Value);
            if (priceMax.HasValue)
                queryToUse += string.Format(" AND SalePrice <= {0}", priceMax.Value);
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND PurchaseDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND PurchaseDate <= '{0}'", toDate);
            if (avlQty.Value)
                queryToUse += string.Format(" AND AvlQuantity > 0");
            if(outOfStock.Value)
                queryToUse += string.Format(" AND AvlQuantity = 0");
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        /// <summary>
        /// Insert/Update Sales
        /// </summary>
        /// <param name="columns"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public int InsertOrUpdateSaleMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            //check this update [no necessary]
            string updateQuery = string.Format("UPDATE {0} SET AmountPaid=AmountPaid+@AmountPaid, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            int rslt = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = sQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public int InsertOrUpdateInvoiceMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            //check this update [no necessary]
            string updateQuery = string.Format("UPDATE {0} SET IsActive=@IsActive WHERE ID=@ID", tableName);
            int rslt = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            // int rowId = sQLHelper.getLastRowId(tableName);
            return rslt;
        }

        public DataTable GetSalesOnDemand()
        {
            string queryToUse = string.Format("Select SalesId, Product, ProductCode, COUNT(*) AS SaleCount from vw_SaleMaster GROUP BY Product, ProductCode ORDER BY SaleCount DESC");
            return sQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteSalesOrder(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblSaleMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        public int DeleteSalesInvoice(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblInvoiceMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        public int DeletePendingPayment(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblPendingPayment WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        public DataTable GetSaleInvoices(string SalesIds)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            if (!string.IsNullOrEmpty(SalesIds)) {
                dbParameterList.Add(new SQLiteParameter("@SalesIds", SalesIds));
            }
            string queryToUse = string.Format("SELECT * FROM vw_SaleMaster Where SalesId IN (@SalesIds)");
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public int InsertPaymentMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            ////construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            //string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            //string updateQuery = string.Format("UPDATE {0} SET Name=@Name, Description=@Description, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            return sQLHelper.InsertOrUpdate(insertQuery, string.Empty, string.Empty, dbParameterList);
        }

        public int InsertOrUpdateSalePerson(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            string updateQuery = string.Format("UPDATE {0} SET Title=@Title, Name=@Name, Contact=@Contact, AlternateContact=@AlternateContact, Email=@Email, Address=@Address WHERE ID=@ID", tableName);

            int res = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = sQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public DataTable GetAllSalesPerson()
        {
            string queryToUse = string.Format("SELECT * FROM vw_SalePerson");
            return sQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteSalesPerson(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblSalePerson WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        //public DataTable SearchSales(string id)
        //{
        //    List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
        //    //construct parameter
        //    if (!string.IsNullOrEmpty(id))
        //    {
        //        dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
        //    }

        //    string queryToUse = string.Format("SELECT Id, Name, Description, CreatedDate FROM tblSaleMaster WHERE IsActive = 1");
        //    return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        //}

        public int InsertOrUpdatePendingPayment(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            string updateQuery = string.Format("UPDATE {0} SET IsPaid=@IsPaid, PaidDate=@PaidDate, IsDiscount=@IsDiscount, PaidAmount=@PaidAmount WHERE ID=@ID", tableName);

            int res = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = sQLHelper.getLastRowId(tableName);
            return rowId;
        }

        //public DataTable GetAllPendingPayment(string id)
        //{
        //    List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
        //    //construct parameter
        //    if (!string.IsNullOrEmpty(id)){
        //        dbParameterList.Add(new SQLiteParameter("@Id", id));
        //    }
        //    string queryToUse = string.Format("SELECT * FROM vw_PendingPaymentMaster WHERE IsPaid = 0");
        //    return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        //}

        public int InsertOrUpdateSupportPaymentMaster(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            foreach (string col in columns.Keys) {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string insertQuery = string.Format("INSERT INTO {0} ({1}) values ({2})", tableName, String.Join(",", columns.Keys), String.Join(",",
                dbParameterList.Select(r => r.ParameterName).ToArray()));
            //string updateQuery = string.Format("UPDATE {0} SET Description=@Description, Amount=@Amount, SupportDate=@SupportDate, Remarks=@Remarks WHERE ID=@ID", tableName);
            string updateQuery = string.Format("UPDATE {0} SET Description=@Description, SupportDate=@SupportDate, Remarks=@Remarks WHERE ID=@ID", tableName);
            int res = sQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = sQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public DataTable GetAllSupportPayment(string id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            if (!string.IsNullOrEmpty(id))
            {
                dbParameterList.Add(new SQLiteParameter("@Id", id));
            }

            string queryToUse = string.Format("SELECT Id, Description, Amount, SupportDate, Remarks FROM tblSupportPaymentMaster WHERE IsActive=1");
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public int DeleteSupportPayment(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblSupportPaymentMaster WHERE Id = @Id");
            return sQLHelper.DeleteFromTable(queryToUse, dbParameterList);
        }

        // dashboard
        public DataTable getCredirDebitByDate(string startDate, string endDate)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("Select SUM(pm.Cr) Credit, SUM(pm.Dr) Debit from tblPaymentMaster pm");
            if (!string.IsNullOrEmpty(startDate))
                queryToUse += string.Format(" AND pm.CreatedDate >= {0}", startDate);
            if (!string.IsNullOrEmpty(endDate))
                queryToUse += string.Format(" AND pm.CreatedDate <= {0}", endDate);

            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        #endregion masters

        #region Reports
        /// <summary>
        /// Search Sales
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="brandId"></param>
        /// <param name="productCode"></param>
        /// <param name="stockCode"></param>
        /// <param name="priceMin"></param>
        /// <param name="priceMax"></param>
        /// <returns></returns>
        public DataTable SearchSales(string productId, string brandId, string productCode, string stockCode, int? priceMin, int? priceMax, string fromDate, string toDate, string salesPersonId)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_SaleMaster Where Quantity > 0 ");
            if (!string.IsNullOrEmpty(productId))
                queryToUse += string.Format(" AND ProductId = {0}", productId);
            if (!string.IsNullOrEmpty(brandId))
                queryToUse += string.Format(" AND BrandId = {0}", brandId);
            if (!string.IsNullOrEmpty(productCode))
                queryToUse += string.Format(" AND ProductCode like '%{0}%'", productCode);
            if (!string.IsNullOrEmpty(stockCode))
                queryToUse += string.Format(" AND StockCode like '%{0}%'", stockCode);
            if (priceMin.HasValue)
                queryToUse += string.Format(" AND SalePrice >= {0}", priceMin.Value);
            if (priceMax.HasValue)
                queryToUse += string.Format(" AND SalePrice <= {0}", priceMax.Value);
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND SaleDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND SaleDate <= '{0}'", toDate);
            if (!string.IsNullOrEmpty(salesPersonId))
                queryToUse += string.Format(" AND SalesPersonId = {0}", salesPersonId);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable SearchSupportPayment(int? priceMin, int? priceMax, string fromDate, string toDate, string description)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM tblSupportPaymentMaster Where IsActive=1 ");
            if (priceMin.HasValue)
                queryToUse += string.Format(" AND Amount >= {0}", priceMin.Value);
            if (priceMax.HasValue)
                queryToUse += string.Format(" AND Amount <= {0}", priceMax.Value);
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND SupportDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND SupportDate <= '{0}'", toDate);
            if (!string.IsNullOrEmpty(description))
                queryToUse += string.Format(" AND Description like '%{0}%'", description);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable SearchPendingPayment(int? priceMin, int? priceMax, string fromDate, string toDate, string salesPersonId, int? isPaid)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_PendingPaymentMaster Where PendingAmount > 0 ");
            if (priceMin.HasValue)
                queryToUse += string.Format(" AND PendingAmount >= {0}", priceMin.Value);
            if (priceMax.HasValue)
                queryToUse += string.Format(" AND PendingAmount <= {0}", priceMax.Value);
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND SaleDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND SaleDate <= '{0}'", toDate);
            if (!string.IsNullOrEmpty(salesPersonId))
                queryToUse += string.Format(" AND SalesPersonId = {0}", salesPersonId);
            if (isPaid.HasValue)
                queryToUse += string.Format(" AND IsPaid = {0}", isPaid.Value);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable SearchPaymentIncome(string fromDate, string toDate, string paymentType)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_PaymentIncome Where Amount > 0 ");
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND TransactionDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND TransactionDate <= '{0}'", toDate);
            if (!string.IsNullOrEmpty(paymentType))
                queryToUse += string.Format(" AND PaymentType = '{0}'", paymentType);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable SearchPaymentInvest(string fromDate, string toDate, string paymentType)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_PaymentInvest Where Amount > 0 ");
            if (!string.IsNullOrEmpty(fromDate))
                queryToUse += string.Format(" AND TransactionDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND TransactionDate <= '{0}'", toDate);
            if (!string.IsNullOrEmpty(paymentType))
                queryToUse += string.Format(" AND PaymentType = '{0}'", paymentType);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable SearchSalesPerson(string name, string contact, string email, string address)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_SalePerson Where IsActive=1");
            if (!string.IsNullOrEmpty(name))
                queryToUse += string.Format(" AND Name like '%{0}%'", name);
            if (!string.IsNullOrEmpty(contact))
                queryToUse += string.Format(" AND Contact like '%{0}%'", contact);
            if (!string.IsNullOrEmpty(email))
                queryToUse += string.Format(" AND Email like '%{0}%'", email);
            if (!string.IsNullOrEmpty(address))
                queryToUse += string.Format(" AND Address like '%{0}%'", address);
            return sQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }
        #endregion
    }
}
