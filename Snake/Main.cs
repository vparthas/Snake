using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake
{
    public partial class Main : Form
    {
        private static readonly int WIDTH = 32, HEIGHT = 24, BOX_DIMEN = 25, BOX_MARGIN = 1;

        private Panel panel;

        private GameState gameState;
        private bool paused;
        private Snake game;
        private Direction latestMove;
        private int score;

        public Main()
        {
            InitializeComponent();

            //reset members
            init();

            //set window title
            this.Text = "Snake";

            //set the window inner dimensions and center it
            this.ClientSize = new Size(800, 600);
            this.CenterToScreen();

            //Make the window non-resizeable
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //Set the keyboard input listener
            this.KeyDown += Main_KeyDown;

            //Setup panel for graphics
            panel = new Panel();
            panel.Name = "panel"; //specify control name, to access it in other parts of code
            panel.Location = new Point(0, 0);
            panel.Size = new Size(800, 600);
            panel.BackColor = Color.White;
            panel.Paint += Paint;
            Controls.Add(panel);
        }

        private void init()
        {
            gameState = GameState.MENU;
            latestMove = Direction.RIGHT;
        }

        //Draw to screen
        private void Paint(object sender, PaintEventArgs e)
        {
            if (gameState == GameState.MENU)
                DrawMenu(e);
            else if (gameState == GameState.GAME)
                DrawGame(e);
            else if (gameState == GameState.END)
                DrawGameOver(e); //TODO
        }

        private void DrawGameOver(PaintEventArgs paint)
        {
            Font myFont = new Font("Arial", 24);
            paint.Graphics.DrawString("Game Over", myFont, Brushes.Blue, 50, 100);

            myFont = new Font("Arial", 14);
            paint.Graphics.DrawString("Your score: " + game.getLength(), myFont, Brushes.Green, 50, 150);

            myFont = new Font("Arial", 14);
            paint.Graphics.DrawString("Press any key to play again.", myFont, Brushes.Red, 50, 200);
        }

        //Draw the menu screen
        private void DrawMenu(PaintEventArgs paint)
        {
            Font myFont = new Font("Arial", 24);
            paint.Graphics.DrawString("Snake", myFont, Brushes.Blue, 50, 100);

            myFont = new Font("Arial", 14);
            paint.Graphics.DrawString("Press any key to play. (Arrows or WASD to move, Space or P to pause)", myFont, Brushes.Red, 50, 150);
        }

        //Draw the game screen
        private void DrawGame(PaintEventArgs paint)
        {
            paint.Graphics.Clear(Color.White);

            if(paused)
            {
                Font myFont = new Font("Arial", 24);
                paint.Graphics.DrawString("Paused", myFont, Brushes.Blue, 50, 100);

                myFont = new Font("Arial", 14);
                paint.Graphics.DrawString("Press Space or P to resume.", myFont, Brushes.Red, 50, 150);

                return;
            }

            drawBox(paint, game.getFood().X, game.getFood().Y, Brushes.Blue);

            foreach (Point p in game.getBody())
                drawBox(paint, p.X, p.Y, Brushes.Red);
        }

        //Draw a box space on the grid
        private void drawBox(PaintEventArgs paint, int x, int y, Brush brush)
        {
            int px = (x * BOX_DIMEN) + BOX_MARGIN;
            int py = (y * BOX_DIMEN) + BOX_MARGIN;
            int dimen = BOX_DIMEN - (2 * BOX_MARGIN);
            paint.Graphics.FillRectangle(brush, px, py, dimen, dimen);
        }

        //Handle keyboard input
        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState == GameState.MENU || gameState == GameState.END)
                startGame();

            else if (gameState == GameState.GAME)
                processGameInput(sender, e);
        }

        //process input events when in game
        private void processGameInput(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Space || e.KeyData == Keys.P)
                pauseControl();

            else if (e.KeyData == Keys.Up || e.KeyData == Keys.W)
                move(Direction.UP);
            else if (e.KeyData == Keys.Down || e.KeyData == Keys.S)
                move(Direction.DOWN);
            else if (e.KeyData == Keys.Left || e.KeyData == Keys.A)
                move(Direction.LEFT);
            else if (e.KeyData == Keys.Right || e.KeyData == Keys.D)
                move(Direction.RIGHT);
        }

        private void move(Direction direction)
        {
            latestMove = direction;
        }

        //start the game
        private void startGame()
        {
            paused = false;
            game = new Snake(WIDTH, HEIGHT);
            gameState = GameState.GAME;

            Thread gameThread = new Thread(this.gameLoop);
            gameThread.Start();
        }

        private void gameLoop()
        {
            //Game loop
            while (!game.isDead())
            {
                int sleepTime = getSleepTime(0);

                this.Invoke((MethodInvoker)delegate
                {
                    if(!paused)
                    {
                        game.move(latestMove);
                        updateScore(game.getLength());

                        sleepTime = getSleepTime(game.getLength());
                    }

                    this.Refresh(); 
                });

                Thread.Sleep(sleepTime);
            }
            
            this.Invoke((MethodInvoker)delegate
            {
                gameState = GameState.END;
                this.Refresh();
            });
        }

        private int getSleepTime(int length)
        {
            double f = 300.0 / (1 + (length / 10.0));
            return (int)f;
        }

        private void updateScore(int score)
        {
            this.score = score;
            this.Text = "Length: " + score;
        }

        //Handle puse controls
        private void pauseControl()
        {
            paused = !paused;
        }
    }

    public enum GameState
    {
        MENU,
        GAME,
        END
    }
}
