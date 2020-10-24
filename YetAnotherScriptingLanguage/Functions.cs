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
        public variables.Variable this[TokensList s] {
            get
            {
                if (this.Type == type.function)
                    return Implimentation.Evaluate(s);
                else
                    Implimentation.Process(s);
                return new variables.Variable(null, varType: variables.Variable.type.Invalid);
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

    class ConditionalProcess : Function
    {
        public TokensList condition;
        public TokensList Block;
        public Token delimiter { get; set; }
        internal ConditionalProcess(string name) : base(name) {
            Limiter = new Token("End");
        }

        public virtual void ConditionToken(TokensList tokens) {
            condition = tokens[0, new Token("Begin")].Remove().Remove(0);
        }
        public virtual void BlockToken(TokensList tokens) => throw new Exception("Not yet Implemented");

        public bool Condition => Convert.ToBoolean(((variables.Variable)Parser.Evaluate(Parser.Parse(condition))).Value);
    }

    class IfProcess : ConditionalProcess
    {
        private TokensList ElseBlock;

        public IfProcess(string name = "If") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("End");
        }

        public override void BlockToken(TokensList tokens)
        {
            int i = 1;
            //fix index
            for (; i < tokens.Count; i++)
            {
                if (tokens[i].IsKeyword == "If")
                {
                    i += tokens[i, new Token("End")].Count;
                }
                if (tokens[i].IsKeyword == "Else")
                {
                    break;
                }
            }
            ElseBlock = i == tokens.Count ? new TokensList() : tokens[i, new Token("End")].Remove(0).Trim(false);
            Block = tokens[condition.Count+2,i-1].Trim(false);
        }

        //extract ifblock
        //extract else block if exists
        protected override void Process(TokensList data)
        {
            ConditionToken(data);
            BlockToken(data);
            if (this.Condition)
            {
                Parser.Evaluate(Parser.Parse(Block));
            }
            else
            {
                Parser.Evaluate(Parser.Parse(ElseBlock));
            }
        }
    }

    class WhileProcess : ConditionalProcess
    {
        public WhileProcess(string name = "While") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("Do");
        }

        public override void BlockToken(TokensList tokens)
        {
            Block = tokens[0, new Token("Begin"), new Token("End")].Remove().Remove(0).Trim(false);
        }

        protected override void Process(TokensList data)
        {
            ConditionToken(data);
            BlockToken(data);
            while (this.Condition)
            {
                Parser.Evaluate(Parser.Parse(Block));
            }
        }
    }

    class ForProcess : WhileProcess
    {
        private variables.Variable step;
        private string indexer;
        public ForProcess(string name = "For") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("END_STATEMENT");
        }

        public override void ConditionToken(TokensList tokens)
        {
            //for i from l to r (by s)? do
            var Indexer = tokens[1];
            indexer = Indexer.Word;
            bool stepAvailable = tokens.Count == 9;
            var leftLim = (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0,new Token("From"), new Token("To")].Remove().Remove(0).Trim(false)));
            var rightLim = (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0, new Token("To"), stepAvailable?new Token("By"): new Token("Do")].Remove().Remove(0).Trim(false))) + new variables.Variable("1");
            step = !stepAvailable ? new variables.Variable("1") : (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0, new Token("By"),  new Token("Do")].Remove().Remove(0).Trim(false)));
            if (leftLim.Type != variables.Variable.type.Decimal || rightLim.Type != variables.Variable.type.Decimal || step.Type != variables.Variable.type.Decimal)
            {
                throw new Exception("Upper and Lower limits must be numbers!");
            }
            Interpreter.Set[Indexer.Word] = leftLim;
            condition = new TokensList();
            condition.Add(Indexer);
            condition.Add(new Token("<"));
            condition.Add(new Token(rightLim.Value.ToString()));
        }

        public override void BlockToken(TokensList tokens)
        {
            Block = tokens[0, new Token("Begin"), new Token("End")].Remove().Remove(0).Trim(false);
        }

        protected override void Process(TokensList data)
        {
            ConditionToken(data);
            BlockToken(data);
            while (this.Condition)
            {
                Parser.Evaluate(Parser.Parse(Block));
                var arg = ((variables.Variable)Interpreter.Get[indexer]);
                var val = Convert.ToDecimal(((variables.Variable)Interpreter.Get[indexer]).Value) + Convert.ToDecimal(step.Value);
                arg.Value = val;
            }
        }
    }

    class ImportProcess : Function
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

    class ClassProcess : Function
    {
        public ClassProcess(string name = "Class") : base(name)
        {
            Type = type.procedure;
        }
        protected override void Process(TokensList data)
        {

        }
    }

    class FunctionProcess : Function
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

    class SpacedProcess : Function
    {
        public SpacedProcess(string name) : base(name)
        {
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }
    }

    class ReturnProcess : SpacedProcess
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

    class VariableProcess : SpacedProcess
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
            var type = data[3].Word == "Decimal" ? variables.Variable.type.Decimal : data[3].Word == "Boolean" ? variables.Variable.type.Boolean : data[3].Word == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
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
            if (!Interpreter.Peek[name])
                Interpreter.Set[name] = v;
            else
                throw new Exception("A variable with the name : " + name + " Already exists in Stack");
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

    class PrintProcess : ArgumentedProcess
    {

        public delegate void PrintInvoked(PrintProcess sender,List<variables.Variable> Arguments);
        public event PrintInvoked PrintHandler;
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
            TokensList curr = new TokensList();
            for(int i = 0; i < args.Count; i++) 
            {
                if (args[i].IsKeyword != "NEXT_ARG")
                {
                    curr.Add(args[i]);
                }
                if(args[i].IsKeyword=="NEXT_ARG" || i == args.Count - 1)
                {
                    Arguments.Add((variables.Variable)Parser.Evaluate(Parser.Parse(curr)));
                    curr.Clear();
                }
            }
        }

        protected override void Process(TokensList data)
        {
            getArgs(data);
            PrintHandler(this,Arguments);
            
        }
    }

    class ReadProcess : ArgumentedProcess
    {

        public delegate variables.Variable ReadInvoked(ReadProcess sender, variables.Variable Argument);
        public event ReadInvoked ReadHandler; 
        public ReadProcess(string name = "Read") : base(name)
        {
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }

        protected override void getArgs(TokensList tokens)
        {
            Arguments.Clear();
            var args = tokens[1].Spread();
            args.Trim(true);
            if (args.Count > 1) throw new Exception("Argument Count Missmatch, Read takes 1 argument");
            else
            {
                if (args.Count == 0)
                {
                    Arguments.Add(new variables.Variable(""));
                }
                else
                {
                    Arguments.Add((variables.Variable)Parser.Evaluate(Parser.Parse(args)));
                }
            }
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            getArgs(data);
            return ReadHandler(this,Arguments[0]);
        }
    }

    class ConstantsMap : Function
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
            if (Name != "False" && Name != "True")
                return new variables.Variable(Canstants[this.Name], variables.Variable.type.Decimal);
            return new variables.Variable(Canstants[this.Name], variables.Variable.type.Boolean);
        }
    }

    class MathProcess : ArgumentedProcess
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
            for (int i = 0; i < args.Count; i++)
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
            DualFunctions.Add("pow", Math.Pow); DualFunctions.Add("max", Math.Max);
            DualFunctions.Add("min", Math.Min); DualFunctions.Add("log_b", Math.Log);
            DualFunctions.Add("round", (double x, double n) => Math.Floor(x * Math.Pow(10, n) + 0.5) / Math.Pow(10, n));
            DualFunctions.Add("rand", (double x, double n) => { return rnd.NextDouble() + rnd.Next((int)x, (int)n); });
        }

        private static void SetUpUnaries()
        {
            UnaryFunctions.Add("sin", Math.Sin); UnaryFunctions.Add("cos", Math.Cos);
            UnaryFunctions.Add("tan", Math.Tan); UnaryFunctions.Add("atan", Math.Atan);
            UnaryFunctions.Add("asin", Math.Asin); UnaryFunctions.Add("acos", Math.Acos);
            UnaryFunctions.Add("exp", Math.Exp); UnaryFunctions.Add("log", Math.Log);
            UnaryFunctions.Add("abs", Math.Abs); UnaryFunctions.Add("floor", Math.Floor);
            UnaryFunctions.Add("sqrt", Math.Sqrt); UnaryFunctions.Add("ceil", Math.Ceiling);
            UnaryFunctions.Add("fact", (double d) =>
            {
                if (d != Math.Truncate(d)) throw new Exception("Factorial Function Takes an Integer input");
                double result = 1;
                for(int i = 1; i <= Math.Floor(d); i++)
                {
                    result *= i;
                }
                return result;
            });
            UnaryFunctions.Add("-", (double d) => -d); UnaryFunctions.Add("+", (double d) => d);
        }


        protected override variables.Variable Evaluate(TokensList data)
        {
            getArgs(data);
            int argsCount = this.Arguments.Count;

            if (argsCount == 1)
            {
                return new variables.Variable(UnaryFunctions[this.Name](Convert.ToDouble(Arguments[0].Value)), variables.Variable.type.Decimal);
            }
            else if (argsCount == 2)
            {
                return new variables.Variable(DualFunctions[this.Name](Convert.ToDouble(Arguments[0].Value), Convert.ToDouble(Arguments[1].Value)), variables.Variable.type.Decimal);
            }
            throw new Exception("Argument Mismatch");
        }
    }

    class SetProcess : ArgumentedProcess
    {
        public SetProcess(string name=":=") : base(name)
        {
            this.Type = type.procedure;
            Limiter = new Token("Previous");
        }

        protected override void getArgs(TokensList tokens)
        {
            Arguments.Clear();
            Token varName = tokens[0];
            var argVal = (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[2, tokens.Count - 1]));
            Arguments.Add(new variables.Variable(varName.Word));
            Arguments.Add(argVal);
        }

        protected override void Process(TokensList data)
        {
            //Variable n as integer (Decima | Boolean | Word | Array?)
            //Variable n as array of Word
            getArgs(data); ;
            var var = ((variables.Variable)Interpreter.Get[(string)Arguments[0].Value]);
            if (var.Type != Arguments[1].Type)
                throw new Exception("Argument Type Missmatch, cannot assign a value of " + Arguments[1].Type.ToString() + " to a variable of type " + Arguments[0].Type.ToString());
            var.Value = Arguments[1].Value;
        }
    }
}
