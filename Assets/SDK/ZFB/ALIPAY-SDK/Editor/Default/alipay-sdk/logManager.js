const logger = (() => {
  if (my.canIUse && my.canIUse('getLogManager')) {
    return my.getLogManager();
  } else {
    const tag = '[NoLogManager]';
    const fallback = console || {};
    return {
      debug: function (...args) {
        (fallback.debug || fallback.log || function () {})(tag + 'debug (仅console, 不落库):', ...args);
      },
      log: function (...args) {
        (fallback.log || function () {})(tag + 'log (仅console, 不落库):', ...args);
      },
      info: function (...args) {
        (fallback.info || fallback.log || function () {})(tag + 'info (仅console, 不落库):', ...args);
      },
      warn: function (...args) {
        (fallback.warn || fallback.log || function () {})(tag + 'warn (仅console, 不落库):', ...args);
      }
    }
  }
})();

const AlipayLogManager = {
  AlipayLogManagerDebug: function (jsonStr) {
    logger.debug(JSON.parse(jsonStr));
  },
  AlipayLogManagerLog: function (jsonStr) {
    logger.log(JSON.parse(jsonStr));
  },
  AlipayLogManagerInfo: function (jsonStr) {
    logger.info(JSON.parse(jsonStr));
  },
  AlipayLogManagerWarn: function (jsonStr) {
    logger.warn(JSON.parse(jsonStr));
  }
};

export default AlipayLogManager;