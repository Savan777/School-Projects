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

namespace ShapeWars
{
    public partial class MenuForm : Form
    {
        Label Header;
        Button start;
        Button start2;
        Button close;
        Button instructions;
        Button highScores;
        Label instruc;
        GameForm game = new GameForm();
        gameform2 game2 = new gameform2();
        Label score;
        StreamReader p1score;
        StreamReader p2score;
        string score1;
        string score2;
        Int32 ttscore;

        public static bool showform;

        public MenuForm()
        {
            InitializeComponent();
            instruc = new Label();
            showform = true;
            score = new Label();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);
            this.Location = new Point(0, 0);
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.Crimson;

            p1score = new StreamReader(@"P1Score.txt");
            if (File.Exists(@"P1Score.txt"))
            {
                score1 = p1score.ReadLine();
                p1score.Close();
            }

            p2score = new StreamReader(@"P2Score.txt");
            if (File.Exists(@"P2Score.txt"))
            {
                score2 = p2score.ReadLine();
                p2score.Close();
            }

            ttscore = Convert.ToInt32(score1) + Convert.ToInt32(score2);

            Header = new Label();
            Header.Text = "Shape Wars";
            Header.Width = 1300;
            Header.Height = 120;
            Header.Location = new Point(this.ClientSize.Width / 2 - Header.Width / 2 + 60, 100);
            Header.ForeColor = Color.BlueViolet;
            Header.Font = new Font("Segoe Keycaps", 67, FontStyle.Regular);
            Header.Visible = true;
            this.Controls.Add(Header);

            start = new Button();
            start.Text = "2 Player";
            start.BackColor = Color.Black;
            start.ForeColor = Color.WhiteSmoke;
            start.Font = new Font("Motorwerk", 12, FontStyle.Regular);
            start.Size = new Size(100, 50);
            start.Location = new Point(this.ClientSize.Width / 2 - start.Width / 2, this.ClientSize.Height / 2 - start.Height / 2);
            start.FlatStyle = FlatStyle.Popup;
            start.Visible = true;
            this.Controls.Add(start);

            start2 = new Button();
            start2.Text = "1 Player";
            start2.BackColor = Color.Black;
            start2.ForeColor = Color.WhiteSmoke;
            start2.Font = new Font("Motorwerk", 12, FontStyle.Regular);
            start2.Size = new Size(100, 50);
            start2.Location = new Point(this.ClientSize.Width / 2 - start.Width / 2, this.ClientSize.Height / 2 - start.Height / 2 - 57);
            start2.FlatStyle = FlatStyle.Popup;
            start2.Visible = true;
            this.Controls.Add(start2);

            close = new Button();
            close.Text = "Close";
            close.BackColor = Color.Black;
            close.ForeColor = Color.WhiteSmoke;
            close.Font = new Font("Motorwerk", 12, FontStyle.Regular);
            close.Size = new Size(100, 50);
            close.Location = new Point(this.ClientSize.Width / 2 - close.Width / 2, this.ClientSize.Height / 2 - close.Height / 2 + 100);
            close.FlatStyle = FlatStyle.Popup;
            close.Visible = true;
            this.Controls.Add(close);

            instructions = new Button();
            instructions.Text = "Instructions";
            instructions.BackColor = Color.Black;
            instructions.ForeColor = Color.WhiteSmoke;
            instructions.Font = new Font("Motorwerk", 12, FontStyle.Regular);
            instructions.Size = new Size(100, 50);
            instructions.Location = new Point(this.ClientSize.Width / 2 - instructions.Width / 2 - 100, this.ClientSize.Height / 2 - instructions.Height / 2 + 50);
            instructions.FlatStyle = FlatStyle.System;
            instructions.Visible = true;
            this.Controls.Add(instructions);

            highScores = new Button();
            highScores.Text = "High Scores";
            highScores.BackColor = Color.Black;
            highScores.ForeColor = Color.WhiteSmoke;
            highScores.Font = new Font("Motorwerk", 12, FontStyle.Regular);
            highScores.Size = new Size(100, 50);
            highScores.Location = new Point(this.ClientSize.Width / 2 - highScores.Width / 2 + 100, this.ClientSize.Height / 2 - highScores.Height / 2 + 50);
            highScores.FlatStyle = FlatStyle.System;
            highScores.Visible = true;
            this.Controls.Add(highScores);

            instruc.Visible = false;
            score.Visible = false;

            start2.Click += new EventHandler(start2_Click);
            close.Click += new EventHandler(close_Click);
            instructions.Click += new EventHandler(instructions_Click);
            highScores.Click += new EventHandler(highScores_Click);
            start.Click += new EventHandler(start_Click);
            this.Paint += new PaintEventHandler(MenuForm_Paint);
        }

        void start2_Click(object sender, EventArgs e)
        {
            showform = false;
            this.Hide();
            game2 = new gameform2();
            game2.Show();
        }

        void MenuForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawString("By: Savan Patel & Jaime Barrillas", new Font("Motorwerk", 30, FontStyle.Italic), Brushes.GhostWhite, new PointF(307, 757)); 
        }

        void start_Click(object sender, EventArgs e)
        {
            showform = false;
            this.Hide();
            game = new GameForm();
            game.Show();
        }

        void highScores_Click(object sender, EventArgs e)
        {
            score.Font = new Font("Jing Jing", 14, FontStyle.Regular);
            score.Location = new Point(890, 465);
            score.Size = new Size(220, 200);
            score.Text = "P1 Score: " + score1 + "\n";
            score.Text += "\nP2 Score: " + score2 + "\n";
            score.Text += "\nTotal Score: " + Convert.ToString(ttscore);
            this.Controls.Add(score);

            if (!score.Visible)
            {
                score.Visible = true;
            }
            else
            {
                score.Visible = false;
            }
        }

        void instructions_Click(object sender, EventArgs e)
        {
            instruc.Font = new Font("Jing Jing", 12, FontStyle.Regular);
            instruc.Text = "P1                                 P2\n";
            instruc.Text += "=======================\n\n";
            instruc.Text += "Up: ↑                           Up: W\n\n";
            instruc.Text += "Down: ↓                     Down: S\n\n";
            instruc.Text += "Left: ←                         Left: A\n\n";
            instruc.Text += "Right: →                       Right: D\n\n";
            instruc.Text += "Shoot: RightShift       Shoot: Space";
            instruc.Size = new Size(400, 400);
            instruc.Location = new Point(50, this.ClientSize.Height / 2 - instruc.Height / 2 + 100);
            this.Controls.Add(instruc);

            if (!instruc.Visible)
            {
                instruc.Visible = true;
            }
            else
            {
                instruc.Visible = false;
            }
        }

        void close_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
