using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
