using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Interpreter
    {
        public static bool Verbose { get; set; }
        public class Block
        {
            public Block(string name = null)
            {
                Name = name;
                variables = new Dictionary<string, Function>();
            }
            public Dictionary<string, Function> variables { get; set; }
            public Dictionary<string, Function> Variables => variables;
            public string Name { get; set; }
        }
        private Parser main;

        private string _script;
        private TranslationUnit Local;
        public static Dictionary<string, Function> Functions = new Dictionary<string, Function>();
        public static Dictionary<string, Action> Actions = new Dictionary<string, Action>();

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

        public static POST.GET Get = new POST.GET();
        public static POST.SET Set = new POST.SET();
        public static POST.POP Pop = new POST.POP();

        public Interpreter(string script)
        {
            _script = script;
            Initialize();
        }
        public void Initialize()
        {
            SetUp();
            Local = new TranslationUnit(_script);
            main = new Parser(Local.Tokens);
        }
        public void SetUp()
        {
            MathProcess.SetupFunctions();
            ConstantsMap.SetUpCanstants();
            keywords = new KeyWords();
            foreach (var word in keywords)
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

            //setup printRead behaviour
            ((PrintProcess)Interpreter.Functions["Print"]).PrintHandler += Interpreter_PrintHandler1;
            ((ReadProcess)Interpreter.Functions["Read"]).ReadHandler += Interpreter_ReadHandler;
        }

        private variables.Variable Interpreter_ReadHandler(ReadProcess sender, variables.Variable Argument)
        {
            Console.Write(sender.Arguments[0].Value + (sender.Arguments[0].Value == ""?"":" "));
            return new variables.Variable(Console.ReadLine());
        }

        private void Interpreter_PrintHandler1(PrintProcess sender, List<variables.Variable> Arguments)
        {
            for (int i = 0; i < sender.Arguments.Count; i++)
                Console.Write(sender.Arguments[i].Value + (i < sender.Arguments.Count - 1 ? " " : Environment.NewLine));
        }

        public static void logStacks()
        {
            foreach(var valuePair in Interpreter.CurrentBlock.Variables)
            {
                Console.WriteLine(valuePair.Key + " : " + valuePair.Value);
            }
        } 
    }
    namespace POST
    {
        class GET
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
                        else
                        {
                            var stacks = Interpreter.ExecutionStack.ToArray();
                            foreach(var block in stacks)
                            {
                                if (block.Variables.ContainsKey(token))
                                    return block.Variables[token];
                            }
                        }
                    }
                    if (Interpreter.Functions.ContainsKey(token)) return Interpreter.Functions[token];
                    throw new Exception("Method undefined");
                }
            }
        }

        class  SET
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

        class POP
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
    }
}
