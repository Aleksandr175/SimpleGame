﻿namespace Search_minimum_way
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
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cb_n = new System.Windows.Forms.ComboBox();
            this.cb_m = new System.Windows.Forms.ComboBox();
            this.cb_objs = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menu_str = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // B_Delete
            // 
            this.B_Delete.BackColor = System.Drawing.Color.MediumOrchid;
            this.B_Delete.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.B_Delete.ForeColor = System.Drawing.Color.SandyBrown;
            this.B_Delete.Image = global::Search_minimum_way.Properties.Resources.Empty_Cells;
            this.B_Delete.Location = new System.Drawing.Point(721, 335);
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
            this.B_Wall.Location = new System.Drawing.Point(721, 145);
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
            this.label1.Location = new System.Drawing.Point(718, 386);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(759, 386);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(699, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 13);
            this.label3.TabIndex = 7;
            // 
            // cb_n
            // 
            this.cb_n.FormattingEnabled = true;
            this.cb_n.Location = new System.Drawing.Point(678, 67);
            this.cb_n.Name = "cb_n";
            this.cb_n.Size = new System.Drawing.Size(121, 21);
            this.cb_n.TabIndex = 8;
            this.cb_n.SelectedIndexChanged += new System.EventHandler(this.cm_n_SelectedIndexChanged);
            // 
            // cb_m
            // 
            this.cb_m.FormattingEnabled = true;
            this.cb_m.Location = new System.Drawing.Point(842, 67);
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
            "Охранник"});
            this.cb_objs.Location = new System.Drawing.Point(721, 210);
            this.cb_objs.Name = "cb_objs";
            this.cb_objs.Size = new System.Drawing.Size(121, 21);
            this.cb_objs.TabIndex = 10;
            this.cb_objs.SelectedIndexChanged += new System.EventHandler(this.cb_objs_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menu_str});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1028, 24);
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
            this.OpenToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.OpenToolStripMenuItem.Text = "Открыть";
            this.OpenToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // SaveToolStripMenuItem
            // 
            this.SaveToolStripMenuItem.Name = "SaveToolStripMenuItem";
            this.SaveToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.SaveToolStripMenuItem.Text = "Сохранить ";
            this.SaveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1028, 460);
            this.Controls.Add(this.cb_objs);
            this.Controls.Add(this.cb_m);
            this.Controls.Add(this.cb_n);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.B_Delete);
            this.Controls.Add(this.B_Wall);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseClick);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button B_Wall;
        private System.Windows.Forms.Button B_Delete;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cb_n;
        private System.Windows.Forms.ComboBox cb_m;
        private System.Windows.Forms.ComboBox cb_objs;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menu_str;
        private System.Windows.Forms.ToolStripMenuItem OpenToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveToolStripMenuItem;


    }
}
