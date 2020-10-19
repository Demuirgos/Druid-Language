using System;

namespace YetAnotherScriptingLanguage
{
    class Program
    {
        static void tokenizingtest()
        {
            TranslationUnit unit = new TranslationUnit(
                "variable a as integer" + Environment.NewLine +
                "a := 23*(15+16)" + Environment.NewLine +
                "variable the_truth as word" + Environment.NewLine +
                "//TESTING COMMENTS" + Environment.NewLine +
                "the_truth:='23 is a prime'" + Environment.NewLine +
                "if ( a <> 2 ) then" + Environment.NewLine +
                "{" + Environment.NewLine +
                "a = 23*3" + Environment.NewLine +
                "}");
            Console.WriteLine("Tokenzing process test : ");
            foreach (var token in unit.Tokens)
            {
                var str = Environment.NewLine;
                foreach (var word in token.Spread())
                    if (word.Word != Environment.NewLine && word.Word != " ")
                        Console.WriteLine(word.Word);
            }
        }
        static void MathEvalTest()
        {
            while (true)
            {
                Console.WriteLine("Evaluating simple Math test : ");
                TranslationUnit tes = new TranslationUnit(Console.ReadLine());
                var res = (variables.Variable)Parser.Evaluate(Parser.Parse(tes.Tokens));
                Console.WriteLine(tes.Code + " = " + res.Value);
            }
        }
        static void Main(string[] args)
        {
            try
            {
                MathEvalTest();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
