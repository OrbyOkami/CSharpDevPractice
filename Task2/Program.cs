namespace Task2
{
    internal class Program
    {
        static void PrintDiamond(int n)
        {
            if (n <= 0 || n % 2 == 0)
            {
                Console.WriteLine("N должно быть положительным нечётным числом");
                return;
            }

            int mid = n / 2;

            // Верхняя часть (включая середину)
            for (int i = 0; i <= mid; i++)
            {
                int outerSpaces = mid - i;
                int innerSpaces = i * 2 - 1;

                Console.Write(new string(' ', outerSpaces));

                if (i == 0)
                {
                    Console.WriteLine("X");
                }
                else
                {
                    Console.Write("X");
                    Console.Write(new string(' ', innerSpaces));
                    Console.WriteLine("X");
                }
            }

            // Нижняя часть
            for (int i = mid - 1; i >= 0; i--)
            {
                int outerSpaces = mid - i;
                int innerSpaces = i * 2 - 1;

                Console.Write(new string(' ', outerSpaces));

                if (i == 0)
                {
                    Console.WriteLine("X");
                }
                else
                {
                    Console.Write("X");
                    Console.Write(new string(' ', innerSpaces));
                    Console.WriteLine("X");
                }
            }
        }
        static void Main(string[] args)
        {
            PrintDiamond(3);
        }
    }
}
