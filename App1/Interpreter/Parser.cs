using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Parser
    {
        public enum state
        {
            Normal,
            Suspended,
            TemporalSuspension,
        }

        public static state ParserState { get; set; }
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
            ParserState = state.Normal;
        }

        

        public static LinkedList<Node> Parse(TokensList expression)
        {
            var Tree = new LinkedList<Node>();
            int i = 0;
            int j = i;
            while(i<expression.Count && ParserState == state.Normal)
            {
                variables.Variable v = null;
                while (i < expression.Count && expression[i].Type == Token.type.Separator) i++;
                j = i;
                if (expression[i].Type == Token.type.constant)
                {
                    v = new variables.Variable(expression[i].Word);
                    i++;
                }
                else if(expression[i].Type == Token.type.Math)
                {
                    v = new variables.Variable(Parser.Evaluate(Parser.Parse(expression[i].Spread())));
                    i++;
                }
                else if (expression[i].Type == Token.type.variable)
                {
                    v = (variables.Variable)Interpreter.Get[expression[i].Word];
                    if (v.Type == variables.Variable.type.Array && (i + 1 < expression.Count && expression[i + 1].Type == Token.type.array))
                    {
                        v = variables.Array.getElement(v as variables.Array, expression[i + 1]);
                        i++;
                    }
                    i++;
                }
                else if (expression[i].Type == Token.type.array)
                {
                    v = new variables.Array(expression[i]);
                    i++;
                }
                else if(expression[i].Type == Token.type.Skip)
                {
                    Parser.ParserState = state.TemporalSuspension;
                    i = expression.Count;
                }
                else if(expression[i].Type == Token.type.Exit)
                {
                    Parser.ParserState = state.Suspended;
                    i = expression.Count;
                }
                else if (expression[i].Type == Token.type.function)
                {
                    Function foo = new Function(expression[i].IsKeyword);
                    var Body = expression[i, foo.Limiter];
                    i += Body.Count;
                    if (foo.Type == Function.type.function)
                    {
                        v = foo[Body];
                        if (foo.Name == "RETURN")
                        {
                            Tree.Clear();
                            Interpreter.ReturnValue.Enqueue(v);
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
                Action o = new Action(new Token("SKIP"));
                if (i < expression.Count)
                {
                    o = new Action(expression[i]);
                    if (o.Operator.IsKeyword == "SET")
                    {
                        Function foo = new Function(expression[i].IsKeyword);
                        var Body = expression[j + 1, foo.Limiter];
                        i += Body.Count - i + j - 2;
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

        public static variables.Variable Evaluate(LinkedList<Node> expression , bool Once=false)
        {
            Func<Node,Node,Node> Merge =  (Node left, Node right) => {
                switch (left.Operation.Operator.IsKeyword)
                {
                    case "POWER":
                        left.Value ^= right.Value;
                        break;
                    case "MULTIPLY":
                        left.Value *= right.Value;
                        break;
                    case "DIVIDE":
                        left.Value /= right.Value;
                        break;
                    case "PLUS":
                        left.Value += right.Value;
                        break;
                    case "MINUS":
                        left.Value -= right.Value;
                        break;
                    case "REMAINDER":
                        left.Value %= right.Value;
                        break;
                    case "SMALLER":
                        left.Value = new variables.Variable(left.Value < right.Value, variables.Variable.type.Boolean);
                        break;
                    case "BIGGER":
                        left.Value = new variables.Variable(left.Value > right.Value, variables.Variable.type.Boolean);
                        break;
                    case "EQUAL":
                        left.Value = new variables.Variable(left.Value == right.Value, variables.Variable.type.Boolean);
                        break;
                    case "DIFF":
                        left.Value = new variables.Variable(left.Value != right.Value, variables.Variable.type.Boolean);
                        break;
                    case "AND":
                        left.Value = new variables.Variable(left.Value & right.Value, variables.Variable.type.Boolean);
                        break;
                    case "OR":
                        left.Value = new variables.Variable(left.Value | right.Value, variables.Variable.type.Boolean);
                        break;
                    case "NOT":
                        left.Value = new variables.Variable(variables.Variable.xor(left.Value, right.Value), variables.Variable.type.Boolean);
                        break;
                    case "APPEND":
                        left.Value = new variables.Array(variables.Array.Insert(left.Value, right.Value));
                        break;
                    case "REMOVE":
                        left.Value = new variables.Array(variables.Array.Remove(left.Value, right.Value));
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
