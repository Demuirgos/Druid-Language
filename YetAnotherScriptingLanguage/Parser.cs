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
        public void Parse() 
        {
            index++;
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
        public Node(variables.Variable value, Action action)
        {
            Value = value;
            Operation = action;
        }
        public Action Operation { get; set; }
        public variables.Variable Value { get; set; }
    }
}
