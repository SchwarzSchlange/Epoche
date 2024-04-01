using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class Line
    {
        public List<Token> LineTokens { get; set; } = new List<Token>();
        public int LineIndex { get; set; }
        public string LineValue { get; set; }


        public void UpdateIndicies()
        {
            for (int i = 0;i < LineTokens.Count;i++ )
            {
                LineTokens[i].IndexInLine = i;
            }
        }


        public Line GetLineBefore()
        {
            if (this == null) { return null; }
            if (LineIndex - 1 >= 0)
            {
                return Program.epocheEngine.ScriptLines[LineIndex - 1];
            }
            else
            {
                return null;
            }
        }

        public Line GetLineAfter()
        {
            if(this == null) { return null; }
            if (LineIndex <= Program.epocheEngine.ScriptLines.Count)
            {
                try
                {
                    return Program.epocheEngine.ScriptLines[LineIndex];
                }
                catch { return null; }
            }
            else
            {
                return null;
            }

        }
    }
}
