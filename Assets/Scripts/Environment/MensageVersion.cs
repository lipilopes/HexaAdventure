using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MensageVersionList
{   
    public string _v;
    [Tooltip("Não utilize os pontos")]
    public int _version;
    [TextArea(25,999999)]
    public string[] _mensageV = { "New","Fixed", "Balanceamento"};
}

public class MensageVersion : MonoBehaviour
{
    [Space, SerializeField]
    protected CodesHistory[] _codeWords;
    [Space]
    public static MensageVersion Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(Instance);

    }

    public List<MensageVersionList> _mensageVersion;

    [SerializeField] int _version;

    [SerializeField] Dropdown _dropdown;
    [SerializeField] Canvas _canvas;
    [SerializeField] Text   _txtCanvas;

    WaitForSeconds wait = new WaitForSeconds(0.01f);

    private void Start()
    {
        if (GameManagerScenes._gms.Adm)
            StartShowMensage();

        _version = Convert.ToInt32(ChangeVersion(Application.version, ".", ""));

        _dropdown.options.Clear();

        for (int i = 0; i < _mensageVersion.Count; i++)
            _dropdown.options.Add(new Dropdown.OptionData() { text = (XmlMenuInicial.Instance!=null ? XmlMenuInicial.Instance.Get(85) : "Versão:")+ " " + _mensageVersion[i]._v });

    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void StartShowMensage()
    {   
        if (PlayerPrefs.HasKey("Version"))
        {
            print("Existe Salvo uma Ultima Versão ("+ PlayerPrefs.GetString("Version") + ")");

            int V = Convert.ToInt32(ChangeVersion(PlayerPrefs.GetString("Version"), ".", ""));

            if (V != _version)
            {
                Debug.LogWarning("Ultima Versão é a antiga");

                if (GetV(_version)!="")
                    SelectDropdown(_version);
                else
                {                    
                    _txtCanvas.text = "          Nota da <b> NOVA </b> Atualização(<b> " + Application.version + " </b>):\n*Apenas Alguns ajustes internos.";
                    CloseMensage(false);
                    GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);
                    _dropdown.GetComponentInChildren<Text>().text= (XmlMenuInicial.Instance != null ? XmlMenuInicial.Instance.Get(85) : "Versão:") + " " + Application.version;
                }
            }
            else
            {
                Debug.LogWarning("Notas Da Atualização não apareceu pois ja foi visualizada");
                CloseMensage(true);
                return;
            }
        }
        else
        {
            //print("Não Existe Salvo a Ultima Versão");
            SelectDropdown((_mensageVersion.Count - 1));
        }

        SalveVersion();
    }

   public void SalveVersion()
    {
        PlayerPrefs.SetString("Version", Application.version);
    }
    public void SelectDropdown(int value)
    {
        if (value==-1)
            value = _mensageVersion.Count - 1;

        _dropdown.value = value;

        StartCoroutine(ShowMensageCoroutine(value));
    }

    IEnumerator ShowAllMensagensCoroutine()
    {
        string msg = "";

        int V, count = _mensageVersion.Count;

        for (int i = count - 1; i >= 0; i--)
        {
            V = _mensageVersion[i]._version;

            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + GetV(V) + "</b>)", 0);

            yield return wait;

            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + GetV(V) + "</b>)", UnityEngine.Random.Range(0.1f, 0.9f));

            msg +=
            "           Nota da Atualização (<b>" + GetV(V) + "</b>):" +
            "\n\n" + GetNew(V) +
            "\n\n" + GetFixed(V) +
            "\n\n" + GetBalance(V) +
            "\n" /*+ GetBalanceBattle(V) + "\n"*/;

            _txtCanvas.text = msg;
            GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);

            yield return wait;

            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + GetV(V) + "</b>)", 1);
        }

        GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);

    }

    IEnumerator ShowMensageCoroutine(int V)
    {
        if (V < _mensageVersion.Count)
        {
            CloseMensage(false);

            string
                _v = CheckAndChangeWords(_mensageVersion[V]._v),
                _n = CheckAndChangeWords(_mensageVersion[V]._mensageV[0]),
                _f = CheckAndChangeWords(_mensageVersion[V]._mensageV[1]),
                _b = CheckAndChangeWords(_mensageVersion[V]._mensageV[2])/*,
               _bB = GetBalanceBattle(V)*/
            ;

            int i = _mensageVersion.Count - 1;

            //print("ShowMensageCoroutine("+V+")");

            string
            msg = "           Nota da"+(V == _mensageVersion.Count-1 ? " <b>NOVA</b>" : "")+" Atualização (<b>" + _v + "</b>):";
            _txtCanvas.text = msg;
            GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);
            yield return wait;
            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>): Get", _mensageVersion.Count / i);

            msg += "\n\n" + _n;
            _txtCanvas.text = msg;
            GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);
            yield return wait;
            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>):  _New_", _mensageVersion.Count / i);

            msg += "\n\n" + _f;
            _txtCanvas.text = msg;
            GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);
            yield return wait;
            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>):  _Fixed_", _mensageVersion.Count / i);

            msg += "\n\n" + _b;
            _txtCanvas.text = msg;
            yield return wait;
            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>):  _Balanciamento_", _mensageVersion.Count/i);

            /* if (_mensageVersion[i]._mensageV.Length >= 4)
             {
                 if (_bB == "")
                     _bB = _mensageVersion[i]._mensageV[3];
                 msg += "\n\n" + _bB;
                 _txtCanvas.text = msg;
                 yield return wait;
                 GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>):  _Balanciamento <color=black><b>BATALHA</b></color>_", 0);
             }*/

            GetComponent<AjustarContent>().Alterar(_txtCanvas.preferredHeight);

            GameManagerScenes._gms.LoadingBar("Nota da Atualização (<b>" + _v + "</b>)", 1);
        }
    }

    public void CloseMensage(bool close)
    {
        if (close && GetComponent<Fade>().canvasGroup.alpha != 0)
            GetComponent<Fade>().FadeOff();
        else
            if (!close && GetComponent<Fade>().canvasGroup.alpha == 0)
            GetComponent<Fade>().FadeOn();

        GetComponent<CanvasGroup>().blocksRaycasts = !close;
    }

    string GetNew(int V)
    {
        foreach (var i in _mensageVersion)
        {
            if (i._version == V)
            {
                return CheckAndChangeWords(i._mensageV[0]);
            }
        }

        return "";
    }

    string GetFixed(int V)
    {
        foreach (var i in _mensageVersion)
        {
            if (i._version == V)
            {
                return CheckAndChangeWords(i._mensageV[1]);
            }
        }

        return "";
    }

    string GetBalance(int V)
    {
        foreach (var i in _mensageVersion)
        {
            if (i._version == V)
            {
                return CheckAndChangeWords(i._mensageV[2]);
            }
        }

        return "";
    }

    string GetBalanceBattle(int V)
    {

        foreach (var i in _mensageVersion)
        {
            if (i._version == V && i._mensageV.Length>=4)
            {
                return CheckAndChangeWords(i._mensageV[3]);
            }
        }

        return "";
    }

    string GetV(int V)
    {
        foreach (var i in _mensageVersion)
        {
            if (i._version == V)
            {
                return CheckAndChangeWords(i._v);
            }
        }

        return "";
    }

    public string ChangeVersion(string descricao, string change, string _New, string _Else = "")
    {
        if (descricao.Contains(change))
        {
            string _old = descricao;

            descricao = _old.Replace(change, _New);
        }
        else
            descricao = _Else;

        return descricao;
    }

    protected string CheckAndChangeWords(string list)
    {
        string _S = list;

        if (_S.Length > 0 && _S != null)
        //foreach (var i in list)
        {
            foreach (var c in _codeWords)
            {
                if (c != null && c._key != "")
                    if (_S.Contains(c._key))
                    {
                        string _old = _S;

                        _S = _old.Replace(c._key, c._changer);
                    }
            }
        }

        return _S;
    }
}
