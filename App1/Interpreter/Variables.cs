using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace YetAnotherScriptingLanguage
{
    namespace variables
    {
        
        public class Variable : Function
        {
            public enum type
            {
                Word,
                Decimal,
                Boolean,
                Void,
                Function,
                Keyword,
                Array,
                Invalid
            }

            public Variable()
            {
                Value = null;
                Name = "";
                Type = type.Invalid;
            }

            public Variable(object value, type varType, string name=null )
            {
                Value = value;
                Name = name;
                Type = varType;
            }

            public Variable(object value, string name = null)
            {
                Value = value;
                Name = name;
                Type = Regex.Match(Convert.ToString(value), "[0-9]+([.][0-9]+)?").Success ? variables.Variable.type.Decimal : variables.Variable.type.Word;
            }

            public Variable(Variable t)
            {
                if (t is null) return;
                this.Value = t.Value;
                this.Type = t.Type;
                this.Name = t.Name;
            }

            public object Value { get; set; }
            public String Name { get; set; }
            public virtual Variable.type Type { get; set; }

            public static Variable operator +(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable(Convert.ToDouble(left.Value) + Convert.ToDouble(right.Value), right.Type);
                        case type.Word:
                            return new Variable((string)left.Value + (string)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static Variable operator *(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable(Convert.ToDouble(left.Value) * Convert.ToDouble(right.Value), right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static Variable operator /(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            if(Convert.ToDouble(right.Value) == 0.0)
                                throw new Exception("Invalid Operation : Dividing by ZERO");
                            else 
                                return new Variable(Convert.ToDouble(left.Value) / Convert.ToDouble(right.Value), right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static Variable operator ^(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable(Math.Pow(Convert.ToDouble(left.Value) ,Convert.ToDouble(right.Value)), right.Type);
                    }

                }
                throw new Exception("Operation Undefined ^(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static Variable operator -(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable(Convert.ToDouble(left.Value) - Convert.ToDouble(right.Value), right.Type);
                    }

                }
                throw new Exception("Operation Undefined -(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }
            
            public static Variable operator %(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Decimal:
                            double lhs = Convert.ToDouble(left.Value);
                            double rhs = Convert.ToDouble(right.Value);
                            if (lhs == Math.Truncate(lhs) && rhs == Math.Truncate(rhs))
                                return new Variable(Convert.ToInt32(left.Value) % Convert.ToInt32(right.Value), type.Decimal);
                            else
                                throw new Exception("Operation % Takes ( integer" + ", " + "integer )");
                    }
                throw new Exception("Operation Undefined %(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static bool operator ==(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Decimal:
                            {
                                double lhs = Convert.ToDouble(left.Value);
                                double rhs = Convert.ToDouble(right.Value);
                                return lhs == rhs;
                            }
                        case type.Word:
                            {
                                string lhs = Convert.ToString(left.Value);
                                string rhs = Convert.ToString(right.Value);
                                return String.Compare(lhs, rhs) == 0;
                            }
                        case type.Boolean:
                            {
                                bool lhs = Convert.ToBoolean(left.Value);
                                bool rhs = Convert.ToBoolean(right.Value);
                                return lhs == rhs;
                            }
                    }
                return false;
            }

            public static bool operator !=(Variable left, Variable right)
            {
                return !(left==right);
            }

            public static bool operator <(Variable left, Variable right)
            {
                if(left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Word:
                            var leftOperand = Convert.ToString(left.Value);
                            var rightOperand = Convert.ToString(right.Value);
                            return String.Compare(leftOperand, rightOperand) < 0;
                        case type.Decimal:
                            return Convert.ToDouble(left.Value) < Convert.ToDouble(right.Value);
                    }
                throw new Exception("Operation Undefined <(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static bool operator >(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Word:
                            var leftOperand = Convert.ToString(left.Value);
                            var rightOperand = Convert.ToString(right.Value);
                            return String.Compare(leftOperand, rightOperand) > 0;
                        case type.Decimal:
                            return Convert.ToDouble(left.Value) > Convert.ToDouble(right.Value);
                    }
                throw new Exception("Operation Undefined >(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static bool operator &(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return Convert.ToBoolean(left.Value) && Convert.ToBoolean(right.Value);
                    }
                throw new Exception("Operation Undefined &(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }

            public static bool operator ~(Variable left)
            {
                    switch (left.Type)
                    {
                        case type.Boolean:
                        return Convert.ToBoolean(left.Value) ^ true;
                    }
                throw new Exception("Operation Undefined ^(" + left.Type.ToString() + ")");
            }

            public static bool xor(Variable left , Variable right)
            {
                switch (right.Type)
                {
                    case type.Boolean:
                        return Convert.ToBoolean(right.Value) ^ true;
                }
                throw new Exception("Operation Undefined !(" + left.Type.ToString() + ")");
            }

            public static bool operator |(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return Convert.ToBoolean(left.Value) || Convert.ToBoolean(right.Value);
                    }
                throw new Exception("Operation Undefined |(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");
            }


        }
        public class Array : variables.Variable
        {
            public type innerType { get; set; }
            public Array(object value, type t, type i, string name) : base(value, t, name)
            {
                innerType = i;
            }
            public Array(Array r)
            {
                this.Value = r.Value;
                this.Type = r.Type;
                this.Name = r.Name;
                this.innerType = r.innerType;
            }

            public Array(Token data)
            {
                var bitsAndPieces = (new Token(data.Word.Substring(1,data.Word.Length-2))).Spread().Trim();
                List<variables.Variable> vars = new List<variables.Variable>();
                TokensList curr = new TokensList();
                for (int i = 0; i < bitsAndPieces.Count; i++)
                {
                    if (bitsAndPieces[i].IsKeyword != "NEXT_ARG")
                    {
                        curr.Add(bitsAndPieces[i]);
                    }
                    if (bitsAndPieces[i].IsKeyword == "NEXT_ARG" || i == bitsAndPieces.Count - 1)
                    {
                        vars.Add(Parser.Evaluate(Parser.Parse(curr)));
                        curr.Clear();
                    }
                }
                for(int i = 0; i < vars.Count - 1; i++)
                {
                    if (vars[i].Type != vars[i + 1].Type)
                        throw new Exception("Invalid Type Initiliazation");
                }
                this.Value = vars;
                this.Type = type.Array;
                this.innerType = vars[0].Type;
            }

            public static variables.Array Insert(variables.Array l, variables.Variable r)
            {
                if (l.innerType != r.Type) throw new Exception("Type missmatch l is a collection of type :" + l.innerType.ToString());
                ((List<variables.Variable>)l.Value).Add(r);
                return l;
            }

            public static variables.Array Insert(variables.Variable l, variables.Array r)
            {
                if (r.innerType != l.Type) throw new Exception("Type missmatch l is a collection of type :" + r.innerType.ToString());
                ((List<variables.Variable>)r.Value).Insert(0, l);
                return r;
            }

            public static variables.Array Insert(variables.Array l, variables.Array r)
            {
                if (l.innerType != r.innerType) throw new Exception("Type missmatch l is a collection of type :" + l.innerType.ToString());
                foreach (var v in (List<variables.Variable>)r.Value)
                {
                    ((List<variables.Variable>)l.Value).Insert(0, v);
                }
                return l;
            }

            public static variables.Array Remove(variables.Variable l, variables.Variable r)
            {
                variables.Array left  = l as variables.Array;
                if (left is null || r.Type != type.Decimal || Convert.ToInt32(r.Value)!=Math.Truncate(Convert.ToDouble(r.Value))) throw new Exception("Type missmatch l is a collection of type :" + left.innerType.ToString());
                ((List<variables.Variable>)left.Value).RemoveAt(Convert.ToInt32(r.Value));
                return left;
            }

            public static variables.Array Insert(variables.Variable l, variables.Variable r)
            {
                variables.Array right = r as variables.Array;
                variables.Array left  = l as variables.Array;
                if (!(right is null) && !(left is null))
                {
                    return variables.Array.Insert(left, right);
                }
                else if (right is null && !(left is null))
                {
                    return variables.Array.Insert(left, r);
                }
                else if (!(right is null) && left is null)
                {
                    return variables.Array.Insert(l, right);
                }
                else
                    throw new Exception("Operation Undefined ::(" + left.Type.ToString() + " , " + right.Type.ToString() + ")");

            }

        }
    }
}
