
using UnityEngine;

public class Oscillator
{

    public float inf, sup;

    public float amplitude;

    private const float pi180 = 0.017453292f;

    private const float halfCicle =        0.5f;
    private const float oneCicle  =        1.0f;
    private const float quarterCicle =     0.25f;
    private const float treeQuarterCicle = 0.75f;
    private const float eighthCicle      = 0.125f;

    private float step;

    private float value;
    private float cicles;

    public Oscillator(float inf = 0, float sup = 359, float amplitude = 1, float step = 1)
    {
        this.inf = inf;
        this.sup = sup;
        this.amplitude = amplitude;

        this.step = step;

        value = inf;
        cicles = 0f;

    }

    //Mathf.Sin(angle in radians)  !!!! converting to grads: rads * pi /180  (pi/180 = 0.017453292)
    
    public float Next()
    {
        float result = Mathf.Sin(value * pi180) * amplitude;
        value += step;
        if (value > sup)
        {
            value = inf;
            cicles = cicles + 1;
        }
        return result;
    }

    public void Reset()
    {
        this.value = inf;
        cicles = 0f;
    }

    public float GetValue()
    {
        return value;
    }
    public float GetCicle()
    {
        return (cicles * 360f + value) / 360f;
    }
    public float Pi180()
    {
        return pi180;
    }
    public float HalfCicle()
    {
        return halfCicle;
    }
    public float OneCicle()
    {
        return oneCicle;
    }
    public float QuarterCicle()
    {
        return quarterCicle;
    }
    public float TreeQuarterCicle()
    {
        return treeQuarterCicle;
    }
    public float EighthCicle()
    {
        return eighthCicle;
    }
}
