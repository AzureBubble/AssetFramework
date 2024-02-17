using Sirenix.OdinInspector;
using UnityEngine;

/// <summary>
/// AssetBundle Packaging tool DescriptionWindow
/// </summary>
public class AssetBundleBuildDescriptionWindow
{
    [OnInspectorGUI]
    public void OnGUI()
    {
        var style = new GUIStyle { alignment = TextAnchor.MiddleCenter };
        style.normal.textColor = Color.white;
        style.fontStyle = FontStyle.Bold;
        GUILayout.Label("AssetBundle Packaging tool", style);
    }
}