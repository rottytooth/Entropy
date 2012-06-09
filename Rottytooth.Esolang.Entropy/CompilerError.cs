using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rottytooth.Esolang.Entropy
{
    public class CompilerError : Exception
    {
        public CompilerError(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}
