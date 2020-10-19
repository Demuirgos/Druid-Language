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
                            return new Variable(Convert.ToDouble(left.Value) * Convert.ToDouble(right.Value), right.Type);
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
                            if(Convert.ToDouble(right.Value) == 0.0)
                                throw new Exception("Invalid Operation : Dividing by ZERO");
                            else 
                                return new Variable(Convert.ToDouble(left.Value) / Convert.ToDouble(right.Value), right.Type);
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
                            return new Variable(Math.Pow(Convert.ToDouble(left.Value) ,Convert.ToDouble(right.Value)), right.Type);
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
                            return new Variable(Convert.ToDouble(left.Value) - Convert.ToDouble(right.Value), right.Type);
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
                throw new Exception("Operation Undefined <(" + left.Type.ToString() + right.Type.ToString() + ")");
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
                throw new Exception("Operation Undefined >(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator &(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return Convert.ToBoolean(left.Value) && Convert.ToBoolean(right.Value);
                    }
                throw new Exception("Operation Undefined &(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static bool operator |(Variable left, Variable right)
            {
                if (left.Type == right.Type)
                    switch (left.Type)
                    {
                        case type.Boolean:
                            return Convert.ToBoolean(left.Value) || Convert.ToBoolean(right.Value);
                    }
                throw new Exception("Operation Undefined |(" + left.Type.ToString() + right.Type.ToString() + ")");
            }

            public static variables.Variable operator %(Variable left, Variable right)
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
