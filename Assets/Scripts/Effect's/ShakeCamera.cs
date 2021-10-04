using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCamera : MonoBehaviour
{
    [SerializeField]
    protected bool _rotation = false;
    [Space]
    [SerializeField,Tooltip("Sem nada e a camera")]
    protected GameObject _whoShake;
    [Space]
    [SerializeField]
    protected int _intensity=1;

    [SerializeField]
    protected float _delay=0.5f;

    [SerializeField]
    protected float _time=1;

    [SerializeField]
    iTween.EaseType _easeType = iTween.EaseType.easeInOutBounce;

    private void OnEnable()
    {
#if UNITY_EDITOR
        StartEffect();
#endif
    }

    public void StartEffect()
    {
        if (_rotation)
            ShakerRotationCamera(_whoShake, _intensity, _delay, _time, _easeType);
        else
            ShakerCamera(_whoShake, _intensity, _delay, _time, _easeType);
    }

    public void ShakerCamera(GameObject who,int intensity,float delay=0.5f,float time=1, iTween.EaseType easeType = iTween.EaseType.easeInOutBounce)
    {
        if (who==null)
            who = Camera.main.gameObject;

        float X = 0.3f * intensity,
              Z = 0.3f * intensity;

        iTween.ShakeRotation(who, iTween.Hash("x", X, "z", Z, "delay", delay, "time", time, "easetype",easeType));
    }

    public void ShakerRotationCamera(GameObject who, int intensity, float delay = 0.5f, float time = 1,iTween.EaseType easeType = iTween.EaseType.easeInOutBounce)
    {
        if (who == null)
            who = Camera.main.gameObject;

        float X = 0.3f * intensity,
              Z = 0.3f * intensity;

        iTween.ShakeRotation(who, iTween.Hash("z", Z, "x", X, "delay", delay, "time", time, "easetype", easeType));
    }
}
