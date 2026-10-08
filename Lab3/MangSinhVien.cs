namespace Lab3
{
    public class MangSinhVien
    {
        private SinhVien[] danhSach;
        private int n;

        public MangSinhVien()
        {
            danhSach = new SinhVien[0];
            n = 0;
        }

        public void NhapDanhSach()
        {
            while (true)
            {
                Console.Write("Enter the number of students (n, 0 < n <= 1,000,000): ");
                if (int.TryParse(Console.ReadLine(), out n) && n > 0 && n <= 1000000)
                {
                    break;
                }
                Console.WriteLine("-> Invalid number! Please enter n such that 0 < n <= 1,000,000.");
            }

            danhSach = new SinhVien[n];
            Console.WriteLine($"\n=== ENTER INFORMATION FOR {n} STUDENTS ===");
            // Code nhập từng sinh viên...
        }

        // Bổ sung phương thức sắp xếp phần Câu 3 (PART A) sử dụng .Length cho mảng:
        public void SelectionSortByStudentID()
        {
            int length = danhSach.Length; // Dùng Length thay vì Count cho mảng
            for (int i = 0; i < length - 1; i++)
            {
                int minIndex = i;
                for (int j = i + 1; j < length; j++)
                {
                    if (danhSach[j].MaSV.CompareTo(danhSach[minIndex].MaSV) < 0)
                    {
                        minIndex = j;
                    }
                }
                if (minIndex != i)
                {
                    SinhVien temp = danhSach[i];
                    danhSach[i] = danhSach[minIndex];
                    danhSach[minIndex] = temp;
                }
            }
        }

        public void InsertionSortByGPA()
        {
            int length = danhSach.Length; // Dùng Length thay vì Count cho mảng
            for (int i = 1; i < length; i++)
            {
                SinhVien key = danhSach[i];
                int j = i - 1;
                while (j >= 0 && danhSach[j].DiemTB < key.DiemTB)
                {
                    danhSach[j + 1] = danhSach[j];
                    j--;
                }
                danhSach[j + 1] = key;
            }
        }
    }
}