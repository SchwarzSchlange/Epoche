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
        public static Epoche epocheEngine = new Epoche();
        static void Main(string[] args)
        {
            InitilazeConsole();

            Log.Sucess("Epoche Script 1.0, running on " + epocheEngine.CurrentRunEnvironment);
            


            Console.WriteLine("Script path :");
            Console.Write(">>> ");


            //string path = Console.ReadLine();

            string path = "C:\\Users\\User\\Desktop\\script.txt";
            if (!File.Exists(path)) { Log.Error("Please enter a valid path!"); return; }


            int index = 1;
            epocheEngine.LastScriptPath = path;
            const Int32 BufferSize = 128;
            using (var fileStream = File.OpenRead(path))
                
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, BufferSize))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    Parser.ParseLine(line, index);

                    index++;
               }
            }

            /*
            foreach (var line in epocheEngine.ScriptLines)
            {
                Console.WriteLine("");
                Console.WriteLine("Line :" + line.LineIndex);
                foreach (var token in line.LineTokens)
                {
                   
                    Console.WriteLine($"({token.TokenLayer}) [" + token.IndexInLine + "] " + token.TokenValue);
                }

                Log.Seperator();
            }
            */
            
            
            

            //TITLE 
            Console.WriteLine("");
            Log.Seperator(10); Console.Write("Output"); Log.Seperator(10);
            Console.WriteLine("");

            var watch = System.Diagnostics.Stopwatch.StartNew();
            Processer.ProcessLines(epocheEngine.ScriptLines);
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;


            Log.Seperator(26);
            Console.WriteLine("");
            Log.Sucess("Runtime : " + elapsedMs + " ms");
            Console.ReadLine();
          
      
        }

        private static void InitilazeConsole()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WindowHeight = 30;
            Console.Title = "Epoche Script 1.0";

        }
    }
}
