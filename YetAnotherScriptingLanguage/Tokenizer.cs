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
            bool isString = false;
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
                        else if(currentChar == '!')
                        {
                            Tokens.Add(new Token("True"));
                        }
                        else if (currentChar == '\'')
                        {
                            while (nextChar != '\'' && i + 1 < TranslationCode.Length)
                            {
                                nextChar = TranslationCode[++i];
                                currentWord.Append(nextChar);
                            }
                            currentWord.Remove(0, 1);
                            currentWord.Remove(currentWord.Length-1, 1);
                            isString = true;
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
                    Tokens.Add(new Token(currentWord.ToString(),isHigh,isString));
                    currentWord.Clear();
                    isHigh = false;
                    isString = false;
                }
                else
                {
                    currentWord.Append(currentChar);
                }
            }
            Tokens.Trim(false);
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
            Separator,
            Ender,
            function
        }

        public Token(String word, bool isHigh = false, bool _isString = false)
        {
            Word = word;
            isString = _isString;
            IsMathEvaluation = isHigh;
        }

        public TokensList Spread()
        {
            return (new TranslationUnit(this.Word)).Tokens;
        }

        public static bool operator ==(Token left, Token right)
        {
            return left.IsMathEvaluation == right.IsMathEvaluation && (left.Word == right.Word || left.IsKeyword==right.IsKeyword);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }

        static private KeyWords Dictionary = new KeyWords();
        public bool IsMathEvaluation { get; set; }
        public String Word { get; }
        private bool isString { get; set; }
        public Token.type Type {
            get
            {
                if (isString) return type.constant;
                if(Word == "\r\n" || Word == "\n")
                {
                    return type.Ender;
                }
                else if(Word == " " || Word == "\r" || Word == "\t")
                {
                    return type.Separator;
                }
                else if (Interpreter.Functions.ContainsKey(this.Word) || Interpreter.Actions.ContainsKey(this.Word))
                {
                    return Token.type.function;
                }
                else if (Interpreter.Keywords.ContainsKey(this.Word))
                {
                    return Token.type.keyword;
                }
                else if (Interpreter.ExecutionStack.Count> 0 && Interpreter.CurrentBlock.Variables.ContainsKey(this.Word))
                {
                    return Token.type.variable;
                }
                else
                {
                    return Token.type.constant;
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

        public override string ToString()
        {
            string result = "";
            foreach(var token in this.mylist)
            {
                result += token.Word + ", ";
            }
            return result;
        }

        public Token this[int index]
        {
            get { return mylist[index]; }
            set { mylist.Insert(index, value); }
        }

        public TokensList Trim(bool EndersToo = true)
        {
            for(int i = 0; i < this.Count; i++)
            {
                if(this[i].Type == Token.type.Separator || (this[i].Type == Token.type.Ender && EndersToo))
                {
                    this.Remove(i);
                    i--;
                }
            }
            return this;
        }
        public TokensList this[int l,Token left, Token limit]
        {
            get
            {
                var result = new TokensList();
                int i = l;
                for (; i < this.Count && this[i] != left; i++) ;
                return this[i, limit];
            }
        }
        public TokensList this[int l,Token t]
        {
            get
            {
                if (t.Word == "Current")
                {
                    return this[l, l];
                }
                else if (t.Word == "Next")
                {
                    return this[l, l + 1];
                }
                else if (t.Word == "Previous")
                {
                    return this[l - 1, new Token("END_STATEMENT")];
                }
                var result = new TokensList();
                Dictionary<String, String> opposites = new Dictionary<String, String>
                {
                    ["End"] = "Begin",
                    ["Else"] = "If"
                };
                int balanced = 0;
                for (int i = l; i<this.Count; i++)
                {
                    result.Add(this[i]);
                    if (opposites.ContainsKey(t.IsKeyword))
                    {
                        if (this[i].IsKeyword == opposites[t.IsKeyword]) 
                            balanced++;
                        if (this[i] == t) 
                            balanced--;
                        if (this[i] == t && balanced == 0) 
                            break;
                    }
                    else
                    {
                        if (this[i] == t) break;
                    }
                }
                return result;
            }
        }

        public TokensList this[int l,int r]
        {
            get {
                var result = new TokensList();
                for(int i = l; i <= r; i++)
                {
                    result.Add(this[i]);
                }
                return result;
            }
            set
            {
                for (int i = l; i < r; i++)
                {
                    this.Remove(i);
                }
                for (int i = l, j = 0; i < r; i++, j++) {
                    this.Add(value[j], i);
                }
            }
        }

        public void Add(Token token,int i=-1)
        {
            if (i == -1)
                mylist.Add(token);
            else
                mylist.Insert(i,token) ;
        }

        public TokensList Remove(int idx = -1)
        {
            if (idx == -1) mylist.RemoveAt(this.Count - 1);
            else 
                mylist.RemoveAt(idx);
            return this;
        }

        public void Clear()
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
