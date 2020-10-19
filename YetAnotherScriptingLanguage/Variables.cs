using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    namespace variables
    {
        
        public class Variable
        {
            public enum type
            {
                Word,
                Decimal,
                Boolean,
                Void,
                function,
                Invalid
            }
            public Variable(object value, type varType, string name=null )
            {
                Value = value;
                Name = name;
                Type = varType;
            }
            public object Value { get; set; }
            public String Name { get; set; }
            public virtual Variable.type Type
            {
                get
                {
                    if (Type == Variable.type.function) return type.function;
                    switch (Value.GetType().Name)
                    {
                        case "Double": return Variable.type.Decimal;
                        case "Boolean": return Variable.type.Boolean;
                        case "String": return Variable.type.Word;
                        default: return Variable.type.Invalid;
                    }
                }
                set { }
            }


            public static Variable operator +(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable((double)left.Value + (double)right.Value, right.Type);
                        case type.Word:
                            return new Variable((string)left.Value + (string)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static Variable operator *(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable((double)left.Value * (double)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static Variable operator /(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            if((double)right.Value == 0)
                                throw new Exception("Invalid Operation : Dividing by ZERO");
                            else 
                                return new Variable((double)left.Value * (double)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined +(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static Variable operator ^(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable(Math.Pow((double)left.Value, (double)right.Value), right.Type);
                    }

                }
                throw new Exception("Operation Undefined ^(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static Variable operator -(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                {
                    object neVal = left.Value;
                    switch (left.Type)
                    {
                        case type.Decimal:
                            return new Variable((double)left.Value - (double)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined -(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator ==(Variable left, Variable right)
            {
                return left.Type == right.Type && left.Value == right.Value;
            }

            public static bool operator !=(Variable left, Variable right)
            {
                return left.Type != right.Type || left.Value != right.Value;
            }

            public static bool operator <(Variable left, Variable right)
            {
                if(left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Word:
                            var leftOperand = ((string)left.Value).ToCharArray();
                            var rightOperand = ((string)right.Value).ToCharArray();
                            return rightOperand.Length == 1 && leftOperand.Length == 1 && leftOperand[0] < rightOperand[0];
                        case type.Decimal:
                            return (double)left.Value < (double)right.Value;
                    }
                throw new Exception("Operation Undefined <(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator >(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Word:
                            var leftOperand = ((string)left.Value).ToCharArray();
                            var rightOperand = ((string)right.Value).ToCharArray();
                            return rightOperand.Length == 1 && leftOperand.Length == 1 && leftOperand[0] > rightOperand[0];

                        case type.Decimal:
                            return (double)left.Value > (double)right.Value;
                    }
                throw new Exception("Operation Undefined >(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator &(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return (bool)left.Value && (bool)right.Value;
                    }
                throw new Exception("Operation Undefined &(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator |(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return (bool)left.Value || (bool)right.Value;
                    }
                throw new Exception("Operation Undefined |(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static variables.Variable operator %(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Decimal:
                            double lhs = (double)left.Value;
                            double rhs = (double)right.Value;
                            if (lhs == Math.Truncate(lhs) && rhs == Math.Truncate(rhs))
                                return new Variable((int)left.Value % (int)right.Value,type.Decimal);
                            else
                                throw new Exception("Operation % Takes ( integer , integer )");
                    }
                throw new Exception("Operation Undefined %(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

        }
        class Array<T>
        {
            public Array(int len = 0){
                elements = new List<Variable>(len);
            }
            public List<Variable> elements;
            public IList<Variable> Elements => elements;
        }
    }
}
