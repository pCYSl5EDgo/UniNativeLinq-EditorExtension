using Unity.Collections;
using NUnit.Framework;

using static NUnit.Framework.Assert;

namespace pcysl5edgo.Collections.LINQ.Test
{
    public class NativeArrayExtensionTests
    {
        private NativeArray<int> array, zero;
        private readonly System.Func<int, bool> pEqualOrGreaterThan100 = x => x * x >= 100;
        private readonly System.Func<int, bool> pLessThan10 = x => x < 10;
        private readonly TestPredicate tp0 = new TestPredicate(0);
        private readonly TestPredicate tp10 = new TestPredicate(10);

        [SetUp]
        public void SetUp()
        {
            zero = default;
            array = new NativeArray<int>(10, Allocator.Temp);
            for (var i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }
        }
        [TearDown]
        public void TearDown()
        {
            array.Dispose();
        }

        [Test]
        public void AnyTest()
        {
            IsFalse(zero.Any());
            IsFalse(zero.Any(pLessThan10));
            IsFalse(zero.Any(pEqualOrGreaterThan100));
            IsFalse(zero.Any(tp10));
            IsTrue(array.Any());
            IsTrue(array.Any(pLessThan10));
            IsFalse(array.Any(pEqualOrGreaterThan100));
            IsTrue(array.Any(tp10));
            IsFalse(array.Any(tp0));
        }

        private readonly struct TestPredicate : IRefFunc<int, bool>
        {
            readonly int lessThan;
            public TestPredicate(int lessThan) => this.lessThan = lessThan;
            public bool Calc(ref int arg0) => arg0 < lessThan;
        }

        [Test]
        public void AllTest()
        {
            IsFalse(zero.All(pEqualOrGreaterThan100));
            IsFalse(zero.All(pLessThan10));
            IsFalse(zero.All(tp0));
            IsFalse(zero.All(tp10));
            IsFalse(array.All(pEqualOrGreaterThan100));
            IsTrue(array.All(pLessThan10));
            IsFalse(array.All(tp0));
            IsTrue(array.All(tp10));
        }

        [Test]
        public void AggregateTest()
        {
            AreEqual("45", array.Aggregate(0UL, (accum, elem) => accum + (ulong)elem, accum => accum.ToString()));
            AreEqual(45, array.Aggregate(0UL, (accum, elem) => accum + (ulong)elem));
            AreEqual(45, array.Aggregate<int>(0, (accum, elem) => accum + elem));
            Aggregate0 agg = default;
            AreEqual("45", array.Aggregate<int, ulong, string, Aggregate0, Aggregate0>(0UL, agg, agg));
            AreEqual(45, array.Aggregate(0UL, agg));
            var seed = 0;
            array.Aggregate<int, Aggregate0>(ref seed, agg);
            AreEqual(45, seed);
            AreEqual(45, array.Aggregate<int, Aggregate0>(0, agg));
        }

        struct Aggregate0 : IRefAction<int, int>, IRefAction<ulong, int>, IRefFunc<ulong, string>
        {
            public string Calc(ref ulong arg0) => arg0.ToString();
            public void Execute(ref int accum, ref int element)
            {
                accum += element;
            }
            public void Execute(ref ulong accum, ref int element)
            {
                accum += (ulong)element;
            }
        }

        [Test]
        public void CastTest()
        {
            var floats = array.Cast<int, float, Cast0>(Allocator.Temp, new Cast0());
            for (int i = 0; i < floats.Length; i++)
                AreEqual((float)array[i], floats[i]);
            floats.Dispose();
        }

        struct Cast0 : IRefAction<int, float>
        {
            public void Execute(ref int arg0, ref float arg1) => arg1 = arg0;
        }

        [Test]
        public void ContainsTest()
        {
            for (int i = 0; i < array.Length; i++)
            {
                IsFalse(zero.Contains(i));
                IsTrue(array.Contains(i));
                IsTrue(array.Contains(i, new Contains0()));
                IsFalse(zero.Contains(i, new Contains0()));
            }
            IsFalse(array.Contains(-100));
        }
        struct Contains0 : IRefFunc<int, int, bool>
        {
            public bool Calc(ref int arg0, ref int arg1) => arg0 == arg1;
        }

        [Test]
        public void CountTest()
        {
            AreEqual(0, zero.Count());
            AreEqual(0L, zero.LongCount());
            AreEqual(array.Length, array.Count());
            AreEqual((long)array.Length, array.LongCount());
            AreEqual(5, array.Count(new Count0()));
            AreEqual(5, array.Count(x => x < 5));
        }
        struct Count0 : IRefFunc<int, bool>
        {
            public bool Calc(ref int arg0) => arg0 < 5;
        }

        [Test]
        public void AverageTest()
        {
            AreEqual(0, zero.Average());
            AreEqual(4, array.Average());
        }

        [Test]
        public void ElementAtTest()
        {
            for (int i = 0; i < array.Length; i++)
            {
                IsTrue(array.TryGetElementAt(i, out var value));
                AreEqual(i, value);
                AreEqual(i, array.ElementAt(i));
            }
        }

        [Test]
        public void EmptyTest()
        {
            IsFalse(Enumerable.Empty<float>().IsCreated);
        }

        [Test]
        public void FirstAndLastTest()
        {
            var xs = new NativeArray<float>(15, Allocator.Temp);
            for (int i = 0; i < xs.Length; i++)
                xs[i] = i * i + 1;
            AreEqual(0, zero.FirstOrDefault());
            AreEqual(0, zero.LastOrDefault());
            AreEqual(1f, xs.First());
            AreEqual(1f, xs.FirstOrDefault());
            AreEqual(197f, xs.Last());
            AreEqual(197f, xs.LastOrDefault());
            xs.Dispose();
        }

        [Test]
        public void MinMaxTest()
        {
            AreEqual(0, array.Min());
            AreEqual(9, array.Max());
            IsTrue(array.TryGetMinMax(out var min, out var max));
            AreEqual(0, min);
            AreEqual(9, max);
            IsFalse(zero.TryGetMinMax(out _, out _));
        }

        [Test]
        public void RangeTest()
        {
            using (var ints = Enumerable.Range(1, 5, Allocator.Temp))
            using (var floats = Enumerable.Range(1f, 5, Allocator.Temp))
            using (var uints = Enumerable.Range(1u, 5, Allocator.Temp))
            using (var ulongs = Enumerable.Range(1ul, 5, Allocator.Temp))
            using (var doubles = Enumerable.Range(1.0, 5, Allocator.Temp))
            {
                for (int i = 0; i < 5; i++)
                {
                    AreEqual(i + 1, ints[i]);
                    AreEqual(i + 1, uints[i]);
                    AreEqual(i + 1, ulongs[i]);
                    AreEqual(i + 1, floats[i]);
                    AreEqual(i + 1, doubles[i]);
                }
            }
        }

        [Test]
        public void RepeatTest()
        {
            TestPredicate source = new TestPredicate(342);
            using (var array2 = Enumerable.Repeat(source, 114, Allocator.Temp))
            {
                for (int i = 0; i < array2.Length; i++)
                    AreEqual(source, array2[i]);
            }
        }

        [Test]
        public void SingleTest()
        {
            AreEqual(0, array.Single(new TestPredicate(1)));
            AreEqual(4, array.Single(x => x == 4));
            IsFalse(array.TryGetSingle(out _, new TestPredicate(0)));
            IsFalse(array.TryGetSingle(out _, new TestPredicate(6)));
            IsTrue(array.TryGetSingle(out var value, new TestPredicate(1)));
            AreEqual(0, value);
        }

        [Test]
        public void SumTest()
        {
            AreEqual(0, zero.Sum());
            AreEqual(45, array.Sum());
        }

        [Test]
        public void DictionaryTest()
        {
            var dictionary = array.ToDictionary(x => x, x => x.ToString());
            for (int i = 0; i < array.Length; i++)
                AreEqual(i.ToString(), dictionary[i]);
            dictionary = array.ToDictionary<int, int, string, Dictionary0, Dictionary0>(new Dictionary0(), new Dictionary0());
            for (int i = 0; i < array.Length; i++)
                AreEqual((i << 1).ToString(), dictionary[i]);
        }

        struct Dictionary0 : IRefFunc<int, int>, IRefFunc<int, string>
        {
            public int Calc(ref int arg0) => arg0;

            string IRefFunc<int, string>.Calc(ref int arg0) => (arg0 << 1).ToString();
        }

        [Test]
        public void ListTest()
        {
            var list = array.ToList();
            for (int i = 0; i < array.Length; i++)
                AreEqual(i, list[i]);
        }
    }
}
