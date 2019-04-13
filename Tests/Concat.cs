using NUnit.Framework;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ.Test
{
    public class Concat
    {
        [Test]
        public void NativeArray_NativeArray()
        {
            var array0 = new NativeArray<int>(1024, Allocator.Temp);
            var array1 = new NativeArray<int>(4096, Allocator.Temp);
            Assert.AreEqual(array0.Length + array1.Length, array0.Concat(array1).Count());
            array0.Dispose();
            array1.Dispose();
        }
    }
}