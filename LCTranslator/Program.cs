using LCTranslator.Translators;
using System;
using System.IO;

namespace LCTranslator
{
    internal class Program
    {
        private static int Main(string[] args)
        {
            if (!TryParseCommandLineArgs(args, out var inFile))
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
                var parser = new Parser(lcText);
                program = parser.ParseProgram();
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

            //var result3 = new Func<Func<Func<Func<int, int>, int>, int>, int>(x => x(y => y()));
            //var result3 = new Func<Func<int, int>, int>((x => x)((y => y)(z => z)));
            //var result3 = new Func<int, Func<int, Func<int, int>>>(z => y => x => x)(3);

            // INTERESTING:
            //var result3 = new Func<int, Func<int, Func<int, int>>>(x => new Func<int, Func<int, int>>(y => new Func<int, int>(z => z)));

            var result3 = new Func<Func<int, int>, Func<int, int>>(x => x)(new Func<int, int>(y => y))(3);

            // Any way to simplify the previous?
            // UPDATE: 
            var result4 = new Func<Func<int, int>, Func<int, int>>(x => x)(y => y);

            var n = 5;
            var result5 = new Func<Func<Func<int, int>, Func<int, int>>, Func<Func<int, int>, Func<int, int>>>(x => x)(y => y)(z => z)(n);

            return 0;
        }

        private static bool TryParseCommandLineArgs(string[] args, out string inFile)
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
