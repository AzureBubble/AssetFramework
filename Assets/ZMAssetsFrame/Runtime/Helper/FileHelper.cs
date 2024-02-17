using System.IO;

public class FileHelper
{
    /// <summary>
    /// 删除文件夹
    /// </summary>
    /// <param name="folderPath"></param>
    public static void DeleteFolder(string folderPath)
    {
        if (Directory.Exists(folderPath))
        {
            string[] pathsArr = Directory.GetFiles(folderPath, "*");
            foreach (string path in pathsArr)
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            Directory.Delete(folderPath);
        }
    }

    /// <summary>
    /// 写入文件到指定路径
    /// </summary>
    /// <param name="filePath">写入路径</param>
    /// <param name="data">文件字节数据</param>
    public static void WriteFile(string filePath, byte[] data)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        using (var stream = File.Create(filePath))
        {
            stream.Write(data, 0, data.Length);
            stream.Dispose();
            stream.Close();
        }
    }
}