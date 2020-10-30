using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    public class Action : Function
    {
        public Action(String action)
        {
            Operator = action;
        }
        public String Operator { get;}
        public Boolean isValidAction {
            get => this.Operator == "*"  || this.Operator == "/" ||
                   this.Operator == "+"  || this.Operator == "-" ||
                   this.Operator == "|"  || this.Operator == "&" ||
                   this.Operator == "<"  || this.Operator == ">" ||
                   this.Operator == "<>" || this.Operator == "=" ||
                   this.Operator == ":=" || this.Operator == "^" ||
                   this.Operator == "%"  || this.Operator == "!" ||
                   this.Operator == "::" ||this.Operator == ">>";
        }
        public Int16 Priority
        {
            get {
                switch (this.Operator)
                {
                    case "^" : return 15;
                    case "%" : return 14;
                    case "/" : 
                    case "*" : return 13;
                    case "-" : 
                    case "+" : return 12;
                    case "<" :
                    case ">" :
                    case "=" :
                    case "<>": return 11;
                    case "!" : return 10;
                    case "|":
                    case "&" : return 9;
                    case "::": 
                    case ">>": return 8;
                    case ":=": return 7;
                    default  : return 0;
                }
            }
        }
    }
}
