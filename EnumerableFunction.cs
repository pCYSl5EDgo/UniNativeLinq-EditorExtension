using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections.LINQ
{
    public static unsafe class Enumerable
    {
        #region  Any
        public static bool Any<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            if (enumerator.MoveNext())
            {
                enumerator.Dispose();
                return true;
            }
            else
            {
                enumerator.Dispose();
                return false;
            }
        }
        public static bool Any<TEnumerable, TEnumerator, TSource, TPredicate>(ref this TEnumerable @this, TPredicate predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (predicate.Calc(ref enumerator.Current))
                {
                    enumerator.Dispose();
                    return true;
                }
            }
            enumerator.Dispose();
            return false;
        }

        public static bool Any<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => array.IsCreated && array.Length != 0;
        public static bool Any<TSource, TPredicate>(this NativeArray<TSource> array, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            if (!array.Any()) return false;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    return true;
            return false;
        }
        public static bool Any<TSource>(this NativeArray<TSource> array, Func<TSource, bool> predicate)
            where TSource : unmanaged
        {
            if (!array.Any()) return false;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (predicate(*ptr))
                    return true;
            return false;
        }
        #endregion

        #region All
        public static bool All<TEnumerable, TEnumerator, TSource, TPredicate>(ref this TEnumerable @this, TPredicate predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (!predicate.Calc(ref enumerator.Current))
                {
                    enumerator.Dispose();
                    return false;
                }
            }
            enumerator.Dispose();
            return true;
        }

        public static bool All<TSource, TPredicate>(this NativeArray<TSource> array, TPredicate predicate)
            where TPredicate : struct, IRefFunc<TSource, bool>
            where TSource : unmanaged
        {
            if (!array.Any()) return false;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (!predicate.Calc(ref *ptr))
                    return false;
            return true;
        }
        public static bool All<TSource>(this NativeArray<TSource> array, Func<TSource, bool> predicate)
            where TSource : unmanaged
        {
            if (!array.Any()) return false;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (!predicate(*ptr))
                    return false;
            return true;
        }
        #endregion

        #region Aggregate
        public static void Aggregate<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, ref TSource seed, TFunc func)
            where TFunc : struct, IRefAction<TSource, TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                func.Execute(ref seed, ref enumerator.Current);
            enumerator.Dispose();
        }
        public static void Aggregate<TEnumerable, TEnumerator, TSource, TAccumulate, TFunc>(ref this TEnumerable @this, ref TAccumulate seed, TFunc func)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                func.Execute(ref seed, ref enumerator.Current);
            enumerator.Dispose();
        }
        public static TResult Aggregate<TEnumerable, TEnumerator, TSource, TAccumulate, TResult, TFunc, TResultFunc>(ref this TEnumerable @this, ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                func.Execute(ref seed, ref enumerator.Current);
            enumerator.Dispose();
            return resultFunc.Calc(ref seed);
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult, TFunc, TResultFunc>(this NativeArray<TSource> array, TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
            return resultFunc.Calc(ref seed);
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult, TFunc, TResultFunc>(this NativeArray<TSource> array, ref TAccumulate seed, TFunc func, TResultFunc resultFunc)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TResultFunc : struct, IRefFunc<TAccumulate, TResult>
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
            return resultFunc.Calc(ref seed);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate, TFunc>(this NativeArray<TSource> array, TAccumulate seed, TFunc func)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
            return seed;
        }

        public static void Aggregate<TSource, TAccumulate, TFunc>(this NativeArray<TSource> array, ref TAccumulate seed, TFunc func)
            where TFunc : struct, IRefAction<TAccumulate, TSource>
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
        }

        public static void Aggregate<TSource, TFunc>(this NativeArray<TSource> array, ref TSource seed, TFunc func)
            where TFunc : struct, IRefAction<TSource, TSource>
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                func.Execute(ref seed, ref *ptr);
        }

        public static TSource Aggregate<TSource, TFunc>(this NativeArray<TSource> array, TSource seed, TFunc func)
            where TFunc : struct, IRefAction<TSource, TSource>
            where TSource : unmanaged
        {
            Aggregate<TSource, TFunc>(array, ref seed, func);
            return seed;
        }

        public static TResult Aggregate<TSource, TAccumulate, TResult>(this NativeArray<TSource> array, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultFunc)
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                seed = func(seed, *ptr);
            return resultFunc(seed);
        }

        public static TAccumulate Aggregate<TSource, TAccumulate>(this NativeArray<TSource> array, TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }

        public static TSource Aggregate<TSource>(this NativeArray<TSource> array, TSource seed, Func<TSource, TSource, TSource> func)
            where TSource : unmanaged
        {
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                seed = func(seed, *ptr);
            return seed;
        }
        #endregion

        #region Cast
        public static NativeArray<TResult> Cast<TSource, TResult, TCast>(this NativeArray<TSource> array, Allocator allocator, TCast cast)
            where TSource : unmanaged
            where TResult : unmanaged
            where TCast : struct, IRefAction<TSource, TResult>
        {
            var src = array.GetPointer();
            var answer = new NativeArray<TResult>(array.Length, allocator, NativeArrayOptions.UninitializedMemory);
            var dest = answer.GetPointer();
            for (int i = 0; i < array.Length; i++, src++, dest++)
                cast.Execute(ref *src, ref *dest);
            return answer;
        }
        #endregion

        #region Contains
        public static bool Contains<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, TSource value)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (value.Equals(enumerator.Current))
                {
                    enumerator.Dispose();
                    return true;
                }
            }
            enumerator.Dispose();
            return false;
        }
        public static bool Contains<TEnumerable, TEnumerator, TSource, TEqualityComparer>(ref this TEnumerable @this, TSource value, TEqualityComparer comparer)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        {
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (comparer.Calc(ref value, ref enumerator.Current))
                {
                    enumerator.Dispose();
                    return true;
                }
            }
            enumerator.Dispose();
            return false;
        }
        public static bool Contains<TSource>(this NativeArray<TSource> array, in TSource value)
            where TSource : unmanaged
        {
            var ptr = UnsafeUtilityEx.GetPointer(array);
            for (int i = 0; i < array.Length; i++, ptr++)
                if (ptr->Equals(value))
                    return true;
            return false;
        }
        public static bool Contains<TSource, TEqualityComparer>(this NativeArray<TSource> array, TSource value, TEqualityComparer comparer)
            where TSource : unmanaged
            where TEqualityComparer : struct, IRefFunc<TSource, TSource, bool>
        {
            var ptr = UnsafeUtilityEx.GetPointer(array);
            for (int i = 0; i < array.Length; i++, ptr++)
                if (comparer.Calc(ref *ptr, ref value))
                    return true;
            return false;
        }
        public static bool Contains<TSource>(this NativeArray<TSource> array, TSource value, System.Collections.Generic.IEqualityComparer<TSource> comparer)
            where TSource : unmanaged
        {
            var ptr = UnsafeUtilityEx.GetPointer(array);
            for (int i = 0; i < array.Length; i++, ptr++)
                if (comparer.Equals(*ptr, value))
                    return true;
            return false;
        }
        #endregion

        #region Count
        public static int Count<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        public static int Count<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, Func<TSource, bool> predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
                if (predicate(enumerator.Current))
                    ++count;
            enumerator.Dispose();
            return count;
        }

        public static int Count<TEnumerable, TEnumerator, TSource, TPredicate>(ref this TEnumerable @this, TPredicate predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            var enumerator = @this.GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
                if (predicate.Calc(ref enumerator.Current))
                    ++count;
            enumerator.Dispose();
            return count;
        }

        public static int Count<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => array.Length;
        public static int Count<TSource, TPredicate>(this NativeArray<TSource> array, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            var count = 0;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (predicate.Calc(ref *ptr))
                    ++count;
            return count;
        }
        public static int Count<TSource>(this NativeArray<TSource> array, Func<TSource, bool> predicate)
            where TSource : unmanaged
        {
            var count = 0;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                if (predicate(*ptr))
                    ++count;
            return count;
        }
        #endregion

        #region Average
        public static Int32 AverageInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int32>
            where TEnumerator : struct, IRefEnumerator<Int32>
        {
            Int32 sum = default;
            var count = 0;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static Int32 AverageInt32<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, Int32>
        {
            Int32 sum = default;
            var count = 0;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static Int64 AverageInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int64>
            where TEnumerator : struct, IRefEnumerator<Int64>
        {
            Int64 sum = default;
            var count = 0;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static Int64 AverageInt64<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, Int64>
        {
            Int64 sum = default;
            var count = 0;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static UInt32 AverageUInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt32>
            where TEnumerator : struct, IRefEnumerator<UInt32>
        {
            UInt32 sum = default;
            var count = 0u;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static UInt32 AverageUInt32<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, UInt32>
        {
            UInt32 sum = default;
            var count = 0u;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }
        public static UInt64 AverageUInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt64>
            where TEnumerator : struct, IRefEnumerator<UInt64>
        {
            UInt64 sum = default;
            var count = 0UL;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static UInt64 AverageUInt64<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, UInt64>
        {
            UInt64 sum = default;
            var count = 0UL;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static Single AverageSingle<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Single>
            where TEnumerator : struct, IRefEnumerator<Single>
        {
            Single sum = default;
            var count = 0;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static Single AverageSingle<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, Single>
        {
            Single sum = default;
            var count = 0;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static Double AverageDouble<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Double>
            where TEnumerator : struct, IRefEnumerator<Double>
        {
            Double sum = default;
            var count = 0;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static Double AverageDouble<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, Double>
        {
            Double sum = default;
            var count = 0;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static Decimal AverageDecimal<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Decimal>
            where TEnumerator : struct, IRefEnumerator<Decimal>
        {
            Decimal sum = default;
            var count = 0;
            foreach (var item in @this)
            {
                sum += item;
                ++count;
            }
            return sum / count;
        }
        public static Decimal AverageDecimal<TEnumerable, TEnumerator, TSource, TFunc>(ref this TEnumerable @this, TFunc func)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TFunc : struct, IRefFunc<TSource, Decimal>
        {
            Decimal sum = default;
            var count = 0;
            foreach (ref var item in @this)
            {
                sum += func.Calc(ref item);
                ++count;
            }
            return sum / count;
        }

        public static float Average(this NativeArray<float> array)
        {
            if (!array.Any()) return 0;
            float sum = default;
            foreach (var item in array)
                sum += item;
            return sum / array.Length;
        }

        public static double Average(this NativeArray<double> array)
        {
            if (!array.Any()) return 0;
            double sum = default;
            foreach (var item in array)
                sum += item;
            return sum / array.Length;
        }

        public static decimal Average(this NativeArray<decimal> array)
        {
            if (!array.Any()) return 0;
            decimal sum = default;
            foreach (var item in array)
                sum += item;
            return sum / array.Length;
        }

        public static long Average(this NativeArray<long> array)
        {
            if (!array.Any()) return 0;
            long sum = default;
            foreach (var item in array)
                sum += item;
            return sum / array.Length;
        }

        public static int Average(this NativeArray<int> array)
        {
            if (!array.Any()) return 0;
            int sum = default;
            foreach (var item in array)
                sum += item;
            return sum / array.Length;
        }

        public static uint Average(this NativeArray<uint> array)
        {
            uint sum = default;
            foreach (var item in array)
                sum += item;
            return sum / (uint)array.Length;
        }

        public static ulong Average(this NativeArray<ulong> array)
        {
            ulong sum = default;
            foreach (var item in array)
                sum += item;
            return sum / (ulong)array.Length;
        }

        public static float AverageSingle<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, float>
        {
            float sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / array.Length;
        }

        public static double AverageDouble<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, double>
        {
            double sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / array.Length;
        }

        public static decimal AverageDecimal<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, decimal>
        {
            decimal sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / array.Length;
        }

        public static int AverageInt32<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, int>
        {
            int sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / array.Length;
        }

        public static uint AverageUInt32<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, uint>
        {
            uint sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / (uint)array.Length;
        }

        public static long AverageInt64<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, long>
        {
            long sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / array.Length;
        }

        public static ulong AverageUInt64<T, TFunc>(this NativeArray<T> array, TFunc func)
            where T : unmanaged
            where TFunc : struct, IRefFunc<T, ulong>
        {
            ulong sum = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
                sum += func.Calc(ref *ptr);
            return sum / (ulong)array.Length;
        }

        public static float Average<T>(this NativeArray<T> array, Func<T, Single> func)
            where T : unmanaged
        {
            float sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / array.Length;
        }

        public static double Average<T>(this NativeArray<T> array, Func<T, Double> func)
            where T : unmanaged
        {
            double sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / array.Length;
        }

        public static decimal Average<T>(this NativeArray<T> array, Func<T, Decimal> func)
            where T : unmanaged
        {
            decimal sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / array.Length;
        }

        public static int Average<T>(this NativeArray<T> array, Func<T, Int32> func)
            where T : unmanaged
        {
            int sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / array.Length;
        }

        public static uint Average<T>(this NativeArray<T> array, Func<T, UInt32> func)
            where T : unmanaged
        {
            uint sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / (uint)array.Length;
        }

        public static long Average<T>(this NativeArray<T> array, Func<T, Int64> func)
            where T : unmanaged
        {
            long sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / array.Length;
        }

        public static ulong Average<T>(this NativeArray<T> array, Func<T, UInt64> func)
            where T : unmanaged
        {
            ulong sum = default;
            for (int i = 0; i < array.Length; i++)
                sum += func(array[i]);
            return sum / (ulong)array.Length;
        }
        #endregion

        public static bool TryGetElementAt<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, int index, out TSource value)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            if (index < 0)
            {
                value = default;
                return false;
            }
            var enumerator = @this.GetEnumerator();
            for (int i = 0; i < index; i++)
            {
                if (enumerator.MoveNext()) continue;
                value = default;
                enumerator.Dispose();
                return false;
            }
            value = enumerator.Current;
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetElementAt<TSource>(this NativeArray<TSource> array, int index, out TSource value)
            where TSource : unmanaged
        {
            if (!array.Any() || index < 0 || index >= array.Length)
            {
                value = default;
                return false;
            }
            value = array[index];
            return true;
        }

        public static ref TSource ElementAt<TSource>(this NativeArray<TSource> array, int index)
            where TSource : unmanaged
        {
            if (!array.Any() || index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException();
            return ref array.GetPointer()[index];
        }

        public static NativeArray<T> Empty<T>() where T : unmanaged => default;

        #region  First & Last
        public static bool TryGetFirst<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, out TSource first)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            if (enumerator.MoveNext())
            {
                first = enumerator.Current;
                enumerator.Dispose();
                return true;
            }
            else
            {
                first = default;
                enumerator.Dispose();
                return false;
            }
        }

        public static ref TSource First<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => ref array.ElementAt(0);
        public static ref TSource Last<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => ref array.ElementAt(array.Length - 1);

        public static TSource FirstOrDefault<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => array.IsCreated && array.Length != 0 ? array[0] : default;
        public static TSource LastOrDefault<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => array.IsCreated && array.Length != 0 ? array[array.Length - 1] : default;

        public static bool TryGetLast<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, out TSource last)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                last = default;
                enumerator.Dispose();
                return false;
            }
            last = enumerator.Current;
            while (enumerator.MoveNext()) ;
            enumerator.Dispose();
            return true;
        }
        #endregion

        #region LongCount
        public static long LongCount<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                ++count;
            enumerator.Dispose();
            return count;
        }

        public static long LongCount<TEnumerable, TEnumerator, TSource, TPredicate>(ref this TEnumerable @this, TPredicate predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            var enumerator = @this.GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                if (predicate.Calc(ref enumerator.Current))
                    ++count;
            enumerator.Dispose();
            return count;
        }

        public static long LongCount<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, Func<TSource, bool> predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var count = 0L;
            while (enumerator.MoveNext())
                if (predicate(enumerator.Current))
                    ++count;
            enumerator.Dispose();
            return count;
        }

        public static long LongCount<TSource>(this NativeArray<TSource> array) where TSource : unmanaged => (long)array.Length;
        public static long LongCount<TSource>(this NativeArray<TSource> array, Func<TSource, bool> predicate) where TSource : unmanaged => (long)Count(array, predicate);
        public static long LongCount<TSource, TPredicate>(this NativeArray<TSource> array, TPredicate predicate) where TSource : unmanaged where TPredicate : struct, IRefFunc<TSource, bool> => (long)Count(array, predicate);
        #endregion

        #region MinMax
        public static bool TryGetMinMaxInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this, out Int32 min, out Int32 max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int32>
            where TEnumerator : struct, IRefEnumerator<Int32>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }
        public static bool TryGetMinMaxInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this, out Int64 min, out Int64 max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int64>
            where TEnumerator : struct, IRefEnumerator<Int64>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMaxUInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this, out UInt32 min, out UInt32 max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt32>
            where TEnumerator : struct, IRefEnumerator<UInt32>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMaxUInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this, out UInt64 min, out UInt64 max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt64>
            where TEnumerator : struct, IRefEnumerator<UInt64>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMaxSingle<TEnumerable, TEnumerator>(ref this TEnumerable @this, out Single min, out Single max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Single>
            where TEnumerator : struct, IRefEnumerator<Single>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMaxDouble<TEnumerable, TEnumerator>(ref this TEnumerable @this, out Double min, out Double max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Double>
            where TEnumerator : struct, IRefEnumerator<Double>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMaxDecimal<TEnumerable, TEnumerator>(ref this TEnumerable @this, out Decimal min, out Decimal max)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Decimal>
            where TEnumerator : struct, IRefEnumerator<Decimal>
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
            {
                enumerator.Dispose();
                min = max = default;
                return false;
            }
            min = max = enumerator.Current;
            while (enumerator.MoveNext())
            {
                var current = enumerator.Current;
                if (max < current)
                    max = current;
                else if (min > current)
                    min = current;
            }
            enumerator.Dispose();
            return true;
        }

        public static bool TryGetMinMax(this NativeArray<Int32> @this, out Int32 min, out Int32 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }

        public static bool TryGetMinMax(this NativeArray<UInt32> @this, out UInt32 min, out UInt32 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Int64> @this, out Int64 min, out Int64 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<UInt64> @this, out UInt64 min, out UInt64 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Single> @this, out Single min, out Single max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Double> @this, out Double min, out Double max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Decimal> @this, out Decimal min, out Decimal max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Byte> @this, out Byte min, out Byte max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<SByte> @this, out SByte min, out SByte max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<Int16> @this, out Int16 min, out Int16 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static bool TryGetMinMax(this NativeArray<UInt16> @this, out UInt16 min, out UInt16 max)
        {
            if (!@this.IsCreated || @this.Length == 0)
            {
                min = max = default;
                return false;
            }
            min = max = @this[0];
            for (int i = 1; i < @this.Length; i++)
            {
                if (min > @this[i])
                    min = @this[i];
                else if (max < @this[i])
                    max = @this[i];
            }
            return true;
        }
        public static UInt16 Min(this NativeArray<UInt16> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt16 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static UInt16 Max(this NativeArray<UInt16> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt16 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static UInt32 Min(this NativeArray<UInt32> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt32 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static UInt32 Max(this NativeArray<UInt32> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt32 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static UInt64 Min(this NativeArray<UInt64> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt64 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static UInt64 Max(this NativeArray<UInt64> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            UInt64 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Int16 Min(this NativeArray<Int16> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int16 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Int16 Max(this NativeArray<Int16> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int16 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Int32 Min(this NativeArray<Int32> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int32 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Int32 Max(this NativeArray<Int32> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int32 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Int64 Min(this NativeArray<Int64> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int64 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Int64 Max(this NativeArray<Int64> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Int64 value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Single Min(this NativeArray<Single> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Single value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Single Max(this NativeArray<Single> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Single value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Double Min(this NativeArray<Double> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Double value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Double Max(this NativeArray<Double> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Double value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Decimal Min(this NativeArray<Decimal> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Decimal value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Decimal Max(this NativeArray<Decimal> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Decimal value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static Byte Min(this NativeArray<Byte> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Byte value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static Byte Max(this NativeArray<Byte> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            Byte value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }
        public static SByte Min(this NativeArray<SByte> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            SByte value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value > @this[i])
                    value = @this[i];
            return value;
        }
        public static SByte Max(this NativeArray<SByte> @this)
        {
            if (!@this.IsCreated || @this.Length == 0)
                throw new InvalidOperationException();
            SByte value = @this[0];
            for (int i = 1; i < @this.Length; i++)
                if (value < @this[i])
                    value = @this[i];
            return value;
        }

        #endregion

        #region Range
        public static NativeArray<Int32> Range(Int32 start, int count, Allocator allocator)
        {
            var answer = new NativeArray<Int32>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (int i = 0; i < count; i++)
                answer[i] = start + i;
            return answer;
        }
        public static NativeArray<Int64> Range(Int64 start, int count, Allocator allocator)
        {
            var answer = new NativeArray<Int64>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + i;
            return answer;
        }
        public static NativeArray<Single> Range(Single start, int count, Allocator allocator)
        {
            var answer = new NativeArray<Single>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + i;
            return answer;
        }
        public static NativeArray<Double> Range(Double start, int count, Allocator allocator)
        {
            var answer = new NativeArray<Double>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + i;
            return answer;
        }
        public static NativeArray<UInt32> Range(UInt32 start, int count, Allocator allocator)
        {
            var answer = new NativeArray<UInt32>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + (uint)i;
            return answer;
        }
        public static NativeArray<UInt64> Range(UInt64 start, int count, Allocator allocator)
        {
            var answer = new NativeArray<UInt64>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + (ulong)i;
            return answer;
        }
        public static NativeArray<Decimal> Range(Decimal start, int count, Allocator allocator)
        {
            var answer = new NativeArray<Decimal>(count, allocator, NativeArrayOptions.UninitializedMemory);
            for (var i = 0; i < count; i++)
                answer[i] = start + (decimal)i;
            return answer;
        }
        #endregion

        public static NativeArray<TResult> Repeat<TResult>(TResult value, int count, Allocator allocator) where TResult : unmanaged
        {
            var answer = new NativeArray<TResult>(count, allocator, NativeArrayOptions.UninitializedMemory);
            UnsafeUtility.MemCpyReplicate(answer.GetPointer(), &value, sizeof(TResult), count);
            return answer;
        }

        #region Single
        public static bool TryGetSingle<TEnumerable, TEnumerator, TSource, TPredicate>(ref this TEnumerable @this, out TSource value, TPredicate predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            value = default;
            var enumerator = @this.GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
            {
                if (predicate.Calc(ref enumerator.Current))
                {
                    value = enumerator.Current;
                    if (++count > 1)
                    {
                        enumerator.Dispose();
                        return false;
                    }
                }
            }
            enumerator.Dispose();
            return count == 1;
        }
        public static bool TryGetSingle<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, out TSource value, Func<TSource, bool> predicate)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            value = default;
            var enumerator = @this.GetEnumerator();
            var count = 0;
            while (enumerator.MoveNext())
            {
                if (predicate(enumerator.Current))
                {
                    value = enumerator.Current;
                    if (++count > 1)
                    {
                        enumerator.Dispose();
                        return false;
                    }
                }
            }
            enumerator.Dispose();
            return count == 1;
        }
        public static bool TryGetSingle<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, out TSource value)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            if (!enumerator.MoveNext())
                goto ERROR;
            value = enumerator.Current;
            if (enumerator.MoveNext())
                goto ERROR;
            enumerator.Dispose();
            return true;
        ERROR:
            enumerator.Dispose();
            value = default;
            return false;
        }
        public static bool TryGetSingle<TSource>(this NativeArray<TSource> array, out TSource value) where TSource : unmanaged
        {
            if (array.IsCreated && array.Length == 1)
            {
                value = array[0];
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
        public static bool TryGetSingle<TSource, TPredicate>(this NativeArray<TSource> array, out TSource value, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            value = default;
            if (!array.IsCreated || array.Length == 0)
                return false;
            var count = 0;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
            {
                if (predicate.Calc(ref *ptr))
                {
                    if (++count > 1) return false;
                    value = *ptr;
                }
            }
            if (count == 0) return false;
            return true;
        }
        public static bool TryGetSingle<TSource>(this NativeArray<TSource> array, out TSource value, Func<TSource, bool> predicate)
            where TSource : unmanaged
        {
            value = default;
            if (!array.IsCreated || array.Length == 0)
                return false;
            var count = 0;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
            {
                if (predicate(*ptr))
                {
                    if (++count > 1) return false;
                    value = *ptr;
                }
            }
            if (count == 0) return false;
            return true;
        }
        public static ref TSource Single<TSource>(this NativeArray<TSource> array) where TSource : unmanaged
        {
            if (array.IsCreated && array.Length == 1)
                return ref *array.GetPointer();
            throw new InvalidOperationException();
        }
        public static ref TSource Single<TSource, TPredicate>(this NativeArray<TSource> array, TPredicate predicate)
            where TSource : unmanaged
            where TPredicate : struct, IRefFunc<TSource, bool>
        {
            if (!array.IsCreated || array.Length == 0) throw new InvalidOperationException();
            var count = 0;
            TSource* answer = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
            {
                if (predicate.Calc(ref *ptr))
                {
                    if (++count > 1) throw new InvalidOperationException();
                    answer = ptr;
                }
            }
            if (count == 0) throw new InvalidOperationException();
            return ref *answer;
        }
        public static ref TSource Single<TSource>(this NativeArray<TSource> array, Func<TSource, bool> predicate)
            where TSource : unmanaged
        {
            if (!array.IsCreated || array.Length == 0) throw new InvalidOperationException();
            var count = 0;
            TSource* answer = default;
            var ptr = array.GetPointer();
            for (int i = 0; i < array.Length; i++, ptr++)
            {
                if (predicate(*ptr))
                {
                    if (++count > 1) throw new InvalidOperationException();
                    answer = ptr;
                }
            }
            if (count == 0) throw new InvalidOperationException();
            return ref *answer;
        }
        #endregion

        #region Sum
        public static Int32 SumInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int32>
            where TEnumerator : struct, IRefEnumerator<Int32>
        {
            Int32 sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static Int64 SumInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Int64>
            where TEnumerator : struct, IRefEnumerator<Int64>
        {
            Int64 sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static UInt32 SumUInt32<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt32>
            where TEnumerator : struct, IRefEnumerator<UInt32>
        {
            UInt32 sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static UInt64 SumUInt64<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, UInt64>
            where TEnumerator : struct, IRefEnumerator<UInt64>
        {
            UInt64 sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static Single SumSingle<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Single>
            where TEnumerator : struct, IRefEnumerator<Single>
        {
            Single sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static Double SumDouble<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Double>
            where TEnumerator : struct, IRefEnumerator<Double>
        {
            Double sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }
        public static Decimal SumDecimal<TEnumerable, TEnumerator>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, Decimal>
            where TEnumerator : struct, IRefEnumerator<Decimal>
        {
            Decimal sum = default;
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                sum += enumerator.Current;
            enumerator.Dispose();
            return sum;
        }

        public static Int32 Sum(this NativeArray<Int32> array)
        {
            Int32 sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static UInt32 Sum(this NativeArray<UInt32> array)
        {
            UInt32 sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static Int64 Sum(this NativeArray<Int64> array)
        {
            Int64 sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static UInt64 Sum(this NativeArray<UInt64> array)
        {
            UInt64 sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static Single Sum(this NativeArray<Single> array)
        {
            Single sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static Double Sum(this NativeArray<Double> array)
        {
            Double sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        public static Decimal Sum(this NativeArray<Decimal> array)
        {
            Decimal sum = 0;
            for (int i = 0; i < array.Length; i++)
                sum += array[i];
            return sum;
        }
        #endregion

        #region ToArray
        public static TSource[] ToArray<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var ptr = CountUp<TEnumerator, TSource>(ref enumerator, out var count);
            if (count == 0)
                return Array.Empty<TSource>();
            var answer = new TSource[count];
            fixed (TSource* destination = &answer[0])
            {
                CopyToAndCleanUp(ptr, count, destination);
            }
            return answer;
        }
        public static NativeArray<TSource> ToNativeArray<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, Allocator allocator)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var enumerator = @this.GetEnumerator();
            var ptr = CountUp<TEnumerator, TSource>(ref enumerator, out var count);
            if (count == 0)
                return default;
            var answer = new NativeArray<TSource>(count, allocator, NativeArrayOptions.UninitializedMemory);
            CopyToAndCleanUp(ptr, count, answer.GetPointer());
            return answer;
        }
        private static TSource* CountUp<TEnumerator, TSource>(ref TEnumerator enumerator, out int count)
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            if (!enumerator.MoveNext())
            {
                count = 0;
                enumerator.Dispose();
                return null;
            }
            var capacity = 16;
            var ptr = UnsafeUtilityEx.Malloc<TSource>(capacity, Allocator.Temp);
            *ptr = enumerator.Current;
            count = 1;
            while (enumerator.MoveNext())
            {
                ptr[count] = enumerator.Current;
                if (++count == capacity)
                {
                    ptr = Lengthen(ptr, capacity, Allocator.Temp);
                    capacity <<= 1;
                }
            }
            enumerator.Dispose();
            return ptr;
        }
        private static void CopyToAndCleanUp<TSource>(TSource* ptr, int count, TSource* destination) where TSource : unmanaged
        {
            UnsafeUtility.MemCpy(destination, ptr, sizeof(TSource) * count);
            UnsafeUtility.Free(ptr, Allocator.Temp);
        }
        private static T* Lengthen<T>(T* source, int capacity, Allocator allocator) where T : unmanaged
        {
            var answer = UnsafeUtilityEx.Malloc<T>(capacity << 1, allocator);
            UnsafeUtility.MemCpy(answer, source, sizeof(T) * capacity);
            UnsafeUtility.Free(source, allocator);
            return answer;
        }
        #endregion

        #region ToDictionary
        public static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TEnumerable, TEnumerator, TSource, TKey, TElement, TKeyFunc, TValueFunc>(ref this TEnumerable @this, TKeyFunc keySelector, TValueFunc elementSelector)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
            where TKeyFunc : struct, IRefFunc<TSource, TKey>
            where TValueFunc : struct, IRefFunc<TSource, TElement>
        {
            var answer = new System.Collections.Generic.Dictionary<TKey, TElement>();
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;
                answer.Add(keySelector.Calc(ref current), elementSelector.Calc(ref current));
            }
            enumerator.Dispose();
            return answer;
        }
        public static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TEnumerable, TEnumerator, TSource, TKey, TElement>(ref this TEnumerable @this, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.Dictionary<TKey, TElement>();
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
            {
                ref var current = ref enumerator.Current;
                answer.Add(keySelector(current), elementSelector(current));
            }
            enumerator.Dispose();
            return answer;
        }

        public static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement, TKeyFunc, TValueFunc>(this NativeArray<TSource> @this, TKeyFunc keySelector, TValueFunc elementSelector)
            where TSource : unmanaged
            where TKeyFunc : struct, IRefFunc<TSource, TKey>
            where TValueFunc : struct, IRefFunc<TSource, TElement>
        {
            var answer = new System.Collections.Generic.Dictionary<TKey, TElement>();
            var ptr = @this.GetPointer();
            for (int i = 0; i < @this.Length; i++, ptr++)
                answer.Add(keySelector.Calc(ref *ptr), elementSelector.Calc(ref *ptr));
            return answer;
        }

        public static System.Collections.Generic.Dictionary<TKey, TElement> ToDictionary<TSource, TKey, TElement>(this NativeArray<TSource> array, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.Dictionary<TKey, TElement>();
            for (int i = 0; i < array.Length; i++)
                answer.Add(keySelector(array[i]), elementSelector(array[i]));
            return answer;
        }
        #endregion

        #region ToHashSet
        public static System.Collections.Generic.HashSet<TSource> ToHashSet<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this, System.Collections.Generic.IEqualityComparer<TSource> comparer)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.HashSet<TSource>(comparer);
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }

        public static System.Collections.Generic.HashSet<TSource> ToHashSet<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.HashSet<TSource>();
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }

        public static System.Collections.Generic.HashSet<TSource> ToHashSet<TSource>(this NativeArray<TSource> @this, System.Collections.Generic.IEqualityComparer<TSource> comparer)
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.HashSet<TSource>(comparer);
            for (int i = 0; i < @this.Length; i++)
                answer.Add(@this[i]);
            return answer;
        }

        public static System.Collections.Generic.HashSet<TSource> ToHashSet<TSource>(this NativeArray<TSource> @this)
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.HashSet<TSource>();
            for (int i = 0; i < @this.Length; i++)
                answer.Add(@this[i]);
            return answer;
        }
        #endregion

        #region ToList
        public static System.Collections.Generic.List<TSource> ToList<TEnumerable, TEnumerator, TSource>(ref this TEnumerable @this)
            where TEnumerable : struct, IRefEnumerable<TEnumerator, TSource>
            where TEnumerator : struct, IRefEnumerator<TSource>
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.List<TSource>();
            var enumerator = @this.GetEnumerator();
            while (enumerator.MoveNext())
                answer.Add(enumerator.Current);
            enumerator.Dispose();
            return answer;
        }
        public static System.Collections.Generic.List<TSource> ToList<TSource>(this NativeArray<TSource> @this)
            where TSource : unmanaged
        {
            var answer = new System.Collections.Generic.List<TSource>(@this.Length);
            for (int i = 0; i < @this.Length; i++)
                answer.Add(@this[i]);
            return answer;
        }
        #endregion
    }
}
