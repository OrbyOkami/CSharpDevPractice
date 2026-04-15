namespace Task3
{
    class Rectangle
    {
        public double X { get; }
        public double Y { get; }

        private double width;
        private double height;

        public double Width
        {
            get => width;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Width не может быть отрицательной");
                width = value;
            }
        }

        public double Height
        {
            get => height;
            set
            {
                if (value < 0)
                    throw new ArgumentException("Height не может быть отрицательной");
                height = value;
            }
        }

        public double Area => Width * Height;

        public double Perimeter => 2 * (Width + Height);

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;     // идёт через setter → проверка
            Height = height;   // идёт через setter → проверка
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Rectangle rect = new Rectangle(10, 20, 5, 3);

                Console.WriteLine($"Координаты: ({rect.X}, {rect.Y})");
                Console.WriteLine($"Ширина: {rect.Width}");
                Console.WriteLine($"Высота: {rect.Height}");
                Console.WriteLine($"Площадь: {rect.Area}");
                Console.WriteLine($"Периметр: {rect.Perimeter}");

                // Попытка задать некорректное значение
                rect.Width = 10; // вызовет исключение
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
