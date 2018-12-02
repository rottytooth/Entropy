using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Text;

namespace Rottytooth.Entropy
{
    [System.Diagnostics.DebuggerDisplay("{Value}")]
    public struct Real
    {
        private float _local;

        private static readonly RNGCryptoServiceProvider Crypto;

        public static readonly float MinMutation = 0.001F;
        public static readonly float MaxMutation = 100F;

        [Obsolete]
        public static int Tolerance
        {
            get;
            set;
        }

        public static float MutationRate
        {
            get; 
            set;
        }

        static Real()
        {
//            Tolerance = 70; // 0 to 255
            MutationRate = 2F;
            Crypto = new RNGCryptoServiceProvider();
        }

        public float Value
        {
            get
            {
                Randomize();
                return _local;
            }
            set { _local = value; }
        }


        private void Randomize()
        {
            byte[] mutate = new byte[1];
            Crypto.GetBytes(mutate);

//            if ((int)mutate[0] >= Tolerance) return;

            byte[] mutateAmount = new byte[1];
            Crypto.GetNonZeroBytes(mutateAmount);

            byte[] positive = new byte[1];
            Crypto.GetNonZeroBytes(positive);

            bool isNegative = ((float) positive[0] < 255.0/2.0);

            float changeAmount = MutationRate / (float) mutateAmount[0];
            if (isNegative) changeAmount = 0 - changeAmount;

            _local += changeAmount;
        }

        public static implicit operator Real(float s)
        {
            return new Real { Value = s };
        }

        public static explicit operator float(Real f)
        {
            return f.Value;
        }

        public static implicit operator Real(double s)
        {
            return new Real { Value = Convert.ToSingle(s) };
        }

        public static explicit operator double(Real f)
        {
            return (double)(f.Value);
        }

        public static bool operator >(Real a, Real b)
        {
            return (a.Value > b.Value);
        }

        public static bool operator >(Real a, Char b)
        {
            return (a.Value > b.ValueAsFloat);
        }

        public static bool operator <(Real a, Real b)
        {
            return (a.Value < b.Value);
        }

        public static bool operator <(Real a, Char b)
        {
            return (a.Value < b.ValueAsFloat);
        }

        public static Real operator -(Real a, Real b)
        {
            return (a.Value - b.Value);
        }

        public static Real operator --(Real a)
        {
            return (a.Value - 1F);
        }

        public static Real operator +(Real a, Real b)
        {
            return (a.Value + b.Value);
        }

        public static Real operator ++(Real a)
        {
            return (a.Value + 1F);
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
