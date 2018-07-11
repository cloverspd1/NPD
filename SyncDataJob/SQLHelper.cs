namespace SyncDataJob
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Web;
    using System.Xml;

    /// <summary>
    /// SQL Helper
    /// </summary>
    public sealed class SqlHelper
    {
        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <param name="connectionName">Name of the connection.</param>
        /// <returns>connection string</returns>
        public static string GetConnectionString(string connectionName)
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
        }

        #region "private utility methods & constructors"
        /// <summary>
        /// Attaches the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <exception cref="System.ArgumentNullException">command</exception>
        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            try
            {
                if ((command == null)) throw new ArgumentNullException("command");
                if (((commandParameters != null)))
                {
                    foreach (SqlParameter p in commandParameters)
                    {
                        if (((p != null)))
                        {
                            if ((p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Input) && p.Value == null)
                            {
                                p.Value = DBNull.Value;
                            }
                            if (p.Value == null)
                            {
                                if (p.SqlDbType == SqlDbType.Int)
                                {
                                    p.Value = 0;
                                }
                                else
                                {
                                    p.Value = " ";
                                }
                            }
                            command.Parameters.Add(p);
                        }
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Assigns the parameter values.
        /// </summary>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="dataRow">The data row.</param>
        /// <exception cref="System.Exception"></exception>
        private static void AssignParameterValues(SqlParameter[] commandParameters, DataRow dataRow)
        {
            try
            {
                if (commandParameters == null || dataRow == null)
                {
                    return;
                }
                int i = 0;
                foreach (SqlParameter commandParameter in commandParameters)
                {
                    if ((commandParameter.ParameterName == null || commandParameter.ParameterName.Length <= 1))
                    {
                        throw new Exception(string.Format("Please provide a valid parameter name on the parameter #{0}, the ParameterName property has the following value: ' {1}' .", i, commandParameter.ParameterName));
                    }
                    if (dataRow.Table.Columns.IndexOf(commandParameter.ParameterName.Substring(1)) != -1)
                    {
                        commandParameter.Value = dataRow[commandParameter.ParameterName.Substring(1)];
                    }
                    i = i + 1;
                }
            }
            catch { }
        }

        /// <summary>
        /// Assigns the parameter values.
        /// </summary>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <exception cref="System.ArgumentException">Parameter count does not match Parameter Value count.</exception>
        private static void AssignParameterValues(SqlParameter[] commandParameters, object[] parameterValues)
        {
            int i = 0;
            int j = 0;
            try
            {
                if ((commandParameters == null) && (parameterValues == null))
                {
                    return;
                }
                if (commandParameters.Length != parameterValues.Length)
                {
                    throw new ArgumentException("Parameter count does not match Parameter Value count.");
                }
                j = commandParameters.Length - 1;
                for (i = 0; i <= j; i++)
                {
                    if (parameterValues[i] is IDbDataParameter)
                    {
                        IDbDataParameter paramInstance = (IDbDataParameter)parameterValues[i];
                        if ((paramInstance.Value == null))
                        {
                            commandParameters[i].Value = DBNull.Value;
                        }
                        else
                        {
                            commandParameters[i].Value = paramInstance.Value;
                        }
                    }
                    else if ((parameterValues[i] == null))
                    {
                        commandParameters[i].Value = DBNull.Value;
                    }
                    else
                    {
                        commandParameters[i].Value = parameterValues[i];
                    }
                }
            }
            catch { }
        }

        /// <summary>
        /// Prepares the command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="mustCloseConnection">if set to <c>true</c> [must close connection].</param>
        /// <exception cref="System.ArgumentNullException">
        /// command
        /// or
        /// commandText
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, ref bool mustCloseConnection)
        {
            try
            {
                if ((command == null)) throw new ArgumentNullException("command");
                if ((commandText == null || commandText.Length == 0)) throw new ArgumentNullException("commandText");
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                    mustCloseConnection = true;
                }
                else
                {
                    mustCloseConnection = false;
                }
                command.Connection = connection;
                command.CommandText = commandText;
                if ((transaction != null))
                {
                    if (transaction.Connection == null) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                    command.Transaction = transaction;
                }
                command.CommandType = commandType;
                if ((commandParameters != null))
                {
                    AttachParameters(command, commandParameters);
                }
                return;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        #endregion

        #region "ExecuteNonQuery"
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            SqlConnection connection =
            default(SqlConnection);
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return ExecuteNonQuery(connection, commandType, commandText, commandParameters);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="paramList">The parameter list.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(string connectionString, string spName, SqlParameter[] paramList)
        {
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if ((paramList != null) && paramList.Length > 0)
                {
                    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, paramList);
                }
                else
                {
                    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }

        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
        {
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
        {
            try
            {
                return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "i"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                SqlCommand cmd = new SqlCommand();
                int retval = 0;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);
                int i = cmd.Parameters.Count;
                retval = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                if ((mustCloseConnection)) connection.Close();
                return retval;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
        {
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            try
            {
                return ExecuteNonQuery(transaction, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                SqlCommand cmd = new SqlCommand();
                int retval = 0;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
                retval = cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
                return retval;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        /// <summary>
        /// Executes the non query.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                return 0;
            }
        }
        #endregion

        #region "ExecuteDataset"
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            SqlConnection connection =
            default(SqlConnection);
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteDataset(connection, commandType, commandText, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;
            PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);
            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if (((dataAdatpter != null))) dataAdatpter.Dispose();
            }
            if ((mustCloseConnection)) connection.Close();
            return ds;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            SqlCommand cmd = new SqlCommand();
            DataSet ds = new DataSet();
            SqlDataAdapter dataAdatpter = null;
            bool mustCloseConnection = false;
            PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
            try
            {
                dataAdatpter = new SqlDataAdapter(cmd);
                dataAdatpter.Fill(ds);
                cmd.Parameters.Clear();
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if (((dataAdatpter != null))) dataAdatpter.Dispose();
            }
            return ds;
        }
        /// <summary>
        /// Executes the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteReader"
        /// <summary>
        /// 
        /// </summary>
        private enum SqlConnectionOwnership
        {
            /// <summary>
            /// The internal
            /// </summary>
            Internal, External
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="connectionOwnership">The connection ownership.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
        {
            if ((connection == null)) throw new ArgumentNullException("connection");
            bool mustCloseConnection = false;
            SqlCommand cmd = new SqlCommand();
            try
            {
                SqlDataReader dataReader =
                default(SqlDataReader);
                PrepareCommand(cmd, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
                if (connectionOwnership == SqlConnectionOwnership.External)
                {
                    dataReader = cmd.ExecuteReader();
                }
                else
                {
                    dataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
                bool canClear = true;
                foreach (SqlParameter commandParameter in cmd.Parameters)
                {
                    if (commandParameter.Direction != ParameterDirection.Input)
                    {
                        canClear = false;
                    }
                }
                if ((canClear)) cmd.Parameters.Clear();
                return dataReader;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                if ((mustCloseConnection)) connection.Close();
                throw;
            }
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(Convert.ToString(ex));
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
                if ((connection != null)) connection.Dispose();
                throw;
            }
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                return ExecuteReader(connection, (SqlTransaction)null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(Convert.ToString(ex));
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteReader(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            if ((transaction == null)) throw new ArgumentNullException("transaction");
            if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
            return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
        }
        /// <summary>
        /// Executes the reader.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteScalar"
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            object functionReturnValue = null;
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            functionReturnValue = null;
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                return ExecuteScalar(connection, commandType, commandText, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                SqlCommand cmd = new SqlCommand();
                object retval = null;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, connection, (SqlTransaction)null, commandType, commandText, commandParameters, ref mustCloseConnection);
                retval = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                if ((mustCloseConnection)) connection.Close();
                return retval;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                SqlCommand cmd = new SqlCommand();
                object retval = null;
                bool mustCloseConnection = false;
                PrepareCommand(cmd, transaction.Connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
                retval = cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                return retval;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteXmlReader"
        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                return ExecuteXmlReader(connection, commandType.ToString(), commandText, (SqlParameter[])null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteXmlReader(connection, Convert.ToString(CommandType.StoredProcedure), spName, commandParameters);
                }
                else
                {
                    return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                return ExecuteXmlReader(transaction, commandType, commandText);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the XML reader.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlParameter[] commandParameters = null;
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    return ExecuteXmlReader(transaction, Convert.ToString(CommandType.StoredProcedure), spName, commandParameters);
                }
                else
                {
                    return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        #endregion

        #region "FillDataset"
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// dataSet
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(string connectionString, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            if ((dataSet == null)) throw new ArgumentNullException("dataSet");
            SqlConnection connection =
            default(SqlConnection);
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                FillDataset(connection, commandType, commandText, dataSet, tableNames, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// dataSet
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(string connectionString, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            if ((dataSet == null)) throw new ArgumentNullException("dataSet");
            SqlConnection connection =
            default(SqlConnection);
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                FillDataset(connection, spName, dataSet, tableNames, parameterValues);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            try
            {
                FillDataset(connection, commandType, commandText, dataSet, tableNames, null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="commandParameters">The command parameters.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlConnection connection, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            try
            {
                FillDataset(connection, null, commandType, commandText, dataSet, tableNames, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// dataSet
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlConnection connection, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((dataSet == null)) throw new ArgumentNullException("dataSet");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
                }
                else
                {
                    FillDataset(connection, CommandType.StoredProcedure, spName, dataSet, tableNames);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames)
        {
            try
            {
                FillDataset(transaction, commandType, commandText, dataSet, tableNames, null);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <exception cref="System.ArgumentNullException">transaction</exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                FillDataset(transaction.Connection, transaction, commandType, commandText, dataSet, tableNames, commandParameters);
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// dataSet
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void FillDataset(SqlTransaction transaction, string spName, DataSet dataSet, string[] tableNames, params object[] parameterValues)
        {
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((dataSet == null)) throw new ArgumentNullException("dataSet");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if ((parameterValues != null) && parameterValues.Length > 0)
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, parameterValues);
                    FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames, commandParameters);
                }
                else
                {
                    FillDataset(transaction, CommandType.StoredProcedure, spName, dataSet, tableNames);
                }
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
        }
        /// <summary>
        /// Fills the dataset.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="transaction">The transaction.</param>
        /// <param name="commandType">Type of the command.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableNames">The table names.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// dataSet
        /// </exception>
        /// <exception cref="System.ArgumentException">The tableNames parameter must contain a list of tables, a value was provided as null or empty string.;tableNames</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private static void FillDataset(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, DataSet dataSet, string[] tableNames, params SqlParameter[] commandParameters)
        {
            if ((connection == null)) throw new ArgumentNullException("connection");
            if ((dataSet == null)) throw new ArgumentNullException("dataSet");
            SqlCommand command = new SqlCommand();
            bool mustCloseConnection = false;
            PrepareCommand(command, connection, transaction, commandType, commandText, commandParameters, ref mustCloseConnection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);
            try
            {
                if ((tableNames != null) && tableNames.Length > 0)
                {
                    string tableName = "Table";
                    int index = 0;
                    for (index = 0; index <= tableNames.Length - 1; index++)
                    {
                        if ((tableNames[index] == null || tableNames[index].Length == 0)) throw new ArgumentException("The tableNames parameter must contain a list of tables, a value was provided as null or empty string.", "tableNames");
                        dataAdapter.TableMappings.Add(tableName, tableNames[index]);
                        tableName = tableName + (index + 1).ToString();
                    }
                }
                dataAdapter.Fill(dataSet);
                command.Parameters.Clear();
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if (((dataAdapter != null))) dataAdapter.Dispose();
            }
            if ((mustCloseConnection)) connection.Close();
        }
        #endregion

        #region "UpdateDataset"
        /// <summary>
        /// Updates the dataset.
        /// </summary>
        /// <param name="insertCommand">The insert command.</param>
        /// <param name="deleteCommand">The delete command.</param>
        /// <param name="updateCommand">The update command.</param>
        /// <param name="dataSet">The data set.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <exception cref="System.ArgumentNullException">
        /// insertCommand
        /// or
        /// deleteCommand
        /// or
        /// updateCommand
        /// or
        /// dataSet
        /// or
        /// tableName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err")]
        public static void UpdateDataset(SqlCommand insertCommand, SqlCommand deleteCommand, SqlCommand updateCommand, DataSet dataSet, string tableName)
        {
            if ((insertCommand == null)) throw new ArgumentNullException("insertCommand");
            if ((deleteCommand == null)) throw new ArgumentNullException("deleteCommand");
            if ((updateCommand == null)) throw new ArgumentNullException("updateCommand");
            if ((dataSet == null)) throw new ArgumentNullException("dataSet");
            if ((tableName == null || tableName.Length == 0)) throw new ArgumentNullException("tableName");
            SqlDataAdapter dataAdapter = new SqlDataAdapter();
            try
            {
                dataAdapter.UpdateCommand = updateCommand;
                dataAdapter.InsertCommand = insertCommand;
                dataAdapter.DeleteCommand = deleteCommand;
                dataAdapter.Update(dataSet, tableName);
                dataSet.AcceptChanges();
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            finally
            {
                if (((dataAdapter != null))) dataAdapter.Dispose();
            }
        }
        #endregion

        #region "CreateCommand"
        /// <summary>
        /// Creates the command.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="sourceColumns">The source columns.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "err"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        public static SqlCommand CreateCommand(SqlConnection connection, string spName, params string[] sourceColumns)
        {
            SqlCommand functionReturnValue =
            default(SqlCommand);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                if ((sourceColumns != null) && sourceColumns.Length > 0)
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    int index = 0;
                    for (index = 0; index <= sourceColumns.Length - 1; index++)
                    {
                        commandParameters[index].SourceColumn = sourceColumns[index];
                    }
                    AttachParameters(cmd, commandParameters);
                }
                functionReturnValue = cmd;
            }
            catch
            {
                //Utility.ErrorLog("EC.Timesheet.SQLHelper", Error: " + err.Message);
            }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteNonQueryTypedParams"
        /// <summary>
        /// Executes the non query typed parameters.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        public static int ExecuteNonQueryTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            int functionReturnValue = 0;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the non query typed parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        public static int ExecuteNonQueryTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            int functionReturnValue = 0;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the non query typed parameters.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static int ExecuteNonQueryTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            int functionReturnValue = 0;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteDatasetTypedParams"
        /// <summary>
        /// Executes the dataset typed parameters.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        public static DataSet ExecuteDatasetTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset typed parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        public static DataSet ExecuteDatasetTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteDataset(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the dataset typed parameters.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static DataSet ExecuteDatasetTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            DataSet functionReturnValue =
            default(DataSet);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteReaderTypedParams"
        /// <summary>
        /// Executes the reader typed parameters.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        public static SqlDataReader ExecuteReaderTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader typed parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        public static SqlDataReader ExecuteReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteReader(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the reader typed parameters.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static SqlDataReader ExecuteReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            SqlDataReader functionReturnValue =
            default(SqlDataReader);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteReader(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteScalarTypedParams"
        /// <summary>
        /// Executes the scalar typed parameters.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// spName
        /// </exception>
        public static object ExecuteScalarTypedParams(string connectionString, string spName, DataRow dataRow)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connectionString, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar typed parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        public static object ExecuteScalarTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteScalar(connection, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the scalar typed parameters.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static object ExecuteScalarTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            object functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName, commandParameters);
                }
                else
                {
                    functionReturnValue = SqlHelper.ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "ExecuteXmlReaderTypedParams"
        /// <summary>
        /// Executes the XML reader typed parameters.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        public static XmlReader ExecuteXmlReaderTypedParams(SqlConnection connection, string spName, DataRow dataRow)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = ExecuteXmlReader(connection, Convert.ToString(CommandType.StoredProcedure), spName, commandParameters);
                }
                else
                {
                    functionReturnValue = ExecuteXmlReader(connection, Convert.ToString(CommandType.StoredProcedure), spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Executes the XML reader typed parameters.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// transaction
        /// or
        /// spName
        /// </exception>
        /// <exception cref="System.ArgumentException">The transaction was rollbacked or commited, please provide an open transaction.;transaction</exception>
        public static XmlReader ExecuteXmlReaderTypedParams(SqlTransaction transaction, string spName, DataRow dataRow)
        {
            XmlReader functionReturnValue =
            default(XmlReader);
            functionReturnValue = null;
            try
            {
                if ((transaction == null)) throw new ArgumentNullException("transaction");
                if ((transaction != null) && (transaction.Connection == null)) throw new ArgumentException("The transaction was rollbacked or commited, please provide an open transaction.", "transaction");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                if (((dataRow != null) && dataRow.ItemArray.Length > 0))
                {
                    SqlParameter[] commandParameters = ODBCHelperParameterCache.GetSpParameterSet(transaction.Connection, spName);
                    AssignParameterValues(commandParameters, dataRow);
                    functionReturnValue = ExecuteXmlReader(transaction, Convert.ToString(CommandType.StoredProcedure), spName, commandParameters);
                }
                else
                {
                    functionReturnValue = ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
                }
            }
            catch { }
            return functionReturnValue;
        }
    }
        #endregion

    /// <summary>
    /// 
    /// </summary>
    public sealed class ODBCHelperParameterCache
    {
        #region "private methods, variables, and constructors"
        /// <summary>
        /// Prevents a default instance of the <see cref="ODBCHelperParameterCache"/> class from being created.
        /// </summary>
        private ODBCHelperParameterCache() { }
        /// <summary>
        /// The parameter cache
        /// </summary>
        private static Hashtable paramCache = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// Discovers the sp parameter set.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
        /// <param name="parameterValues">The parameter values.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities")]
        private static SqlParameter[] DiscoverSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter, params object[] parameterValues)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                SqlCommand cmd = new SqlCommand(spName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter[] discoveredParameters = null;
                connection.Open();
                SqlCommandBuilder.DeriveParameters(cmd);
                connection.Close();
                if (!includeReturnValueParameter)
                {
                    cmd.Parameters.RemoveAt(0);
                }
                discoveredParameters = new SqlParameter[cmd.Parameters.Count];
                cmd.Parameters.CopyTo(discoveredParameters, 0);
                foreach (SqlParameter discoveredParameter in discoveredParameters)
                {
                    discoveredParameter.Value = DBNull.Value;
                }
                return discoveredParameters;
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Clones the parameters.
        /// </summary>
        /// <param name="originalParameters">The original parameters.</param>
        /// <returns></returns>
        private static SqlParameter[] CloneParameters(SqlParameter[] originalParameters)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                int i = 0;
                int j = originalParameters.Length - 1;
                SqlParameter[] clonedParameters = new SqlParameter[j + 1];
                for (i = 0; i <= j; i++)
                {
                    clonedParameters[i] = (SqlParameter)((ICloneable)originalParameters[i]).Clone();
                }
                return clonedParameters;
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "caching functions"
        /// <summary>
        /// Caches the parameter set.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// commandText
        /// </exception>
        public static void CacheParameterSet(string connectionString, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((commandText == null || commandText.Length == 0)) throw new ArgumentNullException("commandText");
                string hashKey = connectionString + ":" + commandText;
                paramCache[hashKey] = commandParameters;
            }
            catch { }
        }
        /// <summary>
        /// Gets the cached parameter set.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="commandText">The command text.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connectionString
        /// or
        /// commandText
        /// </exception>
        public static SqlParameter[] GetCachedParameterSet(string connectionString, string commandText)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
                if ((commandText == null || commandText.Length == 0)) throw new ArgumentNullException("commandText");
                string hashKey = connectionString + ":" + commandText;
                SqlParameter[] cachedParameters = (SqlParameter[])paramCache[hashKey];
                if (cachedParameters == null)
                {
                    return null;
                }
                else
                {
                    return CloneParameters(cachedParameters);
                }
            }
            catch { }
            return functionReturnValue;
        }
        #endregion

        #region "Parameter Discovery Functions"
        /// <summary>
        /// Gets the sp parameter set.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                return GetSpParameterSet(connectionString, spName, false);
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Gets the sp parameter set.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connectionString</exception>
        public static SqlParameter[] GetSpParameterSet(string connectionString, string spName, bool includeReturnValueParameter)
        {
            SqlParameter[] functionReturnValue = null;
            if ((connectionString == null || connectionString.Length == 0)) throw new ArgumentNullException("connectionString");
            SqlConnection connection =
            default(SqlConnection);
            connection = null;
            try
            {
                connection = new SqlConnection(connectionString);
                functionReturnValue = GetSpParameterSetInternal(connection, spName, includeReturnValueParameter);
            }
            finally
            {
                if ((connection != null)) connection.Dispose();
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Gets the sp parameter set.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <returns></returns>
        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                functionReturnValue = GetSpParameterSet(connection, spName, false);
            }
            catch { }
            return functionReturnValue;
        }
        /// <summary>
        /// Gets the sp parameter set.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">connection</exception>
        public static SqlParameter[] GetSpParameterSet(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            if ((connection == null)) throw new ArgumentNullException("connection");
            SqlConnection clonedConnection =
            default(SqlConnection);
            clonedConnection = null;
            try
            {
                clonedConnection = (SqlConnection)(((ICloneable)connection).Clone());
                functionReturnValue = GetSpParameterSetInternal(clonedConnection, spName, includeReturnValueParameter);
            }
            finally
            {
                if ((clonedConnection != null)) clonedConnection.Dispose();
            }
            return functionReturnValue;
        }
        /// <summary>
        /// Gets the sp parameter set internal.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="spName">Name of the sp.</param>
        /// <param name="includeReturnValueParameter">if set to <c>true</c> [include return value parameter].</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">
        /// connection
        /// or
        /// spName
        /// </exception>
        private static SqlParameter[] GetSpParameterSetInternal(SqlConnection connection, string spName, bool includeReturnValueParameter)
        {
            SqlParameter[] functionReturnValue = null;
            functionReturnValue = null;
            try
            {
                if ((connection == null)) throw new ArgumentNullException("connection");
                SqlParameter[] cachedParameters = null;
                string hashKey = null;
                if ((spName == null || spName.Length == 0)) throw new ArgumentNullException("spName");
                hashKey = connection.ConnectionString + ":" + spName + (includeReturnValueParameter == true ? ":include ReturnValue Parameter" : "").ToString();
                cachedParameters = (SqlParameter[])paramCache[hashKey];
                if ((cachedParameters == null))
                {
                    SqlParameter[] spParameters = DiscoverSpParameterSet(connection, spName, includeReturnValueParameter);
                    paramCache[hashKey] = spParameters;
                    cachedParameters = spParameters;
                }
                return CloneParameters(cachedParameters);
            }
            catch { }
            return functionReturnValue;
        }
    }
        #endregion
}