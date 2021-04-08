using LCTranslator.Translators;
using System;
using System.IO;

namespace LCTranslator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (!ParseCommandLineArgs(args, out var inFile))
            {
                PrintUsage();
                return 1;
            }

            string lcText;

            if (string.IsNullOrWhiteSpace(inFile))
            {
                lcText = Console.In.ReadToEnd();
            }
            else
            {
                try
                {
                    lcText = File.ReadAllText(inFile);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine($"Unable to open file for reading: {e.Message}");
                    return 1;
                }
            }

            Expr program;

            try
            {
                program = Parser.ParseProgram(lcText);
            }
            catch (LCException e)
            {
                Console.Error.WriteLine($"ERROR: {e.Message}");
                return 1;
            }

            // TODO: Implement the LC -> C# translator.
            var lcTranslator = new ExprToLCTranslator();
            var lc = lcTranslator.Translate(program);

            Console.WriteLine(lc); // Should match original LC.

            return 0;
        }

        private static bool ParseCommandLineArgs(string[] args, out string inFile)
        {
            inFile = string.Empty;

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-i" when i < args.Length - 1:
                        inFile = args[++i];
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }

        private static void PrintUsage()
        {
            var executableName = AppDomain.CurrentDomain.FriendlyName;
            Console.Error.WriteLine($"Usage: {executableName} [-i in_file]");
        }
    }
}
