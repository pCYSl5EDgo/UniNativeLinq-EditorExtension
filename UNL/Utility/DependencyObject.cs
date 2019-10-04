using UnityEngine;

namespace UniNativeLinq.Editor
{
    public class DependencyObject : ScriptableObject, IDependency
    {
        [field: SerializeField] public string Enumerable { get; private set; }
        [field: SerializeField] public string[] Types { get; private set; }
        [field: SerializeField] public string[] Methods { get; private set; }
    }
}
