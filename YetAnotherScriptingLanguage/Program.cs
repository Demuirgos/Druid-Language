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
                String code1 =
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
                String code2 =
                    "Variable n as Decimal" + Environment.NewLine +
                    "n := 23" + Environment.NewLine +
                    "If n%2=0 Then" + Environment.NewLine +
                    "Print(n,' is divisible by 2')" + Environment.NewLine +
                    "Else If n%3=0 Then" + Environment.NewLine +
                    "Print(n,' is divisible by 3')" + Environment.NewLine +
                    "Else" + Environment.NewLine +
                    "Print(n,' is neither divisible by 2 or 3')" + Environment.NewLine +
                    "End" + Environment.NewLine;
                String code3=
                    "Variable n as Decimal" + Environment.NewLine +
                    "n := 22" + Environment.NewLine +
                    "If n%2<>0 Then" + Environment.NewLine +
                    "Print(n,' is not divisible by 2')" + Environment.NewLine +
                    "End" + Environment.NewLine;
                String code4 =
                    "Function isPrime(n as Decimal) as Boolean"     + Environment.NewLine +
                    "Begin"                                         + Environment.NewLine +
                    "   Variable value as Boolean"                  + Environment.NewLine +
                    "   value:=True"                                + Environment.NewLine +
                    "   If n%2=0 Then"                              + Environment.NewLine +
                    "       Return n=2"                             + Environment.NewLine +
                    "   Else"                                       + Environment.NewLine +
                    "       For i From 3 To floor(sqrt(n)) By 2 Do" + Environment.NewLine +
                    "          Print(i)"                            + Environment.NewLine +
                    "          If n%i=0 Then"                       + Environment.NewLine +
                    "             Return False"                     + Environment.NewLine +
                    "          End"                                 + Environment.NewLine +
                    "       End"                                    + Environment.NewLine +
                    "   End"                                        + Environment.NewLine +
                    "   Return True"                                + Environment.NewLine +
                    "End"                                           + Environment.NewLine +
                    "Variable i as Decimal"                         + Environment.NewLine +
                    "i := Read('Entrez le nombre')"                 + Environment.NewLine +
                    "If isPrime(i) Then"                            + Environment.NewLine +
                    "   Print(i,'is prime')"                        + Environment.NewLine +
                    "Else"                                          + Environment.NewLine +
                    "   Print(i,'is not prime')"                    + Environment.NewLine +
                    "End" + Environment.NewLine;
                BasicSCriptTest(false, code4);

                // add deep return / exit / skip
                // add arrays 
                // add type defs
                // add IDE
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            Console.Read();
        }
    }
}
