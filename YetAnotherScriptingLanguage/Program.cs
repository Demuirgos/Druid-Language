using System;

namespace YetAnotherScriptingLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            TranslationUnit unit = new TranslationUnit(
                "variable a:integer\n" +
                "a := 23\n" +
                "variable the_truth:word\n" +
                "the_truth:='23 is a prime'\n" +
                "if ( a <> 2 ) then\n" +
                "{\n" +
                "a = 23*3\n" +
                "}\n");
            foreach (var token in unit.Tokens)
            {
                if(token.Word!= "\n" && token.Word!=" ")
                    Console.WriteLine(token.Word);
            }
        }
    }
}
