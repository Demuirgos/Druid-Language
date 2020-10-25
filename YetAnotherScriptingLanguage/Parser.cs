using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Parser
    {
        class ProceedFlag
        {
            public bool this[Node left, Node right] => left.Operation.Priority >= right.Operation.Priority || right == null;
        }
        public static bool Verbose { get; set; }
        private int index;
        private TokensList _tokens;
        private static ProceedFlag Proceed = new ProceedFlag();
        public Parser(TokensList tokens)
        {
            index = 0;
            _tokens = tokens;
        }

        public static LinkedList<Node> Parse(TokensList expression)
        {
            var Tree = new LinkedList<Node>();
            int i = 0;
            while(i<expression.Count)
            {
                variables.Variable v = null;
                while (i < expression.Count && expression[i].Type == Token.type.Separator) i++;
                if (expression[i].Type == Token.type.constant)
                {
                    if (expression[i].IsMathEvaluation)
                    {
                        v = new variables.Variable((variables.Variable)Parser.Evaluate(Parser.Parse(expression[i].Spread())));
                    }
                    else
                    {
                        v = new variables.Variable(expression[i].Word);
                    }
                    i++;
                }
                else if (expression[i].Type == Token.type.variable)
                {
                    v = (variables.Variable)Interpreter.Get[expression[i].Word];
                    i++;
                }
                else if(expression[i].Type == Token.type.Skip)
                {
                    i = expression.Count;
                }
                else if(expression[i].Type == Token.type.Exit)
                {
                    Tree.Clear();
                    ConditionalProcess.State = ConditionalProcess.state.exit;
                    return Tree;
                }
                else if (expression[i].Type == Token.type.function)
                {
                    Function foo = new Function(expression[i].IsKeyword);
                    var Body = expression[i, foo.Limiter];
                    i += Body.Count;
                    if (foo.Type == Function.type.function)
                    {
                        v = foo[Body];
                        if (foo.Name == "Return")
                        {
                            Tree.Clear();
                            FunctionProcess.CustomFunction.ReturnValue.Enqueue(v);
                            return Tree;
                        }
                    }
                    else if(foo.Type == Function.type.procedure)
                    {
                        v = foo[Body];
                        continue;
                    }
                }
                while (i < expression.Count && expression[i].Type == Token.type.Separator) i++;
                Action o = new Action("Skip");
                if (i < expression.Count)
                {
                    o = new Action(expression[i].Word);
                    if (o.Operator == ":=")
                    {
                        Function foo = new Function(expression[i].IsKeyword);
                        var Body = expression[i, foo.Limiter];
                        i += Body.Count - 1;
                        v = foo[Body];
                        continue;
                    }
                }
                var node = new Node(v, o);
                Tree.AddLast(node);
                i++;
            }
            return Tree;
        }

        public Node Parse()
        {
            throw new Exception("Not Implemented Yet");
        }

        public static bool IsValid(TokensList expression)
        {

            return true;
        }

        public static variables.Variable Evaluate(LinkedList<Node> expression , bool Once=false)
        {
            Func<Node,Node,Node> Merge =  (Node left, Node right) => {
                switch (left.Operation.Operator)
                {
                    case "^":
                        left.Value ^= right.Value;
                        break;
                    case "*":
                        left.Value *= right.Value;
                        break;
                    case "/":
                        left.Value /= right.Value;
                        break;
                    case "+":
                        left.Value += right.Value;
                        break;
                    case "-":
                        left.Value -= right.Value;
                        break;
                    case "%":
                        left.Value %= right.Value;
                        break;
                    case "<":
                        left.Value = new variables.Variable(left.Value < right.Value, variables.Variable.type.Boolean);
                        break;
                    case ">":
                        left.Value = new variables.Variable(left.Value > right.Value, variables.Variable.type.Boolean);
                        break;
                    case "=":
                        left.Value = new variables.Variable(left.Value == right.Value, variables.Variable.type.Boolean);
                        break;
                    case "<>":
                        left.Value = new variables.Variable(left.Value != right.Value, variables.Variable.type.Boolean);
                        break;
                    case "&":
                        left.Value = new variables.Variable(left.Value & right.Value, variables.Variable.type.Boolean);
                        break;
                    case "|":
                        left.Value = new variables.Variable(left.Value | right.Value, variables.Variable.type.Boolean);
                        break;
                    case "!":
                        left.Value = new variables.Variable(variables.Variable.xor(left.Value,right.Value), variables.Variable.type.Boolean);
                        break;
                }
                left.Operation = right.Operation;
                return left;
            };
            if (expression.Count == 1)
            {
                return expression.First.Value.Value;
            }
            else if (expression.Count > 1)
            {
                var root = expression.First;
                if(!root.Value.Operation.isValidAction)
                {
                    expression.RemoveFirst();
                    return Evaluate(expression);
                }
                var next = root.Next;
                while (!Proceed[root.Value, next.Value])
                {
                    root = next;
                    next = root.Next;
                }
                expression.AddAfter(next,Merge(root.Value, next.Value));
                expression.Remove(root);
                expression.Remove(next);
                return Evaluate(expression);
            }
            return new variables.Variable(null,variables.Variable.type.Invalid);
        }
    }

    class Error : Exception
    {
        public Error(String msg) : base(msg)
        {
            Console.WriteLine("Error Encountered : " + msg);
        }
    }

    class Node
    {
        public Node()
        {
            Value = null;
            Operation = null;
        }

        public Node(variables.Variable value, Action action)
        {
            Value = value;
            Operation = action;
        }

        public Action Operation { get; set; }
        public variables.Variable Value { get; set; }
    }
}
