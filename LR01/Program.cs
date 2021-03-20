using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LR01
{
    public class MyConsole
    {
        /// <summary>
        /// N целых чисел (интервалов времени между отказами элементов)
        /// N>100
        /// </summary>
        /// <returns>int</returns>
        public static int GetNumberExp()
        {
            Console.WriteLine("Введите количество экспериментов");
            int result = 0;
            while (!(int.TryParse(Console.ReadLine(), out result) && result > 100))
                Console.WriteLine("Нужно ввести целое числое больше 100");
            return result;
        }

        public static void PrintArray(double[] array, string mess = "")
        {
            if (mess != string.Empty)
            {
                Console.WriteLine();
                Console.WriteLine(mess);
            }
            Console.WriteLine("{0}\n", string.Join("\n", array));
        }

        public static void PrintArray(int[] array, string mess="")
        {
            if (mess != string.Empty)
            {
                Console.WriteLine();
                Console.WriteLine(mess);
            }
            Console.WriteLine("{0}\n", string.Join(" ", array));
        }

        public static void PrintChart(double[] array)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            double max = array.Max();
            int pow = 0;
            while(max < 10)
            {
                pow++;
                max *= 10;
            }
            while (max > 100)
            {
                pow--;
                max /= 10;
            }
            pow++;
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.CursorLeft = x + i;
                    Console.CursorTop = y + 9 - j;
                    int yi = (int)(array[i] * Math.Pow(10, pow) / max);
                    Console.Write(yi > j ? '+' : ' ');
                }
            }
            Console.CursorLeft = 0;
            Console.CursorTop = y + 10;
        }
    }

    public class MyMath
    {
        public class MyArray
        {
            static Random rnd = new Random();
            /// <summary>
            /// Заполняет массив указанной длинны случайными числами от 0 до 9 включительно
            /// С помощью генератора случайных чисел получить последовательность N целых чисел (интервалов времени между отказами элементов).
            /// </summary>
            /// <param name="n">количество элементов в масиве</param>
            /// <param name="min">начальное 0 по умолчанию</param>
            /// <param name="max">кочечное 9 по умолчанию</param>
            /// <returns></returns>
            public static int[] InitRandom(int n, int min = 0, int max = 10)
            {
                if (min > max)
                {
                    int tmp = min;
                    min = max;
                    max = tmp;
                }
                int[] result = new int[n];
                for (int i = 0; i < result.Length; i++)
                    result[i] = rnd.Next(min, max);
                return result;
            }

            /// <summary>
            /// Распологаем отказы на интервале времени
            /// </summary>
            /// <param name="array">интервалы времени между отказами элементов</param>
            /// <returns>int[]</returns>
            public static int[] Summ(int[] array)
            {
                int[] result = new int[array.Length];
                result[0] = array[0];
                for (int i = 1; i < array.Length; i++)
                    result[i] += result[i - 1] + array[i];
                return result;
            }
            /// <summary>
            /// Считаем частоты
            /// </summary>
            /// <param name="intervalsNumbers">количество интервалов</param>
            /// <param name="array">время отказов</param>
            /// <returns></returns>
            public static int[] SplitInterval(int intervalsNumbers, int[] array, out double intervalWidth)
            {
                int[] result = new int[intervalsNumbers];
                int max = array[array.Length - 1];
                intervalWidth = max / (double)intervalsNumbers;
                foreach (int item in array)
                    result[item == max ? result.Length - 1 : (int)(item / intervalWidth)]++;
                return result;
            }

            public static int[] SplitInterval(int[] array, out double intervalWidth)
            {
                int[] result = null;
                int intervalsNumbers = array.Length / 5;

                do
                {
                    result = SplitInterval(intervalsNumbers, array, out intervalWidth);
                    intervalsNumbers--;
                } while (!CheckingFrequency(result));

                return result;
            }

            public static bool CheckingFrequency(int[] array)
            {
                foreach (int item in array)
                    if (item < 5)
                        return false;
                return true;
            }

            public static double[] ProbabilityDistribution(int[] frequencies, double lambda, double intervalWidth)
            {
                double[] result = new double[frequencies.Length - 1];
                double znam_temp = frequencies[frequencies.Length - 1] - frequencies[0];
                for (int i = 0; i < result.Length; i++)
                {
                    double xi = intervalWidth * i;
                    double xi1 = xi + intervalWidth;
                    double Pxi = 1 - Math.Exp(-lambda * xi);
                    double Pxi1 = 1 - Math.Exp(-lambda * xi1);
                    result[i] = Pxi1 - Pxi;
                }
                return result;
            }
        }
        public static double Xb(int[] frequencies, double intervalWidth, int N)
        {
            double summ = 0;
            for (int i = 0; i < frequencies.Length; i++)
            {
                double xi = i * intervalWidth;
                double xi1 = xi + intervalWidth;
                double x = (xi + xi1) / 2;
                summ += x * frequencies[i];
            }
            return summ / N;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int N = MyConsole.GetNumberExp();
            int[] failureTime = MyMath.MyArray.InitRandom(N);
            MyConsole.PrintArray(failureTime);
            int[] failureTimeOx = MyMath.MyArray.Summ(failureTime);
            MyConsole.PrintArray(failureTimeOx);
            //1. расчет частот.
            double intervalWidth;
            int[] frequencies = MyMath.MyArray.SplitInterval(failureTimeOx, out intervalWidth);
            MyConsole.PrintArray(frequencies, "Частоты");
            Console.WriteLine("Длинна интервала: {0}", intervalWidth);
            //2. выборочная средняя
            double Xb = MyMath.Xb(frequencies, intervalWidth, N);
            Console.WriteLine("Выборочная средняя: {0}", Xb);
            //3. Лябда
            double lambda = 1 / Xb;
            Console.WriteLine("Лябда: {0}", lambda);
            //4. Распределение вероятностей
            double[] P = MyMath.MyArray.ProbabilityDistribution(frequencies, lambda, intervalWidth);
            MyConsole.PrintArray(P, "Распределение вероятностей");
            MyConsole.PrintChart(P);//вывод графика


            /*
           

            //Третий пункт
            double[] P = new double[intervali.Length - 1];
            double znam_temp = intervali[intervali.Length - 1] - intervali[0];
            for (int i = 0; i < P.Length; i++)
            {
                P[i] = Math.Exp(-lambda * (intervali[i])) - Math.Exp(-lambda * intervali[i + 1]);
            }

            //Четвертый пункт
            double[] nT = new double[P.Length];
            for (int i = 0; i < nT.Length; i++)
            {
                nT[i] = exp * P[i];
            }

            //Пятый пункт
            int[] nE = GetArrayOfAmountOfExperiments(experiments_new, intervali);
            double Xi = 0;
            for (int i = 0; i < nE.Length; i++)
            {
                Xi += Math.Pow(nE[i] - nT[i], 2) / nT[i];
            }

            int K = P.Length;
            double s = 1;
            double V = K - (s + 1);

            double tabl = V + Math.Sqrt(2 * V) * (-2.33) + 2.0 / 3.0 * Math.Pow(-2.33, 2) - 2.0 / 3.0 + (1.0 / Math.Sqrt(V));
            if (Xi > tabl) Console.WriteLine("Теория подтверждается");
            else Console.WriteLine("Теория отвергается");
            Console.WriteLine("Лямбда  " + lambda);
            */
            Console.ReadKey();
        }

        static double GetDoubleMiddle(double a, double b)
        {
            return (a + b) / 2;
        }


        /// <summary>
        /// Высчитывает количество элементов на интервале
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        static int CalculateMatchingNumbers(double min, double max, int[] array)
        {
            int tmp = 0;  //Счетчик
            //Перебор всех чисел
            foreach (var a in array)
            {
                if (a >= min && a <= max) tmp++;
            }
            return tmp;
        }

        static int[] GetArrayOfAmountOfExperiments(int[] array, double[] intervali)
        {
            int[] exitArray = new int[intervali.Length - 1];
            for (int i = 0; i < exitArray.Length; i++)
            {
                exitArray[i] = CalculateMatchingNumbers(intervali[i], intervali[i + 1], array);
            }
            return exitArray;
        }
    }
}
