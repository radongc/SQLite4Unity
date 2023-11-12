using Mono.Data.Sqlite;
using SQLite4Unity.Attributes;
using SQLite4Unity.Extension;
using System.Data;
using System.Reflection;

namespace SQLite4Unity.ORM
{
    public class DbSet<T> where T : new()
    {
        protected string DatabaseName;
        protected string TableName;
        private List<ColumnData> ColumnInfo = new List<ColumnData>();

        public DbSet(string databaseName)
        {
            DatabaseName = databaseName;

            GetTableAttributeData(typeof(T));
        }

        protected virtual KeyValuePair<IDbConnection, IDataReader> ExecuteQueryAndGetReader(string query)
        {
            IDbConnection dbConnection = OpenDatabase();
            IDbCommand dbCmdReadValues = dbConnection.CreateCommand();
            dbCmdReadValues.CommandText = query;
            IDataReader dataReader = dbCmdReadValues.ExecuteReader();

            return new KeyValuePair<IDbConnection, IDataReader>(dbConnection, dataReader);
        }

        protected virtual void ExecuteNonQuery(string nonQuery)
        {
            IDbConnection dbConnection = OpenDatabase();
            IDbCommand dbCmdExecute = dbConnection.CreateCommand();
            dbCmdExecute.CommandText = nonQuery;
            dbCmdExecute.ExecuteNonQuery();

            dbConnection.Close();
        }

        protected virtual IDbConnection OpenDatabase()
        {
            string dbName = DatabaseName;

            string dbUri = $"URI=file:{dbName}";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            return dbConnection;
        }

        /// <summary>
        /// Execute an SQL command on the table for type 'T'.
        /// </summary>
        /// <param name="command">The SQL command to execute. (Use @TableName in place of the table name, and @DatabaseName for the database name. Ex: 'SELECT * FROM @TableName ...')</param>
        public void Execute(string command)
        {
            string realCmd = GetORMVariablesFromSQLString(command);

            ExecuteNonQuery(realCmd);
        }

        /// <summary>
        /// Query the table for type 'T' and return a list of found rows.
        /// </summary>
        /// <param name="query">The SQL query to execute. (Use @TableName in place of the table name, and @DatabaseName for the database name. Ex: 'SELECT * FROM @TableName ...')</param>
        /// <returns></returns>
        public List<T> Query(string query)
        {
            string realQuery = GetORMVariablesFromSQLString(query);

            KeyValuePair<IDbConnection, IDataReader> dbQuery = ExecuteQueryAndGetReader(realQuery);

            List<T> rows = new List<T>();

            while (dbQuery.Value.Read())
            {
                Dictionary<string, object> columnValues = new Dictionary<string, object>();

                T schema = new T();

                foreach (ColumnData columnData in ColumnInfo)
                {
                    object colValue = null;

                    if (columnData.Type == typeof(Int32))
                        colValue = (Int32)dbQuery.Value[$"{columnData.ColumnName}"].ToInt32();
                    else if (columnData.Type == typeof(String))
                        colValue = (String)dbQuery.Value[$"{columnData.ColumnName}"].ToString();
                    else if (columnData.Type == typeof(Single))
                        colValue = (Single)dbQuery.Value[$"{columnData.ColumnName}"].ToFloat();

                    columnValues.Add(columnData.ColumnName, colValue);
                }

                foreach (KeyValuePair<string, object> colData in columnValues)
                {
                    PropertyInfo schemaProperty = schema.GetType().GetProperty(colData.Key);

                    // In the case that a column has a different name than its property and is specified by ColumnAttribute.
                    if (schemaProperty == null)
                    {
                        ColumnData dataForColumn = GetColumnDataByColumnName(colData.Key);
                        schemaProperty = schema.GetType().GetProperty(dataForColumn.PropertyName);
                    }

                    if (schemaProperty.PropertyType == typeof(Int32))
                        schemaProperty.SetValue(schema, colData.Value.ToInt32());
                    else if (schemaProperty.PropertyType == typeof(String))
                        schemaProperty.SetValue(schema, colData.Value.ToString());
                    else if (schemaProperty.PropertyType == typeof(Single))
                        schemaProperty.SetValue(schema, colData.Value.ToFloat());
                }

                rows.Add(schema);
            }

            dbQuery.Key.Close();
            dbQuery.Value.Close();

            return rows;
        }

        private ColumnData GetColumnDataByColumnName(string columnName)
        {
            foreach (ColumnData col in ColumnInfo)
            {
                if (col.ColumnName == columnName)
                    return col;
            }

            return null;
        }

        private string GetORMVariablesFromSQLString(string baseString)
        {
            string modifiedQuery = baseString.Replace("@TableName", TableName);
            modifiedQuery = modifiedQuery.Replace("@DatabaseName", DatabaseName);

            return modifiedQuery;
        }

        public List<T> GetAll()
        {
            return Query($"SELECT * FROM {TableName}");
        }

        // Gets all the properties of a given class, and checks to see if the class has the SQLiteTable attrib and its members have a Column attribute attached.
        private void GetTableAttributeData(Type t)
        {
            SQLiteTableAttribute tableAttr;
            ColumnAttribute columnAttr;

            tableAttr = (SQLiteTableAttribute)Attribute.GetCustomAttribute(t, typeof(SQLiteTableAttribute));

            if (tableAttr == null)
            {
                Console.WriteLine($"No attribute in class {t.ToString()}.");
            }
            else
            {
                TableName = tableAttr.TableName;
            }

            PropertyInfo[] propertyInfos = t.GetProperties();

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                columnAttr = (ColumnAttribute)Attribute.GetCustomAttribute(propertyInfos[i], typeof(ColumnAttribute));
                if (columnAttr == null)
                {
                    // If no attribute, take the var name itself as the column name.
                    ColumnInfo.Add(new ColumnData(propertyInfos[i].PropertyType, propertyInfos[i].Name, propertyInfos[i].Name, false));
                }
                else
                {
                    ColumnInfo.Add(new ColumnData(propertyInfos[i].PropertyType, columnAttr.ColumnName, propertyInfos[i].Name, columnAttr.PrimaryKey));
                }
            }
        }
    }
}