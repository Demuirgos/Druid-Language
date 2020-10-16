using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Function
    {
        public Function()
        {
            Implimentation = this;
            Name = null;
        }
        public Function(string name)
        {
            Implimentation = this;
            Name = name;
        }
        public variables.Variable this[TokensList s,int idx]{
            get
            {
                return Implimentation.Evaluate(s, idx);
            }
        }
        protected virtual variables.Variable Evaluate(TokensList data,int idx) => throw new Exception("Not Implemented");
        public Function Implimentation { get; set; }
        public String Name { get; set; }
        public TokensList tokens { get; set; }
        public int index ;
    }

    class ConditionalProcess : Function 
    {
        private TokensList condition;
        public Token delimiter { get; set; }
        internal ConditionalProcess(string name,TokensList list, int idx) : base(name) {
            tokens = list;
            index = idx;
        }
        private void ConditionToken() {
            condition = new TokensList();
            while(tokens[index].IsKeyword != delimiter.Word)
            {
                condition.Add(tokens[index]);
            }
        }
        public bool Condition => (bool)Parser.Evaluate(condition);
    }

    class IfProcess         : ConditionalProcess
    {
        public IfProcess(string name, TokensList tokens, int idx) : base(name, tokens, idx) {
            Name = "If";
            delimiter = new Token("Then");
        }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("",null,variables.type.Invalid);
        }
    }

    class WhileProcess      : ConditionalProcess
    {
        public WhileProcess(string name , TokensList tokens, int idx) : base(name, tokens,idx) {
            Name = "While";
            delimiter = new Token("Do");
        }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("", null, variables.type.Invalid);
        }
    }

    class ForProcess        : WhileProcess
    {
        public ForProcess(string name, TokensList tokens, int idx) : base(name, tokens, idx)
        {
            Name = "For";
        }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("", null, variables.type.Invalid);
        }
    }

    class ImportProcess     : Function
    {
        public ImportProcess(string name = "Import") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("", null, variables.type.Invalid);
        }
    }

    class ClassProcess : Function
    {
        public ClassProcess(string name = "Class") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("",null, variables.type.Invalid);
        }
    }

    class FunctionProcess : Function
    {
        public FunctionProcess(string name = "Function") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("",null, variables.type.Invalid);
        }
    }

    class ReturnProcess : Function
    {
        public ReturnProcess(string name = "Return") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("", null, variables.type.Invalid);
        }
    }

    class VariableProcess : Function
    {
        public VariableProcess(string name = "Variable") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("", null, variables.type.Invalid);
        }
    }

    class ArgumentedProcess : Function
    {
        public List<variables.Variable> Arguments { get; set; }
        public ArgumentedProcess(string name = "Print") : base(name) { }
        public void getArgs()
        {

        }
    }

    class PrintProcess : ArgumentedProcess
    {
        public PrintProcess(string name = "Print") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("",null, variables.type.Invalid);
        }
    }

    class ReadProcess : ArgumentedProcess
    {
        public ReadProcess(string name = "Read") : base(name) { }
        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            return new variables.Variable("",null, variables.type.Invalid);
        }
    }


    class MathProcess     : ArgumentedProcess
    {
        static Dictionary<string, double> Canstants;
        static Dictionary<string, Func<double, double>> UnaryFunctions;
        static Dictionary<string, Func<double, double, double>> DualFunctions;

        public MathProcess(string name) : base(name) {
            Canstants = new Dictionary<string, double>();
            UnaryFunctions = new Dictionary<string, Func<double, double>>();
            DualFunctions = new Dictionary<string, Func<double, double, double>>();
            SetUpCanstants(); SetUpUnaries(); SetUpDuals();
        }

        private void SetUpDuals()
        {
            DualFunctions.Add("pow", Math.Pow); DualFunctions.Add("max", Math.Max); DualFunctions.Add("min", Math.Min);
            DualFunctions.Add("Log_b", Math.Log); DualFunctions.Add("round", (double x,double n) => Math.Floor(x * Math.Pow(10, n) + 0.5) / Math.Pow(10, n));
        }

        private void SetUpUnaries()
        {
            UnaryFunctions.Add("sin", Math.Sin); UnaryFunctions.Add("cos", Math.Cos); UnaryFunctions.Add("tan", Math.Tan);
            UnaryFunctions.Add("asin", Math.Asin); UnaryFunctions.Add("acos", Math.Acos); UnaryFunctions.Add("atan", Math.Atan);
            UnaryFunctions.Add("exp", Math.Exp); UnaryFunctions.Add("log", Math.Log); UnaryFunctions.Add("sqrt", Math.Sqrt);
            UnaryFunctions.Add("abs", Math.Abs); UnaryFunctions.Add("floor", Math.Floor); UnaryFunctions.Add("ceil", Math.Ceiling);
        }

        private void SetUpCanstants()
        {
            Canstants.Add("e", Math.E); Canstants.Add("E", Math.E);
            Canstants.Add("pi", Math.PI); Canstants.Add("PI", Math.PI);
        }

        protected override variables.Variable Evaluate(TokensList data, int idx)
        {
            int argsCount = this.Arguments.Count;
            if(argsCount == 1)
            {
                return new variables.Variable("",UnaryFunctions[this.Name](Convert.ToDouble(Arguments[0].Value)), variables.type.Decimal);
            }
            else if(argsCount == 2)
            {
                return new variables.Variable("", DualFunctions[this.Name](Convert.ToDouble(Arguments[0].Value), Convert.ToDouble(Arguments[1].Value)), variables.type.Decimal) ;
            }
            throw new Exception("Argument Mismatch");
        }
    }
}
