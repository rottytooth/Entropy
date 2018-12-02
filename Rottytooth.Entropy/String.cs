using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rottytooth.Entropy
{
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public class String
    {
        private List<Char> local;

        public String()
        {
        }

        public String(string value)
        {
            Value = value;
        }

        public string Value
        {
            get
            {
                StringBuilder retStr = new StringBuilder();
                foreach (Char a in local)
                {
                    retStr.Append(a);
                }
                return retStr.ToString();
            }
            set
            {
                local = new List<Char>();
                foreach (char a in value)
                {
                    local.Add((Char)a);
                }
            }
        }

        public static implicit operator String(string s)
        {
            return new String { Value = s };
        }

        public static explicit operator string(String f)
        {
            return f.Value;
        }

        public static String operator +(String c1, String c2)
        {
            return new String(c1.ToString() + c2.ToString());
        }

        public static String operator +(String c1, string c2)
        {
            return new String(c1.ToString() + c2);
        }

        public static String operator +(string c1, String c2)
        {
            return new String(c1 + c2.ToString());
        }

        public override string ToString()
        {
            return this.Value;
        }

    }
}
