using System;

namespace RandomSaveRestoreContinue
{
    class Program
    {
        static void Main(string[] args)
        {
			const int seed = 0;

			RandomSource rs = new(seed);
			rs.Next();
			rs.Next(10);
			rs.NextDouble();
			rs.Next(2, 15);
			rs.NextDouble();
			rs.Next(15);

			var json = rs.ValuesAsJson;
			Console.WriteLine("Random #1 Generated Values in Json");
			Console.WriteLine(rs.ValuesAsJson);
			Console.WriteLine();

			Console.WriteLine("Random #2 Continue From Previous Values (in Json)");
			RandomSource rsContinue = new(seed, json);
			Console.WriteLine(rsContinue.Next(10));
			Console.WriteLine(rsContinue.NextDouble());
			Console.WriteLine();

			Console.WriteLine("Random #1 Continue Values");
			Console.WriteLine(rs.Next(10));
			Console.WriteLine(rs.NextDouble());
			Console.WriteLine();

			Console.ReadKey();

		}
    }
}
