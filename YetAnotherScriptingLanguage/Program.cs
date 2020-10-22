using System;

namespace YetAnotherScriptingLanguage
{
    class Program
    {
        static void tokenizingtest()
        {
            TranslationUnit unit = new TranslationUnit("False" + Environment.NewLine);
            Console.WriteLine("Tokenzing process test : ");
            foreach (var token in unit.Tokens)
            {
                Console.WriteLine(token.Word);
            }
        }
        static void MathEvalTest(bool input = true,string val="")
        {
            do
            {
                Console.WriteLine("Evaluating simple Math test : ");
                TranslationUnit tes = !input ? new TranslationUnit(val) : new TranslationUnit(Console.ReadLine());
                var res = (variables.Variable)Parser.Evaluate(Parser.Parse(tes.Tokens));
                Console.WriteLine(tes.Code + " => " + res.Value);
            } while (true && input);
        }
        static void Main(string[] args)
        {
            var Interpreter = new Interpreter("");
            Interpreter.Verbose = true;
            try
            {
                MathEvalTest(true,
                    "Print('test',15)" + Environment.NewLine +
                    "Variable n as Decimal" + Environment.NewLine +
                    "n:=max(5,7)*3+2" + Environment.NewLine +
                    "min(n,5)");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
