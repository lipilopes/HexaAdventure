using UnityEngine.UI;
using UnityEngine;

public class ClearImage : MonoBehaviour
{
    [SerializeField] bool showInMobile=true;

    [SerializeField] float _time;

    [SerializeField]  KeyCode _closeButton;

    [SerializeField] int[] blockFases;

    [SerializeField] Image[] imgs;    

    bool active = true;

    void Start()
    {
        bool isMobile = false;


        if (GameManagerScenes._gms != null)
        {
            isMobile = GameManagerScenes._gms.IsMobile;

            if (blockFases.Length != 0)
                foreach (var fase in blockFases)
                {
                    if (fase == GameManagerScenes._gms.FaseAtual)
                    {
                        Debug.LogError("Clear Image Dont Show in this Fase!!!");

                        _time = 0;

                        active = true;

                        Desativa();
                        return;
                    }
                }
        }
        else
        {
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
            isMobile = true;
#endif
        }

            if (isMobile)//E mobile
        {
            if (!showInMobile)
            {
                _time = 0;

                Desativa();

                Destroy(gameObject);
            }
            else//não e mobile
            {
                if (!showInMobile)
                {
                    active = true;

                    for (int i = 0; i < imgs.Length; i++)
                    {
                        imgs[i].enabled = true;
                    }

                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(_closeButton))
            Image();

        if (_time <= 0)
            return;

            _time -= Time.deltaTime;

            if (_time <= 0)
                Image();       
    }


    private void Image()
    {
        _time = 0;

        active = !active;

       if (GetComponent<Fade>())
        {
            GetComponent<Fade>().StopAll();

            if (!active)
               GetComponent<Fade>().FadeOff();
            else
                GetComponent<Fade>().FadeOn();
        }
        else
        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = active;
        }
    }


    void Desativa()
    {
        active = false;

        for (int i = 0; i < imgs.Length; i++)
        {
            imgs[i].enabled = false;
        }

        if (GetComponent<Fade>())
        {
            GetComponent<Fade>().StopAllCoroutines();
        }
    }
}
