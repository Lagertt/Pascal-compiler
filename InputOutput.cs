using System;
using System.Collections.Generic;
using System.IO;

namespace Compiler
{
    struct TextPosition
    {
        public uint lineNumber; // номер строки
        public byte charNumber; // номер позиции в строке

        public TextPosition(uint ln = 0, byte c = 0)
        {
            lineNumber = ln;
            charNumber = c;
        }
    }

    struct ListOfErr
    {
        public TextPosition errorPosition; // позиция ошибки
        public byte errorCode; // код ошибки
        public string errorText;

        public ListOfErr(TextPosition erP, byte erC, string txt)
        {
            this.errorPosition = erP;
            this.errorCode = erC;
            this.errorText = txt;

        }
    }

    class InputOutput
    {
        protected const byte ERRMAX = 9; // макс. число ошибок в строке, которые увидит пользователь
        public static char Ch { get; set; } // текущий символ
        public static TextPosition positionNow = new TextPosition(); // указатель на текущую позицию
        public static string line = ""; // считанная строка
        public static byte lastInLine = 0; // последний символ в строке
        public static List<ListOfErr> listOfErr; // список ошибок
        public static StreamReader File { get; set; } = new StreamReader("..//..//..//TxtFiles//Program.txt"); // файл для чтения
        public static uint errCount = 0; // количество ошибок
        protected static Dictionary<int, string> DictOfErrors = new Dictionary<int, string>(); // словарь ошибок
        public static bool flag = true; // включается при окончании чтения


        static public void NextCh()
        {               
            if (positionNow.charNumber == lastInLine)
            {
                int old_length = line.Length;
                ReadNextLine();                
                if ((Parser.var_flag || Parser.be_end_flag) && Ch != ';' && !Parser.have_error && old_length!= 0)
                {
                    TextPosition pos = positionNow;
                    pos.charNumber+=2;
                    Error(14, pos); // сформировать ошибку;
                    Parser.have_error = true;
                }
                   
                if (flag)
                    PrintThisLine();

                lastInLine = (line.Length != 0) ? (byte)(line.Length - 1) : (byte)0;
                positionNow.lineNumber = positionNow.lineNumber + 1;
                positionNow.charNumber = 0;
            }
            else
            {                
                ++positionNow.charNumber;
            }


            Ch = (line.Length != 0) ? line[positionNow.charNumber] : ' ';
        }

        // вывод текущей строки
        private static void PrintThisLine()
        {
            Console.WriteLine($"      {line}"); // пробелы для того, чтобы при найденной ошибки в начале символ ^ смог указать на неё
        }

        // считывание следующей строки
        private static void ReadNextLine()
        {
            if (!File.EndOfStream) // если не конец файла
            {
                line = File.ReadLine().Trim().ToLower(); // считать строку из него
                listOfErr = new List<ListOfErr>(); // обнулить список ошибок для текущей строки
            }
            else
                End();
        }

        // вывод сообщения о завершении компиляции
        static void End()
        {
            flag = false;            
            File.Close();
        }

        // вывод списка ошибок
        static public void PrintError(ListOfErr item)
        {
            string s;
            ++errCount; // увеличить кол-во ошибок
            s = "**";  // добавить в итоговую строку **
            if (errCount < 10)
                s += "0"; // добавить в итоговую строку 0 перед числами 1..9
            s += $"{errCount}**";  // добавить в итоговую строку кол-во ошибок
            while (s.Length - 2 < 3 + item.errorPosition.charNumber)
                s += " "; // добавить пробелов до позиции символа с ошибкой
            s += $"^ ошибка код {item.errorCode}\n";  // добавить в итоговую строку указатель и код ошибки
            s += $"****** {item.errorText}";
            Console.ForegroundColor = ConsoleColor.Red; // сделать агрессивный цвет :)
            Console.WriteLine(s);
            Console.ResetColor();
        }

        // редактирует список ошибок
        static public void Error(byte errorCode, TextPosition position)
        {
            ListOfErr e;
            if (listOfErr.Count <= ERRMAX) // если кол-во ошибок не больше максимального
            {
                e = new ListOfErr(position, errorCode, DictOfErrors[errorCode]); // создать новую ошибку
                listOfErr.Add(e); // добавить её в список
                PrintError(e);
            }
            
        }

        // считывание словаря ошибок из файла
        static public void ReadErrorFile()
        {
            StreamReader f = new StreamReader("..//..//..//TxtFiles//ErrorsList.txt");
            while (!f.EndOfStream) // пока не конец файла
            {
                string[] str = f.ReadLine().Split('|'); // в массив строк считываем разделённую строку
                try
                {
                    int code = int.Parse(str[0]); 
                    string text = str[1];
                    DictOfErrors.Add(code, text);
                }
                catch(FormatException e)
                {
                    Console.WriteLine(e.Message); // если что-то пошло не так, выводится ошибка
                }
                
            }
        }

    }
}