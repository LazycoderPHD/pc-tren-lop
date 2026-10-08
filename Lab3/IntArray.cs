using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    public class IntArray
    {
        private int[] arr;

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

            // arr = new int[k] nếu như muốn người dùng tự nhập dữ liệu
        }

        // public void Nhap()
        // {
        //     Console.WriteLine($"\nNhập các phần tử cho mảng ({arr.Length} phần tử):");
        //     for (int i = 0; i < arr.Length; i++)
        //     {
        //         Console.Write($"  Phần tử thứ {i + 1}: ");
        //         arr[i] = int.Parse(Console.ReadLine());
        //     }
        // } nếu như muốn tự người dùng nhập dữ liệu

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

        public void HoanVi(ref int a, ref int b)
        {
            int tam = a;
            a = b;
            b = tam;
        }

        public void InterchangeSort()
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[i] > arr[j])
                    {
                        HoanVi(ref arr[i], ref arr[j]);
                    }
                }
            }
        }

        // 1. Bubble Sort (Sắp xếp nổi bọt)
        public void BubbleSort()
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = n - 1; j > i; j--)
                {
                    if (arr[j] < arr[j - 1])
                    {
                        HoanVi(ref arr[j], ref arr[j - 1]);
                    }
                }
            }
        }

        // 2. Selection Sort (Sắp xếp chọn)
        public void SelectionSort()
        {
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < n; j++)
                {
                    if (arr[j] < arr[minIndex])
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    HoanVi(ref arr[i], ref arr[minIndex]);
                }
            }
        }

        // 3. Insertion Sort (Sắp xếp chèn)
        public void InsertionSort()
        {
            int n = arr.Length;
            for (int i = 1; i < n; i++)
            {
                int key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }

        // 4. Quick Sort (Sắp xếp nhanh - Đệ quy)
        public void QuickSort()
        {
            QuickSortRecursive(0, arr.Length - 1);
        }

        private void QuickSortRecursive(int left, int right)
        {
            if (left >= right) return;

            int pivot = arr[(left + right) / 2];
            int i = left, j = right;

            while (i <= j)
            {
                while (arr[i] < pivot) i++;
                while (arr[j] > pivot) j--;

                if (i <= j)
                {
                    HoanVi(ref arr[i], ref arr[j]);
                    i++;
                    j--;
                }
            }

            if (left < j) QuickSortRecursive(left, j);
            if (i < right) QuickSortRecursive(i, right);
        }

        // 1. Shell Sort
        public void ShellSort()
        {
            int n = arr.Length;
            for (int gap = n / 2; gap > 0; gap /= 2)
            {
                for (int i = gap; i < n; i++)
                {
                    int temp = arr[i];
                    int j = i;
                    while (j >= gap && arr[j - gap] > temp)
                    {
                        arr[j] = arr[j - gap];
                        j -= gap;
                    }
                    arr[j] = temp;
                }
            }
        }

        // 2. Shaker Sort (Bidirectional Bubble Sort)
        public void ShakerSort()
        {
            int left = 0;
            int right = arr.Length - 1;
            while (left < right)
            {
                for (int i = left; i < right; i++)
                {
                    if (arr[i] > arr[i + 1])
                    {
                        HoanVi(ref arr[i], ref arr[i + 1]);
                    }
                }
                right--;

                for (int i = right; i > left; i--)
                {
                    if (arr[i] < arr[i - 1])
                    {
                        HoanVi(ref arr[i], ref arr[i - 1]);
                    }
                }
                left++;
            }
        }

        // 3. Heap Sort
        public void HeapSort()
        {
            int n = arr.Length;
            for (int i = n / 2 - 1; i >= 0; i--)
                Heapify(n, i);

            for (int i = n - 1; i > 0; i--)
            {
                HoanVi(ref arr[0], ref arr[i]);
                Heapify(i, 0);
            }
        }

        private void Heapify(int n, int i)
        {
            int largest = i;
            int left = 2 * i + 1;
            int right = 2 * i + 2;

            if (left < n && arr[left] > arr[largest])
                largest = left;

            if (right < n && arr[right] > arr[largest])
                largest = right;

            if (largest != i)
            {
                HoanVi(ref arr[i], ref arr[largest]);
                Heapify(n, largest);
            }
        }

        // 4. Merge Sort
        public void MergeSort()
        {
            MergeSortRecursive(0, arr.Length - 1);
        }

        private void MergeSortRecursive(int left, int right)
        {
            if (left < right)
            {
                int mid = (left + right) / 2;
                MergeSortRecursive(left, mid);
                MergeSortRecursive(mid + 1, right);
                Merge(left, mid, right);
            }
        }

        private void Merge(int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            int[] L = new int[n1];
            int[] R = new int[n2];

            Array.Copy(arr, left, L, 0, n1);
            Array.Copy(arr, mid + 1, R, 0, n2);

            int i = 0, j = 0, k = left;
            while (i < n1 && j < n2)
            {
                if (L[i] <= R[j])
                {
                    arr[k++] = L[i++];
                }
                else
                {
                    arr[k++] = R[j++];
                }
            }

            while (i < n1) arr[k++] = L[i++];
            while (j < n2) arr[k++] = R[j++];
        }

        // Ví dụ hàm đếm và minh họa cho Interchange Sort
        public long InterchangeSortMinhHoa()
        {
            long count = 0;
            int n = arr.Length;
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    count++; // Đếm số lần so sánh/gán
                    if (arr[i] > arr[j])
                    {
                        HoanVi(ref arr[i], ref arr[j]);
                    }
                }
            }
            return count;
        }

    }
}
