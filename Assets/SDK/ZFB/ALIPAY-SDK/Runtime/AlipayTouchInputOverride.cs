#if (UNITY_WEBGL || UNITY_EDITOR) && SDK_ZFB
using System.Collections.Generic;
using AlipaySdk;
using LitJson;
using UnityEngine;
using UnityEngine.EventSystems;
using Touch = UnityEngine.Touch;

internal class TouchData
{
    public Touch touch;
    public long timeStamp;
}

[RequireComponent(typeof(StandaloneInputModule))]
public class AlipayTouchInputOverride : BaseInput
{
    private readonly List<TouchData> _touches = new List<TouchData>();

    private readonly List<TouchData> _changedTouches = new List<TouchData>();

    private readonly Dictionary<int, TouchData> _activeTouches = new Dictionary<int, TouchData>();

    private readonly HashSet<int> _lastActiveIds = new HashSet<int>();

    private StandaloneInputModule _standaloneInputModule = null;
    private static float? cachedPixelRatio = null;
    private static float? cachedWindowHeight = null;

    public static float PixelRatio
    {
        get
        {
            if (cachedPixelRatio == null)
            {
                JsonData systemInfo = JsonMapper.ToObject(AlipaySDK.API.GetSystemInfoSync());
                cachedPixelRatio = float.Parse(systemInfo["pixelRatio"].ToString());
            }
            return (float)cachedPixelRatio;
        }
    }

    public static float WindowHeight
    {
        get
        {
            if (cachedWindowHeight == null)
            {
                JsonData systemInfo = JsonMapper.ToObject(AlipaySDK.API.GetSystemInfoSync());
                cachedWindowHeight = float.Parse(systemInfo["windowHeight"].ToString());
            }
            return (float)cachedWindowHeight;
        }
    }
    public static void SetDevicePixelRatio(float pixelRatio)
    {
        cachedPixelRatio = pixelRatio;
    }
    protected override void Awake()
    {
        base.Awake();
        _standaloneInputModule = GetComponent<StandaloneInputModule>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        InitAlipayTouchEvents();
        if (_standaloneInputModule != null)
            _standaloneInputModule.inputOverride = this;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        UnregisterAlipayTouchEvents();
        if (_standaloneInputModule != null)
            _standaloneInputModule.inputOverride = null;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < _touches.Count; i++)
        {
            var td = _touches[i];
            if (td.touch.phase == TouchPhase.Began)
            {
                var t = td.touch;
                t.phase = TouchPhase.Stationary;
                td.touch = t;
                _touches[i] = td;
            }
        }
        RemoveEndedTouches();
        _changedTouches.Clear();
    }

    private void RemoveEndedTouches()
    {
        if (_touches.Count == 0) return;

        _touches.RemoveAll(td =>
        {
            var ph = td.touch.phase;
            if (ph == TouchPhase.Ended || ph == TouchPhase.Canceled)
            {
                _activeTouches.Remove(td.touch.fingerId);
                _lastActiveIds.Remove(td.touch.fingerId);
                return true;
            }
            return false;
        });
    }

    private void InitAlipayTouchEvents()
    {
        AlipaySDK.API.OnTouchStart(OnAlipayTouchStart);
        AlipaySDK.API.OnTouchMove(OnAlipayTouchMove);
        AlipaySDK.API.OnTouchEnd(OnAlipayTouchEnd);
        AlipaySDK.API.OnTouchCancel(OnAlipayTouchCancel);
    }

    private void UnregisterAlipayTouchEvents()
    {
        AlipaySDK.API.OffTouchStart(OnAlipayTouchStart);
        AlipaySDK.API.OffTouchMove(OnAlipayTouchMove);
        AlipaySDK.API.OffTouchEnd(OnAlipayTouchEnd);
        AlipaySDK.API.OffTouchCancel(OnAlipayTouchCancel);
    }

    private void OnAlipayTouchStart(OnTouchListenerResult touchEvent)
    {
        ProcessFullTouchList(touchEvent);
    }

    private void OnAlipayTouchMove(OnTouchListenerResult touchEvent)
    {
        ProcessFullTouchList(touchEvent);
    }

    private void OnAlipayTouchEnd(OnTouchListenerResult touchEvent)
    {
        foreach (var at in touchEvent.touches)
        {
            Vector2 pos = FixTouchPos(new Vector2(at.clientX, at.clientY));
            TouchData data = FindTouchData(at.identifier) ?? FindOrCreateTouchData(at.identifier);
            UpdateTouchDataFinal(data, pos, touchEvent.timeStamp, TouchPhase.Ended);
            if (!_changedTouches.Contains(data))
                _changedTouches.Add(data);
        }
    }

    private void OnAlipayTouchCancel(OnTouchListenerResult touchEvent)
    {
        foreach (var at in touchEvent.touches)
        {
            Vector2 pos = FixTouchPos(new Vector2(at.clientX, at.clientY));
            TouchData data = FindTouchData(at.identifier) ?? FindOrCreateTouchData(at.identifier);
            UpdateTouchDataFinal(data, pos, touchEvent.timeStamp, TouchPhase.Canceled);
            if (!_changedTouches.Contains(data))
                _changedTouches.Add(data);
        }
    }

    private void ProcessFullTouchList(OnTouchListenerResult touchEvent)
    {
        var currentMap = new Dictionary<int, Vector2>();
        foreach (var at in touchEvent.touches)
        {
            Vector2 pos = FixTouchPos(new Vector2(at.clientX, at.clientY));
            currentMap[at.identifier] = pos;
        }

        var currentIds = new HashSet<int>(currentMap.Keys);

        var endedIds = new List<int>();
        foreach (var id in _lastActiveIds)
        {
            if (!currentIds.Contains(id))
                endedIds.Add(id);
        }

        foreach (var id in endedIds)
        {
            var data = FindTouchData(id);
            if (data != null)
            {
                UpdateTouchDataFinal(data, data.touch.position, touchEvent.timeStamp, TouchPhase.Ended);
                if (!_changedTouches.Contains(data))
                    _changedTouches.Add(data);
            }
        }

        foreach (var kv in currentMap)
        {
            int id = kv.Key;
            Vector2 pos = kv.Value;
            var data = FindOrCreateTouchData(id);

            if (!_lastActiveIds.Contains(id))
            {
                var t = data.touch;
                t.fingerId = id;
                t.phase = TouchPhase.Began;
                t.position = pos;
                t.rawPosition = pos;
                t.deltaPosition = Vector2.zero;
                t.deltaTime = 0f;
                data.touch = t;
                data.timeStamp = touchEvent.timeStamp;

                if (!_touches.Contains(data))
                    _touches.Add(data);

                _activeTouches[id] = data;

                if (!_changedTouches.Contains(data))
                    _changedTouches.Add(data);
            }
            else
            {
                var oldPos = data.touch.position;
                if (pos != oldPos)
                {
                    UpdateTouchDataFinal(data, pos, touchEvent.timeStamp, TouchPhase.Moved);
                }
                else
                {
                    UpdateTouchDataFinal(data, pos, touchEvent.timeStamp, TouchPhase.Stationary);
                }

                if (!_changedTouches.Contains(data) && data.touch.phase != TouchPhase.Stationary)
                    _changedTouches.Add(data);
            }
        }
        _lastActiveIds.Clear();
        foreach (var id in currentIds)
            _lastActiveIds.Add(id);
    }

    private TouchData FindTouchData(int identifier)
    {
        if (_activeTouches.TryGetValue(identifier, out var data))
            return data;

        foreach (var td in _touches)
        {
            if (td.touch.fingerId == identifier)
                return td;
        }

        return null;
    }

    private TouchData FindOrCreateTouchData(int identifier)
    {
        var existing = FindTouchData(identifier);
        if (existing != null)
            return existing;

        var data = new TouchData();
        Touch t = new Touch
        {
            pressure = 1.0f,
            maximumPossiblePressure = 1.0f,
            type = TouchType.Direct,
            tapCount = 1,
            fingerId = identifier,
            radius = 0,
            radiusVariance = 0,
            altitudeAngle = 0,
            azimuthAngle = 0,
            deltaTime = 0f,
            deltaPosition = Vector2.zero,
            position = Vector2.zero,
            rawPosition = Vector2.zero,
            phase = TouchPhase.Canceled
        };

        data.touch = t;
        data.timeStamp = 0;

        _touches.Add(data);
        _activeTouches[identifier] = data;
        return data;
    }

    private static void UpdateTouchData(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
    {
        var t = data.touch;
        t.phase = phase;
        t.deltaPosition = pos - t.position;
        t.position = pos;
        t.rawPosition = pos;

        if (data.timeStamp <= 0)
            t.deltaTime = 0f;
        else
            t.deltaTime = (timeStamp - data.timeStamp) / 1000f;

        data.touch = t;
    }

    private static void UpdateTouchDataFinal(TouchData data, Vector2 pos, long timeStamp, TouchPhase phase)
    {
        UpdateTouchData(data, pos, timeStamp, phase);
        data.timeStamp = timeStamp;
    }

    private static Vector2 FixTouchPos(Vector2 pos)
    {
        pos.x = Mathf.RoundToInt(pos.x * PixelRatio);
        pos.y = Mathf.RoundToInt(WindowHeight * PixelRatio - pos.y * PixelRatio);
        return pos;
    }

#if !UNITY_EDITOR
    public override bool touchSupported => true;
    public override bool mousePresent => false;
    public override int touchCount => _changedTouches.Count;
    public override Touch GetTouch(int index) => _changedTouches[index].touch;
#endif
}
#endif