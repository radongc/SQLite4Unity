using Mono.Data.Sqlite;
using SQLite4Unity.Attributes;
using SQLite4Unity.Extension;
using System.Data;
using System.Diagnostics;
using System.Reflection;

namespace SQLite4Unity.ORM
{
    public class DbSet<T> where T : new()
    {
        private string DatabaseName;
        private string TableName;
        private List<ColumnData> ColumnInfo = new List<ColumnData>();

        public DbSet(string databaseName)
        {
            DatabaseName = databaseName;

            GetTableAttributeData(typeof(T));
        }

        /// <summary>
        /// Execute a query on the specified database.
        /// </summary>
        /// <param name="query">The query to execute.</param>
        /// <returns>The IdbConnection and IDataReader. Both should be closed when finished.</returns>
        public KeyValuePair<IDbConnection, IDataReader> ExecuteQueryAndGetReader(string query)
        {
            IDbConnection dbConnection = OpenDatabase();
            IDbCommand dbCmdReadValues = dbConnection.CreateCommand();
            dbCmdReadValues.CommandText = query;
            IDataReader dataReader = dbCmdReadValues.ExecuteReader();

            return new KeyValuePair<IDbConnection, IDataReader>(dbConnection, dataReader);
        }

        /// <summary>
        /// Execute a command on the specified database.
        /// </summary>
        /// <param name="nonQuery">The command to execute.</param>
        public void ExecuteNonQuery(string nonQuery)
        {
            IDbConnection dbConnection = OpenDatabase();
            IDbCommand dbCmdExecute = dbConnection.CreateCommand();
            dbCmdExecute.CommandText = nonQuery;
            dbCmdExecute.ExecuteNonQuery();

            dbConnection.Close();
        }

        private IDbConnection OpenDatabase()
        {
            string dbName = DatabaseName;

            string dbUri = $"URI=file:{dbName}";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            return dbConnection;
        }

        /// <summary>
        /// Retrieves all rows as POCOs for the specified table type 'T'.
        /// </summary>
        /// <returns>A deserialized list of all rows in the table.</returns>
        public List<T> GetAll()
        {
            KeyValuePair<IDbConnection, IDataReader> dbQuery = ExecuteQueryAndGetReader($"SELECT * FROM {TableName}");

            List<T> rows = new List<T>();

            while (dbQuery.Value.Read())
            {
                Dictionary<string, object> columnValues = new Dictionary<string, object>();

                T schema = new T();

                foreach (ColumnData columnData in ColumnInfo)
                {
                    object colValue = null;

                    if (columnData.Type == typeof(Int32))
                        colValue = (Int32)dbQuery.Value[$"{columnData.Name}"].ToInt32();
                    else if (columnData.Type == typeof(String))
                        colValue = (String)dbQuery.Value[$"{columnData.Name}"].ToString();
                    else if (columnData.Type == typeof(Single))
                        colValue = (Single)dbQuery.Value[$"{columnData.Name}"].ToFloat();

                    columnValues.Add(columnData.Name, colValue);
                }

                foreach (KeyValuePair<string, object> colData in columnValues)
                {
                    PropertyInfo schemaProperty = schema.GetType().GetProperty(colData.Key);

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
                    ColumnInfo.Add(new ColumnData(propertyInfos[i].PropertyType, propertyInfos[i].Name, false));
                }
                else
                {
                    ColumnInfo.Add(new ColumnData(propertyInfos[i].PropertyType, columnAttr.ColumnName, columnAttr.PrimaryKey));
                }
            }
        }
    }
}