using GameFramework.AssetBundleFramework;
using Sirenix.OdinInspector.Editor.Modules;
using UnityEditor;
using UnityEngine;

public class BuildHotPatchWindow : AssetBundleBehaviour
{
    protected string[] buildButtonNameArr = new[] { "打包热更", "上传资源" }; // (Packaged Hotfix) (Upload Resource)
    [HideInInspector] public string hotPatchNotice = "Please enter the Notice of this Hotfix(请输入本次热更描述信息)";
    [HideInInspector] public string hotPatchVersion = "1"; // 热更补丁版本 Hot patch version

    public override void Initialize()
    {
        base.Initialize();
    }

    public override void OnGUI()
    {
        base.OnGUI();

        GUILayout.BeginArea(new Rect(0, 400, 800, 600));

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Please Enter This Hotfix Announcement(请输入本次热更公告)");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        hotPatchNotice = EditorGUILayout.TextField(hotPatchNotice, GUILayout.Width(800), GUILayout.Height(80));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal();
        hotPatchVersion = EditorGUILayout.TextField("HotPatch Version(热更资源版本)", hotPatchVersion, GUILayout.Width(800));
        GUILayout.EndHorizontal();

        GUILayout.EndArea();
    }

    public override void DrawAddModuleBundleButton()
    {
        base.DrawAddModuleBundleButton();

        GUIContent addContent = EditorGUIUtility.IconContent("CollabCreate Icon".Trim(), "");
        if (GUILayout.Button(addContent, GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
        {
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
            GUI.DrawTexture(new Rect(545, 13, 30, 30), EditorGUIUtility.IconContent("CollabPush").image);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    /// <summary>
    /// 打包 AB 包资源
    /// Package AB package resources
    /// </summary>
    public override void BuildBundle()
    {
        base.BuildBundle();

        if (moduleDataList.Count == 0 || moduleDataList == null) return;

        foreach (var moduleData in moduleDataList)
        {
            if (moduleData.isBuild)
            {
                AssetBundleBuildCompiler.BuildAssetBundle(moduleData, BuildType.HotPatch, hotPatchVersion, hotPatchNotice);
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
            }
        }
    }
}