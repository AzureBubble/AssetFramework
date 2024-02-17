using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.Callbacks;

public class AssetBundleBuildWindow : OdinMenuEditorWindow
{
    // 窗口的宽高设置
    // Set the width and height of the window
    private static int width = 985;

    private static int height = 612;
    //private static bool isDoubleClick;

    public BuildAssetBundleWindow buildAssetBundleWindow = new BuildAssetBundleWindow();
    public BuildHotPatchWindow buildHotPatchWindow = new BuildHotPatchWindow();
    public AssetBundleBuildDescriptionWindow descriptionWindow = null;

    [MenuItem("GameTool/AssetBundleTool")]
    public static void ShowAssetBundleWindow()
    {
        AssetBundleBuildWindow window = GetWindow<AssetBundleBuildWindow>();
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(width, height);
        window.ForceMenuTreeRebuild(); // 强制绘制菜单树 // Force a menu tree to be drawn
    }

    [OnOpenAsset]
    private static bool OpenAssetBundleConfig(int instanceID, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceID) is BuildBundleConfigura)
        {
            //isDoubleClick = true;
            ShowAssetBundleWindow();
            return true;
        }
        return false;
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        // 对每个模块的按钮进行初始化
        // Initialize buttons for each module
        buildAssetBundleWindow.Initialize();
        buildHotPatchWindow.Initialize();

        OdinMenuTree menuTree = new OdinMenuTree(supportsMultiSelect: false)
        {
            { "Build", null,EditorIcons.House },
            { "Build/AssetBundle", buildAssetBundleWindow ,EditorIcons.UnityLogo },
            { "Build/HotPatch", buildHotPatchWindow ,EditorIcons.UnityLogo },
        };

        //if (!isDoubleClick)
        //{
        //    descriptionWindow = new AssetBundleBuildDescriptionWindow();

        //    menuTree.Selection.Add(menuTree.MenuItems[0]);
        //}

        //menuTree.SortMenuItemsByName();

        //isDoubleClick = false;

        return menuTree;
    }
}