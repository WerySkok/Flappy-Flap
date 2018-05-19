using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DrawGame2FlappyBird
{
    public partial class Form1 : Form
    {
        public Form1()
        {

            InitializeComponent();
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you realy want to exist?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Stop);
            if (result == DialogResult.Yes)
                Application.Exit();
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            label1.Text = "Your name?";
            playButton.Visible = false;
            lbButton.Visible = false;

            textBox1.Visible = true;
            flapButton.Visible = true;
        }

        private void flapButton_Click(object sender, EventArgs e)
        {
            PlayerName = textBox1.Text.Trim();
            if (PlayerName != "")
            {
                textBox1.Visible = false;
                label1.Visible = false;
                flapButton.Visible = false;
                exitButton.Visible = false;
                timer1.Enabled = true;
                this.Focus();
            }
            else
            {
                MessageBox.Show("Please, write your name", "Player", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        
        // Задаём переменные
        string PlayerName = ""; // имя игрока
        bool isOver = false;
        int Score = 0;
        /*
          -=стандартное=-
        */
        Bitmap b;
        Graphics g;
        Random r = new Random(); 
        /*
          -=Трава=-
        */
        SolidBrush GrassBrush; //кисть 
        Rectangle grassRect;   //форма 
        int grassH = 50;       //высота 
        /*
          -=Птица=-
        */
        SolidBrush BirdBrush;           //кисть героя
        Rectangle Bird;                 //форма героя
        int BirdW = 49, BirdH = 42;    //высота, ширина героя
        double gravity = 3;
        int jump = 6;
        int isJump;
        Image birdSprite = Image.FromFile("Bird_01.png");
        int SpriteTick = 0, ChangeTick = 10, Sprite_n=1;
        /*
          -=трубы=-         
        */
        List<Tube> Tubes = new List<Tube>();
        int tick = 0, newTubeInterval = 350;
        int TubeD = 100;
        int HoleSize = 100;

        private void Form1_Load(object sender, EventArgs e)
        {
            //инициализация
            b = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            g = Graphics.FromImage(b);
            pictureBox1.Image = b;
            GrassBrush = new SolidBrush(Color.ForestGreen);
            grassRect = new Rectangle(0, pictureBox1.Height - grassH, pictureBox1.Width, grassH);
            Bird = new Rectangle(100, (pictureBox1.Height - grassH)/2, BirdW, BirdH);
            BirdBrush = new SolidBrush(Color.Yellow);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Background();
            BirdLogic();
            TubeLogic();
            UIDraw();
            GameOver();
            pictureBox1.Invalidate();
        }

        public void Background() //отрисовка фона
        {
            g.Clear(Color.SkyBlue);
            g.FillRectangle(GrassBrush, grassRect);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
            {
                Application.Exit();
            }
            if (e.KeyData == Keys.P)
            {
                if (!timer1.Enabled)
                {
                    timer1.Enabled = true;
                }
                else if (timer1.Enabled)
                {
                    timer1.Enabled = false;
                    g.DrawString("Пауза", new Font("Segoe UI Bold", 30), Brushes.White, new Point(pictureBox1.Width / 3, pictureBox1.Height / 3));
                    pictureBox1.Invalidate();
                }
            }
            if (e.KeyData == Keys.Space)
            {
                isJump = 1;
            }
        }

        public void BirdLogic() //отрисовка героя
        {
            Bird.Y += (int)gravity;
            if (Bird.IntersectsWith(grassRect))
            {
                isOver = true;
            }
            gravity += 0.1;
            if (isJump>0)
            {
                gravity = 3;
                if (Bird.Y-jump+gravity>0)
                {
                    Bird.Y -= jump;
                }
                isJump++;
            }
            if (isJump>=10)
            {
                isJump = 0;
            }
            SpriteTick++;
            if (SpriteTick>=ChangeTick&&Sprite_n==1)
            {
                Sprite_n = 2;
                birdSprite = Image.FromFile("Bird_02.png");
                SpriteTick = 0;
            }
            else if (SpriteTick>=ChangeTick&&Sprite_n==2)
            {
                Sprite_n = 1;
                birdSprite = Image.FromFile("Bird_01.png");
                SpriteTick = 0;
            }
            g.DrawImage(birdSprite, Bird);
        }

        public void TubeLogic()
        {
            tick++;
            if (tick > newTubeInterval)
            {
                int TubeHeight = r.Next(50, pictureBox1.Height - grassH - HoleSize);
                Tubes.Add(new Tube(pictureBox1.Width, 0, TubeD, TubeHeight));
                Tubes.Add(new Tube(pictureBox1.Width, TubeHeight+HoleSize, TubeD, pictureBox1.Height-TubeHeight-HoleSize-grassH));
                tick = 0;
            }
            for (int i = 0; i < Tubes.Count; i++)
            {
                Tubes[i].Draw(g);
                if (Tubes[i].R.X<=0-TubeD)
                {
                    Tubes.RemoveAt(i);
                }
                if (Tubes[i].R.IntersectsWith(Bird))
                {
                    isOver = true;
                }
            }
            for (int i = 0; i < Tubes.Count; i+=2)
            {
                if (Bird.X + BirdW / 2 == Tubes[i].R.X + TubeD / 2)
                {
                    Score++;
                }
            }
        }

        private void MenuButton_Click(object sender, EventArgs e)
        {
            int GotTubesCount = Tubes.Count;
            for (int i = 0; i < GotTubesCount; i++)
            {
                Tubes.RemoveAt(0);
            }
            Bird.Y = (pictureBox1.Height - grassH) / 2;
            isOver = false;
            Score = 0;
            tick = 0;
            MenuButton.Visible = false;
            playButton.Visible = true;
            lbButton.Visible = true;
            label1.Text = "Flappy flap!";
        }

        private void lbButton_Click(object sender, EventArgs e)
        {
            lbButton.Visible = false;
            playButton.Visible = false;
            label1.Text = "Leader Board";
            string records = "";
            listBox1.Items.Clear();
            /*
              -=Считываем=-
            */
            StreamReader sr = new StreamReader("Scores.txt");
            while (records != null)
            {
                listBox1.Items.Add(records);
                records = sr.ReadLine();
            }
            sr.Close();
            /*
              -===========-
            */
            listBox1.Items.RemoveAt(0); // убираем мусор
            int count=listBox1.Items.Count/2;
            string[] s = new string[count];
            int[] n = new int[count];
            for (int i = 0; i < count; i++)
            {
                s[i] = listBox1.Items[i*2].ToString();
                n[i] = int.Parse(listBox1.Items[i*2+1].ToString());
            }
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count-1-i; j++)
                {
                    if (n[j]<n[j+1])
                    {
                        int temp = n[j];
                        n[j] = n[j + 1];
                        n[j + 1] = temp;
                        string temp1 = s[j];
                        s[j] = s[j + 1];
                        s[j + 1] = temp1;
                    }
                }
            }
            listBox1.Items.Clear();
            for (int i = 0; i < count; i++)
            {
                listBox1.Items.Add(s[i] + " ― " + n[i]);
            }
            listBox1.Visible = true;
            BackButton.Visible = true;
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            listBox1.Visible = false;
            label1.Text = "Flappy flap!";
            lbButton.Visible = true;
            playButton.Visible = true;
            BackButton.Visible = false;
        }

        public void GameOver()
        {
            if (isOver)
            {
                timer1.Enabled = false;
                pictureBox1.Invalidate();
                g.Clear(Color.SkyBlue);
                label1.Visible = true;
                label1.Text = "Game over!\nScore:\n" + Score;
                pictureBox1.Invalidate();
                if (Score!=0)
                {
                    StreamWriter sw = new StreamWriter("Scores.txt", true);
                    sw.WriteLine(PlayerName);
                    sw.WriteLine(Score);
                    sw.Close();
                }
                MenuButton.Visible = true;
                exitButton.Visible = true;
            }
        }
        public void UIDraw()
        {
            g.DrawString(""+Score, new Font("Adobe Fan Heiti Std B", 30), Brushes.Black, new Point(5, pictureBox1.Height - 45));
        }
    }
}
