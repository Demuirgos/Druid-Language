using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Action : Function
    {
        public Action(String action)
        {
            Operator = action;
        }
        public String Operator { get;}
        public Boolean isValidAction {
            get => this.Operator == "*"  || this.Operator == "/"  || 
                   this.Operator == "+"  || this.Operator == "-"  ||
                   this.Operator == "|"  || this.Operator == "&"  ||
                   this.Operator == "<"  || this.Operator == ">"  ||
                   this.Operator == "<>" || this.Operator == "="  ||
                   this.Operator == ":=" || this.Operator == "^"  ||
                   this.Operator == "%"  || this.Operator == "!"  ;
        }
        public Int16 Priority
        {
            get {
                switch (this.Operator)
                {
                    case "^" : return 10;
                    case "%" : return 9;
                    case "/" : return 8;
                    case "*" : return 7;
                    case "-" : return 6;
                    case "+" : return 5;
                    case "<" :
                    case ">" :
                    case "=" :
                    case "<>": return 4;
                    case "!" : return 3;
                    case "|":
                    case "&" : return 2;
                    case ":=": return 1;
                    default  : return 0;
                }
            }
        }
    }
}
