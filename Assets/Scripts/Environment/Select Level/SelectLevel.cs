using System.Collections;
using System;
using UnityEngine.UI;
using UnityEngine;

public class SelectLevel : MonoBehaviour
{
    int _maxDemoFase = 5;
    [SerializeField] UnityEngine.UI.Text _nameHeroText;
    [SerializeField] GameObject _loading;
    [SerializeField] GameObject _painel;
    [Space]
    [SerializeField] GameObject _recordPainel;
    [Space]
    [SerializeField] AudioClip _clickCerto;
    [SerializeField] AudioClip _clickErrado;

    [SerializeField] AudioClip _transicaoMisteriosa;
    [SerializeField] AudioClip _transicaoIntensa;
    [Space]
    [SerializeField] Sprite    _blockSpriteFase;
    [Space]
    [Header("Battle Mode")]
    [SerializeField] GameObject _painelBattleMode;
    [Header("Player 1")]
    [SerializeField] GameObject[] _battleModeP1Panel;
    [SerializeField] Image[]      _battleModeP1IconMob;
    [SerializeField] Text[]       _battleModeP1NameMob;
    [SerializeField] Image[]      _battleModeP1ClassMob;
    [Header("Player 2")]
    [SerializeField] GameObject[] _battleModeP2Panel;
    [SerializeField] Image[]      _battleModeP2IconMob;
    [SerializeField] Text[]       _battleModeP2NameMob;
    [SerializeField] Image[]      _battleModeP2ClassMob;
    [Space]
    [SerializeField] BlockLevel[] _buttons;
    [Space]
    [SerializeField,TextArea] string[] _dicasLoading;

    GameManagerScenes _gms;
    AudioSource audioSource;

    WaitForSeconds firstWaitStart   = new WaitForSeconds(0.002f);
    WaitForSeconds loadingWaitStart = new WaitForSeconds(0f);
    WaitForSeconds loadingWaitStartBattleMode = new WaitForSeconds(0.1f);

    public Sprite BlockSprite
    {
        get { return _blockSpriteFase; }
    }

    public static SelectLevel Instante;

    private void Awake()
    {
        if (Instante == null)
            Instante = this;
        else
            Destroy(this);

        audioSource = GetComponent<AudioSource>();

        _gms = GameManagerScenes._gms;
    }

    IEnumerator Start()
    {
        print("START");
        _maxDemoFase = _gms.MaxFaseDemo;
        int fasesCount = _buttons.Length;
        _gms.Paused = false;

        _dicasLoading = GetComponent<XmlSelectScene>().Get();

        if (_dicasLoading.Length>0)
        LoadingText(true, _dicasLoading[UnityEngine.Random.Range(0, _dicasLoading.Length)]);

        _painel.SetActive(false);
        _painelBattleMode.SetActive(false);

        #region Modo Historia
        if (_gms.GameMode == Game_Mode.History)
        {
            yield return firstWaitStart;

            for (int i = 1; i < fasesCount; i++)
            {
                _buttons[i].Block(true);
            }    

            LoadingText(false);
            _painel.SetActive(true);

            string skin = "";

            _nameHeroText.text =_gms.Demo ? "<color=red>Demo</color>\n" : "";

            if (_gms.Adm)
            {
                _nameHeroText.text += "<color=red>Administrador</color>\n";
                skin = "[" + _gms.SkinName() + "]";
            }
            else
                _nameHeroText.text += "";

            if (NeedShowNamePlayer())
                _nameHeroText.text += "Mob:\n" + _gms.HeroName() + skin + "\n<color=green>" + (/*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(1)/* : "Dificuldade"*/) + ": <b>" + _gms.Dificuldade() + "</b></color>";
            else
                _nameHeroText.text += "<color=green>" + (/*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(1)/* : "Dificuldade"*/) + ": <b>" + _gms.DificuldadeString() + "</b></color>";

            if (_gms.GameMode == Game_Mode.History)
            {
                for (int i = 1; i < fasesCount; i++)
                {
                    yield return loadingWaitStart;

                    if (_gms.Demo && !_gms.Adm && i >= _maxDemoFase )
                    {
                        _buttons[i].Block(null);
                        _gms.LoadingBar(XmlMenuInicial.Instance.Get(186)//Carregando
                            + " - " + _gms.NameFase(i) + "..."
                            , i, _maxDemoFase);
                    }                     
                    else
                    {
                        _buttons[i].Block(!_gms.CheckBlockFase(i));

                        _gms.LoadingBar(XmlMenuInicial.Instance.Get(186)//Carregando
                            + " - " + _gms.NameFase(i) + "..."
                            , i, fasesCount - 1);
                    }
                }
            }
        }
        #endregion

        #region Modo Batalha
        if (GameManagerScenes.BattleMode)
        {
            LoadingText(false);
            _painelBattleMode.SetActive(true);

            yield return firstWaitStart;

            int count = _gms._battleModeGamePlay._battleModeMobs.Count;
            for (int i = 0; i < count; i++)
            {
                int Count = 0,
                    idMob = _gms._battleModeGamePlay._battleModeMobs[i]._idMob,
                    time  = _gms._battleModeGamePlay._battleModeMobs[i]._Time;

                Count = time == 0 ? _battleModeP1Panel.Length : _battleModeP2Panel.Length;


                yield return loadingWaitStartBattleMode;

                _gms.LoadingBar(XmlMenuInicial.Instance.Get(186)//Carregando
                    + " - " + _gms.HeroName(idMob) + "..."
                    , i, count-1);

                for (int j = 0; j < Count; j++)
                {
                    if (time == 0)
                    {
                        if (!_battleModeP1Panel[j].activeInHierarchy)
                        {
                            _battleModeP1Panel[j].SetActive(true);
                            _battleModeP1NameMob[j].text = _gms.HeroName(idMob);
                            _battleModeP1IconMob[j].sprite = _gms.SpritePerfil(idMob);
                            _battleModeP1ClassMob[j].sprite = _gms.SpriteClass(idMob);
                            break;
                        }
                    }
                    else
                    {
                        if (!_battleModeP2Panel[j].activeInHierarchy)
                        {
                            _battleModeP2Panel[j].SetActive(true);
                            _battleModeP2NameMob[j].text = _gms.HeroName(idMob);
                            _battleModeP2IconMob[j].sprite = _gms.SpritePerfil(idMob);
                            _battleModeP2ClassMob[j].sprite = _gms.SpriteClass(idMob);
                            break;
                        }
                    }
                }
            }

            yield return new WaitForSeconds(1);

            //LoadingText(true, _dicasLoading[UnityEngine.Random.Range(0, _dicasLoading.Length)]);

//#if UNITY_EDITOR
//           UnityEngine.Events.UnityAction[] _t = { () => _gms.LoadLevel(0) };

//            _gms.QuestionPainel(
//                    "<b><color=red>Erro, Caso Continue Unity Ira Travar, Faça build para testar!!!</color></b>",
//                "Voltar",
//                "Voltar", _t, _t);

//            Debug.LogError("<b><color=red>Erro, Caso Continue Unity Ira Travar, Faça build para testar!!!</color></b>");

//            yield break;
//#endif
            _gms.LoadLevel("GamePlay", XmlMenuInicial.Instance.Get(203)/*Modo Batalha*/ + " - " + XmlMenuInicial.Instance.Get(186) + "...");//Carregando
        }
        #endregion
    }

    public void GoLevel(int level)
    {
        if (_gms.CheckMobBanned() ||
            _gms.CheckMobBlocked())
        {
            _gms.NewInfo(
                _gms.AttDescriçãoMult(
                     /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(56)//: "<color=red>{0} não esta qualificado para tal!!!</color>"
                     , _gms.HeroName()), 
                1.5f);

            UnityEngine.Events.UnityAction[] _t = { () => _gms.ChangePlayer() };

            _gms.QuestionPainel(
              /* !_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(57) /*: "Trocar mob Aleatoriamente, para um qualificado??"*/,
               /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(50) /*: "Sim"*/,
             /*  !_gms.IsMobile ? */XmlMenuInicial.Instance.Get(13)/* : "Não"*/,
                _t, 
                null);

            Audio(false);
            return;
        }
        if (_gms.CheckSkinBlocked())
        {
            _gms.NewInfo(_gms.AttDescriçãoMult(
                ("<color=red>" + XmlMenuInicial.Instance.Get(58) + "</color>" //"<color=red>A skin {0},nao esta qualificado para tal!!!</color>"
                )
                , _gms.SkinName()), 1.5f);

            UnityEngine.Events.UnityAction[] _t = { () => _gms.PlayerSkinID = 0 };
            _gms.QuestionPainel(
                _gms.AttDescriçãoMult(
                    /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(59) //: "Não esta qualificado para tal\nTrocar Skin {0}, para a padrão??"
                    ,
                     _gms.SkinName()),
               /* !_gms.IsMobile ? */XmlMenuInicial.Instance.Get(50) //: "Sim"
                ,
               /* !_gms.IsMobile ? */XmlMenuInicial.Instance.Get(13) //: "Não"
                , _t, null);
            Audio(false);
            return;
        }
        if (!_buttons[level]._block || _gms.Adm)
        {
            Audio();

            _gms.FaseAtual = (level);

            AudioClip transicaoF = _gms.TransicaoFase(-1, level);

            if (transicaoF != null)
                _gms.GetComponentInChildren<MusicLevel>().StartMusic(true, transicaoF);

            _painel.SetActive(false);

            LoadingText(true, _dicasLoading[UnityEngine.Random.Range(0, _dicasLoading.Length)]);

//#if UNITY_EDITOR

//            Debug.LogError("<b><color=red>Erro, Caso Continue Unity Ira Travar, Faça build para testar!!!</color></b>");

//            return;
//#endif

            _gms.LoadLevel("GamePlay", _gms.NameFase(level) + " - "+XmlMenuInicial.Instance.Get(186)+"...");//Carregando

            return;
        }

        Audio(false);
    }

    void Audio(bool certo = true)
    {
        if (certo)
            audioSource.clip = _clickCerto;
        else
            audioSource.clip = _clickErrado;

        audioSource.Play();
    }

    public void BackMenuButton()
    {
        Audio();

        _gms.LoadLevel(0);
    }

    public void RecordFase(int level)
    {
        float x = _buttons[level].transform.position.x,
              y = _buttons[level].transform.position.y;


        Vector2 screenPoint = new Vector2(x, y+25);
        string txt          = /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(41) /*: "Desbloquei essa Fase para mais informacoes..."*/;//Desbloquei essa Fase para mais informacoes...

        if (!_buttons[level]._block)
        {
            float recordSeg   = /*_gms._Fases[_gms.PlayerID-1]._Fases[level]._seg*/_gms.FaseSeg(level, 0, false),
                    recordMin = /*_gms._Fases[_gms.PlayerID-1]._Fases[level]._min*/_gms.FaseMin(level, 0, false);

            bool complete        = _gms.Mob[_gms.PlayerID - 1]._Fases[_gms._dlf].Fase[level]._complete,
                  completeKill   = _gms.Mob[_gms.PlayerID - 1]._Fases[_gms._dlf].Fase[level]._completeKill;
            //bool? completePortal = _gms.Mob[_gms.PlayerID - 1]._Fases[_gms._dlf].Fase[level]._completePortal;

            int completePortal = _gms.Mob[_gms.PlayerID - 1]._Fases[_gms._dlf].Fase[level]._completePortal;

            string recordTxt = "",
                   completeTxt = "",
                   kill = "",
                   portal = "",
                   record = (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(7)/* : "RECORD"*/);

            if (complete)
            {
                if (recordSeg != -1)
                {
                    string seg = "",
                           min = "";

                    if (recordSeg <= 9)
                        seg = "0" + recordSeg;
                    else
                        seg = recordSeg.ToString("F0");

                    if (recordMin < 0)
                        recordMin = 0;

                    if (recordMin <= 9)
                        min = "0" + recordMin;
                    else
                        min = recordMin.ToString("F0");

                    if (recordMin < 60)
                        recordTxt = record + "\n<b>" + min + "</b> : <b>" + seg + "</b>";
                    else
                        recordTxt = record + "\n<b><color=black>"+ (/*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(42)/* : "Time Over!!!"*/) +" </color></b>";//Time Over!!!
                }
                else
                    recordTxt = record + "\n<b>"+ (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(43) /*: "Sem dados"*/) + "</b>";//Sem dados


                if (completeKill)
                    kill = "\n<color=green><size=25>✓</size>    ̷"+ (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(44) /*: "K̷i̷l̷l̷"*/) + "</color>";//K̷i̷l̷l̷
                else
                    kill = "\n<color=red><size=25><b>x</b></size>   "+ (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(45) /*: "K͟i͟l͟l͟"*/) + "</color>";//K͟i͟l͟l͟

                if (completePortal == 1)//true)
                    portal = "\n   <color=green><size=25>✓</size> " + (/*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(46) /*: "P̷o̷r̷t̷a̷l̷"*/) + "</color>";//P̷o̷r̷t̷a̷l̷
                if (completePortal == 0)//false)
                    portal = "\n     <color=red><size=25><b>x</b></size>   "+ (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(47) /*: "P͟o͟r͟t͟a͟l͟"*/) + "</color>";//P͟o͟r͟t͟a͟l͟


                completeTxt = kill + portal;

            }
            else
                recordTxt = (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(48) /*: "Você não completou essa fase!!!"*/);//Voce nao completou essa fase!!!


            txt = recordTxt + completeTxt;
        }

        _recordPainel.transform.position = screenPoint;
        _recordPainel.GetComponentInChildren<UnityEngine.UI.Text>().text = txt;

        Invoke("ShowRecordFase", 0.1f);
    }

    void ShowRecordFase()
    {
        _recordPainel.GetComponent<Animator>().SetBool("Show", true);
    }

    public void ClearRecordFase()
    {
        _recordPainel.GetComponent<Animator>().SetBool("Show", false);
    }

    bool NeedShowNamePlayer()
    {
        if (_gms.Adm)
            return true;

        int  count   = _gms.PlayerCount;
        int  current = 0;
        int  need    = 1;
        bool active  = false;

        for (int i = 1; i < count; i++)
        {
            if (!_gms.CheckMobBlocked(i))
            {
                current++;

                if (current == need)
                    break;
            }
        }

        active = need <= current;

        print("NeedShowNamePlayer need(" + need + ") / current(" + current + ") " + active);

        return active;
    }

    void LoadingText(bool active,string txt= "L O A D I N G...")
    {
        if (txt == "L O A D I N G...")
            txt = XmlMenuInicial.Instance.Get(186)+"...";

        _loading.GetComponent<UnityEngine.UI.Text>().text = txt;
        _loading.SetActive(active);
    }
}
