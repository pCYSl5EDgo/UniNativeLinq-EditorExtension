using UnityEditor;

namespace UniNativeLinq.Editor
{
    public class SettingWindow : EditorWindow
    {
        private IEnumerableCollectionProcessor enumerableCollectionProcessor;
        private IWhetherToIncludeOrNotView whetherToIncludeOrNotView;
        private IWhetherToUseApiOrNotView whetherToUseApiOrNotView;

        [MenuItem("UniNativeLinq/Open Settings &#c")]
        static void Open()
        {
            var window = GetWindow<SettingWindow>(typeof(SceneView));
            Initialize(window);
        }

        private static void Initialize(SettingWindow window)
        {
            if (window.enumerableCollectionProcessor == null)
                window.enumerableCollectionProcessor = new EnumerableCollectionProcessor();
            if (window.whetherToIncludeOrNotView == null)
                window.whetherToIncludeOrNotView = new WhetherToIncludeOrNotViewImpl(window.enumerableCollectionProcessor);
            if (window.whetherToUseApiOrNotView == null)
                window.whetherToUseApiOrNotView = new WhetherToUseApiOrNotViewImpl();
        }

        void OnGUI()
        {
            if (whetherToIncludeOrNotView is null || enumerableCollectionProcessor is null)
                Initialize(this);
            whetherToIncludeOrNotView.Draw();
        }
    }
}