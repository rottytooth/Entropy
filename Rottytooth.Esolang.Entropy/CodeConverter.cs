using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Irony.Parsing;

namespace Rottytooth.Esolang.Entropy
{
    public class CodeConverter
    {
        private ParseTree _programTree;

        private readonly string constDeclarePlaceholder = "#stringvars#";

        private int _constCount = 0;

        public ParseTree ProgramTree
        {
            get { return _programTree; }
        }

        public CodeConverter(ParseTree programTree)
        {
            _programTree = programTree;
        }


        public string ToCSharp()
        {
            StringBuilder retProgram = new StringBuilder();
            StringBuilder constDeclarations = new StringBuilder();
            BuildProgram(_programTree.Root, retProgram, constDeclarations);
            return Regex.Replace(retProgram.ToString(), constDeclarePlaceholder.ToString(), constDeclarations.ToString());
        }

        private void BuildProgram(ParseTreeNode currentNode, StringBuilder retProgram, StringBuilder constDeclarations)
        {
            string currentVariable; // used to hold value of current variable

            switch (currentNode.Term.Name)
            {
                case "programDeclaration":
                    // get namespace and program name, send the rest of the
                    // child nodes back to BuildProgram()

                    BuildProgramHeader(currentNode, retProgram, constDeclarations);
                    break;

                case "localVariableDeclaration":
                    BuildDeclareVariable(currentNode, retProgram);
                    break;

                case "assignmentOperator":
                    BuildAssignment(currentNode, retProgram, constDeclarations);
                    break;

                case "printStatement":
                    retProgram.Append("Console.Write(");
                    foreach (ParseTreeNode subNode in currentNode.ChildNodes)
                    {
                        BuildProgram(subNode, retProgram, constDeclarations);
                    }
                    retProgram.AppendLine(");");
                    break;

                case "ifStatement":
                    //Console.WriteLine("reached if statement");
                    BuildIfStatement(currentNode, retProgram, constDeclarations);
                    break;

                case "whileLoop":
                    BuildWhileLoop(currentNode, retProgram, constDeclarations);
                    break;

                case "CharLiteral":
                    currentVariable = "CONST" + _constCount.ToString();
                    constDeclarations.AppendLine("Rottytooth.Esolang.Entropy.Char " + 
                        currentVariable + " = " + currentNode.Token.Text + ";");

                    retProgram.Append(currentVariable);
                    _constCount++;
                    break;

                case "StringLiteral":
                    currentVariable = "CONST" + _constCount.ToString();
                    constDeclarations.AppendLine("Rottytooth.Esolang.Entropy.String " +
                        currentVariable + " = " + currentNode.Token.Text + ";");

                    retProgram.Append(currentVariable);
                    _constCount++;
                    break;

                case "Number":
                    currentVariable = "CONST" + _constCount.ToString();
                    constDeclarations.AppendLine("Rottytooth.Esolang.Entropy.Real " +
                        currentVariable + " = " + currentNode.Token.Text + ";");

                    retProgram.Append(currentVariable);
                    _constCount++;
                    break;

                case "variable_id":
                    retProgram.Append(" " + currentNode.Token.Text + " ");
                    break;

                // expressions are lowest in order of operations
                case "==":
                case "<":
                case ">":
                case "+":
                case "-":
                case "*":
                case "/":
                case "%":
                case "<=":
                case ">=":
                case "(":
                case ")":
                    retProgram.Append(" " + currentNode.Term.Name + " ");
                    break;

                default:
                    // if we don't recognize the command, drill down to its child nodes
                    foreach (ParseTreeNode node in currentNode.ChildNodes)
                    {
                        BuildProgram(node, retProgram, constDeclarations);
                    }
                    break;
            }
        }

        private void BuildWhileLoop(ParseTreeNode currentNode, StringBuilder retProgram, StringBuilder constDeclarations)
        {
            if (currentNode.ChildNodes[0].Term.Name.ToLower() != "while")
            {
                throw new CompilerError("error building while loop: no \"while\" command");
            }

            if (currentNode.ChildNodes[1].Term.Name.ToLower() != "expression")
            {
                throw new CompilerError("assignment without valid expression");
            }

            retProgram.Append("while (");
            BuildProgram(currentNode.ChildNodes[1], retProgram, constDeclarations);
            retProgram.AppendLine(")");
            retProgram.AppendLine("{");
            BuildProgram(currentNode.ChildNodes[2], retProgram, constDeclarations);
            retProgram.AppendLine("}");
        }

        private void BuildIfStatement(ParseTreeNode currentNode, StringBuilder retProgram, StringBuilder constDeclarations)
        {
            if (currentNode.ChildNodes[0].Term.Name.ToLower() != "if")
            {
                throw new CompilerError("error building if statement: no \"if\" command");
            }

            if (currentNode.ChildNodes[1].Term.Name.ToLower() != "expression")
            {
                throw new CompilerError("assignment without valid expression");
            }

            retProgram.Append("if (");
            BuildProgram(currentNode.ChildNodes[1], retProgram, constDeclarations);
            retProgram.AppendLine(")");
            retProgram.AppendLine("{");
            BuildProgram(currentNode.ChildNodes[2], retProgram, constDeclarations);
            retProgram.AppendLine("}");
        }

        private void BuildAssignment(ParseTreeNode currentNode, StringBuilder retProgram, StringBuilder constDeclarations)
        {
            string variableName = null;

            if (currentNode.ChildNodes[1].Term.Name.ToLower() != "identifier_id")
            {
                throw new CompilerError("assignment without valid identifier");
            }
            if (currentNode.ChildNodes[3].Term.Name.ToLower() != "expression")
            {
                throw new CompilerError("assignment without valid expression");
            }

            variableName = currentNode.ChildNodes[1].Token.Value.ToString().ToLower();

            retProgram.Append(variableName + " = ");
            BuildProgram(currentNode.ChildNodes[3], retProgram, constDeclarations);
            retProgram.AppendLine(";");
        }

        private void BuildDeclareVariable(ParseTreeNode currentNode, StringBuilder retProgram)
        {
            int nodeCount = 0;
            string variableName = null, variableType = null;

            for (; nodeCount < currentNode.ChildNodes.Count; nodeCount++)
            {
                if (currentNode.ChildNodes[nodeCount].Term.Name.ToLower() == "identifier_id")
                {
                    variableName = currentNode.ChildNodes[nodeCount].Token.Value.ToString().ToLower();
                }
                if (currentNode.ChildNodes[nodeCount].Term.Name.ToLower() == "datatype")
                {
                    variableType = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        currentNode.ChildNodes[nodeCount].ChildNodes[0].Term.Name);
                }
            }

            retProgram.AppendLine("Rottytooth.Esolang.Entropy." + variableType + " " + variableName + ";");
        }

        private void BuildProgramHeader(ParseTreeNode currentNode, StringBuilder retProgram, StringBuilder constDeclarations)
        {
            int nodeCount = 0;
            string namespaceID = null, programID = null;

            for ( ; nodeCount < currentNode.ChildNodes.Count; nodeCount++)
            {
                if (currentNode.ChildNodes[nodeCount].Term.Name.ToLower() == "namespace_id")
                {
                    namespaceID = currentNode.ChildNodes[nodeCount].Token.Value.ToString();
                }
                if (currentNode.ChildNodes[nodeCount].Term.Name.ToLower() == "program_id")
                {
                    programID = currentNode.ChildNodes[nodeCount].Token.Value.ToString();
                }

                if (namespaceID != null && programID != null)
                {
                    break;
                }
            }

            if (namespaceID == null) throw new SyntaxError("no namespace provided");
            if (programID == null) throw new SyntaxError("no program name provided");

            retProgram.Append(WriteProgramHeader(namespaceID, programID));

            for ( ; nodeCount < currentNode.ChildNodes.Count; nodeCount++)
            {
                BuildProgram(currentNode.ChildNodes[nodeCount], retProgram, constDeclarations);
            }

            retProgram.AppendLine("}"); // end function
            retProgram.AppendLine("}"); // end class
            retProgram.AppendLine("}"); // end namespace
        }

        private string WriteProgramHeader(string namespaceID, string programID)
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("using System;");
            header.AppendLine("using Rottytooth.Esolang.Entropy;");
            header.AppendLine();
            header.AppendLine("namespace " + namespaceID);
            header.AppendLine("{");
            header.AppendLine("public class " + programID);
            header.AppendLine("{");
            header.AppendLine("static void Main()");
            header.AppendLine("{");

            // here we are setting the resulting program to use the mutation rate that's currently set in Real since we know this was set by the compiler as a setting
            // kind of a round-about way of getting the value here, but is good enough for now
            // FIXME: let's make this less confusing
            header.AppendLine("\tRottytooth.Esolang.Entropy.Real.MutationRate = " +
                              Rottytooth.Esolang.Entropy.Real.MutationRate.ToString() + "F;");
            header.AppendLine(constDeclarePlaceholder); // place holder for string variables
            return header.ToString();
        }

    }
}
