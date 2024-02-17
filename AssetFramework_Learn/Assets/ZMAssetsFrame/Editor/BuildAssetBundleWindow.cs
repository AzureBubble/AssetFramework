using GameFramework.AssetBundleFramework;
using UnityEditor;
using UnityEngine;

public class BuildAssetBundleWindow : AssetBundleBehaviour
{
    protected string[] buildButtonNameArr = new[] { "打包资源", "内嵌资源" };

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void DrawAddModuleBundleButton()
    {
        base.DrawAddModuleBundleButton();

        GUIContent addContent = EditorGUIUtility.IconContent("CollabCreate Icon".Trim(), "");
        if (GUILayout.Button(addContent, GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
            AssetBundleModuleConfigWindow.ShowModuleConfigWindow("");
        }
    }

    public override void DrawBuildButtons()
    {
        base.DrawBuildButtons();

        GUILayout.BeginArea(new Rect(0, 555, 800, 600));
        GUILayout.BeginHorizontal();
        {
            for (int i = 0; i < buildButtonNameArr.Length; i++)
            {
                GUIStyle style = UnityEditorUtility.GetGUIStyle("PreButtonBlue");
                style.fixedHeight = 55f;
                if (GUILayout.Button(buildButtonNameArr[i], style, GUILayout.Height(400)))
                {
                    switch (i)
                    {
                        case 0:
                            // 打包 AB 包按钮事件
                            // Package AB button event
                            BuildBundle();
                            break;

                        case 1:
                            CopyBundleToStreamingAssetsPath();
                            break;
                    }
                }
            }

            // 打包图标绘制
            // Package icon drawing
            GUI.DrawTexture(new Rect(130, 13, 30, 30), EditorGUIUtility.IconContent(currentPlatform).image);
            // 内嵌图标绘制
            // Inline icon drawing
            GUI.DrawTexture(new Rect(545, 13, 30, 30), EditorGUIUtility.IconContent("SceneSet Icon").image);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    public override void BuildBundle()
    {
        base.BuildBundle();

        if (moduleDataList.Count == 0 || moduleDataList == null) return;

        foreach (var moduleData in moduleDataList)
        {
            if (moduleData.isBuild)
            {
                AssetBundleBuildCompiler.BuildAssetBundle(moduleData);
            }
        }
    }

    /// <summary>
    /// 内嵌资源到 StreamingAssets 路径
    /// Embed resources into the StreamingAssets path
    /// </summary>
    public void CopyBundleToStreamingAssetsPath()
    {
        if (moduleDataList.Count == 0 || moduleDataList == null) return;

        foreach (var item in moduleDataList)
        {
            if (item.isBuild)
            {
                AssetBundleBuildCompiler.CopyAssetBundleToStreamingAssets(item);
            }
        }
    }
}