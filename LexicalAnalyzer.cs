using System;
using System.IO;

namespace Compiler
{
    class LexicalAnalyzer
    {        
        public const byte
            star = 21, // *
            slash = 60, // /
            equal = 16, // =
            comma = 20, // ,
            semicolon = 14, // ;
            colon = 5, // :
            quote = 19, // '
            point = 61,	// .
            arrow = 62,	// ^
            leftpar = 9,	// (
            rightpar = 4,	// )
            lbracket = 11,	// [
            rbracket = 12,	// ]
            flpar = 63,	// {
            frpar = 64,	// }
            later = 65,	// <
            greater = 66,	// >
            laterequal = 67,	//  <=
            greaterequal = 68,	//  >=
            latergreater = 69,	//  <>
            plus = 70,	// +
            minus = 71,	// –
            lcomment = 72,	//  (*
            rcomment = 73,	//  *)
            assign = 51,	//  :=
            twopoints = 74,	//  ..
            ident = 2,	// идентификатор
            floatc = 82,	// вещественная константа
            intc = 15,	// целая константа
            chark = 83, // символьная константа
            casesy = 31, // case
            elsesy = 32, // else
            filesy = 57, // file
            gotosy = 33, // goto
            thensy = 52, // then
            typesy = 34, // type
            untilsy = 53, // until
            dosy = 54, //do
            withsy = 37, //with
            ifsy = 56, //if
            insy = 100, //in
            ofsy = 101, //of
            orsy = 102, //or
            tosy = 103, //to
            endsy = 104, //end
            varsy = 105, //var
            divsy = 106, //div
            andsy = 107, //and
            notsy = 108, //not
            forsy = 109, //for
            modsy = 110, //mod
            nilsy = 111, //nil
            setsy = 112, //set
            beginsy = 113, //begin
            whilesy = 114, //while
            arraysy = 115, //array
            constsy = 116, //const
            labelsy = 117, //label
            downtosy = 118, //downto
            packedsy = 119, //packed
            recordsy = 120, //record
            repeatsy = 121, //repeat
            programsy = 122, //program
            functionsy = 123, //function
            proceduresy = 124, //proceduren
            falsesy = 23,
            truesy = 22,

            readsy = 125,
            readlnsy = 126,
            writesy = 127,
            writelnsy = 128;
        

        static byte symbol; // код символа
        static TextPosition token; // позиция символа

        static int nmb_int; // значение целой константы
        static float nmb_float; // значение вещественной константы 

        static bool flag_pair = false; //флаг показывает, открыта ли скобка

        public static string ident_name = "";
        public static int int_value = 0;
        public static double float_value = 0;



        public static StreamWriter File = new StreamWriter("..//..//..//TxtFiles//SymbolCode.txt", false);


        static public byte NextSym()
        {
            //пропуск табуляций и пробелов
            while ((InputOutput.Ch == '\t' || InputOutput.Ch == ' ') && InputOutput.flag) 
                InputOutput.NextCh();
            token.lineNumber = InputOutput.positionNow.lineNumber;
            token.charNumber = InputOutput.positionNow.charNumber;

            //сканировать символ

            if (InputOutput.flag)
            {
                switch (InputOutput.Ch)
                {
                    case char c when char.IsDigit(InputOutput.Ch): // сканирование цифры
                        byte digit;
                        Int16 maxint = Int16.MaxValue; // максимальный предел целочисленной конастанты (32767)
                        float maxfloat = 1000; // максимальный предел вещественной конастанты (для теста)
                        nmb_int = 0;
                        nmb_float = 0;
                        while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                        {
                            digit = (byte)(InputOutput.Ch - '0');
                            if (nmb_int < maxint / 10 ||
                                 (nmb_int == maxint / 10 &&
                                   digit <= maxint % 10))
                                nmb_int = 10 * nmb_int + digit; //записываем каждый символ в свой разряд числа
                            else
                            {
                                // константа превышает предел
                                InputOutput.Error(203, InputOutput.positionNow);
                                nmb_int = 0;
                                while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                                    InputOutput.NextCh();
                            }
                            InputOutput.NextCh();
                        }
                        if (InputOutput.Ch == '.') //если началась вещественная часть числа
                        {
                            int count_raz = 0;
                            InputOutput.NextCh();
                            while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                            {
                                digit = (byte)(InputOutput.Ch - '0');
                                if (nmb_float < maxint / 10 ||
                                     (nmb_float == maxint / 10 &&
                                       digit <= maxint % 10))
                                {
                                    nmb_float = 10 * nmb_float + digit; //записываем каждый символ в свой разряд числа
                                    count_raz++;
                                }

                                else
                                {
                                    // константа превышает предел
                                    InputOutput.Error(203, InputOutput.positionNow);
                                    nmb_float = 0;
                                    while (InputOutput.Ch >= '0' && InputOutput.Ch <= '9')
                                        InputOutput.NextCh();
                                }
                                InputOutput.NextCh();
                            }
                            if (InputOutput.Ch == '.')
                            {
                                if (count_raz > 0)
                                {
                                    InputOutput.Error(100, InputOutput.positionNow); //ошибка в записи числа
                                    break;
                                }
                                else                                
                                    InputOutput.positionNow.charNumber--;   
                            }                                    
                            else
                                //складываем целую и вещественную часть
                                nmb_float = (float)(nmb_int + nmb_float / Math.Pow(10, count_raz));
                        }

                        //присваиваем значение в symbol в зависимости от типа числа
                        int_value = nmb_int;
                        float_value = nmb_int;

                        symbol = (nmb_float == 0) ? intc : floatc;

                        break;

                    case char c when char.IsLetter(InputOutput.Ch): // сканирование буквы
                        string name = "";
                        while ((InputOutput.Ch >= 'a' && InputOutput.Ch <= 'z') || 
                                (InputOutput.Ch >= 'A' && InputOutput.Ch <= 'Z') ||
                                  (InputOutput.Ch >= '0' && InputOutput.Ch <= '9'))
                        {
                            name += InputOutput.Ch; //записываем слово целиком

                            if (InputOutput.positionNow.charNumber == InputOutput.lastInLine)
                            {
                                InputOutput.NextCh();
                                break;
                            }

                            InputOutput.NextCh();                            
                        }

                        //если нашлась соответствующая константа в словаре, то возвращаем её значение, иначе ident
                        byte value;
                        symbol = (Keywords.kw[name.Length].TryGetValue(name, out value)) ? value : ident;

                        if (symbol == ident)
                            ident_name = name;
                        break;


                    case '<':
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = laterequal;
                            InputOutput.NextCh();
                        }
                        else
                            if (InputOutput.Ch == '>')
                        {
                            symbol = latergreater;
                            InputOutput.NextCh();
                        }
                        else
                            symbol = later;
                        break;

                    case '>': //добавлено к шаблону
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = greaterequal;
                            InputOutput.NextCh();
                        }
                        else
                            symbol = greater;
                        break;

                    case ':':
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '=')
                        {
                            symbol = assign;
                            InputOutput.NextCh();
                        }
                        else
                            symbol = colon;
                        break;

                    case ';':
                        symbol = semicolon;
                        InputOutput.NextCh();
                        break;

                    case '.':
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '.')
                        {
                            symbol = twopoints;
                            InputOutput.NextCh();
                        }
                        else
                            symbol = point;
                        break;

                    //неразрешённые символы
                    case '&':
                    case '?':
                    case '%':
                        symbol = 0;
                        InputOutput.Error(6, InputOutput.positionNow);
                        InputOutput.NextCh();
                        break;

                    case '/':
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '/') //начало однострочного комментария
                        {
                            while (InputOutput.positionNow.charNumber != InputOutput.lastInLine)
                                InputOutput.NextCh(); //пропускаем всё до конца строки
                            InputOutput.NextCh();
                            NextSym();
                        }
                        else
                        {
                            symbol = semicolon;
                        }
                        break;

                    case '(':
                        InputOutput.NextCh();
                        if (InputOutput.Ch == '*') //начало многострочного комментария1
                        {
                            StartComment('*');
                            NextSym();
                        }
                        else
                        {
                            flag_pair = true; //открыть скобку
                            symbol = leftpar;

                        }                            
                        break;

                    case ')':
                        if (flag_pair) //если до этого была открыта скобка
                            symbol = rightpar;
                        else
                            InputOutput.Error(110, InputOutput.positionNow); //ошибка на не открытый комментарий
                        flag_pair = false; //закрыть скобку
                        InputOutput.NextCh();
                        break;

                    case '{': //начало многострочного комментария2
                        flag_pair = true;
                        StartComment('{');
                        NextSym();
                        break;

                    case '}':
                        if (flag_pair)
                            symbol = frpar;
                        else
                            InputOutput.Error(15, InputOutput.positionNow);
                        flag_pair = false;
                        InputOutput.NextCh();
                        break;

                    default: //обычная контанта, не требующая особой обработки
                        name = InputOutput.Ch.ToString();

                        //если она есть в талице ключевых значений
                        if (Keywords.kw[1].TryGetValue(name, out value))
                        {
                            InputOutput.NextCh();
                            symbol = value;

                        }
                        else
                        {
                            InputOutput.Error(6, InputOutput.positionNow);
                            InputOutput.NextCh();
                            symbol = 0;
                        }
                        break;

                }
            }
            else
                File.Close();


            // запись символьного кода в файл
            if (symbol != 0 && InputOutput.flag)
                File.Write(symbol + " ");

            if (InputOutput.positionNow.charNumber == 0 && InputOutput.flag)
                File.Write('\n');


            return symbol;
        }


        static public void StartComment(char a)
        {
            TextPosition start_position = InputOutput.positionNow;
            if (a == '{') //если многострочный комментарий 1 типа
            {
                while (InputOutput.Ch != '}') //пока не встретился символ закрытия
                {
                    InputOutput.NextCh(); //читаем следующий символ
                    if (InputOutput.flag == false) //если файл кончился
                    {
                        InputOutput.Error(10, start_position); //ошибка о незакрытом комментарии
                        symbol = 0;

                        return; //выход из функции StartComment
                    }

                }

                InputOutput.NextCh();
                symbol = 0;
                return; //выход из функции StartComment
            }
            if (a == '*') //если многострочный комментарий 2 типа
            {
                while(InputOutput.flag != false)
                {
                    InputOutput.NextCh();
                    if (InputOutput.Ch == '*')
                    {
                        InputOutput.NextCh();
                        if (InputOutput.Ch == ')') //если подряд встретился символы '*)'
                        {
                            InputOutput.NextCh();
                            symbol = 0;
                            return; //выход из функции StartComment
                        }
                    }
                }

                if (InputOutput.flag)
                {
                    InputOutput.Error(10, start_position); //ошибка о незакрытом комментарии
                    symbol = 0;
                    return; //выход из функции StartComment

                }

            }
            return; //выход из функции StartComment
        }

    }
}
