using NUnit.Framework;
using pcysl5edgo.Collections.LINQ;
using Unity.Collections;
using UnityEngine;

namespace Tests
{
    public class NewTestScript
    {
        // A Test behaves as an ordinary method
        [Test]
        public void NewTestScriptSimplePasses()
        {
            var array = new NativeArray<long>(64, Allocator.Temp);
            var index = 0L;
            foreach (ref var item in array.AsRefEnumerable())
                item = index++;
            if (array.TryGetMax(out var y))
                Debug.Log(y.ToString());
            foreach (ref var item in array.Select(x => x << 1).WithIndex())
                Assert.AreEqual(item.Item1, item.Item2 << 1);
            var xs = array.SelectIndex((x, i) => x * i);
            foreach (ref var x in xs)
                Debug.Log(x.ToString());
            array.Dispose();
        }
    }
}