using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace DrawGame2FlappyBird
{
    class Tube
    {
        public Rectangle R;
        public Tube(int X, int Y, int d, int height)
        {
            R = new Rectangle(X, Y, d, height);
        }
        public void Draw(Graphics g)
        {
            g.FillRectangle(new SolidBrush(Color.Green), R);
            R.X--;
        }
    }
}
