using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class ErrorHandler
    {
        public static void LineBased(Line line,string message)
        {
            Log.Error(message + " at line " + line.LineIndex);

            Console.ReadLine();

            Environment.Exit(0);
        }

    }
}
