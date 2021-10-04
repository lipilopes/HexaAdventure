using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Internal;

[System.Serializable]
class CheatCode
{
    public string     _caracter = "";
    public UnityEvent _event;
}

public class MenuScene : MonoBehaviour
{
    public static MenuScene Instance;

    [Header("Painels")]
    [SerializeField]
    GameObject[] _painels;
    [SerializeField]   
    GameManagerScenes Gms;
    [SerializeField]
    GameManagerScenes _gms;

    [Header("Audio")]
    [SerializeField]
    AudioClip _clickCerto;
    [SerializeField]
    AudioClip _clickErrado;
    AudioSource _audioSorce;

    [Header("Painel Inicial")]
    [SerializeField]
    GameObject _gameMode;

    [Header("Options")]
    [SerializeField]
    Toggle _moveCamera;
    [SerializeField]
    Slider _sensibilityMouse;
    [SerializeField]
    Slider _soundEffect;
    [SerializeField]
    Slider _soundMusic;
    [SerializeField]
    Dropdown _dificuldade;
    [SerializeField]
    Dropdown _language;

    [Space, Header("Extras")]
    [SerializeField]
    Dropdown _selectPlayer;
    [SerializeField]
    GameObject _itemSelectPlayer;
    [SerializeField]
    Dropdown _selectSkinPlayer;   
    [SerializeField]
    Text _totalTimeHoursPainel1;
    [SerializeField]
    GameObject[] _extras;
    int _hour, _min, _seg;
    [Space]
    [Header("Dropdown - SelectHero")]
    [SerializeField] Sprite _spriteBlockSelectHero;
    [SerializeField] Sprite _spriteBanSelectHero;
    public Color[] _colorStatusMob;
    /// <summary>
    /// 0: Hp 1:Damage 2:Speed
    /// </summary>
    [SerializeField] Slider[] _sliderStatusMob;
    [Space]
    [Header("Buttons")]
    [SerializeField] UnityEngine.EventSystems.EventTrigger _resetGameButton;
    [SerializeField] Text _achievementButton;
    [SerializeField] Text _extraButton;
    [SerializeField] Text _notaAtualizacaoButton;
    [SerializeField] Text _getCode;
    [SerializeField] Text _gameModeHistoryButton;
    [SerializeField] Button _gameModeBattleButton;
    [Space]
    [Header("Text")]
    [SerializeField] Text _gameModeText;

    [Space, SerializeField]
    CheatCode[] _cheats;

    WaitForSeconds waitCheatLoad = new WaitForSeconds(0f);
    WaitForSeconds waitLoad      = new WaitForSeconds(0f);
    float  cheatValue  = -1;
    string cheatValueY = "";

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        GmsInstantiate();       

        _audioSorce = GetComponent<AudioSource>();

        if (_gms == null)
            GmsInstantiate();

        if (!_gms.IsMobile)
        {
            _moveCamera.isOn        = _gms.MoveCameraArrow;            
        }
        else
        {
            _moveCamera.gameObject.SetActive(false);
            //_sensibilityMouse.GetComponentInChildren<Text>().text = "<b>Touch</b> Sensibility:";
        }


        //_language.gameObject.SetActive(!_gms.IsMobile);

        _gms.Paused = false;

        _sensibilityMouse.value = _gms.MouseSensibility;

        _soundEffect.value      = _gms.SoundEffect;

        _soundMusic.value       = _gms.SoundMusic;           

        ActivePainel(0);

        StartCoroutine(LoadTranslate());

        _gms.GameModeChange(Game_Mode.History);

        return;
    }

    IEnumerator LoadTranslate()
    {
            while (XmlMenuInicial.Instance == null)
            {
                yield return waitCheatLoad;
            }

            TotalHours();

            _sensibilityMouse.GetComponentsInChildren<Text>()[0].text = _gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(0), _gms.IsMobile ? "Touch" : "Mouse");

            _dificuldade.options.Clear();

            for (int i = 0; i < _gms._dificuldadeCount; i++)            
               _dificuldade.options.Add(new Dropdown.OptionData() { text = _gms.DificuldadeString(i) });                              

            _dificuldade.value = System.Convert.ToInt32(_gms.Dificuldade());

            //_language .GetComponentsInChildren<Text>()[0].text = xmlMenu.Get(2) + " " + _gms.Language();

            _language.value    = System.Convert.ToInt32(_gms.Language());

            _soundEffect.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(3);

            _soundMusic.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(4);

            _moveCamera.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(5);

            _resetGameButton.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(8);
            //_resetGameButton.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(11);

            _extraButton.text = XmlMenuInicial.Instance.Get(14);

            _achievementButton.text = XmlMenuInicial.Instance.Get(15);

            _notaAtualizacaoButton.text = XmlMenuInicial.Instance.Get(16);

            _getCode.text = XmlMenuInicial.Instance.Get(86);

            _gameModeHistoryButton.text = XmlMenuInicial.Instance.Get(201);//Historia

            _gameModeBattleButton.GetComponentInChildren<Text>().text = XmlMenuInicial.Instance.Get(202);//Batalha
        
            _gameModeText.text = XmlMenuInicial.Instance.Get(204);//Modo de Jogo
    }

    void GmsInstantiate()
    {
        _gms = GameManagerScenes._gms;

        if (_gms == null)
            _gms = Instantiate(Gms);
    }

    public void ActivePainel(int value)
    {
        for (int i = 0; i < _painels.Length; i++)
        {
            _painels[i].SetActive(false);
        }

        if (value >= 0 && value <= _painels.Length - 1)
            _painels[value].SetActive(true);

        if (value == 0 && XmlMenuInicial.Instance != null)
            TotalHours();

        if (value==0)
            _gms.GameModeChange(Game_Mode.History);
    }

    public void Audio(bool interative = true)
    {
        if (_audioSorce == null)
            return;

        if (interative)
            _audioSorce.clip = _clickCerto;
        else
            _audioSorce.clip = _clickErrado;

        _audioSorce.Play();
    }

    #region Painel Inicial
    public void PlayButton()
    {
        if (_gms == null)
            GmsInstantiate();

        if (_gms.Adm ||
            !_gms.BattleModeBlocked)
        {
            bool cB = CheckModeBattle();

            _gameModeBattleButton.GetComponentInChildren<Text>().enabled  = cB;
            _gameModeBattleButton.GetComponentsInChildren<Image>()[1].enabled = !cB;

            _gameMode.SetActive(true);
            return;
        }

        GameModeHistoryButton();
    }

    public void OptionButton()
    {
        ActivePainel(1);

        //_dificuldade.value = System.Convert.ToInt32(_gms.Dificuldade());
        _dificuldade.GetComponentsInChildren<Text>()[0].text = XmlMenuInicial.Instance.Get(1) + " <b>" + _gms.DificuldadeString() + "</b>";
        _language.value    = System.Convert.ToInt32(_gms.Language());
        _language.GetComponentsInChildren<Text>()[0].text = " <b>" + _gms.Language() + "</b>";
    }

    public void CloseButton()
    {
        Audio();

        UnityAction[] _t =
    {
         () => Application.Quit()
    };

        _gms.QuestionPainel(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(38) /*: "Deseja Sair do jogo??"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(39) /*: "Sair"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(13) /*: "<color=red>Cancelar</color>"*/,
                            _t, null);
    }

    public void GameModeHistoryButton()
    {
        print("GameModeHistoryButton()");

        if (_gms.CheckMobBanned() ||
            _gms.CheckMobBlocked())
        {
            _gms.NewInfo(
                _gms.AttDescriçãoMult(
                    ("<color=red>" + XmlMenuInicial.Instance.Get(56) + "</color>"),
                     //"<color=red>{0},nao esta qualificado para tal!!!</color>"
                    _gms.HeroName())
                , 1.5f);

            UnityAction[] _t = { () => _gms.ChangePlayer() };
            _gms.QuestionPainel(
               /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(57)/* : "Trocar mob Aleatoriamente, para um qualificado??"*/,
               /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(50) /*: "Sim"*/,
               /*!_gms.IsMobile ? */XmlMenuInicial.Instance.Get(13) /*: "Não"*/, _t, null);
            return;
        }

        if (_gms.CheckSkinBlocked())
        {
            _gms.NewInfo(
                _gms.AttDescriçãoMult(
                ("<color=red>" + XmlMenuInicial.Instance.Get(58) + "</color>")
                //"<color=red>A skin {0},nao esta qualificado para tal!!!</color>"                
                , _gms.SkinName()),
                1.5f);

            UnityAction[] _t = { () => _gms.PlayerSkinID = 0 };
            _gms.QuestionPainel(
                _gms.AttDescriçãoMult(
                    /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(59) /*:
                "Não esta qualificado para tal\nTrocar Skin {0}, para a padrão??"*/,
                _gms.SkinName()),
                /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(50)/* : "Sim"*/,
                /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(13)/* : "Não"*/, _t, null);
            return;
        }

        _gms.GameModeChange(Game_Mode.History);

        _gms.LoadLevel(1, loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    int needStartBattleMode = 2,
        countMobUnlocked    = 0;

    public void GameModeBattleButton()
    {
        print("GameModeBattleButton()");

        if (!CheckModeBattle())
        {
                _gms.NewInfo(
                    _gms.AttDescriçãoMult(
                        ("<color=red>" + XmlMenuInicial.Instance.Get(208) + "</color>"),
                         //"<color=red>É necessario ter no minimo {0} Mob's desbloqueados para entrar nesse modo!!!\nVocê Tem {1}</color>"
                         needStartBattleMode + "",
                         countMobUnlocked + "")
                    , 6f);

                return;
        }

        _gms.GameModeChange(Game_Mode.Battle);

        ActivePainel(4);

        _gameMode.SetActive(false);
    }

    bool CheckModeBattle()
    {
        if (!_gms.Adm)
        {
            countMobUnlocked = 0;
            int count = _gms.PlayerCount;
            for (int i = 0; i < count; i++)
            {
                if (!_gms.CheckMobBlocked(i))
                {
                    countMobUnlocked++;

                    if (countMobUnlocked >= needStartBattleMode)
                        break;
                }
            }
            //Viking Honor
            bool _completeFase12 = _gms.AchiementComplete(5);

            print(countMobUnlocked + "/" + needStartBattleMode + " Mobs Desbloqueados!!");

            return !(countMobUnlocked < needStartBattleMode) && _completeFase12;
        }

        return true;
    }
    #endregion

    #region Option Scene
    public void AttDificuldade()
    {
        if ((int)_gms.Dificuldade() == _dificuldade.value)
            return;

       Audio();

        _gms.ChangeDificuldade(_dificuldade.value);
    }

    public void AttMoveCamera()
    {
        Audio();
        _gms.MoveCameraArrow = _moveCamera.isOn;
    }

    public void AttSensibilityMouse()
    {
        float value = _sensibilityMouse.value;
        _gms.MouseSensibility = value;
    }

    public void AttSoundEffect()
    {
        float value = _soundEffect.value;
        _gms.SoundEffect = (value);
    }

    public void AttSoundMusic()
    {
        float value = _soundMusic.value;
        _gms.SoundMusic = (value);
    }

    public void ResetSave()
    {
        UnityAction[] _t =
            {
                 () => _gms.ResetSave    (),
                 () => _gms = Instantiate(Gms),
                 () => _gms.LoadLevel    (0,/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(10) /*: "Apagando Save"*/),
                 () => _gms.NewInfo      (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(10) /*: "Save Apagado com sucesso!!!"*/, 3)
           };

        _gms.QuestionPainel(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(11)/*:"Deseja Apagar Seu Save??"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(12)/*:"Apagar"*/,
                            /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(13)/*:"Cancelar"*/,
                           _t, null);
    }

    public void AttLanguage()
    {
        Audio();
        
        if (_language.value != (int)_gms.Language())
        {
            _gms.ChangeLanguage(_language.value);
            _gms.LoadLevel(0, loadMode: UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
    #endregion   

    #region Extras
    public void AtualizaSelectPlayerDropDown()
    {
        _selectPlayer.options.Clear();

        int count = _gms.PlayerCount;


        _itemSelectPlayer.GetComponentsInChildren<Image>()[1].sprite = _gms.SpriteClass(0);

        for (int i = 0; i < count; i++)
        {           
            string name   = _gms.HeroName(i/* + 1*/);
            Sprite sprite = _gms.SpritePerfil(i),
                   classMob = _gms.SpriteClass(i);

            bool ban     = _gms.CheckMobBanned(i/* + 1*/), 
                blocked = _gms.CheckMobBlocked(i);

            _itemSelectPlayer.SetActive(!blocked);

            if (!ban && !blocked)          
                _itemSelectPlayer.GetComponentsInChildren<Image>()[1].sprite = classMob;

            if (blocked)
            {
                name = "[<color=magenta><b>"+(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(60) /*: "Bloqueado"*/) +"</b></color>]"; 

                 sprite = _spriteBlockSelectHero;
            }

            if (ban)
            {
                name += " [<color=black><b>"+(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(61) /*: "Banido"*/) +"</b></color>]"; 

                 sprite = _spriteBanSelectHero;
            }

            _selectPlayer.options.Add(new Dropdown.OptionData() { text = name, image = sprite });
           
            
            //_selectPlayer.options[_selectPlayer.options.Count - 1].text = name +"["+ (_selectPlayer.options.Count - 1) + "] ";


            //GameObject.Find().name = "Acheii";

            //print("["+ _selectPlayer.options[_selectPlayer.options.Count - 1]. + "]: " + _selectPlayer.options[_selectPlayer.options.Count-1].text);
        }

        _selectPlayer.value = _gms.PlayerID - 1;

        AtualizaSliderStatusMob(_gms.PlayerID - 1);
    }

    public void SelectPlayer(Dropdown dropDown)
    {
        if (!_selectPlayer.gameObject.activeInHierarchy)
            return;

        if (dropDown == null)
            return;

        int PlayerSelect = dropDown.value;

        Debug.LogWarning("SelectPlayer[" + (dropDown.value) + "]: " + dropDown.captionText.text);

        if (_gms.PlayerID == PlayerSelect + 1)
        {
           // Audio(false);
            return;
        }

        bool blocked = _gms.CheckMobBlocked(PlayerSelect),
             banned  = _gms.CheckMobBanned (PlayerSelect);

        if (banned)
        {
            _gms.NewInfo(_gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(62) //"{0}\nEsta Banido!!!"
                ,_gms.HeroName(PlayerSelect)), 5);

            Audio(false);
        }
        if (blocked)
        {
            //Debug.LogError("<color=red>SelectPlayer[" + (PlayerSelect) + "]: " + dropDown.captionText.text + " Esta Bloqueado!!!</color>");

            //_gms.NewInfo(_gms.HeroName(PlayerSelect) + "\nEsta Bloqueado!!!", 5);
            Audio(false);

            _gms.NewInfo(
                XmlMenuInicial.Instance.Get(63) //"Este Mob\nEsta Bloqueado!!!"
                , 5);
        }
        if (!blocked && !banned)
        {
            //print("SelectPlayer[" + (PlayerSelect) + "]: " + dropDown.captionText.text);

            Audio(true);

            _selectSkinPlayer.GetComponentInChildren<Text>().text = "[<color=magenta><b>" + (XmlMenuInicial.Instance.Get(60) /*: "Bloqueado"*/) + "</b></color>]";

            _gms.PlayerID = (PlayerSelect) + 1;           

            _gms.NewInfo(_gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(64) //"Player trocado para:\n{0}"
                , _gms.HeroName(PlayerSelect)), 2.5f);

            _gms.ChangeSalveExtraPlayer();

            _selectSkinPlayer.value = _gms.CheckSkinBlocked((PlayerSelect) + 1, _gms.PlayerSkinID) ? 0 : _gms.PlayerSkinID;

            AttExtras(false);
        }

        dropDown.value = _gms.PlayerID-1;

        AtualizaSliderStatusMob(_gms.PlayerID - 1);
    }

    void AtualizaSliderStatusMob(int mob)
    {
        int count = _sliderStatusMob.Length;

        int HP     = _gms.CalculeHealthValueBase(mob),
            Damage = _gms.CalculeDamageValueBase(mob);

        float value;

        value = ((float)HP / (float)_gms.CalculeStatusMobMaxHp);
        _sliderStatusMob[0].value = value;
        _sliderStatusMob[0].GetComponentsInChildren<Image>()[1].color = value != 1 ? _colorStatusMob[0] : _colorStatusMob[1];
        print("ATTSLider: " + HP + " - " + _gms.CalculeStatusMobMaxHp + " = " + value);

        value = ((float)Damage / (float)_gms.CalculeStatusMobMaxAttackDamage);
        _sliderStatusMob[1].value = value;
        _sliderStatusMob[1].GetComponentsInChildren<Image>()[1].color = value != 1 ? _colorStatusMob[0] : _colorStatusMob[1];

        _sliderStatusMob[2].gameObject.SetActive(false);
    }

    public void AtualizaSkinDropDown()
    {
        _selectSkinPlayer.options.Clear();

        int playerId = _gms.PlayerID - 1;
        int count    = _gms.SkinCount(playerId);

        print("AtualizaSkinDropDown - " + _gms.HeroName(playerId) + ": " + count);

        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                bool blocked = _gms.CheckSkinBlocked(playerId, i);

                string name = _gms.SkinName(playerId, i, false);

                string type = _gms.GetSkinType(playerId, i, false);

                Sprite sprite = _gms.SpritePerfil(playerId, i);

                name += type == "" ? "" : "\n" + type;

                if (blocked)
                {
                    name = name + " - [<color=magenta><b>"+(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(60) /*: "Bloqueado"*/)+"</b></color>] ";

                    sprite = _spriteBlockSelectHero;
                }
                else
                if(_gms.Adm && _gms.Mob[playerId]._skinHero[i]._skinTag.Length > 0)
                {
                    name += " { ";

                    /* foreach (var t in _gms._Fases[playerId]._skinHero[i]._skinTag)
                     {
                         name += _gms.AttDescrição(t.ToString(),"_"," ",t.ToString())+", ";
                     }*/


                    int SkinTCount = _gms.Mob[playerId]._skinHero[i]._skinTag.Length;

                    for (int skinT = 0; skinT < SkinTCount; skinT++)
                    {
                        name += _gms.GetSkinTag(_gms.Mob[playerId]._skinHero[i]._skinTag[skinT]);

                        name += (skinT != SkinTCount - 1 ? ", " : " ");                       
                    }

                    name += "}";
                }

                _selectSkinPlayer.options.Add(new Dropdown.OptionData() { text = name, image = sprite });
            }
        }

        _selectSkinPlayer.value = -1;

        _selectSkinPlayer.value = _gms.CheckSkinBlocked() ? 0 : _gms.PlayerSkinID;
    }

    public void SelectSkinPlayer(Dropdown dropDown)
    {
        if (!_selectSkinPlayer.gameObject.activeInHierarchy)
            return;

        if (dropDown == null)
            return;

        int SkinSelect = dropDown.value;

        if (_gms.PlayerSkinID == SkinSelect)
        {
            // Audio(false);
            return;
        }

        Debug.LogWarning("SkinSelect[" + (dropDown.value) + "]: " + _gms.SkinName(-1, SkinSelect));

        bool blocked = _gms.CheckSkinBlocked(-1,SkinSelect);

        if (blocked)
        {
            Audio(false);
            _gms.NewInfo(/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(65) /*: "Esta Skin\nEsta Bloqueado!!!"*/, 5);
        }

        if (!blocked)
        {
            //print("SelectPlayer[" + (PlayerSelect) + "]: " + dropDown.captionText.text);

            Audio(true);

            _gms.NewInfo(_gms.AttDescriçãoMult(
                /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(66) /*:
                "Skin trocada para:\n{0}"*/,
                _gms.SkinName(-1, SkinSelect)), 2.5f);

            _gms.PlayerSkinID = SkinSelect;
        }

        dropDown.value = _gms.PlayerSkinID;
    }
  
    bool attExtras = false;
    public void AttExtras(bool check)
    {
        if (check && attExtras)
        {
            ActivePainel(2);
            return;
        }

        StartCoroutine(AttExtrasCoroutine());
    }
    IEnumerator AttExtrasCoroutine()
    {
        attExtras = true;

        int i = 0,loading = 0;

        _gms.LoadingBar("Loading Extras",0,_extras.Length);

        TotalHours(true);

        yield return waitLoad;

        #region Walk
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalWalkers;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(18)+" <b>" + loading + "</b>" /*: "Walk: <b>"+loading+"</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Turnos Player
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalTurnos;
        _extras[i].SetActive(loading > 0);          
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(19) + " <b>" + loading + "</b>" /*: "Turnos    [<color=blue><b>JOGADOR</b></color>]: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Dano Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalDanoCausado;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(20) + " <b>" + loading + "</b>" /*: "Dano    [<color=red><b>CAUSADO_</b></color>]: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Dano Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);       

        loading = _gms.TotalDanoRecebido;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(21) + " <b>" + loading + "</b>" /*: "Dano    [<color=red><b>RECEBIDO</b></color>]: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Dano DEFENDIDO
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);        

        loading = _gms.TotalDanoDefendido;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(22) + " <b>" + loading + "</b>" /*: "Dano    [<color=blue><b>DEFENDIDO</b></color>]: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Vida Recuperada
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);       

        loading = _gms.TotalVidaRecuperada;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(23) + " <b>" + loading + "</b>" /*: "Vida Recuperada: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Mortes Player
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        

        loading = _gms.TotalGameOver;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(24) + " <b>" + loading + "</b>" /*: "Mortes    [<color=blue><b>CAUSADO</b></color>]: <b>" + loading + "</b>"*/;

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Kills
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);       

        loading = _gms.TotalMorteMob;
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =/* !_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(25) + " <b>" + loading + "</b>" /*: "Mortes    [<color=red><b>INIMIGOS</b></color>]: <b>" + loading + "</b>"*/;


        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Dbuff Recebido
        #region Burn Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        

        loading = _gms.TotalBurn(true, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(26),//Dbuff {0} [_color=red;_b;RECEBIDO_/b;_/color;]:
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Poison Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        

        loading = _gms.TotalPoison(true, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(26),//Dbuff {0} [_color=red;_b;RECEBIDO_/b;_/color;]:
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Petrify Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalPetrify(true, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(26),//Dbuff {0} [_color=red;_b;RECEBIDO_/b;_/color;]:
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Stun Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        

        loading = _gms.TotalStun(true, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(26),//Dbuff {0} [_color=red;_b;RECEBIDO_/b;_/color;]:
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Stun, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Bleed Recebido
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalBleed(true, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(26),//Dbuff {0} [_color=red;_b;RECEBIDO_/b;_/color;]:
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion
        #endregion

        #region Dbuff Causado
        #region Burn Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalBurn(false, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text = 
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(27),//Dbuff {0} [<color=blue><b>CAUSADO_</b></color>]
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire,true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Poison Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalPoison(false, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(27),//Dbuff {0} [<color=blue><b>CAUSADO_</b></color>]
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region Petrify Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalPetrify(false, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(27),//Dbuff {0} [<color=blue><b>CAUSADO_</b></color>]
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region  Stun Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);

        loading = _gms.TotalStun(false, 0);
        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
            _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(27),//Dbuff {0} [<color=blue><b>CAUSADO_</b></color>]
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Stun, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region  Bleed Causado
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);        
        loading = _gms.TotalBleed(false, 0);

        _extras[i].SetActive(loading > 0);
        _extras[i].GetComponentInChildren<Text>().text =
          _gms.AttDescriçãoMult
            (XmlMenuInicial.Instance.Get(27),//Dbuff {0} [<color=blue><b>CAUSADO_</b></color>]
            XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed, true))
            + " <b>" + loading + "</b>";

        if (loading > 0)
        _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion            
        #endregion

        #region  Quadra Kill
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        loading = _gms.TotalQuadraKill;

        _extras[i].SetActive(/*loading > 0*/_gms.Adm);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(36) + " <b>" + loading + "</b>" /*: "Total <color=blue><b>Mortes Quadruplas</b></color>:  <b>" + loading + "</b>"*/;

        if (_extras[i].activeInHierarchy)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        #region  Penta Kill
        i++;
        _gms.LoadingBar("Loading Extras", i, _extras.Length);
        loading = _gms.TotalPentaKill;

        _extras[i].SetActive(/*loading > 0*/_gms.Adm);
        _extras[i].GetComponentInChildren<Text>().text = /*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(37) + " <b>" + loading + "</b>"/* : "Total <color=blue><b>Mortes Quíntuplas</b></color>:  <b>" + loading + "</b>"*/;

        if (_extras[i].activeInHierarchy)
            _painels[2].GetComponent<AjustarContent>().Alterar(true);
        yield return waitLoad;
        #endregion

        _gms.LoadingBar("Loading Extras", _extras.Length, _extras.Length);

        if (NeedShowSelectPlayer())
        {
            _gms.LoadingBar("Loading Extras...", 0, 1);

            if (_selectPlayer.interactable)
            AtualizaSelectPlayerDropDown();

            yield return waitLoad;

            _gms.LoadingBar("Loading Extras...", 1, 1);

            _painels[2].GetComponent<AjustarContent>().Alterar(true);

            //_selectPlayer.gameObject.SetActive(true);            
        }
        
        if (NeedShowSelectSkin())
        {
            _gms.LoadingBar("Loading Extras.", 0, 1);          

            yield return waitLoad;

            AtualizaSkinDropDown();

            //_selectSkinPlayer.value = /*_gms.CheckSkinBlocked(-1,-1) ? 0 : */_gms.PlayerSkinID;

            _gms.LoadingBar("Loading Extras.", 1, 1);

            _selectSkinPlayer.gameObject.SetActive(true);

            _painels[2].GetComponent<AjustarContent>().Alterar(true);           
        }

        ActivePainel(2);      

        _painels[2].GetComponent<AjustarContent>().Alterar(true);
    }

    bool NeedShowSelectPlayer()
    {
        bool _r = false;

        if (_gms.Adm)
            _r = true;

        if (_r != true)
        {
            int count = _gms.PlayerCount;
            int current = 0;
            int need = 1;

            for (int i = 1; i < count; i++)
            {
                if (!_gms.CheckMobBlocked(i))
                {
                    current++;
                }
            }

            _r = (need <= current);
        }

        _selectPlayer.interactable = (_r);

        _selectPlayer.captionText.text = _gms.HeroName();
        _selectPlayer.captionImage.sprite = _gms.SpritePerfil(-1);

        AtualizaSliderStatusMob(_gms.PlayerID-1);

        _selectPlayer.GetComponentsInChildren<Image>()[2].enabled = _r;

        return _r;
    }

    bool NeedShowSelectSkin()
    {
        int current = _gms.SkinCount(-1);
        int desblock = _gms.SkinCountDesblock(-1);
        int need    = 2;

        _selectSkinPlayer.interactable                                = desblock >= need;
        _selectSkinPlayer.GetComponentsInChildren<Image>()[2].enabled = desblock >= need;

        if (_gms.Adm)
            return true;

        return need <= current;
    }

    void TotalHours(bool extra = false,int mobID=-1)
    {
        if (_gms == null)
            return;
       
        if (extra)
        {
            if (mobID == -1)
                mobID = _gms.PlayerID;

            _hour = _gms.TotalTimeHourMob(mobID);

            _min = _gms.TotalTimeMinutesMob(mobID);

            _seg = _gms.TotalTimeSegMob(mobID);
        }
        else
        {
            _totalTimeHoursPainel1.text = _gms.Demo ? "<color=red>DEMO</color>\n" : "";

            _gms.CalculeTotalTimeAllPlayer();

            if (_gms.Adm)
                _totalTimeHoursPainel1.text += "<color=red>Administrador</color>\n";
            

            _totalTimeHoursPainel1.text += (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(85) /*: "Versão"*/)+" " + Application.version;

            _hour = _gms.TotalTimeHourAllPlayer;

            _min = _gms.TotalTimeMinutesAllPlayer;

            _seg = _gms.TotalTimeSegAllPlayer;
        }

        string hour = _hour <= 9 ? "0" + _hour.ToString("F0")  : _hour.ToString("F0"), 
               min  = _min  <= 9 ? "0" + _min .ToString("F0")  : _min .ToString("F0"), 
               seg  = _seg  <= 9 ? "0" + _seg .ToString("F0")  : _seg .ToString("F0");

        if (extra)
            _extras[0].GetComponentInChildren<Text>().text = (/*!_gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(17)/*: "Tempo de Jogo: "*/) + " <b>" + hour + ":" + min + ":" + seg + "</b>";
        else
            _totalTimeHoursPainel1.text += "\n" + hour + ":" + min + ":" + seg;
    }
    #endregion

    #region Battle Mode
   
    #endregion

    #region Cheats
    /// <summary>
    /// Only Teste Cheats
    /// </summary>
    /// <param name="id"></param>
    public void CheckCheat(InputField id)
    {
        if (id.text == "" || id.text == null || (_gms == null))
        {
            return;
        }

        string check = id.text;       

        foreach (var i in _cheats)
        {
            cheatValue  = -1;
            cheatValueY = "";

            string cheat = i._caracter;
            float  value  = -1;
            string valueY = "";

            //if (cheat.Contains("%") || cheat.Contains("°"))
            {
                if (cheat.Contains("%"))
                {
                    cheat = _gms.AttDescrição(cheat, "%", "");

                    string teste = _gms.AttDescrição(id.text, cheat, "");

                    if (teste.Length > 0)
                    {
                        print(teste);
                        valueY = teste;
                        check = _gms.AttDescrição(id.text, valueY.ToString(), "");

                        print(cheat + " - " + valueY + " / " + check);
                    }

                }

                if (valueY=="")
                    cheat = i._caracter;

                if (cheat.Contains("°"))
                {
                    cheat = _gms.AttDescrição(cheat, "°", "");

                    string teste = _gms.AttDescrição(id.text, cheat, "");

                    if (teste.Length > 0)
                    {
                        print(cheat + "\n" + teste);
                        value = System.Convert.ToSingle(teste);
                        check = _gms.AttDescrição(id.text, value.ToString(), "");

                        print(cheat + " - " + value + " / " + check);
                    }

                }
            }                      

            if (cheat == check)
            {
                print("Code: " + cheat + " / value "+value+" Active.");

                if (value!=-1)
                cheatValue = value;

                if (valueY!="")
                    cheatValueY = valueY;

                i._event.Invoke();

                id.text = "";

                return;
            }
        }

            if (_gms != null)
                _gms.NewInfo("<color=red>Codigo não Reconhecido!!!</color>", 1);
    }

    public void CheatCompleteAllAchievement()
    {
        StartCoroutine(CheatCompleteAllAchievementCoroutine());

        string trapaca = "Desbloqueando Todas as Conquistas!!!";

        _gms.NewInfo("<color=green>Trapaça Ativada\n" + trapaca + "</color>", 3);
    }
    IEnumerator CheatCompleteAllAchievementCoroutine()
    {
        int countAchievement = _gms._achievement.Count;

        int count=0,current=0;

        foreach (var c in _gms._achievement)
        {
            if (!c._complete)
                count++;
        }

        foreach (var c in _gms._achievement)
        {
            if (!c._complete)
            {
                current++;

                _gms.LoadingBar("Desbloqueando Conquista\n" + c._name, current, count);

                _gms.CheckAchievement(_gms.SeachIndexAchievement(c), c._max);

                yield return waitCheatLoad;
            }
        }
    }

    public void CheatCompleteAchievement()
    {
        print("CheatValue:" + cheatValue);
        string trapaca="";

        if (cheatValue < 0 || cheatValue >= _gms._achievement.Count)
            trapaca = "<color=red>Trapaça Ativada\nPorém Não encontrada!!!</color>";
        else
        {
            trapaca = "Desbloqueando Conquista " + _gms._achievement[(int)cheatValue]._name + " !!!";

            StartCoroutine(CheatCompleteAchievementCoroutine((int)cheatValue));
        }

        _gms.NewInfo("<color=green>Trapaça Ativada\n" + trapaca + "</color>", 3);
    }
    IEnumerator CheatCompleteAchievementCoroutine(int ID)
    {
        _gms.LoadingBar("Desbloqueando Conquista - " + _gms._achievement[ID]._name, 0);

        yield return new WaitForSeconds(0.5f);

        _gms.LoadingBar("Desbloqueando Conquista - " + _gms._achievement[ID]._name, 0);

        if (_gms._achievement[ID]._complete)
        {
            _gms.NewInfo("<color=red>Erro!!!\nConquista(" + _gms._achievement[ID]._name + ") ja esta desbloqueada!!!</color>", 3);
        }
        else
            _gms.CheckAchievement(ID, _gms._achievement[ID]._max,false);

        _gms.LoadingBar("Desbloqueando Conquista - " + _gms._achievement[ID]._name, 1);
    }


    public void CheatCompleteAllFases()
    {
        StartCoroutine(CheatCompleteAllFasesCoroutine());

        string trapaca = "Desbloqueando Todas as Fases";

        _gms.NewInfo("<color=green>Trapaça Ativada\n" + trapaca + "</color>", 3);
    }
    IEnumerator CheatCompleteAllFasesCoroutine()
    {
        int countHero     = _gms.PlayerCount;
        int countFase     = _gms.FaseCount;
        int countUsedHero = 0;
        int count         = 0;

        foreach (var h in _gms.Mob)
            if (!h._blocked)
                countUsedHero++;

        for (int hero = 0; hero < countHero; hero++)
        {
            if (!_gms.CheckMobBlocked(hero))
            {
                count++;

                for (int fase = 0; fase < countFase; fase++)
                {
                    if (_gms.Mob[hero]._Fases[_gms._dlf].Fase[fase]._completeKill   != true || 
                        _gms.Mob[hero]._Fases[_gms._dlf].Fase[fase]._completePortal != 1)//true)
                    {
                        if (!_gms.CheckBlockFase(fase, hero: hero+1))
                        {
                            //_gms.FaseSeg(fase, 99999, idPlayer: hero + 1);
                            //_gms.FaseMin(fase, 99999, idPlayer: hero + 1);
                            _gms.TimeRecord(fase,-1,-1,hero + 1);
                        }

                        if (_gms.Mob[hero]._Fases[_gms._dlf].Fase[fase]._completeKill != true)
                            _gms.CompleteFase(fase, true, hero + 1);

                        if (_gms.Mob[hero]._Fases[_gms._dlf].Fase[fase]._completePortal != 1)//true/* && _gms._Fases[hero]._Fases[fase]._completePortal !=null*/)
                            _gms.CompleteFase(fase, false, hero + 1);

                        _gms.LoadingBar((count) + " / " + (countUsedHero) + "\nDesbloqueando " + _gms.HeroName(hero) + " - " + _gms.NameFase(fase), fase, countFase - 1);
                        yield return waitCheatLoad;
                    }
                }

                if (!_gms.CheckMobBlocked(hero))
                    _gms.Vibrate();

                yield return _gms.waitDelayBar;
            }
        }
    }

    public void CheatGoToRandomFase()
    {
        int fase = Random.Range(0, _gms.FaseCount);

        string trapaca = "Go To " + _gms.NameFase(fase) + "!!!";

        _gms.NewInfo("<color=green>Trapaça Ativada\n" + trapaca + "</color>", 3);

        if (_gms.CheckMobBlocked() ||
            _gms.CheckMobBanned())
        {
            _gms.NewInfo("<color=red>" + _gms.HeroName(_gms.PlayerID) + ",não esta qualificado para tal coisa!!!</color>", 3);
            return;
        }

        StartCoroutine(CheatGoToRandomFaseCoroutine(fase));
    }
    IEnumerator CheatGoToRandomFaseCoroutine(int fase)
    {
        _gms._atualFase = fase;

        yield return waitCheatLoad;

        _gms.LoadLevel("GamePlay");
    }

    public void CheatDesblockMob()
    {
        print("CheatValue:" + cheatValue);
        int mobID = (int)cheatValue-1;

        string trapaca;

        if (mobID >= 0 || mobID < _gms.PlayerCount)
        {
            trapaca = "<color=green>Trapaça Ativada\nDesbloqueio do(a) " + _gms.HeroName(mobID);           

            StartCoroutine(CheatDesblockMobCoroutine(mobID));
        }
        else
            trapaca = "<color=red>Trapaça Ativada\nPorém Não encontrada!!!</color>";

        _gms.NewInfo(trapaca + "</color>", 3);
    }
    IEnumerator CheatDesblockMobCoroutine(int mobID)
    {
        _gms.LoadingBar("Desbloqueando "+_gms.HeroName(mobID),0);

        yield return new WaitForSeconds(0.5f);

        _gms.LoadingBar("Desbloqueando " + _gms.HeroName(mobID), 1);

        if (!_gms.CheckMobBlocked(mobID))
        {
            _gms.NewInfo("<color=red>Erro!!!\n"+_gms.HeroName(mobID)+" ja esta desbloqueado!!!</color>",3);
        }
        else
            _gms.MobDesblock(mobID);
    }

    public void CheatDesblockSkin()
    {
        print("CheatValue:" + cheatValue);
        int skinID = (int)cheatValue - 1;

        string trapaca;

        if (skinID >= 0 || skinID < _gms.SkinCount(-1)/* && _gms.Adm*/)
        {
            trapaca = "<color=green>Trapaça Ativada\nDesbloqueio da Skin <color=blue>" + _gms.SkinName(skinID: skinID)+"</color> do(a) "+_gms.HeroName(-1);

            StartCoroutine(CheatDesblockSkinCoroutine(skinID));
        }
        else
            trapaca = "<color=red>Trapaça Não encontrado!!!";

        _gms.NewInfo(trapaca + "</color>", 3);
    }
    IEnumerator CheatDesblockSkinCoroutine(int skinID)
    {
        _gms.LoadingBar("Desbloqueando skin ["+_gms.SkinName(skinID: skinID) +"] do " + _gms.HeroName(-1), 0);

        yield return new WaitForSeconds(0.5f);

        _gms.LoadLevel(0, "Desbloqueando Skin " + _gms.SkinName(skinID: skinID));

        _gms.SkinDesblock(-1,skinID,true);      
    }

    public void CheatGameAdm(string _name)
    {
        if (!_gms.Adm)
        {
            _gms.NewInfo("<color=green>Bem vindo\nSr(a) <b>" + _name + "</b>!!!</color>", 5);
            _gms.NewInfo("<color=green>Beneficios de Adm Ativos!!!</color>", 3);

            _gms.Adm = true;

            _gms.LoadLevel(0, "Carregando Benefios de Adm " + _name + "...");
        }
        else
        {
            _gms.NewInfo("<color=yellow>Sr(a) <b>" + _name + "</b>,Destativando Beneficios de Adm!!!</color>", 3);

            _gms.Adm = false;

            _gms.LoadLevel(0, _name + " Desligando Benefios de Adm...");
        }
    }

    public void CheatHistory(bool startGame)
    {
        print("CheatValue:" + cheatValue);

        string trapaca = "";

        if (HudHistory.Instance == null)
        {
            _gms.NewInfo("<color=red>Trapaça Não encontrada!!!\nInstance Not Found!!!</color>", 3);
            return;
        }

        if (cheatValue < 0 || cheatValue >= _gms.FaseCount)
        {
            trapaca = "<color=red>Trapaça Não encontrada!!!</color>";
        }
        else
        {
            trapaca = "<color=green>Trapaça Ativada\nIniciando Modo historia da fase " + cheatValue + "!!!</color>";

            StartCoroutine(CheatHistoryCoroutine((int)cheatValue, startGame));
        }

        _gms.NewInfo("" + trapaca + "", 3);
    }
    IEnumerator CheatHistoryCoroutine(int ID, bool startGame)
    {

        yield return new WaitForSeconds(0.5f);

        _gms.FaseAtual = ID;

        yield return new WaitForSeconds(1f);

        HudHistory.Instance.StartChat(startGame);
    }


    public void CheatTesteAchievement()
    {
        print("CheatValue:" + cheatValue);

        string trapaca = "";

        if (XmlAchievement.Instance == null)
        {
            _gms.NewInfo("<color=red>Trapaça Não encontrada!!!\nInstance Not Found!!!</color>", 3);
            return;
        }

        if (cheatValue < 0 || cheatValue >= _gms._achievement.Count)
        {
            trapaca = "<color=red>Trapaça Não encontrada!!!</color>";
        }
        else
        {
            trapaca = "<color=green>Trapaça Ativada\nBuscando Consquista -> " + cheatValue + "!!!</color>";

            StartCoroutine(CheatTesteAchievementCoroutine((int)cheatValue));
        }

        _gms.NewInfo("" + trapaca + "", 3);
    }
    IEnumerator CheatTesteAchievementCoroutine(int ID)
    {

        yield return new WaitForSeconds(0.5f);

        AchievementXml a = XmlAchievement.Instance.GetAchievement(ID);

        if (a!=null)              
        GameManagerScenes._gms.NewInfo(a._ID+" - "+a._nameX+" / "+a._description+" / "+a._type+" / "+a._dlc,5);
        else
            GameManagerScenes._gms.NewInfo("Erro", 1);
    }


    public void CheatTesteFindFound()
    {
        print("CheatValue:" + cheatValueY);

        string trapaca = "<color=green>Trapaça Ativada\nBuscando Arquivo -> " + cheatValueY + "</color>";

        StartCoroutine(CheatTesteFindFoundCoroutine(cheatValueY));

        _gms.NewInfo("" + trapaca + "", 3);
    }
    IEnumerator CheatTesteFindFoundCoroutine(string ID)
    {

        yield return new WaitForSeconds(0.5f);

        string arquivo = "HexaAdventureHistory0.xml";

       int    countFase  = GameManagerScenes._gms.FaseCount;
       string language   = GameManagerScenes._gms.Language().ToString();

        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(HistoryDatabase));

        string nameStream = "";

        ID = _gms.AttDescrição(ID,"$L",language, ID);

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + ID + arquivo;
#elif UNITY_ANDROID
        nameStream =Application.streamingAssetsPath + ID + arquivo;
#elif UNITY_IPHONE
        nameStream =Application.streamingAssetsPath + ID + arquivo;
#else
        nameStream =Application.streamingAssetsPath + ID + arquivo;
#endif

        GameManagerScenes._gms.NewInfo("[" + System.IO.File.Exists(nameStream) + "] - " + nameStream, 10);
    }

    public void CheatTesteBuildXml()
    {
        print("CheatValue:" + cheatValueY);

        string trapaca = "<color=green>Trapaça Ativada\nCriando Arquivo -> " + cheatValueY + "</color>";

        _gms.NewInfo("" + trapaca + "", 3);

        XmlMobPassive.Instance.CreateXML(cheatValueY);
    }

    public void CheatTesteCompleteFase()
    {

        if (cheatValue <0 || cheatValue> GameManagerScenes._gms.FaseCount)
        {
            _gms.NewInfo("Fase Não encontrada", 3);
            return;
        }
        print("CheatValue:" + cheatValue);

        string trapaca = "<color=green>Trapaça Ativada\nSalvando dados da fase = " + cheatValue + "</color>";

        StartCoroutine(CheatTesteCompleteFaseCoroutine((int)cheatValue));

        _gms.NewInfo("" + trapaca + "", 3);
    }
    IEnumerator CheatTesteCompleteFaseCoroutine(int ID)
    {

        yield return new WaitForSeconds(0.5f);


        int      R = Random.Range(0, 2);
        bool _kill = R == 0 ? false : true;
        float _seg = Random.Range(0, 60),
              _min = Random.Range(0, 60);


        GameManagerScenes._gms.CompleteFase(
            ID,
            _kill,
            _seg,
            _min);

        GameManagerScenes._gms.NewInfo(
            "Fase("+GameManagerScenes._gms.NameFase(ID)+") ["+ ID + "] "+
            "-> Kill ["+ _kill + "] / [ "+ _min + ":"+ _seg + " ] / "
            +GameManagerScenes._gms.NamePlayer, 
            10);
    }

    public void CheatDesblockBattleMode()
    {
        print("CheatValue:" + cheatValue);

        string trapaca = "<color=green>Trapaça Ativada\nDesbloqueando Modo Batalha -> " + cheatValue + "</color>";

        _gms.NewInfo("" + trapaca + "", 3);

        _gms.BattleModeDesblock();
    }

    public void CheatTestDescriptionAtt()
    {
        if (!_gms.Adm)
            return;
        print("CheatValue:" + cheatValue);

#if UNITY_EDITOR
        string trapaca = "<color=green>Trapaça Ativada\nDesbloqueando Modo Batalha -> " + cheatValue + "</color>";

        _gms.NewInfo("" + trapaca + "", 3);

        StartCoroutine(CheatTestDescriptionAttCoroutine());
#endif
    }

    IEnumerator CheatTestDescriptionAttCoroutine()
    {
        WaitForSeconds w = new WaitForSeconds(0.5f);

       foreach (var mob in _gms.Mob)
        {
            if (mob._prefabHero != null)
            {
               GameObject obj = Instantiate(mob._prefabHero/*.GetComponent<SkillManager>()*/);
               yield return w;
            }
        }
    }

    public void CheatDemoOff()
    {
        bool value = !_gms.Demo;

        StartCoroutine(CheatDemoOffCoroutine(value));

        string trapaca = "Modo Demo ";
        trapaca +=value ? "Ligado" : "Desligado";

        _gms.NewInfo("<color=green>Trapaça Ativada\n" + trapaca + "</color>", 3);
    }
    IEnumerator CheatDemoOffCoroutine(bool value)
    {
        string trapaca = value ? "Ligado" : "Desligado";
    
        _gms.LoadingBar(trapaca+"...", 0);

        yield return _gms.waitDelayBar;

        _gms.LoadingBar(trapaca + "...", 1);

        _gms.Demo = value;
    }
    #endregion
}
