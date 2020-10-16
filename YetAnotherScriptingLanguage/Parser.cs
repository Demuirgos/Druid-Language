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
        public static object Evaluate(TokensList expression)
        {
            throw new Exception("Not yet implemented");
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
        public Node(object value, Action action)
        {
            Value = value;
            Operation = action;
        }
        public Action Operation { get; }
        public object Value { get; set; }
    }
}
