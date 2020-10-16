using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class TranslationUnit
    {
        public TranslationUnit(String code)
        {
            Code = code;
            Tokens = Tokenizer.Tokenize(this);
        }
        public static TranslationUnit operator +(TranslationUnit prefix,TranslationUnit suffix) => new TranslationUnit(prefix.Code + "\n" + suffix.Code);
        public TokensList Tokens { get; set; }
        public String Code { get; set; }
    }
}
