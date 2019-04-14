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

        public static Single Average(ref this NativeEnumerable<Single> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Single sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / @this.Length;
        }
        
        public static Double Average(ref this NativeEnumerable<Double> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Double sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / @this.Length;
        }
        
        public static Decimal Average(ref this NativeEnumerable<Decimal> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Decimal sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / @this.Length;
        }
        
        public static Int32 Average(ref this NativeEnumerable<Int32> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Int32 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / @this.Length;
        }
        
        public static Int64 Average(ref this NativeEnumerable<Int64> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Int64 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / @this.Length;
        }
        
        public static UInt64 Average(ref this NativeEnumerable<UInt64> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            UInt64 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / (ulong)@this.Length;
        }
        
        public static UInt32 Average(ref this NativeEnumerable<UInt32> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            UInt32 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum / (uint)@this.Length;
        }

        public static Single Sum(ref this NativeEnumerable<Single> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Single sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Double Sum(ref this NativeEnumerable<Double> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Double sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Decimal Sum(ref this NativeEnumerable<Decimal> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Decimal sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Int32 Sum(ref this NativeEnumerable<Int32> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Int32 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static Int64 Sum(ref this NativeEnumerable<Int64> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            Int64 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static UInt64 Sum(ref this NativeEnumerable<UInt64> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            UInt64 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
        
        public static UInt32 Sum(ref this NativeEnumerable<UInt32> @this)
        {
            var ptr = @this.Ptr;
            if (ptr == null || @this.Length == 0) return default;
            UInt32 sum = default;
            for (var i = 0; i < @this.Length; i++, ptr++)
                sum += *ptr;
            return sum;
        }
    }
}