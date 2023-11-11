using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName;
        public bool PrimaryKey = false;

        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
