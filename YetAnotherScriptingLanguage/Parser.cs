using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Parser
    {
        public static bool Verbose { get; set; }
        private int index;
        private TokensList _tokens;
        public Parser(TokensList tokens)
        {
            index = 0;
            _tokens = tokens;
        }

        public LinkedList<Node> Parse(TokensList expression)
        {
            var Tree = new LinkedList<Node>();
            for(int i = 0 ; i < expression.Count ; i++)
            {
                if (i % 2 == 0)
                {

                }
                else
                {

                }
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

        public static object Evaluate(TokensList expression)
        {
            Action<Node,Node> Merge =  (Node left, Node right) => {
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
                    case "EQUAL":
                        left.Value = new variables.Variable(left.Value == right.Value, variables.Variable.type.Boolean);
                        break;
                    case "Diff":
                        left.Value = new variables.Variable(left.Value != right.Value, variables.Variable.type.Boolean);
                        break;
                    case "And":
                        left.Value = new variables.Variable(left.Value & right.Value, variables.Variable.type.Boolean);
                        break;
                    case "Or":
                        left.Value = new variables.Variable(left.Value | right.Value, variables.Variable.type.Boolean);
                        break;
                }
                left.Operation = right.Operation;
            };
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
            isRoot = true;
        }

        public Node(variables.Variable value, Action action)
        {
            Value = value;
            Operation = action;
        }

        public bool isRoot { get; set; }
        public Action Operation { get; set; }
        public variables.Variable Value { get; set; }
    }
}
