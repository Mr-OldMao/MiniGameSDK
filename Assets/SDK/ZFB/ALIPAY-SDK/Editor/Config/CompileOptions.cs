using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CompileOptions 
{
	/// <summary>
	/// Development Build
	/// </summary>
	public bool DevelopBuild = false;

	/// <summary>
	/// Autoconnect Profiler
	/// </summary>
	public bool AutoProfile = false;

	/// <summary>
	/// Scripts Only Build
	/// </summary>
	public bool ScriptOnly = false;

	/// <summary>
	///  Il2CppCodeGeneration.OptimizeSize
	/// </summary>
	public bool Il2CppOptimizeSize = true;

	/// <summary>
	/// Profiling Funcs
	/// </summary>
	public bool profilingFuncs = true;

	/// <summary>
	/// WebGL2.0
	/// </summary>
	public bool Webgl2 = false;
 
	/// <summary>
	/// UseStreamingAssets
	/// </summary>
	public bool UseStreamingAssets = true;

	/// <summary>
	/// ProfilingMemory
	/// </summary>
	public bool ProfilingMemory = false;

	/// <summary>
	/// CleanBuild
	/// </summary>
	public bool CleanBuild = false;

}
