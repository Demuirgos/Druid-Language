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
            public Variables(string name, T value)
            {
                Value = value;
                Name = name;
            }
            public T Value { get; set; }
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
            public Variable(string name, object value,string varType) : base(name, value) {
                setType(varType);
            }
            private void setType(string varType)
            {
                switch (varType)
                {
                    case ("Decimal"):
                        Type = type.Decimal;
                        break;
                    case ("Word"):
                        Type = type.Word;
                        break;
                    case ("Boolean"):
                        Type = type.Boolean;
                        break;
                    default:
                        Type = type.Invalid;
                        break;
                }
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
