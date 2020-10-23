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
        static void MathEvalTest(bool input = true, string val = "")
        {
            do
            {
                Console.WriteLine("Evaluating simple Math test : ");
                TranslationUnit tes = !input ? new TranslationUnit(val) : new TranslationUnit(Console.ReadLine());
                var res = (variables.Variable) Parser.Evaluate(Parser.Parse(tes.Tokens));
                Console.Write(val + " => " + res.Value);
            } while (true && input);
        }
        static void BasicSCriptTest(bool input = true, string val = "")
        {
            do
            {
                Console.WriteLine("Running Basic Script :");
                TranslationUnit tes = !input ? new TranslationUnit(val) : new TranslationUnit(Console.ReadLine());
                var res = (variables.Variable)Parser.Evaluate(Parser.Parse(tes.Tokens));
            } while (true && input);
        }
        static void Main(string[] args)
        {
            var Interpreter = new Interpreter("");
            Interpreter.Verbose = true;
            try
            {
                String code =
                    "Variable n as Decimal" + Environment.NewLine +
                    "Variable i as Decimal" + Environment.NewLine +
                    "Variable k as Decimal" + Environment.NewLine +
                    "k := Read('Enter the number to calculate the Factorial : ')" + Environment.NewLine +
                    "n := 1" + Environment.NewLine +
                    "i := 1" + Environment.NewLine +
                    "While i < k + 1 Do" + Environment.NewLine +
                    "n := n * i" + Environment.NewLine +
                    "i := i + 1" + Environment.NewLine +
                    "End" + Environment.NewLine +
                    "Print('factorial of :',k,' is ',n)" + Environment.NewLine;
                BasicSCriptTest(false, code);
                MathEvalTest(false,"fact(6)");
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.Read();
        }
    }
}
