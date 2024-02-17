using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetBundleBehaviour
{
    // 模块资源按钮绘制的宽高设置
    // Width and height Settings for module resource button drawing
    protected readonly int btnWidth = 130;

    protected readonly int btnHeight = 170;

    /// <summary>
    /// 每行最多绘制多少个模块
    /// LMaximum number of modules to draw per row
    /// </summary>
    private readonly int rowCount = 6;

    /// <summary>
    /// 所有资源模块列表
    /// List of all resource modules
    /// </summary>
    protected List<BundleModuleData> moduleDataList;

    /// <summary>
    /// 行列表 用于分行绘制
    /// The row and column table is used to draw rows
    /// </summary>
    protected List<List<BundleModuleData>> rowModuleDataList;

    /// <summary>
    /// 当前打包平台
    /// Current packaging platform
    /// </summary>
    protected string currentPlatform;

    /// <summary>
    /// 初始化函数
    /// Initialize the function
    /// </summary>
    public virtual void Initialize()
    {
        // 获取多模块资源配置列表
        // Obtain the multi-module resource configuration list
        moduleDataList = BuildBundleConfigura.Instance.assetBundleConfigList;
        rowModuleDataList = new List<List<BundleModuleData>>();

        for (int i = 0; i < moduleDataList.Count; i++)
        {
            // 计算模块绘制的行数索引
            // Calculates the index of the number of rows drawn by the module
            int rowIndex = Mathf.FloorToInt(i / rowCount);
            if (rowModuleDataList.Count < rowIndex + 1)
            {
                // 如果超出行最大模块数 则新增一行数据
                // If the maximum number of modules in a row is exceeded, add a new row of data
                rowModuleDataList.Add(new List<BundleModuleData>());
            }
            // 往行列表中添加模块数据
            // Adds module data to the column table
            rowModuleDataList[rowIndex].Add(moduleDataList[i]);
        }

#if UNITY_IOS
        currentPlatform = "BuildSettings.iPhone";
#elif UNITY_ANDROID
        currentPlatform = "BuildSettings.Android";
#endif
    }

    /// <summary>
    /// 绘制方法
    /// Drawing method
    /// </summary>
    [OnInspectorGUI] // 窗口被打开后 按帧进行绘制 // The window is opened and drawn by frame
    public virtual void OnGUI()
    {
        if (rowModuleDataList == null) return;

        // 获取 Unity 内置的图标
        // Get Unity's built-in icon
        GUIContent gUIContent = EditorGUIUtility.IconContent("SceneAsset Icon".Trim(), "测试文字显示");
        gUIContent.tooltip = "单击进行选中/取消\n快速双击可打开配置窗口";

        // 行
        // row
        for (int i = 0; i < rowModuleDataList.Count; i++)
        {
            if (rowModuleDataList[i].Count == 0) continue;

            GUILayout.BeginHorizontal();
            {
                // 列
                // column
                for (int j = 0; j < rowModuleDataList[i].Count; j++)
                {
                    // 取得当前列表中的第 i 行的第 j 个模块数据
                    // Gets the jTH module data in row i in the current list
                    BundleModuleData bundleModuleData = rowModuleDataList[i][j];

                    if (GUILayout.Button(gUIContent, GUILayout.Width(btnWidth), GUILayout.Height(btnHeight)))
                    {
                        bundleModuleData.isBuild = !bundleModuleData.isBuild;

                        // 检测按钮双击
                        // Double-click the detection button
                        if (Time.realtimeSinceStartup - bundleModuleData.lastClickBtnTime <= 0.18f)
                        {
                            AssetBundleModuleConfigWindow.ShowModuleConfigWindow(bundleModuleData.moduleName);
                        }
                        bundleModuleData.lastClickBtnTime = Time.realtimeSinceStartup;
                    }
                    GUI.Label(new Rect((j + 1) * 20 + (j * 112), 150 * (i + 1) + i * 20, 115, 20), bundleModuleData.moduleName, new GUIStyle { alignment = TextAnchor.MiddleCenter });

                    // 绘制按钮选中的高亮效果
                    // Draw the highlight effect selected by the button
                    if (bundleModuleData.isBuild)
                    {
                        GUIStyle gUIStyle = UnityEditorUtility.GetGUIStyle("LightmapEditorSelectedHighlight");
                        gUIStyle.contentOffset = new Vector2(100, -70);
                        GUI.Toggle(new Rect(10 + (j * 133), -160 + 1 * (i + 1) + ((i + 1) * 170), 120, 160), true, EditorGUIUtility.IconContent("Collab"), gUIStyle);
                    }
                }

                // 绘制添加资源模块按钮
                // Draw Add resource module button
                if (i == rowModuleDataList.Count - 1)
                {
                    if (rowModuleDataList[i].Count < rowCount)
                    {
                        DrawAddModuleBundleButton();
                    }
                }
            }
            GUILayout.EndHorizontal();

            // 绘制添加资源模块按钮
            // Draw Add resource module button
            if (i == rowModuleDataList.Count - 1)
            {
                if (rowModuleDataList[i].Count == rowCount)
                {
                    GUILayout.BeginHorizontal();
                    DrawAddModuleBundleButton();
                    GUILayout.EndHorizontal();
                }
            }
        }

        if (rowModuleDataList.Count == 0)
        {
            // 绘制添加资源模块按钮
            // Draw Add resource module button
            DrawAddModuleBundleButton();
        }

        DrawBuildButtons();
    }

    /// <summary>
    /// 绘制打包按钮
    /// Draw pack button
    /// </summary>
    public virtual void DrawBuildButtons()
    {
    }

    /// <summary>
    /// 打包资源
    /// Package resource
    /// </summary>
    public virtual void BuildBundle()
    {
    }

    /// <summary>
    /// 绘制添加资源模块按钮
    /// Draws the Add resource module button
    /// </summary>
    public virtual void DrawAddModuleBundleButton()
    {
    }
}