using System;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal struct RotateLabel : IDisposable
    {
        private Matrix4x4 OriginalValue;

        public void Dispose()
        {
            GUI.matrix = OriginalValue;
        }
    }
}
