using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Informacao
{
    public string _infos;
    public float _times;
    public bool _simbols = false;
}

public class InfoTable : MonoBehaviour
{
    public static InfoTable Instance;
    [SerializeField] GameObject table;
    [SerializeField] float _time;

    [SerializeField] bool simbol = false;//simbolo que representa que tem mais mensagens
    Text textTable;

    [SerializeField] bool open;

    public List<Informacao> Info;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        textTable = table.GetComponentInChildren<Text>();  
    }

    private void Update()
    {
        if (!table.GetComponent<Animator>().GetBool("Show") && Info.Count == 0)
            return;

        if (open)
        {
            OpenInf();
            open = false;
        }

        ShowTable();
    }

    void ShowTable()
    {
        if (_time > 0 && table.GetComponent<Animator>().GetBool("Show"))
        {
            _time -= Time.deltaTime;

            if (_time <= 0)
            {
                CloseInfo();

                return;
            }

            if (!simbol)
            {
                if (Info.Count > 0)
                {
                    for (int i = 0; i < Info.Count - 1; i++)
                    {
                        if (!Info[i]._simbols)
                        {
                            Info[i]._infos = Info[i]._infos + "\n...";
                            Info[i]._simbols = true;
                        }
                    }
                }

                if (Info[0] != null)
                    textTable.text = Info[0]._infos;

                simbol = true;
                return;
            }
        }

        //if (table.GetComponent<CanvasGroup>().alpha == 0)
        //{
            //if (Info.Count >= 1 && table.GetComponent<Animator>().GetBool("Show") == false)
            //{
            //    textTable.text = Info[0]._infos;

            //    _time          = Info[0]._times;

            //    simbol         = false;

            //    table.GetComponent<Animator>().SetBool("Show", true);

            //    return;
            //}
            //else
            //if (Info.Count < 1)
            //{
            //    table.GetComponent<Animator>().SetBool("Show", false);
            //}
        //}
    }

    void OpenInf()
    {
        if (Info.Count >= 1 && table.GetComponent<Animator>().GetBool("Show") == false)
        {
            textTable.text = Info[0]._infos;

            _time          = Info[0]._times;

            simbol         = false;

            table.GetComponent<Animator>().SetBool("Show", true);
        }     
    }

    public void CloseInfo(bool click = false)
    {
        table.GetComponent<Animator>().SetBool("Show", false);

        if (click)
            GetComponent<ButtonManager>().ClickAudio();

        //textTable.text = "";

        simbol = false;

        if (Info.Count >= 1)
        {
            if (Info[0] != null)
                Info.Remove(Info[0]);

            Invoke("OpenInf", 0.10f);
        }
    }

    public void NewInfo(string inf, float time)
    {
        if (inf == "" || inf == null)
            return;

        if (CheckInfos(inf))
            return;

        simbol          = false;

        Informacao info = new Informacao();

        info._infos   = inf;
        info._times   = time+0.15f;//time anim
        info._simbols = false;

        Info.Add(info);

        Invoke("OpenInf", 0.10f);
    }

    /// <summary>
    /// check mensagens repitidas
    /// </summary>
    /// <param name="inf"></param>
    /// <returns></returns>
    bool CheckInfos(string inf)
    {
        for (int i = 0; i < Info.Count; i++)
            if (Info[i]._infos.Contains(inf))
                return true;

        return false;
    }
}
