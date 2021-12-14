using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab4Denschyk
{
    class Program
    {
        static void Main(string[] args)
        {


            MPI.Environment.Run(ref args, comm =>
            {
                int count;
                for (count = 1000; count % comm.Size != 0; count++) ;

                Random random = new Random();
                int[] numbers = new int[count];

                for (int i = 0; i < count; ++i)
                    numbers[i] = random.Next(20000);

                if (comm.Rank == 0)
                {

                    int[][] splits = new int[comm.Size][];
                    for (var i = 0; i < comm.Size; i++)
                    {
                        int size = count / comm.Size;
                        int[] array =  numbers.Skip(i * (size)).Take(size).ToArray();
                        splits[i] = array;
                    }
                    var msg = comm.Scatter(splits.ToArray(), 0);
                    int result = msg.Max();
                    Console.WriteLine("Rank " + comm.Rank + " calculated norm \"" + result + "\".");
                 
                    var finalValues = comm.Gather(result, 0);

                    Console.WriteLine("Final Value: \"" + Math.Max(finalValues.Max(), result) + "\".");

                }
                else // not rank 0
                {

                    // program for all other ranks
                    int[] msg = comm.Scatter<int[]>(0);
                    int result = msg.Max();

                    Console.WriteLine("Rank " + comm.Rank + " calculated norm \"" + result + "\".");

                    comm.Gather(result, 0);

                }
            });

        }


    }
}
