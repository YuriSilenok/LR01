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

        public static void PrintArray(int[] array, string mess = "")
        {
            if (mess != string.Empty)
            {
                Console.WriteLine();
                Console.WriteLine(mess);
            }
            Console.WriteLine("{0}\n", string.Join(" ", array));
        }

        public static void PrintChart(int[] array)
        {
            double[] result = new double[array.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = array[i];
            }
            PrintChart(result);
        }

        public static void PrintChart(double[] array)
        {
            int x = Console.CursorLeft;
            int y = Console.CursorTop;
            double max = array.Max();
            int pow = 0;
            while (max < 10)
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
                    result[i] = result[i - 1] + array[i];
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


            public static int[] TheoreticalFrequencies(double[] P, int N)
            {
                int[] result = new int[P.Length];
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = (int)(P[i] * N + 0.5);
                }
                return result;
            }

            public static double[] ProbabilityDistribution(int[] frequencies, double lambda, double intervalWidth)
            {
                double[] result = new double[frequencies.Length];
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

        public static double PearsonsCriterion(int[] xY, int[] xT)
        {
            double result = 0;
            for (int i = 0; i < xY.Length; i++)
            {
                result += Math.Pow(xY[i] - xT[i], 2) / xT[i];
            }
            return result;
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
            MyConsole.PrintChart(frequencies);
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
            //5. Теоретические частоты
            int[] nT = MyMath.MyArray.TheoreticalFrequencies(P, N);
            MyConsole.PrintArray(nT, "Теоретические частоты");
            MyConsole.PrintChart(nT);
            //6. Найти критерий Пирсона
            double X = MyMath.PearsonsCriterion(frequencies, nT);
            Console.WriteLine("Критерий Пирсона: {0}", X);
            //7. Проверка теории
            double V = P.Length - 2; //K-(s+1) где s=1, K=P.Length
            Console.WriteLine("Количество степеней свободы: {0}", V);
            double tabl = V + Math.Sqrt(2 * V) * (-2.33) + 2.0 / 3.0 * Math.Pow(-2.33, 2) - 2.0 / 3.0 + (1.0 / Math.Sqrt(V));
            Console.WriteLine("Процентная точка распределения X^2: {0}", tabl);
            if (X > tabl) Console.WriteLine("Теория подтверждена");
            else Console.WriteLine("Теория отвергнута");
            Console.ReadKey();
        }
    }
}