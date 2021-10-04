using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AjustarContent : MonoBehaviour
{
    [SerializeField, Tooltip("O que vai mudar de tamanho")]
    RectTransform contenct;
    [Space]
    [SerializeField, Tooltip("Valor minimo da lista Sem alterar o tamanho")]
    protected int   volume; ///Valor minimo para nao alterar tamanho
    [SerializeField, Tooltip("Valor de crescimento para cada item da lista")]
    protected float tamanho;///Valor do tamanho
    [Space]
    [SerializeField]
    protected Scrollbar scroll;

    protected Vector2 _default = new Vector2();

    public int Volume { get { return volume; } }

    int current = 0;

    protected void Awake()
    {
        _default = contenct.sizeDelta;
    }

    /*protected IEnumerator Start()
    {
        yield return new WaitForSeconds(0);

        RectTransform rt = contenct;
        rt.sizeDelta = _default;
    }*/

   protected void OnEnable()
    {
        RectTransform rt = contenct;
        rt.sizeDelta     = _default;

        current = 0;
    }

    public void Alterar(int count)
    {
        if (scroll!=null)
            scroll.value = 1;

        if (contenct==null)
            return;

        RectTransform rt = contenct;
        rt.sizeDelta     = _default;

        //current = 0;

        print("Contenct: " + count);

        if (count >= volume)
        {
           // print("Contenct: Aumentou");
            rt.sizeDelta = new Vector2(0, contenct.rect.height + (tamanho * count));
        }
        //else
        //{
        //    //print("Contenct: Reseta");
        //    RectTransform rt = contenct;
        //    rt.sizeDelta = _default;

        //}
    }

    public void Alterar(float count)
    {
        if (scroll != null)
            scroll.value = 1;

        if (contenct == null)
            return;

        RectTransform rt = contenct;
        rt.sizeDelta = _default;

        //current = 0;

        print("Contenct: " + count);

        if (count >= volume)
        {
            // print("Contenct: Aumentou");
            rt.sizeDelta = new Vector2(0, contenct.rect.height + (tamanho * count));
        }
        //else
        //{
        //    //print("Contenct: Reseta");
        //    RectTransform rt = contenct;
        //    rt.sizeDelta = _default;

        //}
    }

    public void Alterar(bool aumenta,bool resetCurrent=false)
    {
        if (scroll != null)
            scroll.value = 1;

        if (contenct == null)
            return;

        RectTransform rt = contenct;
        /*rt.sizeDelta     = _default;*/

        if (resetCurrent)
            current = 0;

        if (aumenta)
            current++;
        else
        {
            current--;

            if (current < 0)
                current = 0;
        }

        //print("Contenct: " + current);

        if (current >= volume)
        {
            //print("Contenct: Aumentou");
            rt.sizeDelta = new Vector2(0, contenct.rect.height + (tamanho/* * current*/));
        }        
        //else
        //{
        //    //print("Contenct: Reseta");
        //    RectTransform rt = contenct;
        //    rt.sizeDelta = _default;

        //}
    }
}
