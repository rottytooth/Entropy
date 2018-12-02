using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rottytooth.Entropy
{
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public class Char
    {
        private Real local;

        public char Value
        {
            get
            {
                return (char)Convert.ToInt32(Math.Round(local.Value));
            }
            set 
            { 
                local = Convert.ToSingle((int)value); 
            }
        }

        public float ValueAsFloat
        {
            get
            {
                return local.Value;
            }
            set
            {
                local.Value = value;
            }
        }

        public static implicit operator Char(char s)
        {
            return new Char { Value = s };
        }

        public static explicit operator char(Char f)
        {
            return f.Value;
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }

        public static bool operator >(Char a, Char b)
        {
            return (a.ValueAsFloat > b.ValueAsFloat);
        }

        public static bool operator >(Char a, Real b)
        {
            return (a.ValueAsFloat > b.Value);
        }

        public static bool operator <(Char a, Char b)
        {
            return (a.ValueAsFloat < b.ValueAsFloat);
        }

        public static bool operator <(Char a, Real b)
        {
            return (a.ValueAsFloat < b.Value);
        }

        public static Char operator -(Char a, Real b)
        {
            return new Char { ValueAsFloat = a.ValueAsFloat - b.Value };
        }

    }
}
