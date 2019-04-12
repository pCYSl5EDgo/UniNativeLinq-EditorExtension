using NUnit.Framework;
using Unity.Collections;
using UnityEngine;
using Debug = UnityEngine.Debug;
using LE = System.Linq.Enumerable;
using Random = UnityEngine.Random;

namespace pcysl5edgo.Collections.LINQ.Test
{
    public sealed class NativeEnumerableSpeedTests
    {
        private const int Count = 1000;
        private SW sw;

        [SetUp]
        public void SetUp()
        {
            sw = SW.Create();
        }

        private struct Multiply : IRefAction<Matrix4x4, Matrix4x4>
        {
            public void Execute(ref Matrix4x4 arg0, ref Matrix4x4 arg1) => arg0 *= arg1;
        }

        [Test]
        public void Aggregate_Matrix4x4_Multiply_Test()
        {
            using (var matrices = new NativeArray<Matrix4x4>(1024, Allocator.Persistent))
            {
                InitializeMatrixArray(matrices);
                var identity = Matrix4x4.identity;
                sw.Start();
                for (var i = 0; i < Count; i++)
                    identity = matrices.Aggregate<Matrix4x4>(identity, (accumulate, seed) => accumulate * seed);
                Debug.Log(sw.Stop().ToString());

                sw.Start();
                for (var i = 0; i < Count; i++)
                    matrices.Aggregate<Matrix4x4, Multiply>(ref identity, default);
                Debug.Log(sw.Stop().ToString());

                identity = Matrix4x4.identity;
                sw.Start();
                for (var i = 0; i < Count; i++)
                    identity = LE.Aggregate(matrices, identity, (accumulate, seed) => accumulate * seed);
                Debug.Log(sw.Stop().ToString());
            }
        }

        private void InitializeMatrixArray(NativeArray<Matrix4x4> matrices)
        {
            foreach (ref var matrix in matrices.AsRefEnumerable())
                for (var i = 0; i < 4; i++)
                for (var j = 0; j < 4; j++)
                    matrix[i, j] = Random.Range(0.00001f, 1f);
        }

        [Test]
        public void Aggregate_Int32_Add_Test()
        {
            using (var array = new NativeArray<int>(114514, Allocator.Temp))
            {
                InitializeIntArray(array);
                sw.Start();
                for (var i = 0; i < Count; i++)
                    array.Average();
                Debug.Log(sw.Stop().ToString());

                sw.Start();
                for (var i = 0; i < Count; i++)
                    LE.Average(array);
                Debug.Log(sw.Stop().ToString());
            }
        }

        private void InitializeIntArray(NativeArray<int> array)
        {
            foreach (ref var i in array.AsRefEnumerable())
                i = Random.Range(1, 1000);
        }

        [Test]
        public void WhereCount_long_Test()
        {
            using (var array = new NativeArray<long>(114514, Allocator.Temp))
            {
                InitializeLongArray(array);
                sw.Start();
                for (var i = 0; i < Count; i++)
                    array.Append(1000).Where(new DetectOdd()).Append(1000).Count();
                Debug.Log(sw.Stop().ToString());

                sw.Start();
                for (var i = 0; i < Count; i++)
                    LE.Count(LE.Append(LE.Where(LE.Append(array, 1000), x => (x & 1) == 1), 1000));
                Debug.Log(sw.Stop().ToString());
            }
        }

        private struct DetectOdd : IRefFunc<long, bool>
        {
            public bool Calc(ref long arg0) => (arg0 & 1) == 1;
        }

        private void InitializeLongArray(NativeArray<long> array)
        {
            for (var i = 0; i < array.Length; i++)
                array[i] = i;
        }
    }
}