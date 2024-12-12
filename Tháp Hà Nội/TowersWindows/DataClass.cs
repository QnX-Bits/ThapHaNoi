using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowersWindows
{
    // Lớp đại diện cho một đĩa trong bài toán Tháp Hà Nội
    class Disks
    {
        public int DiskNo { get; set; }         // Số thứ tự của đĩa.
        public PictureBox box { get; set; }     // Hộp hình ảnh đại diện đĩa.
        public int width { get; set; }          // Chiều rộng của đĩa.
        public char peg { get; set; }           // Cột mà đĩa hiện đang nằm (A, B, C).
    }
}
