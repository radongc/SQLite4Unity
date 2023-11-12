namespace SQLite4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class SQLiteTableAttribute : Attribute
    {
        public string TableName;

        public SQLiteTableAttribute(string tablename)
        {
            TableName = tablename;
        }
    }
}
