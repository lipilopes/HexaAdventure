using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]
class BattleModePlayers
{   
    public Button _buttonPlayer;
    public Button _buttonPlayerOptions;
    /// <summary>
    /// Is a Player or IA?
    /// </summary>
    public bool  _isAPlayer = true;
    public int[] _mobsSelect;
    [Space]
    public int[] _mobsRespawX;
    public int[] _mobsRespawY;
    [Space]
    public BattleModeMobsSelections[] _mobsSelectionsPlayer;
}

[System.Serializable]
class BattleModeMobsSelections
{
    public CanvasGroup _canvasGroup;
    public Button[]    _mobSelection      = new Button[3];
    public Image[]     _classMobSelection = new Image [3];
    public Text[]      _nameMobSelection  = new Text  [3];
}

[System.Serializable]
class BattleModeBalance
{   
    public string _nameMob;
    public int    _valueStatus;
}

public class BattleModePanel : MonoBehaviour
{
    GameManagerScenes _gms;
    MenuScene _mS;

    [SerializeField] bool loadComplete= false;

    /// <summary>
    /// Mobs  Banneds
    /// </summary>
    List<int> mobsBanneds = new List<int>();

    /// <summary>
    /// Mobs Active In List / PlayerList
    /// </summary>
    List<int> mobsActive = new List<int>();

    /// <summary>
    /// Mobs Active In List dont Banneds
    /// </summary>
    List<int> mobsActiveAll = new List<int>();

    [Header("Mode")]
    /// <summary>
    /// BattleMode 1x1 / 2x2 / 3x3 ...
    /// </summary>
    [SerializeField] int _battleMode = 0;
    int _battleModeCount = 3;

    /// <summary>
    /// Dont use It
    /// </summary>
    [SerializeField] int playerSelectDontUse = 0;
    int _playerSelect
    {
        get { return playerSelectDontUse; }
        set
        {
            playerSelectDontUse = value;

            if (value      <= -1             ||
                value      >= _player.Length ||
               _battleMode >= _player[value]._mobsSelectionsPlayer.Length ||
               _mobSelect  >= _player[value]._mobsSelectionsPlayer[_battleMode]._mobSelection.Length)
            {
                _selectionMobObj.GetComponent<CanvasGroup>().alpha = 0;
                return;
            }

            LoadSelectMobs(_player[value]._isAPlayer);

            _selectionMobObj.GetComponent<CanvasGroup>().alpha = 1;

            float x = _player[value]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.x,
                  y = _player[value]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.y,
                  z = _player[value]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.z;

            Vector3 screenPoint = new Vector3(x, y, z);
            _selectionMobObj.transform.position = screenPoint;

            Vector2 size =(_battleMode == 0 ? new Vector2(226,203) : new Vector2(180, 156.6f));
            _selectionMobObj.GetComponent<RectTransform>().sizeDelta = size;
        }
    }

    /// <summary>
    /// Dont use It
    /// </summary>
    [SerializeField] int mobSelectDontUse = 0;
    int _mobSelect
    {
        get { return mobSelectDontUse; }
        set
        {
            mobSelectDontUse = value;

            if (value         <= -1             ||
                _playerSelect >= _player.Length ||
                _battleMode   >= _player[_playerSelect]._mobsSelectionsPlayer.Length ||
                value         >= _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection.Length)
            {
                _selectionMobObj.GetComponent<CanvasGroup>().alpha = 0;
                return;
            }

            _selectionMobObj.GetComponent<CanvasGroup>().alpha = 1;

            float x = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[value].transform.position.x,
                  y = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[value].transform.position.y,
                  z = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[value].transform.position.z;

            Vector3 screenPoint = new Vector3(x, y, z);
            _selectionMobObj.transform.position = screenPoint;

            Vector2 size = (_battleMode == 0 ? new Vector2(226, 203) : new Vector2(180, 156.6f));
            _selectionMobObj.GetComponent<RectTransform>().sizeDelta = size;
        }
    }

    [Header("Player's")]
    [SerializeField] BattleModePlayers[] _player = new BattleModePlayers[2];

    //[SerializeField] BattleModeMobsSelections[] _mobsSelectionsPlayer1;
    //[SerializeField] Button _buttonPlayer1;
    //[SerializeField] Button _buttonPlayer1Options;
    ///// <summary>
    ///// Is a Player1 or IA?
    ///// </summary>
    //[SerializeField] bool   _isAPlayer1=true;

    //[Header("Player 2")]
    //[SerializeField] BattleModeMobsSelections[] _mobsSelectionsPlayer2;
    //[SerializeField] Button _buttonPlayer2;
    //[SerializeField] Button _buttonPlayer2Options;
    ///// <summary>
    ///// Is a Player 2 or IA?
    ///// </summary>
    //[SerializeField] bool   _isAPlayer2=false;

    [Header("Seletion Mob")]
    [SerializeField] GameObject _selectionMobObj;
    [SerializeField] Sprite _selectionMobBanned;
    [SerializeField] Sprite _selectionMobBlocked;
    [SerializeField] Sprite _selectionMobAbsent;
    [Space]
    [SerializeField] GameObject[] _selectionMobList = new GameObject[0];

    [Header("Button")]
    [SerializeField] Button _buttonChangeBattleMode;

    [Header("Atribute")]
    [SerializeField] GameObject _painelShowAtribute;
    [SerializeField] Text    _textMobShowAtribute;
    [SerializeField] Slider  _sliderHpMobShowAtribute;
    [SerializeField] Slider  _sliderAtkMobShowAtribute;
    [SerializeField] Slider  _sliderSpeedMobShowAtribute;

    [Header("Text")]//Translate
    [SerializeField] Text _textBattleModeSelecao;
    [SerializeField] Text _textBattleModeConfig;
    [Space]//Conf
    [SerializeField] Text _textButtonToggleUniqueSelection;
    [SerializeField] Text _textButtonToggleMobsPassive;
    [SerializeField] Text _textButtonToggleRoundMode;
    [SerializeField] Text _textButtonSliderStatusHp;
    [SerializeField] Text _textButtonSliderStatusDamage;
    [SerializeField] Text _textButtonSliderStatusCriticalChance;
    [SerializeField] Text _textButtonSliderResistenceFire;
    [SerializeField] Text _textButtonSliderResistencePoison;
    [SerializeField] Text _textButtonSliderResistencePetrify;
    [SerializeField] Text _textButtonSliderResistenceBleed;

    bool att = false;

    WaitForSeconds _loadingSelectionModePanel = new WaitForSeconds(0.08f);
    bool uniqueSelection = false;

    private void Start()
    {
        _gms = GameManagerScenes._gms;

        _mS = MenuScene.Instance;        

        StartCoroutine(LoadTranslate());
        LoadSelectionModePanel();

        _mobSelect = -1;
        FindMobSelect();

        foreach (var item in _player)
        {
            //Clear All Canvas
            int countCanvas = item._mobsSelectionsPlayer.Length;
            for (int i = 0; i < countCanvas; i++)
            {
                item._mobsSelectionsPlayer[i]._canvasGroup.alpha = 0;
                item._mobsSelectionsPlayer[i]._canvasGroup.blocksRaycasts = false;
            }

            //active Canvas
            item._mobsSelectionsPlayer[_battleMode]._canvasGroup.alpha = 1;
            item._mobsSelectionsPlayer[_battleMode]._canvasGroup.blocksRaycasts = true;

            //Change Sprite
            int countMob = item._mobsSelectionsPlayer[_battleMode]._mobSelection.Length;
            for (int i = 0; i < countMob; i++)
            {
                if (item._mobsSelect[i] != -1)
                {
                    item._mobsSelectionsPlayer[_battleMode]._mobSelection[i].GetComponent<Image>().sprite =
                    _gms.SpritePerfil(item._mobsSelect[i]);

                    item._mobsSelectionsPlayer[_battleMode]._nameMobSelection[i].text = _gms.HeroName(item._mobsSelect[i]);
                }
                else
                {
                    item._mobsSelectionsPlayer[_battleMode]._mobSelection[i].GetComponent<Image>().sprite = _selectionMobAbsent;
                    item._mobsSelectionsPlayer[_battleMode]._nameMobSelection[i].text = "-";
                }
            }
        }

        StartConfig();
    }

    void OnDisable()
    {
        StopCoroutine(EUpdateShowAtributeMob());
    }

    IEnumerator LoadTranslate()
    {
        while (XmlMenuInicial.Instance == null)
            yield return _loadingSelectionModePanel;

        _textBattleModeSelecao.text = XmlMenuInicial.Instance.Get(205);//Seleção de Mob's

        //_textBattleModeConfig.text = XmlMenuInicial.Instance.Get(205);

        int playerCount = _player.Length;
        for (int i = 0; i < playerCount; i++)
        {
            _player[i]._buttonPlayer.GetComponentInChildren<Text>().text =
            XmlMenuInicial.Instance.Get(_player[i]._isAPlayer ? 206/*Jogador*/ : 207/*Computer*/) + "\n" + (1 + i);
        }       
    }

    public void LoadSelectionModePanel()
    {
        if (att)
            return;

        att = true;

        StartCoroutine(LoadSelectionModePanelCoroutine());
    }
    public IEnumerator LoadSelectionModePanelCoroutine()
    {
        while (GameManagerScenes._gms == null)
            yield return _loadingSelectionModePanel;

        _gms = GameManagerScenes._gms;

        _gms.Balance(_gms.Adm);

        int count = _selectionMobList.Length;
        string nameHero = "";

        mobsActive.Clear();

        _textMobShowAtribute.text = "";

        _sliderHpMobShowAtribute.value    = 0;
        _sliderAtkMobShowAtribute.value   = 0;
        _sliderSpeedMobShowAtribute.gameObject.SetActive(false)/*.value = 0*/;

        for (int i = 0; i < count; i++)
        {
            int id = i;
            //Registra Button
            _selectionMobList[id].GetComponent<Button>().onClick.AddListener(() => BattleModeSelectMob(id));

            //
           EventTrigger trigger = _selectionMobList[id].GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback = new EventTrigger.TriggerEvent();
            entry.callback.AddListener(new UnityEngine.Events.UnityAction<BaseEventData>((data) => ShowAtributeMob(id)));
            trigger.triggers.Add(entry);

            //classe
            _selectionMobList[id].GetComponentsInChildren<Image>()[1].enabled = false;

            if (id < _gms.PlayerCount)
            {
                if (_gms.CheckMobBanned(id))
                {
                    _selectionMobList[id].GetComponent<Image>().sprite = _selectionMobBanned;
                    _selectionMobList[id].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));

                    nameHero = _gms.HeroName(id) + "- [<color=black><b>" + (XmlMenuInicial.Instance.Get(61) /*: "Banido"*/) + "</b></color>]";

                    mobsBanneds.Add(id);
                }
                else
                if (_gms.CheckMobBlocked(id))
                {
                    _selectionMobList[id].GetComponent<Image>().sprite = _selectionMobBlocked;
                    _selectionMobList[id].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));

                    nameHero = /*_gms.HeroName(id) +*/"[<color=magenta><b>" + (XmlMenuInicial.Instance.Get(60) /*: "Bloqueado"*/) + "</b></color>]";

                    mobsActiveAll.Add(id);
                }
                else
                {
                    //classe
                    _selectionMobList[id].GetComponentsInChildren<Image>()[1].enabled = true;
                    _selectionMobList[id].GetComponentsInChildren<Image>()[1].sprite = _gms.SpriteClass(id);


                    _selectionMobList[id].GetComponent<Image>().sprite = _gms.SpritePerfil(id);
                    _selectionMobList[id].GetComponent<Button>().onClick.AddListener(() => _mS.Audio());

                    mobsActive.Add(id);
                    mobsActiveAll.Add(id);

                    nameHero = _gms.HeroName(id);
                }
            }
            else
            {
                _selectionMobList[id].GetComponent<Image>().sprite = _selectionMobAbsent;
                _selectionMobList[id].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));

                nameHero = "???";
            }

            _gms.LoadingBar(nameHero + "\n" + XmlMenuInicial.Instance.Get(186)/*loading*/ + "...", id, count - 1);

            yield return _loadingSelectionModePanel;
        }

        StartCoroutine(EUpdateShowAtributeMob());
        loadComplete = true;

        yield return new WaitForSeconds(2);       
    }

    void LoadSelectMobs(bool player)
    {
        var list = mobsActive;

        if (!player)
            list = mobsActiveAll;

        int count = _selectionMobList.Length;
        for (int i = 0; i < count; i++)
        {
            if (i < _gms.PlayerCount)
            {
                _selectionMobList[i].GetComponentsInChildren<Image>()[1].enabled = false;
                if (player && _gms.CheckMobBanned(i) ||
                    mobsBanneds.Contains(i))
                {
                    _selectionMobList[i].GetComponent<Image>().sprite = _selectionMobBanned;
                    _selectionMobList[i].GetComponent<Button>().onClick.RemoveListener(() => _mS.Audio());
                    _selectionMobList[i].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));
                }
                else
                if (player && _gms.CheckMobBlocked(i) ||
                    !list.Contains(i))
                {
                    _selectionMobList[i].GetComponent<Image>().sprite = _selectionMobBlocked;
                    _selectionMobList[i].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));                   
                }
                else
                {
                    _selectionMobList[i].GetComponent<Image>().sprite = _gms.SpritePerfil(i);
                    _selectionMobList[i].GetComponent<Button>().onClick.AddListener(() => _mS.Audio());
                   _selectionMobList[i].GetComponentsInChildren<Image>()[1].enabled =true;
                }
            }
            else
            {
                _selectionMobList[i].GetComponent<Image>().sprite = _selectionMobAbsent;
                _selectionMobList[i].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));
            }
        }        
    }

    public void BattleModePlay()
    {
        print("BattleModePlay()");

        StartCoroutine(BattleModePlayCoroutine());
    }
    IEnumerator BattleModePlayCoroutine()
    {
        _gms._battleModeGamePlay._battleModeMobs.Clear();

        _gms.LoadingBar(XmlMenuInicial.Instance.Get(186)/*Carregando*/, 0);

        int count = _battleMode,
            countPlayer = _player.Length;

        for (int i = 0; i < countPlayer; i++)
        {
            for (int j = 0; j <= count; j++)
            {
                if (_player[i]._mobsSelect[j] == -1)
                {
                    _playerSelect = i;
                    _mobSelect    = j;                   

                    int mobId = _player[i]._isAPlayer ? mobsActive[(Random.Range(0, mobsActive.Count))] : mobsActiveAll[(Random.Range(0, mobsActiveAll.Count))];

                    _player[i]._mobsSelect[j] = mobId;

                    BattleModeSelectMob(mobId);

                    yield return _loadingSelectionModePanel;

                    _gms.LoadingBar(XmlMenuInicial.Instance.Get(210) + " [ " + _gms.HeroName(_player[i]._mobsSelect[j]) + " ]"/*Selecionando Mob's Aleatoriamente*/, 0.5f);

                    yield return _loadingSelectionModePanel;
                }

                _gms.BattleModeAddMobsInList(
                    _player[i]._mobsSelect[j],
                    0,
                    _player[i]._mobsRespawX[j],
                    _player[i]._mobsRespawY[j],
                    i,
                    _player[i]._isAPlayer);
            }
        }

        UpDateSelectMob();

        yield return new WaitForSeconds(1.5f);

        _gms.FaseAtual = (12);

        _gms.LoadingBar(XmlMenuInicial.Instance.Get(210)/*Selecionando Mob's Aleatoriamente*/, 1);
        _gms.LoadLevel(1, loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    public void BattleModeSelectMob(int idMob)
    {
        var list = mobsActive;

        if (!_player[_playerSelect]._isAPlayer)
            list = mobsActiveAll;

        if (idMob >= _gms.PlayerCount)
        {
            print("BattleModeSelectMob(" + idMob + ") - Return[" + idMob + ">=" + _gms.PlayerCount + "]");
            return;
        }
        //Check se Mob Existe na lista
        if (!list.Contains(idMob))
        {
            print("BattleModeSelectMob(" + idMob + ") - Return[Banido ou ja Selecionado]");
            return;
        }

        int oldSelect = _player[_playerSelect]._mobsSelect[_mobSelect];

        string nameHero = _gms.HeroName(idMob);

        if (_gms.CheckMobBlocked(idMob) && _player[_playerSelect]._isAPlayer ||
            _gms.CheckMobBanned(idMob))
        {
            _mS.Audio(false);
            print("BattleModeSelectMob(" + idMob + " -> " + nameHero + ") - Return[Mob Bloqueado ou banido]");
            return;
        }

        print("Player (" + _playerSelect + ") - Selection Mob[" + _mobSelect + "/" + (1 + _battleMode) + "](" + idMob + " -> " + nameHero + ")");

        _player[_playerSelect]._mobsSelect[_mobSelect] = idMob;

        _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].GetComponent<Image>().sprite
        = _gms.SpritePerfil(idMob);

        _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._nameMobSelection[_mobSelect].text
        = (_gms.BattleModeOptionRoundActive ? "(" + (_mobSelect+ 1) + ") " : "") + nameHero;

        _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._classMobSelection[_mobSelect].GetComponentsInChildren<Image>()[1].sprite
        = _gms.SpriteClass(idMob);


        if (uniqueSelection)
        {
            //Ativa o Antigo
            if (oldSelect != -1)
                BanFreeMobInList(oldSelect,false);
            
                BanFreeMobInList(idMob);
        }

        FindMobSelect();

        _playerChangeSelect = -1;
        _mobChangeSelect    = -1;

        print("Player(" + (1+_playerSelect) + "/" + _player.Length + ") - Mob(" + (1 + _mobSelect) + "/" + (1 + _battleMode) + ")");
    }

    public void ShowAtributeMob(int idMob)
    {
        _painelShowAtribute.SetActive(true);

        var list = mobsActive;

        if (!_player[_playerSelect]._isAPlayer)
            list = mobsActiveAll;

        if (idMob >= _gms.PlayerCount)
        {
            _textMobShowAtribute.text = "<b>?????</b>";
            EUpdateHp    = 1;
            EUpdateAtk   = 1;
            EUpdateSpeed = 1;
            return;
        }
            if (!list.Contains(idMob))
            _textMobShowAtribute.text = "[Select] - <color=red>"+_gms.HeroName(idMob)+"</color>";
        else
        _textMobShowAtribute.text = _gms.HeroName(idMob);

        float hp    = _gms.CalculeHealthValueBase(idMob) / (float)_gms.CalculeStatusMobMaxHp, 
              atk   = _gms.CalculeDamageValueBase(idMob) / (float)_gms.CalculeStatusMobMaxAttackDamage, 
              speed = 0;

        EUpdateHp = hp;
        EUpdateAtk = atk;
        EUpdateSpeed = speed;     

        _sliderSpeedMobShowAtribute.gameObject.SetActive(false);
    }

    WaitForSeconds wAddS = new WaitForSeconds(0f);
    float EUpdateHp, EUpdateAtk, EUpdateSpeed;
    IEnumerator EUpdateShowAtributeMob()
    {
        while (true)//_sliderHpMobShowAtribute.value != Hp && _sliderAtkMobShowAtribute.value != Atk/* && _sliderSpeedMobShowAtribute.value == Speed*/)
        {
            //print("While");

            if (_sliderHpMobShowAtribute.value != EUpdateHp)
            {
                if (_sliderHpMobShowAtribute.value > EUpdateHp)
                    _sliderHpMobShowAtribute.value -= 0.01f;
                else
            if (_sliderHpMobShowAtribute.value < EUpdateHp)
                    _sliderHpMobShowAtribute.value += 0.01f;
            }

            if (_sliderAtkMobShowAtribute.value != EUpdateAtk)
            {
                if (_sliderAtkMobShowAtribute.value > EUpdateAtk)
                    _sliderAtkMobShowAtribute.value -= 0.01f;
                else
            if (_sliderAtkMobShowAtribute.value < EUpdateAtk)
                    _sliderAtkMobShowAtribute.value += 0.01f;
            }

            if (_sliderSpeedMobShowAtribute.value >= EUpdateSpeed)
                _sliderSpeedMobShowAtribute.value = EUpdateSpeed;

            if (EUpdateHp - _sliderHpMobShowAtribute.value <= 0.009f   &&
                EUpdateAtk - _sliderAtkMobShowAtribute.value <= 0.009f )
            {
                //_sliderHpMobShowAtribute.value  = EUpdateHp;
                //_sliderAtkMobShowAtribute.value = EUpdateAtk;

                _sliderHpMobShowAtribute.GetComponentsInChildren<Image>()[1].color    = EUpdateHp    != 1 ? _mS._colorStatusMob[0] : _mS._colorStatusMob[1];
                _sliderAtkMobShowAtribute.GetComponentsInChildren<Image>()[1].color   = EUpdateAtk   != 1 ? _mS._colorStatusMob[0] : _mS._colorStatusMob[1];
                _sliderSpeedMobShowAtribute.GetComponentsInChildren<Image>()[1].color = EUpdateSpeed != 1 ? _mS._colorStatusMob[0] : _mS._colorStatusMob[1];
            }

            yield return wAddS;

            //Debug.LogWarning("HP:" + (EUpdateHp - _sliderHpMobShowAtribute.value));
            //Debug.LogWarning("Atk:" + (EUpdateAtk - _sliderAtkMobShowAtribute.value));
        }
    }

    /// <summary>
    /// Pula pra proximo espação em braco na area de seleção
    /// </summary>
    /// <param name="count"></param>
    void FindMobSelect(int count = 0)
    {
        count++;

        if (_battleMode > _mobSelect)
        {
            _mobSelect++;
        }
        else
        {
            _playerSelect++;

            _mobSelect = 0;

            for (int i = _playerSelect; i < _player.Length; i++)
            {
                for (int j = 0; j <= _battleMode; j++)
                {
                    if (_player[i]._mobsSelect[j] == -1)
                    {
                        _mobSelect = j;
                        break;
                    }
                }
            }
            

            if (_playerSelect >=_player.Length)
                _playerSelect = 0;
        }        

        if (count > ((_battleMode + 1) * 2)+1/*+1 pra nao ter erro*/)
            return;

        //if (_player[_playerSelect]._mobsSelect[_mobSelect] != -1)
        //    FindMobSelect(count);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="idMob"></param>
    /// <param name="idPlayer"></param>
    /// <param name="idMobSelect"></param>
    /// <param name="idBattleMode">-1 for all</param>
    public void BattleModeSelectMob(int idMob, int idPlayer, int idMobSelect, int idBattleMode)
    {
        if (idMob >= _gms.PlayerCount)
        {
            print("BattleModeSelectMob(" + idMob + ") - Return[" + idMob + ">=" + _gms.PlayerCount + "]");
            return;
        }
        string nameHero = (_gms.BattleModeOptionRoundActive ? "(" + (_mobSelect + 1) + ") " : "") + "-";

        if (idMob != -1)
        {
            nameHero = (_gms.BattleModeOptionRoundActive ? "(" + (_mobSelect + 1) + ") " : "") + _gms.HeroName(idMob);

            if (_gms.CheckMobBlocked(idMob) ||
                _gms.CheckMobBanned(idMob))
            {
                _mS.Audio(false);
                print("BattleModeSelectMob(" + idMob + " -> " + nameHero + ") - Return[Mob Bloqueado ou banido]");
                return;
            }
        }

        print("Player (" + idPlayer + ") - Selection Mob[" + idMobSelect + "/" + (1 + idBattleMode) + "](" + idMob + " -> " + nameHero + ")");

        if (idBattleMode == -1)
        {
            #region 
            foreach (var item in _player[idPlayer]._mobsSelectionsPlayer)
            {
                if (idMobSelect < _player[idPlayer]._mobsSelectionsPlayer.Length)
                {
                    if (idMobSelect == -1)
                    {
                        int CountMobSelect = item._mobSelection.Length;
                        for (int i = 0; i < CountMobSelect; i++)
                        {
                            if (idMob == -1)
                            {
                                if (uniqueSelection && _player[idPlayer]._mobsSelect[i] != -1)//free
                                    BanFreeMobInList(_player[idPlayer]._mobsSelect[i], false);

                                item._mobSelection[i].GetComponent<Image>().sprite
                                = _selectionMobAbsent;

                                item._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                                = _selectionMobAbsent;
                            }
                            else
                            {

                                if (uniqueSelection && idMob != -1)//Block
                                    BanFreeMobInList(_player[idPlayer]._mobsSelect[i]);

                                item._mobSelection[i].GetComponent<Image>().sprite
                                   = _gms.SpritePerfil(idMob);

                                item._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                                = _gms.SpriteClass(idMob);
                            }

                            item._nameMobSelection[i].text
                                = nameHero;
                        }
                    }
                    else
                    {
                        if (idMobSelect < item._mobSelection.Length)
                        {
                            if (idMob == -1)
                            {

                                if (uniqueSelection && idMobSelect != -1)//free
                                    BanFreeMobInList(idMobSelect, false);

                                item._mobSelection[idMobSelect].GetComponent<Image>().sprite
                                 = _selectionMobAbsent;

                                item._classMobSelection[idMobSelect].GetComponentsInChildren<Image>()[1].sprite
                                = _selectionMobAbsent;
                            }
                            else
                            {
                                if (uniqueSelection && idMobSelect != -1)//Select
                                    BanFreeMobInList(idMobSelect);


                                item._mobSelection[idMobSelect].GetComponent<Image>().sprite
                               = _gms.SpritePerfil(idMob);

                                item._classMobSelection[idMobSelect].GetComponentsInChildren<Image>()[1].sprite
                                = _gms.SpriteClass(idMob);
                            }

                            item._nameMobSelection[idMobSelect].text
                                = nameHero;
                        }
                    }
                }
            }
            #endregion
        }
        else
        {
            #region
            if (idMobSelect == -1)
            {
                int CountMobSelect = _player[idPlayer]._mobsSelect.Length;
                for (int i = 0; i < CountMobSelect; i++)
                {
                    if (idMob == -1)
                    {
                        if (uniqueSelection && _player[idPlayer]._mobsSelect[i] != -1)//Ban
                            BanFreeMobInList(idMobSelect);

                        _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._mobSelection[i].GetComponent<Image>().sprite
                        = _gms.SpritePerfil(idMob);

                        _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                        = _gms.SpriteClass(idMob);
                    }
                    else
                    {
                        if (uniqueSelection && _player[idPlayer]._mobsSelect[i] != -1)//free
                            BanFreeMobInList(idMobSelect, false);

                        _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._mobSelection[i].GetComponent<Image>().sprite
                        = _selectionMobAbsent;

                        _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                        = _selectionMobAbsent;
                    }

                    _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._nameMobSelection[i].text
                        = (_gms.BattleModeOptionRoundActive ? "("+(i + 1) + ") " : "") + nameHero;
                }
            }
            else
            {
                if (idMob == -1)
                {
                    if (uniqueSelection && idMobSelect != -1)//ban
                        BanFreeMobInList(idMobSelect);

                    _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._mobSelection[idMobSelect].GetComponent<Image>().sprite
                    = _gms.SpritePerfil(idMob);

                    _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._classMobSelection[idMobSelect].GetComponentsInChildren<Image>()[1].sprite
                    = _gms.SpriteClass(idMob);
                }
                else
                {
                    if (uniqueSelection && idMobSelect != -1)//free
                        BanFreeMobInList(idMobSelect, false);

                    _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._mobSelection[idMobSelect].GetComponent<Image>().sprite
                    = _selectionMobAbsent;

                    _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._classMobSelection[idMobSelect].GetComponentsInChildren<Image>()[1].sprite
                    = _selectionMobAbsent;
                }

                _player[idPlayer]._mobsSelectionsPlayer[idBattleMode]._nameMobSelection[idMobSelect].text
                    = (_gms.BattleModeOptionRoundActive ? "(<b> " + (idMobSelect + 1) + " </b>)" : "")  + nameHero;
            }
            #endregion
        }

        if (idMobSelect == -1)
        {
            foreach (var item in _player)
            {
                if (item == _player[idPlayer])
                {
                    int CountMobSelect = item._mobsSelect.Length;
                    for (int i = 0; i < CountMobSelect; i++)
                        item._mobsSelect[i] = idMob;
                }
            }
        }
        else
            _player[idPlayer]._mobsSelect[idMobSelect] = idMob;

        print("Player(" + idPlayer + "/" + _player.Length + ") - Mob(" + idMobSelect + "/" + (1 + idBattleMode) + ")");

        FindMobSelect();
    }

    public void UpDateSelectMob()
    {
        foreach (var item in _player)
        {
            int count = item._mobsSelectionsPlayer.Length;
            for (int i = 0; i < count; i++)
            {
                int count1 = item._mobsSelectionsPlayer[i]._mobSelection.Length;
               
                for (int j = 0; j < count1; j++)
                {
                    int idMob = item._mobsSelect[j];
                   
                    if (idMob != -1)
                    {
                        item._mobsSelectionsPlayer[i]._mobSelection[j].GetComponent<Image>().sprite
                        = _gms.SpritePerfil(idMob);

                        item._mobsSelectionsPlayer[i]._nameMobSelection[j].text
                        = (_gms.BattleModeOptionRoundActive ? "(" + (j + 1) + ") " : "") + _gms.HeroName(idMob);
                    }
                    else
                    {
                        item._mobsSelectionsPlayer[i]._mobSelection[j].GetComponent<Image>().sprite = _selectionMobAbsent;
                        item._mobsSelectionsPlayer[i]._nameMobSelection[j].text = (_gms.BattleModeOptionRoundActive ? "(" + (j + 1) + ") " : "") + "-";
                        item._mobsSelectionsPlayer[i]._classMobSelection[j].GetComponentsInChildren<Image>()[1].sprite = _selectionMobAbsent;
                    }
                }
            }

        }

    }

    /// <summary>
    /// Change Player for Computer
    /// </summary>
    /// <param name="playerNumber">Use Real Number 1 or 2</param>
    public void ChangePlayerForIa(int playerNumber)
    {
        playerNumber--;

        Button buttonPlayer = null;
        int idTranslate = -1;

        buttonPlayer = _player[playerNumber]._buttonPlayer;

        _player[playerNumber]._isAPlayer = !_player[playerNumber]._isAPlayer;

        idTranslate = _player[playerNumber]._isAPlayer ? 206/*Jogador*/ : 207/*Computer*/;

        int number = 1 + playerNumber;

        //Muda posicao na lista
        _playerSelect = playerNumber;
        _mobSelect = 0;

        buttonPlayer.GetComponentInChildren<Text>().text = XmlMenuInicial.Instance.Get(idTranslate) + "\n" + (number);//Jogador       

        //clear select List
        BattleModeSelectMob(-1, playerNumber, -1, -1);

        LoadSelectMobs(_player[playerNumber]._isAPlayer);

        if (!UniqueSelecetionBattleMode())
            ChangeBattleMode(0);
    }

    public void ButtonChangeBattleMode()
    {
        int value = 1+_battleMode;

        ChangeBattleMode(value);
    }

    void ChangeBattleMode(int value)
    {
        _battleMode = value;

        if (_battleMode > _battleModeCount - 1)
            _battleMode = 0;

        if(!UniqueSelecetionBattleMode())
            _battleMode=0;
                //return;

        int mode = 1 + _battleMode;

        _buttonChangeBattleMode.GetComponentInChildren<Text>().text = mode + " x " + mode;

        foreach (var item in _player)
        {
            //Clear All Canvas
            int countCanvas = item._mobsSelectionsPlayer.Length;
            for (int i = 0; i < countCanvas; i++)
            {
                item._mobsSelectionsPlayer[i]._canvasGroup.alpha = 0;
                item._mobsSelectionsPlayer[i]._canvasGroup.blocksRaycasts = false;
            }

            //active Canvas
            item._mobsSelectionsPlayer[_battleMode]._canvasGroup.alpha = 1;
            item._mobsSelectionsPlayer[_battleMode]._canvasGroup.blocksRaycasts = true;

            //Change Sprite
            int countMob = item._mobsSelectionsPlayer[_battleMode]._mobSelection.Length;
            for (int i = 0; i < countMob; i++)
            {
                if (item._mobsSelect[i] != -1)
                {
                    item._mobsSelectionsPlayer[_battleMode]._mobSelection[i].GetComponent<Image>().sprite
                    = _gms.SpritePerfil(item._mobsSelect[i]);

                    item._mobsSelectionsPlayer[_battleMode]._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                    = _gms.SpriteClass(item._mobsSelect[i]);

                    item._mobsSelectionsPlayer[_battleMode]._nameMobSelection[i].text = (_gms.BattleModeOptionRoundActive ? "(" + (i + 1) + ") " : "") + _gms.HeroName(item._mobsSelect[i]);
                }
                else
                {
                    item._mobsSelectionsPlayer[_battleMode]._mobSelection[i].GetComponent<Image>().sprite
                     = _selectionMobAbsent;
                    item._mobsSelectionsPlayer[_battleMode]._classMobSelection[i].GetComponentsInChildren<Image>()[1].sprite
                     = _selectionMobAbsent;

                    item._mobsSelectionsPlayer[_battleMode]._nameMobSelection[i].text = (_gms.BattleModeOptionRoundActive ? "(" + (i + 1) + ") " : "") + "-";
                }
            }
        }

        if (uniqueSelection)
        {
            //Libera Mobs banidos, do qual ele nao pode participar
            for (int p = 0; p < _player.Length; p++)
            {
                for (int i = 0; i < _battleModeCount; i++)
                {
                    if (i > _battleMode)
                    {
                        BanFreeMobInList(_player[p]._mobsSelect[i], false);
                        _player[p]._mobsSelect[i] = -1;
                    }
                }
            }

            UpDateSelectMob();
        }

        //Zera lista de selecao
        _playerSelect = 0;
        _mobSelect = _player[_playerSelect]._mobsSelect[0] == -1 ? 0 : _battleMode;
        _playerChangeSelect = -1;
        _mobChangeSelect = -1;
    }

    //Change Mob Select Position
    /// <summary>
    /// Mudar Posição na lista
    /// </summary>
    int _playerChangeSelect = -1;
    /// <summary>
    /// Mudar Posição na lista
    /// </summary>
    int _mobChangeSelect    = -1;

    bool UniqueSelecetionBattleMode()
    {
        int EspaçosLivres = (_battleMode + 1) * 2;

        if (uniqueSelection && _player[0]._isAPlayer && _player[1]._isAPlayer)
        {           
            if (EspaçosLivres > mobsActive.Count)
            {
                _gms.NewInfo(
                    _gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(211)/*Libere {0} mobs, para aumentar o time!!!*/
                , "<color=red>" + (EspaçosLivres - mobsActive.Count) + "</color>")
                , 4);

                Debug.LogError("Espaços livres  [" + EspaçosLivres + "] / mobs [" + mobsActive.Count + "] -> " + (EspaçosLivres - mobsActive.Count));
                return false;
            }
        }


        Debug.LogError("Espaços livres  [" + EspaçosLivres + "] / mobs [" + mobsActive.Count + "] -> " + (EspaçosLivres - mobsActive.Count));
        return true;
    }

    public void ChangeMobSelectPlayer(int player)
    {
        _playerSelect = player;

        if (_battleMode != 0)
        {
            if (_playerChangeSelect == -1 || _playerChangeSelect != _playerSelect)
                _playerChangeSelect = player;
        }
    }
    public void ChangeMobSelect(int mobPlayerList)
    {
        _mobSelect = mobPlayerList;

        if (_battleMode != 0)
        {
            if (_mobChangeSelect == -1)
            _mobChangeSelect = mobPlayerList;
            else
            if (_playerChangeSelect == _playerSelect && _mobChangeSelect != mobPlayerList)
            {
                //Change Mob Select Position
                int changeOld = _player[_playerChangeSelect]._mobsSelect[_mobChangeSelect],
                    changeNew = _player[_playerSelect]._mobsSelect[_mobSelect];

                _player[_playerChangeSelect]._mobsSelect[_mobChangeSelect] = changeNew;
                _player[_playerSelect]._mobsSelect[_mobSelect] = changeOld;

                UpDateSelectMob();

                _playerChangeSelect = -1;
                _mobChangeSelect    = -1;
            }
        }

        _selectionMobObj.GetComponent<CanvasGroup>().alpha = (_mobChangeSelect != -1) ? 1 : 0;

        if (_selectionMobObj.GetComponent<CanvasGroup>().alpha == 1)
        {

            float x = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.x,
                  y = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.y,
                  z = _player[_playerSelect]._mobsSelectionsPlayer[_battleMode]._mobSelection[_mobSelect].transform.position.z;

            Vector3 screenPoint = new Vector3(x, y, z);
            _selectionMobObj.transform.position = screenPoint;

            Vector2 size = (_battleMode == 0 ? new Vector2(226, 203) : new Vector2(180, 156.6f));
            _selectionMobObj.GetComponent<RectTransform>().sizeDelta = size;
        }
    }

    void BanFreeMobInList(int mobID,bool ban=true)
    {
        if (mobID==-1)
            return;

        if (ban)
        {
            if (mobsActiveAll.Contains(mobID))
                mobsActiveAll.Remove(mobID);
            if (mobsActive.Contains(mobID) && !_gms.CheckMobBlocked(mobID))
                mobsActive.Remove(mobID);
            if (!mobsBanneds.Contains(mobID))
                mobsBanneds.Add(mobID);

            _selectionMobList[mobID].GetComponent<Image>().sprite = _selectionMobBanned;
            _selectionMobList[mobID].GetComponent<Button>().onClick.RemoveListener(() => _mS.Audio());
            _selectionMobList[mobID].GetComponent<Button>().onClick.AddListener(() => _mS.Audio(false));          
        }
        else
        {
            if (!mobsActiveAll.Contains(mobID))
                mobsActiveAll.Add(mobID);
            if (!mobsActive.Contains(mobID) && !_gms.CheckMobBlocked(mobID))
                mobsActive.Add(mobID);
            if (mobsBanneds.Contains(mobID))
                mobsBanneds.Remove(mobID);

            _selectionMobList[mobID].GetComponent<Image>().sprite = _gms.SpritePerfil(mobID);
            _selectionMobList[mobID].GetComponent<Button>().onClick.RemoveListener(() => _mS.Audio(false));
            _selectionMobList[mobID].GetComponent<Button>().onClick.AddListener(() => _mS.Audio());
        }

        _selectionMobList[mobID].GetComponentsInChildren<Image>()[1].enabled = !ban;
    }

    #region Config Manager
    [Header("Config Manager")]
    [SerializeField] float  _baseStatusHp             = 60;
    [SerializeField] float  _baseStatusDamage         = 40;
    [SerializeField] float  _baseStatusCriticalChance = 10;
    [SerializeField] float  _baseResistenceFire       = 10;
    [SerializeField] float  _baseResistencePoison     = 10;
    [SerializeField] float  _baseResistencePetrify    = 15;
    [SerializeField] float  _baseResistenceBleed      = 10;
    [SerializeField] Toggle _buttonToggleUniqueSelection;
    [SerializeField] Toggle _buttonToggleMobPassive;
    [SerializeField] Toggle _buttonToggleRoundMode;
    [SerializeField] Slider _buttonSliderStatusHp;
    [SerializeField] Slider _buttonSliderStatusDamage;
    [SerializeField] Slider _buttonSliderStatusCriticalChance;

    [SerializeField] Slider _buttonSliderResistenceFire;
    [SerializeField] Slider _buttonSliderResistencePoison;
    [SerializeField] Slider _buttonSliderResistencePetrify;
    [SerializeField] Slider _buttonSliderResistenceBleed;

    void StartConfig()
    {
        print("StartConfig");

        _buttonToggleUniqueSelection.gameObject.SetActive(false);
        uniqueSelection = _gms.BattleModeOptionUniqueSelection;
        _buttonToggleUniqueSelection.isOn     = uniqueSelection;
        _textButtonToggleUniqueSelection.text = XmlMenuInicial.Instance.Get(212);//Seleção Única
        _buttonToggleUniqueSelection.gameObject.SetActive(true);

        _buttonToggleMobPassive.isOn = _gms.BattleModeOptionPassiveMobActive;
        _textButtonToggleMobsPassive.text = XmlMenuInicial.Instance.Get(32);//Passivas dos mob's

        _buttonToggleRoundMode.isOn       = _gms.BattleModeOptionRoundActive;
        _textButtonToggleRoundMode.text = XmlMenuInicial.Instance.Get(33);//Batalha Por Rodada

        #region Status
        //_buttonSliderStatusHp.minValue = _baseStatusHp / -1;
        _buttonSliderStatusHp.value    = _gms.BattleModeOptionStatusHpBaseExtra;
        ButtonSliderStatusHp();

        //_buttonSliderStatusDamage.minValue = _baseStatusDamage / -1;
        _buttonSliderStatusDamage.value = _gms.BattleModeOptionStatusDamageBaseExtra;
        ButtonSliderStatusDamage();

        _buttonSliderStatusCriticalChance.minValue = _baseStatusCriticalChance / -1;
        _buttonSliderStatusCriticalChance.value    = _gms.BattleModeOptionStatusCriticalChanceBaseExtra;       
        ButtonSliderStatusCriticalChance();
        #endregion

        #region Resistence
        _buttonSliderResistenceFire.minValue = _baseResistenceFire / -1;
        _buttonSliderResistenceFire.value    = _gms.BattleModeOptionDbuffFireResistenceExtra;      
        ButtonSliderResistenceFire();

        _buttonSliderResistencePoison.minValue = _baseResistencePoison / -1;
        _buttonSliderResistencePoison.value   = _gms.BattleModeOptionDbuffPoisonResistenceExtra;     
        ButtonSliderResistencePoison();

        _buttonSliderResistencePetrify.minValue = _baseResistencePetrify / -1;
        _buttonSliderResistencePetrify.value    = _gms.BattleModeOptionDbuffPetrifyResistenceExtra;       
        ButtonSliderResistencePetrify();

        _buttonSliderResistenceBleed.minValue = _baseResistenceBleed / -1;
        _buttonSliderResistenceBleed.value = _gms.BattleModeOptionDbuffBleedResistenceExtra;       
        ButtonSliderResistenceBleed();     
        #endregion
    }

    public void ButtonToggleUniqueSelection()
    {
        _gms.BattleModeOptionUniqueSelection = _buttonToggleUniqueSelection.isOn;

        uniqueSelection = _buttonToggleUniqueSelection.isOn;

        if (!loadComplete)
          return;       

        //Libera Mobs banidos, do qual ele nao pode participar
        for (int p = 0; p < _player.Length; p++)
        {
            for (int i = 0; i < _battleModeCount; i++)
            {
                BanFreeMobInList(_player[p]._mobsSelect[i], false);

                _player[p]._mobsSelect[i] = -1;
            }
        }

        UpDateSelectMob();

        //Check Se modo Battle Esta Apto
        if (!UniqueSelecetionBattleMode())
            ChangeBattleMode(0);

        //Zera lista de selecao
        _mobSelect = 0;
        _playerSelect = 0;
        _playerChangeSelect = -1;
        _mobChangeSelect = -1;
    }

    public void ButtonToggleMobsPassive()
    {
        _gms.BattleModeOptionPassiveMobActive = _buttonToggleMobPassive.isOn;        
    }

    public void ButtonToggleRoundMode()
    {
        _gms.BattleModeOptionRoundActive = _buttonToggleRoundMode.isOn;

        int count = _player.Length;

        for (int i = 0; i < count; i++)
        {

            int count1 = _player[i]._mobsSelectionsPlayer.Length;
            for (int j = 0; j < count1; j++)
            {

                int count2 = _player[i]._mobsSelectionsPlayer[j]._nameMobSelection.Length;

                for (int n = 0; n < count2; n++)
                {
                        _player[i]._mobsSelectionsPlayer[j]._nameMobSelection[n].text = (_gms.BattleModeOptionRoundActive ? "(" + (n + 1) + ") " : "") + (_player[i]._mobsSelect[n] == -1 ? "-" : _gms.HeroName(_player[i]._mobsSelect[n]));
                }
            }
        }
        
    }

    public void ButtonSliderStatusHp()
    {
        float value = _buttonSliderStatusHp.value;

        _gms.BattleModeOptionStatusHpBaseExtra = (int)value;

        _textButtonSliderStatusHp.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(28),//Status {0}
            XmlMenuInicial.Instance.Get(30)//Hp
            )+" - "+(value + (value>0? _baseStatusHp : 0));
    }
    public void ButtonSliderStatusDamage()
    {
        float value = _buttonSliderStatusDamage.value;

        _gms.BattleModeOptionStatusDamageBaseExtra = (int)value;

        _textButtonSliderStatusDamage.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(28),//Status {0}
            "<color=red><b>" + XmlMenuInicial.Instance.Get(31) + "</b></color>"//Dano
            ) + " - " + (value + (value>0 ? _baseStatusDamage : 0));
    }
    public void ButtonSliderStatusCriticalChance()
    {
        float value = _buttonSliderStatusCriticalChance.value;

        _gms.BattleModeOptionStatusCriticalChanceBaseExtra = value;

        _gms.BattleModeOptionCriticalDamageActive = (value + _baseStatusCriticalChance > 0 ? true : false);

        _textButtonSliderStatusCriticalChance.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(28),//Status {0}
            "<color=magenta><b>" + XmlMenuInicial.Instance.Get(161) + "</b></color>"//Critical
            )+" - "+(value + _baseStatusCriticalChance > 100 ? 100 : value + _baseStatusCriticalChance) + "%";
    }

    public void ButtonSliderResistenceFire()
    {
        float value = _buttonSliderResistenceFire.value;

        _buttonSliderResistenceFire.value = value;

        _gms.BattleModeOptionDbuffFireResistenceExtra = value;

        _textButtonSliderResistenceFire.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(29),//Resistence {0}
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire, true)
            ) + " - " + (value + _baseResistenceFire > 100 ? 100 : value + _baseResistenceFire) + "%";
    }
    public void ButtonSliderResistencePoison()
    {
        float value = _buttonSliderResistencePoison.value;

        _gms.BattleModeOptionDbuffPoisonResistenceExtra = value;

        _buttonSliderResistencePoison.value = value;

        _textButtonSliderResistencePoison.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(29),//Resistence {0}
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar, true)
            ) + " - " + (value + _baseResistencePoison > 100 ? 100 : value + _baseResistencePoison) + "%";
    }
    public void ButtonSliderResistencePetrify()
    {
        float value = _buttonSliderResistencePetrify.value;

        _gms.BattleModeOptionDbuffPetrifyResistenceExtra = value;

        _buttonSliderResistencePetrify.value = value;

        _textButtonSliderResistencePetrify.text =
    _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(29),//Resistence {0}
    XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar, true)
    ) + " - " + (value + _baseResistencePetrify > 100 ? 100 : value + _baseResistencePetrify) + "%";
    }
    public void ButtonSliderResistenceBleed()
    {
        float value = _buttonSliderResistenceBleed.value;

        _gms.BattleModeOptionDbuffBleedResistenceExtra = (int)value;

        _buttonSliderResistenceBleed.value = value;

        _textButtonSliderResistenceBleed.text =
            _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(29),//Resistence {0}
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed, true)
            ) + " - " + (value + _baseResistenceBleed > 100 ? 100 : value + _baseResistenceBleed) + "%";
    }
    #endregion

}
