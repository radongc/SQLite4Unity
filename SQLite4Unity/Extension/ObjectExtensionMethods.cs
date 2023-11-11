using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLite4Unity.Extension
{
    public static class ObjectExtensionMethods
    {
        // Saves a bit of time.
        public static ushort ToUshort(this object obj)
        {
            return Convert.ToUInt16(obj);
        }

        public static int ToInt32(this object obj)
        {
            return Convert.ToInt32(obj);
        }

        public static float ToFloat(this object obj)
        {
            float ret;
            float.TryParse(obj.ToString(), out ret);

            return ret;
        }

        public static bool ToBool(this object obj)
        {
            return Convert.ToBoolean(obj);
        }
    }
}
