using System;
using System.IO;

namespace Compiler
{
    class Program
    {       

        static void Main()
        {
            Console.ForegroundColor = ConsoleColor.White;
            InputOutput.NextCh();
            InputOutput.ReadErrorFile();

            while (InputOutput.flag)
            {
                Parser.Parse();
            }

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine($"Компиляция завершена: ошибок — {InputOutput.errCount}");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Вывод:\n{CodeGeneration.output_str}");
            Console.ResetColor();

            Console.ReadLine();


           
        }
                
    }
}
