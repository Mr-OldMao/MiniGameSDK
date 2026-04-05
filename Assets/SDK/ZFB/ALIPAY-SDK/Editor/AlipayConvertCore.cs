using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LitJson;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.Rendering;

public class AlipayConvertCore
{
    public static AlipayBuildConfig AlipayConfig;
    private static RuleConfig replaceRules = new RuleConfig();
    private static RuleConfig renderAnalysisRules;
    private const string webglDir = "webgl"; // 导出的webgl目录
    private const string alipayGameDir = "alipaygame"; // 支付宝小游戏目录
    private const string frameworkFileName = "webgl.wasm.framework.unityweb.js";
    private const string symbolFileName = "webgl.wasm.symbols.unityweb";
    private const string wasmFileName = "webgl.wasm.code.unityweb.wasm";
    private const string dataFilename = "webgl.data.unityweb.bin.data";
    public static string WebglPath => Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir);
    public static string AlipayGamePath => Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, alipayGameDir);
    public static string WasmFileMd5 = string.Empty;
    public static string DataFileSize = string.Empty;
    public static string DataFileMd5 = string.Empty;
    public static string WebGLDataFilePath = string.Empty;
    public static string WebGLDataFileName = string.Empty;
    private static string buildToolVersion = "1.11.25120202";
    private static bool isInit = false;

    static AlipayConvertCore()
    {
        Init();
    }

    public static void Init()
    {
        if (isInit)
        {
            return;
        }

        AlipayConfig = AlipayUtil.GetAlipayBuildConfig();
        PlayerSettings.WebGL.threadsSupport = false;
        PlayerSettings.runInBackground = false;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
        //PlayerSettings.WebGL.template = "PROJECT:AlipayDefault";
        //PlayerSettings.WebGL.template = "PROJECT:Default";
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.dataCaching = false;
#if UNITY_2021_2_OR_NEWER
        PlayerSettings.WebGL.debugSymbolMode = WebGLDebugSymbolMode.External;
#else
        PlayerSettings.WebGL.debugSymbols = true;
#endif
        isInit = true;
    }

    /// <summary>
    /// 检查必要的配置--->导出路径
    /// </summary>
    private static bool CheckConfig()
    {
        string dir = Path.Combine(Application.dataPath, "ALIPAY-SDK");
        if (!Directory.Exists(dir))
        {
            EditorUtility.DisplayDialog("警告", "当前AlipaySDK的文件夹路径不正确，请确认SDK路径", "确认");
            return false;
        }
        if (string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.DerivedPath))
        {
            EditorUtility.DisplayDialog("警告", "项目导出路径未填写，请选择导出路径", "确认");
            return false;
        }
        if (AlipayConfig.AlipayProjectCfg.DataFileLoadType == "CDN" && string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.CDN))
        {
            EditorUtility.DisplayDialog("警告", "当前配置使用CDN加载方式，但CDN地址未填写，请填写CDN地址！", "确认");
            return false;
        }
        if (AlipayConfig.CompileOptions.UseStreamingAssets && string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.CDN))
        {
            EditorUtility.DisplayDialog("警告", "当前选择了使用StreamingAssets目录，但CDN地址未填写，请填写CDN地址！", "确认");
            return false;
        }
        if (AlipayConfig.AlipayProjectCfg.LoadingImage == null || string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.LoadingImage.name))
        {
            EditorUtility.DisplayDialog("警告", "当前没有配置启动页背景图，为了更好的用户体验，请配置启动页后再试！", "确认");
            return false;
        }
        return true;
    }

    /// <summary>
    /// 打包WebGL
    /// </summary>
    /// <returns></returns>
    public static bool WebGLBuild()
    {
        //初始化配置
        Init();
        //检查配置
        if (CheckConfig())
        {
            if (!Build())
            {
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// 转换为支付宝小游戏
    /// </summary>
    /// <returns></returns>
    public static bool AlipayConvert()
    {
#if ALIPAY_ADDRESSABLES_CHECKER
        if (!AlipayAddressablesConfigChecker.CheckAddressableConfig())
        {
            Debug.LogError("请正确配置Addressables！");
            return false;
        }
#endif

        if (!CheckConfig())
        {
            return false;
        }
        //创建输出目录
        if (Directory.Exists(AlipayGamePath))
        {
            Directory.Delete(AlipayGamePath, true);
        }
        Directory.CreateDirectory(AlipayGamePath);
        //转换文件：webgl.wasm.framework.unityweb.js
        FrameworkConvertCode();
        //处理文件：webgl.wasm.symbols.unityweb
        var symbolSucceed = HandleSymbolFile();
        //压缩数据文件
        var dataSucceed = CompressData();
        //压缩代码文件
        var wasmSucceed = CompressWasmCode();

        if (!dataSucceed || !wasmSucceed)
        {
            return false;
        }
        //完成转换、处理所有文件
        FinishConvert();
        return true;
    }

    /// <summary>
    /// 打包WebGL并转换为支付宝小游戏
    /// </summary>
    /// <returns></returns>
    public static bool WebglBuildAndConvert()
    {
        //初始化配置
        Init();

#if ALIPAY_ADDRESSABLES_CHECKER
        if (!AlipayAddressablesConfigChecker.CheckAddressableConfig())
        {
            Debug.LogError("请正确配置Addressables！");
            return false;
        }
#endif

        //检查配置
        if (CheckConfig())
        {
            //创建输出目录
            if (Directory.Exists(AlipayGamePath))
            {
                Directory.Delete(AlipayGamePath, true);
            }
            Directory.CreateDirectory(AlipayGamePath);
            if (Directory.Exists(WebglPath))
            {
                Directory.Delete(WebglPath, true);
            }
            Directory.CreateDirectory(WebglPath);

            SettingAlipayTextureMinJslib();
            UpdateGraphicAPI();
            EditorUtility.SetDirty(AlipayConfig);
            AssetDatabase.SaveAssets();

            //UnityWebGL打包
            if (!Build())
            {
                return false;
            }
#if UNITY_2021_2_OR_NEWER
            if (!AlipayConfig.CompileOptions.DevelopBuild)
            {
                Debug.Log("Preprocess Smbols...");
                var symFile1 = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Library", "Bee", "artifacts", "WebGL", "build", "debug_WebGL_wasm", "build.js.symbols");
                AlipayUtil.PreprocessSymbols(symFile1, GetWebGLSymbolPath());
            }
#endif
            //转换文件：webgl.wasm.framework.unityweb.js
            FrameworkConvertCode();
            //处理文件：webgl.wasm.symbols.unityweb
            var symbolSucceed = HandleSymbolFile();
            //压缩数据文件
            var dataSucceed = CompressData();
            //压缩代码文件
            var wasmSucceed = CompressWasmCode();
            //完成转换、处理所有文件
            FinishConvert();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Unity打包WebGL
    /// </summary>
    /// <returns></returns>
    private static bool Build()
    {
        Debug.Log("开始打包。。。");

        PlayerSettings.WebGL.emscriptenArgs = string.Empty;

#if UNITY_2021_2_OR_NEWER
        PlayerSettings.WebGL.emscriptenArgs += " -s EXPORTED_FUNCTIONS=_sbrk,_emscripten_stack_get_base,_emscripten_stack_get_end";
        PlayerSettings.WebGL.emscriptenArgs += ",_main";
#endif
        PlayerSettings.runInBackground = false;
        PlayerSettings.WebGL.emscriptenArgs += $" -s TOTAL_MEMORY={AlipayConfig.AlipayProjectCfg.MemorySize}MB";
        if (AlipayConfig.CompileOptions.ProfilingMemory)
        {
            PlayerSettings.WebGL.emscriptenArgs += " --memoryprofiler ";
        }
        if (AlipayConfig.CompileOptions.profilingFuncs)
        {
            PlayerSettings.WebGL.emscriptenArgs += " --profiling-funcs ";
        }
        string original_EXPORTED_RUNTIME_METHODS = "\"ccall\",\"cwrap\",\"stackTrace\",\"addRunDependency\",\"removeRunDependency\",\"FS_createPath\",\"FS_createDataFile\",\"stackTrace\",\"writeStackCookie\",\"checkStackCookie\"";
        // 添加额外的EXPORTED_RUNTIME_METHODS
        string additional_EXPORTED_RUNTIME_METHODS = ",\"lengthBytesUTF8\",\"stringToUTF8\"";
        PlayerSettings.WebGL.emscriptenArgs += " -s EXPORTED_RUNTIME_METHODS='[" + original_EXPORTED_RUNTIME_METHODS + additional_EXPORTED_RUNTIME_METHODS + "]'";

#if UNITY_2022_1_OR_NEWER
#if UNITY_2021_2_OR_NEWER
        // 默认更改为OptimizeSize，减少代码包体积
        PlayerSettings.SetIl2CppCodeGeneration(NamedBuildTarget.WebGL, AlipayConfig.CompileOptions.Il2CppOptimizeSize ? Il2CppCodeGeneration.OptimizeSize : Il2CppCodeGeneration.OptimizeSpeed);
#else
        EditorUserBuildSettings.il2CppCodeGeneration = AlipayConfig.CompileOptions.Il2CppOptimizeSize ? Il2CppCodeGeneration.OptimizeSize : Il2CppCodeGeneration.OptimizeSpeed;
#endif
#endif
        UnityEngine.Debug.Log("PlayerSettings.WebGL.emscriptenArgs : " + PlayerSettings.WebGL.emscriptenArgs);

        BuildOptions option = BuildOptions.None;
        if (AlipayConfig.CompileOptions.DevelopBuild)
        {
            option |= BuildOptions.Development;
        }
        if (AlipayConfig.CompileOptions.AutoProfile)
        {
            option |= BuildOptions.ConnectWithProfiler;
        }
        if (AlipayConfig.CompileOptions.ScriptOnly)
        {
            option |= BuildOptions.BuildScriptsOnly;
        }
#if UNITY_2021_2_OR_NEWER
        if (AlipayConfig.CompileOptions.CleanBuild)
        {
            option |= BuildOptions.CleanBuildCache;
        }
#endif
        if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.WebGL)
        {
            UnityEngine.Debug.LogFormat("[Builder] Current target is: {0}, switching to: {1}", EditorUserBuildSettings.activeBuildTarget, BuildTarget.WebGL);
            return false;
        }
        var projDir = Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir);
        var result = BuildPipeline.BuildPlayer(GetScenePaths(), projDir, BuildTarget.WebGL, option);
        if (result.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            UnityEngine.Debug.LogFormat("[Builder] BuildPlayer failed. emscriptenArgs:{0}", PlayerSettings.WebGL.emscriptenArgs);
            return false;
        }
        return true;
    }

    /// <summary>
    /// 获取选中出包的场景路径
    /// </summary>
    /// <returns></returns>
    private static string[] GetScenePaths()
    {
        List<string> scenes = new List<string>();
        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            var scene = EditorBuildSettings.scenes[i];
            UnityEngine.Debug.LogFormat("[Builder] Scenes [{0}]: {1}, [{2}]", i, scene.path, scene.enabled ? "x" : " ");
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }
        return scenes.ToArray();
    }
    /// <summary>
    /// framework.js文件转换
    /// </summary>
    private static void FrameworkConvertCode()
    {
        string alipayOutPath = AlipayGamePath;

        string text = string.Empty;
#if UNITY_2020_1_OR_NEWER
        text = File.ReadAllText(Path.Combine(WebglPath, "Build", "webgl.framework.js"), Encoding.UTF8);
#else
        text = File.ReadAllText(Path.Combine(WebglPath, "Build", "webgl.wasm.framework.unityweb"), Encoding.UTF8);
#endif
        for (int i = 0; i < replaceRules.rules.Count; i++)
        {
            var rule = replaceRules.rules[i];
            text = Regex.Replace(text, rule.old, rule.newStr);
        }

        text = AudioAdaptation(text);

        string endString = string.Empty;
        if (text.Contains("UnityModule"))
        {
            endString = ";module.exports = UnityModule;";
        }
        else if (text.Contains("unityFramework")) //2021, 2022, unity 6  
        {
            endString = ";module.exports = unityFramework;";
        }
        else if (text.Contains("tuanjieFramework"))//团结
        {
            endString = ";module.exports = tuanjieFramework;";
        }
        else
        {
            if (text.StartsWith("(") && text.EndsWith(")"))
            {
                text = text.Substring(1, text.Length - 2);
            }
            text = "module.exports = " + text;
        }
        string startInput = "const APGameGlobal = require('./alipay-sdk/global').default;";
        text = startInput + text + endString;
        File.WriteAllText(Path.Combine(alipayOutPath, frameworkFileName), text, new UTF8Encoding(false));
        Debug.Log($"{frameworkFileName} 转换完成！！! ");
    }

    /// <summary>
    /// 处理data资源文件
    /// </summary>
    /// <returns></returns>
    public static bool CompressData()
    {
        var dataPath = GetWebGLDataPath();
        if (!File.Exists(dataPath))
        {
            Debug.LogError("data file not exist");
            return false;
        }
        DataFileSize = AlipayUtil.GetFileSize(dataPath);
        DataFileMd5 = AlipayUtil.GenerateMD5FromFile(dataPath);
        WebGLDataFileName = DataFileMd5 + "." + dataFilename;
        WebGLDataFilePath = Path.Combine(WebglPath, WebGLDataFileName);
        // 拷贝data文件到原build目录下->webgl
        File.Copy(dataPath, WebGLDataFilePath, true);

        if (AlipayConfig.AlipayProjectCfg.UseDataZip)
        {
            string zipFilePath = WebGLDataFilePath + ".zip";

            var createZipResult = AlipayUtil.ZipCreateFromFile(WebGLDataFilePath, zipFilePath);
            if (!createZipResult)
            {
                Debug.LogError("Zip Data File Failed ！");
                return false;
            }

            Debug.Log($"Zip Data File  Successfully: {zipFilePath}");
            if (AlipayConfig.AlipayProjectCfg.UseDataBrotli)
            {
                string brFilePath = WebGLDataFilePath + ".br";
                var createBrResult = AlipayUtil.BrotliFile(WebGLDataFilePath, brFilePath);
                if (!createBrResult)
                {
                    Debug.LogError("Brotli Wasm File Failed ！");
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 处理wasm文件
    /// </summary>
    /// <returns></returns>
    public static bool CompressWasmCode()
    {
        var codePath = GetWebGLCodePath();
        if (!File.Exists(codePath))
        {
            Debug.LogError("wasm file not exist");
            return false;
        }
        WasmFileMd5 = AlipayUtil.GenerateMD5FromFile(codePath);
        string fileName = WasmFileMd5 + "." + wasmFileName;
        string webglWasmFile = Path.Combine(WebglPath, fileName);
        File.Copy(codePath, webglWasmFile, true);
        string alipayWasmFile = Path.Combine(AlipayGamePath, fileName) + ".zip";
        var createResult = AlipayUtil.ZipCreateFromFile(webglWasmFile, alipayWasmFile);
        if (!createResult)
        {
            Debug.LogError("Zip Wasm File Failed ！");
            return false;
        }
        return true;
    }

    #region 符号文件处理

    /// <summary>
    /// 处理symbol符号文件
    /// </summary>
    /// <returns></returns>
    public static bool HandleSymbolFile()
    {
        var symbolPath = GetWebGLSymbolPath();
        string savePath = Path.Combine(AlipayGamePath, symbolFileName);
        if (!File.Exists(symbolPath))
        {
            Debug.LogWarning("symbol file not exist");
            return false;
        }

        string symbolContent = GetSymbolFile(symbolPath);
        File.WriteAllText(savePath, symbolContent);
        return true;
    }

    private static string GetSymbolFile(string symbolPath)
    {
        string content = File.ReadAllText(symbolPath);
#if UNITY_2021_1_OR_NEWER
        return ProcessJsonValues(content);
#elif UNITY_2020_1_OR_NEWER
        string cleaned = Regex.Replace(
        content,
        ":\s*"(.?)\|.?"",
        m => ": "" + m.Groups[1].Value + """
        );
        return ProcessJsonValues(cleaned);
#elif UNITY_2018_1_OR_NEWER
        string[] lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        Dictionary<string, string> symbols = new Dictionary<string, string>();
        for (int i = 0; i < lines.Length; i++)
        {
        string pattern = @"(\d+):'(.*?)'";
        Match match = Regex.Match(lines[i], pattern);
        if (match.Success)
        {
        string key = match.Groups[1].Value;
        string value = match.Groups[2].Value;
        symbols[key] = value;
        }
        }
        StringBuilder jsonBuilder = new StringBuilder();
        jsonBuilder.Append("{");
        bool first = true;
        foreach (var pair in symbols.OrderBy(kvp => int.Parse(kvp.Key)))
        {
        if (!first) jsonBuilder.Append(",");
        first = false;
        jsonBuilder.Append($""{pair.Key}":"{pair.Value}"");
        }
        jsonBuilder.Append("}");
        return ProcessJsonValues(jsonBuilder.ToString());
#endif
    }

    private static string ProcessJsonValues(string json)
    {
        string pattern = @"(?<key>""[^""]+""):\s*""(?<val>[^""]*)""";
        return Regex.Replace(json, pattern, m =>
        {
            var key = m.Groups["key"].Value;
            var val = m.Groups["val"].Value;
            var newVal = EncodeSymbol(val);
            return $"{key}: \"{newVal}\"";
        });
    }

    private static string EncodeSymbol(string s)
    {
        return Regex.Replace(s, @"[ ,()\[\]\{\}]", m => "\\\\" + ((int)m.Value[0]).ToString("x2"));
    }
    #endregion

    /// <summary>
    /// 工程转换完成
    /// </summary>
    public static void FinishConvert()
    {
        string alipayDataFilePath = string.Empty;
        string dataPackage = string.Empty;
        string loadFromSubpackage = "false";
        string url = string.Empty;
        string streamingAssetsUrl = string.Empty;
        string loadingImage = string.Empty;
        Subpackage[] subPackages = null;

        if (!string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.CDN))
        {
            AlipayConfig.AlipayProjectCfg.CDN = AlipayConfig.AlipayProjectCfg.CDN.TrimEnd('/');
            if (AlipayConfig.CompileOptions.UseStreamingAssets)
            {
                streamingAssetsUrl = AlipayConfig.AlipayProjectCfg.CDN + "/" + "StreamingAssets";
            }
        }

        if (AlipayConfig.AlipayProjectCfg.DataFileLoadType == E_DataFileLoadType.游戏包内.ToString())
        {
            loadFromSubpackage = "true";
            dataPackage = "data-package";
            subPackages = new Subpackage[] { new Subpackage() { name = dataPackage, root = "data-package/" }
};
            alipayDataFilePath = dataPackage + "/" + WebGLDataFileName; //设置配置文件中datafile路径 
                                                                        //创建data-package目录
            string dataFilePath = Path.Combine(AlipayGamePath, dataPackage);
            if (Directory.Exists(dataFilePath))
            {
                Directory.Delete(dataFilePath, true);
            }
            Directory.CreateDirectory(dataFilePath);
            //创建data-package/game.js文件
            File.WriteAllText(Path.Combine(dataFilePath, "game.js"), string.Empty);
            //将data文件从webgl下复制到alipay/data-package目录下
            File.Copy(WebGLDataFilePath, Path.Combine(dataFilePath, WebGLDataFileName), true);
        }
        else
        {
            if (!string.IsNullOrEmpty(AlipayConfig.AlipayProjectCfg.CDN))
            {
                var subpath = (AlipayConfig.AlipayProjectCfg.DataFileSubPath ?? "").Trim('/');
                if (string.IsNullOrEmpty(subpath))
                    url = $"{AlipayConfig.AlipayProjectCfg.CDN}/{WebGLDataFileName}";
                else
                    url = $"{AlipayConfig.AlipayProjectCfg.CDN}/{subpath}/{WebGLDataFileName}";
            }
            else
            {
                Debug.LogWarning("您没有填写资源CDN, 请在game.js中填写data文件的url！！！");
                if (AlipayConfig.CompileOptions.UseStreamingAssets)
                {
                    Debug.LogWarning("您没有填写资源CDN，但是勾选了使用StreamingAssets目录。请在game.js中填写streamingAssetsUrl路径！！！");
                }
            }
        }

        if (AlipayConfig.AlipayProjectCfg.LoadingImage != null)
        {
            string sourceFilePath = AssetDatabase.GetAssetPath(AlipayConfig.AlipayProjectCfg.LoadingImage);
            string fileNameWithExtension = Path.GetFileName(sourceFilePath);

            string targetDirectory = Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, alipayGameDir);
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            string targetFilePath = Path.Combine(targetDirectory, fileNameWithExtension);
            try
            {
                File.Copy(sourceFilePath, targetFilePath, true);
                Debug.Log($"加载图处理成功: {targetFilePath}");
                loadingImage = fileNameWithExtension;
            }
            catch (IOException ioEx)
            {
                Debug.LogError($"加载图处理失败: {ioEx.Message}");
            }
        }
        else
        {
            EditorUtility.DisplayDialog("注意", "为了更好的用户体验，加载图为必填项，请配置加载图。否者审核将不予通过！", "确定");
        }

        string targetFolderPath = Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, alipayGameDir);
        string defaultPath = Path.Combine(Application.dataPath, "ALIPAY-SDK", "Editor", "Default");

        GenerateDefaultFile(defaultPath, targetFolderPath, streamingAssetsUrl, url, DataFileSize, DataFileMd5, dataPackage, alipayDataFilePath, loadFromSubpackage, loadingImage);
        GenerateGameJson(subPackages, streamingAssetsUrl, url, Convert.ToInt32(DataFileSize), DataFileMd5, dataPackage, alipayDataFilePath, loadFromSubpackage.Equals("true", StringComparison.OrdinalIgnoreCase));
        Debug.Log("完成转换");
    }

    /// <summary>
    /// 处理game.js文件以及默认js文件
    /// </summary>
    /// <param name="defaultFolderPath"></param>
    /// <param name="targetFolderPath"></param>
    /// <param name="streamingAssetsUrl"></param>
    /// <param name="url"></param>
    /// <param name="dataFileSize"></param>
    /// <param name="dataFileMd5"></param>
    /// <param name="dataPackage"></param>
    /// <param name="alipayDataFilePath"></param>
    /// <param name="loadFromSubpackage"></param>
    /// <param name="loadingImage"></param>
    public static void GenerateDefaultFile(string defaultFolderPath, string targetFolderPath, string streamingAssetsUrl, string url, string dataFileSize,
                                    string dataFileMd5, string dataPackage, string alipayDataFilePath,
                                    string loadFromSubpackage, string loadingImage)
    {
        CopyFilesRecursively(defaultFolderPath, targetFolderPath);
        string gameJsFilePath = Path.Combine(targetFolderPath, "game.js");

        string zipurl = string.Empty;
        string brUrl = string.Empty;
        if (!string.IsNullOrEmpty(url))
        {
            zipurl = AlipayConfig.AlipayProjectCfg.UseDataZip ? url + ".zip" : string.Empty;
            brUrl = AlipayConfig.AlipayProjectCfg.UseDataBrotli ? url + ".br" : string.Empty;
        }
        if (File.Exists(gameJsFilePath))
        {
            string gameJsContent = File.ReadAllText(gameJsFilePath);
            gameJsContent = Regex.Replace(gameJsContent, @"streamingAssetsUrl:\s*'[^']*'", $"streamingAssetsUrl: '{streamingAssetsUrl}'");
            gameJsContent = Regex.Replace(gameJsContent, @"url:\s*'[^']*'", $"url: '{url}'");
            gameJsContent = Regex.Replace(gameJsContent, @"zipUrl:\s*'[^']*'", $"zipUrl: '{zipurl}'");
            gameJsContent = Regex.Replace(gameJsContent, @"brUrl:\s*'[^']*'", $"brUrl: '{brUrl}'");
            gameJsContent = Regex.Replace(gameJsContent, @"size:\s*\d+", $"size: {dataFileSize}");
            gameJsContent = Regex.Replace(gameJsContent, @"md5:\s*'[^']*'", $"md5: '{dataFileMd5}'");
            gameJsContent = Regex.Replace(gameJsContent, @"subpackage:\s*'[^']*'", $"subpackage: '{dataPackage}'");
            gameJsContent = Regex.Replace(gameJsContent, @"path:\s*'[^']*'", $"path: '{alipayDataFilePath}'");
            gameJsContent = Regex.Replace(gameJsContent, @"loadFromSubpackage:\s*(true|false)", $"loadFromSubpackage: {loadFromSubpackage}");
            gameJsContent = Regex.Replace(gameJsContent, @"backgroundImage:\s*'[^']*'", $"backgroundImage: '{loadingImage}'");
            File.WriteAllText(gameJsFilePath, gameJsContent);
        }
        else
        {
            Debug.LogWarning("默认的game.js文件不存在！生成新的game.js文件替代");
            GenerateJsFile(streamingAssetsUrl, url, DataFileSize, DataFileMd5, dataPackage, alipayDataFilePath, loadFromSubpackage, loadingImage, gameJsFilePath);
        }
        string unityNamespaceFilePath = Path.Combine(targetFolderPath, "unity-namespace.js");
        if (File.Exists(unityNamespaceFilePath))
        {
            string unityNamespaceContent = File.ReadAllText(unityNamespaceFilePath);
            unityNamespaceContent = Regex.Replace(unityNamespaceContent, @"DATA_CDN:\s*'[^']*'", $"DATA_CDN: '{AlipayConfig.AlipayProjectCfg.CDN}'");
            unityNamespaceContent = Regex.Replace(unityNamespaceContent, @"unityVersion:\s*'[^']*'", $"unityVersion: '{Application.unityVersion}'");
            unityNamespaceContent = Regex.Replace(unityNamespaceContent, @"bundleHashLength:\s*\d+", $"bundleHashLength: {AlipayConfig.AlipayProjectCfg.BundleHashLength}");


            string cacheableFileIdentifierArray = CreateJsArrayFromConfigString(AlipayConfig.AlipayProjectCfg.CacheableFileIdentifier);
            string excludeFileIdentifierArray = CreateJsArrayFromConfigString(AlipayConfig.AlipayProjectCfg.ExcludeFileIdentifier);
            unityNamespaceContent = Regex.Replace(unityNamespaceContent, @"const cacheableFileIdentifier = \[[^\]]*\];", $"const cacheableFileIdentifier = {cacheableFileIdentifierArray};");
            unityNamespaceContent = Regex.Replace(unityNamespaceContent, @"const excludeFileIdentifier = \[[^\]]*\];", $"const excludeFileIdentifier = {excludeFileIdentifierArray};");

            File.WriteAllText(unityNamespaceFilePath, unityNamespaceContent);
        }
    }

    private static string CreateJsArrayFromConfigString(string configString)
    {
        if (string.IsNullOrEmpty(configString))
        {
            return "[]";
        }
        var items = configString.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        var jsArray = $"[{string.Join(", ", items.Select(x => $"'{x}'"))}]";
        return jsArray;
    }

    /// <summary>
    /// 拷贝Default文件夹下的文件到目标文件夹
    /// </summary>
    /// <param name="sourceDir"></param>
    /// <param name="destinationDir"></param>
    private static void CopyFilesRecursively(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(destinationDir))
        {
            Directory.CreateDirectory(destinationDir);
        }
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var fileName = Path.GetFileName(file);
            if (fileName.EndsWith(".meta"))
            {
                continue;
            }
            var destFile = Path.Combine(destinationDir, fileName);
            File.Copy(file, destFile, true); // true: 允许覆盖
        }
        foreach (var directory in Directory.GetDirectories(sourceDir))
        {
            var destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
            CopyFilesRecursively(directory, destDir);
        }
    }

    /// <summary>
    /// 生成game.js文件
    /// </summary>
    /// <param name="streamingAssetsUrl"></param>
    /// <param name="url"></param>
    /// <param name="dataFileSize"></param>
    /// <param name="dataFileMd5"></param>
    /// <param name="dataPackage"></param>
    /// <param name="alipayDataFilePath"></param>
    /// <param name="loadFromSubpackage"></param>
    /// <param name="loadingImagePath"></param>
    /// <param name="gameJsFilePath"></param>
    private static void GenerateJsFile(string streamingAssetsUrl, string url, string dataFileSize, string dataFileMd5,
                                string dataPackage, string alipayDataFilePath, string loadFromSubpackage, string loadingImagePath,
                                string gameJsFilePath)
    {
        string zipUrl = string.Empty;
        if (AlipayConfig.AlipayProjectCfg.UseDataZip)
        {
            zipUrl = url + ".zip";
        }
        string gameJsContent = $@"const loaderOptions = {{
        streamingAssetsUrl: '{streamingAssetsUrl}',
        data: {{
            url: '{url}',
            zipUrl: '{zipUrl}',
            size: {dataFileSize},
            md5: '{dataFileMd5}',
            subpackage: '{dataPackage}',
            path: '{alipayDataFilePath}',
            loadFromSubpackage: {loadFromSubpackage},
        }},
        loadingPageConfig: {{
            scaleMode: 'centerCrop', // centerCrop , fitXY , fitCenter 三种 缩放模式（默认全屏）
            designWidth: 0,
            designHeight: 0,
            textConfig: {{
                firstStartText: '首次加载请耐心等待',
                downloadingText: ['正在加载资源'],
                compilingText: '编译中',
                initText: '初始化中',
                completeText: '开始游戏',
                textDuration: 1500,
                style: {{
                    bottom: 100,
                    height: 0,
                    width: 0,
                    lineHeight: 0,
                    color: '#ffffff',
                    fontSize: 14,
                }},
            }},
            barConfig: {{
                style: {{
                    width: 960,
                    height: 40,
                    padding: 0,
                    bottom: 40,
                    backgroundColor: '#07C160',
                    defaultBackgroundColor: '#802b2b2b',
                }},
            }},
            materialConfig: {{
                backgroundImage:  '{loadingImagePath}',
            }},
        }},
    }};
    
    async function main() {{
        const unityInstance = await my.loadUnity(loaderOptions);
    }}
    main().catch((err) => {{
        console.error(err);
    }});";

        File.WriteAllText(gameJsFilePath, gameJsContent);
    }

    private static void GenerateGameJson(Subpackage[] subPackages, string streamingAssetsUrl, string url, int dataFileSize, string dataFileMd5, string dataPackage, string alipayDataFilePath, bool loadFromSubpackage)
    {
        #region  处理game.json
        string orientation = "portrait";
        if (AlipayConfig.AlipayProjectCfg.Orientation != AlipayScreenOrientation.Portrait)
        {
            orientation = "landscape";
        }
        string gameJsonFilePath = Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, alipayGameDir, "game.json");
        string frameworkModulePath = frameworkFileName;
        string wasmName = WasmFileMd5 + "." + wasmFileName + ".zip";

        string zipurl = string.Empty;
        string brurl = string.Empty;
        if (!string.IsNullOrEmpty(url))
        {
            zipurl = AlipayConfig.AlipayProjectCfg.UseDataZip ? url + ".zip" : string.Empty;
            brurl = AlipayConfig.AlipayProjectCfg.UseDataBrotli ? url + ".br" : string.Empty;
        }

        GameJson gameData = new GameJson
        {
            screenOrientation = orientation,
            showStatusBar = false,
            subpackages = subPackages,
            plugins = new Plugins
            {
                UnityLoader = new UnityLoader
                {
                    unityVersion = Application.unityVersion,
                    exporterVersion = buildToolVersion,
                    frameworkModulePath = frameworkModulePath,
                    wasmBinaryMd5 = WasmFileMd5,
                    frameworkWasmBinaryPath = wasmName,
                    streamingAssetsUrl = streamingAssetsUrl,
                    data = new Data
                    {
                        url = url,
                        zipUrl = zipurl,
                        brUrl = brurl,
                        size = dataFileSize,
                        md5 = dataFileMd5,
                        subpackage = dataPackage,
                        path = alipayDataFilePath,
                        loadFromSubpackage = loadFromSubpackage,
                    }
                }
            },
        };
        string jsonData = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(gameJsonFilePath, jsonData);
        #endregion
    }

    public static string GetWebGLDataPath()
    {
#if UNITY_2020_1_OR_NEWER
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.data");
#else
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.data.unityweb");
#endif
    }

    public static string GetWebGLCodePath()
    {
#if UNITY_2020_1_OR_NEWER
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.wasm");
#else
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.wasm.code.unityweb");
#endif
    }

    public static string GetWebGLSymbolPath()
    {
#if UNITY_2020_1_OR_NEWER
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.symbols.json");
#else
        return Path.Combine(AlipayConfig.AlipayProjectCfg.DerivedPath, webglDir, "Build", "webgl.wasm.symbols.unityweb");
#endif
    }

    private static string AudioAdaptation(string input)
    {
        var text = input;
        var audioRules = new[]{
            new NewRule()
            {
                old = @": *_JS_Sound_",
                newStr = ":APGameGlobal.ALIPAYWASMSDK._JS_Sound_"
            },
            new NewRule()
            {
                old = @"unexportedRuntimeFunction\('jsAudio\S*?',\s*(?:false|true)\);\n?",
                newStr = string.Empty
            }
        };

#if UNITY_2022_3_OR_NEWER
        AlipayConfig.AlipayProjectCfg.ReplaceUntiyAudioJS = true;
#endif

        if (AlipayConfig.AlipayProjectCfg.ReplaceUntiyAudioJS)
        {
            for (int i = 0; i < audioRules.Length; i++)
            {
                var rule = audioRules[i];
                text = Regex.Replace(text, rule.old, rule.newStr);
            }

            string[] prefixs =
            {
                "_JS_Sound_",
                "jsAudio",
            };
            foreach (var prefix in prefixs)
            {
                text = RemoveFunctionsWithPrefix(text, prefix);
            }
        }
        return text;
    }

    /// <summary>
    /// 移除输入js代码字符串中所有以prefix为前缀的函数的函数体，function与函数名之间仅允许有一个空格
    /// </summary>
    private static string RemoveFunctionsWithPrefix(string input, string prefix)
    {
        StringBuilder output = new StringBuilder();

        int braceCount = 0;
        int lastIndex = 0;
        int index = input.IndexOf("function " + prefix);

        while (index != -1)
        {
            output.Append(input, lastIndex, index - lastIndex);
            lastIndex = index;

            while (input[lastIndex] != '{')
            {
                lastIndex++;
            }

            braceCount = 1;
            ++lastIndex;

            while (braceCount > 0)
            {
                if (input[lastIndex] == '{')
                {
                    ++braceCount;
                }
                else if (input[lastIndex] == '}')
                {
                    --braceCount;
                }
                ++lastIndex;
            }

            index = input.IndexOf("function " + prefix, lastIndex);
        }

        output.Append(input, lastIndex, input.Length - lastIndex);

        return output.ToString();
    }

    private static void UpdateGraphicAPI()
    {
        if (AlipayConfig.CompileOptions.Webgl2)
        {
            PlayerSettings.SetUseDefaultGraphicsAPIs(BuildTarget.WebGL, false);
            PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES3 });
        }
        // else
        // {
        //     PlayerSettings.SetGraphicsAPIs(BuildTarget.WebGL, new GraphicsDeviceType[] { GraphicsDeviceType.OpenGLES2 });
        // }
    }

    private static void SettingAlipayTextureMinJslib()
    {
        string ds = System.IO.Path.DirectorySeparatorChar.ToString();
        string jsLibRootDir = $"Assets{ds}ALIPAY-SDK{ds}Runtime{ds}Plugins{ds}WebGL{ds}";
        string[] jsLibs = new string[]
        {
            $"{jsLibRootDir}Alipay-TextureMin-JS-WEBGL1.jslib",
            $"{jsLibRootDir}Alipay-TextureMin-JS-WEBGL2.jslib",
            $"{jsLibRootDir}Alipay-TextureMin-JS-WEBGL2-Linear.jslib"
        };
        int index = 0;
        if (AlipayConfig.CompileOptions.Webgl2)
        {
            if (PlayerSettings.colorSpace == ColorSpace.Linear)
            {
                index = 2;
            }
            else
            {
                index = 1;
            }
        }

        for (int i = 0; i < jsLibs.Length; i++)
        {
            Debug.Log($"SetCompatibleWithPlatform :{jsLibs[i]} ");
            var importer = AssetImporter.GetAtPath(jsLibs[i]) as PluginImporter;
            bool value = i == index;
            importer.SetCompatibleWithPlatform(BuildTarget.WebGL, value);
            importer.SaveAndReimport();
        }
    }
}