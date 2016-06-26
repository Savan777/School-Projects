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
    public partial class gameform2 : Form
    {
        //variables
        int maxHealth = 20; //could be used for upgrades/allows for easy switching of values
        int p1score = 0; //holds score

        Label scoreDisplay;

        //player rects
        Rectangle player1;
        int p1Health;
        int p1X;
        int p1Y;
        bool[] p1Move; //used to check whether the player can move
        string p1DirectionFacing = "up"; //used to check direction
        ProgressBar p1HealthBar;
        Label p1HealthDisplay;
        Rectangle[] p1Shot; //rect for p1's gun
        int[] p1ShotX;
        int[] p1ShotY;
        int p1ShotNum = 0; //keeps track of which rect is selected
       
        Rectangle[] enemyS1; //lvl 1 enemy
        int[] e1X;
        int[] e1Y;
        Rectangle[] enemyS2; //lvl 2 enemy
        int[] e2Health;
        int[] e2X;
        int[] e2Y;
        
        Rectangle[] enemyS3; //lvl 3 enemy
        int[] e3Health;
        int[] e3X;
        int[] e3Y;
        Rectangle[] enemyBoss; //Boss
        int[] eBHealth;
        int[] bX;
        int[] bY;

        //PowerUps
        Rectangle healthPowerUp;

        //Images
        Image playerShotPic;
        Image healthPowerUpPic;

        //Timers
        Timer refreshMiscT; //timer to refresh the screen *also for miscellaneous jobs
        Timer movementT; //used solely for movement of stuff
        Timer intersectTimer; //timer used to check for intersection between any shots and rectangles

        //Misc
        Random rand = new Random();

        //obstacle rects
        Rectangle[] obstacle; //triangles
        Rectangle core; //the core to protect
        int coreMiddleX; //gets the x value of the middle of the core
        int coreMiddleY; //gets the Y value of the middle of the core
        Point[] triangle0; //points
        Point[] triangle1; //points
        Point[] triangle2; //points
        Point[] triangle3; //points

        GameOver_form gameOverForm = new GameOver_form();
        private bool gameover;

        StreamWriter P1scoreWriter;
        StreamReader P1scoreReader;

        string P1tempScore;

        public gameform2()
        {
            InitializeComponent();
            //Loading pictures
            playerShotPic = Image.FromFile(@"PlayerShot.jpg", true);
            healthPowerUpPic = Image.FromFile(@"HealthPowerUp.png", true);

            //initailizing arrays
            obstacle = new Rectangle[4];
            triangle0 = new Point[3];
            triangle1 = new Point[3];
            triangle2 = new Point[3];
            triangle3 = new Point[3];

            p1Move = new bool[2]; //0 = horizontal movement, 1 = vertical movement

            p1Shot = new Rectangle[10];
            p1ShotX = new int[p1Shot.Length];
            p1ShotY = new int[p1Shot.Length];
           
            for (int i = 0; i < p1Shot.Length; i++)
            {
                p1ShotY[i] = -10;
            }

            //enemy arrays
            enemyS1 = new Rectangle[5];
            e1X = new int[enemyS1.Length];
            e1Y = new int[enemyS1.Length];
            for (int i = 0; i < e1X.Length; i++)
            {
                e1X[i] = 7;
                e1Y[i] = 0;
            }

            enemyS2 = new Rectangle[5];
            e2Health = new int[enemyS2.Length];
            e2X = new int[enemyS2.Length];
            e2Y = new int[enemyS2.Length];
            for (int i = 0; i < e2X.Length; i++)
            {
                e2X[i] = 7;
                e2Y[i] = 0;
                e2Health[i] = 2;
            }

            enemyS3 = new Rectangle[5];
            e3Health = new int[enemyS3.Length];
            e3X = new int[enemyS3.Length];
            e3Y = new int[enemyS3.Length];
  
            for (int i = 0; i < e3X.Length; i++)
            {
                e3X[i] = 7;
                e3Y[i] = 0;
                e3Health[i] = 4;
            }

            enemyBoss = new Rectangle[4];
            eBHealth = new int[enemyBoss.Length];
            bX = new int[enemyBoss.Length];
            bY = new int[enemyBoss.Length];
    
            for (int i = 0; i < bX.Length; i++)
            {
                bX[i] = 7;
                bY[i] = 0;
                eBHealth[i] = 8;
            }
        }

        private void gameform2_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;
            this.FormClosing += new FormClosingEventHandler(gameform2_FormClosing);

            //makes the obstacle
            for (int i = 0; i < obstacle.Length; i++)
            { //uses 'Screen.PrimaryScreen' in order to make the playing field fit for different sized screens
                obstacle[i].Size = new Size((Screen.PrimaryScreen.WorkingArea.Width / 5), (Screen.PrimaryScreen.WorkingArea.Height / 5));
            }
            obstacle[0].Location = new Point(100, 100);
            obstacle[1].Location = new Point((this.ClientSize.Width - obstacle[1].Width - 100), 100);
            obstacle[2].Location = new Point(100, (this.ClientSize.Height - obstacle[2].Height - 100));
            obstacle[3].Location = new Point((this.ClientSize.Width - obstacle[3].Width - 100), (this.ClientSize.Height - obstacle[3].Height - 100));

            //gets all points for drawing the triangles
            triangle0[0] = new Point(obstacle[0].X, obstacle[0].Y);
            triangle0[1] = new Point((obstacle[0].X + obstacle[0].Width), obstacle[0].Y);
            triangle0[2] = new Point(obstacle[0].X, (obstacle[0].Y + obstacle[0].Height));

            triangle1[0] = new Point((obstacle[1].X + obstacle[1].Width), obstacle[1].Y);
            triangle1[1] = new Point(obstacle[1].X, obstacle[1].Y);
            triangle1[2] = new Point((obstacle[1].X + obstacle[1].Width), (obstacle[1].Y + obstacle[1].Height));

            triangle2[0] = new Point(obstacle[2].X, (obstacle[2].Y + obstacle[2].Height));
            triangle2[1] = new Point(obstacle[2].X, obstacle[2].Y);
            triangle2[2] = new Point((obstacle[2].X + obstacle[2].Width), (obstacle[2].Y + obstacle[2].Height));

            triangle3[0] = new Point((obstacle[3].X + obstacle[3].Width), (obstacle[3].Y + obstacle[3].Height));
            triangle3[1] = new Point((obstacle[3].X + obstacle[3].Width), obstacle[3].Y);
            triangle3[2] = new Point(obstacle[3].X, (obstacle[3].Y + obstacle[3].Height));

            core = new Rectangle();
            core.Size = new Size(150, 150);
            core.Location = new Point((this.ClientSize.Width / 2) - (core.Width / 2), (this.ClientSize.Height / 2) - (core.Height / 2));
            coreMiddleX = core.X + (core.Width / 2); //these two may be used when i figure out how to stop movement-
            coreMiddleY = core.Y + (core.Height / 2); //when players hit the middle, too lazy to do it right now :P

            //Stuff
            scoreDisplay = new Label();
            scoreDisplay.Size = new Size(140, 30);
            scoreDisplay.Location = new Point(coreMiddleX - (scoreDisplay.Width / 2), coreMiddleY - (scoreDisplay.Height / 2));
            scoreDisplay.ForeColor = Color.White;
            scoreDisplay.BackColor = Color.Blue;
            scoreDisplay.TextAlign = ContentAlignment.MiddleCenter;
            scoreDisplay.Text = "P1 Score: " + p1score;
            this.Controls.Add(scoreDisplay);

            //player Rects
            player1 = new Rectangle();
            player1.Size = new Size(45, 45);
            p1Health = maxHealth;
            p1HealthDisplay = new Label();
            p1HealthDisplay.Size = new System.Drawing.Size(player1.Width, 5);
            p1HealthBar = new ProgressBar();
            p1HealthBar.Size = p1HealthDisplay.Size;
            p1HealthBar.Location = new Point(0, 0);
            p1HealthBar.Maximum = p1Health;
            p1HealthBar.Minimum = 0;
            p1HealthBar.Value = p1Health;
            this.Controls.Add(p1HealthDisplay);
            p1HealthDisplay.Controls.Add(p1HealthBar);

            player1.Location = new Point((this.ClientSize.Width / 2) - (player1.Width / 2), 100);
            p1HealthDisplay.Location = new Point(player1.Left, player1.Bottom);

            for (int i = 0; i < p1Shot.Length; i++)
            {
                p1Shot[i].Size = new Size(playerShotPic.Width, playerShotPic.Height);
                p1Shot[i].Location = new Point(5, -500); //offscreen

            }

            //enemy Rects
            for (int i = 0; i < enemyS1.Length; i++)
            {
                enemyS1[i].Size = new Size(30, 30);
                enemyS1[i].Location = new Point(5, -500);
                enemyS2[i].Size = player1.Size;
                enemyS2[i].Location = new Point(5, -500);
                enemyS3[i].Size = new Size(55, 55);
                enemyS3[i].Location = new Point(5, -500);
            }

            for (int i = 0; i < enemyBoss.Length; i++)
            {
                enemyBoss[i].Size = new Size(player1.Width * 2, player1.Height * 2);
                enemyBoss[i].Location = new Point(5, -500);
            }

            //PowerUps
            healthPowerUp = new Rectangle();
            healthPowerUp.Size = new System.Drawing.Size(healthPowerUpPic.Width, healthPowerUpPic.Height);
            healthPowerUp.Location = new Point(5, -500);

            //Timers
            refreshMiscT = new Timer();
            refreshMiscT.Interval = (1000 / 60);
            refreshMiscT.Tick += refreshMiscT_Tick;
            refreshMiscT.Start();

            movementT = new Timer();
            movementT.Interval = refreshMiscT.Interval;
            movementT.Tick += movementT_Tick;
            movementT.Start();

            intersectTimer = new Timer();
            intersectTimer.Interval = (1000 / 60);
            intersectTimer.Tick += intersectTimer_Tick;
            intersectTimer.Start();

            this.KeyDown += new KeyEventHandler(gameform2_KeyDown);
            this.KeyUp += new KeyEventHandler(gameform2_KeyUp);

            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(gameform2_Paint);
        }

        void gameform2_KeyUp(object sender, KeyEventArgs e)
        {
            //P1 movement
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.D)
            { //upon P1 pressing 'A' or 'D' movement will be set to 0 pixels
                p1X = 0;
            }

            if (e.KeyCode == Keys.W || e.KeyCode == Keys.S)
            { //upon P1 pressing 'W' or 'S' movement will be set to 0 pixels
                p1Y = 0;
            }
        }

        void gameform2_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillPolygon(Brushes.Cyan, triangle0);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle1);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle2);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle3);
            e.Graphics.FillEllipse(Brushes.Blue, core);
            e.Graphics.FillRectangle(Brushes.Orange, player1);
            for (int i = 0; i < p1Shot.Length; i++)
            {
                e.Graphics.DrawImage(playerShotPic, p1Shot[i]);
            }
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS1);
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS2);
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS3);
            e.Graphics.FillRectangles(Brushes.Black, enemyBoss);

            e.Graphics.DrawImage(healthPowerUpPic, healthPowerUp);
        }

        void intersectTimer_Tick(object sender, EventArgs e)
        {
            if (healthPowerUp.Y >= 0 && player1.IntersectsWith(healthPowerUp))
            {
                p1HealthBar.Step = 5;
                p1Health += 5;
                p1HealthBar.PerformStep();
                healthPowerUp.Location = new Point(5, -500);
                if (p1Health > 20)
                {
                    p1Health = 20;
                }
            }

            for (int i = 0; i < p1Shot.Length; i++)
            { //double for loop to search through both arrays, player gun shots and enemy
                for (int j = 0; j < enemyS1.Length; j++)
                {
                    //p1Shot
                    if (p1Shot[i].Bottom > 0 && p1Shot[i].IntersectsWith(enemyS1[j]))
                    {
                        p1Shot[i].Location = new Point(5, -500);
                        enemyS1[j].Location = new Point(5, -500);
                        p1score += 10;
                    }
                    else if (p1Shot[i].Bottom > 0 && p1Shot[i].IntersectsWith(enemyS2[j]))
                    {
                        e2Health[j]--;
                        p1Shot[i].Location = new Point(5, -500);
                        if (e2Health[j] <= 0)
                        {
                            enemyS2[j].Location = new Point(5, -500);
                            p1score += 15;
                            e2Health[j] = 2;
                        }
                    }
                    else if (p1Shot[i].Bottom > 0 && p1Shot[i].IntersectsWith(enemyS3[j]))
                    {
                        e3Health[j]--;
                        p1Shot[i].Location = new Point(5, -500);
                        if (e3Health[j] <= 0)
                        {
                            enemyS3[j].Location = new Point(5, -500);
                            p1score += 20;
                            e3Health[j] = 4;
                        }
                    }
                }

                for (int j = 0; j < enemyBoss.Length; j++)
                {
                    //Boss intersection
                    if (p1Shot[i].Bottom > 0 && p1Shot[i].IntersectsWith(enemyBoss[j]))
                    {
                        eBHealth[j]--;
                        p1Shot[i].Location = new Point(5, -500);
                        if (eBHealth[j] <= 0)
                        {
                            enemyBoss[j].Location = new Point(5, -500);
                            p1score += 30;
                            eBHealth[j] = 8;
                        }
                    }
                }

                scoreDisplay.Text = "P1 Score: " + p1score;
            }

            for (int i = 0; i < enemyS1.Length; i++)
            {
                //player and enemy intersection
                if (player1.Bottom > 0 && player1.IntersectsWith(enemyS1[i]))
                {
                    p1Health--;
                    p1HealthBar.Step = -1;
                    p1HealthBar.PerformStep();
                    enemyS1[i].Location = new Point(5, -500);
                    if (p1Health <= 0)
                    {
                        player1.Location = new Point(5, -500);
                    }
                }
                else if (player1.Bottom > 0 && player1.IntersectsWith(enemyS2[i]))
                {
                    p1Health--;
                    p1HealthBar.Step = -1;
                    p1HealthBar.PerformStep();
                    enemyS2[i].Location = new Point(5, -500);
                    if (p1Health <= 0)
                    {
                        player1.Location = new Point(5, -500);
                    }
                }
                else if (player1.Bottom > 0 && player1.IntersectsWith(enemyS3[i]))
                {
                    p1Health--;
                    p1HealthBar.Step = -1;
                    p1HealthBar.PerformStep();
                    enemyS3[i].Location = new Point(5, -500);
                    if (p1Health <= 0)
                    {
                        player1.Location = new Point(5, -500);
                    }
                }
            }

            for (int i = 0; i < enemyBoss.Length; i++)
            {
                //player and boss intersection
                if (player1.Bottom > 0 && player1.IntersectsWith(enemyBoss[i]))
                {
                    p1Health--;
                    p1HealthBar.Step = -1;
                    p1HealthBar.PerformStep();
                    enemyBoss[i].Location = new Point(5, -500);
                    if (p1Health <= 0)
                    {
                        player1.Location = new Point(5, -500);
                    }
                }
            }
        }

        void movementT_Tick(object sender, EventArgs e)
        { //*0 = ←→, 1 = ↑↓
            canP1Move();

            //P1 movement
            if (p1Move[0])
            {
                player1.X += p1X;
            }

            if (p1Move[1])
            {
                player1.Y += p1Y;
            }
            p1HealthDisplay.Location = new Point(player1.Left, player1.Bottom);

            //player gun movement
            for (int i = 0; i < p1Shot.Length; i++)
            {
                if (p1Shot[i].Y > 0)
                {
                    p1Shot[i].X += p1ShotX[i];
                    p1Shot[i].Y += p1ShotY[i];
                }

                if (p1Shot[i].Left < 0 || p1Shot[i].Right > this.ClientSize.Width || p1Shot[i].Top < 0 || p1Shot[i].Bottom > this.ClientSize.Height)
                { //moves gun shot off screen
                    p1Shot[i].Location = new Point(5, -500);
                    if (p1DirectionFacing == "left")
                    {
                        p1ShotX[i] = -10;
                        p1ShotY[i] = 0;
                    }
                    else if (p1DirectionFacing == "right")
                    {
                        p1ShotX[i] = 10;
                        p1ShotY[i] = 0;
                    }
                    else if (p1DirectionFacing == "up")
                    {
                        p1ShotX[i] = 0;
                        p1ShotY[i] = -10;
                    }
                    else if (p1DirectionFacing == "down")
                    {
                        p1ShotX[i] = 0;
                        p1ShotY[i] = 10;
                    }
                }
            }

            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (enemyS1[i].Y > -7)
                { //enemy1 movement
                    
                        if (enemyS1[i].Left < player1.Left - 3)
                        {
                            e1X[i] = 15;
                            enemyS1[i].X += e1X[i];
                        }
                        else if (enemyS1[i].Left > player1.Left + 3)
                        {
                            e1X[i] = -15;
                            enemyS1[i].X += e1X[i];
                        }

                        if (enemyS1[i].Top < player1.Top - 3)
                        {
                            e1Y[i] = 15;
                            enemyS1[i].Y += e1Y[i];
                        }
                        else if (enemyS1[i].Top > player1.Top + 3)
                        {
                            e1Y[i] = -15;
                            enemyS1[i].Y += e1Y[i];
                        }
                    
                }

                if (enemyS2[i].Y > -7)
                { //enemy2 movement
                    
                        if (enemyS2[i].Left < player1.Left - 3)
                        {
                            e2X[i] = 7;
                            enemyS2[i].X += e2X[i];
                        }
                        else if (enemyS2[i].Left > player1.Left + 3)
                        {
                            e2X[i] = -7;
                            enemyS2[i].X += e2X[i];
                        }

                        if (enemyS2[i].Top < player1.Top - 3)
                        {
                            e2Y[i] = 7;
                            enemyS2[i].Y += e2Y[i];
                        }
                        else if (enemyS2[i].Top > player1.Top + 3)
                        {
                            e2Y[i] = -7;
                            enemyS2[i].Y += e2Y[i];
                        }
                   
                }

                if (enemyS3[i].Y > -7)
                { //enemy3 movement
                    
                        if (enemyS3[i].Left < player1.Left - 3)
                        {
                            e3X[i] = 5;
                            enemyS3[i].X += e3X[i];
                        }
                        else if (enemyS3[i].Left > player1.Left + 3)
                        {
                            e3X[i] = -5;
                            enemyS3[i].X += e3X[i];
                        }

                        if (enemyS3[i].Top < player1.Top - 3)
                        {
                            e3Y[i] = 5;
                            enemyS3[i].Y += e3Y[i];
                        }
                        else if (enemyS3[i].Top > player1.Top + 3)
                        {
                            e3Y[i] = -5;
                            enemyS3[i].Y += e3Y[i];
                        }
                    
                }
            }

            for (int i = 0; i < enemyBoss.Length; i++)
            {
                if (enemyBoss[i].Y > -7)
                { //enemyBoss movement
                   
                        if (enemyBoss[i].Left < player1.Left - 3)
                        {
                            bX[i] = 3;
                            enemyBoss[i].X += bX[i];
                        }
                        else if (enemyBoss[i].Left > player1.Left + 3)
                        {
                            bX[i] = -3;
                            enemyBoss[i].X += bX[i];
                        }

                        if (enemyBoss[i].Top < player1.Top - 3)
                        {
                            bY[i] = 3;
                            enemyBoss[i].Y += bY[i];
                        }
                        else if (enemyBoss[i].Top > player1.Top + 3)
                        {
                            bY[i] = -3;
                            enemyBoss[i].Y += bY[i];
                        }
                    
                }
            }
            if (p1Health == 0)
            {
                // write the player 1 and 2 scores to a text file
                P1scoreReader = new StreamReader(@"P1Score.txt");
                if (File.Exists(@"P1Score.txt"))
                {
                    P1tempScore = P1scoreReader.ReadLine();
                }
                P1scoreReader.Close();

                if (p1score > Convert.ToInt32(P1tempScore))
                {
                    P1scoreWriter = new StreamWriter(@"P1Score.txt", false);
                    P1scoreWriter.WriteLine(p1score);
                    P1scoreWriter.Close();
                }
                
                gameover = true;
                gameOverForm = new GameOver_form();
                gameOverForm.Show();
                this.Close();
            }
        }

        void refreshMiscT_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (rand.Next(10000) < 20 && enemyS1[i].Bottom < 0)
                {
                    enemyS1[i].Y = 0;
                    enemyS1[i].X = (this.ClientSize.Width / 2);
                }
                
                if (rand.Next(10000) < 20 && enemyS2[i].Bottom < 0)
                {
                    enemyS2[i].Y = (this.ClientSize.Height / 2);
                    enemyS2[i].X = 0;
                }
                
                if (rand.Next(10000) < 20 && enemyS3[i].Bottom < 0)
                {
                    enemyS3[i].Y = this.ClientSize.Height;
                    enemyS3[i].X = (this.ClientSize.Width / 2);
                }
            }

            for (int i = 0; i < enemyBoss.Length; i++)
            {
                if (rand.Next(10000) < 20 && enemyBoss[i].Bottom < 0)
                {
                    enemyBoss[i].X = this.ClientSize.Width;
                    enemyBoss[i].Y = (this.ClientSize.Height / 2);
                }
            }

            if (p1Health <= 0)
            {
                refreshMiscT.Stop();
                movementT.Stop();
                intersectTimer.Stop();
            }

            if (healthPowerUp.Bottom < 0 && rand.Next(10000) < 10)
            {
                do
                {
                    healthPowerUp.Location = new Point(rand.Next(this.ClientSize.Width), rand.Next(this.ClientSize.Height));
                } while (healthPowerUp.IntersectsWith(core));
            }

            this.Refresh();
        }

        void gameform2_KeyDown(object sender, KeyEventArgs e)
        {

            //P1 movement
            if (e.KeyCode == Keys.A)
            { //upon P1 pressing 'A' or 'D' movement will be set to 5 pixels
                p1X = -5;
                p1DirectionFacing = "left";
            }
            else if (e.KeyCode == Keys.D)
            {
                p1X = 5;
                p1DirectionFacing = "right";
            }
            else if (e.KeyCode == Keys.W)
            { //upon P1 pressing 'W' or 'S' movement will be set to 5 pixels
                p1Y = -5;
                p1DirectionFacing = "up";
            }
            else if (e.KeyCode == Keys.S)
            {
                p1Y = 5;
                p1DirectionFacing = "down";
            }

            if (e.KeyCode == Keys.Space)
            {
                if (p1ShotNum >= 10 && p1Shot[0].Bottom < 0)
                {
                    p1ShotNum = 0;
                }

                if (p1ShotNum < 10 && p1Shot[p1ShotNum].Bottom < 0)
                {
                    //moves the gun shot to the middle of the player
                    p1Shot[p1ShotNum].X = player1.Left + (player1.Width / 2) - (p1Shot[0].Width / 2);
                    p1Shot[p1ShotNum].Y = player1.Top + (player1.Height / 2) - (p1Shot[0].Height / 2);

                    if (p1X != 0 && p1Y != 0)
                    {
                        p1ShotX[p1ShotNum] = p1X * 2;
                        p1ShotY[p1ShotNum] = p1Y * 2;
                    }
                    else
                    {
                        if (p1DirectionFacing == "left")
                        {
                            p1ShotX[p1ShotNum] = -10;
                        }
                        else if (p1DirectionFacing == "right")
                        {
                            p1ShotX[p1ShotNum] = 10;
                        }
                        else if (p1DirectionFacing == "up")
                        {
                            p1ShotY[p1ShotNum] = -10;
                        }
                        else if (p1DirectionFacing == "down")
                        {
                            p1ShotY[p1ShotNum] = 10;
                        }
                    }

                    p1ShotNum++;
                }
            }
        }

        void gameform2_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!gameover)
            {
                MenuForm.showform = true;
            }
        }

        private void canP1Move()
        { //checks to see if the player has intersected with the sides or any rectangle *sets p1Move to true to allow movement *0 = ←→, 1 = ↑↓
            if (p1X < 0 && player1.Left > 0 || p1X > 0 && player1.Right < this.ClientSize.Width)
            {
                p1Move[0] = true;
            }
            else
            {
                p1Move[0] = false;
            }

            if (p1Y < 0 && player1.Top > 0 || p1Y > 0 && player1.Bottom < this.ClientSize.Height)
            {
                p1Move[1] = true;
            }
            else
            {
                p1Move[1] = false;
            }

            if (player1.IntersectsWith(core) && p1X > 0 && player1.Bottom > (core.Top + 6) && player1.Top < (core.Bottom - 6) && player1.Right <= coreMiddleX) //+/-6 is to allow player to move at top/bottom of core
            { //these next few will check for intersection with the middle circle and the player *if headed at middle p#Move = false
                p1Move[0] = false;
            }
            else if (player1.IntersectsWith(core) && p1X < 0 && player1.Bottom > (core.Top + 6) && player1.Top < (core.Bottom - 6) && player1.Left > coreMiddleX)
            {
                p1Move[0] = false;
            }
            else if (player1.IntersectsWith(core) && p1Y > 0 && player1.Right > (core.Left + 6) && player1.Left < (core.Right - 6) && player1.Bottom <= coreMiddleY)
            {
                p1Move[1] = false;
            }
            else if (player1.IntersectsWith(core) && p1Y < 0 && player1.Right > (core.Left + 6) && player1.Left < (core.Right - 6) && player1.Top > coreMiddleY)
            {
                p1Move[1] = false;
            }
        }
    }
}
