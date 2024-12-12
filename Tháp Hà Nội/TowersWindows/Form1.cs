using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowersWindows
{
    // Lớp chính của giao diện bài toán Tháp Hà Nội
    public partial class Form1 : Form
    {
        private List<string> moves = new List<string>();       // Danh sách các bước di chuyển
        private List<Disks> _TowerDisks = new List<Disks>();   // Danh sách các đĩa
        private AnimateView animate = new AnimateView();       // Đối tượng hỗ trợ hoạt ảnh
        private int _DiskCount = 3;                            // Số lượng đĩa mặc định
        private int diskHeight = 30;                           // Chiều cao của một đĩa
        private int baseHeight = 20;                           // Chiều cao của đáy tháp
        public Form1()
        {   
            InitializeComponent();
            AnimateView.view = panel1;
            resetPanel();
        }
        
        //Vẽ tháp và đĩa
        private void setupTower()
        {
            panel1.Paint += delegate
            {
                setBase();
            };
        }

        private void setBase()
        {
            SolidBrush sb = new SolidBrush(Color.RosyBrown);
            Graphics g = panel1.CreateGraphics();
            int topSpacing = 100;
            int width = 20;

            g.FillRectangle(sb, 0, panel1.Height - baseHeight, panel1.Width, baseHeight);

            drawPeg(panel1, g, sb, 1, width, topSpacing);

            drawPeg(panel1, g, sb, 2, width, topSpacing);

            drawPeg(panel1, g, sb, 3, width, topSpacing);
        }
        private void drawPeg(Panel canvas, Graphics g, SolidBrush sb, int pegNo, int pegWidth, int topSpacing)
        {
            g.FillRectangle(sb, ((int)(canvas.Width / 4) * pegNo) - (pegWidth / 2), topSpacing, pegWidth, canvas.Height - topSpacing);
        }
        //Vẽ và hiển thị đĩa
        private void populateDisks()
        {
            int ii = 1;
            foreach (Disks disk in _TowerDisks)
            {
                PictureBox panelBox = disk.box;
                panelBox.BackColor = colorSelector(disk);
                disk.width = 200 - (20 * ii);
                panelBox.Width = disk.width;
                panelBox.Height = diskHeight;
                panelBox.Tag = disk.DiskNo;
                panelBox.BorderStyle = BorderStyle.FixedSingle;
                Point boxLocation = new Point(getDiskX(disk), (panel1.Height - baseHeight) - (diskHeight * ii++));
                panelBox.Location = boxLocation;
                disk.box = panelBox;
                panel1.Controls.Add(disk.box);
            }
        }
        //Hỗ trợ vị trí và màu sắc
        //Tính toán vị trí
        private int getDiskX(Disks disk)
        {
            int X = 0;
            int Peg = 0;
            switch (disk.peg)
            {
                case 'A': Peg = 1; break;
                case 'B': Peg = 2; break;
                case 'C': Peg = 3; break;
            }
            X = ((panel1.Width / 4) * Peg) - (int)(disk.width / 2);

            return X;
        }

        private int getDiskY(Disks disk)
        {
            int Y = 0;
            int stackNo = _TowerDisks.Count(x => x.peg == disk.peg);
            Y = panel1.Height - baseHeight - (diskHeight * stackNo);
            return Y;
        }
        //Chọn màu sắc cho đĩa
        private Color colorSelector(Disks disk)
        {
            switch (disk.DiskNo)
            {
                case 1: return Color.Red;
                case 2: return Color.OrangeRed;
                case 3: return Color.Yellow;
                case 4: return Color.Green;
                case 5: return Color.Blue;
                case 6: return Color.Purple;
                case 7: return Color.LightBlue;
                default: return Color.Black;
            }
        }
        //Xử lý giải bài toán Tháp Hà Nội
        private void BtnSolve_Click(object sender, EventArgs e)
        {
            resetPanel();
            btnSolve.Enabled = false;
            moves.Clear();
            int NumberOfDisks = _DiskCount;
            solveTower(NumberOfDisks);
            listMoves.DataSource = null;
            listMoves.DataSource = moves;
            btnSolve.Enabled = true;
        }

        private void solveTower(int numberOfDisks)
        {
            char startPeg = 'A';
            char endPeg = 'C';
            char tempPeg = 'B';
            solveTowers(numberOfDisks, startPeg, endPeg, tempPeg);
        }

        private void solveTowers(int n, char startPeg, char endPeg, char tempPeg)
        {
            if (n > 0)
            {
                solveTowers(n - 1, startPeg, tempPeg, endPeg);

                Disks currentDisk = _TowerDisks.Find(x => x.DiskNo == n);
                currentDisk.peg = endPeg;

                animate.moveUp(currentDisk.box, 50);
                if (startPeg < endPeg)
                    animate.moveRight(currentDisk.box, getDiskX(currentDisk));
                else
                    animate.moveLeft(currentDisk.box, getDiskX(currentDisk));
                animate.moveDown(currentDisk.box, getDiskY(currentDisk));
                string line = string.Format("Di chuyển đĩa {0} từ {1} đến {2}", n, startPeg, endPeg);
                Console.WriteLine(line);
                moves.Add(line);
                solveTowers(n - 1, tempPeg, endPeg, startPeg);
            }
        }
        //Tính số bước tối thiểu
        public static int GetMoveCount(int numberOfDisks)
        {
            double numberOfDisks_Double = numberOfDisks;
            return (int)Math.Pow(2.0, numberOfDisks_Double) - 1;
        }
        // Quản lý thay đổi và cập nhật giao diện, xử lí lỗi
        private void resetPanel()
        {
            setupTower();
            panel1.Controls.Clear();
            _TowerDisks = Enumerable.Range(1, _DiskCount).Select(i => new Disks() { DiskNo = i, peg = 'A', box = new PictureBox() }).OrderByDescending(i => i.DiskNo).ToList();

            populateDisks();
            lblCounter.Text = string.Format("Đường đi ngắn nhất {0}", GetMoveCount(_DiskCount));
        }
        private void DiskCount_ValueChanged(object sender, EventArgs e)
        {
            _DiskCount = (int)DiskCount.Value;
            resetPanel();
        }
        
        // Xử lý sự kiện click
        private void lblCounter_Click(object sender, EventArgs e)
        {
            
        }
        // Xử lý sự kiện khi form load
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
