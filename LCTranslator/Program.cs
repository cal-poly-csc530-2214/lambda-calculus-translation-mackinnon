using LCTranslator.Analysis;
using LCTranslator.Translation;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.IO;
using System.Threading.Tasks;

namespace LCTranslator
{
    internal class Program
    {
        private static async Task<int> Main(string[] args)
        {
            if (!TryParseCommandLineArgs(args, out var inFile))
            {
                Console.Error.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} [-i in_file]");
                return 1;
            }

            if (!TryReadFileText(inFile, out var lcText))
            {
                return 1;
            }

            if (!TryTranslate(lcText, out var cSharp))
            {
                return 1;
            }

            if (!DoesUserWantToRun())
            {
                Console.WriteLine("Weak.");
                return 0;
            }

            Console.WriteLine("So you have chosen death.");

            if (!await TryRunCSharpAsync(cSharp))
            {
                return 1;
            }

            // We did it!
            return 0;
        }

        private static bool TryTranslate(string lcCode, out string cSharpCode)
        {
            try
            {
                var parser = new Parser(lcCode);
                var typeInferrer = new TypeInferrer();
                var lcTranslator = new ExprToLCTranslator();
                var cSharpTranslator = new ExprToCSharpTranslator();

                var program = parser.ParseExpression();
                typeInferrer.InferTypes(program);

                var typedLcCode = lcTranslator.Translate(program);
                cSharpCode = cSharpTranslator.Translate(program);

                Console.WriteLine($"---Translated LC (typed)---\n{typedLcCode}\n");
                Console.WriteLine($"-------Translated C#-------\n{cSharpCode}\n");

                return true;
            }
            catch (LCException e)
            {
                cSharpCode = string.Empty;

                Console.Error.WriteLine($"ERROR: {e.Message}");
                return false;
            }
        }

        private static async Task<bool> TryRunCSharpAsync(string cSharp)
        {
            var script = CSharpScript.Create(cSharp)
                .WithOptions(ScriptOptions.Default.WithImports("System"));

            Console.WriteLine("Running C# code...");

            ScriptState<object> state;

            try
            {
                state = await script.RunAsync();
            }
            catch (CompilationErrorException e)
            {
                Console.Error.WriteLine($"A compilation error occurred: {e.Message}");
                return false;
            }

            if (state.Exception is not null)
            {
                Console.Error.WriteLine($"Exception encountered in script: {state.Exception.Message}");
                return false;
            }

            Console.WriteLine($"Expression returned '{state.ReturnValue ?? "void"}'.");

            return true;
        }

        private static bool DoesUserWantToRun()
        {
            Console.Write("Would you like to run the C# code (y/n)? ");

            while (true)
            {
                var response = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(response))
                {
                    continue;
                }

                switch (response.Trim().ToLower())
                {
                    case "y":
                        return true;
                    case "n":
                        return false;
                    default:
                        Console.Write("Please type 'y' or 'n': ");
                        break;
                }
            }
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

        private static bool TryReadFileText(string fileName, out string contents)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                contents = Console.In.ReadToEnd();
                return true;
            }

            try
            {
                contents = File.ReadAllText(fileName);
                return true;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Unable to open file for reading: {e.Message}");
                contents = string.Empty;
                return false;
            }
        }
    }
}
