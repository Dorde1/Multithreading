using System;
using System.Threading;
using System.Diagnostics;

namespace Multithreading {
  class Program {
    static void Main(string[] args) {
      int length;
      double minimum;
      double maximum;
      
      do {
        Console.Clear();
        Console.Write("Length of Array: ");
      } while (!int.TryParse(Console.ReadLine(), out length));
      
      do {
        Console.Clear();
        Console.Write("Minimum Number: ");
      } while (!double.TryParse(Console.ReadLine(), out minimum));
      
      do {
        Console.Clear();
        Console.Write("Maximum Number: ");
      } while (!double.TryParse(Console.ReadLine(), out maximum));
      Console.Clear();
      Console.WriteLine("Creating numbers...");
      double[] numbers = CreateNumbers(length, minimum, maximum, out double actualMax);
      
      int numThreads;
      do {
        Console.Clear();
        Console.WriteLine($"Your computer has {Environment.ProcessorCount} threads");
        Console.Write("Enter Number of Threads: ");
      } while (!int.TryParse(Console.ReadLine(), out numThreads));

      int partition = length / numThreads;
      Console.Clear();
      Console.WriteLine($"Array Length: {length}, Number of Threads: {numThreads}, Partition Length: {partition}, Minimum Number: {minimum}, Maximum Number: {maximum}");
      Thread[] threads = new Thread[numThreads];
      double[] maxArray = new double[numThreads];
      Stopwatch watch = new Stopwatch();
      watch.Start();
      for (int i = 0; i < numThreads; i++) {
        int offset = partition * i;
        threads[i] = new Thread(index => {
          maxArray[(int)index] = FindMax(numbers, offset, partition);
        });
        threads[i].Start(i);
      }

      double maxFound = minimum - 1;
      for (int i = 0; i < numThreads; i++) {
        threads[i].Join();
        maxFound = maxArray[i] > maxFound ? maxArray[i] : maxFound;
      }
      watch.Stop();
      Console.WriteLine($"Finished in {watch.ElapsedMilliseconds}ms");
      Console.WriteLine($"Actual Max: {actualMax}");
      Console.WriteLine($"Max Found: {maxFound}");
      Console.ReadKey();
    }
    private static T FindMax<T>(T[] input, int offset, int length) where T: IComparable<T> {
      if (offset + length > input.Length)
        throw new IndexOutOfRangeException();
      T max = input[offset++];
      for (int i = 0; i < length-1; i++)
        Console.WriteLine($"Thread {Environment.CurrentManagedThreadId}, current max : {max = (input[offset + i].CompareTo(max) > 0 ? input[offset + i] : max)}");
      return max;
    }
    private static double[] CreateNumbers(int length, double minimum, double maximum, out double max) {
      double[] output = new double[length];
      double value;
      max = minimum-1;
      Random rand = new Random();
      for (int i = 0; i < length; i++) {
        value = rand.NextDouble() * (maximum - minimum) + minimum;
        output[i] = value;
        max = value > max ? value : max;
      }
      return output;
    }
  }
}