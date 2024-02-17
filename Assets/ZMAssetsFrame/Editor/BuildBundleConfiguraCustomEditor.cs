using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildBundleConfigura))]
public class BuildBundleConfiguraCustomEditor : OdinEditor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Open In Editor"))
        {
            AssetBundleBuildWindow.ShowAssetBundleWindow();
        }

        base.OnInspectorGUI();
    }
}