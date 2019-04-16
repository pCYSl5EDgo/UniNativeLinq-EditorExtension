using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections
{
    public static unsafe class UnsafeUtilityEx
    {
        public static T* Malloc<T>(long count, Allocator allocator)
            where T : unmanaged => (T*) UnsafeUtility.Malloc(sizeof(T) * count, 4, allocator);

        public static void ReAlloc<T>(ref T* ptr, ref long count, Allocator allocator)
            where T : unmanaged
        {
            var tmp = Malloc<T>(count << 1, allocator);
            UnsafeUtility.MemCpy(tmp, ptr, count * sizeof(T));
            UnsafeUtility.Free(ptr, allocator);
            ptr = tmp;
            count <<= 1;
        }

        public static void ReAlloc<T>(ref T* ptr, long oldCount, long newCount, Allocator allocator)
            where T : unmanaged
        {
            var tmp = Malloc<T>(newCount, allocator);
            UnsafeUtility.MemCpy(tmp, ptr, oldCount * sizeof(T));
            UnsafeUtility.Free(ptr, allocator);
            ptr = tmp;
        }

        public static T* GetPointer<T>(this NativeArray<T> array)
            where T : unmanaged => (T*) NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);

        public static void MemMove<T>(T* dest, T* src, long count)
            where T : unmanaged
            => UnsafeUtility.MemMove(dest, src, count * sizeof(T));
        
        public static void MemCpy<T>(T* dest, T* src, long count)
            where T : unmanaged
            => UnsafeUtility.MemCpy(dest, src, count * sizeof(T));
    }
}