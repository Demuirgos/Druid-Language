using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Function
    {
        public enum type
        {
            procedure,
            function
        }
        public Function()
        {
            Implimentation = this;
            Name = null;
        }
        public Function(string name)
        {
            Name = name;
            if (Interpreter.Functions.ContainsKey(name))
            {
                Implimentation = Interpreter.Get[name];
            }
            else
            {
                Implimentation = this;
            }
        }
        public variables.Variable this[TokensList s]{
            get
            {
                return Implimentation.Evaluate(s);
            }
        }
        protected virtual variables.Variable Evaluate(TokensList data) => throw new Exception("Not Implemented");
        protected virtual void Process(TokensList data) => throw new Exception("Not Implemented");
        public type Type { get; set; }
        public Function Implimentation { get; set; }
        public String Name { get; set; }
        public TokensList Body { get; set; }
        private Token limiter = null;
        public Token Limiter {
            get => this.Implimentation.limiter;
            set
            {
                limiter = value;
            }
        }
    }

    class IdentityProcedure : Function
    {
        internal IdentityProcedure() : base()
        {
            Type = type.function;
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            throw new Exception("Not yet implemented");
        }
    }

    class ConditionalProcess: Function 
    {
        private TokensList condition;
        public Token delimiter { get; set; }
        internal ConditionalProcess(string name) : base(name) {
            Type = type.procedure;
            Limiter = new Token("End");
        }
        private void ConditionToken(TokensList tokens,ref int index) {
            condition = new TokensList();
            while(tokens[index].IsKeyword != delimiter.Word)
            {
                condition.Add(tokens[index]);
            }
        }
        public bool Condition => (bool)Parser.Evaluate(Parser.Parse(condition));
    }

    class IfProcess         : ConditionalProcess
    {
        public IfProcess(string name  = "If") : base(name)
        {
            delimiter = new Token("Then");
        }
        protected override void Process(TokensList data)
        {
            
        }
    }

    class WhileProcess      : ConditionalProcess
    {
        public WhileProcess(string name = "While") : base(name)
        {
            delimiter = new Token("Do");
        }
        protected override void Process(TokensList data)
        {
             
        }
    }

    class ForProcess        : WhileProcess
    {
        public ForProcess(string name = "For") : base(name)
        {
            Type = type.procedure;
        }
        protected override void Process(TokensList data)
        {
            
        }
    }

    class ImportProcess     : Function
    {
        public ImportProcess(string name = "Import") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }
        protected override void Process(TokensList data)
        {

        }
    }

    class ClassProcess      : Function
    {
        public ClassProcess(string name = "Class") : base(name)
        {
            Type = type.procedure;
        }
        protected override void Process(TokensList data)
        {

        }
    }

    class FunctionProcess   : Function
    {
        public FunctionProcess(string name = "Function") : base(name)
        { 
            Type = type.function;
            Limiter = new Token("END");
        }
        protected override variables.Variable Evaluate(TokensList data)
        {
            return new variables.Variable(null, variables.Variable.type.Invalid);
        }
        public List<variables.Variable.type> Signature { get; set; }
    }

    class SpacedProcess     : Function
    {
        public SpacedProcess(string name) : base(name)
        { 
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }
    }

    class ReturnProcess     : SpacedProcess
    {
        public ReturnProcess(string name = "Return") : base(name)
        {
            Type = type.function;
        }
        protected override variables.Variable Evaluate(TokensList data)
        {
            return new variables.Variable(null, variables.Variable.type.Invalid);
        }
    }

    class VariableProcess   : SpacedProcess
    {
        public VariableProcess(string name = "Variable") : base(name)
        {
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }
        protected override void Process(TokensList data)
        {
            var v = new variables.Variable(null, variables.Variable.type.Invalid);
        }
    }

    class ArgumentedProcess : Function
    {
        public ArgumentedProcess(string name) : base(name) { 
        }
        public List<variables.Variable> Arguments { get; set; }
        public void getArgs(TokensList tokens,int index)
        {

        }
    }

    class PrintProcess      : ArgumentedProcess
    {
        public PrintProcess(string name = "Print") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }
        protected override void Process(TokensList data)
        {
            foreach (var arg in Arguments)
                Console.Write(arg.Value);
        }
    }

    class ReadProcess       : ArgumentedProcess
    {
        public ReadProcess(string name = "Read") : base(name)
        {
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }
        protected override variables.Variable Evaluate(TokensList data)
        {
            Console.Write(Arguments[0].Value);
            return new variables.Variable(Console.ReadLine(), variables.Variable.type.Word);
        }
    }

    class ConstantsMap      :Function
    {
        public static Dictionary<string, double> Canstants = new Dictionary<string, double>();
        
        public ConstantsMap(string name) : base(name)
        {
            Limiter = new Token("Current");
        }

        public static void SetUpCanstants()
        {
            Canstants.Add("e", Math.E); Canstants.Add("E", Math.E);
            Canstants.Add("pi", Math.PI); Canstants.Add("PI", Math.PI);
            Canstants.Add("False", 0); Canstants.Add("True", 1);
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            if(Name!="False" && Name!="True")
                return new variables.Variable(Canstants[this.Name], variables.Variable.type.Decimal);
            return new variables.Variable(Canstants[this.Name], variables.Variable.type.Boolean);
        }
    }

    class MathProcess       : ArgumentedProcess
    {
        public static Dictionary<string, Func<double, double>> UnaryFunctions = new Dictionary<string, Func<double, double>>();
        public static Dictionary<string, Func<double, double, double>> DualFunctions = new Dictionary<string, Func<double, double, double>>();
        static Random rnd = new Random();
        public MathProcess(string name) : base(name)
        {
            Limiter = new Token("END_ARG");
        }

        public static void SetupFunctions()
        {
            SetUpDuals(); SetUpUnaries();
        }

        private static void SetUpDuals()
        {
            DualFunctions.Add("pow", Math.Pow); DualFunctions.Add("max", Math.Max); DualFunctions.Add("min", Math.Min);
            DualFunctions.Add("Log_b", Math.Log); DualFunctions.Add("round", (double x,double n) => Math.Floor(x * Math.Pow(10, n) + 0.5) / Math.Pow(10, n));
            DualFunctions.Add("rand", (double x, double n) => { return rnd.NextDouble() + rnd.Next((int)x,(int)n); });
        }

        private static void SetUpUnaries()
        {
            UnaryFunctions.Add("sin", Math.Sin); UnaryFunctions.Add("cos", Math.Cos); UnaryFunctions.Add("tan", Math.Tan);
            UnaryFunctions.Add("asin", Math.Asin); UnaryFunctions.Add("acos", Math.Acos); UnaryFunctions.Add("atan", Math.Atan);
            UnaryFunctions.Add("exp", Math.Exp); UnaryFunctions.Add("log", Math.Log); UnaryFunctions.Add("sqrt", Math.Sqrt);
            UnaryFunctions.Add("abs", Math.Abs); UnaryFunctions.Add("floor", Math.Floor); UnaryFunctions.Add("ceil", Math.Ceiling);
        }


        protected override variables.Variable Evaluate(TokensList data)
        {
            int argsCount = this.Arguments.Count;
            
            if(argsCount == 1)
            {
                return new variables.Variable(UnaryFunctions[this.Name](Convert.ToDouble(Arguments[0].Value)), variables.Variable.type.Decimal);
            }
            else if(argsCount == 2)
            {
                return new variables.Variable(DualFunctions[this.Name](Convert.ToDouble(Arguments[0].Value), Convert.ToDouble(Arguments[1].Value)), variables.Variable.type.Decimal) ;
            }
            throw new Exception("Argument Mismatch");
        }
    }
}
