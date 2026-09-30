using bai2;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Kiểm tra Câu 1 & 2 & 3 (Phần A)
        Console.Write(">> Nhập kích thước mảng ngẫu nhiên k: ");
        int.TryParse(Console.ReadLine(), out int k);

        IntArray objA = new IntArray(k);
        Console.WriteLine(">> Giá trị mảng ngẫu nhiên objA:");
        objA.Xuat();

        Console.Write("\n>> Nhập giá trị x cần tìm kiếm tuần tự: ");
        int.TryParse(Console.ReadLine(), out int x);
        int kqTT = objA.TimTuanTu(x);
        if (kqTT == -1)
            Console.WriteLine($"-> Không tìm thấy {x} trong mảng.");
        else
            Console.WriteLine($"-> Tìm thấy {x} tại vị trí {kqTT}");

        // Kiểm tra phần minh họa (Phần C)
        objA.MinhHoaTimTuanTu(x);

        // Kiểm tra tìm kiếm nhị phân (yêu cầu mảng đã sắp xếp)
        // Bạn có thể tạo mảng thủ công đã sắp xếp hoặc dùng thuật toán sắp xếp
        int[] sortedData = { 2, 5, 8, 12, 16, 23, 38, 56, 72, 91 };
        IntArray objB = new IntArray(sortedData);
        Console.WriteLine("\n>> Mảng đã sắp xếp (objB cho tìm kiếm nhị phân):");
        objB.Xuat();

        Console.Write(">> Nhập giá trị x cần tìm kiếm nhị phân trong objB: ");
        int.TryParse(Console.ReadLine(), out int xBinary);
        objB.MinhHoaTimNhiPhan(xBinary);

        Console.ReadKey();
    }
}