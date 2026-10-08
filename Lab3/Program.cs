using Lab3;

class Program
{
    static void TestInterchangeSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>>Original array:");
        objTam.Xuat();
        objTam.InterchangeSort();
        Console.WriteLine("\n>>Interchange Sort:");
        objTam.Xuat();
    }

    static void TestBubbleSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>>Original array:");
        objTam.Xuat();
        objTam.BubbleSort();
        Console.WriteLine("\n>>Bubble Sort:");
        objTam.Xuat();
    }

    static void TestSelectionSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>>Original array:");
        objTam.Xuat();
        objTam.SelectionSort();
        Console.WriteLine("\n>>Selection Sort:");
        objTam.Xuat();
    }

    static void TestInsertionSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>>Original array:");
        objTam.Xuat();
        objTam.InsertionSort();
        Console.WriteLine("\n>>Insertion Sort:");
        objTam.Xuat();
    }

    static void TestQuickSort(IntArray obj)
    {
        IntArray objTam = new IntArray(obj);
        Console.WriteLine("\n>>Original array:");
        objTam.Xuat();
        objTam.QuickSort();
        Console.WriteLine("\n>>Quick Sort:");
        objTam.Xuat();
    }

    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.Write("Nhập số lượng phần tử k: ");
        int k = int.Parse(Console.ReadLine()!);

        IntArray objA = new IntArray(k); // Tạo mảng với k phần tử ngẫu nhiên

        TestInterchangeSort(objA);
        TestBubbleSort(objA);
        TestSelectionSort(objA);
        TestInsertionSort(objA);
        TestQuickSort(objA);
    }
}