using System;
using System.Collections.Generic;


namespace Compiler
{
    /*Описание переменных простых типов. Описание массивов. Операторы:
	присваивания, составной (использовать простые и индексированные переменные).*/

    public struct Variable // переменная
    {
        public string name; // её имя
        public string type; // тип
        public int level; // область видимости
        public string value;

        public Variable(string name, string type, int level, string value)
        {
            this.name = name;
            this.type = type;
            this.level = level;
            this.value = value;
        }
    }

    public struct ArrayType
    {
        public string var_name; // ссылка на переменную
        public string el_type; // тип элементов 
        public int lower; // нижний предел
        public int upper; // верхний предел

        public ArrayType(string var_name, string el_type, int lower, int upper)
        {
            this.var_name = var_name;
            this.el_type = el_type;
            this.lower = lower;
            this.upper = upper;
        }

    }

    class Parser
    {
        //К простым относятся целые и вещественные типы, логический, символьный, перечислимый и диапазонный тип.
        public static string[] types = {"shortint", "integer", "longint", "byte", "word",
            "real", "double", "single", "extended", "boolean", "char"};

        public static string[] int_types = { "shortint", "integer", "longint", "byte", "word" };
        public static string[] real_types = { "real", "double", "single", "extended" };       

        public static List<Variable> variables = new List<Variable>();
        public static List<string> variables_names = new List<string>();
        public static List<ArrayType> arrays = new List<ArrayType>();
        public static List<string> arrays_names = new List<string>();


        private static int beg_end_count = 0;

        private static int symbol; // текущий символ

        public static bool have_error = false;
        public static bool var_flag = false;
        public static bool be_end_flag = false;


        public static TextPosition old_pos;


        public static void Parse()
        {
            have_error = true;
            symbol = LexicalAnalyzer.NextSym();
            have_error = false;


            switch (symbol)
            {
                case LexicalAnalyzer.varsy:
                    if (!be_end_flag)
                        VarPart();
                    else
                        VarDescription();
                    break;
                case LexicalAnalyzer.beginsy:
                    BeginPart();
                    break;
                case LexicalAnalyzer.endsy:
                    EndPart();
                    break;
                case LexicalAnalyzer.ident:
                    if (be_end_flag)
                        AssigmentPart();
                    break;
                case LexicalAnalyzer.writesy:
                case LexicalAnalyzer.writelnsy:
                case LexicalAnalyzer.readsy:
                case LexicalAnalyzer.readlnsy:
                    CodeGeneration.ChooseAction(symbol);
                    break;
            }

        }

        // пропускает символы до начала следующей строки
        private static void SkipStr()
        {
            while (InputOutput.positionNow.charNumber != 0)
            {                
                symbol = LexicalAnalyzer.NextSym(); // пропускаем символы до начала следующей строки 
            }

        }

        // проверка на соответствие ожидаемому символу
        private static bool Accept(int symbolexpected, TextPosition error_position)
        {
            if (symbol == symbolexpected) //если текущий символ совпал с ожидаемым
            {
                return true;
            }
            else
            {
                InputOutput.Error((byte)symbolexpected, error_position); // сформировать ошибку
                return false;
            }

        }

        // функция для обработки ошибок в конце строки
        private static void EndStrParse(byte code)
        {
            if (symbol != code)
            {
                if (!have_error) // если ошибка не была отловлена в модуле ввода вывода (т.е. конец строки ещё не наступил)
                {



                    if (!Accept(code, InputOutput.positionNow))
                        have_error = true; // отмечаем, что ошибка уже добавлена

                    SkipStr();

                    if (var_flag)
                        symbol = LexicalAnalyzer.NextSym();

                    var_flag = false;
                }
                else if (InputOutput.positionNow.charNumber == 0 && symbol == LexicalAnalyzer.ident)
                    symbol = LexicalAnalyzer.NextSym();
            }
            else
            {
                var_flag = false;

                if (InputOutput.flag && !be_end_flag)
                    symbol = LexicalAnalyzer.NextSym();

            }
        }








        /*///////////////////////////БЛОК АНАЛИЗА VAR///////////////////////////////*/

        // анализ конструкции var
        private static void VarPart()
        {
            do
            {
                var_flag = true;
                VarDescription();
            }
            while (symbol == LexicalAnalyzer.ident); // пока текущий символ является идентификатором			

            if (symbol == LexicalAnalyzer.beginsy)
                BeginPart();
        }

        // анализ строки с описанием однотипных переменных
        private static void VarDescription()
        {
            have_error = false;
            List<string> names = new List<string>(); // список имён однотипных переменных
            Variable new_var = new Variable(); // новая переменная

            do
            {
                if (symbol != LexicalAnalyzer.ident)
                    symbol = LexicalAnalyzer.NextSym(); //если не первое вхождение цикла, то пропуск символа ';'                
                Accept(LexicalAnalyzer.ident, InputOutput.positionNow); //проверяем, нет ли зарезервированных слов 

                if (variables_names.Contains(LexicalAnalyzer.ident_name))
                    InputOutput.Error(96, InputOutput.positionNow); // сформировать ошибку о повторяющейся переменной
                else
                {
                    names.Add(LexicalAnalyzer.ident_name); //добавляем имя переменной в список имёт текущей строки var
                }

                symbol = LexicalAnalyzer.NextSym();
            }
            while (symbol == LexicalAnalyzer.comma); // пока встречается запятая



            // двоеточие
            if (Accept(LexicalAnalyzer.colon, InputOutput.positionNow))
            {
                symbol = LexicalAnalyzer.NextSym();

                // не зарезервированное слово
                if (symbol == LexicalAnalyzer.ident)
                {                    
                    string type = LexicalAnalyzer.ident_name; //берём текстовое значение
                    if (Array.Exists(types, element => element == type)) //проверка присутствия типа в списке возможных
                    {
                        new_var.type = type; // создаём новую переменную
                        for (int i = 0; i < names.Count; i++)
                        {
                            new_var.name = names[i];
                            new_var.level = beg_end_count;
                            variables.Add(new_var); // добавление новой переменной в список
                            variables_names.Add(new_var.name); // добавить перменную в список
                        }
                    }
                    else
                    {
                        InputOutput.Error(94, InputOutput.positionNow); // сформировать ошибку о несуществующем типе
                    }

                    old_pos = InputOutput.positionNow;
                    symbol = LexicalAnalyzer.NextSym();
                }
                else if (symbol == LexicalAnalyzer.arraysy)
                {
                    ArrayDescription(names);
                }
                else
                {
                    InputOutput.Error(94, InputOutput.positionNow); // сформировать ошибку о несуществующем типе
                    symbol = LexicalAnalyzer.NextSym();
                }


                // обработка точки с запятой

                EndStrParse(LexicalAnalyzer.semicolon);
            }
            else
            {
                var_flag = false;
                while (InputOutput.positionNow.charNumber != 0)
                    symbol = LexicalAnalyzer.NextSym(); // пропускаем символы до начала следующей строки
                symbol = LexicalAnalyzer.NextSym();
            }
            var_flag = false;


        }        

        // анализ описания массивов
        private static void ArrayDescription(List<string> names)
		{
            Variable new_var = new Variable();
            ArrayType new_array = new ArrayType();

            symbol = LexicalAnalyzer.NextSym();
			if (Accept(LexicalAnalyzer.lbracket, InputOutput.positionNow)) // [
			{
                symbol = LexicalAnalyzer.NextSym();
                if (Accept(LexicalAnalyzer.intc, InputOutput.positionNow))                
                    new_array.lower = LexicalAnalyzer.int_value; // целое число            

                symbol = LexicalAnalyzer.NextSym();
                Accept(LexicalAnalyzer.twopoints, InputOutput.positionNow); // ..


                symbol = LexicalAnalyzer.NextSym();
                if (Accept(LexicalAnalyzer.intc, InputOutput.positionNow)) // целое число
                    new_array.upper = LexicalAnalyzer.int_value;


                symbol = LexicalAnalyzer.NextSym();
                if (Accept(LexicalAnalyzer.rbracket, InputOutput.positionNow))
                {
                    symbol = LexicalAnalyzer.NextSym();
                    Accept(LexicalAnalyzer.ofsy, InputOutput.positionNow); // of

                    symbol = LexicalAnalyzer.NextSym();
                    string type = LexicalAnalyzer.ident_name;
                    if (Array.Exists(types, element => element == type)) //проверка присутствия типа в списке возможных
                    {
                        new_array.el_type = type;

                        new_var.type = "array"; // создаём новую переменную
                        for (int i = 0; i < names.Count; i++)
                        {
                            new_var.name = names[i];
                            variables.Add(new_var); // добавление новой переменной в список
                            variables_names.Add(new_var.name); // добавить перменную в список

                            new_array.var_name = new_var.name;
                            arrays.Add(new_array);
                            arrays_names.Add(new_array.var_name);
                        }
                    }
                    else
                    {
                        InputOutput.Error(94, InputOutput.positionNow); // сформировать ошибку о несуществующем типе
                    }

                    symbol = LexicalAnalyzer.NextSym();
                }
                else
                    SkipStr();            
                
            }
            else
                SkipStr();
        }

        

        















        /*///////////////////////////БЛОК АНАЛИЗА BEGIN - END///////////////////////////////*/

        private static void BeginPart()
        {
            be_end_flag = true;
            beg_end_count++;
            int numb_of_current_block = beg_end_count;
            while (numb_of_current_block == beg_end_count)
            {
                if (!InputOutput.flag)
                {
                    InputOutput.Error(98, InputOutput.positionNow); // сформировать ошибку о не закрытом блоке
                    break;
                }
                Parse();

            }

        }

        private static void EndPart()
        {
            if (beg_end_count == 0)
            {
                InputOutput.Error(97, InputOutput.positionNow); // сформировать ошибку о не открытом блоке
            }
            else
            {                             

                old_pos = InputOutput.positionNow;
                be_end_flag = false;
                symbol = LexicalAnalyzer.NextSym();

                if (beg_end_count == 1) // конец последнего блока
                {
                    InputOutput.flag = false;
                    EndStrParse(LexicalAnalyzer.point); // .                    
                }
                else
                {
                    be_end_flag = true;
                    EndStrParse(LexicalAnalyzer.semicolon); // ;
                }

                beg_end_count--;

            }

        }





















        /*///////////////////////////БЛОК АНАЛИЗА :=///////////////////////////////*/



        private static void AssigmentPart()
        {
            have_error = false;
            if (variables_names.Contains(LexicalAnalyzer.ident_name)) 
            {
                int index = variables_names.IndexOf(LexicalAnalyzer.ident_name);
                if (variables[index].level > beg_end_count)
                    InputOutput.Error(82, InputOutput.positionNow); // неверный уровень

                symbol = LexicalAnalyzer.NextSym();

                string type1 = variables[index].type; // тип переменной, в которую заносится значение

                if (symbol == LexicalAnalyzer.assign) // простая переменная
                {
                    Variable new_var = variables[index];

                    symbol = LexicalAnalyzer.NextSym();
                    if (!CheckTypes(type1))
                        InputOutput.Error(67, old_pos); // несоответствие типов
                    else
                    {
                        old_pos = InputOutput.positionNow;
                        symbol = LexicalAnalyzer.NextSym();
                        if (symbol != LexicalAnalyzer.semicolon)
                        {
                            string cur_val = CodeGeneration.new_value;
                            CodeGeneration.ChooseAction(symbol);
                            if (CodeGeneration.new_value != cur_val)
                                new_var.value = CodeGeneration.new_value;
                        }
                        else
                        {
                            int new_index = variables_names.IndexOf(LexicalAnalyzer.ident_name);
                            if (variables[new_index].value != null)
                                new_var.value = variables[new_index].value;
                            else
                                InputOutput.Error(72, old_pos); // нет значения
                        }
                    }                    

                    variables[index] = new_var;
                    
                    SkipStr();
                }

                else
                {
                    if (variables[index].type == "array")
                    {
                        string name = variables[index].name;
                        index = arrays_names.IndexOf(name);
                        type1 = arrays[index].el_type;
                        if (Accept(LexicalAnalyzer.lbracket, InputOutput.positionNow)) // индексированная переменная
                        {
                            symbol = LexicalAnalyzer.NextSym();
                            if (symbol != LexicalAnalyzer.intc)
                                if (symbol != LexicalAnalyzer.ident)
                                    InputOutput.Error(107, InputOutput.positionNow); // неверный индекс массива
                                else
                                {
                                    if (!variables_names.Contains(LexicalAnalyzer.ident_name))
                                        InputOutput.Error(102, InputOutput.positionNow); // не найдена переменная
                                    else
                                    {
                                        int index_i = variables_names.IndexOf(LexicalAnalyzer.ident_name);
                                        if (variables[index_i].value == null)
                                        {
                                            InputOutput.Error(72, InputOutput.positionNow); // нет значения
                                        }
                                        else
                                        {
                                            if (int.Parse(variables[index_i].value) < arrays[index].lower || int.Parse(variables[index_i].value) > arrays[index].upper)
                                                InputOutput.Error(55, InputOutput.positionNow); // выход за пределы                                                             

                                            if (variables[index_i].level > beg_end_count)
                                                InputOutput.Error(82, InputOutput.positionNow); // неверный уровень

                                            if (variables[index_i].type != "integer")
                                                InputOutput.Error(107, InputOutput.positionNow); // неверный индекс массива
                                        }                                                                              
                                    }
                                }
                            else
                            {
                                index = arrays_names.IndexOf(name);

                                if (LexicalAnalyzer.int_value < arrays[index].lower || LexicalAnalyzer.int_value > arrays[index].upper)
                                    InputOutput.Error(55, InputOutput.positionNow); // выход за пределы                                                             

                            }

                            symbol = LexicalAnalyzer.NextSym();
                            if (Accept(LexicalAnalyzer.rbracket, InputOutput.positionNow))
                            {
                                symbol = LexicalAnalyzer.NextSym();
                                Accept(LexicalAnalyzer.assign, InputOutput.positionNow);

                                symbol = LexicalAnalyzer.NextSym();
                                if (!CheckTypes(type1))
                                    InputOutput.Error(67, old_pos); // несоответствие типов
                                SkipStr();
                            }
                            else
                            {                                
                                SkipStr();
                            }
                        }
                        else
                        {
                            SkipStr();
                        }
                    }
                    else
                    {
                        Accept(LexicalAnalyzer.assign, InputOutput.positionNow);
                        SkipStr();
                    }
                }                
            }                    
            else
            {                
                InputOutput.Error(102, InputOutput.positionNow); // не найдена переменная
                SkipStr();
            }

        }

        public static bool CheckTypes(string need_type)
        {
            old_pos = InputOutput.positionNow;

            switch (symbol)
            {
                case LexicalAnalyzer.ident:
                    int index = variables_names.IndexOf(LexicalAnalyzer.ident_name);
                    if (index != -1)
                    {
                        if (variables[index].level > beg_end_count)
                            InputOutput.Error(82, InputOutput.positionNow); // неверный уровень
                        
                        string var_type = variables[index].type;
                        if (Array.Exists(int_types, element => element == var_type))
                            var_type = "int";
                        if (Array.Exists(real_types, element => element == var_type))
                            var_type = "real";

                        switch (var_type)
                        {
                            case "int":
                                return Array.Exists(int_types, element => element == need_type);

                            case "real":
                                return Array.Exists(real_types, element => element == need_type);

                            case "char":
                                symbol = LexicalAnalyzer.NextSym();
                                symbol = LexicalAnalyzer.NextSym();
                                return Accept(LexicalAnalyzer.quote, InputOutput.positionNow);

                            case "boolean":
                                return (need_type == "boolean");
                            case "array":
                                if (need_type == "array")
                                {
                                    symbol = LexicalAnalyzer.NextSym();
                                    return (symbol == LexicalAnalyzer.semicolon);
                                }
                                else
                                    return false;
                            default:
                                return false;
                        }                        
                    }
                    else
                    {
                        InputOutput.Error(102, old_pos); // не найдена переменная
                        return false;
                    }


                case LexicalAnalyzer.intc:
                    return Array.Exists(int_types, element => element == need_type);

                case LexicalAnalyzer.floatc:
                    return Array.Exists(real_types, element => element == need_type);

                case LexicalAnalyzer.quote:
                    if (need_type == "char")
                    {
                        symbol = LexicalAnalyzer.NextSym();
                        symbol = LexicalAnalyzer.NextSym();
                        Accept(LexicalAnalyzer.quote, old_pos);
                        return true;
                    }
                    return false;
                    

                case LexicalAnalyzer.falsesy:
                case LexicalAnalyzer.truesy:
                    return (need_type == "boolean");                                   

                default:
                    return false;
            }
        }
    }
}



