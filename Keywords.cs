using System.Collections.Generic;

namespace Compiler
{
    class Keywords
    {
        public static Dictionary<int, Dictionary<string, byte>> kw = new Dictionary<int, Dictionary<string, byte>>();

        static Keywords()
        {
            Dictionary<string, byte> tmp = new Dictionary<string, byte>();
            //в kw[1] будут лежать коды символьных констант
            tmp["*"] = LexicalAnalyzer.star;
            tmp["/"] = LexicalAnalyzer.slash;
            tmp["="] = LexicalAnalyzer.equal;
            tmp[","] = LexicalAnalyzer.comma;
            tmp[";"] = LexicalAnalyzer.semicolon;
            tmp[": "] = LexicalAnalyzer.colon;
            tmp["."] = LexicalAnalyzer.point;
            tmp["^"] = LexicalAnalyzer.arrow;
            tmp["("] = LexicalAnalyzer.leftpar;
            tmp[")"] = LexicalAnalyzer.rightpar;
            tmp["["] = LexicalAnalyzer.lbracket;
            tmp["]"] = LexicalAnalyzer.rbracket;
            tmp["{"] = LexicalAnalyzer.flpar;
            tmp["}"] = LexicalAnalyzer.frpar;
            tmp["<"] = LexicalAnalyzer.later;
            tmp[">"] = LexicalAnalyzer.greater;
            tmp["<="] = LexicalAnalyzer.laterequal;
            tmp["=>"] = LexicalAnalyzer.greaterequal;
            tmp["<>"] = LexicalAnalyzer.latergreater;
            tmp["+"] = LexicalAnalyzer.plus;
            tmp["-"] = LexicalAnalyzer.minus;        
            tmp["(*"] = LexicalAnalyzer.lcomment;
            tmp["*)"] = LexicalAnalyzer.lcomment;
            tmp[":="] = LexicalAnalyzer.assign;
            tmp[".."] = LexicalAnalyzer.twopoints;
            tmp["'"] = LexicalAnalyzer.quote;
            kw[1] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["do"] = LexicalAnalyzer.dosy;
            tmp["if"] = LexicalAnalyzer.ifsy;
            tmp["in"] = LexicalAnalyzer.insy;
            tmp["of"] = LexicalAnalyzer.ofsy;
            tmp["or"] = LexicalAnalyzer.orsy;
            tmp["to"] = LexicalAnalyzer.tosy;
            kw[2] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["end"] = LexicalAnalyzer.endsy;
            tmp["var"] = LexicalAnalyzer.varsy;
            tmp["div"] = LexicalAnalyzer.divsy;
            tmp["and"] = LexicalAnalyzer.andsy;
            tmp["not"] = LexicalAnalyzer.notsy;
            tmp["for"] = LexicalAnalyzer.forsy;
            tmp["mod"] = LexicalAnalyzer.modsy;
            tmp["nil"] = LexicalAnalyzer.nilsy;
            tmp["set"] = LexicalAnalyzer.setsy;
            kw[3] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["then"] = LexicalAnalyzer.thensy;
            tmp["else"] = LexicalAnalyzer.elsesy;
            tmp["case"] = LexicalAnalyzer.casesy;
            tmp["file"] = LexicalAnalyzer.filesy;
            tmp["goto"] = LexicalAnalyzer.gotosy;
            tmp["type"] = LexicalAnalyzer.typesy;
            tmp["true"] = LexicalAnalyzer.truesy;
            tmp["with"] = LexicalAnalyzer.withsy;
            tmp["read"] = LexicalAnalyzer.readsy;

            kw[4] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["begin"] = LexicalAnalyzer.beginsy;
            tmp["while"] = LexicalAnalyzer.whilesy;
            tmp["array"] = LexicalAnalyzer.arraysy;
            tmp["const"] = LexicalAnalyzer.constsy;
            tmp["label"] = LexicalAnalyzer.labelsy;
            tmp["false"] = LexicalAnalyzer.falsesy;
            tmp["until"] = LexicalAnalyzer.untilsy;
            tmp["write"] = LexicalAnalyzer.writesy;
            kw[5] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["downto"] = LexicalAnalyzer.downtosy;
            tmp["packed"] = LexicalAnalyzer.packedsy;
            tmp["record"] = LexicalAnalyzer.recordsy;
            tmp["repeat"] = LexicalAnalyzer.repeatsy;
            tmp["readln"] = LexicalAnalyzer.readlnsy;
            kw[6] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["program"] = LexicalAnalyzer.programsy;
            tmp["writeln"] = LexicalAnalyzer.writelnsy;
            kw[7] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["function"] = LexicalAnalyzer.functionsy;
            kw[8] = tmp;

            tmp = new Dictionary<string, byte>();
            tmp["procedure"] = LexicalAnalyzer.proceduresy;
            kw[9] = tmp;
        }

    }
}