using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using Microsoft.CSharp;

namespace Rottytooth.Entropy
{
    internal class CodeGenerator
    {
        internal static void Compile(string cSharpCode, string outputFileName)
        {
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
            parameters.ReferencedAssemblies.Add("Rottytooth.Entropy.dll");
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = outputFileName;
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, cSharpCode);

            if (results.Errors.Count > 0)
            {
                foreach (System.CodeDom.Compiler.CompilerError CompErr in results.Errors)
                {
                    Console.Error.WriteLine(
                        "Line number " + CompErr.Line + 
                         ", Error Number: " + CompErr.ErrorNumber + 
                         ", '" + CompErr.ErrorText + ";\n");
                }
            }

        }
    }
}
