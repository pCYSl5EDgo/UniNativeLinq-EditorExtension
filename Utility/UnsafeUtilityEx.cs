using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace pcysl5edgo.Collections
{
    public unsafe static class UnsafeUtilityEx
    {
        public static T* Malloc<T>(int count, Allocator allocator) where T : unmanaged => (T*)UnsafeUtility.Malloc(sizeof(T) * count, 4, allocator);
        public static T* GetPointer<T>(this NativeArray<T> array) where T : unmanaged => (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);
    }
}