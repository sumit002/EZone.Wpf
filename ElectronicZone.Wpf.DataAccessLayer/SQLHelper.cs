using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace ElectronicZone.Wpf.DataAccessLayer
{
    public class SQLHelper : SqlDataAccess, IDisposable
    {
        #region Properties
        SQLiteConnection con = null;
        IDbTransaction trans = null;
        #endregion
        /// <summary>
        /// SQLHelper Default Constructor
        /// </summary>
        public SQLHelper()
        {
            this.con = new SQLiteConnection(m_connectionString);
            con.Open();
        }

        private void BeginTransaction()
        {
            if (trans == null)
                trans = con.BeginTransaction();
        }

        public void CommitTransaction()
        {
            trans.Commit();
            trans = null;
        }

        public void RollbackTransaction()
        {
            trans.Rollback();
            trans = null;
        }

        public void Dispose()
        {
            if (trans != null) CommitTransaction();
            con.Close();
        }

        #region Methods
        public int InsertOrUpdateTable(string queryToUse, List<SQLiteParameter> dbParameterList)
        {
            //using (SQLiteConnection con = new SQLiteConnection(SqlDataAccess.m_connectionString))
            {
                //con.Open();
                BeginTransaction();
                using (SQLiteCommand insUpdQuery = new SQLiteCommand(queryToUse, con))
                {
                    if (dbParameterList != null)
                        insUpdQuery.Parameters.AddRange(dbParameterList.ToArray());
                    int result = insUpdQuery.ExecuteNonQuery();
                    insUpdQuery.Parameters.Clear();
                    return result;
                }
            }
        }

        public int getLastRowId(string tableName)
        {
            int lastID = 0;
            //string queryToUse = "SELECT last_insert_rowid()";
            string queryToUse = string.Format("SELECT Id FROM {0} ORDER BY Id DESC LIMIT 1", tableName);
            DataTable dt = GetSelectedValue(queryToUse, null);
            lastID = int.Parse(dt.Rows[0][0].ToString());
            return lastID;
        }

        public int InsertOrUpdate(string insertQuery, string updateQuery, string existanceTestQuery, List<SQLiteParameter> dbParameterList)
        {
            string queryToUse = string.Empty;
            //using (SQLiteConnection con = new SQLiteConnection(m_connectionString))
            //{
            //con.Open();
            BeginTransaction();
                using (SQLiteCommand selCmd = new SQLiteCommand(existanceTestQuery, con))
                {
                    selCmd.Parameters.AddRange(dbParameterList.ToArray());
                    object reslt = selCmd.ExecuteScalar();
                    if (reslt == null)
                    {
                        queryToUse = insertQuery;
                    }
                    else
                    {
                        queryToUse = updateQuery;
                    }
                    selCmd.Parameters.Clear();
                }
            //}
            return InsertOrUpdateTable(queryToUse, dbParameterList);
        }

        public int DeleteFromTable(string queryToUse, List<SQLiteParameter> dbParameterList)
        {
            //using (SQLiteConnection con = new SQLiteConnection(m_connectionString))
            {
                //con.Open();
                BeginTransaction();

                using (SQLiteCommand deleteQuery = new SQLiteCommand(queryToUse, con))
                {
                    deleteQuery.Parameters.AddRange(dbParameterList.ToArray());
                    int result = deleteQuery.ExecuteNonQuery();
                    deleteQuery.Parameters.Clear();
                    return result;
                }
            }
        }

        public DataTable GetSelectedValue(string query, List<SQLiteParameter> dbParameterList)
        {
            //using (SQLiteConnection con = new SQLiteConnection(m_connectionString))
            {
                //con.Open();
                using (SQLiteCommand selCmd = new SQLiteCommand(query, con))
                {
                    if (dbParameterList != null)
                        selCmd.Parameters.AddRange(dbParameterList.ToArray());
                    using (SQLiteDataAdapter da = new SQLiteDataAdapter(selCmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        selCmd.Parameters.Clear();
                        return dt;
                    }
                }
            }
        }

        public int InsertTable(string insertQuery, string existanceQuery, List<SQLiteParameter> dbParameterList)
        {
            string queryToUse = string.Empty;
            //using (SQLiteConnection con = new SQLiteConnection(m_connectionString))
            BeginTransaction();
            {
                //con.Open();
                using (SQLiteCommand selCmd = new SQLiteCommand(existanceQuery, con))
                {
                    selCmd.Parameters.AddRange(dbParameterList.ToArray());
                    object reslt = selCmd.ExecuteScalar();
                    selCmd.Parameters.Clear();
                    if (reslt == null)
                    {
                        return InsertOrUpdateTable(insertQuery, dbParameterList);
                    }
                    else
                    {
                        return 90;
                    }
                }
            }
        }
        #endregion
    }
}
