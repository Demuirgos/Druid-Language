using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace YetAnotherScriptingLanguage
{
   public class Function
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
                variables.Variable v = new variables.Variable(null, varType: variables.Variable.type.Invalid);
                if (this.Type == type.function)
                    v = Implimentation.Evaluate(s);
                else
                    Implimentation.Process(s);
                return v;
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
        private Token limiter = null;
        public Token Limiter {
            get => this.Implimentation.limiter;
            set
            {
                limiter = value;
            }
        }
    }

    class ConditionalProcess : Function
    {
        public enum state
        {
            exit,
            skip,
            normal
        }
        public virtual Token delimiter { get; set; }
        internal ConditionalProcess(string name) : base(name) {
            Limiter = new Token("END");
        }

        public static state LoopState = state.normal;
        public static state State
        {
            get
            {
                return LoopState;
            }
            set
            {
                LoopState = value;
            }
        }

        public virtual Tuple<TokensList, TokensList, TokensList> BlockExtraction(TokensList tokens) => throw new Exception("Not yet Implemented");

        public bool this[TokensList l] => Convert.ToBoolean(((variables.Variable)Parser.Evaluate(Parser.Parse(l))).Value);

    }

    class IfProcess : ConditionalProcess
    {
        public IfProcess(string name = "IF") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("END");
        }

        public override Tuple<TokensList,TokensList,TokensList> BlockExtraction(TokensList tokens)
        {
            int i = 1;
            var condition = tokens[0, new Token("THEN")].Remove().Remove(0);
            for (; i < tokens.Count; i++)
            {
                if (tokens[i].IsKeyword == "IF")
                {
                    i += tokens[i, new Token("END")].Count;
                }
                if (tokens[i].IsKeyword == "ELSE")
                {
                    break;
                }
            }
            var ElseBlock = i == tokens.Count ? new TokensList() : tokens[i,tokens.Count-1].Remove(0).Trim(false);
            var Block = tokens[condition.Count+2,i-1].Trim(false);
            return new Tuple<TokensList, TokensList, TokensList>(condition, Block, ElseBlock);
        }

        //extract ifblock
        //extract else block if exists
        protected override void Process(TokensList data)
        {
            var Blocks = BlockExtraction(data);
            if (this[Blocks.Item1])
            {
                Parser.Evaluate(Parser.Parse(Blocks.Item2));
            }
            else
            {
                Parser.Evaluate(Parser.Parse(Blocks.Item3));
            }
        }
    }

    class WhileProcess : ConditionalProcess
    {
        public WhileProcess(string name = "WHILE") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("END");
        }


        public Tuple<TokensList, TokensList, TokensList> BlockToken(TokensList tokens)
        {
            var condition = tokens[0, new Token("DO")].Remove().Remove(0);
            var Block = tokens[0, new Token("DO"), new Token("END")].Remove().Remove(0).Trim(false);
            return new Tuple<TokensList, TokensList, TokensList>(condition, Block, null);
        }

        protected override void Process(TokensList data)
        {
            var Blocks = BlockToken(data);
            while (this[Blocks.Item1])
            {
                Parser.Evaluate(Parser.Parse(Blocks.Item2));
                if (Parser.ParserState == Parser.state.TemporalSuspension)
                {
                    Parser.ParserState = Parser.state.Normal;
                    continue;
                }
                else if (Parser.ParserState == Parser.state.Suspended || Interpreter.ReturnValue.Count > 0)
                {
                    break;
                }
            }
            Parser.ParserState = Parser.state.Normal;
        }
    }

    class ForProcess : WhileProcess
    {
        private variables.Variable step;
        public ForProcess(string name = "FOR") : base(name)
        {
            Type = type.procedure;
            delimiter = new Token("END");
        }

        public TokensList ConditionToken(TokensList tokens)
        {
            //for i from l to r (by s)? do
            var Indexer = tokens[1];
            bool stepAvailable = tokens[0, new Token("DO")].HasToken(new Token("BY"));
            var leftLim = (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0,new Token("FROM"), new Token("TO")].Remove().Remove(0).Trim(false)));
            var rightLim = (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0, new Token("TO"), stepAvailable?new Token("BY"): new Token("DO")].Remove().Remove(0).Trim(false))) + new variables.Variable("1");
            step = !stepAvailable ? new variables.Variable("1") : (variables.Variable)Parser.Evaluate(Parser.Parse(tokens[0, new Token("BY"),  new Token("DO")].Remove().Remove(0).Trim(false)));
            if (leftLim.Type != variables.Variable.type.Decimal || rightLim.Type != variables.Variable.type.Decimal || step.Type != variables.Variable.type.Decimal)
            {
                throw new Exception("Upper and Lower limits must be numbers!");
            }
            Interpreter.Set[Indexer.Word] = leftLim;
            var condition = new TokensList();
            condition.Add(Indexer);
            condition.Add(new Token("SMALLER"));
            condition.Add(new Token(rightLim.Value.ToString()));
            return condition;
        }

        public override Tuple<TokensList,TokensList,TokensList> BlockExtraction(TokensList tokens)
        {
            var condition = ConditionToken(tokens);
            var Block = tokens[0, new Token("DO"), new Token("END")].Remove().Remove(0).Trim(false);
            return new Tuple<TokensList, TokensList, TokensList>(condition, Block, null);
        }

        protected override void Process(TokensList data)
        {
            var Blocks = BlockExtraction(data);
            var IndexerName = Blocks.Item1[0].Word;
            while (this[Blocks.Item1])
            {
                Parser.Evaluate(Parser.Parse(Blocks.Item2));
                var arg = ((variables.Variable)Interpreter.Get[IndexerName]);
                var val = Convert.ToDecimal(((variables.Variable)Interpreter.Get[IndexerName]).Value) + Convert.ToDecimal(step.Value);
                arg.Value = val;
                if (Parser.ParserState == Parser.state.TemporalSuspension)
                {
                    Parser.ParserState = Parser.state.Normal;
                    continue;
                }
                else if (Parser.ParserState == Parser.state.Suspended || Interpreter.ReturnValue.Count > 0)
                {
                    break;
                }
            }
            if(Interpreter.Pop[IndexerName])
                Parser.ParserState = Parser.state.Normal;
            else
                throw new Exception("Loop's temprorary variable not Cleared Properly");
        }
    }

    class ClassProcess : Function
    {
        public ClassProcess(string name = "CLASS") : base(name)
        {
            Type = type.procedure;
        }
        protected override void Process(TokensList data)
        {

        }
    }

    class FunctionProcess : Function
    {
        public class CustomFunction : ArgumentedProcess
        {

            public KeyValuePair<List<variables.Variable>, variables.Variable.type> Signature { get; set; }
            public TokensList Body { get; set; }
            public CustomFunction(type t,TokensList body,KeyValuePair<List<variables.Variable>,variables.Variable.type> signature, string name) : base(name)
            {
                Type = t;
                Body = body;
                Signature = signature;
                if(Type == type.procedure)
                {
                    Limiter = new Token("END_STATEMENT");
                }
                else if(Type == type.function)
                {
                    Limiter = new Token("NEXT");
                }
            }

            
            protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
            {
                List<variables.Variable> Arguments = new List<variables.Variable>();
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
                return Arguments;
            }

            void InitProcessBlock()
            {
                Interpreter.Block block = new Interpreter.Block();
                for(int i = 0; i < this.Signature.Key.Count; i++)
                {
                    block.Variables.Add(this.Signature.Key[i].Name, new variables.Variable(VariableProcess.DefaultValue(this.Signature.Key[i].Type), this.Signature.Key[i].Type));
                }
                Interpreter.Set.Insert(block);
            }

            void FillProcessBlock(List<variables.Variable> Arguments)
            {
                InitProcessBlock();
                for (int i = 0; i < this.Signature.Key.Count; i++)
                {
                    if(this.Signature.Key[i].Type == Arguments[i].Type)
                    {
                        var varName = this.Signature.Key[i].Name;
                        var varValue = Arguments[i].Value;
                        ((variables.Variable)Interpreter.Get[varName]).Value = varValue;
                    }
                    else
                    {
                        string msg = "Argument Missmatch " + this.Name + "(";
                        for(int j = 0; j < this.Signature.Key.Count; j++)
                        {
                            msg += this.Signature.Key[j].Type.ToString() + (i == this.Signature.Key.Count - 1 ? "" : ", ");
                        }
                        msg += ")";
                        throw new Exception(msg);
                    }
                }
            }

            void ArgumentBlockPreProcess(TokensList data)
            {
                FillProcessBlock(ArgumentsExtraction(data));
            }

            protected override void Process(TokensList data)
            {
                ArgumentBlockPreProcess(data);
                Parser.Evaluate(Parser.Parse(this.Body));
                Interpreter.ExecutionStack.Pop();
            }

            protected override variables.Variable Evaluate(TokensList data)
            {
                ArgumentBlockPreProcess(data);
                Parser.Evaluate(Parser.Parse(this.Body));
                var r = Interpreter.ReturnValue.Dequeue();
                if (r.Type != this.Signature.Value)
                    throw new Exception("The function " + this.Name + " returns a : " + this.Signature.Value.ToString());
                variables.Variable result = new variables.Variable(r);
                Interpreter.ReturnValue.Clear();
                Interpreter.ExecutionStack.Pop();
                Parser.ParserState = Parser.state.Normal;
                return result;
            }

        }
        public FunctionProcess(string name = "FUNCTION") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END");
        }

        List<variables.Variable> getSignature(TokensList data)
        {
            List<variables.Variable>  Signature = new List<variables.Variable>();
            TokensList tokens = data[2].Spread().Trim().Remove(new Token("NEXT_ARG"));
            for (int i = 0; i < tokens.Count; i+=3)
            {
                var varname = tokens[i];
                var vartype = tokens[i+2];
                var type = vartype.Word == "Decimal" ? variables.Variable.type.Decimal : vartype.Word == "Boolean" ? variables.Variable.type.Boolean : vartype.Word == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
                var defaultVal = VariableProcess.DefaultValue(type);
                var v = new variables.Variable(defaultVal, type, varname.Word);
                Signature.Add(v);
            }
            return Signature;
        }

        string getName(TokensList data)
        {
            return data[1].Word;
        }

        TokensList getBody(TokensList data)
        {
            return data[0, new Token("BEGIN"), new Token("END")].Remove().Remove(0).Trim(false);
        }

        type getType(bool ThenFollow)
        {
            if (ThenFollow)
            {
                return type.procedure;
            }
            else
            {
                return type.function;
            }
        }

        variables.Variable.type getReturn(TokensList data)
        {
            return data[4].Word == "Decimal" ? variables.Variable.type.Decimal : data[4].Word == "Boolean" ? variables.Variable.type.Boolean : data[4].Word == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
        }

        protected override void Process(TokensList data)
        {
            string name = getName(data);
            Interpreter.Post[name] = new FunctionProcess.CustomFunction(getType(data[4].IsKeyword == "Begin"), getBody(data), new KeyValuePair<List<variables.Variable>,variables.Variable.type>(getSignature(data), getReturn(data)), name);
        }
    }

    class ReturnProcess : Function
    {
        public ReturnProcess(string name = "RETURN") : base(name)
        {
            this.Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            var PostReturn = data[1, this.Limiter];
            var r = Parser.Evaluate(Parser.Parse(PostReturn));
            Parser.ParserState = Parser.state.Suspended;
            return r;
        }
    }

    class ArrayProcess : Function
    {
        public ArrayProcess(string name = "ARRAY") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }

        protected override void Process(TokensList data)
        {
            //array name[l,l,l,l] of type
            //::[] prepend
            //[]::append
            data.Trim();
            if (data.Count != 5)
                new Exception("Syntax error Array Declared Incorrectly");
            var name = data[1].Word;
            var dimensions = new Token(data[2].Word.Substring(1, data[2].Word.Length - 2)).Spread().Trim().Remove(new Token("NEXT_ARG"));
            var dimensionsLens = new List<int>();
            var lenght =1;
            foreach(var len in dimensions)
            {
                lenght *= Convert.ToInt32(len.Word);
                dimensionsLens.Add(Convert.ToInt32(len.Word));
            }
            var type = data[4].Word == "Decimal" ? variables.Variable.type.Decimal : data[4].Word == "Boolean" ? variables.Variable.type.Boolean : data[4].Word == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
            List<variables.Variable> defaultVal = new List<variables.Variable>(lenght);
            for(int i = 0; i < lenght; i++)
            {
                defaultVal.Add(new variables.Variable(VariableProcess.DefaultValue(type), type));
            }
            var v = new variables.Array(defaultVal,variables.Variable.type.Array, dimensionsLens, type, name);
            if (!Interpreter.Peek[name])
                Interpreter.Set[name] = v;
            else
                throw new Exception("A variable with the name : " + name + " Already exists in Stack");
        }
    }

    class VariableProcess : Function
    {
        public VariableProcess(string name = "VARIABLE") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }
        public static object DefaultValue(variables.Variable.type t) {
            object defaultVal = null;
            switch (t)
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
                case variables.Variable.type.Invalid:
                    throw new Exception("Invalid Type");
            }
            return defaultVal;
        }

        protected override void Process(TokensList data)
        {
            data.Trim(false);
            var names = data[1, new Token("OFTYPE")].Remove().Remove(new Token("NEXT_ARG"));
            var typeToken = data[0, new Token("OFTYPE"), new Token("END_STATEMENT")].Remove(0).Remove()[0];
            var type = typeToken.IsKeyword == "Decimal" ? variables.Variable.type.Decimal : typeToken.IsKeyword == "Boolean" ? variables.Variable.type.Boolean : typeToken.IsKeyword == "Word" ? variables.Variable.type.Word : variables.Variable.type.Invalid;
            object defaultVal = DefaultValue(type);
            foreach (var nameToken in names)
            {
                if (!Interpreter.Keywords.ContainsKey(nameToken.Word))
                {
                    var v = new variables.Variable(defaultVal, type, nameToken.Word);
                    if (!Interpreter.Peek[nameToken.Word])
                        Interpreter.Set[nameToken.Word] = v;
                    else
                        throw new Exception("A variable with the name : " + nameToken.Word + " Already exists in Stack");
                }
            }
        }
    }

    class DeleteProcess : Function
    {
        public DeleteProcess(string name = "DELETE") : base(name)
        {
            this.Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }

        protected override void Process(TokensList data)
        {
            data.Trim(false);
            var names = data[1, new Token("END_STATEMENT")].Remove().Remove(new Token("NEXT_ARG"));
            foreach (var nameToken in names)
            {
                if (!Interpreter.Keywords.ContainsKey(nameToken.Word))
                {
                    if (!Interpreter.Pop[nameToken.Word])
                        throw new Exception("No variable with the name : " + nameToken.Word + " exists in Stack");
                }
            }
        }
    }

    class ArgumentedProcess : Function
    {
        public ArgumentedProcess(string name) : base(name) {
        }
        protected virtual List<variables.Variable> ArgumentsExtraction(TokensList tokens) => throw new Exception("Not yet Implemented");
    }

    class PrintProcess : ArgumentedProcess
    {

        public delegate void PrintInvoked(PrintProcess sender,List<variables.Variable> Arguments);
        public event PrintInvoked PrintHandler;
        public PrintProcess(string name = "PRINT") : base(name)
        {
            Type = type.procedure;
            Limiter = new Token("END_STATEMENT");
        }

        protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
        {
            var Arguments = new List<variables.Variable>();
            var args = tokens[1].Spread().Trim();
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
            return Arguments;
        }

        protected override void Process(TokensList data)
        {
            PrintHandler(this, ArgumentsExtraction(data));
        }
    }

    class OpenProcess : ArgumentedProcess
    {

        protected string fileExtensionRestriction;
        string content = "";
        public OpenProcess(string name = "OPEN") : base(name)
        {
            Type = type.function;
            fileExtensionRestriction = "";
            Limiter = new Token("END_STATEMENT");
        }

        protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
        {
            var Arguments = new List<variables.Variable>();
            var args = tokens[1].Spread().Trim(true);
            if (args.Count != 1) throw new Exception("Argument Count Missmatch, Read takes at least 1 argument");
            else
            {
                if (!String.IsNullOrWhiteSpace(fileExtensionRestriction))
                {
                    if (args[0].Word.EndsWith(fileExtensionRestriction))
                    {
                        Arguments.Add((variables.Variable)Parser.Evaluate(Parser.Parse(args)));
                    }
                    else
                    {
                        throw new Exception("File extension Missmatch" + " The required File is of type : " + fileExtensionRestriction);
                    }
                }
                else
                {
                    Arguments.Add((variables.Variable)Parser.Evaluate(Parser.Parse(args)));
                }
            }
            return Arguments;
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            var Arguments = ArgumentsExtraction(data);
            StreamReader sr = new StreamReader((String)Arguments[0].Value);
            string content = sr.ReadToEnd();
            return new variables.Variable(content, variables.Variable.type.Word);
        }

    }
    
    class ImportProcess : OpenProcess
    {
        public ImportProcess(string name = "IMPORT") : base(name)
        {
            Type = type.procedure;
            fileExtensionRestriction = ".aysl";
            Limiter = new Token("END_STATEMENT");
        }
        protected async override void Process(TokensList data)
        {
            string content = null;
            if(Interpreter.Mode == Interpreter.mode.console)
            {
                content = (String)(this.Evaluate(data).Value);
            }
            else if(Interpreter.Mode == Interpreter.mode.Uwp)
            {
                var Arguments = ArgumentsExtraction(data);
                content = await PathIO.ReadTextAsync((String)Arguments[0].Value);
            }
            Parser.Evaluate(Parser.Parse(Tokenizer.Tokenize(new TranslationUnit(content))));
        }
    }

    class ReadProcess : ArgumentedProcess
    {

        public delegate variables.Variable ReadInvoked(ReadProcess sender, variables.Variable Argument);
        public event ReadInvoked ReadHandler; 
        public ReadProcess(string name = "READ") : base(name)
        {
            Type = type.function;
            Limiter = new Token("END_STATEMENT");
        }


        protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
        {
            var Arguments = new List<variables.Variable>();
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
            return Arguments;
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            var r = ReadHandler(this, ArgumentsExtraction(data)[0]);
            return r;
        }
    }

    class ConstantsMap : Function
    {
        public static Dictionary<string, double> Canstants = new Dictionary<string, double>();
        public ConstantsMap(string name) : base(name)
        {
            Type = type.function;
            Limiter = new Token("CURRENT");
        }

        public static void SetUpCanstants()
        {
            Canstants.Add("e", Math.E); Canstants.Add("E", Math.E);
            Canstants.Add("pi", Math.PI); Canstants.Add("PI", Math.PI);
            Canstants.Add("FALSE", 0); Canstants.Add("TRUE", 1);
        }

        protected override variables.Variable Evaluate(TokensList data)
        {
            if (Name != "FALSE" && Name != "TRUE")
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
            Limiter = new Token("NEXT");
            this.Type = type.function;
        }

        protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
        {
            var Arguments = new List<variables.Variable>();
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
            return Arguments;
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
                if (d != Math.Truncate(d) || d < 0) throw new Exception("Factorial Function Takes an Integer input");
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
            var Arguments = ArgumentsExtraction(data);
            int argsCount = Arguments.Count;

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
        public SetProcess(string name="SET") : base(name)
        {
            this.Type = type.procedure;
            Limiter = new Token("PREVIOUS");
        }

        protected override List<variables.Variable> ArgumentsExtraction(TokensList tokens)
        {
            List<variables.Variable> Arguments =  new List<variables.Variable>();
            TokensList varName = tokens[0,new Token("SET")].Remove();
            TokensList varVal = tokens[varName.Count + 1, tokens.Count - 1].Trim();
            var argVal = Parser.Evaluate(Parser.Parse(varVal));
            var indexVal = varName.Count != 2 ? null : Parser.Evaluate(Parser.Parse(new Token(varName[1].Word.TrimStart('[').TrimEnd(']')).Spread()));
            Arguments.Add(new variables.Variable(varName[0].Word));
            Arguments.Add(argVal);
            if(!(indexVal is null))
                Arguments.Add(indexVal);
            return Arguments;
        }

        protected override void Process(TokensList data)
        {
            //Variable n as integer (Decima | Boolean | Word | Array?)
            //Variable n as array of Word
            var Arguments = ArgumentsExtraction(data); ;
            var var = ((variables.Variable)Interpreter.Get[(string)Arguments[0].Value]);
            if (var.Type == variables.Variable.type.Array)
            {
                if (Arguments.Count == 3)
                {
                    var index = Convert.ToInt32(Arguments[2].Value.ToString());
                    var = ((List<variables.Variable>)((variables.Variable)Interpreter.Get[(string)Arguments[0].Value]).Value)[index];
                }
            }
            if (var.Type != Arguments[1].Type)
                throw new Exception("Argument Type Missmatch, cannot assign a value of " + Arguments[1].Type.ToString() + " to a variable of type " + Arguments[0].Type.ToString());
            var.Value = Arguments[1].Value;
        }
    }
}
