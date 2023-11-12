using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite4Unity.ORM
{
    internal class ColumnData
    {
        public Type Type;
        public string ColumnName;
        public string PropertyName;
        public bool IsPrimaryKey;

        public ColumnData(Type type, string columnName, string propertyName, bool isPrimaryKey)
        {
            Type = type;
            ColumnName = columnName;
            PropertyName = propertyName;
            IsPrimaryKey = isPrimaryKey;
        }

        public override string ToString()
        {
            return $"(Type: {Type.ToString()}, Name: {ColumnName}, Primary Key: {IsPrimaryKey})";
        }
    }
}
