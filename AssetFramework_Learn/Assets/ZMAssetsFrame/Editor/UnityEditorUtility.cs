using UnityEngine;

public class UnityEditorUtility
{
    /// <summary>
    /// 获取指定名字的Unity内置GUIStyle
    /// Gets Unity's built-in GUIStyle for the specified name
    /// </summary>
    /// <param name="styleName">GUIStyle的名字 GUIStyle's name</param>
    /// <returns></returns>
    public static GUIStyle GetGUIStyle(string styleName)
    {
        GUIStyle gUIStyle = null;
        foreach (var item in GUI.skin.customStyles)
        {
            if (string.Equals(item.name.ToLower(), styleName.ToLower()))
            {
                gUIStyle = item;
                break;
            }
        }
        return gUIStyle;
    }
}