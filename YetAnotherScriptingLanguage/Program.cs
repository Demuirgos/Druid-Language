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
            } while (true && input);
        }
        static void Main(string[] args)
        {
            var Interpreter = new Interpreter("");
            Interpreter.Verbose = true;
            try
            {
                MathEvalTest(false,
                    "Variable n as Decimal" + Environment.NewLine +
                    "Variable m as Decimal" + Environment.NewLine +
                    "Variable k as Decimal" + Environment.NewLine +
                    "n := 1+2+3+4" + Environment.NewLine +
                    "m := 1*2*3*4" + Environment.NewLine +
                    "k := m-n" + Environment.NewLine +
                    "Print('factorial(m) != sum(1,m) coz (k=m-n) =>',k,'<>',0)" + Environment.NewLine + 
                    "n := 1+2+3" + Environment.NewLine +
                    "m := 1*2*3" + Environment.NewLine +
                    "k := m-n" + Environment.NewLine +
                    "Print('but factorial(3) != sum(1,3) coz (k=m-n) =>',k,'==',0)" + Environment.NewLine
                    );
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
