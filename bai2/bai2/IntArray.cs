using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bai2
{
    public class IntArray
    {
        private int[] arr;

        // 1. Get/Set properties và Indexer
        public int[] Arr
        {
            get { return arr; }
            set { arr = value; }
        }

        public int this[int index]
        {
            get { return arr[index]; }
            set { arr[index] = value; }
        }

        public int Length
        {
            get { return arr != null ? arr.Length : 0; }
        }

        // 2. Constructors
        public IntArray()
        {
            arr = new int[0];
        }

        public IntArray(int k)
        {
            if (KiemTraKT(k))
            {
                arr = new int[k];
                Random rand = new Random();
                for (int i = 0; i < k; i++)
                {
                    arr[i] = rand.Next(1, 201); // Giá trị ngẫu nhiên từ 1 đến 200
                }
            }
            else
            {
                arr = new int[0];
            }
        }

        public IntArray(int[] a)
        {
            if (a != null && KiemTraKT(a.Length))
            {
                arr = new int[a.Length];
                Array.Copy(a, arr, a.Length);
            }
            else
            {
                arr = new int[0];
            }
        }

        public IntArray(IntArray obj)
        {
            if (obj != null && obj.arr != null)
            {
                arr = new int[obj.arr.Length];
                Array.Copy(obj.arr, arr, obj.arr.Length);
            }
            else
            {
                arr = new int[0];
            }
        }

        // 3. Kiểm tra kích thước hợp lệ (0 < n <= 1,000,000)
        public bool KiemTraKT(int n)
        {
            return n > 0 && n <= 1000000;
        }

        // Nhập mảng từ bàn phím
        public void Nhap()
        {
            int n;
            do
            {
                Console.Write(">> Nhập số lượng phần tử (0 < n <= 1,000,000): ");
                int.TryParse(Console.ReadLine(), out n);
            } while (!KiemTraKT(n));

            arr = new int[n];
            for (int i = 0; i < n; i++)
            {
                Console.Write($"   Nhập phần tử thứ {i}: ");
                int.TryParse(Console.ReadLine(), out arr[i]);
            }
        }

        // Xuất mảng
        public void Xuat()
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("Mảng rỗng!");
                return;
            }
            Console.WriteLine(string.Join(" ", arr));
        }

        // --- Các phương thức xử lý mảng (Câu 2) ---

        // 1a. Tìm kiếm tuần tự
        public int TimTuanTu(int x)
        {
            int n = arr.Length;
            for (int i = 0; i < n; i++)
            {
                if (arr[i] == x)
                    return i;
            }
            return -1;
        }

        // 1b. Tìm kiếm nhị phân (Yêu cầu mảng phải được sắp xếp tăng dần)
        public int TimNhiPhan(int x)
        {
            int left = 0, right = arr.Length - 1;
            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (arr[mid] == x)
                    return mid;
                else if (arr[mid] < x)
                    left = mid + 1;
                else
                    right = mid - 1;
            }
            return -1;
        }

        // 2. Tìm vị trí phần tử lớn nhất (nếu trùng, trả về vị trí cuối cùng)
        public int TimViTriMax()
        {
            if (arr == null || arr.Length == 0) return -1;
            int maxIdx = 0;
            int maxVal = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (arr[i] >= maxVal)
                {
                    maxVal = arr[i];
                    maxIdx = i;
                }
            }
            return maxIdx;
        }

        // 3. Xóa lần xuất hiện đầu tiên của phần tử có giá trị x
        public bool XoaPhanTu(int x)
        {
            int idx = TimTuanTu(x);
            if (idx == -1) return false;

            int[] newArr = new int[arr.Length - 1];
            for (int i = 0, j = 0; i < arr.Length; i++)
            {
                if (i == idx) continue;
                newArr[j++] = arr[i];
            }
            arr = newArr;
            return true;
        }

        // 4. Chèn phần tử x ngay sau phần tử lớn nhất (trường hợp trùng max, chèn sau vị trí cuối)
        public void ChenSauMax(int x)
        {
            int maxIdx = TimViTriMax();
            int insertPos = (maxIdx == -1) ? 0 : maxIdx + 1;

            int[] newArr = new int[arr.Length + 1];
            for (int i = 0, j = 0; i < newArr.Length; i++)
            {
                if (i == insertPos)
                {
                    newArr[i] = x;
                }
                else
                {
                    newArr[i] = arr[j++];
                }
            }
            arr = newArr;
        }

        // --- Phần C: Minh họa thuật toán tìm kiếm (Câu 5) ---
        public void MinhHoaTimTuanTu(int x)
        {
            Console.WriteLine($"\n--- Minh họa Tìm kiếm tuần tự cho x = {x} ---");
            int comparisons = 0;
            int foundIdx = -1;

            for (int i = 0; i < arr.Length; i++)
            {
                comparisons++;
                Console.WriteLine($"So sánh arr[{i}] ({arr[i]}) với {x}");
                if (arr[i] == x)
                {
                    foundIdx = i;
                    break;
                }
            }

            Console.WriteLine($"Tổng số lần so sánh: {comparisons}");
            if (foundIdx != -1)
                Console.WriteLine($"Kết quả: Tìm thấy tại vị trí {foundIdx}");
            else
                Console.WriteLine("Kết quả: Không tìm thấy!");
        }

        public void MinhHoaTimNhiPhan(int x)
        {
            Console.WriteLine($"\n--- Minh họa Tìm kiếm nhị phân cho x = {x} ---");
            int left = 0, right = arr.Length - 1;
            int comparisons = 0;
            int foundIdx = -1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                comparisons++;
                Console.WriteLine($"Phạm vi hiện tại: [left = {left}, right = {right}], mid = {mid}, arr[mid] = {arr[mid]}");
                Console.WriteLine($"So sánh arr[{mid}] ({arr[mid]}) với {x}");

                if (arr[mid] == x)
                {
                    foundIdx = mid;
                    break;
                }
                else if (arr[mid] < x)
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            Console.WriteLine($"Tổng số lần so sánh: {comparisons}");
            if (foundIdx != -1)
                Console.WriteLine($"Kết quả: Tìm thấy tại vị trí {foundIdx}");
            else
                Console.WriteLine("Kết quả: Không tìm thấy!");
        }


    }
}
