using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameJson
{
    public string screenOrientation;
    public bool showStatusBar;
    public Plugins plugins;
    public Subpackage[] subpackages;
}

[Serializable]
public class Plugins
{
    public UnityLoader UnityLoader;
}

[Serializable]
public class UnityLoader
{
    public string unityVersion;
    public string exporterVersion;
    public string frameworkModulePath;
    public string frameworkWasmBinaryPath;
    public string wasmBinaryMd5;
    public string streamingAssetsUrl;
    public Data data;
}

[Serializable]
public class Data
{
    public string url;
    public string zipUrl;
    public string brUrl;
    public int size;
    public string md5;
    public string subpackage;
    public string path;
    public bool loadFromSubpackage;
}

[Serializable]
public class Subpackage
{
    public string name;
    public string root;
}
