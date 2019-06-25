using UnityEngine;

namespace UniNativeLinq.Editor
{
    [CreateAssetMenu(menuName = "Linq/Dependency")]
    public sealed class EnumerableDependencyObject : ScriptableObject
    {
        public string Source => name;
        public string[] Destination;
    }
}
