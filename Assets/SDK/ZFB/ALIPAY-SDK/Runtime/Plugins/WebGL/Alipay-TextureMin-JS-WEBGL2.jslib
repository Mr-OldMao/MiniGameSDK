mergeInto(LibraryManager.library, {
  glCompressedTexImage2D: function (
    target,
    level,
    internalFormat,
    width,
    height,
    border,
    imageSize,
    data
  ) {
    var lastTid = window._lastTextureId;
    function getMatchId() {
      var webgl2c = internalFormat == 37492;
      if (webgl2c && APGameGlobal.USED_TEXTURE_COMPRESSION) {
        var length = HEAPU8.subarray(data, data + 1)[0];
        var d = HEAPU8.subarray(data + 1, data + 1 + length);
        var res = [];
        d.forEach(function (v) {
          res.push(String.fromCharCode(v));
        });
        var matchId = res.join('');
        var start1 = res.length - 5;
        if (res[start1] == '_') {
          start1++;
          var size = res[start1++];
          if (size != '4' && size != '5' && size != '6' && size != '8') {
            return [matchId, '8x8'];
          }
          var astcBlockSize = size + 'x' + size;
          start1 -= 2;
          return [matchId.substr(0, start1), astcBlockSize];
        } else {
          return [matchId, '8x8'];
        }
      }
      return [-1, '8x8'];
    }
    var matchIdInfo = getMatchId();
    var matchId = matchIdInfo[0];
    var astcBlockSize = matchIdInfo[1];

    function compressedImage2D(rawData) {
      var format = 0;
      var dataOffset = 16;
      var compressFormat = APGameGlobal.TextureCompressedFormat;
      switch (compressFormat) {
        case 'astc':
          var astcList = GLctx.getExtension('WEBGL_compressed_texture_astc');
          if (astcBlockSize == '4x4') {
            format = astcList.COMPRESSED_RGBA_ASTC_4x4_KHR;
            break;
          }
          if (astcBlockSize == '5x5') {
            format = astcList.COMPRESSED_RGBA_ASTC_5x5_KHR;
            break;
          }
          if (astcBlockSize == '6x6') {
            format = 37812;
            break;
          }
          format = astcList.COMPRESSED_RGBA_ASTC_8x8_KHR;
          break;
        case 'etc2':
          format = GLctx.getExtension(
            'WEBGL_compressed_texture_etc'
          ).COMPRESSED_RGBA8_ETC2_EAC;
          break;
        case 'dds':
          format = GLctx.getExtension(
            'WEBGL_compressed_texture_s3tc'
          ).COMPRESSED_RGBA_S3TC_DXT5_EXT;
          dataOffset = 128;
          break;
      }

      GLctx['compressedTexImage2D'](
        target,
        level,
        format,
        width,
        height,
        border,
        new Uint8Array(rawData, dataOffset)
      );
    }

    function texImage2D(image) {
      GLctx.texImage2D(
        GLctx.TEXTURE_2D,
        0,
        GLctx.RGBA,
        GLctx.RGBA,
        GLctx.UNSIGNED_BYTE,
        image
      );
    }

    function renderTexture(id) {
      if (!GL.textures[lastTid]) {
        UnityJsBridge.handleMsgFromUnity(
          'localLog',
          JSON.stringify({
            level: 'error',
            message: '[Texture] renderTexture not find lastTid',
            __invokeType__: 'async',
            __callBackId__: '-1',
          })
        );
        return;
      }
      var _data = APGameGlobal.DownloadedTextures[id].data;
      var tid = lastTid;
      GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[tid]);

      var textureFormat = APGameGlobal.TextureCompressedFormat;
      if (
        !textureFormat ||
        (textureFormat === 'dds' && (width % 4 !== 0 || height % 4 !== 0))
      ) {
        texImage2D(_data);
      } else {
        compressedImage2D(_data);
      }

      GLctx.bindTexture(
        GLctx.TEXTURE_2D,
        window._lastBoundTexture ? GL.textures[window._lastBoundTexture] : null
      );
    }

    function renderTransparent() {
      GLctx.texImage2D(
        GLctx.TEXTURE_2D,
        0,
        GLctx.RGBA,
        1,
        1,
        0,
        GLctx.RGBA,
        GLctx.UNSIGNED_SHORT_4_4_4_4,
        new Uint16Array([0, 0])
      );
    }

    if (matchId != -1) {
      renderTransparent();
      APGameGlobal.APDownloadTexture(matchId, width, height, () => {
        renderTexture(matchId);
      });
      return;
    }

    if (GL.currentContext.version >= 2) {
      if (GLctx.currentPixelUnpackBufferBinding) {
        GLctx['compressedTexImage2D'](
          target,
          level,
          internalFormat,
          width,
          height,
          border,
          imageSize,
          data
        );
      } else {
        GLctx['compressedTexImage2D'](
          target,
          level,
          internalFormat,
          width,
          height,
          border,
          HEAPU8,
          data,
          imageSize
        );
      }
      return;
    }
    GLctx['compressedTexImage2D'](
      target,
      level,
      internalFormat,
      width,
      height,
      border,
      data ? HEAPU8.subarray(data, data + imageSize) : null
    );
  },
  glCompressedTexSubImage2D: function (
    target,
    level,
    xoffset,
    yoffset,
    width,
    height,
    format,
    imageSize,
    data
  ) {
    var lastTid = window._lastTextureId;
    function getMatchId() {
      var webgl2c = format == 37492;
      if (webgl2c && APGameGlobal.USED_TEXTURE_COMPRESSION) {
        var length = HEAPU8.subarray(data, data + 1)[0];
        var d = HEAPU8.subarray(data + 1, data + 1 + length);
        var res = [];
        d.forEach(function (v) {
          res.push(String.fromCharCode(v));
        });
        var matchId = res.join('');
        var start1 = res.length - 5;
        if (res[start1] == '_') {
          start1++;
          var size = res[start1++];
          if (size != '4' && size != '5' && size != '6' && size != '8') {
            return [matchId, '8x8'];
          }
          var astcBlockSize = size + 'x' + size;
          start1 -= 2;
          return [matchId.substr(0, start1), astcBlockSize];
        } else {
          return [matchId, '8x8'];
        }
      }
      return [-1, '8x8'];
    }
    var matchIdInfo = getMatchId();
    var matchId = matchIdInfo[0];
    var astcBlockSize = matchIdInfo[1];

    function compressedImage2D(rawData) {
      var format = 0;
      var dataOffset = 16;
      var compressFormat = APGameGlobal.TextureCompressedFormat;
      switch (compressFormat) {
        case 'astc':
          var astcList = GLctx.getExtension('WEBGL_compressed_texture_astc');
          if (astcBlockSize == '4x4') {
            format = astcList.COMPRESSED_RGBA_ASTC_4x4_KHR;
            break;
          }
          if (astcBlockSize == '5x5') {
            format = astcList.COMPRESSED_RGBA_ASTC_5x5_KHR;
            break;
          }
          if (astcBlockSize == '6x6') {
            format = 37812;
            break;
          }
          format = astcList.COMPRESSED_RGBA_ASTC_8x8_KHR;
          break;
        case 'etc2':
          format = GLctx.getExtension(
            'WEBGL_compressed_texture_etc'
          ).COMPRESSED_RGBA8_ETC2_EAC;
          break;
        case 'dds':
          format = GLctx.getExtension(
            'WEBGL_compressed_texture_s3tc'
          ).COMPRESSED_RGBA_S3TC_DXT5_EXT;
          dataOffset = 128;
          break;
      }
      GLctx['compressedTexSubImage2D'](
        target,
        level,
        xoffset,
        yoffset,
        width,
        height,
        format,
        new Uint8Array(rawData, dataOffset)
      );
    }

    function texImage2D(image) {
      GLctx.texSubImage2D(
        target,
        level,
        xoffset,
        yoffset,
        width,
        height,
        GLctx.RGBA,
        GLctx.UNSIGNED_BYTE,
        image
      );
    }
    function renderTexture(id) {
      if (!GL.textures[lastTid]) {
        UnityJsBridge.handleMsgFromUnity(
          'localLog',
          JSON.stringify({
            level: 'error',
            message: '[Texture] renderTexture not find lastTid',
            __invokeType__: 'async',
            __callBackId__: '-1',
          })
        );
        return;
      }
      var _data = APGameGlobal.DownloadedTextures[id].data;
      var tid = lastTid;

      GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[tid]);

      var textureFormat = APGameGlobal.TextureCompressedFormat;
      if (
        !textureFormat ||
        (textureFormat === 'dds' && (width % 4 !== 0 || height % 4 !== 0))
      ) {
        texImage2D(_data);
      } else {
        compressedImage2D(_data);
      }
      GLctx.bindTexture(
        GLctx.TEXTURE_2D,
        window._lastBoundTexture ? GL.textures[window._lastBoundTexture] : null
      );
    }
    var p = window._lastTexStorage2DParams;
    if (matchId != -1) {
      var f = GLctx.RGBA8;
      var compressFormat = APGameGlobal.TextureCompressedFormat;
      switch (compressFormat) {
        case 'astc':
          var astcList = GLctx.getExtension('WEBGL_compressed_texture_astc');
          if (astcBlockSize == '4x4') {
            f = astcList.COMPRESSED_RGBA_ASTC_4x4_KHR;
            break;
          }
          if (astcBlockSize == '5x5') {
            f = astcList.COMPRESSED_RGBA_ASTC_5x5_KHR;
            break;
          }
          if (astcBlockSize == '6x6') {
            f = 37812;
            break;
          }
          f = astcList.COMPRESSED_RGBA_ASTC_8x8_KHR;
          break;
        case 'etc2':
          f = GLctx.getExtension(
            'WEBGL_compressed_texture_etc'
          ).COMPRESSED_RGBA8_ETC2_EAC;
          break;
        case 'dds':
          f = GLctx.getExtension(
            'WEBGL_compressed_texture_s3tc'
          ).COMPRESSED_RGBA_S3TC_DXT5_EXT;
          break;
      }
      GLctx['texStorage2D'](p[0], p[1], f, width, height);

      APGameGlobal.APDownloadTexture(matchId, width, height, function () {
        renderTexture(matchId);
      });
      return;
    }

    if (GL.currentContext.version >= 2) {
      if (GLctx.currentPixelUnpackBufferBinding) {
        GLctx['compressedTexSubImage2D'](
          target,
          level,
          xoffset,
          yoffset,
          width,
          height,
          format,
          imageSize,
          data
        );
      } else {
        GLctx['compressedTexSubImage2D'](
          target,
          level,
          xoffset,
          yoffset,
          width,
          height,
          format,
          HEAPU8,
          data,
          imageSize
        );
      }
      return;
    }
    GLctx['compressedTexSubImage2D'](
      target,
      level,
      xoffset,
      yoffset,
      width,
      height,
      format,
      data ? HEAPU8.subarray(data, data + imageSize) : null
    );
  },

});
