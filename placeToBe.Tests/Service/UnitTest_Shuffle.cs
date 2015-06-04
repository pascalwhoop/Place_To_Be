using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

    class UnitTest_Shuffle
    {
        static void Main(string[] args)
        {
            Test t = new Test();

            double[] abc = new double[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            t.shuffle(abc);
            for (int i = 0; i < abc.Length; i++)
            {
                Console.WriteLine(i + ". " + abc[i]);
            }
            Console.ReadLine();

        }

        public class Test
        {

            Random _random = new Random();

            public double[] shuffle(double[] o)
            {
                int n = o.Length;

                for (int i = 0; i < n; i++)
                {
                    //NextDouble() gibt eine Zufallszahl zwischen 0 und 1 wie Math.Random() RR-RANDOOM Java.
                    int r = i + (int)(_random.NextDouble() * (n - i));
                    double t = o[r];
                    o[r] = o[i];
                    o[i] = t;
                }
                return o;
            }
        }

    }
