using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for testing a faster trigonometry functions
/// </summary>
public class FastTrig
{

    private Dictionary<float, float> _cachedSins = new Dictionary<float, float>();
    private Dictionary<float, float> _cachedCoss = new Dictionary<float, float>();

    private const float _cacheStep = 0.01f;
    private float _factor = Mathf.PI / 180.0f;

    public FastTrig() {
        for (float angleDegrees = 0; angleDegrees <= 360.0;
            angleDegrees += _cacheStep) {
            float angleRadians = angleDegrees * _factor;
            _cachedSins.Add(angleDegrees, Mathf.Sin(angleRadians));
        }

        for (float angleDegrees = 0; angleDegrees <= 360.0;
           angleDegrees += _cacheStep) {
            float angleRadians = angleDegrees * _factor;
            _cachedCoss.Add(angleDegrees, Mathf.Cos(angleRadians));
        }
    }

    public float CacheStep {
        get {
            return _cacheStep;
        }
    }

    public float Sin(float angleDegrees) {
        float value;
        if (_cachedSins.TryGetValue(angleDegrees, out value)) {
            return value;
        }
        return 0.0f;
    }

    public float Cos(float angleDegrees) {
        float value;
        if (_cachedCoss.TryGetValue(angleDegrees, out value)) {
            return value;
        }
        return 0.0f;
    }
}
