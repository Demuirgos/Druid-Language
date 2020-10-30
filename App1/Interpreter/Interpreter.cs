using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    public class Interpreter
    {


        public static bool Verbose { get; set; }
        public class Block
        {
            public Block(string name = null)
            {
                Name = name;
                variables = new Dictionary<string, Function>();
            }
            public Dictionary<string, Function> variables;
            public Dictionary<string, Function> Variables => variables;
            public string Name { get; set; }
        }
        private Parser main;

        private string _script;
        private TranslationUnit Local;
        public static Dictionary<string, Function> Functions = new Dictionary<string, Function>();
        public static Dictionary<string, Action> Actions = new Dictionary<string, Action>();
        public static Queue<variables.Variable> ReturnValue = new Queue<variables.Variable>();

        private static KeyWords keywords;
        public static KeyWords Keywords { 
            get
            {
                if (keywords == null)
                {
                    keywords = new KeyWords();
                }
                return keywords;
            }
        }

        public TokensList tokens { get; set; }
        public int index = 0;

        private static Stack<Block> levels = new Stack<Block>();
        public static Stack<Block> ExecutionStack => levels;
        public static Block CurrentBlock => levels.Peek();

        public static COMMANDS.GET  Get  = new COMMANDS.GET() ;
        public static COMMANDS.SET  Set  = new COMMANDS.SET() ;
        public static COMMANDS.POP  Pop  = new COMMANDS.POP() ; 
        public static COMMANDS.PEEK Peek = new COMMANDS.PEEK(); 
        public static COMMANDS.POST Post = new COMMANDS.POST(); 

        public Interpreter(string script)
        {
            _script = script;
            Initialize();
        }
        public void Initialize()
        {
            setUpExternals();
            SetUp();
            Local = new TranslationUnit(_script);
            main = new Parser(Local.Tokens);
        }
        
        void setUpExternals()
        {
            MathProcess.SetupFunctions();
            ConstantsMap.SetUpCanstants();
        }

        public void SetUp()
        {
            foreach (var word in Interpreter.Keywords)
            {
                switch (word.Value)
                {
                    case ("If"): 
                        Interpreter.Functions.Add("If", new IfProcess());
                        break;
                    case ("While"):
                        Interpreter.Functions.Add("While", new WhileProcess());
                        break;
                    case ("Print"):
                        Interpreter.Functions.Add("Print", new PrintProcess());
                        break;
                    case ("Read"):
                        Interpreter.Functions.Add("Read", new ReadProcess());
                        break;
                    case ("Return"):
                        Interpreter.Functions.Add("Return", new ReturnProcess());
                        break;
                    case ("Variable"):
                        Interpreter.Functions.Add("Variable", new VariableProcess());
                        break;
                    case ("For"):
                        Interpreter.Functions.Add("For", new ForProcess());
                        break;
                    case ("Import"):
                        Interpreter.Functions.Add("Import", new ImportProcess());
                        break;
                    case ("Class"):
                        Interpreter.Functions.Add("Class", new ClassProcess());
                        break;
                    case ("Function"):
                        Interpreter.Functions.Add("Function", new FunctionProcess());
                        break;
                    case ("ASSIGNMENT"):
                        Interpreter.Functions.Add("ASSIGNMENT", new SetProcess());
                        break;
                    case ("Open"):
                        Interpreter.Functions.Add("Open", new OpenProcess());
                        break;
                    case ("Array"):
                        Interpreter.Functions.Add("Array", new ArrayProcess());
                        break;
                }
            }
            foreach (var maps in MathProcess.DualFunctions.Keys)
            {
                Interpreter.Functions.Add(maps, new MathProcess(maps));
            }
            foreach (var maps in MathProcess.UnaryFunctions.Keys)
            {
                Interpreter.Functions.Add(maps, new MathProcess(maps));
            }
            foreach (var csts in ConstantsMap.Canstants.Keys)
            {
                Interpreter.Functions.Add(csts, new ConstantsMap(csts));
            }
            Interpreter.Keywords.UpdateColorMap();
        }

        public void Reset()
        {
            Functions.Clear();
            Interpreter.ExecutionStack.Clear();
            SetUp();
        }

        public static void logStacks()
        {
            foreach(var stack in Interpreter.ExecutionStack)
            {
                foreach(var variable in stack.Variables)
                    Console.WriteLine(variable.Key + " : " + ((variables.Variable)variable.Value).Type.ToString());
            }
        } 
    }
    namespace COMMANDS
    {
        public class GET
        {
            internal GET() { }
            public Function this[string token]
            {
                get
                {
                    if (Interpreter.ExecutionStack.Count > 0)
                    {
                        if (Interpreter.CurrentBlock.Variables.ContainsKey(token))
                            return Interpreter.CurrentBlock.Variables[token];
                    }
                    if (Interpreter.Functions.ContainsKey(token)) return Interpreter.Functions[token];
                    throw new Exception(token + " Method undefined");
                }
            }
        }

        public class SET
        {
            internal SET() { }
            public Function  this[string token]
            {
                set
                {
                    value.Name = token;
                    if (Interpreter.ExecutionStack.Count == 0)
                    {
                        Interpreter.Set.Insert(new Interpreter.Block());
                    }
                    Interpreter.CurrentBlock.Variables.Add(token, value);
                }
            }

            public void Insert(Interpreter.Block block)
            {
                Interpreter.ExecutionStack.Push(block);
            }
        }

        public class POP
        {
            internal POP() { }
            public bool this[string token]
            {
                get
                {
                    bool found = Interpreter.CurrentBlock.Variables.ContainsKey(token);
                    if (found)
                    {
                        Interpreter.CurrentBlock.Variables.Remove(token);
                    }
                    return found;
                }
            }
        }

        public class PEEK
        {
            internal PEEK() { }
            public bool this[string token]
            {
                get
                {
                    return (Interpreter.ExecutionStack.Count>0 && Interpreter.CurrentBlock.Variables.ContainsKey(token)) || Interpreter.Functions.ContainsKey(token);
                }
            }
        }

        public class POST
        {
            internal POST() { }
            public Function this[string token]
            {
                set
                {
                    Interpreter.Functions.Add(token, value);
                }
            }
        }
    }
}
