using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    namespace variables
    {
        public enum type
        {
            Word,
            Decimal,
            Boolean,
            Void,
            Invalid
        }
        class Variables<T>
        {
            T typeRef;
            public Variables(T value, string name=null )
            {
                Value = value;
                Name = name;
            }
            public object Value { get; set; }
            public String Name { get; set; }
            public virtual type Type
            {
                get
                {
                    switch (Value.GetType().Name)
                    {
                        case "Double": return type.Decimal;
                        case "Boolean": return type.Boolean;
                        case "String": return type.Word;
                        default: return type.Invalid;
                    }
                }
                set { }
            }
        }
        class Variable : Variables<object>
        {
            public Variable(object value, type varType) : base(value)
            {
                Type = varType;
            }
            public Variable(string name ,object value,type varType) : base(value,name) {
                Type = varType;
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
                            if (lhs == Math.Truncate(lhs) && rhs==Math.Truncate(rhs))
                                return new Variable((int)left.Value % (int)right.Value,type.Decimal);
                            else
                                throw new Exception("Operation % Takes ( integer , integer )");
                    }
                throw new Exception("Operation Undefined |(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public override type Type { get; set; }
        }
        class Array<T>
        {
            public Array(int len = 0){
                elements = new List<Variables<T>>(len);
            }
            public List<Variables<T>> elements;
            public IList<Variables<T>> Elements => elements;
        }
    }
}
