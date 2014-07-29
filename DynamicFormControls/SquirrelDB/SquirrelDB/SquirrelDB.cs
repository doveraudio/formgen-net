using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace Squirrel
{
    /// <summary>
    /// SquirrelDB :
    /// A simple wrapper to update offline databases from an online source.
    /// </summary>
    public class SquirrelDB
    {
        private bool debug = false;

        private string _basepath;

        private List<string> _columns;

        private string _creationString;

        private string _database;

        private string _extension;

        private string _host;

        private string _log = "";

        private string _mysqlConnectionstring;

        private string _password;

        private List<string> _primaryKey;
        private string _sql;

        private string _sqliteConnectionstring;

        private string _table;

        private string _username;
        private List<string> _vartypes;
        private DataTable dTable;
        /*
         * 
         * SQLite variables
         * 
         */
        
        public DataSet _dbDataSet;
        public DbDataAdapter liteAdapter;
        public DbCommandBuilder liteBuilder;
        public DbCommand liteCommand;
        public DbConnection liteConnection;

        public DbProviderFactory liteFac;
        /*
         * MySQL variables
         */
        public DbDataAdapter myAdapter;
        public DbCommandBuilder myBuilder;
        public DbCommand myCommand;
        public DbConnection myConnection;

        public DbProviderFactory myFactory;

        public SquirrelDB()
        {
            _columns = new List<string>();
            _vartypes = new List<string>();
            _primaryKey = new List<string>();
            _dbDataSet = new DataSet();
        }

        private bool addTableToSet(DataTable table)
        {
            try
            {
                _dbDataSet.Tables.Add(table);
                return true;
            }
            catch (Exception ex)
            {
                Log += String.Format("\r\nFailed to add table{0} to dataset. Exception: {1}", table.TableName, ex.Message);
                return false;
            }
        }

        private bool removeTableFromSet(DataTable table)
        {
            try { _dbDataSet.Tables.Remove(table); return true; }
            catch (Exception ex) { Log += String.Format("\r\nFailed to remove table {0}. Exception {1}", table.TableName, ex.Message); return false; }
        }

        public DataSet DbDataset
        { get { return _dbDataSet; } set { _dbDataSet = value; } }

        public string Basepath
        { get { return _basepath; } set { _basepath = value; } }

        public List<string> Columns
        { get { return _columns; } set { _columns = value; } }

        public string CreationString
        { get { return _creationString; } set { _creationString = value; } }

        public string Database
        { get { return _database; } set { _database = value; } }

        public string Extension
        { get { return _extension; } set { _extension = value; } }

        public string Host
        { get { return _host; } set { _host = value; } }

        public string Log
        { get { return _log; } set { if (debug) { _log += "\r\n" + value; } } }

        public string MySqlConnectionString
        {
            get
            {
                if (_mysqlConnectionstring == "")
                {
                }
                return _mysqlConnectionstring;
            }
            set
            {
                _mysqlConnectionstring = value;
            }
        }

        public string Password
        { get { return _password; } set { _password = value; } }

        public List<string> PrimaryKey
        { get { return _primaryKey; } set { _primaryKey = value; } }

        public string SQL
        { get { return _sql; } set { _sql = value; } }

        public string SQLiteConnectionString
        { get { return _sqliteConnectionstring; } set { _sqliteConnectionstring = value; } }

        public string Table
        {
            get { return _table; }
            set
            {
                _columns = new List<string>();
                _vartypes = new List<string>();
                _primaryKey = new List<string>();
                _table = value;
            }
        }

        public string Username
        { get { return _username; } set { _username = value; } }

        public List<string> VarTypes
        { get { return _vartypes; } set { _vartypes = value; } }

        public void addColumn(string column)
        {
            _columns.Add(column);
        }

        public void addPrimaryKey(string column)
        {
            _primaryKey.Add(column);
        }

        public void addVartype(string vartype)
        {
            _vartypes.Add(vartype);
        }

        public void connectFactories()
        {
            myConnection = myFactory.CreateConnection();
            liteConnection = liteFac.CreateConnection();
        }
        public void connectSqLite()
        {
            liteConnection = liteFac.CreateConnection();
        }
        /// <summary>
        /// Converts a .net datatype in string form to a SQLite variant type.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string convertDataTypeSQLite(string value)
        {
            switch (value.ToLower())
            {
                case ("bool"):
                    return "integer";

                case ("byte"):
                    return "text";

                case ("sbyte"):
                    return "text";

                case ("char"):
                    return "text";

                case ("decimal"):
                    return "real";

                case ("double"):
                    return "real";

                case ("float"):
                    return "real";

                case ("int"):
                    return "integer";

                case ("uint"):
                    return "integer";

                case ("long"):
                    return "numeric";

                case ("ulong"):
                    return "numeric";

                case ("object"):
                    return "blob";

                case ("short"):
                    return "integer";

                case ("ushort"):
                    return "short";

                case ("string"):

                    return "text";

                default:
                    return "text";
            }
        }

        public bool createLocalDatabase()
        {
            liteConnection.ConnectionString = String.Format("DataSource={0}{1}.{2};", _basepath, _database, _extension);
            try
            {
                if (System.IO.File.Exists(_basepath + _database + "." + _extension))
                {
                    return false;
                }
                else
                {
                    SQLiteConnection.CreateFile(String.Format("{0}{1}.{2}", _basepath, _database, _extension));
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log += ("File Creation Failed! \r\n" + ex.Message);
                return false;
            }
        }
        public bool createRemoteDatabase()
        {
            DbCommand myCommand = myFactory.CreateCommand();
            myConnection.ConnectionString = String.Format("Host={0}; Uid={1}; Pwd={2}; Allow Zero DateTime=true;", _host, _username, _password);
            myCommand.CommandText = String.Format("CREATE DATABASE {0};", _database);
            myCommand.Connection = myConnection;
            try
            {
                myConnection.Open();
                myCommand.ExecuteNonQuery();
                myConnection.Close();
                
            }
            catch (Exception ex)
            {
                Log = ex.Message;
                return false;
            }

            return true;
        
        }
        /// <summary>
        /// Creates a table in the local database using the specified columns, vartypes, and constraints. Specify primary key in the List of String with the primary key.
        /// </summary>
        public bool createLocalTable()
        {
            _creationString = "CREATE TABLE";
            _creationString += " " + _table + " (";
            foreach (string column in _columns)
            {
                _creationString += "`" + column + "`" + " " + _vartypes[_columns.IndexOf(column)] + ",";
            }
            readRemotePrimaryKeys();
            if (_primaryKey.Count > 0)
            {
                _creationString += " PRIMARY KEY(";
                foreach (string key in _primaryKey)
                {
                    _creationString += key + ",";
                }
                _creationString = _creationString.Substring(0, _creationString.Length - 1);
                _creationString += ")";
            }

            _creationString += ");";
            try
            {
                DbCommand liteCommand = liteFac.CreateCommand();
                liteCommand.CommandText = _creationString;
                liteCommand.Connection = liteConnection;
                liteConnection.Open();
                liteCommand.ExecuteNonQuery();
                liteConnection.Close();
                liteConnection.Dispose();
                liteCommand.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                liteConnection.Close();
                liteConnection.Dispose();
                Log += "Create local table failure: \r\n" + ex.Message;
                return false;
            }
        }

        public void deleteColumn(string column)
        {
            _columns.Remove(column);
        }

        public void deleteColumns(string[] columns)
        {
            foreach (string column in columns)
            {
                _columns.Remove(column);
            }
        }

        public void deletePrimaryKey(string column)
        {
            _primaryKey.Remove(column);
        }

        public void deletePrimaryKeys(string[] columns)
        {
            foreach (string column in columns)
            {
                _primaryKey.Remove(column);
            }
        }

        public void deleteVartype(string vartype)
        {
            _vartypes.Remove(vartype);
        }

        public void deleteVartypes(string[] vartypes)
        {
            foreach (string vartype in vartypes)
            {
                _vartypes.Remove(vartype);
            }
        }

        /// <summary>
        /// Get names of columns from the MySQL table, and add them to the List of Columns.
        /// </summary>
        public void getColumnsFromMySqlTable()
        {
            setColumns();
            _sql = "SELECT * FROM " + Table + ";";

            getMySqlTable();
            foreach (DataColumn col in dTable.Columns)
            {
                addColumn(col.ColumnName);
            }
        }
        public void getColumnsFromLocalTable()
        {
            setColumns();
            _sql = "SELECT * FROM " + Table + ";";

            getLocalTable();
            foreach (DataColumn col in dTable.Columns)
            {
                addColumn(col.ColumnName);
            }
        }
        public string getColumnString(string delimiter)
        {
            string result = "";

            foreach (string column in _columns)
            {
                result += column + delimiter;
            }
            result = result.Substring(result.Length - 1);
            return result;
        }

        public string getPrimaryKeyString(string delimiter)
        {
            string result = "";

            foreach (string column in _primaryKey)
            {
                result += column + delimiter;
            }
            result = result.Substring(result.Length - 1);
            return result;
        }

        /// <summary>
        /// Acquire the columns from the MySQL table.
        /// </summary>
        public void getVartypesFromRemoteTable()
        {
            setVartypes();
            _sql = "SELECT * FROM " + Table + ";";
            string vartype = "";
            getMySqlTable();
            foreach (DataColumn col in dTable.Columns)
            {
                vartype = convertDataTypeSQLite(col.DataType.Name);

                if (col.Unique)
                {
                    vartype += " UNIQUE";
                }
                if (col.AutoIncrement)
                {
                    vartype += "";
                }

                addVartype(vartype);
            }
        }
        public void getVartypesFromLocalTable()
        {
            setVartypes();
            _sql = "SELECT * FROM " + Table + ";";
            string vartype = "";
            getLocalTable();
            foreach (DataColumn col in dTable.Columns)
            {
                vartype = convertDataTypeSQLite(col.DataType.Name);

                if (col.Unique)
                {
                    vartype += " UNIQUE";
                }
                if (col.AutoIncrement)
                {
                    vartype += "";
                }

                addVartype(vartype);
            }
        }
        public string getVartypeString(string delimiter)
        {
            string result = "";

            foreach (string vartype in _vartypes)
            {
                result += vartype + delimiter;
            }
            result = result.Substring(result.Length - 1);
            return result;
        }

        public void insertColumn(int index, string column)
        {
            _columns.Insert(index, column);
        }

        public void insertPrimaryKey(int index, string vartype)
        {
            _primaryKey.Insert(index, vartype);
        }

        public void insertVartype(int index, string vartype)
        {
            _vartypes.Insert(index, vartype);
        }

        public void loadFactories()
        {
            myFactory = DbProviderFactories.GetFactory("MySql.Data.MySqlClient");
            liteFac = DbProviderFactories.GetFactory("System.Data.SQLite");
        }

        public void setColumns(List<string> columns)
        { _columns = columns; }

        public void setColumns(string[] columns)
        {
            setColumns(new List<string>(columns));
        }

        public void setColumns()
        {
            _columns.Clear();
        }
        
        public void setPrimaryKey(List<string> columns)
        { _primaryKey = columns; }

        public void setPrimaryKey(string[] columns)
        {
            setPrimaryKey(new List<string>(columns));
        }

        public void setPrimaryKey()
        {
            _primaryKey.Clear();
        }

        public void setSelectStatement()
        {
            _sql = "SELECT";

            _sql += " ";
            foreach (string column in _columns)
            {
                _sql += "`" + column + "`" + ", ";
            }
            _sql = _sql.Substring(0, _sql.Length - 2);
            _sql += " FROM " + _table + ";";
        }

        public void setMySqlConnectionString(string host = "", string database = "", string username = "", string password = "")
        {
            if (host == "")
            {
                host = _host;
            }
            else
            {
                _host = host;
            }
            if (database == "")
            {
                database = _database;
            }
            else
            {
                _database = database;
            }
            if (username == "")
            {
                username = _username;
            }
            else
            {
                _username = username;
            }
            if (password == "")
            {
                password = _password;
            }
            else
            {
                _password = password;
            }
            MySqlConnectionString = String.Format("Host={0}; Database={1}; Uid={2}; Pwd={3}; Allow Zero DateTime=true;", host, database, username, password);
            myConnection.ConnectionString = MySqlConnectionString;
        }

        public void setSqliteConnectionString(string basepath = "", string database = "", string extension = "")
        {
            if (basepath == "")
            {
                basepath = _basepath;
            }
            else
            {
                _basepath = basepath;
            }
            if (database == "")
            {
                database = _database;
            }
            else
            {
                _database = database;
            }
            if (extension == "")
            {
                extension = _extension;
            }
            else
            {
                _extension = extension;
            }
            SQLiteConnectionString = String.Format("DataSource={0}{1}.{2};", basepath, database, extension);
            liteConnection.ConnectionString = SQLiteConnectionString;
        }

        public void setTablename(string name)
        {
            _table = name;
        }

        public void setVartypes(List<string> vartypes)
        { _vartypes = vartypes; }

        public void setVartypes()
        {
            _vartypes.Clear();
        }

        public void TranslateRemoteDB()
        {
            liteConnection = new SQLiteConnection(SQLiteConnectionString);
            setSelectStatement();
            DbCommand liteCommand = liteFac.CreateCommand();
            liteCommand.CommandText = _sql;
            liteCommand.Connection = liteConnection;
            liteCommand.Connection.Open();
            liteAdapter = liteFac.CreateDataAdapter();

            liteAdapter.SelectCommand = liteCommand;
            liteBuilder = liteFac.CreateCommandBuilder();
            liteBuilder.DataAdapter = liteAdapter;
            liteAdapter.InsertCommand = liteBuilder.GetInsertCommand();
            liteAdapter.UpdateCommand = liteBuilder.GetUpdateCommand();
            liteAdapter.DeleteCommand = liteBuilder.GetDeleteCommand();
            
            
            Log = ("Connecting to SQLite Database file\r\n");
            try
            {
                //liteCon = new SQLiteConnection(SQLiteConnectionString);
                //liteCon.Open();
                //liteCommand.Connection = liteCon;
                Log = ("Connection Succesful, Connected to SQLite\r\n");
            }
            catch (Exception ex)
            {
                Log = ("Connection Failed :\r\n" + ex.Message);
            }

            Log = ("Creating Table for mysql transfer\r\n");

            try
            {
                myConnection.Open();
                Log = ("MySQL Connection Successful\r\n");
            }
            catch (Exception ex)
            {
                Log = (String.Format("Connection failed:\r\n Exception:\r\n {0}\n\rMessage:\r\n{1}", ex.HResult.ToString(), ex.Message));
            }
            try
            {
                
               // liteCommand = liteFac.CreateCommand();
                //liteAdapter = liteFac.CreateDataAdapter();

                
                dTable = getMySqlTable();
                // di it work?
                //System.Console.WriteLine(dTable.Rows.Count);
                int count = 0;
                liteAdapter.Update(dTable);
                using (IDbTransaction tran = liteConnection.BeginTransaction())
                {
                    int rowCount = dTable.Rows.Count;
                    int columnCount = dTable.Columns.Count;

                    liteCommand.Connection = liteConnection;
                    
                    for(int row=0;row<rowCount;row++)
                    {
                        for (int column = 0; column < columnCount;column++)
                        {
                            liteAdapter.InsertCommand.Parameters[column].Value = dTable.Rows[row][column];
                            

                        }
                    //liteCommand.ExecuteNonQuery();
                        liteAdapter.InsertCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                Log = ("Database Reading Failed\r\n" + ex.Message);
            }
            myConnection.Close();
        }

        public void UpdateLocalDB()
        {
            setSelectStatement();
            Log = ("Connecting to SQLite Database file\r\n");
            try
            {
                liteConnection = new SQLiteConnection(SQLiteConnectionString);
                liteConnection.Open();
                Log = ("Connection Succesful, Connected to SQLite\r\n");
            }
            catch (Exception ex)
            {
                Log = ("Connection Failed :\r\n" + ex.Message);
            }

            Log = ("Updating Table for mysql transfer\r\n");

            try
            {
                myConnection.Open();
                Log = ("MySQL Connection Successful\r\n");
            }
            catch (Exception ex)
            {
                Log = (String.Format("Connection failed:\r\n Exception:\r\n {0}\n\rMessage:\r\n{1}", ex.HResult.ToString(), ex.Message));
            }
            try
            {
                dTable = getMySqlTable();

                setupLiteTable();

                using (IDbTransaction tran = liteConnection.BeginTransaction())
                {
                    liteCommand.Connection = liteConnection;
                    foreach (DataRow row in dTable.Rows)
                    {
                        liteAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                myConnection.Close();
                liteConnection.Close();
            }
            catch (Exception ex)
            {
                Log = ("Database Reading Failed\r\n" + ex.Message);
            }
            
        }
        public void UpdateLocalDB(DataTable datatable)
        {
            setSelectStatement();
            Log = ("Connecting to SQLite Database file\r\n");
            try
            {
                liteConnection = new SQLiteConnection(SQLiteConnectionString);
                liteConnection.Open();
                Log = ("Connection Succesful, Connected to SQLite\r\n");
            }
            catch (Exception ex)
            {
                Log = ("Connection Failed :\r\n" + ex.Message);
            }

            Log = ("Updating Table for mysql transfer\r\n");

            
            try
            {
                dTable = datatable;

                setupLiteTable();

                using (IDbTransaction tran = liteConnection.BeginTransaction())
                {
                    int i = 0;
                    liteCommand.Connection = liteConnection;
                    foreach (DataRow row in dTable.Rows)
                    {
                        foreach (DbParameter parameter in liteAdapter.InsertCommand.Parameters)
                        {
                            parameter.Value = row[i];
                            i++;

                        }

                        liteAdapter.InsertCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
                myConnection.Close();
                liteConnection.Close();
            }
            catch (Exception ex)
            {
                Log = ("Database Reading Failed\r\n" + ex.Message);
            }

        }
        public void UpdateRemoteDB(DataTable Table)
        {
            dTable = Table;
            setSelectStatement();
            liteAdapter = liteFac.CreateDataAdapter();
            myAdapter = myFactory.CreateDataAdapter();
            Log = ("Connecting to MySql Database file\r\n");
            Log = ("Updating Table for mysql transfer\r\n");
            try
            {
                myConnection.Open();
                Log = ("MySQL Connection Successful\r\n");
            }
            catch (Exception ex)
            {
                Log = (String.Format("Connection failed:\r\n Exception:\r\n {0}\n\rMessage:\r\n{1}", ex.HResult.ToString(), ex.Message));
            }
            try
            {
                using (IDbTransaction tran = myConnection.BeginTransaction())
                {
                    myCommand.Connection = myConnection;
                    foreach (DataRow row in dTable.Rows)
                    {
                        myAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                Log = ("Database Reading Failed\r\n" + ex.Message);
            }
            myConnection.Close();
            myConnection.Dispose();
        }
        public void UpdateRemoteDB()
        {
            setSelectStatement();
            Log = ("Connecting to SQLite Database file\r\n");
            try
            {
                liteConnection = new SQLiteConnection(SQLiteConnectionString);
                liteConnection.Open();
                Log = ("Connection Succesful, Connected to SQLite\r\n");
            }
            catch (Exception ex)
            {
                Log = ("Connection Failed :\r\n" + ex.Message);
            }

            Log = ("Updating Table for mysql transfer\r\n");

            try
            {
                myConnection.Open();
                Log = ("MySQL Connection Successful\r\n");
            }
            catch (Exception ex)
            {
                Log = (String.Format("Connection failed:\r\n Exception:\r\n {0}\n\rMessage:\r\n{1}", ex.HResult.ToString(), ex.Message));
            }
            try
            {
                dTable = getLocalTable();

                setupLiteTable();

                using (IDbTransaction tran = myConnection.BeginTransaction())
                {
                    myCommand.Connection = myConnection;
                    foreach (DataRow row in dTable.Rows)
                    {
                        myAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    tran.Commit();
                }
            }
            catch (Exception ex)
            {
                Log = ("Database Reading Failed\r\n" + ex.Message);
            }
            myConnection.Close();
            
            myConnection.Dispose();
            
        }
        public DataTable getMySqlTable()
        {
            myCommand = myFactory.CreateCommand();
            myCommand.CommandText = _sql;
            myCommand.Connection = myConnection;
            myAdapter = myFactory.CreateDataAdapter();
            myAdapter.SelectCommand = myCommand;
            myBuilder = myFactory.CreateCommandBuilder();
            myBuilder.DataAdapter = myAdapter;
            myAdapter.InsertCommand = myBuilder.GetInsertCommand();
            myAdapter.UpdateCommand = myBuilder.GetUpdateCommand();
            myAdapter.DeleteCommand = myBuilder.GetDeleteCommand();
            dTable = new DataTable();
            myAdapter.FillSchema(dTable, System.Data.SchemaType.Source);
            myAdapter.Fill(dTable);
            return dTable;
        }
        public DataTable getLocalTable()
        {
           
            
            if (liteConnection.State == ConnectionState.Open)
            {

               liteConnection.Close();

            }
            
            DbCommand liteCommand = liteFac.CreateCommand();
            liteCommand.CommandText = _sql;
            liteCommand.Connection = liteConnection;
            liteCommand.Connection.Open();
            liteAdapter = liteFac.CreateDataAdapter();
            
            liteAdapter.SelectCommand = liteCommand;
            liteBuilder = liteFac.CreateCommandBuilder();
            liteBuilder.DataAdapter = liteAdapter;
            liteAdapter.InsertCommand = liteBuilder.GetInsertCommand();
            liteAdapter.UpdateCommand = liteBuilder.GetUpdateCommand();
            liteAdapter.DeleteCommand = liteBuilder.GetDeleteCommand();
            dTable = new DataTable();
            
            //liteAdapter.FillSchema(dTable, System.Data.SchemaType.Mapped);

            //dTable.Constraints.Clear();

            liteAdapter.Fill(dTable);
            return dTable;
        }
        private void readRemotePrimaryKeys()
        {
            getColumnsFromMySqlTable();

            DataColumn[] primarykey = dTable.PrimaryKey;
            foreach (DataColumn col in dTable.Columns)
            {
                foreach (DataColumn cpk in primarykey)
                {
                    if (cpk == col)
                    {
                        addPrimaryKey(cpk.ColumnName);
                    }
                }
            }
        }
        private void readLocalPrimaryKeys()
        {
            getColumnsFromLocalTable();

            DataColumn[] primarykey = dTable.PrimaryKey;
            foreach (DataColumn col in dTable.Columns)
            {
                foreach (DataColumn cpk in primarykey)
                {
                    if (cpk == col)
                    {
                        addPrimaryKey(cpk.ColumnName);
                    }
                }
            }
        }
        private void setupLiteTable()
        {
            liteCommand = liteFac.CreateCommand();
            liteCommand.CommandText = _sql;
            liteCommand.Connection = liteConnection;
            liteAdapter = liteFac.CreateDataAdapter();
            liteAdapter.SelectCommand = liteCommand;
            liteBuilder = liteFac.CreateCommandBuilder();
            liteBuilder.DataAdapter = liteAdapter;
            liteAdapter.InsertCommand = liteBuilder.GetInsertCommand();
            liteAdapter.UpdateCommand = liteBuilder.GetUpdateCommand();
            liteAdapter.DeleteCommand = liteBuilder.GetDeleteCommand();
        }

        public void setupMysqlTable()
        {
            myCommand = myFactory.CreateCommand();
            myCommand.CommandText = _sql;
            myCommand.Connection = myConnection;
            myAdapter = myFactory.CreateDataAdapter();
            myAdapter.SelectCommand = myCommand;
            myBuilder = myFactory.CreateCommandBuilder();
            myBuilder.DataAdapter = myAdapter;
            myAdapter.SelectCommand = myCommand;
            myAdapter.InsertCommand = myBuilder.GetInsertCommand();
            myAdapter.UpdateCommand = myBuilder.GetUpdateCommand();
            myAdapter.DeleteCommand = myBuilder.GetDeleteCommand();
        
        }
    }
}