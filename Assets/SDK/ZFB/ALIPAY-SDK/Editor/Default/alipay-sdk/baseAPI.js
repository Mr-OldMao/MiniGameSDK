import APGameGlobal from './global';
const AlipayBaseAPI = {
  GetUserAvatarData: function (url, successCallback, errorCallback) {
    let image = my.createImage();
    image.src = url;

    image.onload = function () {
      try {
        const canvas = my.createOffscreenCanvas();
        canvas.width = image.width;
        canvas.height = image.height;
        const gl = canvas.getContext('webgl');
        const shaderProgram = initShaderProgram(gl, vsSource, fsSource);
        if (!shaderProgram) {
          throw new Error('Failed to initialize shader program');
        }
        gl.useProgram(shaderProgram);
        const positions = new Float32Array([
          -1.0, -1.0, 1.0, -1.0, -1.0, 1.0, 1.0, 1.0,
        ]);

        const positionBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, positions, gl.STATIC_DRAW);

        const texCoords = new Float32Array([
          0.0, 1.0, 1.0, 1.0, 0.0, 0.0, 1.0, 0.0,
        ]);

        const texCoordBuffer = gl.createBuffer();
        gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
        gl.bufferData(gl.ARRAY_BUFFER, texCoords, gl.STATIC_DRAW);

        const texture = gl.createTexture();
        gl.bindTexture(gl.TEXTURE_2D, texture);
        gl.texImage2D(
          gl.TEXTURE_2D,
          0,
          gl.RGBA,
          gl.RGBA,
          gl.UNSIGNED_BYTE,
          image
        );
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.LINEAR);
        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.LINEAR);

        gl.viewport(0, 0, canvas.width, canvas.height);

        const positionLocation = gl.getAttribLocation(
          shaderProgram,
          'a_position'
        );
        gl.enableVertexAttribArray(positionLocation);
        gl.bindBuffer(gl.ARRAY_BUFFER, positionBuffer);
        gl.vertexAttribPointer(positionLocation, 2, gl.FLOAT, false, 0, 0);

        const texCoordLocation = gl.getAttribLocation(
          shaderProgram,
          'a_texCoord'
        );
        gl.enableVertexAttribArray(texCoordLocation);
        gl.bindBuffer(gl.ARRAY_BUFFER, texCoordBuffer);
        gl.vertexAttribPointer(texCoordLocation, 2, gl.FLOAT, false, 0, 0);

        gl.clear(gl.COLOR_BUFFER_BIT);
        gl.drawArrays(gl.TRIANGLE_STRIP, 0, 4);

        const dataURL = canvas.toDataURL('image/png');

        const base64Data = dataURL.split(',')[1];
        const arrayBuffer = my.base64ToArrayBuffer(base64Data);
        const uintArray = new Uint8Array(arrayBuffer);
        const arrayLength = uintArray.length;

        const bufferPtr = APGameGlobal.Module._malloc(arrayLength);
        APGameGlobal.Module.HEAPU8.set(uintArray, bufferPtr);

        APGameGlobal.Module.dynCall_viiii(
          successCallback,
          bufferPtr,
          arrayLength,
          canvas.width,
          canvas.height
        );

        APGameGlobal.Module._free(bufferPtr);
        gl.deleteProgram(shaderProgram);
        gl.deleteBuffer(positionBuffer);
        gl.deleteBuffer(texCoordBuffer);
        gl.deleteTexture(texture);
      } catch (err) {
        console.error(err);
        handleError(err);
      }
    };

    image.onerror = function (err) {
      handleError(err);
    };

    function handleError(err) {
      console.error('图像加载出错', err);
      const errorMessage = JSON.stringify(err);
      const bufferPtr = APGameGlobal.Module._malloc(errorMessage.length + 1);
      const errorByteArray = new Uint8Array(errorMessage.length + 1);
      for (let i = 0; i < errorMessage.length; i++) {
        errorByteArray[i] = errorMessage.charCodeAt(i);
      }
      errorByteArray[errorMessage.length] = 0;
      APGameGlobal.Module.HEAPU8.set(errorByteArray, bufferPtr);
      APGameGlobal.Module.dynCall_vi(errorCallback, bufferPtr);
      APGameGlobal.Module._free(bufferPtr);
    }

    const vsSource = `
    attribute vec2 a_position;
    attribute vec2 a_texCoord;
    varying vec2 v_texCoord;
    
    void main() {
        gl_Position = vec4(a_position, 0.0, 1.0);
        v_texCoord = a_texCoord;
    }
`;

    const fsSource = `
    precision mediump float;
    varying vec2 v_texCoord;
    uniform sampler2D u_texture;
    
    void main() {
        gl_FragColor = texture2D(u_texture, v_texCoord);
    }
`;

    function initShaderProgram(gl, vsSource, fsSource) {
      try {
        const vertexShader = loadShader(gl, gl.VERTEX_SHADER, vsSource);
        const fragmentShader = loadShader(gl, gl.FRAGMENT_SHADER, fsSource);

        const shaderProgram = gl.createProgram();
        gl.attachShader(shaderProgram, vertexShader);
        gl.attachShader(shaderProgram, fragmentShader);
        gl.linkProgram(shaderProgram);
        gl.deleteShader(vertexShader);
        gl.deleteShader(fragmentShader);

        if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS)) {
          console.error(
            '无法初始化着色器程序: ' + gl.getProgramInfoLog(shaderProgram)
          );
          return null;
        }
        return shaderProgram;
      } catch (err) {
        console.error('Error initializing shader program:', err);
        return null;
      }
    }

    function loadShader(gl, type, source) {
      try {
        const shader = gl.createShader(type);
        gl.shaderSource(shader, source);
        gl.compileShader(shader);

        if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
          console.error('编译着色器时出错: ' + gl.getShaderInfoLog(shader));
          gl.deleteShader(shader);
          return null;
        }
        return shader;
      } catch (err) {
        console.error('Error compiling shader:', err);
        return null;
      }
    }
  },

  OnStartFeedback: function (param) {
    if (my.canIUse && my.canIUse('onStartFeedback')) {
      my.onStartFeedback(() => {
        return param;
      });
    } else {
      console.error(
        '[error] 当前环境不支持 my.onStartFeedback 能力！请查看文档！'
      );
    }
  },

  SendMonitor: function (type, params) {
    try {
      if (my.isIDE) {
        return;
      }
      const packet = {
        type: type,
        params: params,
        __invokeType__: 'async',
        __callBackId__: -1,
      };
      UnityJsBridge.handleMsgFromUnity('sendMonitor', JSON.stringify(packet));
    } catch (error) {
      console.error('[Monitor] sendMonitor error:', error);
    }
  },
};
export default AlipayBaseAPI;
