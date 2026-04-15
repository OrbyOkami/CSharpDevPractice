using System.Text;

namespace Test
{
    internal class Program
    {
        static string percentAnswer(double initial_deposit, int years, double interest_rate)
        {
            var answer = new StringBuilder();
            for (int i = 1; i < years + 1; i++)
            {
                double amount = Math.Round(initial_deposit * Math.Pow((1 + (interest_rate / 100)), i), 2);
                answer.Append($"Год {i}: {amount:F2} руб. \n");
            }
            return answer.ToString();
        }
        static void Main(string[] args)
        {
            Console.WriteLine(percentAnswer(1000, 3, 10));
        }
    }
}
