using UnityEngine;

namespace UniNativeLinq.Editor
{
    public interface IDrawableWithEnumerableAndScrollPosition
    {
        void RegisterEnumerableCollectionProcessor(IEnumerableCollectionProcessor enumerableCollectionProcessor);
        void Draw(ref Vector2 scrollPosition);
    }
}
