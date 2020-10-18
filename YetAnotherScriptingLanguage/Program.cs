using System;

namespace YetAnotherScriptingLanguage
{
    class Program
    {
        static void Main(string[] args)
        {
            TranslationUnit unit = new TranslationUnit(
                "variable a as integer\n" +
                "a := 23*(15+16)\n" +
                "variable the_truth as word\n" +
                "//TESTING COMMENTS\n" +
                "the_truth:='23 is a prime'\n" +
                "if ( a <> 2 ) then\n" +
                "{\n" +
                "a = 23*3\n" +
                "}\n");
            foreach (var token in unit.Tokens)
            {
                if(token.IsKeyword!= "END_STATEMENT" && token.IsKeyword!= "SPACE")
                    Console.WriteLine(token.Word);
            }
        }
    }
}
