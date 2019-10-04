using UnityEngine;

namespace UniNativeLinq.Editor
{
    public interface IDrawable
    {
        void Draw(ref Vector2 scrollPosition);
    }
}
