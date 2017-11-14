using System;

namespace Furcadia.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            var filename = @"F:\urcadia\dream.map";
            var dream = Dream.FromFile(filename);

            Console.WriteLine($"{dream.Name} {dream.Width}x{dream.Height}");

            Console.ReadLine();
        }
    }
}
