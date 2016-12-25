using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.Resources;

namespace devourer
{
    class Eater
    {
        Point Position;
        Bitmap EaterImage = null;
        Bitmap EaterImage2 = null;
        object LockDraw = new object();
        int interval = 3; // скорость передвижения смайлик
        int press = 0;//переменная для открытия и закрытия рта
        int direction = 0;//направление движения
        int width, height;
        // 0 - left
        // 1 - right
        // 2 - up
        // 3 - down
        
        public Eater(int x, int y,bool bot)
        {
            Position.X = x;//начальное положение по х
            Position.Y = y;//начальное положение по у
            if (EaterImage == null)
            {
                if (bot) 
                    EaterImage = (Bitmap)devourer.Properties.Resources.eaterBot.Clone();
                else
                    EaterImage = (Bitmap)devourer.Properties.Resources.eater.Clone();//присваивание картинок
                width = EaterImage.Width;
                height = EaterImage.Height;
            }

            if (EaterImage2 == null)
            {
                if (bot)
                    EaterImage2 = (Bitmap)devourer.Properties.Resources.eater2Bot.Clone();
                else
                    EaterImage2 = (Bitmap)devourer.Properties.Resources.eater2.Clone();//присваивание картинок
            }
        }

        public Rectangle GetFrame()
        {
                Rectangle myFrame = new Rectangle(Position.X, Position.Y, width, height);//получения области смайлика на picturebox
                return myFrame;
        }

        private Bitmap RotateImage(Bitmap img)
        {
            var bit = (Bitmap)img.Clone();//каждый раз новая картинка, т.к передается по ссылке
            switch (direction)
            {
                case 0:
                    bit.RotateFlip(RotateFlipType.RotateNoneFlipNone);
                    break;
                case 1:
                    bit.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 2:
                    bit.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 3:
                    bit.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
            }
            return bit;
        }

        public void Draw(Graphics g)
        {
            lock (LockDraw)
            {
                Rectangle destR = new Rectangle(Position.X, Position.Y, width, height);//область на Picturebox
                Rectangle srcR = new Rectangle(0, 0, width, height);//область смайлика
                Bitmap image;

                if (press % 2 == 0)
                {
                    image = EaterImage;//открытый рот   
                }
                else
                {
                    image = EaterImage2;//закрытый рот
                }

                g.DrawImage(RotateImage(image), destR, srcR, GraphicsUnit.Pixel);//рисование смайлика на Picturebox
            }
        }

        public bool MoveLeft(Rectangle r)
        {
                if (Position.X <= 0)//проверка условия достижимости левой границы picturebox
                    return false;

                Position.X -= interval;
                direction = 0;
                press = (press % 2 == 0) ? 1 : 0;
                return true;
        }

        public bool MoveRight(Rectangle r)//r - область видимости picturebox
        {
                if (Position.X >= r.Width - width)
                    return false;

                Position.X += interval;
                direction = 1;//направление в право
                press = (press % 2 == 0) ? 1 : 0;
                return true;
        }

        public bool MoveUp(Rectangle r)
        {
                if (Position.Y <= 0)
                    return false;

                Position.Y -= interval;
                direction = 2;
                press = (press % 2 == 0) ? 1 : 0;
                return true;
        }

        public bool MoveDown(Rectangle r)
        {
                if (Position.Y >= r.Height - height)
                    return false;

                Position.Y += interval;
                direction = 3;
                press = (press % 2 == 0) ? 1 : 0;
                return true;
        }

    }
}
