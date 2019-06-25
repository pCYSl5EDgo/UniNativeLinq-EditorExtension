using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    static class ScrollHelper
    {
        private static readonly Delegate GetTopScrollView;

        public static ScrollViewStateWrapper GetCurrentScroll()
        {
            var obj = GetTopScrollView.DynamicInvoke(Array.Empty<object>());
            return obj is null ? null : Unsafe.As<ScrollViewStateWrapper>(obj);
        }

        static ScrollHelper()
        {
            GetTopScrollView = typeof(GUI).GetMethod(nameof(GetTopScrollView), BindingFlags.NonPublic | BindingFlags.Static).CreateDelegate(typeof(Func<>).MakeGenericType(typeof(GUI).Assembly.GetType("UnityEngine.ScrollViewState")));
        }
    }

    public sealed class ScrollViewStateWrapper
    {
        public Rect position;
        public Rect visibleRect;
        public Rect viewRect;
        public Vector2 scrollPosition;
        public bool apply;
    }
}