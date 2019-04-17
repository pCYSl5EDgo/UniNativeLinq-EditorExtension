using System.Linq;
using NUnit.Framework;
using Unity.Collections;
using UnityEngine;

namespace pcysl5edgo.Collections.LINQ.Test
{
    public class Distinct
    {
        [Test]
        public void FromNativeArray()
        {
            var array = new NativeArray<int>(4, Allocator.Temp)
            {
                [0] = 0,
                [1] = 1,
                [2] = 2,
                [3] = 3,
            };
            var distinct = array.Distinct();
            Assert.IsFalse(distinct.CanFastCount());

            var enumerator = distinct.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
            enumerator.Dispose();

            array[3] = 0;
            enumerator = distinct.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
            enumerator.Dispose();

            array[3] = 0;
            array[2] = 1;
            array[1] = 2;
            array[0] = 3;
            enumerator = distinct.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
            enumerator.Dispose();

            array.Dispose();
        }

        private struct Div : IRefAction<int, int>
        {
            public void Execute(ref int arg0, ref int arg1)
            {
                arg1 = arg0 % 10;
            }
        }
        [Test]
        public void RangeSelectDistinctTest()
        {
            var range = Enumerable.Range(0, 400).Select<int, Div>(new Div()).ToNativeArray(Allocator.Temp);
            Assert.AreEqual(400, range.Length);
            var distinct = range.Distinct();
            var enumerator = distinct.GetEnumerator();
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(0, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(1, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(2, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(3, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(4, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(5, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(6, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(7, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(8, enumerator.Current);
            Assert.IsTrue(enumerator.MoveNext());
            Assert.AreEqual(9, enumerator.Current);
            Assert.IsFalse(enumerator.MoveNext());
            enumerator.Dispose();
            
            Assert.AreEqual(10, distinct.Count());
            range.Dispose();
        }
    }
}