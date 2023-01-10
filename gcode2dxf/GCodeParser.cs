using System;
using System.Collections.Generic;
using System.Text;

namespace gcode2dxf
{
    public class GCode
    {
        public struct Str
        {
            public string clearStr; // чистая строка 
            public Dictionary<string, double> atoms; // параметры которые удалось отпарсить

            /* //является элемент сномером поз последним
             public bool IsEnd(int pos)
             {
                 if ((atoms.Count-1)== pos) return true;
                 return false;
             }
             */
            public bool IsSetAtom(string name)
            {
                if (atoms.ContainsKey(name)) return true;
                return false;
            }

            public bool IsSetAtom(string name, double val)
            {
                if (atoms.ContainsKey(name) && atoms[name] == val) return true;
                return false;
            }

            public double GetValAtom(string name)
            {
                if (atoms.ContainsKey(name)) return atoms[name];
                return -9999999999999;
            }
        }

        public static Str ParseNewLine(string lineToParse)
        {
            List<string> parLst = new List<string>(); // перечисляем все что парсим
            parLst.Add("G");
            parLst.Add("X");
            parLst.Add("Y");
            parLst.Add("Z");
            parLst.Add("M");
            parLst.Add("E");
            parLst.Add("A");
            parLst.Add("B");
            parLst.Add("V");
            parLst.Add("S");
            parLst.Add("R");
            parLst.Add("I");
            parLst.Add("J");
            parLst.Add("K");
            parLst.Add("F");
            Str gcStr;
            gcStr.clearStr = lineToParse; // чистая строка
            gcStr.atoms = new Dictionary<string, double>(); // все параметры строки будут тут

            lineToParse.ToUpper(); // меняем регистр на заглавный
            lineToParse = lineToParse.Replace('.', ','); //точки в запятые для приведения типов
            if (lineToParse.IndexOf(";") != -1) { lineToParse = lineToParse.Remove(lineToParse.IndexOf(";")); }
            if (lineToParse.IndexOf("/") != -1) { lineToParse = lineToParse.Remove(lineToParse.IndexOf("/")); }

            int startPos;
            string codeNum;
            //M G F X Y Z E A B C D E0
            // Ищем каждый параметр
            foreach (string currentAtom in parLst)
            {
                startPos = 0;
                if (lineToParse.IndexOf(currentAtom, startPos) != -1) // такой символ есть в строке и перед ним не стоит / тогда парсим 
                {
                    // Заносим в буферную структуру имя параметра
                    gcStr.atoms[currentAtom] = -1; //  Значение по умолчанию выбираем
                    // вычленяем номер кода
                    double codeNumer = 1; // тут будет записано значение

                    if (lineToParse[0] == '(') { continue; } // отбрасываем

                    startPos = lineToParse.IndexOf(currentAtom, startPos) + currentAtom.Length; // начинаем с первого символа следующего за параметром
                    // ищем число. если попадается знак запоминаем его. если чтото другое то игнорируем.
                    while (!char.IsDigit(lineToParse[startPos]) && lineToParse.Length < startPos)
                    {
                        if (lineToParse[startPos] == '-') { codeNumer = -1; } // отрицательное
                        // чтото пошло совмем не так. Отбрасыввем строку. 
                        if (lineToParse[startPos] == '=' || lineToParse[startPos] == '(' || lineToParse[startPos] == ')' || lineToParse[startPos] == '/' || startPos > 5 + lineToParse.IndexOf(currentAtom, startPos) + currentAtom.Length)
                        {
                            continue;
                        }
                        startPos++;
                    }

                    int stopPos = startPos + 1;
                    while (lineToParse.Length - 1 > stopPos && (lineToParse[stopPos] == ',' || char.IsDigit(lineToParse[stopPos])))
                    {
                        stopPos++;
                    }


                    if (lineToParse.Length - 1 >= stopPos)
                    {
                        codeNum = lineToParse.Substring(startPos, stopPos - startPos + 1);
                    }
                    else
                    {
                        codeNum = "-1";
                    }
                    try
                    {
                        codeNumer *= Convert.ToDouble(codeNum);//число комманды, знак был считан ранее, умножаем на него    
                    }
                    catch
                    {
                        // listBox1.Items.Add("code num err" + codenum);
                        // MessageBox.Show("code num err" + codenum, "Выход из программы !",  MessageBoxButtons.OK);
                        continue;
                    }
                    gcStr.atoms[currentAtom] = codeNumer;
                }
            }
            return gcStr;
        }
    }
    public class Gcode3d
    {
        public static int getintdatafromkom(string s)
        {
            string[] sa = s.Split(':');
            string sb = sa[sa.Length - 1];
            return int.Parse(sb.Trim());
        }
        public static double getdoubledatafromkom(string s)
        {
            string[] sa = s.Split(':');
            return double.Parse(sa[sa.Length - 1].Trim().Replace('.', ','));
        }
        public struct Layer3d
        {
            public bool is_load; //номер
            public long file_pos; //позиция начала ччтения
            public string time; // время
            public List<GCode.Str> cmd;
        }
        public struct File3d
        {
            public string filename; // чистая строка 
            public int time; // чистая строка 
            public double filament; //
            public double h_layer; //  
            public dPoint MinPos;
            public dPoint MaxPos;
            public int layers_cnt;
            public double tbed;
            public List<double> tex;
            public GCode.Str houming;
            public List<GCode.Str> head;
            public Gcode3d.Layer3d[] layer;
            public List<GCode.Str> tail;
        }
    }

    public struct dPoint // вспомогательный для рисунка
    {
        public double X;
        public double Y;
        public double Z;
        public dPoint(double x, double y)
        {
            this.X = x;
            this.Y = y;
            this.Z = 0;
        }
        public dPoint(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
}
