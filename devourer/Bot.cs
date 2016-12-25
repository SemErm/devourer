using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
namespace devourer
{
    class Bot
    {
        private Eater eaterBot;
        private Game TheGame;
        private Score TheScores;
        private int direction;//направление
        private int numberOfTerns;//колиество шагов

        public Bot(Game game)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            eaterBot = new Eater(rnd.Next(30, 450), rnd.Next(30, 400), true);//в этой квадратике в любом месте
            TheGame = game;
            TheScores = new Score();
        }

        public void startBot(object pBox)
        {
            var pictureBox = (PictureBox)pBox;
            while (true)
            {
                if (numberOfTerns != 0)
                {
                    GoEater(pictureBox);//движение смайлика
                    DecrementNumberOfTurns();//уменьшение количества ходов
                }
                else
                {
                    SetDirectionAndTurns();//генерирование новых направления и количества ходов
                }
                Thread.Sleep(50);
            }
        }

        public void GoEater(PictureBox pictureBox)
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            Rectangle eaterBotFrame = eaterBot.GetFrame();//область смайлика
            switch (direction)
            {
                case 0:
                    //проверка, чтобы смайлик не упирался в камень или в границу pictureBox
                    if (!TheGame.CheckIntersectionStone("left", eaterBotFrame) && eaterBot.MoveLeft(pictureBox.ClientRectangle))
                    {
                        pictureBox.Invalidate(eaterBot.GetFrame());//перерисовка
                    }
                    else
                    {
                        SetDirectionAndTurns();//смена направления и количества ходов
                    }
                    break;
                case 1:
                    //проверка, чтобы смайлик не упирался в камень или в границу pictureBox
                    if (!TheGame.CheckIntersectionStone("right", eaterBotFrame) && eaterBot.MoveRight(pictureBox.ClientRectangle))
                    {
                        pictureBox.Invalidate(eaterBot.GetFrame());//перерисовка
                    }
                    else
                    {
                        SetDirectionAndTurns();//смена направления и количества ходов
                    }
                    break;
                case 2:
                    //проверка, чтобы смайлик не упирался в камень или в границу pictureBox
                    if (!TheGame.CheckIntersectionStone("up", eaterBotFrame) && eaterBot.MoveUp(pictureBox.ClientRectangle))
                    {
                        pictureBox.Invalidate(eaterBot.GetFrame());//перерисовка
                    }
                    else
                    {
                        SetDirectionAndTurns();//смена направления и количества ходов
                    }
                    break;
                case 3:
                    //проверка, чтобы смайлик не упирался в камень или в границу pictureBox
                    if (!TheGame.CheckIntersectionStone("down", eaterBotFrame) && eaterBot.MoveDown(pictureBox.ClientRectangle))
                    {
                        pictureBox.Invalidate(eaterBot.GetFrame());//перерисовка
                    }
                    else
                    {
                        SetDirectionAndTurns();//смена направления и количества ходов
                    }
                    break;
            }

            if (TheGame.CheckIntersection(eaterBot.GetFrame()))//если яблоко съедино
            {
                TheScores.Increment();//увеличение очков
            }
        }

        public void SetDirectionAndTurns()
        {
            Random rnd = new Random((int)DateTime.Now.Ticks);
            direction = rnd.Next(0, 4);//генерирование направления
            // 0 - left
            // 1 - right
            // 2 - up
            // 3 - down
            numberOfTerns = rnd.Next(1, 25);//генерирование количества ходов
        }

        public void DecrementNumberOfTurns()
        {
            numberOfTerns--;//уменьшение количества ходов
        }

        public Eater GetEater()
        {
            return eaterBot;//возврат смайлика
        }

        public Score GetScores()
        {
            return TheScores;//возврат очков
        }

    }
}
