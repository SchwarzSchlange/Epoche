using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Epoche.Token;

namespace Epoche
{
    public class Parser
    {
        public static EpocheClass PreParseForProgramInformation(string programPath)
        {
            if (!File.Exists(programPath)) { Log.Error($"Pre Parsing Error : The given path for the epoche class does not exist! ({programPath})"); return null; }

            int index = 1;

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(programPath))

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    line = line.Trim();
                    var stringTokens = line.Split(' ');

                    if (stringTokens[0] == "program")
                    {
                        if (stringTokens.Length != 2) { Log.Error("Please give program information for the class"); return null; }
                        
                        string programName = stringTokens[1];

                        var epocheClass = new EpocheClass();

                        epocheClass.ProgramName = programName;
                        epocheClass.referencePathToClassFile = programPath;
                        epocheClass.ProgramEngine.LastScriptPath = programPath;
                       

                        return epocheClass;
                    }

                    index++;
                }
            }

            return null;
        }

        public static void ParseLine(string LineText,int LineIndex,EpocheClass epocheClass)
        {
            Line line = new Line { LineIndex = LineIndex, LineTokens = new List<Token>(),LineValue = LineText,RootClass = epocheClass};
            Tokenize(line);


            epocheClass.ProgramEngine.ScriptLines.Add(line);
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
