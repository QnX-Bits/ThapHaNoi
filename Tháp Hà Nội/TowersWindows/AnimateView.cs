using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowersWindows
{
    // Lớp hỗ trợ hoạt ảnh di chuyển đĩa
    class AnimateView
    {
        public static Panel view;

        public void moveUp(PictureBox Disk, int newY)
        {
            while (Disk.Location.Y > newY)
            {
                Disk.Location = new Point(Disk.Location.X, Disk.Location.Y - 10);
                view.Refresh();
                Thread.Sleep(10);
            }
        }

        public void moveDown(PictureBox Disk, int newY)
        {
            while (Disk.Location.Y < newY)
            {
                Disk.Location = new Point(Disk.Location.X, Disk.Location.Y + 10);
                view.Refresh();
                Thread.Sleep(10);
            }
        }

        public void moveRight(PictureBox Disk, int newX)
        {
            while (Disk.Location.X < newX)
            {
                Disk.Location = new Point(Disk.Location.X + 10, Disk.Location.Y);
                view.Refresh();
                Thread.Sleep(10);
            }
        }

        public void moveLeft(PictureBox Disk, int newX)
        {
            while (Disk.Location.X > newX)
            {
                Disk.Location = new Point(Disk.Location.X - 10, Disk.Location.Y);
                view.Refresh();
                Thread.Sleep(10);
            }
        }
    }
}
