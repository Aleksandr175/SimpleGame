using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace Search_minimum_way
{
    public partial class Form1 : Form
    {
        struct Cell
        {
            Rectangle location;
	        public Rectangle Location
	        {
		        get { return location; }
		        set { location = value; }
	        }
            int obj; // стена, проход, старт , финиш
            public int Obj
            {
                get { return obj; }
                set { obj = value; }
            }
        };
        Graphics Draw_Srfce; //создаём графическую поверхность поля
        int Num_Cells_V ; // количество клеток по вертикали
        int Num_Cells_H ; // количество клеток по горизонатли
        Cell[,] Field_Cells;
        int Height_Cell = 10;
        Point Control_Point = new Point(35,35);
        
        int u = 0;
        // загрузка картинок 
        Image im_em_cel = Image.FromFile(@"Icons\Empty_Cells.PNG");
        Image im_wall = Image.FromFile(@"Icons\Wall.PNG");
        Image im_door_open_V = Image.FromFile(@"Icons\Door_open_V.PNG");
        Image im_door_closed_V = Image.FromFile(@"Icons\Door_closed_V.PNG");
        Image im_door_open_H = Image.FromFile(@"Icons\Door_open_H.PNG");
        Image im_door_closed_H = Image.FromFile(@"Icons\Door_closed_H.PNG");
        Image im_guard = Image.FromFile(@"Icons\Guard.PNG");
        /////////////////

        bool f_start = false; // установлен ли старт
        bool f_finish = false;// установлен ли финиш
        bool f_pove_way = false; // проложен ли путь
        List <List<int>> ArrWay; // массив пути
        
        
        public Form1()
        {
            InitializeComponent();

            for (int i = 7; i < 51; i++)
            {
                cb_n.Items.Add(i);
                cb_m.Items.Add(i);
            }
            cb_n.SelectedIndex = 0;
            cb_m.SelectedIndex = 0;
            
        }

        private void Create_Field()
        {
            Num_Cells_H = Convert.ToInt32(cb_n.SelectedItem);
            Num_Cells_V = Convert.ToInt32(cb_m.SelectedItem);
            Field_Cells = new Cell[Num_Cells_H ,Num_Cells_V];
            if (Num_Cells_V > Num_Cells_H)
                Height_Cell = 400 / Num_Cells_V;
            else Height_Cell = 400 / Num_Cells_H;

            Size s_Cell = new Size(Height_Cell, Height_Cell);
            Point Cell_Point = new Point(Control_Point.X, Control_Point.Y);
            //Draw_Srfce = this.CreateGraphics();
            for (int i = 0; i < Field_Cells.GetLength(0); i++ )
                for (int j = 0; j < Field_Cells.GetLength(1); j++ )
                {
                    Cell_Point.X = i*Height_Cell + Control_Point.X;
                    Cell_Point.Y = j*Height_Cell + Control_Point.Y;                    
                    Field_Cells[i, j].Location = new Rectangle(Cell_Point, s_Cell);
                    Field_Cells[i, j].Obj = 0;
                }
            Invalidate();
            //
              
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {


        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            u++;
            //Graphics g = e.Graphics;
            Draw_Srfce = e.Graphics;
                Print_Field( e);
        }
        private void Print_Field( PaintEventArgs e)
        {
            Image tmp = im_em_cel;
            Graphics g = e.Graphics;
            for (int i = 0; i < Field_Cells.GetLength(0); i++)
                for (int j = 0; j < Field_Cells.GetLength(1); j++)
                {
                    switch (Field_Cells[i, j].Obj)
                    {
                        case 0: // пустая
                            tmp = im_em_cel;
                            break;
                        case 1: // стена
                            tmp = im_wall;
                            break;
                        case 20: // дверь горизонтальная открытая 
                            tmp = im_door_open_H;
                            break;
                        case 21: // дверь вертикальная открытая
                            tmp = im_door_open_V;
                            break;
                        case 22: // дверь горизонтальная закрытая 
                            tmp = im_door_closed_H;
                            break;
                        case 23: // дверь вертикальная закрытая
                            tmp = im_door_closed_V;
                            break;
                        case 30: // охранник
                            tmp = im_guard;
                            break;

                    }
                    
                    g.DrawImage(tmp, Field_Cells[i, j].Location);
                }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            B_Wall.Enabled = false;
            //cb_objs.Enabled = true;
            B_Delete.Enabled = true;
        }

        private void B_Delete_Click(object sender, EventArgs e)
        {
            B_Wall.Enabled = true;
            B_Delete.Enabled = false;
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (e.Location.X - Control_Point.X) / Height_Cell;
            int y = (e.Location.Y - Control_Point.Y) / Height_Cell;
            if (e.Button == MouseButtons.Right)
            {
                B_Wall.Enabled = true;
                B_Delete.Enabled = false;
                Field_Cells[x, y].Obj = 0;
                Invalidate();
                return;
            }
            
            label1.Text = Convert.ToString(x) + " " + Convert.ToString(y);
            if ((x >= 0 && x < Num_Cells_H) && (y >= 0 && y < Num_Cells_V))
            {

                if (B_Wall.Enabled == false)
                {
                    
                    Field_Cells[x, y].Obj = 1;
                }
                
                else if (B_Delete.Enabled == false)
                {
                    Field_Cells[x, y].Obj = 0;                   
                }
                else
                {
                    /*
                    Дверь открытая вертикальная
                    Дверь открытая горизонатальная
                    Дверь закрытая вертикальная
                    Дверь закрытая горизонатальная*/
                    
                    switch (Convert.ToString(cb_objs.SelectedItem))
                    {
                        case "Дверь открытая вертикальная":
                            
                            Field_Cells[x, y].Obj = 21;
                            break;
                            
                        case "Дверь открытая горизонатальная":
                            Field_Cells[x, y].Obj = 20;
                            break;

                        case "Дверь закрытая вертикальная":
                            Field_Cells[x, y].Obj = 23;
                            break;

                        case "Дверь закрытая горизонатальная":
                            Field_Cells[x, y].Obj = 22;
                            break;

                        case "Охранник":
                            Field_Cells[x, y].Obj = 30;
                            break;
                           
                    }
                }
                Invalidate();

            }
        }

        

      

        private void cm_n_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cb_m.SelectedItem) != 0)
                Create_Field();
           // MessageBox.Show("НSSS");
        }

        private void cb_m_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Convert.ToInt32(cb_n.SelectedItem) != 0)
                Create_Field();
        }
        private void cb_objs_SelectedIndexChanged(object sender, EventArgs e)
        {
            B_Delete.Enabled = true;
            B_Wall.Enabled = true;

        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog  saveFile = new SaveFileDialog();
			saveFile.Filter = "Text file (*.txt)|*.txt";
			saveFile.Title = "Сохранить проект как ...";

            if(saveFile.ShowDialog() == System.Windows.Forms.DialogResult.OK && saveFile.FileName.Length > 0 )
            {
                StreamWriter file = new StreamWriter(new FileStream(saveFile.FileName, FileMode.Create,FileAccess.Write));
                
                for (int i = 0; i < Field_Cells.GetLength(1); i++)
                {
                    for (int j = 0; j < Field_Cells.GetLength(0); j++)
                    {
                        file.Write(Field_Cells[j,i].Obj + " ");

                    }
                    file.WriteLine("");
                }
                file.Close();

            }
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog  openFile = new OpenFileDialog();
			openFile.Filter = "Text file (*.txt)|*.txt";
			openFile.Title = "Открыть ...";

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK && openFile.FileName.Length > 0)
            {

            }
        }
    }

        
    }

