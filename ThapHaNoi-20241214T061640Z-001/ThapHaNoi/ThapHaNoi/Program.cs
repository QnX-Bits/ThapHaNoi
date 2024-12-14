using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThapHaNoi
{
    class HanoiTower
    {
        private int n;
        private Stack<(int value, ConsoleColor color)>[] towers;
        private List<(int disk, int source, int target)> steps;
        private Stack<(int count, int source, int aux, int target)> moveStack;
        private Random random;

        // Mảng màu để áp dụng cho các đĩa
        private static ConsoleColor[] diskColors = new ConsoleColor[]
        {
        ConsoleColor.Red,
        ConsoleColor.Yellow,
        ConsoleColor.Green,
        ConsoleColor.Blue,
        ConsoleColor.Magenta,
        ConsoleColor.Cyan,
        ConsoleColor.White
        };

        public HanoiTower(int numberOfDisks)
        {
            if (numberOfDisks <= 0)
            {
                throw new ArgumentException("Số đĩa phải lớn hơn 0");
            }

            n = numberOfDisks;
            random = new Random();
            InitializeTowers();
            steps = new List<(int disk, int source, int target)>();
            moveStack = new Stack<(int count, int source, int aux, int target)>();
        }

        private void InitializeTowers()
        {
            towers = new Stack<(int value, ConsoleColor color)>[3];
            for (int i = 0; i < 3; i++)
            {
                towers[i] = new Stack<(int value, ConsoleColor color)>();
            }
        }

        public void RandomizeTower()
        {
            Console.WriteLine("Tạo giá trị ngẫu nhiên cho các đĩa...");

            HashSet<int> usedValues = new HashSet<int>();
            while (usedValues.Count < n)
            {
                int value = random.Next(1, n * 2);
                usedValues.Add(value);
            }

            towers[0].Clear();
            foreach (int value in usedValues)
            {
                towers[0].Push((value, GetDiskColor(value)));
            }

            Console.WriteLine("Trạng thái ban đầu:");
            DisplayState();
        }

        // Sắp xếp các đĩa trên một cọc theo thứ tự giảm dần (lớn ở dưới, nhỏ ở trên)
        public void SortTower(int towerIndex)
        {
            if (towerIndex < 0 || towerIndex > 2)
            {
                throw new ArgumentException("Chỉ số cọc không hợp lệ (0-2)");
            }

            Console.WriteLine($"Sắp xếp cọc {(char)(towerIndex + 'A')}...");

            // Lấy tất cả các giá trị từ cọc cần sắp xếp
            List<(int value, ConsoleColor color)> values = new List<(int value, ConsoleColor color)>();
            while (towers[towerIndex].Count > 0)
            {
                values.Add(towers[towerIndex].Pop());
            }

            // Sắp xếp giảm dần và đưa lại vào cọc
            values.Sort((a, b) => b.value.CompareTo(a.value));
            foreach (var valueWithColor in values)
            {
                towers[towerIndex].Push(valueWithColor);
            }

            Console.WriteLine($"Đã sắp xếp xong cọc {(char)(towerIndex + 'A')}:");
            DisplayState();
        }

        // Kiểm tra xem một cọc đã được sắp xếp đúng chưa
        public bool IsTowerSorted(int towerIndex)
        {
            if (towers[towerIndex].Count <= 1)
                return true;

            var values = towers[towerIndex].Select(x => x.value).ToArray();
            for (int i = 0; i < values.Length - 1; i++)
            {
                if (values[i] < values[i + 1])
                    return false;
            }
            return true;
        }

        // Phương thức chọn màu cho đĩa
        private ConsoleColor GetDiskColor(int diskValue)
        {
            // Sử dụng giá trị đĩa để chọn màu một cách ngẫu nhiên nhưng nhất quán
            return diskColors[Math.Abs(diskValue) % diskColors.Length];
        }

        public void Solve()
        {
            Console.WriteLine($"\nGiải pháp tháp Hà Nội với {n} đĩa:");
            DisplayState();

            moveStack.Push((n, 0, 1, 2));

            while (moveStack.Count > 0)
            {
                var (count, source, aux, target) = moveStack.Pop();

                if (count == 1)
                {
                    MoveDisk(source, target);
                    DisplayState();
                }
                else
                {
                    moveStack.Push((count - 1, aux, source, target));
                    moveStack.Push((1, source, aux, target));
                    moveStack.Push((count - 1, source, target, aux));
                }
            }

            Console.WriteLine("\nCác bước di chuyển:");
            for (int i = 0; i < steps.Count; i++)
            {
                var (disk, source, target) = steps[i];
                Console.WriteLine($"Bước {i + 1}: Di chuyển đĩa {disk} từ cọc {(char)(source + 'A')} sang cọc {(char)(target + 'A')}");
            }
            Console.WriteLine($"\nTổng số bước thực hiện: {steps.Count}");
            Console.WriteLine($"Số bước tối thiểu lý thuyết: {Math.Pow(2, n) - 1}");
        }

        private bool IsValidMove(int source, int target)
        {
            if (towers[source].Count == 0)
                return false;
            if (towers[target].Count == 0)
                return true;
            return towers[source].Peek().value < towers[target].Peek().value;
        }

        private void MoveDisk(int source, int target)
        {
            if (!IsValidMove(source, target))
            {
                throw new InvalidOperationException($"Di chuyển không hợp lệ từ cọc {(char)(source + 'A')} sang cọc {(char)(target + 'A')}");
            }

            var (disk, color) = towers[source].Pop();

            // Chọn màu mới khi di chuyển
            color = GetDiskColor(disk);

            towers[target].Push((disk, color));
            steps.Add((disk, source, target));
        }

        private void DisplayState()
        {
            Console.WriteLine("\nTrạng thái hiện tại:");

            int maxHeight = n;
            (int value, ConsoleColor color)[,] display = new (int value, ConsoleColor color)[maxHeight, 3];

            for (int i = 0; i < 3; i++)
            {
                var towerArray = towers[i].ToArray().Reverse().ToArray();
                for (int j = 0; j < towerArray.Length; j++)
                {
                    display[j, i] = towerArray[j];
                }
            }

            for (int i = maxHeight - 1; i >= 0; i--)
            {
                Console.Write("|");
                for (int j = 0; j < 3; j++)
                {
                    if (display[i, j].value == 0)
                    {
                        Console.Write("    |");
                    }
                    else
                    {
                        Console.ForegroundColor = display[i, j].color;
                        Console.Write($"{display[i, j].value,4}");
                        Console.ResetColor();
                        Console.Write("|");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine(new string('-', 20));
            Console.WriteLine("  A     B     C  ");
            Console.WriteLine();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.InputEncoding = Encoding.Unicode;

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Họ tên và MSSV: Châu Gia Hòa - 31231022184 \n                Sử Quốc Thịnh - 31231020243 \n                Nguyễn Đức Tuấn Anh - 31231026504 \n                Lê Hoàng Khang - 31231024814 ");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("BÀI TOÁN THÁP HÀ NỘI");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("Bài toán Tháp Hà Nội là một bài toán kinh điển trong lĩnh vực khoa học máy tính và toán học, thường được dùng để minh họa cho các khái niệm về đệ quy, thuật toán và độ phức tạp tính toán.");

            Console.WriteLine("Sau đây là các bước giải của bài toán:");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("----------------------------------------------------------------------------");

            Console.ResetColor();
            string userInput;
            bool continueProgram = true;
            while (continueProgram)
            {
                try
                {
                    Console.Write("Nhập số đĩa (nhập 0 để thoát): ");
                    int n = int.Parse(Console.ReadLine());

                    if (n == 0)
                    {
                        continueProgram = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Thoát chương trình.");
                        break;
                    }

                    HanoiTower hanoi = new HanoiTower(n);
                    hanoi.RandomizeTower();

                    // Thêm tùy chọn sắp xếp cọc A
                    do
                    {
                        Console.WriteLine("\nBạn có muốn sắp xếp lại cọc A không? (Y/N)");
                        userInput = Console.ReadLine().Trim().ToLower();

                        if (userInput != "y" && userInput != "n")
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Vui lòng chỉ nhập 'Y' hoặc 'N'. Thử lại!");
                            Console.ResetColor();
                        }
                    }
                    while (userInput != "y" && userInput != "n");

                    if (userInput == "n")
                    {
                        continueProgram = false;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Thoát chương trình.");
                        break;
                    }

                    else
                    {
                        hanoi.SortTower(0);
                    }

                    hanoi.Solve();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Vui lòng nhập một số nguyên hợp lệ.");
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi: {ex.Message}");
                }
                do
                {
                    Console.Write("Bạn có muốn thực hiện lại bài toán không? (Y/N): ");
                    userInput = Console.ReadLine().Trim().ToLower();

                    if (userInput != "y" && userInput != "n")
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Vui lòng chỉ nhập 'Y' hoặc 'N'. Thử lại!");
                        Console.ResetColor();
                    }
                }
                while (userInput != "y" && userInput != "n");

                if (userInput == "n")
                {
                    continueProgram = false;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Thoát chương trình.");
                    break;
                }

            }
            Console.ReadKey();
        }
    }
}
