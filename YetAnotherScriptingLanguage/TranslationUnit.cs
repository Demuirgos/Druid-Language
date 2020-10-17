using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class TranslationUnit
    {
        public class Block
        {
            public Block(string name = null)
            {
                Name = name;
            }
            public Dictionary<string, Function> Variables { get; set; }
            public string Name { get; set; }
        }
        public TranslationUnit(String code)
        {
            Code = code;
            Tokens = Tokenizer.Tokenize(this);

        }
        public static TranslationUnit operator +(TranslationUnit prefix,TranslationUnit suffix) => new TranslationUnit(prefix.Code + "\n" + suffix.Code);
        public TokensList Tokens { get; set; }
        public String Code { get; set; }
        private static Stack<Block> levels = new Stack<Block>();
        public static Stack<Block> ExecutionStack => levels;
        public static Block CurrentBlock => levels.Peek();
    }
}
