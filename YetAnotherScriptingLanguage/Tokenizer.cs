using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YetAnotherScriptingLanguage
{
    class Tokenizer
    {
        public static TokensList Tokenize(TranslationUnit code)
        {
            var Tokens = new TokensList();
            Func<char[], char, bool> contains = (char[] chars, char key) =>
            {
                foreach (var c in chars)
                {
                    if (c == key) return true;
                }
                return false;
            };
            StringBuilder currentWord = new StringBuilder();
            string TranslationCode = code.Code + Environment.NewLine;
            bool isHigh = false;
            for (var i = 0;i< TranslationCode.Length; i++)
            {
                var currentChar = TranslationCode[i];
                if(i == TranslationCode.Length-1 || contains(Tokenizer.Separators,currentChar))
                {
                    if(currentWord.Length != 0)
                    {
                        Tokens.Add(new Token(currentWord.ToString()));
                        currentWord.Clear();
                    }
                    currentWord.Append(currentChar);
                    if (i + 1 < TranslationCode.Length)
                    {
                        var nextChar = TranslationCode[i + 1];
                        if ((currentChar == '<' && nextChar == '>') || (currentChar == ':' && nextChar == '='))
                        {
                            currentWord.Append(nextChar);
                            i++;
                        }
                        else if (currentChar == '\'')
                        {
                            while (nextChar != '\'' && i + 1 < TranslationCode.Length)
                            {
                                nextChar = TranslationCode[++i];
                                currentWord.Append(nextChar);
                            }
                        }
                        else if (currentChar == '/' && nextChar == '/')
                        {
                            currentWord.Append(TranslationCode[++i]);
                            Tokens.Add(new Token(currentWord.ToString()));
                            currentWord.Clear();
                            do
                            {
                                nextChar = TranslationCode[++i];
                                currentWord.Append(nextChar);
                            } while (nextChar != '\n');
                        }
                        else if (currentChar == '(')
                        {
                            isHigh = true;
                            int balance = 1;
                            do
                            {
                                nextChar = TranslationCode[++i];
                                currentWord.Append(nextChar);
                                if (nextChar == '(') 
                                    balance++;
                                if (nextChar == ')') 
                                    balance--;
                            } while (balance > 0);
                            currentWord.Remove(0, 1);
                            currentWord.Remove(currentWord.Length-1, 1);
                        }
                        else if(currentChar == '\r' && nextChar == '\n' || currentChar=='\n')
                        {
                            if(currentChar == '\r' && nextChar == '\n')
                            {
                                i++;
                                currentWord.Append(nextChar);
                            }
                        }
                    }
                    Tokens.Add(new Token(currentWord.ToString(),isHigh));
                    currentWord.Clear();
                    isHigh = false;
                }
                else
                {
                    currentWord.Append(currentChar);
                }
            }
            return Tokens;
        }
        public static char[] Separators => ("'<>=+-*/%&|^\t (){}[]:,!\0" + Environment.NewLine).ToCharArray();
    }
    class Token
    {
        public enum type
        {
            keyword,
            variable,
            constant,
            operation,
            function
        }

        public Token(String word, bool isHigh = false)
        {
            Word = word;
            IsFunction = isHigh;
        }

        public TokensList Spread()
        {
            return (new TranslationUnit(this.Word)).Tokens;
        }
        
        static private KeyWords Dictionary = new KeyWords();
        public bool IsFunction { get; set; }
        public String Word { get; }
        public Token.type Type {
            get
            {
                if (Regex.Match(this.Word, "[0-9]+([.][0-9]+)?").Success)
                {
                    return Token.type.constant;
                }
                else if (Interpreter.Keywords.ContainsKey(this.Word))
                {
                    return Token.type.keyword;
                }
                else
                {
                    return Token.type.function;
                }
            }
        }
        public String IsKeyword {
            get => Dictionary[Word];
        }
    }
    class TokensList : IEnumerable<Token>, IEnumerable
    {
        List<Token> mylist = new List<Token>();
        public int Count => mylist.Count;

        public Token this[int index]
        {
            get { return mylist[index]; }
            set { mylist.Insert(index, value); }
        }

        public void Add (Token token)
        {
            mylist.Add(token);
        }

        public void Clear(Token token)
        {
            mylist.Clear();
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return mylist.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
