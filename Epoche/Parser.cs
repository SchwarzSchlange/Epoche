using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Epoche.Token;

namespace Epoche
{
    internal class Parser
    {
        public static void ParseLine(string LineText,int LineIndex)
        {
            Line line = new Line { LineIndex = LineIndex, LineTokens = new List<Token>(),LineValue = LineText };
            Tokenize(line);


            Program.epocheEngine.ScriptLines.Add(line);
        }

        private static int posLayer = 0;
        public static void Tokenize(Line Line)
        {
            Line.LineValue = Line.LineValue.TrimStart();
            Line.LineValue = Line.LineValue.Replace("'"," ' ");
            Line.LineValue = Line.LineValue.Replace("==>"," ==> ");
            Line.LineValue = Line.LineValue.Replace(":"," : ");
            Line.LineValue = Line.LineValue.Replace("("," ( ");
            Line.LineValue = Line.LineValue.Replace(")"," ) ");
            Line.LineValue = Line.LineValue.Replace(","," , ");

            var splittedText = Line.LineValue.Split(' ');


            int index = 0;
            
            foreach(var tokenValue in splittedText)
            {
                if(tokenValue.Trim() == "") { continue; }

                Token.TokenType tokenType = Token.TokenType.Undefined;

                if(index == 0) { tokenType = Token.TokenType.Starter; }
                if(tokenValue == "'") { tokenType = Token.TokenType.String; }

                if(tokenValue == "{") { tokenType = Token.TokenType.InnerStart;posLayer++; }
                if(tokenValue == "}") { tokenType = Token.TokenType.InnerEnd;}

                if(tokenValue == "(") { tokenType = Token.TokenType.ParamStarter;}
                if(tokenValue == ")") { tokenType = Token.TokenType.ParamEnder;}

                if(tokenValue == ",") { tokenType = Token.TokenType.Comma;}

                if(tokenValue == "==>") { tokenType = Token.TokenType.UNTIL;}
                if(tokenValue == ":") { tokenType = Token.TokenType.LOCAL_DEFINER;}
                if(tokenValue == "==" || tokenValue == ">" || tokenValue == "<") { tokenType = Token.TokenType.EQUALITY_EXP; }



                Line.LineTokens.Add(new Token { IndexInLine = index, RootLine = Line, TokenValue = tokenValue, Type = tokenType ,DynamicValue = tokenValue,TokenLayer = posLayer});

                if (tokenValue == "}") { posLayer--; }
                index++;
            }

        }
    }
}
