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
        Control cc = new Control("", 35, 35, 100, 100);
        
        int u = 0;
        // загрузка картинок 
        Image im_em_cel = Image.FromFile(@"Icons\Empty_Cells.PNG");
        Image im_wall = Image.FromFile(@"Icons\Wall.PNG");
        Image im_door_open_V = Image.FromFile(@"Icons\Door_open_V.PNG");
        Image im_door_closed_V = Image.FromFile(@"Icons\Door_closed_V.PNG");
        Image im_door_open_H = Image.FromFile(@"Icons\Door_open_H.PNG");
        Image im_door_closed_H = Image.FromFile(@"Icons\Door_closed_H.PNG");
        Image im_guard = Image.FromFile(@"Icons\Guard.PNG");
        Image im_way_guard = Image.FromFile(@"Icons\Way_Guard.PNG");
        Image brilliant = Image.FromFile(@"Icons\brilliant.png");
        Image card = Image.FromFile(@"Icons\card.png");
        Image chairs_D = Image.FromFile(@"Icons\chairs_D.png");
        Image chairs_L = Image.FromFile(@"Icons\chairs_L.png");
        Image chairs_R = Image.FromFile(@"Icons\chairs_R.png");
        Image chairs_U = Image.FromFile(@"Icons\chairs_U.png");
        Image key = Image.FromFile(@"Icons\key.png");
        Image laser2_horiz = Image.FromFile(@"Icons\laser2_horiz.png");
        Image laser2_vert = Image.FromFile(@"Icons\laser2_vert.png");
        Image money = Image.FromFile(@"Icons\money.png");
        Image picture1 = Image.FromFile(@"Icons\picture1.png");
        Image picture2 = Image.FromFile(@"Icons\picture2.png");
        Image picture3 = Image.FromFile(@"Icons\picture3.png");
        Image Player = Image.FromFile(@"Icons\Player.PNG");
        Image rubin = Image.FromFile(@"Icons\rubin.png");
        Image sofa_D = Image.FromFile(@"Icons\sofa_D.png");
        Image sofa_L = Image.FromFile(@"Icons\sofa_L.png");
        Image sofa_R = Image.FromFile(@"Icons\sofa_R.png");
        Image sofa_U = Image.FromFile(@"Icons\sofa_U.png");
        Image spDL = Image.FromFile(@"Icons\spDL.png");
        Image spLU = Image.FromFile(@"Icons\spLU.png");
        Image spRD = Image.FromFile(@"Icons\spRD.png");
        Image spUR = Image.FromFile(@"Icons\spUR.png");
        Image tableD = Image.FromFile(@"Icons\tableD.png");
        Image tableL = Image.FromFile(@"Icons\tableL.png");
        Image tableR = Image.FromFile(@"Icons\tableR.png");
        Image tableU = Image.FromFile(@"Icons\tableU.png");
        Image player = Image.FromFile(@"Icons\Player.PNG");
        //Image im_way_guard = Image.FromFile(@"Icons\");
        /////////////////

        List <List<int>> WayGuards; // массив пути охранников
        bool f_way_g; // установлен ли охранник (необходимо установить путь)
        int num_guard; // номер охранника
        int num_step_guards; // номер шага охранника



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
            num_step_guards = 1;
            num_guard = 0; 
            
        }

        private void Create_Field()
        {
            
            Num_Cells_H = Convert.ToInt32(cb_n.SelectedItem);
            Num_Cells_V = Convert.ToInt32(cb_m.SelectedItem);
            Field_Cells = new Cell[Num_Cells_H ,Num_Cells_V];
            WayGuards = new List<List<int>>();
            f_way_g = true;
            if (Num_Cells_V > Num_Cells_H)
            {
                Height_Cell = 400 / Num_Cells_V;
            }
            else
            {
                Height_Cell = 400 / Num_Cells_H;
            }

            Size s_Cell = new Size(Height_Cell, Height_Cell);
            Point Cell_Point = new Point(Control_Point.X, Control_Point.Y);
            
            //dataGridView1.
            for (int i = 0; i < Field_Cells.GetLength(0); i++ )
                for (int j = 0; j < Field_Cells.GetLength(1); j++ )
                {
                    Cell_Point.X = i*Height_Cell + Control_Point.X;
                    Cell_Point.Y = j*Height_Cell + Control_Point.Y;                    
                    Field_Cells[i, j].Location = new Rectangle(Cell_Point, s_Cell);
                    Field_Cells[i, j].Obj = 0;
                }
            Invalidate();
            
              
        }

        
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            u++;
            Draw_Srfce = e.Graphics;
            Print_Field( e);
            Print_table();
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
                        case 32: // охранник
                            tmp = chairs_U;
                            break;
                        case 33: // охранник
                            tmp = chairs_R;
                            break;
                        case 34: // охранник
                            tmp = chairs_D;
                            break;
                        case 35: // охранник
                            tmp = chairs_L;
                            break;
                        case 36: // охранник
                            tmp = sofa_U;
                            break;
                        case 37: // охранник
                            tmp = sofa_R;
                            break;
                        case 38: // охранник
                            tmp = sofa_D;
                            break;
                        case 39: // охранник
                            tmp = sofa_L;
                            break;
                        case 40: // охранник
                            tmp = key;
                            break;
                        case 41: // охранник
                            tmp = card;
                            break;
                        case 50: // охранник
                            tmp = money;
                            break;
                        case 51: // охранник
                            tmp = rubin;
                            break;
                        case 52: // охранник
                            tmp = brilliant;
                            break;
                        case 53: // охранник
                            tmp = picture1;
                            break;
                        case 54: // охранник
                            tmp = picture2;
                            break;
                        case 55: // охранник
                            tmp = picture3;
                            break;
                        case 60: // охранник
                            tmp = spLU;
                            break;
                        case 61: // охранник
                            tmp = spUR;
                            break;
                        case 62: // охранник
                            tmp = spRD;
                            break;
                        case 63: // охранник
                            tmp = spDL;
                            break;
                        case 70: // охранник
                            tmp = tableU;
                            break;
                        case 71: // охранник
                            tmp = tableR;
                            break;
                        case 72: // охранник
                            tmp = tableD;
                            break;
                        case 73: // охранник
                            tmp = tableL;
                            break;
                        case 80: // охранник
                            tmp = laser2_horiz;
                            break;
                        case 81: // охранник
                            tmp = laser2_vert;
                            break;
                        case 82: // охранник
                            tmp = laser2_horiz;
                            break;
                        case 83: // охранник
                            tmp = laser2_vert;
                            break;
                        case 100: // охранник
                            tmp = player;
                            break;

                        case 300: // ход охранника
                            tmp = im_way_guard;
                            break;

                    }
                    
                    g.DrawImage(tmp, Field_Cells[i, j].Location);
                }
        }

        private void Print_table()
        {
            dataGridView1.ColumnCount = 6;
            dataGridView1.RowCount = WayGuards.Count;
            for (int i = 0; i < WayGuards.Count; i++)
            {
                dataGridView1.Rows[i].Cells[0].Value = 30;
                for (int j = 1; j < 5; j++)
                {
              
                    dataGridView1.Rows[i].Cells[j].Value = WayGuards[i][j-1];
                }
            }

        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            B_Wall.Enabled = false;
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
            if ((x >= 0 && x < Num_Cells_H) && (y >= 0 && y < Num_Cells_V))
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (!f_way_g)
                    {
                        f_way_g = true;                        
                    }
                    else
                    {
                        B_Delete.Enabled = false;
                        Delete_Cell(x, y);
                        B_Delete.Enabled = true;
                    }
                    Invalidate();
                    return;
                }
                else
                {
                    label1.Text = Convert.ToString(x) + " " + Convert.ToString(y);
                    if (B_Delete.Enabled == false)
                    {
                        Delete_Cell(x, y);
                    }
                    else
                    {                       
                        if (B_Wall.Enabled == false)
                        {
                            f_way_g = true;
                            Field_Cells[x, y].Obj = 1;
                        }
                        else
                        {
                            /*
                            Дверь открытая вертикальная
                            Дверь открытая горизонатальная
                            Дверь закрытая вертикальная
                            Дверь закрытая горизонатальная
                             Chairs_U = 32,
                            Chairs_R = 33,
                            Chairs_D = 34,
                            Chairs_L = 35,
                            sofa_U = 36,
                            sofa_R = 37,
                            sofa_D = 38,
                            sofa_L = 39,
                            Key = 40,
                            Card = 41,
                            Gold = 50,
                            Rubin = 51,
                            Brilliant = 52,
                            Picture1 = 53,
                            Picture2 = 54,
                            Picture3 = 55,
                            SpLU = 60,
                            SpUR = 61,
                            SpRD = 62,
                            SpDL = 63,
                            TableU = 70,
                            TableR = 71,
                            TableD = 72,
                            TableL = 73,
                            LaserHoriz = 80,
                            LaserVertic = 81,
                            LaserHorizMoving = 82,
                            LaserVerticMoving = 83,
                            Player = 100*/

                            switch (Convert.ToString(cb_objs.SelectedItem))
                            {
                                case "Дверь открытая вертикальная":
                                    f_way_g = true;
                                    Field_Cells[x, y].Obj = 23;
                                    break;

                                case "Дверь открытая горизонатальная":

                                    Field_Cells[x, y].Obj = 22;
                                    break;

                                case "Дверь закрытая вертикальная":
                                    Field_Cells[x, y].Obj = 21;
                                    break;

                                case "Дверь закрытая горизонатальная":
                                    Field_Cells[x, y].Obj = 20;
                                    break;

                                case "Chairs_U = 32,":
                                    Field_Cells[x, y].Obj = 32;
                                    break;
                                case "Chairs_R = 33,":

                                    Field_Cells[x, y].Obj = 33;
                                    break;

                                case "Chairs_D = 34,":
                                    Field_Cells[x, y].Obj = 34;
                                    break;

                                case "Chairs_L = 35,":
                                    Field_Cells[x, y].Obj = 35;
                                    break;
                                case "sofa_U = 36,":

                                    Field_Cells[x, y].Obj = 36;
                                    break;

                                case "sofa_R = 37,":
                                    Field_Cells[x, y].Obj = 37;
                                    break;

                                case "sofa_D = 38,":
                                    Field_Cells[x, y].Obj = 38;
                                    break;
                                case "sofa_L = 39,":

                                    Field_Cells[x, y].Obj = 39;
                                    break;

                                case "Key = 40,":
                                    Field_Cells[x, y].Obj = 40;
                                    break;

                                case "Card = 41,":
                                    Field_Cells[x, y].Obj = 41;
                                    break;

                                case "Gold = 50,":
                                    Field_Cells[x, y].Obj = 50;
                                    break;

                                case "Rubin = 51,":
                                    Field_Cells[x, y].Obj = 51;
                                    break;
                                case "Brilliant = 52,":

                                    Field_Cells[x, y].Obj = 52;
                                    break;

                                case "Picture1 = 53,":
                                    Field_Cells[x, y].Obj = 53;
                                    break;

                                case "Picture2 = 54,":
                                    Field_Cells[x, y].Obj = 54;
                                    break;
                                case "Picture3 = 55,":

                                    Field_Cells[x, y].Obj = 55;
                                    break;

                                case "SpLU = 60,":
                                    Field_Cells[x, y].Obj = 60;
                                    break;

                                case "SpUR = 61,":
                                    Field_Cells[x, y].Obj = 61;
                                    break;
                                case "SpRD = 62,":
                                    Field_Cells[x, y].Obj = 62;
                                    break;

                                case "SpDL = 63,":
                                    Field_Cells[x, y].Obj = 63;
                                    break;
                                case "TableU = 70,":

                                    Field_Cells[x, y].Obj = 70;
                                    break;

                                case "TableR = 71,":
                                    Field_Cells[x, y].Obj = 71;
                                    break;

                                case "TableD = 72,":
                                    Field_Cells[x, y].Obj = 72;
                                    break;

                                case "TableL = 73,":
                                    Field_Cells[x, y].Obj = 73;
                                    break;
                                case "LaserHoriz = 80,":
                                    Field_Cells[x, y].Obj = 80;
                                    break;

                                case "LaserVertic = 81,":
                                    Field_Cells[x, y].Obj = 81;
                                    break;
                                case "LaserHorizMoving = 82,":

                                    Field_Cells[x, y].Obj = 82;
                                    break;

                                case "LaserVerticMoving = 83,":
                                    Field_Cells[x, y].Obj = 83;
                                    break;

                                case "Player = 100":
                                    Field_Cells[x, y].Obj = 100;
                                    break;

                               

                                case "Охранник":
                                    if (!f_way_g)
                                    {
                                        Field_Cells[x, y].Obj = 300;
                                        WayGuards.Add(new List<int>());
                                        WayGuards[WayGuards.Count - 1].Add(num_guard);
                                        WayGuards[WayGuards.Count - 1].Add(num_step_guards);
                                        WayGuards[WayGuards.Count - 1].Add(y);
                                        WayGuards[WayGuards.Count - 1].Add(x);
                                        
                                        num_step_guards++;
                                        Invalidate();
                                        return;
                                    }
                                    Field_Cells[x, y].Obj = 30;
                                    f_way_g = false;
                                    num_step_guards = 1;
                                    num_guard++;
                                    WayGuards.Add(new List<int>());
                                    WayGuards[WayGuards.Count - 1].Add(num_guard);
                                    WayGuards[WayGuards.Count - 1].Add(0);
                                    WayGuards[WayGuards.Count - 1].Add(y);
                                    WayGuards[WayGuards.Count - 1].Add(x);
                                    break;
                            }
                        }
                    }

                    Invalidate();
                }
            }
        }


        private void Delete_Cell(int x, int y)
        {
            if (Field_Cells[x, y].Obj == 300)
            {
                for (int i = 0; i < WayGuards.Count; i++)
                {
                    if (WayGuards[i][3] == x)
                        if (WayGuards[i][2] == y)
                        {
                            WayGuards.RemoveAt(i);
                            break;
                        }
                }

            }
            if (Field_Cells[x, y].Obj == 30)
            {
                for (int i = 0; i < WayGuards.Count; i++)
                {
                    // если это местоположение охранника
                    if (WayGuards[i][1] == 0)
                    {
                        // если координата удаления соответсвует координате охранника
                       
                            if (WayGuards[i][2] == y && WayGuards[i][3] == x)
                            {
                                // запоминаем идентификатор удаляемого охранника
                                int id = WayGuards[i][0];
                                WayGuards.RemoveAt(i); // удаляем охранника
                                // далее ищем шаги этого охранника для удаления
                                for (; i < WayGuards.Count; i++)
                                    if (WayGuards[i][0] == id)
                                    {
                                        int xx = WayGuards[i][3];
                                        int yy = WayGuards[i][2];
                                        bool fl = false;
                                        for (int j = 0; j < WayGuards.Count; j++)
                                        {
                                            // если клетка пересекается с другим охранником
                                            if (WayGuards[j][2] == yy && WayGuards[j][3] == xx)
                                            {
                                                if (j != i && WayGuards[i][0] != WayGuards[j][0])
                                                {
                                                    fl = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (!fl)
                                            Field_Cells[WayGuards[i][3], WayGuards[i][2]].Obj = 0;
                                        WayGuards.RemoveAt(i);
                                        i--;
                                    }
                                f_way_g = true;
                                break;
                            }
                    }                    
                }
            }
            Field_Cells[x, y].Obj = 0;
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
                file.WriteLine(Field_Cells.GetLongLength(1) + " " + Field_Cells.GetLongLength(0));
                for (int i = 0; i < Field_Cells.GetLength(1); i++)
                {
                    for (int j = 0; j < Field_Cells.GetLength(0); j++)
                    {
                        file.Write(Field_Cells[j, i].Obj + " ");

                    }
                    file.WriteLine("");
                }
                for (int i = 0; i < WayGuards.Count; i++)
                {
                    file.Write("30");
                    for (int j = 0; j < WayGuards[i].Count; j++)
                    {
                        file.Write(" " + WayGuards[i][j]);
                    }
                    file.WriteLine("");
                }
                int[,] a = new int[Field_Cells.GetLength(1), Field_Cells.GetLength(0)];           
                a = MakeRooms();
                for (int i = 0; i < a.GetLength(0); i++)
                {
                    for (int j = 0; j < a.GetLength(1); j++)
                    {
                        file.Write(" " + a[i,j]);
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
                string [] file = File.ReadAllLines(openFile.FileName);
                string[] fline = file[0].Split(' ');
                cb_m.SelectedItem = Int32.Parse(fline[0]) - 7;
                cb_n.SelectedItem = Int32.Parse(fline[1]) - 7;
                Create_Field();
            }
        }


        private int[,] MakeRooms()
        {
            int[,] a = new int [Field_Cells.GetLength(1), Field_Cells.GetLength(0)];
            
            for (int i = 0; i < Field_Cells.GetLength(1); i++)
            {
                for (int j = 0; j < Field_Cells.GetLength(0); j++)
                {
                   // все стены это,двери -1, остальное 0
                    if (Field_Cells[j, i].Obj == 1  || 
                        Field_Cells[j, i].Obj == 20 ||
                        Field_Cells[j, i].Obj == 21 ||
                        Field_Cells[j, i].Obj == 22 ||
                        Field_Cells[j, i].Obj == 23 
                        )
                        a[i,j] = -1; 
                    else  a[i, j] = 0;
                }
            }

            int num_room = 1;
            for(int i = 0; i < a.GetLength(0); i++)
                for(int j = 0; j < a.GetLength(1); j++)
                {
                    // если втречаем не стену, распространяем волну номера комнаты
                    if(a[i,j] == 0)
                    {
                        a[i, j] = num_room;
                        FindRoom(a, num_room, i, j);
                        num_room++;
                    }
                }
            return a;
        }
        
        private int[,] FindRoom(int [,] a, int num_room, int i, int j)
        {
            if ( i < a.GetLength(0) - 1)
            {
                
                if( j < a.GetLength(1) - 1)
                {
                    // комната закончилась
                    if (a[i, j + 1] != 0 && 
                        a[i + 1, j] != 0 &&
                        (j == 0 ? 1 : a[i, j - 1]) != 0 &&
                        (i == 0 ? 1 : a[i - 1, j]) != 0
                        )
                    {
                        return a;
                    }
                    if (a[i, j + 1] == 0)
                    {
                        a[i, j + 1] = num_room;
                        FindRoom(a, num_room, i, j+1);
                    }
                    if (a[i + 1, j] == 0)
                    {
                        a[i + 1, j] = num_room;
                        FindRoom(a, num_room, i+1, j);
                    }
                    if ((j == 0 ? 1 : a[i, j - 1]) == 0)
                    {
                        a[i, j - 1] = num_room;
                        FindRoom(a, num_room, i, j - 1);
                    }
                    if ((i == 0 ? 1 : a[i - 1, j]) == 0)
                    {
                        a[i - 1, j ] = num_room;
                        FindRoom(a, num_room, i - 1, j);
                    }
                }
                
            }
            return a;
        }


        
    }

        
    }

