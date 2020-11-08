using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
            bool isMathEvaluation = false;
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
                        if (contains(Operators,currentChar))
                        {
                            while (contains(Operators, nextChar))
                            {
                                currentWord.Append(nextChar);
                                i++;
                                nextChar = TranslationCode[i + 1];
                            }
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
                            do
                            {
                                nextChar = TranslationCode[++i];
                            } while (nextChar != '\n');
                            currentWord.Clear();
                        }
                        else if (currentChar == '[')
                        {
                            int balance = 1;
                            do
                            {
                                nextChar = TranslationCode[++i];
                                currentWord.Append(nextChar);
                                if (nextChar == '[')
                                    balance++;
                                if (nextChar == ']')
                                    balance--;
                            } while (balance > 0);
                        }
                        else if (currentChar == '(')
                        {
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
                    Tokens.Add(new Token(currentWord.ToString()));
                    currentWord.Clear();
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
        public static char[] Operators => ("<>=+-*/%&|^:!").ToCharArray();
    }
    public class Token
    {
        public enum type
        {
            keyword,
            variable,
            array,
            constant,
            operation,
            Separator,
            Skip,
            Exit,
            Ender,
            Math,
            function,
            Unexpected
        }

        public Token(String _word)
        {
            word = _word;
        }

        public TokensList Spread()
        {
            return (new TranslationUnit(this.Word)).Tokens;
        }

        public static bool operator ==(Token left, Token right)
        {
            return (left.Word == right.Word || left.IsKeyword == right.IsKeyword);
        }

        public static bool operator !=(Token left, Token right)
        {
            return !(left == right);
        }

        static private KeyWords Dictionary => new KeyWords();
        private String word;
        public String Word => (word.StartsWith('\'') && word.EndsWith('\'')) ||(word.StartsWith('(') && word.EndsWith(')')) ? word.Substring(1, word.Length - 2) : word;
        public Token.type Type {
            get
            {
                
                if(word.StartsWith('(') && word.EndsWith(')'))
                {
                    return type.Math;
                }
                else if(word == "\r\n" || word == "\n" || word == "\r\t" || word == "\r")
                {
                    return type.Ender;
                }
                else if (word == " " || word == "\r" || word == "\t")
                {
                    return type.Separator;
                }
                else if (word.StartsWith('[') && word.EndsWith(']'))
                {
                    return type.array;
                }
                else if (Interpreter.Functions.ContainsKey(this.IsKeyword) || Interpreter.Actions.ContainsKey(this.IsKeyword))
                {
                    return Token.type.function;
                }
                else if (Interpreter.Keywords.ContainsKey(this.Word))
                {
                    if (Interpreter.Keywords[this.Word] == "EXIT")
                        return type.Exit;
                    if (Interpreter.Keywords[this.Word] == "SKIP")
                        return type.Skip;
                    return Token.type.keyword;
                }
                else if (Interpreter.ExecutionStack.Count> 0 && Interpreter.CurrentBlock.Variables.ContainsKey(this.IsKeyword))
                {
                    return Token.type.variable;
                }
                else if ((word.StartsWith('\'') && word.EndsWith('\'')) || Regex.Match(word, "[0-9]+([.][0-9]+)?").Success)
                {
                    return type.constant;
                }
                else
                {
                    return Token.type.Unexpected;
                }
            }
        }
        public String IsKeyword {
            get => Dictionary[word];
        }
    }
    public class TokensList : IEnumerable<Token>, IEnumerable
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
        public void Add(Token token,int i=-1)
        {
            if (i == -1)
                mylist.Add(token);
            else
                mylist.Insert(i,token) ;
        }
        public TokensList Remove(Token t)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == t)
                {
                    this.Remove(i);
                    i--;
                }
            }
            return this;
        }
        public bool HasToken(Token t)
        {
            for (int i = 0; i < this.Count; i++)
            {
                if (this[i] == t)
                {
                    return true;
                }
            }
            return false;
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
                if (t.Word == "CURRENT")
                {
                    return this[l, l];
                }
                else if (t.Word == "NEXT")
                {
                    return this[l, l + 1];
                }
                else if (t.Word == "PREVIOUS")
                {
                    return this[l - 1, new Token("END_STATEMENT")];
                }
                var result = new TokensList();
                Dictionary<String, List<String>> opposites = new Dictionary<String, List<String>>();
                opposites["END"] = new List<string>() { "BEGIN", "THEN", "DO" };
                opposites["ELSE"] = new List<string>() { "IF" };
                int balanced = 0;
                for (int i = l; i<this.Count; i++)
                {
                    result.Add(this[i]);
                    if (opposites.ContainsKey(t.IsKeyword))
                    {
                        if (opposites[t.IsKeyword].Contains(this[i].IsKeyword)) 
                            balanced++;
                        if (this[i] == t || (this[i].IsKeyword == "ELSE" && this[i + 1].IsKeyword == "IF")) 
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
