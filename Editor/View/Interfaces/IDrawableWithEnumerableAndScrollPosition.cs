using UnityEngine;

namespace UniNativeLinq.Editor
{
    public interface IDrawableWithEnumerableAndScrollPosition
    {
        void Draw(IEnumerableCollectionProcessor processor, ref Vector2 scrollPosition);
    }
}
