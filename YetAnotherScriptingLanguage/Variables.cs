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
            Invalid
        }
        class Variables<T>
        {
            T typeRef;
            public Variables(string name, T value)
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
            public Variable(string name, object value,type varType) : base(name, value) {
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
                            return new Variable("NameLess", (double)left.Value + (double)right.Value, right.Type);
                        case type.Word:
                            return new Variable("NameLess", (string)left.Value + (string)right.Value, right.Type);
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
                            return new Variable("NameLess", (double)left.Value * (double)right.Value, right.Type);
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
                                return new Variable("NameLess", (double)left.Value * (double)right.Value, right.Type);
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
                            return new Variable("NameLess", Math.Pow((double)left.Value, (double)right.Value), right.Type);
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
                            return new Variable("NameLess", (double)left.Value - (double)right.Value, right.Type);
                    }

                }
                throw new Exception("Operation Undefined -(" + left.Type.ToString() + right.Type.ToString() + ")");
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
