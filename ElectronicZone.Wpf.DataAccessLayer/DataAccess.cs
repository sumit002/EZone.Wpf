using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SQLite;

namespace ElectronicZone.Wpf.DataAccessLayer
{
    public class DataAccess
    {
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public bool IfExistsValue(string tableName, string columnName, string value)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("Select * from {1} Where {0}='{2}' COLLATE NOCASE", columnName, tableName, value);

            DataTable dt = SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
        }

        public DataTable GetAllProducts()
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT Id, Name, Description, CreatedDate FROM tblProductMaster WHERE IsActive = 1");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public int DeleteProduct(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblProductMaster WHERE Id = @Id");
            return SQLHelper.DeleteFromTable(queryToUse, dbParameterList);
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
            return SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
        }

        public DataTable GetAllBrands()
        {
            string queryToUse = string.Format("SELECT Id, Name, Description, CreatedDate, ModifiedDate FROM tblBrandMaster WHERE IsActive = 1");
            return SQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteBrand(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblBrandMaster WHERE Id = @Id");
            return SQLHelper.DeleteFromTable(queryToUse, dbParameterList);
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
            string updateQuery = string.Format("UPDATE {0} SET ProductCode=@ProductCode, StockCode=@StockCode, ItemDesc=@ItemDesc, AvlQuantity=@AvlQuantity, PurchasePrice=@PurchasePrice, SalePrice=@SalePrice, ProductImage=@ProductImage, PurchaseDate=@PurchaseDate, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            int result = SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = SQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public int UpdateStockQuantity(Dictionary<string, string> columns, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            foreach (string col in columns.Keys)
            {
                dbParameterList.Add(new SQLiteParameter("@" + col, columns[col]));
            }
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string updateQuery = string.Format("UPDATE {0} SET AvlQuantity=AvlQuantity-@AvlQuantity, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            return SQLHelper.InsertOrUpdate(string.Empty, updateQuery, existanceTestQuery, dbParameterList);
        }

        public int UpdateStockImage(byte[] pImage, int pId, string tableName)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@ID", pId));
            dbParameterList.Add(new SQLiteParameter("@ProductImage", pImage));
            string existanceTestQuery = string.Format("SELECT 1 FROM {0} WHERE Id = @id", tableName);
            string updateQuery = string.Format("UPDATE {0} SET ProductImage=@ProductImage WHERE ID=@ID", tableName);
            return SQLHelper.InsertOrUpdate(string.Empty, updateQuery, existanceTestQuery, dbParameterList);
        }

        public DataTable GetAllStocks()
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_StockMaster");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
        /// <returns></returns>
        public DataTable SearchStocks(string productId, string brandId, string productCode, string stockCode, int? priceMin, int? priceMax, string fromDate, string toDate, bool? avlQty = false)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //dbParameterList.Add(new SQLiteParameter("@ProjectId", id));
            string queryToUse = string.Format("SELECT * FROM vw_StockMaster Where Quantity>0 ");
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
                queryToUse += string.Format(" AND PurchaseDate >= '{0}'", fromDate);
            if (!string.IsNullOrEmpty(toDate))
                queryToUse += string.Format(" AND PurchaseDate <= '{0}'", toDate);
            if (avlQty.Value)
                queryToUse += string.Format(" AND AvlQuantity>=1");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            string updateQuery = string.Format("UPDATE {0} SET Name=@Name, Description=@Description, ModifiedDate=@ModifiedDate WHERE ID=@ID", tableName);
            int rslt = SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = SQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public DataTable GetAllSales(string id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            if (!string.IsNullOrEmpty(id))
            {
                dbParameterList.Add(new SQLiteParameter("@Id", id));
            }
            string queryToUse = string.Format("SELECT * FROM vw_SaleMaster");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }

        public DataTable GetSaleInvoice(string SalesId)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            if (!string.IsNullOrEmpty(SalesId))
            {
                dbParameterList.Add(new SQLiteParameter("@SalesId", SalesId));
            }
            string queryToUse = string.Format("SELECT * FROM vw_SaleMaster Where SalesId=@SalesId");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.InsertOrUpdate(insertQuery, string.Empty, string.Empty, dbParameterList);
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

            int res = SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = SQLHelper.getLastRowId(tableName);
            return rowId;
        }

        public DataTable GetAllSalesPerson()
        {
            string queryToUse = string.Format("SELECT * FROM tblSalePerson WHERE IsActive = 1");
            return SQLHelper.GetSelectedValue(queryToUse, null);
        }

        public int DeleteSalesPerson(int Id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            dbParameterList.Add(new SQLiteParameter("@Id", Id));
            string queryToUse = string.Format("Delete FROM tblSalePerson WHERE Id = @Id");
            return SQLHelper.DeleteFromTable(queryToUse, dbParameterList);
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
        //    return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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

            int res = SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            int rowId = SQLHelper.getLastRowId(tableName);
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
        //    return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        //}

        public int InsertOrUpdateSupportPaymentMaster(Dictionary<string, string> columns, string tableName)
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
            string updateQuery = string.Format("UPDATE {0} SET Description=@Description, Amount=@Amount, SupportDate=@SupportDate WHERE ID=@ID", tableName);

            int res = SQLHelper.InsertOrUpdate(insertQuery, updateQuery, existanceTestQuery, dbParameterList);
            //int rowId = SQLHelper.getLastRowId(tableName);
            return res;
        }

        public DataTable GetAllSupportPayment(string id)
        {
            List<SQLiteParameter> dbParameterList = new List<SQLiteParameter>();
            //construct parameter
            if (!string.IsNullOrEmpty(id))
            {
                dbParameterList.Add(new SQLiteParameter("@Id", id));
            }

            string queryToUse = string.Format("SELECT Id, Description, Amount, SupportDate FROM tblSupportPaymentMaster WHERE IsActive=1");
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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

            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            string queryToUse = string.Format("SELECT * FROM vw_SaleMaster Where Quantity>0 ");
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
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
            return SQLHelper.GetSelectedValue(queryToUse, dbParameterList);
        }
        #endregion
    }
}
