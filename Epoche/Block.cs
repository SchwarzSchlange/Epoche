using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class Block
    {
        public string Name { get; set; }
        public List<Token> ParameterTokens { get; set; }

        public List<Line> OwnLines { get; set; } = new List<Line>();
    }
}
