namespace Search_minimum_way
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.B_Delete = new System.Windows.Forms.Button();
            this.B_Wall = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_n = new System.Windows.Forms.ComboBox();
            this.cb_m = new System.Windows.Forms.ComboBox();
            this.cb_objs = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_str = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // B_Delete
            // 
            this.B_Delete.BackColor = System.Drawing.Color.MediumOrchid;
            this.B_Delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Delete.ForeColor = System.Drawing.Color.SandyBrown;
            this.B_Delete.Image = global::Search_minimum_way.Properties.Resources.Empty_Cells;
            this.B_Delete.Location = new System.Drawing.Point(514, 200);
            this.B_Delete.Name = "B_Delete";
            this.B_Delete.Size = new System.Drawing.Size(77, 42);
            this.B_Delete.TabIndex = 3;
            this.B_Delete.Text = "Стереть";
            this.B_Delete.UseVisualStyleBackColor = false;
            this.B_Delete.Click += new System.EventHandler(this.B_Delete_Click);
            // 
            // B_Wall
            // 
            this.B_Wall.BackColor = System.Drawing.SystemColors.HotTrack;
            this.B_Wall.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Wall.ForeColor = System.Drawing.Color.SandyBrown;
            this.B_Wall.Image = global::Search_minimum_way.Properties.Resources.Wall;
            this.B_Wall.Location = new System.Drawing.Point(514, 90);
            this.B_Wall.Name = "B_Wall";
            this.B_Wall.Size = new System.Drawing.Size(77, 42);
            this.B_Wall.TabIndex = 0;
            this.B_Wall.Text = "Стена";
            this.B_Wall.UseVisualStyleBackColor = false;
            this.B_Wall.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(511, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // cb_n
            // 
            this.cb_n.FormattingEnabled = true;
            this.cb_n.Location = new System.Drawing.Point(514, 50);
            this.cb_n.Name = "cb_n";
            this.cb_n.Size = new System.Drawing.Size(121, 21);
            this.cb_n.TabIndex = 8;
            this.cb_n.SelectedIndexChanged += new System.EventHandler(this.cm_n_SelectedIndexChanged);
            // 
            // cb_m
            // 
            this.cb_m.FormattingEnabled = true;
            this.cb_m.Location = new System.Drawing.Point(678, 50);
            this.cb_m.Name = "cb_m";
            this.cb_m.Size = new System.Drawing.Size(121, 21);
            this.cb_m.TabIndex = 9;
            this.cb_m.SelectedIndexChanged += new System.EventHandler(this.cb_m_SelectedIndexChanged);
            // 
            // cb_objs
            // 
            this.cb_objs.FormattingEnabled = true;
            this.cb_objs.Items.AddRange(new object[] {
            "Дверь открытая вертикальная",
            "Дверь открытая горизонатальная",
            "Дверь закрытая вертикальная",
            "Дверь закрытая горизонатальная",
            "Охранник",
            "Chairs_U = 32,",
            "Chairs_R = 33,",
            "Chairs_D = 34,",
            "Chairs_L = 35,",
            "sofa_U = 36,",
            "sofa_R = 37,",
            "sofa_D = 38,",
            "sofa_L = 39,",
            "Key = 40,",
            "Card = 41,",
            "Gold = 50,",
            "Rubin = 51,",
            "Brilliant = 52,",
            "Picture1 = 53,",
            "Picture2 = 54,",
            "Picture3 = 55,",
            "SpLU = 60,",
            "SpUR = 61,",
            "SpRD = 62,",
            "SpDL = 63,",
            "TableU = 70,",
            "TableR = 71,",
            "TableD = 72,",
            "TableL = 73,",
            "LaserHoriz = 80,",
            "LaserVertic = 81,",
            "LaserHorizMoving = 82,",
            "LaserVerticMoving = 83,",
            "Player = 100"});
            this.cb_objs.Location = new System.Drawing.Point(514, 156);
            this.cb_objs.Name = "cb_objs";
            this.cb_objs.Size = new System.Drawing.Size(214, 21);
            this.cb_objs.TabIndex = 10;
            this.cb_objs.SelectedIndexChanged += new System.EventHandler(this.cb_objs_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_str});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(829, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menu_str
            // 
            this.menu_str.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenToolStripMenuItem,
            this.SaveToolStripMenuItem});
            this.menu_str.Name = "menu_str";
            this.menu_str.Size = new System.Drawing.Size(45, 20);
            this.menu_str.Text = "Файл";
            // 
            // OpenToolStripMenuItem
            // 
            this.OpenToolStripMenuItem.Name = "OpenToolStripMenuItem";
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.OpenToolStripMenuItem.Text = "Открыть";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.SaveToolStripMenuItem.Text = "Сохранить ";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(603, 183);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(196, 247);
            this.dataGridView1.TabIndex = 12;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(829, 460);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cb_objs);
            this.Controls.Add(this.cb_m);
            this.Controls.Add(this.cb_n);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.B_Delete);
            this.Controls.Add(this.B_Wall);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Wall;
        private System.Windows.Forms.Button B_Delete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_n;
        private System.Windows.Forms.ComboBox cb_m;
        private System.Windows.Forms.ComboBox cb_objs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_str;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;
        private System.Windows.Forms.DataGridView dataGridView1;


    }
}

