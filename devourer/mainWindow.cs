using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace devourer
{
    public partial class mainWindow : Form
    {
        private int TimerSec;//количество секунд на раунд
        private Game TheGame;
        Label[] LabelScores;//массив label'ов для отображения очков ботов
        object LockDraw = new object();

        public mainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)//обработчик события нажатия клавиши
        {
            TheGame = new Game(pictureBox1,(int)NumberOfBots.Value);
            TheGame.InitializeApples();//генерирование яблочек
            TheGame.InitializeStones();//генерирование камней
            TheGame.workBots();//запуск ботов
            initializeLabel();//динамическое создание label
            pictureBox1.Invalidate();//перерисовка PictureBox
            numericUpDown1.Enabled = false;//становится не активнным
            NumberOfBots.Enabled = false;
            start.Enabled = false;
            label3.ForeColor = Color.Black;
            pictureBox1.Focus();//передача фокуса picturebox, иначе не будет работать обработчик события нажатия клавиш
            timer1.Enabled = true;//включения таймера
            TimerSec = Convert.ToInt32(numericUpDown1.Value);//количество секунд для раунда
            label3.Text = "Оставшееся время: " + TimerSec;
        }

        private void initializeLabel()
        {
            int top = 132;//точка от которой будут динамически создавать новые label
            int left = 518;//точка от которой будут динамически создавать новые label
            LabelScores = new Label[TheGame.GetNumberBots()];//массив label ботов
            for (var i = 0; i < TheGame.GetNumberBots(); i++)
            {
                LabelScores[i] = new Label();
                LabelScores[i].Name = "ScoreBot" + i;
                LabelScores[i].Text = "Очки bot" + (i + 1) + ": 0";
                LabelScores[i].Font = new Font(LabelScores[i].Font.Name, 10, LabelScores[i].Font.Style);
                LabelScores[i].Top = top;
                LabelScores[i].Left = left;
                this.Controls.Add(LabelScores[i]);//добавление на форму
                top += 23;
            }
        }

        private void PaintScores()
        {
            ScorePlayer.Text = "Очки игрока: "+TheGame.GetScoreEater().GetScore().ToString();//вывод очков игрока
            for (var i = 0; i < LabelScores.Length; i++)
            {
                LabelScores[i].Text = "Очки bot" + (i + 1) + ": " + TheGame.GetBot(i).GetScores().GetScore().ToString();//вывод очков ботов
            }
        }

        private void PaintBots(Graphics graph)
        {
            for (var i = 0; i < TheGame.GetNumberBots(); i++)
            {
                TheGame.GetBot(i).GetEater().Draw(graph);//рисование ботов
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            lock (LockDraw)
            {
                Graphics graphic = e.Graphics;
                if (TheGame != null)
                {
                    TheGame.GetEater().Draw(graphic);//ривание смайлика
                    PaintScores();
                    PaintBots(graphic);
                    var apples = TheGame.GetApples();
                    var stones = TheGame.GetStones();
                    for (int i = 0; i < apples.Count; i++)
                    {
                        apples[i].Draw(graphic);//рисование каждого яблочка
                    }
                    for (int i = 0; i < stones.Count; i++)
                    {
                        stones[i].Draw(graphic);//рисование каждого камня
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (TimerSec != 0)
            {
                TimerSec--;
                if (TimerSec <= 10) label3.ForeColor = Color.Red;
                label3.Text = "Оставшееся время: " + TimerSec;
                if (TimerSec % 2 == 0)
                {
                    TheGame.AddApple();
                    pictureBox1.Invalidate();
                }
            }
            else
            {
                TheGame.StopBots();
                timer1.Enabled = false;//выключение таймера
                start.Enabled = true;//активация кнопки
                numericUpDown1.Enabled = true;//активация количество секунд
                NumberOfBots.Enabled = true;
                start.Focus();//фокусированиие на кнопке
                MessageBox.Show("Ваши очки: " + TheGame.GetScoreEater().GetScore(), "Конец игры!");//вывод сообщения
            }
        }
    }
}
