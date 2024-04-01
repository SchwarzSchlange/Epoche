using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class Token
    {
        public enum TokenType
        {
            Undefined,
            Starter,
            Variable,
            InnerStart,
            InnerEnd,
            DefinedVariable,
            String,
            EQUALITY_EXP,
            UNTIL,
            LOCAL_DEFINER,
            ParamStarter,
            ParamEnder,
            Comma
        }


        public TokenType Type { get; set; }
        public int IndexInLine { get; set; }
        public string TokenValue { get;  set; }
        public string DynamicValue { get;  set; }
        public Line RootLine { get;  set; }
        public int TokenLayer { get; set; } = 0; // 0 MEANS OUTER ELEMENT E | 1 MEANS UNDER {E} | 2 MEANS UNDER {{E}}
        


        public Token GetTokenBefore()
        {
            if (IndexInLine - 1 >= 0)
            {
                return RootLine.LineTokens[IndexInLine - 1];
            }
            else
            {
                return null;
            }
        }

        public Token GetTokenAfter()
        {
            if(IndexInLine + 1 < RootLine.LineTokens.Count)
            {
                return RootLine.LineTokens[IndexInLine + 1];
            }
            else
            {
                return null;
            }
           
        }
    }
}
