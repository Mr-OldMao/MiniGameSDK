var APAssetBundleLibrary = {
  $APFS: undefined,

  APCheckReady: function () {
    return APFS.fs !== undefined;
  },

  APRegisterAssetBundleUrl: function (ptr) {
    var url = UTF8ToString(ptr);
    var path = APFS.url2path(url);
    if (!APFS.disk.has(path)) {
      APFS.disk.set(path, 0);
    }
  },

  APUnregisterAssetBundleUrl: function (ptr) {
    var url = UTF8ToString(ptr);
    var path = APFS.url2path(url);
    var fd = APFS.path2fd.get(path);
    if (APFS.cache.has(fd)) {
      APFS.cache.delete(fd);
    }
    if (APFS.disk.has(path)) {
      APFS.disk.delete(path);
    }
  },

  AlipayAbfsFetchBundleFromXHR: function (url, id, callback, needRetry) {
    if (!APGameGlobal.ALIPAYWASMSDK) return;

    var _url = UTF8ToString(url);
    var _id = UTF8ToString(id);
    var _callback = function (code, message) {
      var len = lengthBytesUTF8(_id) + 1;
      var idPtr = _malloc(len);
      try {
        stringToUTF8(_id, idPtr, len);
        dynCall('viii', callback, [idPtr, code, 0]);
      } finally {
        _free(idPtr);
      }
    };
    APGameGlobal.ALIPAYWASMSDK.APFS.fetchBundleFromXHR(
      _url,
      _id,
      _callback,
      needRetry
    );
  },

  APFSInit: function (ttl, capacity) {
    if (APFS !== undefined) {
      return;
    }

    if (Module['APFSInit']) {
      APFS = Module['APFSInit'](ttl, capacity);
      return;
    }
    APFS = {};

    var APMap = (function () {
      function APMap(hash, rename) {
        this.hash = hash;
        this.rename = rename;
        this.size = 0;
      }

      APMap.prototype.get = function (key) {
        return this.hash.get(this.rename(key));
      };

      APMap.prototype.set = function (key, value) {
        this.delete(key);
        this.size += value;
        return this.hash.set(this.rename(key), value);
      };

      APMap.prototype.has = function (key) {
        return this.hash.has(this.rename(key));
      };

      APMap.prototype.delete = function (key) {
        this.size -= this.hash.get(this.rename(key)) || 0;
        return this.hash.delete(this.rename(key));
      };

      return APMap;
    })();

    APFS.APABErrorSteps = {
      kWebRequestResponse: 0,
      kLoadBundleFromFile: 1,
      kCacheGet: 2,
    };

    APFS.disk = new APMap(
      unityNamespace.APAssetBundles,
      unityNamespace.PathInFileOS
    );

    APFS.msg = '';

    APFS._url2path = new Map();

    APFS.path2fd = new Map();

    APFS.fd2apStream = new Map();
    APFS.fs = my.getFileSystemManager();
    APFS.nowfd = FS.MAX_OPEN_FDS + 1;

    APFS.url2path = function (url) {
      if (APFS._url2path.has(url)) {
        return APFS._url2path.get(url);
      }
      var path;
      if (url.startsWith('/vfs_streamingassets/')) {
        path = url.replace(
          '/vfs_streamingassets/',
          my.env.USER_DATA_PATH + '/__GAME_FILE_CACHE/StreamingAssets/'
        );
      } else {
        let fileCachePath = my.env.USER_DATA_PATH + '/__GAME_FILE_CACHE';
        if (unityNamespace.DATA_CDN.endsWith('/')) {
          fileCachePath += '/';
        }
        path = url.replace(unityNamespace.DATA_CDN, fileCachePath);
      }
      if (path.indexOf('?') !== -1) {
        path = path.substring(0, path.indexOf('?'));
      }
      APFS._url2path.set(url, path);
      return path;
    };

    APFS.isAPAssetBundle = function (url) {
      if (
        url.startsWith(unityNamespace.DATA_CDN) ||
        url.startsWith('/vfs_streamingassets')
      ) {
        return unityNamespace.isAPAssetBundle(APFS.url2path(url));
      }
      return unityNamespace.isAPAssetBundle(url);
    };

    APFS.newfd = function () {
      return APFS.nowfd++;
    };

    var APFileCache = (function () {
      function APFileCache(ttl, capacity) {
        this.ttl = ttl;
        if (capacity > 0) this.capacity = capacity;
        this.hash = new Map();
        this.reCheckedPath = new Set();
        this.size = 0;
        this.maxSize = 0;
        this.obsolete = '';

        this.totalSizeLoaded = 0;
        this.totalCountLoaded = 0;
        this.currentSize = 0;
        this.currentCount = 0;
        this.totalSizeCleaned = 0;
        this.totalCountCleaned = 0;
        this.reloadSize = 0;
        this.reloadCount = 0;
      }

      APFileCache.prototype.record = function (path) {
        if (this.obsolete.indexOf(path) === -1) {
          if (this.obsolete !== '') this.obsolete += ';';
          this.obsolete += path;
        }
      };

      APFileCache.prototype.get = function (key) {
        var temp = this.hash.get(key);
        if (temp !== undefined) {
          this.hash.delete(key);
          temp.time = Date.now();
          this.hash.set(key, temp);
          return temp.ab;
        }
        return -1;
      };

      APFileCache.prototype.put = function (key, ab, cleanable) {
        if (cleanable === undefined) cleanable = true;
        if (!ab) return;
        var startTime = Date.now();
        var value = {
          ab: ab,
          time: startTime,
          cleanable: cleanable,
        };
        var temp = this.hash.get(key);
        if (temp !== undefined) {
          this.size -= temp.ab.byteLength;
          this.hash.delete(key);
          this.reloadCount++;
          this.reloadSize += ab.byteLength;
        } else {
          this.totalCountLoaded++;
          this.totalSizeLoaded += ab.byteLength;
        }
        if (this.capacity !== undefined && this.size >= this.capacity) {
          var idx = this.hash.keys().next().value;
          this.size -= this.hash.get(idx).ab.byteLength;
          this.hash.delete(idx);
        }
        this.hash.set(key, value);
        this.size += value.ab.byteLength;
        this.maxSize = Math.max(this.size, this.maxSize);
        this.currentSize = this.size;
        this.currentCount = this.hash.size;
      };

      APFileCache.prototype.cleanable = function (key, _cleanable) {
        if (_cleanable === undefined) _cleanable = true;
        var temp = this.hash.get(key);
        if (temp !== undefined) {
          temp.cleanable = _cleanable;
          this.hash.set(key, temp);
          return 0;
        }
        return -1;
      };

      APFileCache.prototype.cleanByTime = function (deadline) {
        var entries = this.hash.entries();
        var entry;
        while ((entry = entries.next().value)) {
          var key = entry[0];
          var value = entry[1];

          if (value.time < deadline && !value.cleanable) {
            var path = key;
            if (APFS.fd2apStream.has(path)) {
              path = APFS.fd2apStream.get(key).path;
            }
            if (!this.reCheckedPath.has(path)) {
              var self = this;
              (function (key, value, path) {
                APFS.fs.access({
                  path: path,
                  success: function (res) {
                    self.DoClean(key, value);
                  },
                  fail: function (err) {
                    console.error('[APAssetBundle] access error: ', err);
                    self.reCheckedPath.add(path);
                  },
                });
              })(key, value, path);
            }
          }

          if (value.time < deadline && value.cleanable) {
            this.DoClean(key, value);
          }
        }
      };

      APFileCache.prototype.DoClean = function (key, value) {
        this.size -= value.ab.byteLength;
        this.hash.delete(key);

        this.totalCountCleaned++;
        this.totalSizeCleaned += value.ab.byteLength;

        var path = key;
        if (APFS.fd2apStream.has(path)) {
          path = APFS.fd2apStream.get(key).path;
        }
        console.log(
          '[APAssetBundle] cache clean path: ' +
            path +
            ' size: ' +
            value.ab.byteLength +
            ' currentCount:' +
            this.hash.size +
            ' currentSize:' +
            this.size
        );
      };

      APFileCache.prototype.logCurrentStatistic = function () {
        const statistics = {
          totalLoaded: {
            count: this.totalCountLoaded,
            size: this.totalSizeLoaded,
          },
          current: {
            count: this.currentCount,
            size: this.currentSize,
          },
          totalCleaned: {
            count: this.totalCountCleaned,
            size: this.totalSizeCleaned,
          },
          reloaded: {
            count: this.reloadCount,
            size: this.reloadSize,
          },
        };

        console.log('[APAssetBundle] Current Statistics:', statistics);
      };

      APFileCache.prototype.RegularCleaning = function (kIntervalSecond) {
        var self = this;
        setInterval(function () {
          self.cleanByTime(Date.now() - self.ttl * 1000);
        }, kIntervalSecond * 1000);
      };

      APFileCache.prototype.delete = function (key) {
        this.size -= this.hash.get(key).ab.byteLength;
        return this.hash.delete(key);
      };

      APFileCache.prototype.has = function (key) {
        return this.hash.has(key);
      };

      return APFileCache;
    })();

    APFS.cache = new APFileCache(ttl, capacity);

    if (true) {
      APFS.cache.RegularCleaning(1);
    }

    APFS.LoadBundleFromFile = function (path) {
      var res = APFS.fs.readFileSync(path);

      if (res.success) {
        var expected_size = APFS.disk.get(path);
        if (expected_size === 0) {
          APFS.disk.set(path, res.data.byteLength);
          expected_size = res.data.byteLength;
        }
        if (res.data.byteLength == expected_size) {
          console.log(
            '[APAssetBundle] LoadBundleFromFile success path: ' +
              path +
              ' size: ' +
              res.data.byteLength
          );
          return res.data;
        }
      }

      var apab_error = {
        stage: APFS.APABErrorSteps.kLoadBundleFromFile,
        path: path,
        size: res ? res.data.byteLength : 0,
        expected_size: expected_size,
        error: res ? res.errorMessage : '',
      };
      UnityJsBridge.handleMsgFromUnity(
        'localLog',
        JSON.stringify({
          level: 'error',
          message:
            '[APAssetBundle] LoadBundleFromFile fail:' +
            JSON.stringify(apab_error),
          __invokeType__: 'async',
          __callBackId__: '-1',
        })
      );
      return '';
    };

    APFS.doAPAccess = function (path, amode) {
      if (amode & ~7) {
        return -28;
      }
      try {
        APFS.fs.accessSync(path);
      } catch (e) {
        return -44;
      }
      return 0;
    };

    APFS.apstat = function (path) {
      try {
        var fd = APFS.path2fd.get(path);
        var stat;
        if (fd !== undefined) {
          stat = {
            mode: 33206,
            size: APFS.cache.get(fd).byteLength,
            dev: 1,
            ino: 1,
            nlink: 1,
            uid: 0,
            gid: 0,
            rdev: 0,
            atime: new Date(),
            mtime: new Date(0),
            ctime: new Date(),
            blksize: 4096,
          };
          stat.blocks = Math.ceil(stat.size / stat.blksize);
          return stat;
        }
        stat = APFS.fs.statSync(path);
        stat.dev = 1;
        stat.ino = 1;
        stat.nlink = 1;
        stat.uid = 0;
        stat.gid = 0;
        stat.rdev = 0;
        stat.atime = new Date(stat.lastAccessedTime * 1000);
        stat.mtime = new Date(0);
        stat.ctime = new Date(stat.lastModifiedTime * 1000);
        delete stat.lastAccessedTime;
        delete stat.lastModifiedTime;
        stat.blksize = 4096;
        stat.blocks = Math.ceil(stat.size / stat.blksize);
        return stat;
      } catch (e) {
        console.error(e);
        throw e;
      }
    };

    APFS.read = function (stream, buffer, offset, length, position) {
      var contents = APFS.cache.get(stream.fd);
      if (contents === -1) {
        console.log('[APAssetBundle] content is null, start to load from file');
        var res = APFS.LoadBundleFromFile(stream.path);
        APFS.cache.put(stream.fd, res, res === '' ? false : true);
        contents = res;
      }
      if (position >= stream.node.usedBytes) {
        return 0;
      }

      var size = Math.min(stream.node.usedBytes - position, length);
      if (size >= 0) {
        buffer.set(
          new Uint8Array(contents.slice(position, position + size)),
          offset
        );
      }

      return size;
    };
  },
};

autoAddDeps(APAssetBundleLibrary, '$APFS');
mergeInto(LibraryManager.library, APAssetBundleLibrary);
