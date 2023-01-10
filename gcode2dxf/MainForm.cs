using netDxf;
using netDxf.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gcode2dxf
{
    public partial class MainForm : Form
    {
        Gcode3d.File3d file3d = new Gcode3d.File3d(); // команды
        public MainForm()
        {
            InitializeComponent();
        }

        private void ConvertClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                LoadFile(openFileDialog.FileName);
            } // нет файла
        }

        private void LoadFile(string fileName)
        {
            file3d = new Gcode3d.File3d();  // очищаем буфер комманд


            StreamReader openedFile = new StreamReader(fileName);  // Поток чтения файла
            file3d.filename = fileName;
            // получаем начало(до 2 слоя) и конец(после последнего слоя) файла
            string line_to_parse = "";
            int layers_cnt = 0;
            //headf

            file3d.head = new List<GCode.Str>();

            double minx = 0, miny = 0, minz = 0;
            while (!openedFile.EndOfStream && !line_to_parse.Contains(";LAYER:"))
            {
                line_to_parse = openedFile.ReadLine();
                GCode.Str atr = GCode.ParseNewLine(line_to_parse);
                file3d.head.Add(GCode.ParseNewLine(line_to_parse));
                //обработчик хэда
                if (line_to_parse.Contains(";TIME:")) { file3d.time = Gcode3d.getintdatafromkom(line_to_parse); }
                if (line_to_parse.Contains(";Filament used:")) { file3d.filament = Gcode3d.getdoubledatafromkom(line_to_parse.Replace('m', ' ')); }
                if (line_to_parse.Contains(";Layer height:")) { file3d.h_layer = Gcode3d.getdoubledatafromkom(line_to_parse); }


                if (line_to_parse.Contains(";MINX:")) { minx = Gcode3d.getdoubledatafromkom(line_to_parse); }
                if (line_to_parse.Contains(";MINY:")) { miny = Gcode3d.getdoubledatafromkom(line_to_parse); }
                if (line_to_parse.Contains(";MINZ:")) { minz = Gcode3d.getdoubledatafromkom(line_to_parse); file3d.MinPos = new dPoint(minx, miny, minz); }


                if (line_to_parse.Contains(";MAXX:")) { minx = Gcode3d.getdoubledatafromkom(line_to_parse); }
                if (line_to_parse.Contains(";MAXY:")) { miny = Gcode3d.getdoubledatafromkom(line_to_parse); }
                if (line_to_parse.Contains(";MAXZ:")) { minz = Gcode3d.getdoubledatafromkom(line_to_parse); file3d.MaxPos = new dPoint(minx, miny, minz); }

                if (atr.IsSetAtom("M", 140) || atr.IsSetAtom("M", 190)) { file3d.tbed = atr.GetValAtom("S"); }
                if (atr.IsSetAtom("M", 104) || atr.IsSetAtom("M", 109))
                {
                    if (file3d.tex == null) { file3d.tex = new List<double>(1); file3d.tex.Add(0); }
                    file3d.tex[0] = (atr.GetValAtom("S"));
                }
                if (atr.IsSetAtom("G", 28)) { file3d.houming = atr; }
                // задаем размерностьмассива слоев
                if (line_to_parse.Contains(";LAYER_COUNT:"))
                {
                    file3d.layers_cnt = Gcode3d.getintdatafromkom(line_to_parse);
                    layers_cnt = file3d.layers_cnt; file3d.layer = new Gcode3d.Layer3d[layers_cnt];
                    int i = 0;// инициализируем значения по умолчанию
                    for (i = 0; i < layers_cnt; i++)
                    {
                        file3d.layer[i].cmd = new List<GCode.Str>();
                        file3d.layer[i].is_load = false;
                        file3d.layer[i].file_pos = 0;
                    }
                }

            }
            int curlayer = 0;
            while (!openedFile.EndOfStream && line_to_parse != ";LAYER:" + (layers_cnt - 11))
            {
                if (file3d.layer[curlayer].is_load == false)
                {
                    file3d.layer[curlayer].file_pos = openedFile.BaseStream.Position;
                    file3d.layer[curlayer].is_load = true;
                }
                if (line_to_parse.Contains(";LAYER")) { curlayer = Gcode3d.getintdatafromkom(line_to_parse); }
                file3d.layer[curlayer].cmd.Add(GCode.ParseNewLine(line_to_parse));
                line_to_parse = openedFile.ReadLine();
            }

            while (!openedFile.EndOfStream && line_to_parse != ";LAYER:" + (layers_cnt - 10))
            {
                line_to_parse = openedFile.ReadLine();
                if (line_to_parse.Contains(";LAYER"))
                {
                    curlayer = Gcode3d.getintdatafromkom(line_to_parse);
                    if (file3d.layer[curlayer].is_load == false)
                    {
                        file3d.layer[curlayer].file_pos = openedFile.BaseStream.Position;
                        file3d.layer[curlayer].is_load = true;
                    }
                }
            }



            while (!openedFile.EndOfStream && line_to_parse != ";End of Gcode")
            {
                GCode.Str atr = GCode.ParseNewLine(line_to_parse);
                file3d.layer[curlayer].cmd.Add(atr);
                line_to_parse = openedFile.ReadLine();
                if (line_to_parse.Contains(";LAYER"))
                {
                    curlayer = Gcode3d.getintdatafromkom(line_to_parse);
                    if (file3d.layer[curlayer].is_load == false)
                    {
                        file3d.layer[curlayer].file_pos = openedFile.BaseStream.Position;
                        file3d.layer[curlayer].is_load = true;
                    }
                }
            }

            file3d.tail = new List<GCode.Str>();
            while (!openedFile.EndOfStream)
            {
                line_to_parse = openedFile.ReadLine();
                GCode.Str atr = GCode.ParseNewLine(line_to_parse);
                file3d.tail.Add(atr);
            }
            openedFile.Close();
            DxfDocument doc;
            Line entity;
            double X1 = 0;
            double Y1 = 0;
            double X2 = 0;
            double Y2 = 0;
            for (int i = 0; i < file3d.layers_cnt; i++)
            {

                doc = new DxfDocument();
                //int counter = 0;
                for (int j = 0; j < file3d.layer[i].cmd.Count;j++)
                {
                    if (file3d.layer[i].cmd[j].IsSetAtom("G") && (file3d.layer[i].cmd[j].IsSetAtom("X") || file3d.layer[i].cmd[j].IsSetAtom("Y")))
                    {
                        if (file3d.layer[i].cmd[j].IsSetAtom("X"))
                        {
                            X2 = file3d.layer[i].cmd[j].GetValAtom("X");
                        }
                        if (file3d.layer[i].cmd[j].IsSetAtom("Y"))
                        {
                            Y2 = file3d.layer[i].cmd[j].GetValAtom("Y");
                        }
                        if (file3d.layer[i].cmd[j].IsSetAtom("E"))
                        {
                            if (X1 != X2 || Y1 != Y2)
                            {
                                entity = new Line(new Vector2(X1, Y1), new Vector2(X2, Y2));
                                // add your entities here
                                doc.Entities.Add(entity);
                            }
                        }
                        X1 = X2;
                        Y1 = Y2;
                        //counter++;
                    }
                }
                string path = Path.GetDirectoryName(fileName)+"\\"+Path.GetFileNameWithoutExtension(fileName) + "_layer" + i.ToString() + ".dxf";
                doc.Save(path);
            }
        }
    }
}
