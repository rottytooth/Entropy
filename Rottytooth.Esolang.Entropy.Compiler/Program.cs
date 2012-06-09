using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Parsing;
using Rottytooth.Esolang;

namespace Rottytooth.Esolang.Entropy
{
    internal class Program
    {
        const int INVALID_PARAMETER = 0x001;

        private static string inputPath;

        //private static bool silentMode = false;

        private static bool _debugMode = false;

        public static int Main(string[] args)
        {
            Dictionary<string, string> arguments = LoadParameters(args);

            if (inputPath == null)
            {
                Console.WriteLine();
                Console.WriteLine("No .vge source file indicated");
                PrintHelp();
                return INVALID_PARAMETER;
            }

            if (arguments.ContainsKey("/h") || arguments.ContainsKey("/?"))
            {
                PrintHelp();
                return 0;
            }

            if (arguments.ContainsKey("/d"))
            {
                _debugMode = true;
            }

            if (arguments.ContainsKey("/m"))
            {
                float mutRate = 0;

                try
                {
                    mutRate = Convert.ToSingle(arguments["/m"]);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Invalid mutation rate provided");
                    PrintHelp();
                    return -1;
                }
                if (mutRate < Real.MinMutation || mutRate > Real.MaxMutation)
                {
                    Console.WriteLine("Mutation rate must be between " + Real.MinMutation.ToString() + " and " + Real.MaxMutation.ToString());
                    PrintHelp();
                    return -2;
                }
                Rottytooth.Esolang.Entropy.Real.MutationRate = mutRate;
            }

            EntropyGrammar grammar = new EntropyGrammar();

            Parser parser = new Parser(grammar);


            try
            {
                TextReader reader = new StreamReader(inputPath);
                System.IO.FileInfo file = new FileInfo(inputPath);
                string fileName = Path.GetFileNameWithoutExtension(file.FullName);
                ParseTree programTree = parser.Parse(reader.ReadToEnd());

                if (CheckForErrors(programTree, parser))
                {
                    return -1;
                }

                if (_debugMode)
                {
                    PrintTree(programTree);
                    return 0;
                }
                else
                {
                    CodeConverter converter = new CodeConverter(programTree);
                    string program = converter.ToCSharp();
                    Console.WriteLine(program);
                    CodeGenerator.Compile(program, fileName + ".exe");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.GetType() + " error thrown: " + ex.Message);
            }
            return 0;
        }

        static bool CheckForErrors(ParseTree programTree, Parser parser)
        {
            if (programTree == null || parser.Context.HasErrors)
            {
                Console.Error.WriteLine("An error has occurred");
                if (parser.Context.CurrentParserInput != null)
                {
                    Console.Error.WriteLine(parser.Context.CurrentParserInput.Token.ToString());
                    Console.Error.WriteLine("Line #" + parser.Context.CurrentParserInput.Token.Location.Line);
                    Console.Error.WriteLine("Character #" + parser.Context.CurrentParserInput.Token.Location.Column);
                    Console.Error.WriteLine("Failed at: " + parser.Context.Source.Text.Substring(
                        parser.Context.CurrentParserInput.Token.Location.Position,
                        (parser.Context.Source.Text.Length - parser.Context.CurrentParserInput.Token.Location.Position > 20) ? 20 : parser.Context.Source.Text.Length - parser.Context.CurrentParserInput.Token.Location.Position)
                        + "...");
                }
                return true;
            }
            return false;
        }

        static void PrintTree(ParseTree programTree)
        {
            TreeCrawler crawler = new TreeCrawler(programTree);
            crawler.DisplayTree();
            Console.WriteLine();
        }

        static void PrintHelp()
        {
            Console.WriteLine();
            Console.WriteLine("Entropy, version " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("\t/d\tDebug mode: parse tree and print to screen");
            Console.WriteLine("\t/c\tTranslate to C#, but do not compile");
            Console.WriteLine("\t/h /?\tPrint this message");
            Console.WriteLine("\t/m num\tSet mutation rate to num, where num is a value between " + Real.MinMutation.ToString() +" and " + Real.MaxMutation.ToString() + ", default is 2.");
        }

        private static Dictionary<string, string> LoadParameters(string[] args)
        {
            Dictionary<string, string> arguments = new Dictionary<string, string>();

            string currArg = "";
            bool waitingForSecondArgument = false;

            foreach (string arg in args)
            {
                // second part of an argument
                if (waitingForSecondArgument)
                {
                    arguments[currArg] = arg;
                    waitingForSecondArgument = false;
                }
                // location of input file
                else if (!arg.StartsWith("/"))
                {
                    inputPath = arg;
                    if (inputPath.EndsWith("\"") && inputPath.StartsWith("\""))
                    {
                        inputPath = inputPath.Substring(1, inputPath.Length - 2);
                    }
                }
                // argument (or first part of argument)
                else
                {
                    arguments.Add(arg, "");
                    currArg = arg;
                    waitingForSecondArgument = IsSecondArgumentRequred(arg);
                }
            }
            return arguments;
        }

        /// <summary>
        /// Whether the argument has a second part
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private static bool IsSecondArgumentRequred(string arg)
        {
            switch (arg)
            {
                //case "/t":
                case "/m":
                    return true;
                default:
                    return false;
            }
        }

    }
}

