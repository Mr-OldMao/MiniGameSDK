using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class AlipayUtil
{
    
    public static string GenerateMD5FromFile(string filePath)
    {
        using (var md5 = MD5.Create())
        {
            using (var stream = File.OpenRead(filePath))
            {
                byte[] retVal = md5.ComputeHash(stream);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }

    public static string GetFileSize(string filePath)
    {
        FileInfo fileInfo = new FileInfo(filePath);
        return fileInfo.Length.ToString();
    }

    public static string GetZipLength(string filePath)
    {
        var fileStream = File.OpenRead(filePath);
        return fileStream.Length.ToString();
    }

    public static AlipayBuildConfig GetAlipayBuildConfig()
    {
        string configPath = "Assets/ALIPAY-SDK/Editor/AlipayBuildConfig.asset";
        AlipayBuildConfig alipayBuildConfig = AssetDatabase.LoadAssetAtPath<AlipayBuildConfig>(configPath);
        if (alipayBuildConfig == null)
        {
            string folderPath = Path.GetDirectoryName(configPath);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }
            alipayBuildConfig = ScriptableObject.CreateInstance<AlipayBuildConfig>();
            AssetDatabase.CreateAsset(alipayBuildConfig, configPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        return alipayBuildConfig;
    }

    public static void OpenFolder(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        // 获取文件夹路径
        string folderPath = System.IO.Path.GetDirectoryName(path);
        // 打开文件夹
        EditorUtility.RevealInFinder(folderPath);
    }

    public static void PreprocessSymbols(string symFile1, string symbolsPath)
    {
        //使用异常捕获处理可能发生的任何I/O错误，并通过Unity的日志系统打印错误消息。
        try
        {
            //使用StreamReader打开symbols文件，逐行读取。
            StreamReader streamReader = new StreamReader(symFile1);
            //对每一行，找到冒号（':'）字符的索引位置，然后根据该索引拆分每一行为键（整数）和值（字符串）。将键值对存入字典中。
            Dictionary<int, string> dictionary = new Dictionary<int, string>();
            string text;
            while ((text = streamReader.ReadLine()) != null)
            {
                int num = text.IndexOf(':');
                if (num >= 0)
                {
                    int key = int.Parse(text.Substring(0, num));
                    string value = text.Substring(num + 1);
                    dictionary.Add(key, DecodeEscapeSequences(value));
                }
            }
            //字典转换为列表并根据键进行排序（整数升序）。
            List<KeyValuePair<int, string>> sortedList = dictionary.ToList();
            sortedList.Sort((KeyValuePair<int, string> P_0, KeyValuePair<int, string> P_1) => P_0.Key.CompareTo(P_1.Key));
            //使用StreamWriter创建或覆写导出JSON格式的文件。
            using (StreamWriter streamWriter = new StreamWriter(symbolsPath))
            {
                streamWriter.WriteLine("{");
                for (int i = 0; i < sortedList.Count; i++)
                {
                    var item = sortedList[i];
                    // 条目格式化为JSON，并且在每个条目后面加上逗号，除了最后一个条目
                    string lineToWrite = string.Format(" \"{0}\": \"{1}\"{2}",
                        item.Key, item.Value, i < sortedList.Count - 1 ? "," : "");

                    streamWriter.Write(lineToWrite);
                    if (i < sortedList.Count - 1)
                    {
                        // 如果不是最后一条，输出换行符
                        streamWriter.WriteLine();
                    }
                }
                streamWriter.WriteLine("\n}");
            }
            streamReader.Close();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }

    private static string DecodeEscapeSequences(string input)
    {
        // 使用正则表达式匹配转义序列并将其替换为实际字符
        string pattern = @"\\([0-9A-Fa-f]{2})";
        return Regex.Replace(input, pattern, match =>
        {
            int hexValue = Convert.ToInt32(match.Groups[1].Value, 16);
            char charValue = (char)hexValue;
            return charValue.ToString();
        });
    }

    /// <summary>
    /// zip 打包文件
    /// </summary>
    /// <param name="fileToZip"></param>
    /// <param name="zipFilePath"></param>
    /// <returns></returns>
    public static bool ZipCreateFromFile(string fileToZip, string zipFilePath)
    {
        try
        {
            if (!File.Exists(fileToZip))
            {
                Debug.LogError("指定的文件不存在：" + fileToZip);
                return false;
            }
            using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    string entryName = Path.GetFileName(fileToZip);
                    ZipArchiveEntry entry = archive.CreateEntry(entryName);
                    using (FileStream fileStream = new FileStream(fileToZip, FileMode.Open, FileAccess.Read))
                    {
                        using (Stream entryStream = entry.Open())
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                    Debug.Log("Create Wasm File ZIP ：" + zipFilePath);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError(" Create ZIP  File Error ：" + e.Message);
            return false;
        }
    }

    /// <summary>
    /// 使用 ZipArchive 将整个文件夹打包成 zip 文件
    /// </summary>
    /// <param name="folderToZip">要打包的文件夹路径</param>
    /// <param name="zipFilePath">目标 zip 文件路径</param>
    /// <returns></returns>
    public static bool ZipCreateFromFolder(string folderToZip, string zipFilePath)
    {
        try
        {
            if (!Directory.Exists(folderToZip))
            {
                Debug.LogError("指定的文件夹不存在：" + folderToZip);
                return false;
            }

            using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    AddFolderToZip(archive, folderToZip, Path.GetFileName(folderToZip));
                }
            }

            Debug.Log("Create Folder ZIP：" + zipFilePath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Create ZIP File Error：" + e.Message);
            return false;
        }
    }

    private static void AddFolderToZip(ZipArchive archive, string folderPath, string entryName)
    {
        string[] files = Directory.GetFiles(folderPath, "*", SearchOption.AllDirectories);

        foreach (string file in files)
        {
            string entryPath = Path.Combine(entryName, file.Substring(folderPath.Length + 1));
            ZipArchiveEntry zipEntry = archive.CreateEntry(entryPath);

            using (FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read))
            using (Stream entryStream = zipEntry.Open())
            {
                fileStream.CopyTo(entryStream);
            }
        }
    }

    /// <summary>
    /// 从zip文件解压到目标文件夹
    /// </summary>
    /// <param name="zipFilePath"></param>
    /// <param name="extractFolderPath"></param>
    public static bool ExtractToDirectory(string zipFilePath, string extractFolderPath)
    {
        try
        {
            using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Open))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Read))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        string filePath = Path.Combine(extractFolderPath, entry.FullName);
                        if (string.IsNullOrEmpty(entry.Name))
                        {
                            Directory.CreateDirectory(filePath);
                        }
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                            entry.Open().CopyTo(new FileStream(filePath, FileMode.Create));
                        }
                    }
                }
            }
            Debug.Log("解压文件成功：" + extractFolderPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(" 解压文件错误：" + e.Message);
            return false;
        }
    }

    public static bool IsZipFileValid(string zipFilePath)
    {
        try
        {
            using (FileStream fs = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read))
            using (ZipArchive archive = new ZipArchive(fs, ZipArchiveMode.Read))
            {
                // 检查是否存在至少一个有效的条目
                return archive.Entries.Count > 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"ZIP文件校验出错：{e.Message}");
            return false;
        }
    }

    /// <summary>
    ///  brotli压缩文件
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="brFilePath"></param>
    /// <returns></returns>
    public static bool BrotliFile(string filePath, string brFilePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError("指定的文件不存在：" + filePath);
                return false;
            }
            using (FileStream inputStream = File.OpenRead(filePath))
            {
                FileStream outputStream = File.OpenWrite(brFilePath);
                using (var compressStream = new BrotliStream(outputStream, CompressionMode.Compress))
                {
                    inputStream.CopyTo(compressStream);
                    Debug.Log("Create Brotli File ZIP ：" + filePath);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("压缩文件错误：" + e.Message);
            return false;
        }
    }

}
