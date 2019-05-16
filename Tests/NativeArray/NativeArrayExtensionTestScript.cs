using NUnit.Framework;
using UniNativeLinq;
using Unity.Collections;
using UnityEngine;

// ReSharper disable InconsistentNaming

namespace Tests.NativeArray
{
    public class NativeArrayExtensionTestScript
    {
        private const long ArrayLength = 128L;
        private NativeArray<long> longs;

        [SetUp]
        public void SetUp()
        {
            longs = new NativeArray<long>((int) ArrayLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            long index = 0;
            foreach (ref var i in longs.AsRefEnumerable())
                i = index++;
        }

        [TearDown]
        public void Dispose()
        {
            longs.Dispose();
        }

        [Test]
        public void AnyTest() => Assert.IsTrue(longs.Any());

        [Test]
        public void CountTest() => Assert.AreEqual(ArrayLength, longs.Count());

        [Test]
        public void TryGetAverageTest()
        {
            Assert.IsTrue(longs.TryGetAverage(out var value));
            Assert.AreEqual((ArrayLength >> 1) * (ArrayLength - 1) / ArrayLength, value);
        }

        [Test]
        public void AppendTest()
        {
            var enumerable = longs.Append(ArrayLength);
            var i = 0L;
            Assert.AreEqual(ArrayLength + 1, enumerable.LongCount());
            foreach (ref var x in enumerable)
            {
                Debug.Log(x);
                Assert.AreEqual(i++, x);
            }
            Assert.AreEqual(ArrayLength + 1, i);
        }

        [Test]
        public void DefaultIfTest()
        {
            NativeArray<long> array = default;
            Assert.IsTrue(array.IsEmpty());
            var enumerable = array.DefaultIfEmpty(128L);
            Assert.IsTrue(enumerable.TryGetFirst(out var first));
            Assert.AreEqual(128L, first);
        }

        [Test]
        public void DistinctTest()
        {
            var duplicate = new NativeArray<long>((int) ArrayLength << 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (var i = 0L; i < ArrayLength; i++)
            {
                duplicate[(int) i << 1] = i;
                duplicate[(int) (i << 1) + 1] = i;
            }
            Assert.IsTrue(longs.SequenceEqual(duplicate.Distinct()));
            duplicate.Dispose();
        }

        [Test]
        public void OrderByTest()
        {
            var descending = longs.OrderByDescending();
            var i = ArrayLength;
            foreach (ref var x in descending)
                Assert.AreEqual(--i, x);
            Assert.AreEqual(0, i);
            i = 0;
            foreach (ref var x in descending.OrderBy())
                Assert.AreEqual(i++, x);
            Assert.AreEqual(ArrayLength, i);
        }

        [Test]
        public void ToPartitionTest()
        {
            var (True, False) = longs.ToPartition(x => (x & 1) == 1);
            Assert.AreEqual(True.Length, False.Length);
            Assert.AreEqual(ArrayLength >> 1, True.Length);
            for (var i = 0L; i < True.Length; i++)
                Assert.AreEqual((i << 1) + 1, True[i]);
            for (var i = 0L; i < False.Length; i++)
                Assert.AreEqual(i << 1, False[i]);
        }

        [Test]
        public void PrependTest()
        {
            var enumerable = longs.Prepend(-1);
            var i = -1;
            Assert.AreEqual(ArrayLength + 1, enumerable.LongCount());
            foreach (ref var x in enumerable)
                Assert.AreEqual(i++, x);
        }

        [Test]
        public void ReverseTest()
        {
            var enumerable = longs.Reverse();
            var i = ArrayLength;
            foreach (ref var x in enumerable)
              Assert.AreEqual(--i, x);
        }

        [Test]
        public void SelectIndexTest()
        {
            var j = 0L;
            foreach (ref var x in longs.SelectIndex((x, i) => x * i))
                Assert.AreEqual(j * j++, x);
        }

        [Test]
        public void SelectTest()
        {
            var j = 0L;
            foreach (ref var x in longs.Select(x => x * 4))
                Assert.AreEqual(j++ * 4, x);
        }
    }
}