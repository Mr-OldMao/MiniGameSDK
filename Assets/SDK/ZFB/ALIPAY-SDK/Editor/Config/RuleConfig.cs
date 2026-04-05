using System;
using System.Collections.Generic;

public class RuleConfig
{
  public string header;
  public List<NewRule> rules;
  public RuleConfig()
  {
    rules = new List<NewRule>();
    rules.Add(new NewRule() { old = @"delete\s*http(\b)", newStr = "null" });
    rules.Add(new NewRule() { old = @"new TextDecoder\([""']utf\-16le[""']\)", newStr = @"undefined/* $0 */" });
    rules.Add(new NewRule() { old = @"if\s*\(\!memoryprofiler\)\s*\{", newStr = "if (false) { $0" });
    rules.Add(new NewRule() { old = @"this\.uiUpdateIntervalMsecs\)", newStr = "$0/}" });
    rules.Add(new NewRule() { old = "UnityLoader.SystemInfo", newStr = "Module.SystemInfo" });
    rules.Add(new NewRule() { old = @"http\.open\( *_method *, *_url *, *true *\);", newStr = "var http = new (window.unityNamespace.UnityLoader.UnityCache.XMLHttpRequest || XMLHttpRequest)();http.open(_method, _url, true); http.url = _url;" });
    rules.Add(new NewRule()
    {
      old = @"var UTF8Decoder=typeof TextDecoder!==" + "\"" + "undefined" + "\"" + @"\?new TextDecoder\(" + "\"" + "utf8" + "\"" + @"\):undefined;",
      newStr = "var UTF8Decoder=undefined;"
    });
    rules.Add(new NewRule()
    {
      old = @"FS\.staticInit\(\);",
      newStr = @"FS.staticInit();
window.unityNamespace.FS = FS;
window.unityNamespace.MEMFS = MEMFS;"
    });

#if UNITY_2018 || UNITY_2019
    rules.Add(new NewRule()
    {
        old = @"Module\.streamingAssetsUrl\(\);",
        newStr = "Module.streamingAssetsUrl;"
    });
#endif

#if UNITY_2021_3_OR_NEWER
    //2024.10.22 新增规则
    rules.Add(new NewRule()
    {
      old = @"function _JS_WebRequest_Send\(",
      newStr = @"function MYFetchResponse(xhr, url) {
            var headersPlain = xhr.getAllResponseHeaders();
            var headers = new Map();
            headersPlain
              .trim()
              .split(/[\r\n]+/)
              .forEach(function (e) {
                var t = e.split(': ');
                var n = t.shift();
                var a = t.join(': ');
                headers.set(n, a);
              });
            this.headers = headers;
            this.url = url;
            this.ok = !!(xhr.status >= 200 && xhr.status < 300);
            this.status = xhr.status;
            this.statusText = xhr.statusText;
            this.parsedBody = new Uint8Array(xhr.response);
            this.originXHR = xhr;
        }
        function _JS_WebRequest_Send("
    });
    rules.Add(new NewRule()
    {
      old = @"fetchImpl\(",
      newStr = @"fetchImpl = function (url, init) {
            var xhr = new (window.unityNamespace.UnityLoader.UnityCache.XMLHttpRequest || XMLHttpRequest)();
            xhr.isFetchApi = true;
            xhr.open(init.method, url, true);
            xhr.responseType = 'arraybuffer';
            Object.keys(init.headers).forEach(function (key) {
              xhr.setRequestHeader(key, init.headers[key]);
            });
            xhr.timeout = init.timeout;
            xhr.onprogress = init.onProgress;
            return new Promise(function (resolve, reject) {
              xhr.onload = function () {
                resolve(new MYFetchResponse(xhr, url));
              };
              xhr.onerror = function (e) {
                var response = new MYFetchResponse(xhr, url);
                response.message = e.errorMessage || e.message || '';
                reject(response);
              };
              xhr.send(init.body);
            });
        };
        fetchImpl("
    });
    rules.Add(new NewRule()
    {
      old = @"var kWebRequestOK *= *0;",
      newStr =
        @"HandleProgress({response,loaded: true,lengthComputable: true,total: 100,type: 'progress'});var kWebRequestOK = 0;"
    });
    rules.Add(new NewRule { old = @"enableStreamingDownload: *true", newStr = "enableStreamingDownload: false" });
#endif

    #region 2025.3新增规则 用于处理压缩纹理
    rules.Add(new NewRule()
    {
      old = @"var exts *= *GLctx\.getSupportedExtensions\(\)( *\|\| *\[\])?;",
      newStr = @"var exts = GLctx.getSupportedExtensions() || [];
if (APGameGlobal.USED_TEXTURE_COMPRESSION) {
  exts.push('WEBGL_compressed_texture_etc1');
  exts.push('WEBGL_compressed_texture_etc');
}"
    });

    rules.Add(new NewRule()
    {
      old = @"exts\.forEach\(function\(ext\)",
      newStr = @"APGameGlobal.APSetSupportedExtensions((GLctx.getSupportedExtensions() || [])); exts.forEach(function(ext)"
    });
    #endregion

    #region 2025.3.25新增 APAseetbundle 规则

#if UNITY_2021_3_OR_NEWER

    rules.Add(new NewRule()
    {
      old = @"if *\(body.length *!= *0\)(\s)*{",
      newStr = @"if (APFS.isAPAssetBundle(response.url)) {
        if (body.length == 0 || response.status >= 400) {
          dynCall(""viiiiii"", onresponse, [
            arg,
            response.status,
            _malloc(1),
            1,
            0,
            kWebRequestOK,
          ]);
          return;
        }
        var http = response.originXHR;
        http.onsave = function http_onsave(e) {
          APFS.cache.cleanable(APFS.path2fd.get(e.filePath));
        };
        var arrayBuffer = body;
        let path = APFS.url2path(response.url);
        let numberfd = APFS.path2fd.get(path);
        if (numberfd == undefined) {
          numberfd = APFS.newfd();
          APFS.path2fd.set(path, numberfd);
        }
        let apStream = APFS.fd2apStream.get(numberfd);
        if (apStream == undefined) {
          apStream = {
            node: { mode: 32768, usedBytes: body.length },
            fd: numberfd,
            path: path,
            seekable: true,
            position: 0,
            stream_ops: MEMFS.stream_ops,
            ungotten: [],
            error: false,
          };
          apStream.stream_ops.read = APFS.read;
          APFS.fd2apStream.set(numberfd, apStream);
        }

        APFS.cache.put(numberfd, arrayBuffer, typeof http.isReadFromCache === 'boolean' ? http.isReadFromCache : false);
        dynCall(""viiiiii"", onresponse, [
          arg,
          response.status,
          0,
          0,
          0,
          kWebRequestOK,
        ]);
        APFS.disk.set(unityNamespace.PathInFileOS(path), body.length);}else if(body.length!=0){"
    });

#elif UNTIY_2021
   
    rules.Add(new NewRule()
    {
      old = @"if *\(byteArray.length *!= *0\)(\s)*{",
      newStr = @"if (APFS.isAPAssetBundle(http.url)) {
          if (byteArray.length == 0 || http.status >= 400) {
            dynCall(""viiiiii"", onresponse, [
              arg,
              http.status,
              _malloc(1),
              1,
              0,
              kWebRequestOK,
            ]);
            return;
          }
          http.onsave = function http_onsave(e) {
            APFS.cache.cleanable(APFS.path2fd.get(e.filePath));
          };
          var arrayBuffer = byteArray.buffer;
          let path = APFS.url2path(http.url);
          let numberfd = APFS.path2fd.get(path);
          if (numberfd == undefined) {
            numberfd = APFS.newfd();
            APFS.path2fd.set(path, numberfd);
          }
          let apStream = APFS.fd2apStream.get(numberfd);
          if (apStream == undefined) {
            apStream = {
              node: {
                mode: 32768,
                usedBytes: byteArray.length
              },
              fd: numberfd,
              path: path,
              seekable: true,
              position: 0,
              stream_ops: MEMFS.stream_ops,
              ungotten: [],
              error: false,
            };
            apStream.stream_ops.read = APFS.read;
            APFS.fd2apStream.set(numberfd, apStream);
          }

          APFS.cache.put(numberfd, arrayBuffer, typeof http.isReadFromCache === 'boolean' ? http.isReadFromCache : false);
          dynCall(""viiiiii"", onresponse, [
            arg,
            http.status,
            0,
            0,
            0,
            kWebRequestOK,
          ]);
          APFS.disk.set(unityNamespace.PathInFileOS(path), byteArray.length)
        } else if (byteArray.length != 0){"
    });

#endif

#if UNITY_2021_1_OR_NEWER
    rules.Add(new NewRule()
    {
      old = @"ret *= *UTF8ToString\(ptr\);",
      newStr = @"ret = UTF8ToString(ptr);
      if (APFS.isAPAssetBundle(ret)) {
        return APFS.url2path(ret);
      }"
    });

    rules.Add(new NewRule()
    {
      old = @"getStreamFromFD: *function\(fd\)(\s)*{",
      newStr = @"getStreamFromFD: function (fd) {
      if (fd > FS.MAX_OPEN_FDS) {
        var stream = APFS.fd2apStream.get(fd);
        if (stream == undefined) throw new FS.ErrnoError(8);
        return stream;
      }"
    });

    rules.Add(new NewRule()
    {
      old = @"return SYSCALLS.doAccess\(path, *amode\)",
      newStr = @"if (typeof APFS !== ""undefined"" && APFS.isAPAssetBundle(path)) {
        if (APFS.path2fd.has(path)) return 0;
        return APFS.doAPAccess(path, amode)
      }
      return SYSCALLS.doAccess(path, amode)"
    });

    rules.Add(new NewRule()
    {
      old = @"return SYSCALLS.doStat\(FS.stat, *stream.path, *buf\)",
      newStr = @"if (fd > FS.MAX_OPEN_FDS) {
        return SYSCALLS.doStat(APFS.apstat, stream.path, buf);
      }
      return SYSCALLS.doStat(FS.stat, stream.path, buf)"
    });

    rules.Add(new NewRule()
    {
      old = @"return SYSCALLS.doStat\(FS.stat, *path, *buf\)",
      newStr = @"if (APFS.isAPAssetBundle(path)) {
        return SYSCALLS.doStat(APFS.apstat, path, buf);
      }
      return SYSCALLS.doStat(FS.stat, path, buf)"
    });

    rules.Add(new NewRule()
    {
      old = @"var pathname *= *SYSCALLS\.getStr\(path\);",
      newStr = @"var pathname = SYSCALLS.getStr(path);
if (typeof APFS !== ""undefined"" && APFS.isAPAssetBundle(pathname)) {
    var numberfd = APFS.path2fd.get(pathname);
    if (numberfd !== undefined) {
        return numberfd;
    }
    const res = APFS.LoadBundleFromFile(pathname);
    numberfd = APFS.newfd();
    let apStream = {
        fd: numberfd,
        path: pathname,
        flags: flags,
        seekable: true,
        position: 0,
        stream_ops: MEMFS.stream_ops,
        ungotten: [],
        node: {
            mode: 32768,
            usedBytes: new Uint8Array(res).length
        },
        error: false,
    };
    apStream.stream_ops.read = APFS.read;
    APFS.path2fd.set(pathname, numberfd);
    APFS.fd2apStream.set(numberfd, apStream);
    APFS.cache.put(numberfd, res);
    return numberfd;
}"
    });

    rules.Add(new NewRule()
    {
      old = @"_fd_close\(fd\)(\s)*{",
      newStr = @"_fd_close(fd) {
      if (fd > FS.MAX_OPEN_FDS) {
        return 0;
      }",
    });

    rules.Add(new NewRule()
    {
      old = @"TTY.init",
      newStr = @"if (typeof _APFSInit !== ""undefined"") _APFSInit(unityNamespace.ttlAssetBundle ? unityNamespace.ttlAssetBundle : 5);TTY.init"
    });
#endif
    #endregion

    #region 2025.4.18新增适配
#if UNITY_2021_3_OR_NEWER
    rules.Add(new NewRule()
    {
      old = @"return\s+MEMFS\.mount\.apply\(null,\s*arguments\)",
      newStr = "return (Module.__unityIdbfsMount = MEMFS.mount.apply(null, arguments))"
    });
#endif
    #endregion

    #region 2025.4.25 新增适配 / 2025.5.23 新增适配 PlayerPrefs - 团结 - Unity2022编辑器
#if UNITY_2022_1_OR_NEWER
    rules.Add(new NewRule()
    {
      old = @"_JS_FileSystem_Sync\(\)(\s)*{",
      newStr = @"_JS_FileSystem_Sync() {
if (typeof indexedDB === 'undefined') {
  return;
}
FS.syncfs(false, () => {});
return;"
    });
#else
    rules.Add(new NewRule()
    {
      old = @"_JS_FileSystem_Sync\(\)(\s)*{",
      newStr = @"_JS_FileSystem_Sync() {
if (typeof indexedDB === 'undefined') {
  return;
}
fs.sync(false);
return;"
    });
#endif
    rules.Add(new NewRule()
    {
      old = @"FileSystem_Initialize\(\)(\s)*{",
      newStr = @"FileSystem_Initialize() {
if (typeof indexedDB === 'undefined') return;"
    });
    #endregion

    #region 2025.5.23新增 Unity2022编辑器 APAssetBundle适配
#if UNITY_2022
    rules.Add(new NewRule()
    {
      old = @"if *\(path\[0\]",
      newStr = @"if (APFS.isAPAssetBundle(path) || path[0]"
    });
    rules.Add(new NewRule()
    {
      old = @"var mode *= *varargs",
      newStr = @"if (typeof APFS !== ""undefined"" && APFS.isAPAssetBundle(path)) {   
        var numberfd = APFS.path2fd.get(path);
        if (numberfd !== undefined) {
            return numberfd;
        }
        const res = APFS.LoadBundleFromFile(path);
        numberfd = APFS.newfd();
        let apStream = {
            fd: numberfd,
            path: path,
            flags: flags,
            seekable: true,
            position: 0,
            stream_ops: MEMFS.stream_ops,
            ungotten: [],
            node: {
                mode: 32768,
                usedBytes: new Uint8Array(res).length
            },
            error: false,
        };
        apStream.stream_ops.read = APFS.read;
        APFS.path2fd.set(path, numberfd);
        APFS.fd2apStream.set(numberfd, apStream);
        APFS.cache.put(numberfd, res);
        return numberfd;
     }
     var mode = varargs"
    });
#endif
    #endregion

    #region 2025.6.26 内存监控

    //监控启动内存
    rules.Add(new NewRule()
    {
      old = @"removeRunDependency\(([""'])wasm-instantiate([""'])\)",
      newStr = @"
 removeRunDependency(""wasm-instantiate"")
  try {
    var params = {
      type: ""heap_init"",
      size: HEAP8.length
    };
    UnityJsBridge.handleMsgFromUnity(""sendMonitor"", JSON.stringify({
      type: 0,
      params: params,
      __invokeType__: ""async"",
      __callBackId__: ""-1""
    }));
  } catch (error) {
  }"
    });

    //监控分配异常
    rules.Add(new NewRule()
    {
      old = @"updateGlobalBufferAndViews\(wasmMemory\.buffer\);return 1\}catch\(e\)\{\}",
      newStr = @"
updateGlobalBufferAndViews(wasmMemory.buffer);
return 1;
} catch (e) {
  try {
    var params = {
      type: ""heap_error"",
      stage: ""WasmMemoryGrowException"",
      error: !!e ? e.toString() : ""empty"",
      oldSize: buffer.byteLength,
      newSize: size,
    };
    UnityJsBridge.handleMsgFromUnity(""sendMonitor"", JSON.stringify({
      type: 2,
      params: params,   
      __invokeType__: ""async"",
      __callBackId__: ""-1""     
    }));
  } catch (error) {
  }
}"
    });

    //监控每次堆扩容
    rules.Add(new NewRule()
    {
      old = @"var replacement *= *emscripten_realloc_buffer",
      newStr = @"
  try {
      var params = {
        type: ""heap_resize"",
        stage: ""TryToRealloc"",
        oldSize: oldSize,
        requestedSize: requestedSize,
        newSize: newSize,
      };
      UnityJsBridge.handleMsgFromUnity(""sendMonitor"", JSON.stringify({
        type: 0,
        params: params,   
        __invokeType__: ""async"",
        __callBackId__: ""-1""     
      }));
    } catch (error) {
    }
var replacement = emscripten_realloc_buffer"
    });

    #endregion

    #region 2025.7.10新增适配---鼠标锁定
    rules.Add(new NewRule()
    {
      old = @"function\s+requestPointerLock\s*\(\s*target\s*\)\s*\{",
      newStr = "function requestPointerLock(target) { return 0; "
    });
    #endregion

    #region 2025.8.1新增适配---屏幕方向锁定
    rules.Add(new NewRule()
    {
      old = @"_JS_ScreenOrientation_Lock\s*\(\s*orientationLockType\s*\)\s*\{",
      newStr = "_JS_ScreenOrientation_Lock(orientationLockType) { return;"
    });
    #endregion

    #region 2025.8.6新增适配---Module
    rules.Add(new NewRule()
    {
      old = @"function\s+Pointer_stringify",
      newStr = "window.unityNamespace.Module = Module; function Pointer_stringify"
    });
    #endregion

    #region 2025.10.22新增---webgl2 Texture

    rules.Add(new NewRule()
    {
      old = @"function _glTexStorage2D\(x0, *x1, *x2, *x3, *x4\)(\s)*{",
      newStr = "function _glTexStorage2D(x0, x1, x2, x3, x4) {window._lastTexStorage2DParams = [x0, x1, x2, x3, x4];if(x2 == 36196 || x2 == 37492 || x2 == 37493){return;}"
    });

    #endregion
  }
}

[Serializable]
public class NewRule
{
  public string old;
  public string newStr;
}


