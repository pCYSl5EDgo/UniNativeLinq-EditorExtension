using System;
using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    internal sealed class DrawableImplWhetherToUseApiOrNot : IDrawable
    {
        public bool FoldoutTopLevel;
        public bool FoldoutSingleLevel;
        public bool FoldoutDoubleLevel;
        private readonly IEnumerableCollectionProcessor processor;
        private readonly ISingleApi[] singleApis;
        private readonly IDoubleApi[] doubleApis;
        public DrawableImplWhetherToUseApiOrNot(IEnumerableCollectionProcessor processor, ISingleApi[] singleApis, IDoubleApi[] doubleApis)
        {
            this.processor = processor;
            this.singleApis = singleApis ?? Array.Empty<ISingleApi>();
            Array.Sort(this.singleApis);
            this.doubleApis = doubleApis ?? Array.Empty<IDoubleApi>();
            Array.Sort(this.doubleApis);
        }

        public void Draw(ref Vector2 scrollPosition)
        {
            if (!FoldoutUtility.Draw(ref FoldoutTopLevel, "Whether to include each api or NOT")) return;
            if (FoldoutUtility.Draw(ref FoldoutSingleLevel, "Single"))
                DrawSingleApis();
            if (FoldoutUtility.Draw(ref FoldoutDoubleLevel, "Double"))
                DrawDoubleApis(ref scrollPosition);
        }

        private void DrawDoubleApis(ref Vector2 scrollPosition)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    for (var i = 0; i < doubleApis.Length; i++)
                    {
                        ref var api = ref doubleApis[i];
                        foreach (var name0 in api.NameCollection)
                            foreach (var name1 in api.NameCollection)
                                api.TrySetEnabled(name0, name1, true);
                    }
                }
                else if (GUILayout.Button("Deselect All"))
                {
                    for (var i = 0; i < doubleApis.Length; i++)
                    {
                        ref var api = ref doubleApis[i];
                        foreach (var name0 in api.NameCollection)
                            foreach (var name1 in api.NameCollection)
                                api.TrySetEnabled(name0, name1, false);
                    }
                }
                else if (GUILayout.Button("Show hidden apis"))
                {
                    foreach (var api in doubleApis)
                        api.IsHided = false;
                }
            }

            using (new IndentScope())
            {
                foreach (var api in doubleApis)
                {
                    api.Draw(processor, ref scrollPosition);
                }
            }
        }

        private void DrawSingleApis()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Select All"))
                {
                    for (var i = 0; i < singleApis.Length; i++)
                    {
                        ref var api = ref singleApis[i];
                        foreach (var name in api.NameCollection)
                        {
                            api.TrySetEnabled(name, true);
                        }
                    }
                }
                else if (GUILayout.Button("Deselect All"))
                {
                    for (var i = 0; i < singleApis.Length; i++)
                    {
                        ref var api = ref singleApis[i];
                        foreach (var name in api.NameCollection)
                        {
                            api.TrySetEnabled(name, false);
                        }
                    }
                }
                else if (GUILayout.Button("Show hidden apis"))
                {
                    foreach (var api in singleApis)
                        api.IsHided = false;
                }
            }

            using (new IndentScope())
            {
                foreach (var api in singleApis)
                {
                    api.Draw(processor);
                }
            }
        }
    }
}
