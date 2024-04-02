using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Epoche.Token;

namespace Epoche
{
    public class EpocheClass
    {
        public string ProgramName { get; set; }

        public string referencePathToClassFile { get; set; }

        public Epoche ProgramEngine { get; set; } = new Epoche();



        public void StartReading()
        {
            if (!File.Exists(referencePathToClassFile)) { Log.Error("Please enter a valid path!"); return; }


            int index = 1;

            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(referencePathToClassFile))

            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    Parser.ParseLine(line, index,this);

                    index++;
                }
            }

            Console.WriteLine("");
            Log.Seperator(10); Console.Write("Output"); Log.Seperator(10);
            Console.WriteLine("");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            ProcessLines(ProgramEngine.ScriptLines);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            Log.Seperator(26);
            Console.WriteLine("");
            Log.Sucess("Runtime : " + elapsedMs + " ms");


   
        }



        static bool isBreaking = false;
        public void ProcessLines(List<Line> lines)
        {

            for (int a = 0; a < lines.Count; a++)
            {
                var line = lines[a];


                //Log.Warning($"Processing Line : {line.LineValue}");

                ReplaceVariablesInLine(line);




                for (int i = 0; i < line.LineTokens.Count; i++)
                {
                    var token = line.LineTokens[i];
                    if (token.Type == Token.TokenType.Starter)
                    {
                        //COMMAND START

                 
                        if (token.TokenValue == "print")
                        {
                            Console.WriteLine(GetStringBetweenTokenTypes(line, Token.TokenType.String, 1));
                        }
                        else if (token.TokenValue == "combine")
                        {
                            string classPath = GetStringBetweenTokenTypes(line, Token.TokenType.String, 1);

                            if (classPath == null) { return; }


                            string fullPathOfClass = Directory.GetCurrentDirectory() + @"\" + classPath;



                        }
                        else if (token.TokenValue == "print_inline")
                        {
                            Console.Write(GetStringBetweenTokenTypes(line, Token.TokenType.String, 1));
                        }
                        else if (token.TokenValue == "var")
                        {
                            var variableTypeToken = token.GetTokenAfter();
                            var variableNameToken = variableTypeToken.GetTokenAfter();

                            var variableValue = "UNDEFINED";
                            if (variableTypeToken.TokenValue == "string")
                            {
                                variableValue = GetStringBetweenTokenTypes(line, Token.TokenType.String, 1);
                            }
                            else if (variableTypeToken.TokenValue == "int")
                            {
                                variableValue = variableNameToken.GetTokenAfter().TokenValue;
                            }

                            AddVariable(new Variable { FirstValue = variableValue, Name = variableNameToken.TokenValue, Value = variableValue });

                        }
                        else if (token.TokenValue == "until")
                        {
                            var untilToken = line.LineTokens.Find(Token => Token.Type == TokenType.UNTIL);
                            var localDefinerToken = line.LineTokens.Find(Token => Token.Type == TokenType.LOCAL_DEFINER);

                            if (untilToken == null || localDefinerToken == null) { ErrorHandler.LineBased(line, "Please include all information to run this command"); }

                            var afterUntilToken = untilToken.GetTokenAfter();

                            var beforeLocalDefinerToken = localDefinerToken.GetTokenBefore();
                            var afterLocalDefinerToken = localDefinerToken.GetTokenAfter();

                            var stateList = GetLinesBetweenType(line.LineIndex, lines, TokenType.InnerStart, TokenType.InnerEnd, token.TokenLayer + 1);


                            var localVariable = new Variable { FirstValue = afterLocalDefinerToken.DynamicValue, Value = afterUntilToken.DynamicValue, Name = beforeLocalDefinerToken.TokenValue };
                            AddVariable(localVariable);
                            for (int iternation = int.Parse(afterLocalDefinerToken.DynamicValue); iternation <= int.Parse(afterUntilToken.DynamicValue); iternation++)
                            {
                                if (isBreaking) { isBreaking = false; break; }
                                ProgramEngine.Variables.Find(v => v.Name == beforeLocalDefinerToken.TokenValue).Value = iternation.ToString();
                                ProcessLines(stateList);
                            }
                            ProgramEngine.Variables.Remove(localVariable);


                            a = stateList.Last().LineIndex;

                        }
                        else if (token.TokenValue == "block" && token.TokenLayer == 0)
                        {
                            //block BLOCK_NAME(param1,param2,param3...)


                            var blockNameToken = line.LineTokens[i + 1];

                            if (blockNameToken == null) { return; }

                            var parameterTokens = GetTokensBetweenTypes(line, TokenType.ParamStarter, TokenType.ParamEnder, 1);



                            parameterTokens.RemoveAll(x => x.Type == TokenType.Comma);
                            parameterTokens.RemoveAll(x => x.Type == TokenType.String);


                            var ownLinesOfBlock = GetLinesBetweenType(line.LineIndex, lines, TokenType.InnerStart, TokenType.InnerEnd, token.TokenLayer + 1);


                            ProgramEngine.Blocks.Add(new Block { Name = blockNameToken.TokenValue, ParameterTokens = parameterTokens, OwnLines = ownLinesOfBlock });

                            a = ownLinesOfBlock.Last().LineIndex;
                        }
                        else if (token.TokenValue == "call")
                        {
                            // call x('30',kaan,...)

                            var blockNameToken = line.LineTokens[i + 1];

                            if (blockNameToken == null) { return; }

                            Block referenceBlock = null;
                            try
                            {
                                referenceBlock = ProgramEngine.Blocks.First(x => x.Name == blockNameToken.TokenValue);

                            }
                            catch
                            {
                                ErrorHandler.LineBased(line, "Can not find that block ");
                                return;
                            }





                            var parameterTokens = GetTokensBetweenTypes(line, TokenType.ParamStarter, TokenType.ParamEnder, 1);

                            parameterTokens.RemoveAll(x => x.Type == TokenType.Comma);
                            parameterTokens.RemoveAll(x => x.Type == TokenType.String);

                            var AddedVariables = new List<Variable>();

                           

                            try
                            {
                                for (int level = 0; level < referenceBlock.ParameterTokens.Count; level++)
                                {



                                    var vararg = new Variable { FirstValue = parameterTokens[level].DynamicValue, Value = parameterTokens[level].DynamicValue, Name = referenceBlock.ParameterTokens[level].TokenValue };


                                    AddVariable(vararg);
                                    AddedVariables.Add(vararg);
                                }
                            }
                            catch
                            {
                                ErrorHandler.LineBased(line, "Argument Error. Please mention all arguments required by block...");
                                return;
                            }

                            ProcessLines(referenceBlock.OwnLines);


                            for (int k = 0; k < AddedVariables.Count; k++) { ProgramEngine.Variables.Remove(AddedVariables[k]); }
                        }
                        else if (token.TokenValue == "is")
                        {
                            var equalityToken = line.LineTokens.Find(eqToken => eqToken.Type == Token.TokenType.EQUALITY_EXP);

                            if (equalityToken == null) { ErrorHandler.LineBased(line, "Please use a expression to handle condition!"); }

                            var valueBefore = GetStringBetweenTokenTypes(line, Token.TokenType.String, 1);
                            var valueAfter = GetStringBetweenTokenTypes(line, Token.TokenType.String, 2);


                            //Log.Info($"[{valueBefore}] [{valueAfter}]");

                            if (equalityToken.TokenValue == "==")
                            {
                                var statementLines = GetLinesBetweenType(line.LineIndex, lines, TokenType.InnerStart, TokenType.InnerEnd, token.TokenLayer + 1);

                                //Log.Warning(statementLines.Last().LineIndex + " " + statementLines.Last().LineValue);

                                // CHECK IF A 'NOT' EXPRESSION ADDED BY USER
                                bool isUsingNotExpression = false;

                                Line notExpressionToken = null;
                                if (statementLines != null)
                                {
                                    notExpressionToken = statementLines.Last().GetLineAfter();
                                }




                                List<Line> notLines = new List<Line>();

                                if (notExpressionToken != null && notExpressionToken.LineValue == "not") { notLines = GetLinesBetweenType(notExpressionToken.LineIndex, lines, TokenType.InnerStart, TokenType.InnerEnd, token.TokenLayer + 1); isUsingNotExpression = true; }


                                /*
                                if (isUsingNotExpression)
                                {
                                    Log.Warning(notExpressionToken.LineIndex);
                                    Log.Warning(notExpressionToken.LineValue);
                                }
                                */


                                //Console.WriteLine("Using :" + isUsingNotExpression.ToString());

                                if (valueBefore == valueAfter)
                                {
                                    // GET CODE BLOCK BETWEEN BRECKETS
                                    // STATE : TRUE

                                    if (isUsingNotExpression) { a = notLines.Last().LineIndex; } // WITH 'NOT' // ESCAPE READING TWICE OF THE LINES
                                    else { a = statementLines.Last().LineIndex; } // WITHOUT 'NOT' | ONLY 'IS' USED BY USER

                                    //Log.Error(a);

                                    ProcessLines(statementLines);

                                    //foreach (var statementLine in statementLines) { Console.WriteLine($"[{statementLine.LineValue.Trim()}]"); }





                                }
                                else
                                {
                                    // GET CODE BLOCK BETWEEN BRECKETS
                                    // STATE : FALSE
                                    if (!isUsingNotExpression) { return; } // NO EXPRESSION FOUND


                                    ProcessLines(notLines);


                                    a = notLines.Last().LineIndex;
                                }


                            }

                        }
                        else if (token.TokenValue == "in")
                        {
                            try
                            {
                                var variableName = token.GetTokenAfter().TokenValue;


                                var readValue = Console.ReadLine();


                                AddVariable(new Variable { FirstValue = readValue, Name = variableName, Value = readValue });
                            }
                            catch
                            {
                                ErrorHandler.LineBased(line, "Enter variable name");
                            }




                        }
                        else if (token.TokenValue == "clear")
                        {
                            Console.Clear();
                        }
                        else if (token.TokenValue == "break")
                        {
                            Console.WriteLine("breaking");
                            isBreaking = true;
                        }

                    }
                }
            }
        }


        public static List<Line> GetLinesBetweenType(int currentLineIndex, List<Line> referenceLines, TokenType referenceTypeStarter, TokenType referenceTypeEnder, int ReferenceLayer = 1)
        {

            Line lineA = null;
            Line lineB = null;


            try
            {
                lineA = referenceLines.First(searched => searched.LineTokens.Count > 0 && searched.LineIndex >= currentLineIndex && searched.LineTokens[0].TokenLayer == ReferenceLayer && searched.LineTokens[0].Type == referenceTypeStarter);
                lineB = referenceLines.First(searched => searched.LineTokens.Count > 0 && searched.LineIndex >= lineA.LineIndex && searched.LineTokens[0].TokenLayer == ReferenceLayer && searched.LineTokens[0].Type == referenceTypeEnder);

                return referenceLines.GetRange(referenceLines.IndexOf(lineA) + 1, referenceLines.IndexOf(lineB) - referenceLines.IndexOf(lineA));
            }
            catch
            {
                ErrorHandler.LineBased(referenceLines.First(), "After this line missing some types...");

            }

            return null;

        }

        public void ReplaceVariablesInLine(Line line)
        {
            foreach (var token in line.LineTokens)
            {
                if (token.TokenValue.Contains("@"))
                {
                    var splitted = token.TokenValue.Split('@');

                    if (ProgramEngine.Variables.Find(var => var.Name == splitted[1]) != null)
                    {
                        token.DynamicValue = ProgramEngine.Variables.Find(var => var.Name == splitted[1]).Value;
                    }
                    else
                    {

                        ErrorHandler.LineBased(line, splitted[1] + " could not be found as variable!");
                    }
                }
            }
        }


        public void AddVariable(Variable variable)
        {
            if (ProgramEngine.Variables.Find(x => variable.Name == x.Name) != null)
            {
                ProgramEngine.Variables.First(x => variable.Name == x.Name).Value = variable.Value;
            }
            else
            {
                ProgramEngine.Variables.Add(variable);
            }

        }

        public static List<Token> GetTokensBetweenTypes(Line referenceLine, TokenType tokenTypeA, TokenType tokenTypeB, int Layer)
        {
            int currentLayer = 0;


            Token aToken = null;
            Token bToken = null;

            foreach (Token token in referenceLine.LineTokens)
            {
                if (token.Type == tokenTypeA)
                {
                    currentLayer++;
                }
                else if (token.Type == tokenTypeB) { currentLayer--; }


                if (currentLayer == Layer)
                {
                    aToken = token;
                    bToken = referenceLine.LineTokens.First(x => x.Type == tokenTypeB && x.IndexInLine > aToken.IndexInLine);

                    break;
                }
            }

            return referenceLine.LineTokens.GetRange(referenceLine.LineTokens.IndexOf(aToken) + 1, referenceLine.LineTokens.IndexOf(bToken) - referenceLine.LineTokens.IndexOf(aToken) - 1);
        }

        public static string GetStringBetweenTokenTypes(Line referenceLine, Token.TokenType tokenType, int Layer = 1)
        {
            int LayerCounter = 0;
            int DoubleCounter = 0;

            Token a = null;
            Token b = null;

            foreach (var token in referenceLine.LineTokens)
            {
                if (token.Type == tokenType)
                {

                    DoubleCounter++;

                    if (DoubleCounter == 2)
                    {
                        DoubleCounter = 0;
                        LayerCounter++;
                    }

                    if (LayerCounter == Layer)
                    {
                        b = token;
                        a = referenceLine.LineTokens.Last(thatToken => thatToken.IndexInLine < b.IndexInLine && thatToken.Type == tokenType);

                        break;
                    }

                }
            }


            var range = referenceLine.LineTokens.GetRange(a.IndexInLine + 1, b.IndexInLine - a.IndexInLine - 1);


            string ValueString = "";
            foreach (var element in range)
            {
                if (ValueString == "")
                {
                    ValueString += element.DynamicValue;
                }
                else
                {
                    ValueString += " " + element.DynamicValue;
                }

            }

            return ValueString;


        }

    }
}
