using System;
using System.Reflection;
using System.Collections.Generic;

namespace pcysl5edgo
{
#if !ENABLE_IL2CPP
    using System.Reflection.Emit;

    public struct GetArrayFromListCache<T>
        where T : unmanaged
    {
        public static readonly Func<List<T>, T[]> GetArray;

        static GetArrayFromListCache()
        {
            var method = new DynamicMethod("GetArray", MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T[]), new Type[] {typeof(List<T>)}, typeof(List<T>), true);
            var fis = typeof(List<T>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            FieldInfo arrayField = default;
            for (var i = 0; i < fis.Length; ++i)
            {
                if (!fis[i].FieldType.IsArray) continue;
                arrayField = fis[i];
                break;
            }
            if (arrayField is null) throw new InvalidOperationException();
            var ilGenerator = method.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0); // 第０引数について
            ilGenerator.Emit(OpCodes.Ldfld, arrayField); // その内部に抱えているprivateなT[]なフィールドを
            ilGenerator.Emit(OpCodes.Ret); // 戻り値とせよ
            GetArray = (Func<List<T>, T[]>) method.CreateDelegate(typeof(Func<List<T>, T[]>));
        }
    }
#elif SLOW_REFLECTION_LIST_ACCESS
    public struct GetArrayFromListCache<T>
        where T : unmanaged
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly FieldInfo ArrayField;

        static GetArrayFromListCache()
        {
            var fis = typeof(List<T>).GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < fis.Length; ++i)
            {
                if (!fis[i].FieldType.IsArray) continue;
                ArrayField = fis[i];
                break;
            }
            if (ArrayField is null) throw new InvalidOperationException();
        }

        public static T[] GetArray(List<T> list) => (T[])ArrayField.GetValue(list);
    }
#endif
}