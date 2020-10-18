using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Tokenizer
    {
        public static TokensList Tokenize(TranslationUnit code)
        {
            var Tokens = new TokensList();
            char[] Separators = ("'<>=+-*/%&|^\t (){}[]:,!\n\r\0" + Environment.NewLine).ToCharArray();
            Func<char[], char, bool> contains = (char[] chars, char key) =>
            {
                foreach (char c in chars)
                {
                    if (c == key) return true;
                }
                return false;
            };
            StringBuilder currentWord = new StringBuilder();
            string TranslationCode = code.Code + Environment.NewLine;
            for (var i = 0;i< TranslationCode.Length; i++)
            {
                var currentChar = TranslationCode[i];
                if(i == TranslationCode.Length-1 || contains(Separators,currentChar))
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
                            Tokens.Add(new Token(currentWord.ToString()));
                            currentWord.Clear();
                            do
                            {
                                currentChar = TranslationCode[++i];
                                nextChar = TranslationCode[i+1];
                                currentWord.Append(currentChar);
                            } while (nextChar != ')');
                        }
                    }
                    Tokens.Add(new Token(currentWord.ToString()));
                    currentWord.Clear();
                }
                else
                {
                    currentWord.Append(currentChar);
                }
            }
            return Tokens;
        }
    }
    class Token
    {
        static private KeyWords Dictionary = new KeyWords();
        public Token(String word)
        {
            Word = word;
        }
        public bool IsFunction { get; set; }
        public String Word { get; }
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
