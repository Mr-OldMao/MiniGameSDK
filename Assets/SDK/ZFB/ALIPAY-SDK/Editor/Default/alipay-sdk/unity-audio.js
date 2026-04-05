import APGameGlobal from './global';
var WEBAudio = {
  audioInstanceIdCounter: 0,
  audioInstances: {},
  audioContext: null,
  audioWebEnabled: 0,
  audioCache: [],
  pendingAudioSources: {},
  FAKEMOD_SAMPLERATE: 44100,
};
function jsAudioMixinSetPitch(source) {
  source.estimatePlaybackPosition = function () {
    var t =
      (WEBAudio.audioContext.currentTime - source.playbackStartTime) *
      source.playbackRate.value;
    if (source.loop && t >= source.loopStart) {
      t =
        ((t - source.loopStart) % (source.loopEnd - source.loopStart)) +
        source.loopStart;
    }
    return t;
  };
  source.setPitch = function (newPitch) {
    var curPosition = source.estimatePlaybackPosition();
    if (curPosition >= 0) {
      source.playbackStartTime =
        WEBAudio.audioContext.currentTime - curPosition / newPitch;
    }
    if (source.playbackRate.value !== newPitch)
      source.playbackRate.value = newPitch;
  };
}
function jsAudioCreateUncompressedSoundClip(buffer, error) {
  var soundClip = { buffer: buffer, error: error };
  soundClip.release = function () {};
  soundClip.getLength = function () {
    if (!this.buffer) {
      console.log('Trying to get length of sound which is not loaded.');
      return 0;
    }
    return this.buffer.length;
  };
  soundClip.getData = function (ptr, length) {
    if (!this.buffer) {
      console.log('Trying to get data of sound which is not loaded.');
      return 0;
    }
    var startOutputBuffer = ptr >> 2;
    var output = APGameGlobal.Module.HEAPF32.subarray(
      startOutputBuffer,
      startOutputBuffer + (length >> 2)
    );
    var numMaxSamples = Math.floor(
      (length >> 2) / this.buffer.numberOfChannels
    );
    var numReadSamples = Math.min(this.buffer.length, numMaxSamples);
    for (var i = 0; i < this.buffer.numberOfChannels; i++) {
      var channelData = this.buffer
        .getChannelData(i)
        .subarray(0, numReadSamples);
      output.set(channelData, i * numReadSamples);
    }
    return numReadSamples * this.buffer.numberOfChannels * 4;
  };
  soundClip.getNumberOfChannels = function () {
    if (!this.buffer) {
      console.log('Trying to get metadata of sound which is not loaded.');
      return 0;
    }
    return this.buffer.numberOfChannels;
  };
  soundClip.getFrequency = function () {
    if (!this.buffer) {
      console.log('Trying to get metadata of sound which is not loaded.');
      return 0;
    }
    return this.buffer.sampleRate;
  };
  soundClip.createSourceNode = function () {
    if (!this.buffer) {
      console.log('Trying to play sound which is not loaded.');
    }
    var source = WEBAudio.audioContext.createBufferSource();
    source.buffer = this.buffer;
    jsAudioMixinSetPitch(source);
    return source;
  };
  return soundClip;
}
function jsAudioCreateChannel(callback, userData) {
  var channel = {
    callback: callback,
    userData: userData,
    source: null,
    gain: WEBAudio.audioContext.createGain(),
    panner: WEBAudio.audioContext.createPanner(),
    threeD: false,
    loop: false,
    loopStart: 0,
    loopEnd: 0,
    pitch: 1,
  };
  channel.panner.rolloffFactor = 0;
  channel.release = function () {
    this.disconnectSource();
    this.gain.disconnect();
    this.panner.disconnect();
  };
  channel.playSoundClip = function (soundClip, startTime, startOffset) {
    try {
      var self = this;
      this.source = soundClip.createSourceNode();
      this.setupPanning();
      this.source.onended = function () {
        self.source.isStopped = true;
        self.disconnectSource();
        if (self.callback) {
          APGameGlobal.Module.dynCall_vi(self.callback, [self.userData]);
        }
      };
      this.source.loop = this.loop;
      this.source.loopStart = this.loopStart;
      this.source.loopEnd = this.loopEnd;
      this.source.start(startTime, startOffset);
      this.source.playbackStartTime =
        startTime - startOffset / this.source.playbackRate.value;
      this.source.setPitch(this.pitch);
    } catch (e) {
      console.error('Channel.playSoundClip error. Exception: ' + e);
    }
  };
  channel.stop = function (delay) {
    if (!this.source) {
      return;
    }
    try {
      channel.source.stop(WEBAudio.audioContext.currentTime + delay);
    } catch (e) {}
    if (delay == 0) {
      this.disconnectSource();
    }
  };
  channel.isPaused = function () {
    if (!this.source) {
      return true;
    }
    if (this.source.isPausedMockNode) {
      return true;
    }
    if (this.source.mediaElement) {
      return this.source.mediaElement.paused || this.source.pauseRequested;
    }
    return false;
  };
  channel.pause = function () {
    if (!this.source || this.source.isPausedMockNode) {
      return;
    }
    if (this.source.mediaElement) {
      this.source._pauseMediaElement();
      return;
    }
    var pausedSource = {
      isPausedMockNode: true,
      buffer: this.source.buffer,
      loop: this.source.loop,
      loopStart: this.source.loopStart,
      loopEnd: this.source.loopEnd,
      playbackRate: this.source.playbackRate.value,
      scheduledStopTime: undefined,
      playbackPausedAtPosition: this.source.estimatePlaybackPosition(),
      setPitch: function (v) {
        this.playbackRate = v;
      },
      stop: function (when) {
        this.scheduledStopTime = when;
      },
    };
    this.stop(0);
    this.disconnectSource();
    this.source = pausedSource;
  };
  channel.resume = function () {
    if (this.source && this.source.mediaElement) {
      this.source.start(undefined, this.source.currentTime);
      return;
    }
    if (!this.source || !this.source.isPausedMockNode) {
      return;
    }
    var pausedSource = this.source;
    var soundClip = jsAudioCreateUncompressedSoundClip(
      pausedSource.buffer,
      false
    );
    this.playSoundClip(
      soundClip,
      WEBAudio.audioContext.currentTime,
      Math.max(0, pausedSource.playbackPausedAtPosition)
    );
    this.source.loop = pausedSource.loop;
    this.source.loopStart = pausedSource.loopStart;
    this.source.loopEnd = pausedSource.loopEnd;
    this.source.setPitch(pausedSource.playbackRate);
    if (typeof pausedSource.scheduledStopTime !== 'undefined') {
      var delay = Math.max(
        pausedSource.scheduledStopTime - WEBAudio.audioContext.currentTime,
        0
      );
      this.stop(delay);
    }
  };
  channel.setLoop = function (loop) {
    this.loop = loop;
    if (!this.source || this.source.loop == loop) {
      return;
    }
    this.source.loop = loop;
  };
  channel.setLoopPoints = function (loopStart, loopEnd) {
    this.loopStart = loopStart;
    this.loopEnd = loopEnd;
    if (!this.source) {
      return;
    }
    if (this.source.loopStart !== loopStart) {
      this.source.loopStart = loopStart;
    }
    if (this.source.loopEnd !== loopEnd) {
      this.source.loopEnd = loopEnd;
    }
  };
  channel.set3D = function (threeD) {
    if (this.threeD == threeD) {
      return;
    }
    this.threeD = threeD;
    if (!this.source) {
      return;
    }
    this.setupPanning();
  };
  channel.setPitch = function (pitch) {
    this.pitch = pitch;
    if (!this.source) {
      return;
    }
    this.source.setPitch(pitch);
  };
  channel.setVolume = function (volume) {
    if (this.gain.gain.value !== volume) {
      this.gain.gain.value = volume;
    }
    if (this.source && this.source.mediaElement) {
      this.source.mediaElement.volume = volume;
    }
  };
  channel.setPosition = function (x, y, z) {
    var p = this.panner;
    if (p.positionX) {
      if (p.positionX.value !== x) p.positionX.value = x;
      if (p.positionY.value !== y) p.positionY.value = y;
      if (p.positionZ.value !== z) p.positionZ.value = z;
    } else if (p._x !== x || p._y !== y || p._z !== z) {
      p.setPosition(x, y, z);
      p._x = x;
      p._y = y;
      p._z = z;
    }
  };
  channel.disconnectSource = function () {
    if (!this.source || this.source.isPausedMockNode) {
      return;
    }
    if (this.source.mediaElement) {
      this.source._pauseMediaElement();
    }
    this.source.onended = null;
    this.source.disconnect();
    delete this.source;
  };
  channel.setupPanning = function () {
    if (this.source.isPausedMockNode) return;
    this.source.disconnect();
    this.panner.disconnect();
    this.gain.disconnect();
    if (this.threeD) {
      this.source.connect(this.panner);
      this.panner.connect(this.gain);
    } else {
      this.source.connect(this.gain);
    }
    this.gain.connect(WEBAudio.audioContext.destination);
  };
  channel.isStopped = function () {
    if (!this.source) {
      return true;
    }
    if (this.source.mediaElement) {
      return this.source.isStopped;
    }
    return false;
  };
  return channel;
}
function _JS_Sound_Create_Channel(callback, userData) {
  if (WEBAudio.audioWebEnabled == 0) return;
  WEBAudio.audioInstances[++WEBAudio.audioInstanceIdCounter] =
    jsAudioCreateChannel(callback, userData);
  return WEBAudio.audioInstanceIdCounter;
}
function _JS_Sound_GetAudioBufferSampleRate(soundInstance) {
  if (WEBAudio.audioWebEnabled == 0) return WEBAudio.FAKEMOD_SAMPLERATE;
  var audioInstance = WEBAudio.audioInstances[soundInstance];
  if (!audioInstance) return WEBAudio.FAKEMOD_SAMPLERATE;
  var buffer = audioInstance.buffer
    ? audioInstance.buffer
    : audioInstance.source
    ? audioInstance.source.buffer
    : 0;
  if (!buffer) return WEBAudio.FAKEMOD_SAMPLERATE;
  return buffer.sampleRate;
}
function _JS_Sound_GetAudioContextSampleRate() {
  if (WEBAudio.audioWebEnabled == 0) return WEBAudio.FAKEMOD_SAMPLERATE;
  return WEBAudio.audioContext.sampleRate;
}
function _JS_Sound_GetLength(bufferInstance) {
  if (WEBAudio.audioWebEnabled == 0) return 0;
  var soundClip = WEBAudio.audioInstances[bufferInstance];
  if (!soundClip) return 0;
  return soundClip.getLength();
}
function _JS_Sound_GetLoadState(bufferInstance) {
  if (WEBAudio.audioWebEnabled == 0) return 2;
  var sound = WEBAudio.audioInstances[bufferInstance];
  if (sound.error) return 2;
  if (sound.buffer || sound.url) return 0;
  return 1;
}
function jsAudioPlayPendingBlockedAudio(soundId) {
  var pendingAudio = WEBAudio.pendingAudioSources[soundId];
  pendingAudio.sourceNode._startPlayback(pendingAudio.offset);
  delete WEBAudio.pendingAudioSources[soundId];
}
function jsAudioPlayBlockedAudios() {
  Object.keys(WEBAudio.pendingAudioSources).forEach(function (audioId) {
    jsAudioPlayPendingBlockedAudio(audioId);
  });
}
function _JS_Sound_Init() {
  try {
    window.AudioContext = window.AudioContext || window.webkitAudioContext;
    WEBAudio.audioContext = new AudioContext();
    var tryToResumeAudioContext = function () {
      if (WEBAudio.audioContext.state === 'suspended')
        WEBAudio.audioContext.resume().catch(function (error) {
          console.warn('Could not resume audio context. Exception: ' + error);
        });
      else APGameGlobal.Module.clearInterval(resumeInterval);
    };
    var resumeInterval = APGameGlobal.Module.setInterval(
      tryToResumeAudioContext,
      400
    );
    WEBAudio.audioWebEnabled = 1;
    var _userEventCallback = function () {
      try {
        if (
          WEBAudio.audioContext.state !== 'running' &&
          WEBAudio.audioContext.state !== 'closed'
        ) {
          WEBAudio.audioContext.resume().catch(function (error) {
            console.warn('Could not resume audio context. Exception: ' + error);
          });
        }
        jsAudioPlayBlockedAudios();
        var audioCacheSize = 20;
        while (WEBAudio.audioCache.length < audioCacheSize) {
          var audio = new Audio();
          audio.autoplay = false;
          WEBAudio.audioCache.push(audio);
        }
      } catch (e) {}
    };
    window.addEventListener('mousedown', _userEventCallback);
    window.addEventListener('touchstart', _userEventCallback);
    APGameGlobal.Module.deinitializers.push(function () {
      window.removeEventListener('mousedown', _userEventCallback);
      window.removeEventListener('touchstart', _userEventCallback);
    });
  } catch (e) {
    alert('Web Audio API is not supported in this browser');
  }
}
function jsAudioCreateUncompressedSoundClipFromCompressedAudio(audioData) {
  var soundClip = jsAudioCreateUncompressedSoundClip(null, false);
  WEBAudio.audioContext.decodeAudioData(
    audioData,
    function (_buffer) {
      soundClip.buffer = _buffer;
    },
    function (_error) {
      soundClip.error = true;
      console.log('Decode error: ' + _error);
    }
  );
  return soundClip;
}
function jsAudioAddPendingBlockedAudio(sourceNode, offset) {
  WEBAudio.pendingAudioSources[sourceNode.mediaElement.src] = {
    sourceNode: sourceNode,
    offset: offset,
  };
}
function jsAudioGetMimeTypeFromType(fmodSoundType) {
  switch (fmodSoundType) {
    case 13:
      return 'audio/mpeg';
    case 20:
      return 'audio/wav';
    default:
      return 'audio/mp4';
  }
}
function jsAudioCreateCompressedSoundClip(audioData, fmodSoundType) {
  var mimeType = jsAudioGetMimeTypeFromType(fmodSoundType);
  var blob = new Blob([audioData], { type: mimeType });
  var soundClip = {
    url: URL.createObjectURL(blob),
    error: false,
    mediaElement: new Audio(),
  };
  soundClip.mediaElement.preload = 'metadata';
  soundClip.mediaElement.src = soundClip.url;
  soundClip.release = function () {
    if (!this.mediaElement) {
      return;
    }
    this.mediaElement.src = '';
    URL.revokeObjectURL(this.url);
    delete this.mediaElement;
    delete this.url;
  };
  soundClip.getLength = function () {
    return this.mediaElement.duration * 44100;
  };
  soundClip.getData = function (ptr, length) {
    console.warn('getData() is not supported for compressed sound.');
    return 0;
  };
  soundClip.getNumberOfChannels = function () {
    console.warn(
      'getNumberOfChannels() is not supported for compressed sound.'
    );
    return 0;
  };
  soundClip.getFrequency = function () {
    console.warn('getFrequency() is not supported for compressed sound.');
    return 0;
  };
  soundClip.createSourceNode = function () {
    var self = this;
    var mediaElement = WEBAudio.audioCache.length
      ? WEBAudio.audioCache.pop()
      : new Audio();
    mediaElement.preload = 'metadata';
    mediaElement.src = this.url;
    var source = WEBAudio.audioContext.createMediaElementSource(mediaElement);
    Object.defineProperty(source, 'loop', {
      get: function () {
        return source.mediaElement.loop;
      },
      set: function (v) {
        if (source.mediaElement.loop !== v) source.mediaElement.loop = v;
      },
    });
    source.playbackRate = {};
    Object.defineProperty(source.playbackRate, 'value', {
      get: function () {
        return source.mediaElement.playbackRate;
      },
      set: function (v) {
        if (source.mediaElement.playbackRate !== v)
          source.mediaElement.playbackRate = v;
      },
    });
    Object.defineProperty(source, 'currentTime', {
      get: function () {
        return source.mediaElement.currentTime;
      },
      set: function (v) {
        if (source.mediaElement.currentTime !== v)
          source.mediaElement.currentTime = v;
      },
    });
    Object.defineProperty(source, 'mute', {
      get: function () {
        return source.mediaElement.mute;
      },
      set: function (v) {
        if (source.mediaElement.mute !== v) source.mediaElement.mute = v;
      },
    });
    Object.defineProperty(source, 'onended', {
      get: function () {
        return source.mediaElement.onended;
      },
      set: function (onended) {
        source.mediaElement.onended = onended;
      },
    });
    source.playPromise = null;
    source.playTimeout = null;
    source.pauseRequested = false;
    source.isStopped = false;
    source._pauseMediaElement = function () {
      if (source.playPromise || source.playTimeout) {
        source.pauseRequested = true;
      } else {
        source.mediaElement.pause();
      }
    };
    source._startPlayback = function (offset) {
      if (source.playPromise || source.playTimeout) {
        source.mediaElement.currentTime = offset;
        source.pauseRequested = false;
        return;
      }
      source.mediaElement.currentTime = offset;
      source.playPromise = source.mediaElement.play();
      if (source.playPromise) {
        source.playPromise
          .then(function () {
            if (source.pauseRequested) {
              source.mediaElement.pause();
              source.pauseRequested = false;
            }
            source.playPromise = null;
          })
          .catch(function (error) {
            source.playPromise = null;
            if (error.name !== 'NotAllowedError') throw error;
            jsAudioAddPendingBlockedAudio(source, offset);
          });
      }
    };
    source.start = function (startTime, offset) {
      if (typeof startTime === 'undefined') {
        startTime = WEBAudio.audioContext.currentTime;
      }
      if (typeof offset === 'undefined') {
        offset = 0;
      }
      var startDelayThresholdMS = 4;
      var startDelayMS = (startTime - WEBAudio.audioContext.currentTime) * 1e3;
      if (startDelayMS > startDelayThresholdMS) {
        source.playTimeout = setTimeout(function () {
          source.playTimeout = null;
          source._startPlayback(offset);
        }, startDelayMS);
      } else {
        source._startPlayback(offset);
      }
    };
    source.stop = function (stopTime) {
      if (typeof stopTime === 'undefined') {
        stopTime = WEBAudio.audioContext.currentTime;
      }
      var stopDelayThresholdMS = 4;
      var stopDelayMS = (stopTime - WEBAudio.audioContext.currentTime) * 1e3;
      if (stopDelayMS > stopDelayThresholdMS) {
        setTimeout(function () {
          source._pauseMediaElement();
          source.isStopped = true;
        }, stopDelayMS);
      } else {
        source._pauseMediaElement();
        source.isStopped = true;
      }
    };
    jsAudioMixinSetPitch(source);
    return source;
  };
  return soundClip;
}
function _JS_Sound_Load(ptr, length, decompress, fmodSoundType) {
  if (WEBAudio.audioWebEnabled == 0) return 0;
  var audioData = APGameGlobal.Module.HEAPU8.buffer.slice(ptr, ptr + length);
  decompress = 1;
  var sound;
  if (decompress) {
    sound = jsAudioCreateUncompressedSoundClipFromCompressedAudio(audioData);
  } else {
    sound = jsAudioCreateCompressedSoundClip(audioData, fmodSoundType);
  }
  WEBAudio.audioInstances[++WEBAudio.audioInstanceIdCounter] = sound;
  return WEBAudio.audioInstanceIdCounter;
}
function jsAudioCreateUncompressedSoundClipFromPCM(
  channels,
  length,
  sampleRate,
  ptr
) {
  var buffer = WEBAudio.audioContext.createBuffer(channels, length, sampleRate);
  for (var i = 0; i < channels; i++) {
    var offs = (ptr >> 2) + length * i;
    var copyToChannel =
      buffer['copyToChannel'] ||
      function (source, channelNumber, startInChannel) {
        var clipped = source.subarray(
          0,
          Math.min(source.length, this.length - (startInChannel | 0))
        );
        this.getChannelData(channelNumber | 0).set(clipped, startInChannel | 0);
      };
    copyToChannel.apply(buffer, [
      APGameGlobal.Module.HEAPF32.subarray(offs, offs + length),
      i,
      0,
    ]);
  }
  return jsAudioCreateUncompressedSoundClip(buffer, false);
}
function _JS_Sound_Load_PCM(channels, length, sampleRate, ptr) {
  if (WEBAudio.audioWebEnabled == 0) return 0;
  var sound = jsAudioCreateUncompressedSoundClipFromPCM(
    channels,
    length,
    sampleRate,
    ptr
  );
  WEBAudio.audioInstances[++WEBAudio.audioInstanceIdCounter] = sound;
  return WEBAudio.audioInstanceIdCounter;
}
function _JS_Sound_Play(bufferInstance, channelInstance, offset, delay) {
  if (WEBAudio.audioWebEnabled == 0) return;
  _JS_Sound_Stop(channelInstance, 0);
  var soundClip = WEBAudio.audioInstances[bufferInstance];
  var channel = WEBAudio.audioInstances[channelInstance];
  if (!soundClip) {
    console.log('Trying to play sound which is not loaded.');
    return;
  }
  try {
    channel.playSoundClip(
      soundClip,
      WEBAudio.audioContext.currentTime + delay,
      offset
    );
  } catch (e) {
    console.error('playSoundClip error. Exception: ' + e);
  }
}
function _JS_Sound_ReleaseInstance(instance) {
  var object = WEBAudio.audioInstances[instance];
  if (object) {
    object.release();
  }
  delete WEBAudio.audioInstances[instance];
}
function _JS_Sound_ResumeIfNeeded() {
  if (WEBAudio.audioWebEnabled == 0) return;
  if (WEBAudio.audioContext.state === 'suspended')
    WEBAudio.audioContext.resume().catch(function (error) {
      console.warn('Could not resume audio context. Exception: ' + error);
    });
}
function _JS_Sound_Set3D(channelInstance, threeD) {
  var channel = WEBAudio.audioInstances[channelInstance];
  channel.set3D(threeD);
}
function _JS_Sound_SetListenerOrientation(x, y, z, xUp, yUp, zUp) {
  if (WEBAudio.audioWebEnabled == 0) return;
  x = -x;
  y = -y;
  z = -z;
  var l = WEBAudio.audioContext.listener;
  if (l.forwardX) {
    if (l.forwardX.value !== x) l.forwardX.value = x;
    if (l.forwardY.value !== y) l.forwardY.value = y;
    if (l.forwardZ.value !== z) l.forwardZ.value = z;
    if (l.upX.value !== xUp) l.upX.value = xUp;
    if (l.upY.value !== yUp) l.upY.value = yUp;
    if (l.upZ.value !== zUp) l.upZ.value = zUp;
  } else if (
    l._forwardX !== x ||
    l._forwardY !== y ||
    l._forwardZ !== z ||
    l._upX !== xUp ||
    l._upY !== yUp ||
    l._upZ !== zUp
  ) {
    l.setOrientation(x, y, z, xUp, yUp, zUp);
    l._forwardX = x;
    l._forwardY = y;
    l._forwardZ = z;
    l._upX = xUp;
    l._upY = yUp;
    l._upZ = zUp;
  }
}
function _JS_Sound_SetListenerPosition(x, y, z) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var l = WEBAudio.audioContext.listener;
  if (l.positionX) {
    if (l.positionX.value !== x) l.positionX.value = x;
    if (l.positionY.value !== y) l.positionY.value = y;
    if (l.positionZ.value !== z) l.positionZ.value = z;
  } else if (l._positionX !== x || l._positionY !== y || l._positionZ !== z) {
    l.setPosition(x, y, z);
    l._positionX = x;
    l._positionY = y;
    l._positionZ = z;
  }
}
function _JS_Sound_SetLoop(channelInstance, loop) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var channel = WEBAudio.audioInstances[channelInstance];
  channel.setLoop(loop);
}
function _JS_Sound_SetLoopPoints(channelInstance, loopStart, loopEnd) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var channel = WEBAudio.audioInstances[channelInstance];
  channel.setLoopPoints(loopStart, loopEnd);
}
function _JS_Sound_SetPaused(channelInstance, paused) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var channel = WEBAudio.audioInstances[channelInstance];
  if (paused != channel.isPaused()) {
    if (paused) channel.pause();
    else channel.resume();
  }
}
function _JS_Sound_SetPitch(channelInstance, v) {
  if (WEBAudio.audioWebEnabled == 0) return;
  try {
    var channel = WEBAudio.audioInstances[channelInstance];
    channel.setPitch(v);
  } catch (e) {
    console.error(
      'JS_Sound_SetPitch(channel=' +
        channelInstance +
        ', pitch=' +
        v +
        ') threw an exception: ' +
        e
    );
  }
}
function _JS_Sound_GetPosition(channelInstance) {
  if (WEBAudio.audioWebEnabled == 0) {
    return 0;
  }
  const channel = WEBAudio.audioInstances[channelInstance];
  if (!channel) {
    return 0;
  }
  const { source } = channel;
  if (!source) {
    return 0;
  }
  return source.estimatePlaybackPosition
    ? source.estimatePlaybackPosition()
    : 0;
}
function _JS_Sound_SetPosition(channelInstance, x, y, z) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var channel = WEBAudio.audioInstances[channelInstance];
  channel.setPosition(x, y, z);
}
function _JS_Sound_SetVolume(channelInstance, v) {
  if (WEBAudio.audioWebEnabled == 0) return;
  try {
    var channel = WEBAudio.audioInstances[channelInstance];
    channel.setVolume(v);
  } catch (e) {
    console.error(
      'JS_Sound_SetVolume(channel=' +
        channelInstance +
        ', volume=' +
        v +
        ') threw an exception: ' +
        e
    );
  }
}
function _JS_Sound_Stop(channelInstance, delay) {
  if (WEBAudio.audioWebEnabled == 0) return;
  var channel = WEBAudio.audioInstances[channelInstance];
  channel.stop(delay);
}

function _JS_Sound_GetData(bufferInstance, ptr, length) {
  if (WEBAudio.audioWebEnabled == 0) return 0;
  var soundClip = WEBAudio.audioInstances[bufferInstance];
  if (!soundClip) return 0;
  return soundClip.getData(ptr, length);
}

function _JS_Sound_GetMetaData(bufferInstance, metaData) {
  if (WEBAudio.audioWebEnabled == 0) {
    APGameGlobal.Module.HEAPU32[metaData >> 2] = 0;
    APGameGlobal.Module.HEAPU32[(metaData >> 2) + 1] = 0;
    return false;
  }
  var soundClip = WEBAudio.audioInstances[bufferInstance];
  if (!soundClip) {
    APGameGlobal.Module.HEAPU32[metaData >> 2] = 0;
    APGameGlobal.Module.HEAPU32[(metaData >> 2) + 1] = 0;
    return false;
  }
  APGameGlobal.Module.HEAPU32[metaData >> 2] = soundClip.getNumberOfChannels();
  APGameGlobal.Module.HEAPU32[(metaData >> 2) + 1] = soundClip.getFrequency();
  return true;
}

function _JS_Sound_IsStopped(channelInstance) {
  if (WEBAudio.audioWebEnabled == 0) {
    return true;
  }
  const channel = WEBAudio.audioInstances[channelInstance];
  if (!channel) {
    return true;
  }
  return channel.isStopped();
}

const unityaudio = {
  _JS_Sound_Create_Channel,
  _JS_Sound_GetLength,
  _JS_Sound_GetLoadState,
  _JS_Sound_Init,
  _JS_Sound_IsStopped,
  _JS_Sound_Load,
  _JS_Sound_Load_PCM,
  _JS_Sound_Play,
  _JS_Sound_ReleaseInstance,
  _JS_Sound_ResumeIfNeeded,
  _JS_Sound_Set3D,
  _JS_Sound_SetListenerOrientation,
  _JS_Sound_SetListenerPosition,
  _JS_Sound_SetLoop,
  _JS_Sound_SetLoopPoints,
  _JS_Sound_SetPaused,
  _JS_Sound_SetPitch,
  _JS_Sound_SetPosition,
  _JS_Sound_SetVolume,
  _JS_Sound_Stop,
  _JS_Sound_GetData,
  _JS_Sound_GetMetaData,
  _JS_Sound_GetPosition,
  _JS_Sound_GetAudioBufferSampleRate,
  _JS_Sound_GetAudioContextSampleRate,
};

export default unityaudio;
