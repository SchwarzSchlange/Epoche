using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epoche
{
    public class Log
    {
        public static void ClearLastLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write(new string(' ', Console.BufferWidth));
            Console.SetCursorPosition(0, Console.CursorTop - 1);
        }


        public static void Info(object text)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{DateTime.UtcNow.ToLongTimeString()}] {text.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Seperator(int count = 15)
        {
            Console.Write(new string('-', count));
        }

        public static void Warning(object text)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine($"[{DateTime.UtcNow.ToLongTimeString()}] {text.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Error(object text)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.UtcNow.ToLongTimeString()}] {text.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void Sucess(object text)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.UtcNow.ToLongTimeString()}] {text.ToString()}");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class ProcessLine
    {
        private string[] Phases = new string[4] { "-", "/", "-", @"\" };
        public int CurrentPhase { get; private set; } = 0;

        public string Print = "";

        public void Continue()
        {
            if (CurrentPhase >= Phases.Length) { CurrentPhase = 0; } else { CurrentPhase++; }

            Print = Phases[CurrentPhase];
        }


        private static void Draw(int progress, int total)
        {
            //draw empty progress bar
            Console.CursorLeft = 0;
            Console.Write("["); //start
            Console.CursorLeft = 32;
            Console.Write("]"); //end
            Console.CursorLeft = 1;
            float onechunk = 30.0f / total;

            //draw filled part
            int position = 1;
            for (int i = 0; i < onechunk * progress; i++)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw unfilled part
            for (int i = position; i <= 31; i++)
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.CursorLeft = position++;
                Console.Write(" ");
            }

            //draw totals
            Console.CursorLeft = 35;
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write(progress.ToString() + " of " + total.ToString() + "    "); //blanks at the end remove any excess
        }
    }
}
