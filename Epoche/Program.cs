using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    internal class Program
    {

        static void Main(string[] args)
        {
            InitilazeConsole();
            Log.Sucess("Epoche Script 1.0");

            /*
            Console.WriteLine("Script path :");
            Console.Write(">>> ");
            */
            
            string path = "E:\\Users\\User\\source\\repos\\Epoche\\Epoche\\bin\\Debug\\main.epoche";


            var MainClass = Parser.PreParseForProgramInformation(path);




            MainClass.StartReading();   
        }

  
        private static void InitilazeConsole()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WindowHeight = 30;
            Console.Title = "Epoche Script 1.0";

        }
    }
}
