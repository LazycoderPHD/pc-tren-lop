using bai2;

class Program
{

    static void TestInterchangeSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>> Mảng gốc trước khi sắp xếp:");
        objTam.Xuat();
        objTam.InterchangeSort();
        Console.WriteLine(">> Sau khi chạy Interchange Sort:");
        objTam.Xuat();
    }

    static void TestInsertionSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>> Mảng gốc trước khi sắp xếp:");
        objTam.Xuat();
        objTam.InsertionSort();
        Console.WriteLine(">> Sau khi chạy Insertion Sort:");
        objTam.Xuat();
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Tạo mảng ngẫu nhiên gồm 10 phần tử để test
        IntArray obj = new IntArray(10);

        TestInterchangeSort(obj);
        TestInsertionSort(obj);

        // Tương tự bạn có thể gọi test cho các thuật toán khác: BubbleSort, SelectionSort, QuickSort, HeapSort, ShellSort, ShakerSort, MergeSort...

        Console.ReadKey();
    }

}