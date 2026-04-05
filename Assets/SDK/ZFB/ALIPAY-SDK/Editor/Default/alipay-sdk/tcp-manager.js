import APGameGlobal from './global';
import { generateUniqueHandleID } from './utils';

const longLiveTCPObjects = {};
const tcpMessageCallbacks = {};
const tcpCloseCallbacks = {};
const tcpErrorCallbacks = {};
const tcpConnectCallbacks = {};

const AlipayTCPManager = {
  Alipay_CreateTCPSocket: function (bindToWifi = false) {
    const handleID = generateUniqueHandleID();
    const options = { bindToWifi: Boolean(bindToWifi) };
    const socket = my.createTCPSocket(options);
    longLiveTCPObjects[handleID] = socket;
    return handleID;
  },

  Alipay_CloseTCPSocket: function (handleID) {
    if (longLiveTCPObjects[handleID]) {
      longLiveTCPObjects[handleID].close();
      delete longLiveTCPObjects[handleID];
    }
  },

  Alipay_ConnectTCPSocket: function (handleID, address, port, timeout = 20000) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const options = { address, port, timeout };
      socket.connect(options);
    }
  },

  Alipay_SendTCPSocketMessage: function (handleID, message, param) {
    const paramObj = JSON.parse(param);
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      socket.send({
        message: message,
      });
    } else {
      console.error('TCPSocket not found for handleID:', handleID);
    }
  },

  Alipay_SendTCPSocketBuffer: function (handleID, dataPtr, dataLength, param) {
    const paramObj = JSON.parse(param);
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const messageData = {
        message: APGameGlobal.Module.HEAPU8.slice(dataPtr, dataPtr + dataLength)
          .buffer,
      };
      if (paramObj.length && paramObj.offset) {
        messageData.offset = paramObj.offset;
        messageData.length = paramObj.length;
      }
      socket.send(messageData);
    } else {
      console.error('TCPSocket not found for handleID:', handleID);
    }
  },

  Alipay_OnTCPSocketMessage: function (handleID, callback) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_message';
      tcpMessageCallbacks[callbackKey] = (messageEvent) => {
        const { remoteInfo, message } = messageEvent;
        let uint8Message;
        if (Array.isArray(message)) {
          try {
            uint8Message = new Uint8Array(message);
          } catch (e) {
            console.error('Failed to convert message to Uint8Array:', e);
            return;
          }
        } else if (message instanceof ArrayBuffer) {
          uint8Message = new Uint8Array(message);
        } else if (message instanceof Uint8Array) {
          uint8Message = message;
        } else {
          console.error('Invalid message type:', message);
          return;
        }

        const remoteInfoJson = JSON.stringify(remoteInfo);
 
        const remoteInfoPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(remoteInfoJson) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          remoteInfoJson,
          remoteInfoPtr,
          APGameGlobal.Module.lengthBytesUTF8(remoteInfoJson) + 1
        );

        const messageBuffer = APGameGlobal.Module._malloc(
          uint8Message.byteLength
        );
        APGameGlobal.Module.HEAPU8.set(uint8Message, messageBuffer);

        const handleIDPtr = APGameGlobal.Module._malloc(
          APGameGlobal.Module.lengthBytesUTF8(handleID) + 1
        );
        APGameGlobal.Module.stringToUTF8(
          handleID,
          handleIDPtr,
          APGameGlobal.Module.lengthBytesUTF8(handleID) + 1
        );

        APGameGlobal.Module.dynCall_viiii(
          callback,
          handleIDPtr,
          remoteInfoPtr,
          messageBuffer,
          uint8Message.byteLength
        );

        APGameGlobal.Module._free(messageBuffer);
        APGameGlobal.Module._free(remoteInfoPtr);
        APGameGlobal.Module._free(handleIDPtr);
      };
      socket.onMessage(tcpMessageCallbacks[callbackKey]);
    }
  },

  Alipay_OffTCPSocketMessage: function (handleID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_message';
      if (tcpMessageCallbacks[callbackKey]) {
        socket.offMessage(tcpMessageCallbacks[callbackKey]);
        delete tcpMessageCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnTCPSocketClose: function (handleID, callbackID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_close';
      tcpCloseCallbacks[callbackKey] = (event) => {
        const resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onClose(tcpCloseCallbacks[callbackKey]);
    }
  },

  Alipay_OffTCPSocketClose: function (handleID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_close';
      if (tcpCloseCallbacks[callbackKey]) {
        socket.offClose(tcpCloseCallbacks[callbackKey]);
        delete tcpCloseCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnTCPSocketError: function (handleID, callbackID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_error';
      tcpErrorCallbacks[callbackKey] = (event) => {
        const resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onError(tcpErrorCallbacks[callbackKey]);
    }
  },

  Alipay_OffTCPSocketError: function (handleID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_error';
      if (tcpErrorCallbacks[callbackKey]) {
        socket.offError(tcpErrorCallbacks[callbackKey]);
        delete tcpErrorCallbacks[callbackKey];
      }
    }
  },

  Alipay_OnTCPSocketConnect: function (handleID, callbackID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_connect';
      tcpConnectCallbacks[callbackKey] = (event) => {
        const resultString = JSON.stringify({
          messageId: callbackID,
          result: event,
        });
        APGameGlobal.Module.SendMessage(
          'AlipayBridge',
          'ReceiveMessageFromJS',
          resultString
        );
      };
      socket.onConnect(tcpConnectCallbacks[callbackKey]);
    }
  },

  Alipay_OffTCPSocketConnect: function (handleID) {
    const socket = longLiveTCPObjects[handleID];
    if (socket) {
      const callbackKey = handleID + '_connect';
      if (tcpConnectCallbacks[callbackKey]) {
        socket.offConnect(tcpConnectCallbacks[callbackKey]);
        delete tcpConnectCallbacks[callbackKey];
      }
    }
  },
};

export default AlipayTCPManager;
