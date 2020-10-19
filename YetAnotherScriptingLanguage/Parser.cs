using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Parser
    {
        class ProceedFlag
        {
            public bool this[Node left, Node right] => left.Operation.Priority > right.Operation.Priority;
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
            for(int i = 0 ; i < expression.Count ; i+=2)
            {
                variables.Variable v = null;
                if (expression[i].Type == Token.type.constant && !expression[i].IsFunction)
                {
                    v = new variables.Variable(expression[i].Word);
                }
                else if (expression[i].Type == Token.type.constant && expression[i].IsFunction)
                {
                    v = new variables.Variable((variables.Variable)Parser.Evaluate(Parser.Parse(expression[i].Spread())));
                }
                var o = new Action(expression[i+1].Word);
                var node = new Node(v, o);
                Tree.AddLast(node);
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

        public static object Evaluate(LinkedList<Node> expression , bool Once=false)
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
                }
                left.Operation = right.Operation;
                return left;
            };
            if (expression.Count != 1)
            {
                var root = expression.First;
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
            else
            {
                return expression.First.Value.Value;
            }
            throw new Exception("not yet made");
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
