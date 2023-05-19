using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    /*
        Действия +, -
        Чтение из консоли, вывод в консоль
    */

    class CodeGeneration 
    {
        public static string output_str = "";
        public static string new_value = "";

        public static void ChooseAction(int code_action)
        {
            switch (code_action)
            {
                case LexicalAnalyzer.readsy:
                case LexicalAnalyzer.readlnsy:
                    ReadLn();
                    break;
                case LexicalAnalyzer.writesy:
                    Write();
                    break;
                case LexicalAnalyzer.writelnsy:
                    Write();
                    output_str += "\n";
                    break;
                default:
                    MathAction();
                    break;
            }
        }

        private static void Write()
        {
            byte symbol = LexicalAnalyzer.NextSym();

            do
            {
                symbol = LexicalAnalyzer.NextSym();

                switch (symbol)
                {
                    case LexicalAnalyzer.quote:
                        int pos_start = InputOutput.positionNow.charNumber;
                        do
                        {
                            symbol = LexicalAnalyzer.NextSym();
                        }
                        while (symbol != LexicalAnalyzer.quote);
                        int pos_end = InputOutput.positionNow.charNumber - 1;

                        int length = pos_end - pos_start;
                        output_str += InputOutput.line.Substring(pos_start, length);

                        break;


                    case LexicalAnalyzer.ident:
                        int index = Parser.variables_names.IndexOf(LexicalAnalyzer.ident_name);
                        output_str += Parser.variables[index].value;
                        break;
                }

                symbol = LexicalAnalyzer.NextSym();

            }
            while (symbol == LexicalAnalyzer.comma);          
        }        

        private static void ReadLn()
        {
            byte symbol = LexicalAnalyzer.NextSym();

            do
            {
                symbol = LexicalAnalyzer.NextSym();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("> ");
                new_value = Console.ReadLine();
                Console.ResetColor();

                int index = Parser.variables_names.IndexOf(LexicalAnalyzer.ident_name);
                Variable copy_var = Parser.variables[index];

                ChooseType(ref copy_var, new_value);
                Parser.variables[index] = copy_var;

                symbol = LexicalAnalyzer.NextSym();
            }
            while (symbol == LexicalAnalyzer.comma);

        }

        private static void ChooseType(ref Variable var, string value)
        {
            string need_type = var.type;
            if (Array.Exists(Parser.int_types, element => element == need_type))
                need_type = "int";
            else if (Array.Exists(Parser.real_types, element => element == need_type))
                need_type = "real";

            try
            {
                switch (need_type)
                {
                    case "real":
                        float.Parse(value);
                        break;

                    case "int":
                        int.Parse(value);
                        break;

                    case "boolean":
                        bool.Parse(value);
                        break;

                    case "char":
                        char.Parse(value);
                        break;
                }
                var.value = value;
            }
            catch
            {
                TextPosition t = new TextPosition();
                t.lineNumber = InputOutput.positionNow.lineNumber;
                t.charNumber = 3;
                InputOutput.Error(78, t); // ошибка при вводе
            }


        }

        private static void MathAction()
        {
            string line = InputOutput.line.Split(":=")[1].Trim();

            line = line.Substring(0, line.Length - 1);
            string a = line.Split(' ')[0], b = line.Split(' ')[2];
            string op = line.Split(' ')[1];


            if (Parser.variables_names.Contains(a))
            {
                int index = Parser.variables_names.IndexOf(a);
                if (Parser.int_types.Contains(Parser.variables[index].type))
                    a = Parser.variables[index].value;                
            }

            if (Parser.variables_names.Contains(b))
            {
                int index = Parser.variables_names.IndexOf(b);
                if (Parser.int_types.Contains(Parser.variables[index].type))
                    b = Parser.variables[index].value;
            }


            try
            {
                int a_value = int.Parse(a);
                int b_value = int.Parse(b);
                switch (op)
                {
                    case "+":
                        new_value = (a_value + b_value).ToString();
                        break;
                    case "-":
                        new_value = (a_value - b_value).ToString();
                        break;
                    default:
                        InputOutput.Error(199, InputOutput.positionNow);
                        break;
                }
            }
            catch
            {
                InputOutput.Error(90, InputOutput.positionNow); // not int
            }

            
        }


    }
}
