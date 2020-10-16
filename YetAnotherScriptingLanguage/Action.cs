using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    class Action : Function
    {
        public Action(String action)
        {
            Operation = action;
        }
        public String Operation { get;}
        public Boolean isValidAction {
            get => this.Operation == "*"  || this.Operation == "/"  || 
                   this.Operation == "+"  || this.Operation == "-"  ||
                   this.Operation == "|"  || this.Operation == "&"  ||
                   this.Operation == "<"  || this.Operation == ">"  ||
                   this.Operation == "<>" || this.Operation == "="  ||
                   this.Operation == ":=" || this.Operation == "^"  ;
        }
        public Int16 Priority
        {
            get {
                switch (this.Operation)
                {
                    case "^" : return 10; 
                    case "*" :  
                    case "%" : 
                    case "/" : return 9;
                    case "+" :
                    case "-" : return 8;
                    case "<" :
                    case ">" :
                    case "=" :
                    case "<>": return 7;
                    case ":=": return 6;
                    default  : return 0;
                }
            }
        }
    }
}
