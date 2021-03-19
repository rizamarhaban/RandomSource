# Random Source with Store-Reload to Continue

## My Issue
Recently I had some simulation project to dealt with. The existing code has a Random number that is part of stochastic discrete-event simulation code and it is deterministic. The issue is, how do we run tens of thousands random generated values with a specified seed and then require to be paused or stop and continue the next time with the same seed next value. Kind of like we need to have indexing on the random generated value. I also don't understand random number generator (RNG) algorithm, so I need to find and use simple solution as currently I cannot find the solution of this in internet or maybe I missed the search. Anyway,

## Solution #1 ← *not so good*
Generate as much Random numbers as required and put in a List<>. The issue with this is the Random values can be `int`, `double` and sometimes with `minValue` or `maxValue`. We also know that the next value after `int` or `double` can be different. It depends on which dice were thrown first. However, as the discrete-event simulation model is getting complex and using discrete, categorical or continuous distribution random variables, it is impossible to use listed random numbers, because we might don't know which will be called out first, is it `Random.Next()`, or `Random.NextDouble()`, etc. This solution probably working if we are dealing with single value type of random number.

## Solution #2
With that issue, I created a random source that can store the values generated and can be converted to `Json` string. This json string can then be loaded back and restore the random source *state* to continue to the last next value. This solution might not work for some people, but it surely works for my case. Here is how I did it,

As the idea is simple, we just capturing whatever the random value has been generated, index it and store it in a simple `struct` model. Every time the `Random.Next()` or `Random.NextDouble()` were called, it will be stored in a read only list. Once it is required to be stopped/paused, we can then collect this list and convert it into `Json` string. Once we need it back, we just read the list, follow the same random called pattern by looping until the last index reach. This call following the type and the parameters as required. That's all. I believe, it has lots of limitations (probably), but again it works fine for my case as of now. Here is the demo code from `Program.cs`;

## Demo Code

``` CSharp
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

    Console.WriteLine("Random #1 Generated Values");
    Console.WriteLine("rs.Next();");
    Console.WriteLine("rs.Next(10);");
    Console.WriteLine("rs.NextDouble();");
    Console.WriteLine("rs.Next(2, 15);");
    Console.WriteLine("rs.NextDouble();");
    Console.WriteLine("rs.Next(15);");
    Console.WriteLine();

    var json = rs.ValuesAsJson;
    Console.WriteLine("Random #1 Generated Values in Json");
    Console.WriteLine(json);
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
```

### The Output

``` Text
Random #1 Generated Values
rs.Next();
rs.Next(10);
rs.NextDouble();
rs.Next(2, 15);
rs.NextDouble();
rs.Next(15);

Random #1 Generated Values in Json
[
  {
    "Index": 1,
    "Value": 1559595546,
    "MinValue": 0,
    "MaxValue": 0,
    "Type": "Int32"
  },
  {
    "Index": 2,
    "Value": 8,
    "MinValue": 0,
    "MaxValue": 10,
    "Type": "Int32"
  },
  {
    "Index": 3,
    "Value": 0.7680226893946634,
    "Type": "Double"
  },
  {
    "Index": 4,
    "Value": 9,
    "MinValue": 2,
    "MaxValue": 15,
    "Type": "Int32"
  },
  {
    "Index": 5,
    "Value": 0.2060331540210327,
    "Type": "Double"
  },
  {
    "Index": 6,
    "Value": 8,
    "MinValue": 0,
    "MaxValue": 15,
    "Type": "Int32"
  }
]

Random #2 Continue From Previous Values (in Json)
9
0.44217787331071584

Random #1 Continue Values
9
0.44217787331071584
```
The main point is generating this `Json`;

``` JSON
[
  {
    "Index": 1,
    "Value": 1559595546,
    "MinValue": 0,
    "MaxValue": 0,
    "Type": "Int32"
  },
  {
    "Index": 2,
    "Value": 8,
    "MinValue": 0,
    "MaxValue": 10,
    "Type": "Int32"
  },
  {
    "Index": 3,
    "Value": 0.7680226893946634,
    "Type": "Double"
  },
  {
    "Index": 4,
    "Value": 9,
    "MinValue": 2,
    "MaxValue": 15,
    "Type": "Int32"
  },
  {
    "Index": 5,
    "Value": 0.2060331540210327,
    "Type": "Double"
  },
  {
    "Index": 6,
    "Value": 8,
    "MinValue": 0,
    "MaxValue": 15,
    "Type": "Int32"
  }
]
```

Feedback the `Json` to the `RandomSource`, and it will recall each of the random item. Looping until it reaches the last index. Once we continue, it will generate the next random as required. As this shown from **Random #2** and **Random #1** next values are the same.

Hopefully, this simple class can be very useful for anyone who has the same issue as I am. Enjoy!



