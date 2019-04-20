using System;
using Unity.Collections;

namespace pcysl5edgo.Collections.LINQ
{
    public static unsafe class NativeEnumerableHelper
    {
        public static NativeEnumerable<T> AsRefEnumerable<T>(this NativeArray<T> array)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new NativeEnumerable<T>(array);

#if UNSAFE_ARRAY_ENUMERABLE
        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ArrayEnumerable<T>(array);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this T[] array, long offset, long count)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ArrayEnumerable<T>(array, offset, count);

        public static ArrayEnumerable<T> AsRefEnumerable<T>(this ArraySegment<T> arraySegment)
            where T : unmanaged
#if STRICT_EQUALITY
            , IEquatable<T>
#endif
            => new ArrayEnumerable<T>(arraySegment);
        
        public static Single Sum(ref this ArrayEnumerable<Single> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Single sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Double Sum(ref this ArrayEnumerable<Double> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Double sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Decimal Sum(ref this ArrayEnumerable<Decimal> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Decimal sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Int32 Sum(ref this ArrayEnumerable<Int32> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Int32 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Int64 Sum(ref this ArrayEnumerable<Int64> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Int64 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static UInt32 Sum(ref this ArrayEnumerable<UInt32> enumerable)
        {
            if (enumerable.Length == 0) return default;
            UInt32 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static UInt64 Sum(ref this ArrayEnumerable<UInt64> enumerable)
        {
            if (enumerable.Length == 0) return default;
            UInt64 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static Single Average(ref this ArrayEnumerable<Single> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Single sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }
        
        public static UInt64 Average(ref this ArrayEnumerable<UInt64> enumerable)
        {
            if (enumerable.Length == 0) return default;
            UInt64 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (ulong)enumerable.Length;
        }
        public static Double Average(ref this ArrayEnumerable<Double> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Double sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }
        
        public static Decimal Average(ref this ArrayEnumerable<Decimal> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Decimal sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }
        public static Int32 Average(ref this ArrayEnumerable<Int32> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Int32 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (int)enumerable.Length;
        }
        public static Int64 Average(ref this ArrayEnumerable<Int64> enumerable)
        {
            if (enumerable.Length == 0) return default;
            Int64 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }
        public static UInt32 Average(ref this ArrayEnumerable<UInt32> enumerable)
        {
            if (enumerable.Length == 0) return default;
            UInt32 sum = default;
            var ptr = enumerable.GetPointer();
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (uint)enumerable.Length;
        }
#endif

        public static Single Average(ref this NativeEnumerable<Single> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Single sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }

        public static Double Average(ref this NativeEnumerable<Double> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Double sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }

        public static Decimal Average(ref this NativeEnumerable<Decimal> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Decimal sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }

        public static Int32 Average(ref this NativeEnumerable<Int32> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Int32 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (int)enumerable.Length;
        }

        public static Int64 Average(ref this NativeEnumerable<Int64> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Int64 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / enumerable.Length;
        }

        public static UInt64 Average(ref this NativeEnumerable<UInt64> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            UInt64 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (ulong) enumerable.Length;
        }

        public static UInt32 Average(ref this NativeEnumerable<UInt32> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            UInt32 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum / (uint) enumerable.Length;
        }

        public static Single Sum(ref this NativeEnumerable<Single> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Single sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static Double Sum(ref this NativeEnumerable<Double> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Double sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static Decimal Sum(ref this NativeEnumerable<Decimal> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Decimal sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static Int32 Sum(ref this NativeEnumerable<Int32> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Int32 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static Int64 Sum(ref this NativeEnumerable<Int64> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            Int64 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static UInt64 Sum(ref this NativeEnumerable<UInt64> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            UInt64 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }

        public static UInt32 Sum(ref this NativeEnumerable<UInt32> enumerable)
        {
            var ptr = enumerable.Ptr;
            if (ptr == null || enumerable.Length == 0) return default;
            UInt32 sum = default;
            for (var i = 0; i < enumerable.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
    }
}