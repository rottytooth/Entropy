using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Irony.Parsing;

namespace Rottytooth.Entropy
{
    public sealed class EntropyGrammar : Grammar
    {

        public EntropyGrammar() : base(false) // turns off case-sensitivity
        {

            #region definitions
            #region literals
            // Str
            StringLiteral stringLiteral = TerminalFactory.CreateCSharpString("StringLiteral");
            // Char
            StringLiteral charLiteral = TerminalFactory.CreateCSharpChar("CharLiteral");
            // Real
            NumberLiteral number = CreateRealNumber("Number");
            #endregion


            #region non-terminals
            NonTerminal program = new NonTerminal("program"); //, typeof(ProgramNode));
            this.Root = program; // specify the non-terminal which is the root of the AST

            NonTerminal programDeclaration = new NonTerminal("programDeclaration");

            NonTerminal statementList = new NonTerminal("statementList"); //, typeof(StatementListNode));
            NonTerminal emptyStatement = new NonTerminal("emptyStatement");
            NonTerminal statement = new NonTerminal("statement"); //, typeof(StatementNode));

            NonTerminal printStatement = new NonTerminal("printStatement");
            NonTerminal ifStatement = new NonTerminal("ifStatement");//, typeof(IfStatementNode));
            NonTerminal whileLoop = new NonTerminal("whileLoop");
            NonTerminal expression = new NonTerminal("expression"); //, typeof(ExpressionNode));
            NonTerminal binaryOperator = new NonTerminal("binaryOperator"); //, typeof(BinaryOperatorNode));
            NonTerminal assignment = new NonTerminal("assignmentOperator");
            NonTerminal localVariableDeclaration = new NonTerminal("localVariableDeclaration");
            NonTerminal conditionalExpression = new NonTerminal("conditionalExpression");
            NonTerminal functionDeclaration = new NonTerminal("functionDeclaration");
            #endregion


            #region terminals
            CommentTerminal singleLineComment = new CommentTerminal("SingleLineComment", "//", "\r", "\n", "\u2085", "\u2028", "\u2029");
            NonGrammarTerminals.Add(singleLineComment);
            CommentTerminal delimitedComment = new CommentTerminal("DelimitedComment", "/*", "*/");
            NonGrammarTerminals.Add(delimitedComment);

            IdentifierTerminal variable_id = TerminalFactory.CreateCSharpIdentifier("variable_id");
            IdentifierTerminal namespace_id = TerminalFactory.CreateCSharpIdentifier("namespace_id");
            IdentifierTerminal function_id = TerminalFactory.CreateCSharpIdentifier("function_id");
            IdentifierTerminal program_id = TerminalFactory.CreateCSharpIdentifier("program_id");
            IdentifierTerminal identifier_id = CreateCSharpIdentifier("identifier_id");
            NonTerminal datatype = new NonTerminal("datatype");
            //identifierTerminal.("set", "to", "if", "freight", "cost", "is", "loop", "through", "order");

            this.MarkPunctuation(";", "[", "]", "(", ")");
            #endregion
            #endregion

            #region grammar

            //<Program> ::= "Program" <ProgramName> <StatementList> <FunctionList> "End" "Program"
            programDeclaration.Rule = ToTerm("Program") + namespace_id + program_id + "[" + statementList + "]";
            program.Rule = programDeclaration; // +Symbol("End") + Symbol("Program");

            datatype.Rule = ToTerm("char") | "real" | "string";

            #region statements
            //<StatementList> ::= <Statement>*
            statementList.Rule = MakeStarRule(statementList, null, statement);

            statement.Rule = emptyStatement | printStatement | ifStatement
                | whileLoop
                | localVariableDeclaration | assignment;

            emptyStatement.Rule = ToTerm(";"); //setVariable + ";" | ifStatement | orderLoop | expression + ";";

//            printStatement.Rule = ToTerm("print") + (stringLiteral | charLiteral) + ";";
            printStatement.Rule = ToTerm("print") + expression + ";";

            //<IfStatement> ::= "if" <Expression> "[" <StatementList> "]"
            ifStatement.Rule = ToTerm("if") + expression + "[" + statementList + "]";
            whileLoop.Rule = ToTerm("while") + expression + "[" + statementList + "]";

            assignment.Rule = "let" + identifier_id + (ToTerm("=") | "+=" | "-=" | "*=" | "/=" | "%=" | "&=" | "|=" | "^=" | "<<=" | ">>=") + expression + ";";

            //functionDeclaration.Rule = "function" + identifier_id + "(" + 

            localVariableDeclaration.Rule = ToTerm("declare") + identifier_id + datatype + ";"; // identifier_id
            #endregion


            #region expressions

            //<Expression> ::= <number> | <variable> | <string>
            //  | <Expression> <BinaryOperator> <Expression>
            //  | "(" <Expression> ")"
            expression.Rule = number | variable_id | stringLiteral | charLiteral
                | expression + binaryOperator + expression
                | conditionalExpression 
                | "(" + expression + ")";

            //<BinaryOperator> ::= "+" | "-" | "*" | "/" | "<" | ">" | "<=" | ">=" | "is"
            binaryOperator.Rule = ToTerm("+") | "-" | "*" | "/" | "%";

            // "=="
            conditionalExpression.Rule = expression + (ToTerm("<") | ">" | "<=" | ">=") + expression; 

            #endregion
            #endregion
        }

        public static NumberLiteral CreateRealNumber(string name)
        {
            NumberLiteral term = new NumberLiteral(name);
            term.DefaultFloatType = TypeCode.Double;
            return term;
        }

        public static IdentifierTerminal CreateCSharpIdentifier(string name)
        {
            IdentifierTerminal id = new IdentifierTerminal(name, IdOptions.AllowsEscapes | IdOptions.CanStartWithEscape);
            id.AddPrefix("@", IdOptions.IsNotKeyword);
            //From spec:
            //Start char is "_" or letter-character, which is a Unicode character of classes Lu, Ll, Lt, Lm, Lo, or Nl 
            id.StartCharCategories.AddRange(new UnicodeCategory[] {
                 UnicodeCategory.UppercaseLetter, //Ul
                 UnicodeCategory.LowercaseLetter, //Ll
                 UnicodeCategory.TitlecaseLetter, //Lt
                 UnicodeCategory.ModifierLetter,  //Lm
                 UnicodeCategory.OtherLetter,     //Lo
                 UnicodeCategory.LetterNumber     //Nl
              });
            //Internal chars
            /* From spec:
            identifier-part-character: letter-character | decimal-digit-character | connecting-character |  combining-character |
                formatting-character
      */
            id.CharCategories.AddRange(id.StartCharCategories); //letter-character categories
            id.CharCategories.AddRange(new UnicodeCategory[] {
                UnicodeCategory.DecimalDigitNumber, //Nd
                UnicodeCategory.ConnectorPunctuation, //Pc
                UnicodeCategory.SpacingCombiningMark, //Mc
                UnicodeCategory.NonSpacingMark,       //Mn
                UnicodeCategory.Format                //Cf
              });
            //Chars to remove from final identifier
            id.CharsToRemoveCategories.Add(UnicodeCategory.Format);
            return id;
        }
    }
}
