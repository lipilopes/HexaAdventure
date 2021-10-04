using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HudHistory : MonoBehaviour
{
    public static HudHistory Instance;

    //[SerializeField] string _nameXml = "HexaAdventureHistory";
    [Header("HUD")]
    [SerializeField] protected GameObject _hudHistory;
    [Space]
    [Header("Left")]
    [SerializeField] protected CanvasGroup _hudBallonLeft;
    [SerializeField] protected Image       _hudSpriteChatLeft;
    [SerializeField] protected Text        _hudNameBallonLeft;
    [SerializeField] protected Text        _hudChatBallonLeft;
    [Space]
    [Header("Right")]
    [SerializeField] protected CanvasGroup _hudBallonRight;
    [SerializeField] protected Image       _hudSpriteChatRight;
    [SerializeField] protected Text        _hudNameBallonRight;
    [SerializeField] protected Text        _hudChatBallonRight;
    [Space]
    [SerializeField] float _timeToPass = 4;
    WaitForSeconds timeToPass;

    [SerializeField]
    private List<History> currentHistory = new List<History>();

    /// <summary>
    /// Tempo de espera para esperar o Game Manager
    /// </summary>
    WaitForSeconds wait   = new WaitForSeconds(1);
    /// <summary>
    /// Tempo de espera pra pular as falas da historia
    /// </summary>
    WaitForSeconds update = new WaitForSeconds(1f);

    bool? _startGame = null;

    public bool HistoryPass = false;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        if (_hudHistory!=null)
        _hudHistory.gameObject.SetActive(false);

        timeToPass = new WaitForSeconds(_timeToPass);
    }

    protected IEnumerator UpdateCoroutine()
    {
        while (_hudHistory.gameObject.activeSelf)
        {
            yield return update;

            #if UNITY_ANDROID || UNITY_IOS || UNITY_WP8
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                print("Touch");
                PassChat();
            }
#else
               if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                print("Click");
                PassChat();
            }
#endif          
        }        
    }

    IEnumerator TimePassChat()
    {
        yield return timeToPass;

        if (_hudHistory.gameObject.activeSelf)
            PassChat();
    }

    //Chamado apos criar o mapa e antes de criar os mobs
    public void StartChat(bool startGame=true)
    {
        print("StartChat(" + startGame + ")");

        HistoryPass = true;

        if (!startGame)//Para de contar tempo
        {
            if (TotalTime.Instance!=null)
            TotalTime.Instance.PauseTimer();
        }

        //if (GameManagerScenes._gms.IsMobile)
        //{
        //    SkipHistory();
        //    return;
        //}

        _startGame = startGame;

        if (!GameManagerScenes._gms.IsMobile)
            currentHistory = XmlChatFases.Instance.GetHistory(GameManagerScenes._gms.FaseAtual, startGame);
        else
            Debug.LogError("Mobile Cant Seach Xml");

        if (currentHistory.Count <= 0)
        {
            SkipHistory();
            return;
        }

       // CheckAndChangeWords();

        _hudHistory.gameObject.SetActive(true);

        PassChat();

        StartCoroutine(UpdateCoroutine());               
    }

    public void StartChat(List<History> listHistory)
    {
        print("StartChat()");

        HistoryPass = true;

        //if (GameManagerScenes._gms.IsMobile)
        //{
        //    SkipHistory();
        //    return;
        //}

        currentHistory = listHistory;

        if (currentHistory.Count <= 0)
        {
            SkipHistory();
            return;
        }

        // CheckAndChangeWords();

        _hudHistory.gameObject.SetActive(true);

        PassChat();

        StartCoroutine(UpdateCoroutine());
    }

    public void PassChat()
    {        
        StopCoroutine(TimePassChat());

        if (!_hudHistory.gameObject.activeSelf)        
            return;

        print("PassChat()");

        if (currentHistory.Count <= 0)
        {
            SkipHistory();
            return;
        }

        History chat = currentHistory[0];

        currentHistory.Remove(chat);

        if (chat._left)
        {
            print("PassChat() -> Left");

            _hudBallonRight.alpha = _hudSpriteChatRight.sprite == null ? 0 : 0.4f;
            _hudBallonLeft.alpha  = 1f;

            _hudSpriteChatLeft.sprite = GameManagerScenes._gms.SpriteChat(chat._mobID);
            _hudNameBallonLeft.text   = GameManagerScenes._gms.HeroName(chat._mobID);
            _hudChatBallonLeft.text   = chat._chat;

        }
        else
        {
            print("PassChat() -> Right");

            _hudBallonRight.alpha = 1f;
            _hudBallonLeft.alpha  = _hudSpriteChatLeft.sprite == null ? 0 : 0.4f;

            _hudSpriteChatRight.sprite = GameManagerScenes._gms.SpriteChat(chat._mobID);
            _hudNameBallonRight.text   = GameManagerScenes._gms.HeroName(chat._mobID);
            _hudChatBallonRight.text   = chat._chat;
        }

        StartCoroutine(TimePassChat());
    }

    public void SkipHistory()
    {
        print("SkipHistory()");

        StopCoroutine(UpdateCoroutine());
        StopCoroutine(TimePassChat());

        _hudSpriteChatLeft.sprite  = null;
        _hudSpriteChatRight.sprite = null;
        _hudHistory.gameObject.SetActive(false);

        currentHistory.Clear();

        if (_startGame==true)
        {
            //Start Game
            if (GridMap.Instance!=null)
            {
                //RespawMob.Instance.StartTurn();
                RespawMob.Instance.EventRespawMobFase();
            }
        }
        else
        {
            //Game over
        }

        HistoryPass = false;
    }
}
