//Shape Wars
//By: Savan & J.B.
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Text;

namespace ShapeWars
{
    public partial class GameOver_form : Form
    {
        Label title;
        StreamReader scorereader;
        StreamReader scoreReader2;
        string p1Score;
        string p2Score;
        Int32 totalScore;
        Button close;

        // initializing global variables for reading font from textfile from debug folder
        PrivateFontCollection a;
        Font aduore3D;

        public GameOver_form()
        {
            InitializeComponent();
        }

        private void GameOver_form_Load(object sender, EventArgs e)
        {
            this.BackColor = Color.Black;
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
            this.Paint += new PaintEventHandler(GameOver_form_Paint);

            title = new Label();
            title.Size = new Size(700, 200);
            title.ForeColor = Color.MediumPurple;
            title.Text = "Game Over";
            title.Font = new Font("Comic Sans MS", 72, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout);
            title.Location = new Point(this.ClientSize.Width / 2 - title.Width / 2 + 70, 100);
            title.Visible = true;
            this.Controls.Add(title);

            close = new Button();
            close.Size = new Size(100, 50);
            close.Text = "Exit";
            close.Font = new Font("Jing Jing", 14, FontStyle.Regular);
            close.FlatStyle = FlatStyle.Flat;
            close.ForeColor = Color.SkyBlue;
            close.Location = new Point(550, 650);
            close.Visible = true;
            this.Controls.Add(close);

            scorereader = new StreamReader(@"P1Score.txt");
            if (File.Exists(@"P1Score.txt"))
            {
                p1Score = scorereader.ReadLine();
                scorereader.Close();
            }

            scoreReader2 = new StreamReader(@"P2Score.txt");
            if (File.Exists(@"P2Score.txt"))
            {
                p2Score = scoreReader2.ReadLine();
                scoreReader2.Close();
            }

            totalScore = Convert.ToInt32(p1Score) + Convert.ToInt32(p2Score);

            //taking the text font from the text file in the debug folder
            a = new PrivateFontCollection();
            a.AddFontFile(@"Ardour 3D GM.ttf");
            aduore3D = new Font(a.Families[0], 30);

            close.Click += new EventHandler(close_Click);
        }

        void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        void GameOver_form_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("Player 1", new Font("Motorwerk", 30, FontStyle.Regular), Brushes.Orange, new PointF(100, 400));
            e.Graphics.DrawString("Total Score", new Font("Motorwerk", 30, FontStyle.Regular), Brushes.Yellow, new PointF(500, 400));
            e.Graphics.DrawString("Player 2", new Font("Motorwerk", 30, FontStyle.Regular), Brushes.Green, new PointF(950, 400));
            e.Graphics.DrawString(p1Score, aduore3D, Brushes.Purple, new PointF(140, 500));
            e.Graphics.DrawString(Convert.ToString(totalScore), aduore3D, Brushes.Purple, new PointF(557, 500));
            e.Graphics.DrawString(p2Score, aduore3D, Brushes.Purple, new PointF(990, 500));
            e.Graphics.DrawString("By: Savan Patel & Jaime Barrillas", new Font("Motorwerk",30,FontStyle.Italic),Brushes.GhostWhite,new PointF(307,757)); 
        }
    }
}
