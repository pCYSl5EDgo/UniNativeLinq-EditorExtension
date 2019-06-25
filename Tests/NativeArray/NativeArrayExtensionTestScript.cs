//using NUnit.Framework;
//using UniNativeLinq;
//using Unity.Collections;
//using UnityEngine;

//// ReSharper disable InconsistentNaming

//namespace Tests.NativeArray
//{
//    public class NativeArrayExtensionTestScript
//    {
//        [Test]
//        public void X()
//        {
//            var array = new NativeArray<int>(32, Allocator.Temp);
//            for (int i = 0; i < array.Length; i++)
//            {
//                array[i] = i;
//            }
//            var enumerable = new NativeEnumerable<int>(array);

//            var xs = enumerable.OrderByDescending();
//            foreach (var x in xs)
//            {
//                Debug.Log(x);
//            }
//            array.Dispose();
//        }
//        /*
//        private const long ArrayLength = 128L;
//        private NativeArray<long> longs;

//        private void SetUp()
//        {
//            longs = new NativeArray<long>((int) ArrayLength, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
//            long index = 0;
//            foreach (ref var i in longs.AsRefEnumerable())
//                i = index++;
//        }

//        private void Dispose()
//        {
//            longs.Dispose();
//        }

//        [Test]
//        public void AnyTest()
//        {
//            SetUp();
//            Assert.IsTrue(longs.Any());
//            Dispose();
//        }

//        [Test]
//        public void CountTest()
//        {
//            SetUp();
//            Assert.AreEqual(ArrayLength, longs.Count());
//            Dispose();
//        }

//        [Test]
//        public void TryGetAverageTest()
//        {
//            SetUp();
//            Assert.IsTrue(longs.TryGetAverage(out var value));
//            Assert.AreEqual((ArrayLength >> 1) * (ArrayLength - 1) / ArrayLength, value);
//            Dispose();
//        }

//        [Test]
//        public void AppendTest()
//        {
//            SetUp();
//            var enumerable = longs.Append(ArrayLength);
//            var i = 0L;
//            Assert.AreEqual(ArrayLength + 1, enumerable.LongCount());
//            foreach (ref var x in enumerable)
//            {
//                Debug.Log(x);
//                Assert.AreEqual(i++, x);
//            }
//            Assert.AreEqual(ArrayLength + 1, i);
//            Dispose();
//        }

//        [Test]
//        public void DefaultIfTest()
//        {
//            SetUp();
//            NativeArray<long> array = default;
//            Assert.IsTrue(array.IsEmpty());
//            var enumerable = array.DefaultIfEmpty(128L);
//            Assert.IsTrue(enumerable.TryGetFirst(out var first));
//            Assert.AreEqual(128L, first);
//            Dispose();
//        }

//        [Test]
//        public void DistinctTest()
//        {
//            SetUp();
//            var duplicate = new NativeArray<long>((int) ArrayLength << 1, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
//            for (var i = 0L; i < ArrayLength; i++)
//            {
//                duplicate[(int) i << 1] = i;
//                duplicate[(int) (i << 1) + 1] = i;
//            }
//            Assert.IsTrue(longs.SequenceEqual(duplicate.Distinct()));
//            duplicate.Dispose();
//            Dispose();
//        }

//        [Test]
//        public void OrderByTest()
//        {
//            SetUp();
//            var descending = longs.OrderByDescending();
//            var i = ArrayLength;
//            foreach (ref var x in descending)
//                Assert.AreEqual(--i, x);
//            Assert.AreEqual(0, i);
//            i = 0;
//            foreach (ref var x in descending.OrderBy())
//                Assert.AreEqual(i++, x);
//            Assert.AreEqual(ArrayLength, i);
//            Dispose();
//        }

//        [Test]
//        public void ToPartitionTest()
//        {
//            SetUp();
//            var (True, False) = longs.ToPartition(x => (x & 1) == 1);
//            Assert.AreEqual(True.Length, False.Length);
//            Assert.AreEqual(ArrayLength >> 1, True.Length);
//            for (var i = 0L; i < True.Length; i++)
//                Assert.AreEqual((i << 1) + 1, True[i]);
//            for (var i = 0L; i < False.Length; i++)
//                Assert.AreEqual(i << 1, False[i]);
//            Dispose();
//        }

//        [Test]
//        public void PrependTest()
//        {
//            SetUp();
//            var enumerable = longs.Prepend(-1);
//            var i = -1;
//            Assert.AreEqual(ArrayLength + 1, enumerable.LongCount());
//            foreach (ref var x in enumerable)
//                Assert.AreEqual(i++, x);
//            Dispose();
//        }

//        [Test]
//        public void ReverseTest()
//        {
//            SetUp();
//            var enumerable = longs.Reverse();
//            var i = ArrayLength;
//            foreach (ref var x in enumerable)
//              Assert.AreEqual(--i, x);
//            Dispose();
//        }

//        [Test]
//        public void SelectIndexTest()
//        {
//            SetUp();
//            var j = 0L;
//            foreach (ref var x in longs.SelectIndex((x, i) => x * i))
//                Assert.AreEqual(j * j++, x);
//            Dispose();
//        }

//        [Test]
//        public void SelectTest()
//        {
//            SetUp();
//            var j = 0L;
//            foreach (ref var x in longs.Select(x => x * 4))
//                Assert.AreEqual(j++ * 4, x);
//            Dispose();
//        }

//        [Test]
//        public void SelectManyTest()
//        {
//            SetUp();
//            var enumerable = longs.SelectMany(x => Enumerable.Repeat(x, 2));
//            Assert.AreEqual(2 * ArrayLength, enumerable.LongCount());
//            var i = 0L;
//            foreach (ref var x in enumerable)
//            {
//                Debug.Log(x);
//                Assert.AreEqual(i++ >> 1, x);
//            }
//            Dispose();
//        }

//        [Test]
//        public void SkipTest()
//        {
//            SetUp();
//            var i = ArrayLength >> 1;
//            foreach (ref var x in longs.Skip(ArrayLength >> 1))
//                Assert.AreEqual(i++, x);
//            Assert.AreEqual(ArrayLength, i);
//            i = 0;
//            foreach (ref var x in longs.SkipLast(ArrayLength >> 1))
//                Assert.AreEqual(i++, x);
//            Assert.AreEqual(ArrayLength >> 1, i);
//            i = 30L;
//            foreach (ref var x in longs.SkipWhile(x => x < 30L))
//                Debug.Log(x);
//            Dispose();
//        }
//        */
//    }
//}