import APGameGlobal from './global';
import AlipayUDPManager from './udp-manager';
import AlipayTCPManager from './tcp-manager';
import AlipayBaseAPI from './baseAPI';
import texture from './texture';
import AlipayFS from './fs';
import AlipayLogManager from './logManager';
import unityaudio from './unity-audio';

const ALIPAYWASMSDK = {
  ...AlipayUDPManager,
  ...AlipayTCPManager,
  ...AlipayBaseAPI,
  ...AlipayFS,
  ...AlipayLogManager,
  ...unityaudio,
};

APGameGlobal.ALIPAYWASMSDK = ALIPAYWASMSDK;
export default ALIPAYWASMSDK;
