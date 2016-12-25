using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace devourer
{
    class Game
    {
        Eater TheEater;//смайлик с начальными координатами
        Score TheScore;//очки смайлика
        List<Apple> Apples = new List<Apple>();//список яблок
        List<Stone> Stones = new List<Stone>();//список камней
        PictureBox picture;
        object lockApple = new object();//объект для блокировки 
        object lockStones = new object();//объект для блокировки 
        object lockDeleteApple = new object();//объект для блокировки 
        List<Bot> Bots = new List<Bot>();//список ботов
        List<Thread> ThreadBots = new List<Thread>();//список потоков
        int numberOfBots;//количество ботов в игре

        const int NUMBER_OF_APPLES = 50;//начальное количество яблок
        const int NUMBER_OF_STONES = 15;//начальное количество камней
        

        public Game(PictureBox pic,int nOb)
        {
            TheEater = new Eater(100, 100, false);
            TheScore = new Score();
            this.picture = pic;
            picture.KeyDown += new KeyEventHandler(PictureBox1_KeyDown);//обработчик события нажатия клавиш, когда активен picturebox
            numberOfBots = nOb;
        }

        private void PictureBox1_KeyDown(object sender, KeyEventArgs e)
        {
            Rectangle eaterFrame = TheEater.GetFrame();//область смайлика
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if (!CheckIntersectionStone("left", eaterFrame) && TheEater.MoveLeft(picture.ClientRectangle))//проверка, что смайлик не наткнулся в камень и в границу PictureBox
                    {
                        //перемещение смайлика с передачей области Picturebox
                        picture.Invalidate(TheEater.GetFrame());//перерисовка области смайлика
                    }
                    break;
                case Keys.Right:
                    if (!CheckIntersectionStone("right", eaterFrame) && TheEater.MoveRight(picture.ClientRectangle))//проверка, что смайлик не наткнулся в камень и в границу PictureBox
                    {
                        picture.Invalidate(TheEater.GetFrame());//перерисовка области смайлика
                    }
                    break;
                case Keys.Up:
                    if (!CheckIntersectionStone("up", eaterFrame) && TheEater.MoveUp(picture.ClientRectangle))//проверка, что смайлик не наткнулся в камень и в границу PictureBox
                    {
                        picture.Invalidate(TheEater.GetFrame());//перерисовка области смайлика
                    }
                    break;
                case Keys.Down:
                    if (!CheckIntersectionStone("down", eaterFrame) && TheEater.MoveDown(picture.ClientRectangle))//проверка, что смайлик не наткнулся в камень и в границу PictureBox
                    {
                        picture.Invalidate(TheEater.GetFrame());//перерисовка области смайлика
                    }
                    break;
                default:
                    break;
            }
            if (CheckIntersection(TheEater.GetFrame()))
            {
                TheScore.Increment();//увеличение количества очков
            }

        }

        public void InitializeApples()//создание яблок
        {
            Random rnd = new Random();
            for (int i = 0; i < NUMBER_OF_APPLES; i++)
            {
                Apples.Add(new Apple(
                    rnd.Next(10, picture.ClientRectangle.Right - 10),
                    rnd.Next(10, picture.ClientRectangle.Bottom - 30)
                    ));
            }
        }

        public void InitializeStones()//создание камней
        {
            Random rnd = new Random((int)DateTime.Now.Millisecond);
            for (int i = 0; i < NUMBER_OF_STONES; i++)
            {
                Stones.Add(new Stone(
                    rnd.Next(20, picture.ClientRectangle.Right - 20),
                    rnd.Next(20, picture.ClientRectangle.Bottom - 20))
                    );
            }
        }

        public bool CheckIntersection(Rectangle eater)//проверка на пересечение с яблоками
        {
            lock (lockApple)
            {
                for (int i = 0; i < Apples.Count; i++)
                {
                    Rectangle appleRect = Apples[i].GetFrame();//возвращает область, в которой находится яблочко
                    if (eater.IntersectsWith(appleRect))//проверка сжирания смайликом яблока
                    {
                        Apples.RemoveAt(i);
                        picture.Invalidate(appleRect);
                        return true;
                    }
                }
            }
            return false;
        }

        public bool CheckIntersectionStone(string direction, Rectangle eaterRect)//проверка на пересечение с камнями
        {
            lock (lockStones)
            {
                for (int i = 0; i < Stones.Count; i++)//перебор всех камнеей
                {
                    Rectangle stoneRect = Stones[i].GetFrame();//возвращает область, в которой находится камень
                    if (eaterRect.IntersectsWith(stoneRect))//проверка 
                    {
                        switch (direction)
                        {
                            case "left":
                                if ((stoneRect.Right + 3 >= eaterRect.Left) && (eaterRect.Left > stoneRect.Left))
                                    return true;
                                break;
                            case "right":
                                if ((stoneRect.Left + 3 >= eaterRect.Right) && (eaterRect.Left < stoneRect.Left))
                                    return true;
                                break;
                            case "up":
                                if ((stoneRect.Bottom + 3 >= eaterRect.Top) && (stoneRect.Top < eaterRect.Top))
                                    return true;
                                break;
                            case "down":
                                if ((stoneRect.Top + 3 <= eaterRect.Bottom) && (eaterRect.Top < stoneRect.Top))
                                    return true;
                                break;
                        }
                    }
                }
            }
            return false;
        }

        public void workBots()//запуск ботов
        {
            for (var i = 0; i < numberOfBots; i++)
            {
                Bots.Add(new Bot(this));
                Thread t = new Thread(new ParameterizedThreadStart(Bots[Bots.Count - 1].startBot));//присваивание потоку функцию
                ThreadBots.Add(t);//добавление потока к списку потоков
                ThreadBots[ThreadBots.Count-1].IsBackground = true;
                ThreadBots[ThreadBots.Count - 1].Start(picture);//запуск потока
                Thread.Sleep(10);
            }
        }

        public void StopBots()//остановка ботов
        {
            for (var i = 0; i < numberOfBots; i++)
                ThreadBots[i].Abort();//завершение потока
        }

        public Bot GetBot(int index)//возвратить бота
        {
            return Bots[index];
        }

        public List<Apple> GetApples()//возвращает список яблок
        {
            if (Apples.Count > 0)
                return Apples;
            else return new List<Apple>();
        }

        public List<Stone> GetStones()//возвращает список камней
        {
            if (Stones.Count > 0)
                return Stones;
            else return new List<Stone>();
        }

        public void RemoveApple(int index)//удаляет съеденное яблоко из списка
        {
            lock(lockDeleteApple)
            {
                Apples.RemoveAt(index);
            }
        }

        public void AddApple()//добавление нового яблочка
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            Apples.Add(new Apple(
                    rnd.Next(10, picture.ClientRectangle.Right - 10),
                    rnd.Next(10, picture.ClientRectangle.Bottom - 30)
                    ));
        }

        public Eater GetEater()//возвращает объект смайлика
        {
            return TheEater;
        }

        public Score GetScoreEater()
        {
            return TheScore;
        }
        public int GetNumberBots()
        {
            return Bots.Count;
        }

    }
}
