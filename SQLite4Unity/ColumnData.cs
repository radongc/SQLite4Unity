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
        public string Name;
        public bool IsPrimaryKey;

        public ColumnData(Type type, string name, bool isPrimaryKey)
        {
            Type = type;
            Name = name;
            IsPrimaryKey = isPrimaryKey;
        }

        public override string ToString()
        {
            return $"(Type: {Type.ToString()}, Name: {Name}, Primary Key: {IsPrimaryKey})";
        }
    }
}
