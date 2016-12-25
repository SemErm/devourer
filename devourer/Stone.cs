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
    class Stone
    {
        Point Position;//позиция яблока
        Bitmap StoneImage = null;
        object LockStones = new object();
        object LockDraw = new object();
        int width;
        int height;

        public Stone(int x, int y)
		{
			Position.X = x;//положение камня на pictirebox
			Position.Y = y;
            if (StoneImage == null)
			{
                StoneImage = (Bitmap)devourer.Properties.Resources.stone.Clone();// инициализация картинки яблока
                width = StoneImage.Width;
                height = StoneImage.Height;
			}
		}

        public Rectangle GetFrame()
        {
                Rectangle myFrame = new Rectangle(Position.X, Position.Y, width, height);//область яблочка на picturebox
                return myFrame;
            
        }

        public void Draw(Graphics g)
        {
            lock (LockDraw)//блокировка
            {
                Rectangle destR = new Rectangle(Position.X, Position.Y, width, height);//область на picturebox
                Rectangle srcR = new Rectangle(0, 0, width, height);//область самого яблочка
                g.DrawImage(StoneImage, destR, srcR, GraphicsUnit.Pixel);//рисуем ялочко в Picturebox
            }
        }
    }
}
