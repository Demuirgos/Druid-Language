using System;
using System.Collections.Generic;
using System.Text;

namespace YetAnotherScriptingLanguage
{
    public class Action : Function
    {
        public Action(Token action)
        {
            Operator = action;
        }
        public Token Operator { get;}
        public Boolean isValidAction {
            get => this.Operator.IsKeyword == "MULTIPLY"    || this.Operator.IsKeyword == "DIVIDE"  ||
                   this.Operator.IsKeyword == "PLUS"        || this.Operator.IsKeyword == "MINUS"   ||
                   this.Operator.IsKeyword == "OR"          || this.Operator.IsKeyword == "AND"     ||
                   this.Operator.IsKeyword == "SMALLER"     || this.Operator.IsKeyword == "BIGGER"  ||
                   this.Operator.IsKeyword == "DIFF"        || this.Operator.IsKeyword == "EQUAL"   ||
                   this.Operator.IsKeyword == "ASSIGNMENT"  || this.Operator.IsKeyword == "POWER"   ||
                   this.Operator.IsKeyword == "REMAINDER"   || this.Operator.IsKeyword == "NOT"     ||
                   this.Operator.IsKeyword == "APPEND"      || this.Operator.IsKeyword == "REMOVE";
        }
        public Int16 Priority
        {
            get {
                switch (this.Operator.IsKeyword)
                {
                    case "POWER"        : return 15;
                    case "REMAINDER"    : return 14;
                    case "DIVID"        : 
                    case "MULTIPLY"     : return 13;
                    case "MINUS"        : 
                    case "PLUS"         : return 12;
                    case "SMALLER"      :
                    case "BIGGER"       :
                    case "EQUAL"        :
                    case "DIFF"         : return 11;
                    case "NOT"          : return 10;
                    case "OR"           :
                    case "AND"          : return 9;
                    case "APPEND"       : 
                    case "REMOVE"       : return 8;
                    case "SET"          : return 7;
                    default             : return 0;
                }
            }
        }
    }
}
