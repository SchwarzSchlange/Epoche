using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class Epoche
    {
        public string CurrentRunEnvironment { get; private set; }
        public string LastScriptPath { get; set; }

        public Epoche() {
            CurrentRunEnvironment = Environment.MachineName;
        }

        public List<Line> ScriptLines { get; set; } = new List<Line>();

        public List<Variable> Variables { get; set; } = new List<Variable> { };
        public List<Block> Blocks { get; set; } = new List<Block> { };
    }
}
