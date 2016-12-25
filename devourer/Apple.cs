using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace devourer
{
    class Apple
    {
        Point Position;//позиция яблока
        Bitmap AppleImage = null;
        object LockDraw = new object();
        int width;
        int height;

        public Apple(int x, int y)
		{
			Position.X = x;//положение яблока на pictirebox
			Position.Y = y;
            if (AppleImage == null)
			{
                AppleImage = (Bitmap)devourer.Properties.Resources.apple.Clone();// инициализация картинки яблока
                width = AppleImage.Width;
                height = AppleImage.Height;
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
                Rectangle srcR = new Rectangle(0, 0,width, height);//область самого яблочка
                g.DrawImage(AppleImage, destR, srcR, GraphicsUnit.Pixel);//рисуем ялочко в Picturebox
            }
        }
    }
}
