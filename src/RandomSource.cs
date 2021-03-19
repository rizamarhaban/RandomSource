using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RandomSaveRestoreContinue
{
    public class RandomSource : Random
    {
        private readonly int seed = 0;
        private readonly HashSet<IRandomValue> values = new();

        private int index = 0;
        private Random rnd = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public RandomSource(int seed)
        {
            this.seed = seed;
            this.rnd = new(this.seed);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="previousValues">The previous values.</param>
        public RandomSource(int seed, IRandomValue[] previousValues) : this(seed)
        {
            AdvanceRandomToNextIndex(previousValues);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RandomSource"/> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        /// <param name="previousValuesFromJson">The previous values from json.</param>
        /// <exception cref="ArgumentNullException">Missing Json string</exception>
        public RandomSource(int seed, string previousValuesFromJson) : this(seed)
        {
            if (string.IsNullOrEmpty(previousValuesFromJson))
                throw new ArgumentNullException("Missing Json string");

            AdvanceRandomToNextIndex(JsonSerializer.Deserialize<IRandomValue[]>(previousValuesFromJson));
        }

        private void AdvanceRandomToNextIndex(IRandomValue[] previousValues)
        {
            for (int i = 0; i < previousValues.Length; i++)
            {
                var val = previousValues[i];
                if (val.Type == TypeCode.Int32)
                {
                    var intValue = (RandomIntegerValue)val;
                    if (intValue.MinValue == 0 && intValue.MaxValue == 0)
                    {
                        rnd.Next();
                    }
                    else
                    {
                        rnd.Next(intValue.MinValue, intValue.MaxValue);
                    }
                }
                else
                {
                    if (val.Type == TypeCode.Double)
                        rnd.Next();
                }
            }
        }

        /// <summary>
        /// Gets the random generated values.
        /// </summary>
        public IReadOnlyList<IRandomValue> Values
            => new ReadOnlyCollection<IRandomValue>(values.ToArray());

        /// <summary>
        /// Gets the random generated values as Json string.
        /// </summary>
        public string ValuesAsJson
            => JsonSerializer.Serialize(values, new JsonSerializerOptions { WriteIndented = true });

        /// <summary>
        /// Nexts this instance.
        /// </summary>
        /// <returns></returns>
        public new int Next()
        {
            index++;
            var val = rnd.Next();
            values.Add(new RandomIntegerValue(index, val, 0, 0));
            return val;
        }

        /// <summary>
        ///    Returns a non-negative random integer that is less than the specified maximum.
        /// </summary>
        /// <param name="max">
        ///    The exclusive upper bound of the random number to be generated. maxValue must
        ///    be greater than or equal to 0.
        /// </param>
        /// <returns>
        ///    A 32-bit signed integer that is greater than or equal to 0, and less than maxValue;
        ///    that is, the range of return values ordinarily includes 0 but not maxValue. However,
        ///    if maxValue equals 0, maxValue is returned.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///    maxValue is less than 0.
        /// </exception>
        public new int Next(int maxValue)
        {
            index++;
            var val = rnd.Next(maxValue);
            values.Add(new RandomIntegerValue(index, val, 0, maxValue));
            return val;
        }

        /// <summary>
        ///    Returns a random integer that is within a specified range.
        /// </summary>
        /// <param name="minMalue">
        ///    The inclusive lower bound of the random number returned.
        /// </param>
        /// <param name="maxValue">
        ///    The exclusive upper bound of the random number returned. maxValue must be greater.
        ///    than or equal to minValue.</param>
        /// <returns>
        ///    A 32-bit signed integer greater than or equal to minValue and less than maxValue;
        ///    that is, the range of return values includes minValue but not maxValue. If minValue
        ///    equals maxValue, minValue is returned.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        ///    minValue is greater than maxValue.
        /// </exception>
        public new int Next(int minMalue, int maxValue)
        {
            index++;

            var val = rnd.Next(minMalue, maxValue);
            values.Add(new RandomIntegerValue(index, val, minMalue, maxValue));

            return val;
        }

        /// <summary>
        ///   Returns a random floating-point number that is greater than or equal to 0.0,
        ///   and less than 1.0.
        /// </summary>
        /// <returns>
        ///   A double-precision floating point number that is greater than or equal to 0.0,
        ///   and less than 1.0.
        ///  </returns>
        public new double NextDouble()
        {
            index++;

            var val = rnd.NextDouble();
            values.Add(new RandomDoubleValue(index, val));

            return val;
        }

        /// <summary>
        ///   Reset the Random Source.
        /// </summary>
        public void Reset()
        {
            if (seed == 0)
                rnd = new();
            else
                rnd = new(seed);

            values.Clear();
            index = 0;
        }

        [JsonInterfaceConverter(typeof(RandomValueConverter))]
        public interface IRandomValue
        {
            /// <summary>
            /// Gets the index of this Random value.
            /// </summary>
            int Index { get; }

            /// <summary>
            /// Gets the type of this Random value.
            /// </summary>
            TypeCode Type { get; }
        }

        /// <summary>
        /// Stores the Random Integer Value
        /// </summary>
        /// <seealso cref="RandomSource.IRandomValue" />
        [Serializable]
        public readonly struct RandomIntegerValue : IRandomValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RandomIntegerValue"/> struct.
            /// </summary>
            /// <param name="index">The index number.</param>
            /// <param name="value">The Random value.</param>
            /// <param name="minValue">The minimum bound value.</param>
            /// <param name="maxValue">The maximum bound value.</param>
            [JsonConstructor]
            public RandomIntegerValue(int index, int value, int minValue, int maxValue)
            {
                Index = index;
                Value = value;
                MinValue = minValue;
                MaxValue = maxValue;
            }

            /// <summary>
            /// Gets the index number.
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// Gets the Random value.
            /// </summary>
            public int Value { get; }

            /// <summary>
            /// Gets the minimum value.
            /// </summary>
            public int MinValue { get; }

            /// <summary>
            /// Gets the maximum value.
            /// </summary>
            public int MaxValue { get; }

            /// <summary>
            /// Gets the type of this Random value.
            /// </summary>
            public TypeCode Type => TypeCode.Int32;
        }

        /// <summary>
        /// Stores the Random Double Value
        /// </summary>
        /// <seealso cref="RandomSource.IRandomValue" />
        [Serializable]
        public readonly struct RandomDoubleValue : IRandomValue
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="RandomDoubleValue"/> struct.
            /// </summary>
            /// <param name="index">The index number.</param>
            /// <param name="value">The Random value.</param>
            [JsonConstructor]
            public RandomDoubleValue(int index, double value)
            {
                Index = index;
                Value = value;
            }

            /// <summary>
            /// Gets the index number.
            /// </summary>
            public int Index { get; }

            /// <summary>
            /// Gets the Random value.
            /// </summary>
            public double Value { get; }

            /// <summary>
            /// Gets the type of this Random value.
            /// </summary>
            public TypeCode Type => TypeCode.Double;
        }

        /// <summary>
        /// Random Value Converter for converting IRandomValue to RandomIntegerValue or RandomDoubleValue
        /// </summary>
        /// <seealso cref="System.Text.Json.Serialization.JsonConverter{RandomSource.IRandomValue}" />
        private class RandomValueConverter : JsonConverter<IRandomValue>
        {
            public override IRandomValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                using var doc = JsonDocument.ParseValue(ref reader);
                var type = (TypeCode)doc.RootElement.GetProperty("Type").GetInt32();

                if (type == TypeCode.Int32)
                    return JsonSerializer.Deserialize<RandomIntegerValue>(doc.RootElement.GetRawText());

                return JsonSerializer.Deserialize<RandomDoubleValue>(doc.RootElement.GetRawText());
            }

            public override void Write(Utf8JsonWriter writer, IRandomValue value, JsonSerializerOptions options)
            {
                switch (value)
                {
                    case null:
                        JsonSerializer.Serialize(writer, (IRandomValue)null, options);
                        break;
                    default:
                        JsonSerializer.Serialize(writer, value, value.GetType(), options);
                        break;
                }
            }
        }

    }
}
