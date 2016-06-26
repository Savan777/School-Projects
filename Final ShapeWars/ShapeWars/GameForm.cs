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
{ //Lines: 1173
    public partial class GameForm : Form
    {
        //variables
        int maxHealth = 20; //could be used for upgrades/allows for easy switching of values
        int p1score = 0; //holds score
        int p2score = 0;
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
        Rectangle player2;
        int p2Health;
        int p2X;
        int p2Y;
        bool[] p2Move; //used to check whether the player can move
        string p2DirectionFacing = "down"; //used to check direction
        ProgressBar p2HealthBar;
        Label p2HealthDisplay;
        Rectangle[] p2Shot; //rect for p2's gun
        int[] p2ShotX;
        int[] p2ShotY;
        int p2ShotNum = 0; //keeps track of which rect is selected
        Rectangle[] enemyS1; //lvl 1 enemy
        int[] e1X;
        int[] e1Y;
        int[] e1Distance1; //used to keep the distance from the player
        int[] e1Distance2; //used to keep the distance from the player
        Rectangle[] enemyS2; //lvl 2 enemy
        int[] e2Health;
        int[] e2X;
        int[] e2Y;
        int[] e2Distance1; //used to keep the distance from the player
        int[] e2Distance2; //used to keep the distance from the player
        Rectangle[] enemyS3; //lvl 3 enemy
        int[] e3Health;
        int[] e3X;
        int[] e3Y;
        int[] e3Distance1; //used to keep the distance from the player
        int[] e3Distance2; //used to keep the distance from the player
        Rectangle[] enemyBoss; //Boss
        int[] eBHealth;
        int[] bX;
        int[] bY;
        int[] bDistance1; //used to keep the distance from the player
        int[] bDistance2; //used to keep the distance from the player

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
        StreamWriter P2scoreWriter;
        StreamReader P1scoreReader;
        StreamReader P2scoreReader;

        string P1tempScore;
        string P2tempScore;

        public GameForm()
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
            p2Move = new bool[2]; //0 = horizontal movement, 1 = vertical movement

            p1Shot = new Rectangle[10];
            p1ShotX = new int[p1Shot.Length];
            p1ShotY = new int[p1Shot.Length];
            p2Shot = new Rectangle[10];
            p2ShotX = new int[p2Shot.Length];
            p2ShotY = new int[p2Shot.Length];
            for (int i = 0; i < p1Shot.Length; i++)
            {
                p1ShotY[i] = -10;
                p2ShotY[i] = 10;
            }

            //enemy arrays
            enemyS1 = new Rectangle[5];
            e1X = new int[enemyS1.Length];
            e1Y = new int[enemyS1.Length];
            e1Distance1 = new int[enemyS1.Length];
            e1Distance2 = new int[enemyS1.Length];
            for (int i = 0; i < e1X.Length; i++)
            {
                e1X[i] = 7;
                e1Y[i] = 0;
            }

            enemyS2 = new Rectangle[5];
            e2Health = new int[enemyS2.Length];
            e2X = new int[enemyS2.Length];
            e2Y = new int[enemyS2.Length];
            e2Distance1 = new int[enemyS2.Length];
            e2Distance2 = new int[enemyS2.Length];
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
            e3Distance1 = new int[enemyS3.Length];
            e3Distance2 = new int[enemyS3.Length];
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
            bDistance1 = new int[enemyBoss.Length];
            bDistance2 = new int[enemyBoss.Length];
            for (int i = 0; i < bX.Length; i++)
            {
                bX[i] = 7;
                bY[i] = 0;
                eBHealth[i] = 8;
            }
        }

        private void GameForm_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            this.MaximizeBox = false;

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
            scoreDisplay.Text = "P1 Score: " + p1score + "\nP2 Score: " + p2score;
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

            player2 = new Rectangle();
            player2.Size = new Size(45, 45);
            p2Health = maxHealth;
            p2HealthDisplay = new Label();
            p2HealthDisplay.Size = new System.Drawing.Size(player2.Width, 5);
            p2HealthBar = new ProgressBar();
            p2HealthBar.Size = p2HealthDisplay.Size;
            p2HealthBar.Location = new Point(0, 0);
            p2HealthBar.Maximum = p2Health;
            p2HealthBar.Minimum = 0;
            p2HealthBar.Value = p2Health;
            this.Controls.Add(p2HealthDisplay);
            p2HealthDisplay.Controls.Add(p2HealthBar);

            player1.Location = new Point((this.ClientSize.Width / 2) - (player1.Width / 2), 100);
            p1HealthDisplay.Location = new Point(player1.Left, player1.Bottom);
            player2.Location = new Point((this.ClientSize.Width / 2) - (player1.Width / 2), (this.ClientSize.Height - player2.Height - 100));
            p2HealthDisplay.Location = new Point(player2.Left, player2.Bottom);

            for (int i = 0; i < p1Shot.Length; i++)
            {
                p1Shot[i].Size = new Size(playerShotPic.Width, playerShotPic.Height);
                p1Shot[i].Location = new Point(5, -500); //offscreen
                p2Shot[i].Size = new Size(playerShotPic.Width, playerShotPic.Height);
                p2Shot[i].Location = new Point(5, -500);
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

            //Form events
            this.KeyDown += GameForm_KeyDown;
            this.KeyUp += GameForm_KeyUp;

            this.DoubleBuffered = true;
            this.Paint += new PaintEventHandler(GameForm_Paint);
            this.FormClosing += new FormClosingEventHandler(GameForm_FormClosing);
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

            if (healthPowerUp.Y >= 0 && player2.IntersectsWith(healthPowerUp))
            {
                p2HealthBar.Step = 5;
                p2Health += 5;
                p2HealthBar.PerformStep();
                healthPowerUp.Location = new Point(5, -500);
                if (p2Health > 20)
                {
                    p2Health = 20;
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

                    //p2Shot
                    if (p2Shot[i].Bottom > 0 && p2Shot[i].IntersectsWith(enemyS1[j]))
                    {
                        p2Shot[i].Location = new Point(5, -500);
                        enemyS1[j].Location = new Point(5, -500);
                        p2score += 10;
                    }
                    else if (p2Shot[i].Bottom > 0 && p2Shot[i].IntersectsWith(enemyS2[j]))
                    {
                        e2Health[j]--;
                        p2Shot[i].Location = new Point(5, -500);
                        if (e2Health[j] <= 0)
                        {
                            enemyS2[j].Location = new Point(5, -500);
                            p2score += 15;
                            e2Health[j] = 2;
                        }
                    }
                    else if (p2Shot[i].Bottom > 0 && p2Shot[i].IntersectsWith(enemyS3[j]))
                    {
                        e3Health[j]--;
                        p2Shot[i].Location = new Point(5, -500);
                        if (e3Health[j] <= 0)
                        {
                            enemyS3[j].Location = new Point(5, -500);
                            p2score += 20;
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
                    else if (p2Shot[i].Bottom > 0 && p2Shot[i].IntersectsWith(enemyBoss[j]))
                    {
                        eBHealth[j]--;
                        p2Shot[i].Location = new Point(5, -500);
                        if (eBHealth[j] <= 0)
                        {
                            enemyBoss[j].Location = new Point(5, -500);
                            p2score += 30;
                            eBHealth[j] = 8;
                        }
                    }
                }

                scoreDisplay.Text = "P1 Score: " + p1score + "\nP2 Score: " + p2score;
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
                
                if (player2.Bottom > 0 && player2.IntersectsWith(enemyS1[i]))
                {
                    p2Health--;
                    p2HealthBar.Step = -1;
                    p2HealthBar.PerformStep();
                    enemyS1[i].Location = new Point(5, -500);
                    if (p2Health <= 0)
                    {
                        player2.Location = new Point(5, -500);
                    }
                }
                else if (player2.Bottom > 0 && player2.IntersectsWith(enemyS2[i]))
                {
                    p2Health--;
                    p2HealthBar.Step = -1;
                    p2HealthBar.PerformStep();
                    enemyS2[i].Location = new Point(5, -500);
                    if (p2Health <= 0)
                    {
                        player2.Location = new Point(5, -500);
                    }
                }
                else if (player2.Bottom > 0 && player2.IntersectsWith(enemyS3[i]))
                {
                    p2Health--;
                    p2HealthBar.Step = -1;
                    p2HealthBar.PerformStep();
                    enemyS3[i].Location = new Point(5, -500);
                    if (p2Health <= 0)
                    {
                        player2.Location = new Point(5, -500);
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
                else if (player2.Bottom > 0 && player2.IntersectsWith(enemyBoss[i]))
                {
                    p2Health--;
                    p2HealthBar.Step = -1;
                    p2HealthBar.PerformStep();
                    enemyBoss[i].Location = new Point(5, -500);
                    if (p2Health <= 0)
                    {
                        player2.Location = new Point(5, -500);
                    }
                }
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

            if (p1Health <= 0 && p2Health <= 0)
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

        void movementT_Tick(object sender, EventArgs e)
        { //*0 = ←→, 1 = ↑↓
            canP1Move();
            canP2Move();

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

            //p2 movement
            if (p2Move[0])
            {
                player2.X += p2X;
            }

            if (p2Move[1])
            {
                player2.Y += p2Y;
            }
            p2HealthDisplay.Location = new Point(player2.Left, player2.Bottom);

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

                if (p2Shot[i].Y > 0)
                {
                    p2Shot[i].X += p2ShotX[i];
                    p2Shot[i].Y += p2ShotY[i];
                }

                if (p2Shot[i].Left < 0 || p2Shot[i].Right > this.ClientSize.Width || p2Shot[i].Top < 0 || p2Shot[i].Bottom > this.ClientSize.Height)
                {
                    p2Shot[i].Location = new Point(5, -500);
                    if (p2DirectionFacing == "left")
                    {
                        p2ShotX[i] = -10;
                        p2ShotY[i] = 0;
                    }
                    else if (p2DirectionFacing == "right")
                    {
                        p2ShotX[i] = 10;
                        p2ShotY[i] = 0;
                    }
                    else if (p2DirectionFacing == "up")
                    {
                        p2ShotX[i] = 0;
                        p2ShotY[i] = -10;
                    }
                    else if (p2DirectionFacing == "down")
                    {
                        p2ShotX[i] = 0;
                        p2ShotY[i] = 10;
                    }
                }
            }

            enemyDistance();

            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (enemyS1[i].Y > -7)
                { //enemy1 movement
                    if (e1Distance1[i] <= e1Distance2[i])
                    {
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
                    else
                    {
                        if (enemyS1[i].Left < player2.Left - 3)
                        {
                            e1X[i] = 15;
                            enemyS1[i].X += e1X[i];
                        }
                        else if (enemyS1[i].Left > player2.Left + 3)
                        {
                            e1X[i] = -15;
                            enemyS1[i].X += e1X[i];
                        }

                        if (enemyS1[i].Top < player2.Top - 3)
                        {
                            e1Y[i] = 15;
                            enemyS1[i].Y += e1Y[i];
                        }
                        else if (enemyS1[i].Top > player2.Top + 3)
                        {
                            e1Y[i] = -15;
                            enemyS1[i].Y += e1Y[i];
                        }
                    }
                }

                if (enemyS2[i].Y > -7)
                { //enemy2 movement
                    if (e2Distance1[i] <= e2Distance2[i])
                    {
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
                    else
                    {
                        if (enemyS2[i].Left < player2.Left - 3)
                        {
                            e2X[i] = 7;
                            enemyS2[i].X += e2X[i];
                        }
                        else if (enemyS2[i].Left > player2.Left + 3)
                        {
                            e2X[i] = -7;
                            enemyS2[i].X += e2X[i];
                        }

                        if (enemyS2[i].Top < player2.Top - 3)
                        {
                            e2Y[i] = 7;
                            enemyS2[i].Y += e2Y[i];
                        }
                        else if (enemyS2[i].Top > player2.Top + 3)
                        {
                            e2Y[i] = -7;
                            enemyS2[i].Y += e2Y[i];
                        }
                    }
                }

                if (enemyS3[i].Y > -7)
                { //enemy3 movement
                    if (e3Distance1[i] <= e3Distance2[i])
                    {
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
                    else
                    {
                        if (enemyS3[i].Left < player2.Left - 3)
                        {
                            e3X[i] = 5;
                            enemyS3[i].X += e3X[i];
                        }
                        else if (enemyS3[i].Left > player2.Left + 3)
                        {
                            e3X[i] = -5;
                            enemyS3[i].X += e3X[i];
                        }

                        if (enemyS3[i].Top < player2.Top - 3)
                        {
                            e3Y[i] = 5;
                            enemyS3[i].Y += e3Y[i];
                        }
                        else if (enemyS3[i].Top > player2.Top + 3)
                        {
                            e3Y[i] = -5;
                            enemyS3[i].Y += e3Y[i];
                        }
                    }
                }
            }

            for (int i = 0; i < enemyBoss.Length; i++)
            {
                if (enemyBoss[i].Y > -7)
                { //enemyBoss movement
                    if (bDistance1[i] <= bDistance2[i])
                    {
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
                    else
                    {
                        if (enemyBoss[i].Left < player2.Left - 3)
                        {
                            bX[i] = 3;
                            enemyBoss[i].X += bX[i];
                        }
                        else if (enemyBoss[i].Left > player2.Left + 3)
                        {
                            bX[i] = -3;
                            enemyBoss[i].X += bX[i];
                        }

                        if (enemyBoss[i].Top < player2.Top - 3)
                        {
                            bY[i] = 3;
                            enemyBoss[i].Y += bY[i];
                        }
                        else if (enemyBoss[i].Top > player2.Top + 3)
                        {
                            bY[i] = -3;
                            enemyBoss[i].Y += bY[i];
                        }
                    }
                }
            }
            if (p1Health == 0 && p2Health == 0)
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

                P2scoreReader = new StreamReader(@"P2Score.txt");
                if (File.Exists(@"P2Score.txt"))
                {
                    P2tempScore = P2scoreReader.ReadLine();
                }
                P2scoreReader.Close();

                if (p2score > Convert.ToInt32(P2tempScore))
                {
                    P2scoreWriter = new StreamWriter(@"P2Score.txt", false);
                    P2scoreWriter.WriteLine(p2score);
                    P2scoreWriter.Close();
                }
                
                gameover = true;
                gameOverForm = new GameOver_form();
                gameOverForm.Show();
                this.Close();
            }
        }

        void GameForm_KeyDown(object sender, KeyEventArgs e)
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

            //P2 movement
            if (e.KeyCode == Keys.Left)
            { //upon P1 pressing '←' or '→' movement will be set to 5 pixels
                p2X = -5;
                p2DirectionFacing = "left";
            }
            else if (e.KeyCode == Keys.Right)
            {
                p2X = 5;
                p2DirectionFacing = "right";
            }
            else if (e.KeyCode == Keys.Up)
            { //upon P2 pressing '↑' or '↓' movement will be set to 5 pixels
                p2Y = -5;
                p2DirectionFacing = "up";
            }
            else if (e.KeyCode == Keys.Down)
            {
                p2Y = 5;
                p2DirectionFacing = "down";
            }

            if (e.KeyCode == Keys.Enter)
            {
                if (p2ShotNum >= 10 && p2Shot[0].Bottom < 0)
                {
                    p2ShotNum = 0;
                }

                if (p2ShotNum < 10 && p2Shot[p2ShotNum].Bottom < 0)
                {
                //moves the gun shot to the middle of the player
                    p2Shot[p2ShotNum].X = player2.Left + (player2.Width / 2) - (p2Shot[0].Width / 2);
                    p2Shot[p2ShotNum].Y = player2.Top + (player2.Height / 2) - (p2Shot[0].Height / 2);

                    if (p2X != 0 && p2Y != 0)
                    {
                        p2ShotX[p2ShotNum] = p2X * 2;
                        p2ShotY[p2ShotNum] = p2Y * 2;
                    }
                    else
                    {
                        if (p2DirectionFacing == "left")
                        {
                            p2ShotX[p2ShotNum] = -10;
                        }
                        else if (p2DirectionFacing == "right")
                        {
                            p2ShotX[p2ShotNum] = 10;
                        }
                        else if (p2DirectionFacing == "up")
                        {
                            p2ShotY[p2ShotNum] = -10;
                        }
                        else if (p2DirectionFacing == "down")
                        {
                            p2ShotY[p2ShotNum] = 10;
                        }
                    }

                    p2ShotNum++;
                }
            }
        }

        void GameForm_KeyUp(object sender, KeyEventArgs e)
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

            //P2 movement
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
            { //upon P1 pressing '←' or '→' movement will be set to 0 pixels
                p2X = 0;
            }

            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
            { //upon P2 pressing '↑' or '↓' movement will be set to 0 pixels
                p2Y = 0;
            }
        }

        // checks which form is on when current form is closed 
        void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!gameover)
            {
                MenuForm.showform = true;
            }
        }

        void GameForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillPolygon(Brushes.Cyan, triangle0);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle1);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle2);
            e.Graphics.FillPolygon(Brushes.Cyan, triangle3);
            e.Graphics.FillEllipse(Brushes.Blue, core);
            e.Graphics.FillRectangle(Brushes.Orange, player1);
            e.Graphics.FillRectangle(Brushes.Green, player2);
            for (int i = 0; i < p1Shot.Length; i++)
            {
                e.Graphics.DrawImage(playerShotPic, p1Shot[i]);
                e.Graphics.DrawImage(playerShotPic, p2Shot[i]);
            }
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS1);
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS2);
            e.Graphics.FillRectangles(Brushes.DarkSlateGray, enemyS3);
            e.Graphics.FillRectangles(Brushes.Black, enemyBoss);

            e.Graphics.DrawImage(healthPowerUpPic, healthPowerUp);
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

        private void canP2Move()
        { //checks to see if the player has intersected with the sides or any rectangle *sets p2Move to true to allow movement
            if (p2X < 0 && player2.Left > 0 || p2X > 0 && player2.Right < this.ClientSize.Width)
            {
                p2Move[0] = true;
            }
            else
            {
                p2Move[0] = false;
            }

            if (p2Y < 0 && player2.Top > 0 || p2Y > 0 && player2.Bottom < this.ClientSize.Height)
            {
                p2Move[1] = true;
            }
            else
            {
                p2Move[1] = false;
            }

            if (player2.IntersectsWith(core) && p2X > 0 && player2.Bottom > (core.Top + 6) && player2.Top < (core.Bottom - 6) && player2.Right <= coreMiddleX) //+/-6 is to allow player to move at left/right of core
            { //these next few will check for intersection with the middle circle and the player *if headed at middle p#Move = false
                p2Move[0] = false;
            }
            else if (player2.IntersectsWith(core) && p2X < 0 && player2.Bottom > (core.Top + 6) && player2.Top < (core.Bottom - 6) && player2.Left > coreMiddleX)
            {
                p2Move[0] = false;
            }
            else if (player2.IntersectsWith(core) && p2Y > 0 && player2.Right > (core.Left + 6) && player2.Left < (core.Right - 6) && player2.Bottom < coreMiddleY)
            {
                p2Move[1] = false;
            }
            else if (player2.IntersectsWith(core) && p2Y < 0 && player2.Right > (core.Left + 6) && player2.Left < (core.Right - 6) && player2.Top > coreMiddleY)
            {
                p2Move[1] = false;
            }
        }

        private void enemyDistance()
        {
            //gets the distance of enemy 1 to player one and two
            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (p1Health > 0)
                { //get distance of player as long as they are alive
                    e1Distance1[i] = Convert.ToInt32(Math.Abs(player1.Left - enemyS1[i].Left) + Math.Abs(player1.Top - enemyS1[i].Top));
                }
                else
                { //else set the distance to a super high number so th enemies follow the other player
                    e1Distance1[i] = 100000;
                }

                if (p2Health > 0)
                {
                    e1Distance2[i] = Convert.ToInt32(Math.Abs(player2.Left - enemyS1[i].Left) + Math.Abs(player2.Top - enemyS1[i].Top));
                }
                else
                {
                    e1Distance2[i] = 100000;
                }
            }

            //gets the distance of enemy 2 to player one and two
            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (p1Health > 0)
                {
                    e2Distance1[i] = Convert.ToInt32(Math.Abs(player1.Left - enemyS2[i].Left) + Math.Abs(player1.Top - enemyS2[i].Top));
                }
                else
                {
                    e2Distance1[i] = 100000;
                }

                if (p2Health > 0)
                {
                    e2Distance2[i] = Convert.ToInt32(Math.Abs(player2.Left - enemyS2[i].Left) + Math.Abs(player2.Top - enemyS2[i].Top));
                }
                else
                {
                    e2Distance2[i] = 100000;
                }
            }

            //gets the distance of enemy 3 to player one and two
            for (int i = 0; i < enemyS1.Length; i++)
            {
                if (p1Health > 0)
                {
                    e3Distance1[i] = Convert.ToInt32(Math.Abs(player1.Left - enemyS3[i].Left) + Math.Abs(player1.Top - enemyS3[i].Top));
                }
                else
                {
                    e3Distance1[i] = 100000;
                }

                if (p2Health > 0)
                {
                    e3Distance2[i] = Convert.ToInt32(Math.Abs(player2.Left - enemyS3[i].Left) + Math.Abs(player2.Top - enemyS3[i].Top));
                }
                else
                {
                    e3Distance2[i] = 100000;
                }
            }

            //gets the distance of boss to player one and two
            for (int i = 0; i < enemyBoss.Length; i++)
            {
                if (p1Health > 0)
                {
                    bDistance1[i] = Convert.ToInt32(Math.Abs(player1.Left - enemyBoss[i].Left) + Math.Abs(player1.Top - enemyBoss[i].Top));
                }
                else
                {
                    bDistance1[i] = 100000;
                }

                if (p2Health > 0)
                {
                    bDistance2[i] = Convert.ToInt32(Math.Abs(player2.Left - enemyBoss[i].Left) + Math.Abs(player2.Top - enemyBoss[i].Top));
                }
                else
                {
                    bDistance2[i] = 100000;
                }
            }
        }
    }
}