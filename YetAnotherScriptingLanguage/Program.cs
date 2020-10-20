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
        static void MathEvalTest()
        {
            while (true)
            {
                Console.WriteLine("Evaluating simple Math test : ");
                TranslationUnit tes = new TranslationUnit(Console.ReadLine());
                var res = (variables.Variable)Parser.Evaluate(Parser.Parse(tes.Tokens));
                Console.WriteLine(tes.Code + " => " + res.Value);
            }
        }
        static void Main(string[] args)
        {
            var Interpreter = new Interpreter("");
            try
            {
                MathEvalTest();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
