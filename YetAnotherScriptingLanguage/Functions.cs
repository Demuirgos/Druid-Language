using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Function
    {
        public class Block
        {
            public Block(string name = null)
            {
                Name = name;
            }
            public Dictionary<string, Function> Variables { get; set; }
            public string Name { get;set;}
        }
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
        protected virtual variables.Variable Evaluate(string data) => throw new Exception("Not Implemented");
        public Function Implimentation { get; set; }
        public String Name { get; set; }
        private static Stack<Block> levels = new Stack<Block>();
        public static Stack<Block> ExecutionStack => levels;
        public Block CurrentBlock => levels.Peek();
    }

    class ConditionalProcess : Function 
    {
        internal ConditionalProcess(string name,TokensList list, int idx) : base(name) { 
        
        }
        private TokensList ConditionToken() { throw new Exception("not yet implemented"); }
        public bool Condition => (bool)Parser.Evaluate(ConditionToken());
    }

    class IfProcess         : ConditionalProcess
    {
        public IfProcess(string name, TokensList tokens, int idx) : base(name, tokens, idx) {
            Name = "If";
        }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("",null,"");
        }
    }

    class WhileProcess      : ConditionalProcess
    {
        public WhileProcess(string name , TokensList tokens, int idx) : base(name, tokens,idx) {
            Name = "While";
        }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("", null, "");
        }
    }

    class ForProcess        : WhileProcess
    {
        public ForProcess(string name, TokensList tokens, int idx) : base(name, tokens, idx)
        {
            Name = "For";
        }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("", null, "");
        }
    }

    class ReturnProcess     : Function
    {
        public ReturnProcess(string name = "Return") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("", null, "");
        }
    }

    class ImportProcess     : Function
    {
        public ImportProcess(string name = "Import") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("", null, "");
        }
    }

    class VariableProcess   : Function
    {
        public VariableProcess(string name = "Variable") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("", null, "");
        }
    }

    class ClassProcess : Function
    {
        public ClassProcess(string name = "Class") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("",null,"");
        }
    }

    class FunctionProcess : Function
    {
        public FunctionProcess(string name = "Function") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("",null,"");
        }
    }

    class PrintProcess : Function
    {
        public PrintProcess(string name = "Print") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("",null,"");
        }
    }

    class ReadProcess : Function
    {
        public ReadProcess(string name = "Read") : base(name) { }
        protected override variables.Variable Evaluate(string data)
        {
            return new variables.Variable("",null,"");
        }
    }
}
