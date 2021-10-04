using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public enum AutoStart
{
   Awake,
   Start,
   OnEnable,
   OnDisable
}

[System.Serializable]
public class _AutoStart
{
    [Tooltip("Quando Ativar o Auto Start")]
    public AutoStart _autoStart   = AutoStart.Awake;
    [Tooltip("Quando Ativar Ja inicia o FadeOFF")]
    public bool _autoStartFadeOff = true;
    [Tooltip("Quando acabar GameObject Sera desativado")]
    public bool _desativeComplete = false;
}

[RequireComponent(typeof(MaskableGraphic))]
public class Fade : MonoBehaviour {

    [Tooltip("Inicia Automaticamente")]
    public bool         autoStart  = false;
    [Tooltip("Lista do auto Start")]
    public _AutoStart[] autoStartList;
    [Space]
    [Tooltip("Cor inicial")]
    public Color atualColor  = Color.white;
    [Tooltip("Cor final")]
    public Color updateColor = new Color(1f, 1f, 1f, 0);

    Color ToColor;

    [Tooltip("tempo de espera para ativar/desativar")]
    public float delay      = 0.5f;
    [Tooltip("tempo para completar o efeito")]
    public float timeToFade = 1f;

    [Tooltip("Efeito")]
    public iTween.EaseType easeType;

    public bool? desativeComplete = null;

    MaskableGraphic graphic;

    [HideInInspector]
    public CanvasGroup canvasGroup;

    WaitForSeconds waitDesative = new WaitForSeconds(0f);

	void Awake ()
    {
        graphic     = GetComponent<MaskableGraphic>();
        canvasGroup = GetComponent<CanvasGroup>();

        if (autoStart)
            StartAutoStart(AutoStart.Awake);
    }

    void Start()
    {
        if (autoStart)
            StartAutoStart(AutoStart.Start);
    }

    void OnEnable()
    {
        StopCoroutine(DesativeCoroutine());

        if (autoStart)
            StartAutoStart(AutoStart.OnEnable);
    }

    void OnDisable()
    {
        if (autoStart)
        {
           StartAutoStart(AutoStart.OnDisable);    
        }   
    }

    void UpdateColor (Color newColor)
    {
        StopCoroutine(DesativeCoroutine());

        //print(gameObject.name + " -UpdateColor()");

        if (canvasGroup!=null)
        {
            canvasGroup.alpha = newColor.a;
        }
        else
        graphic.color = newColor;

        if (desativeComplete != null)
        {
            if (desativeComplete == true)
            {
                if (canvasGroup != null)
                {
                    if (canvasGroup.alpha == ToColor.a)
                    {
                        StartCoroutine(DesativeCoroutine());
                    }
                }
                else
                    if (graphic.color == ToColor)
                    StartCoroutine(DesativeCoroutine());
            }
        }
    }

    IEnumerator DesativeCoroutine()
    {
        //print(gameObject.name + " -Start DesativeCoroutine()");

        yield return waitDesative;

        //print(gameObject.name + " -Over DesativeCoroutine()");

        gameObject.SetActive(false);
        desativeComplete = null;
    }

    public void StopAll()
    {
        StopAllCoroutines();
    }

    public void FadeOff()
    {
        ToColor = updateColor;

        //print(gameObject.name + " FadeOff()");

        iTween.ValueTo(gameObject,
    iTween.Hash("from", atualColor,
                "to", updateColor,
                "time", timeToFade,
                "delay", delay,
                "easytype", easeType,
                "onupdatetarget", gameObject,
                "onupdate", "UpdateColor"));
    }

    public void FadeOn()
    {
        ToColor = atualColor;

        //print(gameObject.name + " FadeOn()");

        iTween.ValueTo(gameObject,
    iTween.Hash("from", updateColor,
                "to", atualColor,
                "time", timeToFade,
                "delay", delay,
                "easytype", easeType,
                "onupdatetarget", gameObject,
                "onupdate", "UpdateColor"));
    }

    void StartAutoStart(AutoStart _case)
    {       
        if (autoStartList.Length > 0)
            foreach (var list in autoStartList)
            {
                if (list._autoStart == _case)
                {
                    desativeComplete = list._desativeComplete;

                    if (list._autoStartFadeOff)
                        FadeOff();
                    else
                        FadeOn();

                    //print(gameObject.name + " AutoStart: " + _case);
                }
            }
    }
}
