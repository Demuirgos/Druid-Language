using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Function
    {
        public enum type
        {
            undetermined,
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
                if (this.Type == type.function)
                    return Implimentation.Evaluate(s);
                else
                    Implimentation.Process(s);
                return new variables.Variable(null,varType: variables.Variable.type.Invalid);
            }
        }
        protected virtual variables.Variable Evaluate(TokensList data) => throw new Exception("Not Implemented");
        protected virtual void Process(TokensList data) => throw new Exception("Not Implemented");
        private type Ftype = Function.type.undetermined;
        public type Type
        {
            get
            {
                if (Ftype == type.undetermined)
                    return this.Implimentation.Ftype;
                return this.Ftype;
            }
            set
            {
                Ftype = value;
            }
        }
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
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }
        protected override void Process(TokensList data)
        {
            //Variable n as integer (Decima | Boolean | Word | Array?)
            //Variable n as array of Word
            data.Trim();
            if (data.Count != 4 || data.Count != 6)
                new Exception("Syntax error Variable Declared Incorrectly");
            var name = data[1].Word;
            var type = data[3].Word=="Decimal"?variables.Variable.type.Decimal: data[3].Word == "Boolean" ? variables.Variable.type.Boolean: data[3].Word == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
            object defaultVal = null;
            switch (type)
            {
                case variables.Variable.type.Decimal:
                    defaultVal = 0;
                    break;
                case variables.Variable.type.Boolean:
                    defaultVal = false;
                    break;
                case variables.Variable.type.Word:
                    defaultVal = "";
                    break;
            }
            var v = new variables.Variable(defaultVal, type, name);
            Interpreter.Set[name] = v;
        }
    }

    class ArgumentedProcess : Function
    {
        public ArgumentedProcess(string name) : base(name) { 
        }
        private List<variables.Variable> arguments = new List<variables.Variable>();
        public List<variables.Variable> Arguments => arguments;
        protected virtual void getArgs(TokensList tokens) => throw new Exception("Not yet Implimented");
    }

    class PrintProcess      : ArgumentedProcess
    {
        public PrintProcess(string name = "Print") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }

        protected override void getArgs(TokensList tokens)
        {
            Arguments.Clear();
            var args = tokens[1].Spread();
            args.Trim();
            foreach(var arg in args)
            {
                if(arg.IsKeyword != "NEXT_ARG")
                Arguments.Add(new variables.Variable(arg.Word));
            }
        }

        protected override void Process(TokensList data)
        {
            getArgs(data);
            for(int i=0;i<this.Arguments.Count;i++)
                Console.Write(Arguments[i].Value + (i<Arguments.Count - 1 ? " " : Environment.NewLine));
        }
    }

    class ReadProcess       : ArgumentedProcess
    {
        public ReadProcess(string name = "Read") : base(name)
        {
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
            Type = type.function;
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
            Limiter = new Token("Next");
            this.Type = type.function;
        }

        protected override void getArgs(TokensList tokens)
        {
            Arguments.Clear();
            TokensList args = tokens[1].Spread();
            TokensList currentArg = new TokensList();
            for(int i = 0; i < args.Count; i++)
            {
                currentArg.Add(args[i]);
                if (args[i].IsKeyword == "NEXT_ARG" || i == args.Count - 1)
                {
                    currentArg.Trim();
                    var arg = (variables.Variable)Parser.Evaluate(Parser.Parse(currentArg));
                    Arguments.Add(arg);
                    currentArg.Clear();
                }
            }

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
            UnaryFunctions.Add("neg", (double d) => -d);
        }


        protected override variables.Variable Evaluate(TokensList data)
        {
            getArgs(data);
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
