using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent( typeof(CheckGrid),   typeof(RespawMob),typeof(TurnSystem))]
public class GridMap : MonoBehaviour
{
    public static GridMap Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }


    public GameObject[] objects;

    public int width, height;
    int[] build = new int[144];

    private int[,] pos;

    private float _width = 1.732F,
                  _height = 2,
                  _gap = 0;       //Espaço entre os hex

    private Vector3 _startPos;

    [Space(7), Header("Hex na Cena")]
    public List<GameObject> hex;

    public List<HexManager> hexManager;

    /// <summary>
    /// X
    /// </summary>
    public float Width { get {return width; } }
    /// <summary>
    /// Y
    /// </summary>
    public float Height { get{ return height; } }

    /// <summary>
    /// Cria a Grid
    /// </summary>
    public void CreateGrid/*void Start*/()
    {
        Gap();
        CalcStartPos();

        pos = new int[width, height];
        hex = new List<GameObject>(1);

        if ((width * height) != build.Length)
        {
            Debug.LogError("Erro no tamanho da lista contruicao, o tamanho tem q ser de " + width * height + " para evitar erros...");
            build = new int[width * height];
            Debug.LogError("Erro Corrigido: New Size(Contruição) = " + build.Length);
        }

        int _maxAtualBuild = GetComponent<RespawMob>().build.Length;

        for (int i = 0; i < _maxAtualBuild; i++)
        {
            build[i] = GetComponent<RespawMob>().build[i];
        }

        #region Create Grid
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                pos[x, y] = build[x * height + y];
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                GameObject obj = Instantiate(objects[pos[x, y]]) as GameObject;

                hex.Add(obj);
                hexManager.Add(obj.GetComponent<HexManager>());

                Vector2 posBidimencional = new Vector2(x, y);
                obj.transform.position = WorldPosition(posBidimencional);
                obj.transform.parent = this.transform.parent;
                obj.name = "Hex" + x + "x" + y;

                //print("Obj: " + pos[x, y] + "  Instaciando: x " + x + " y " + y + " pos:" + posBidimencional);

                obj.transform.SetParent(this.transform);

                obj.GetComponent<HexManager>().free = true;
                obj.GetComponent<HexManager>().puxeItem = false;

                obj.GetComponent<HexManager>().floorType = pos[x, y];
                obj.GetComponent<HexManager>().x = x;
                obj.GetComponent<HexManager>().y = y;
            }
        }
        #endregion

        Debug.LogWarning("__CreateGrid Complete__");

        if (CheckGrid.Instance != null)
            CheckGrid.Instance.SetUp();
        else
        {
            CheckGrid CG = GameObject.FindObjectOfType<CheckGrid>();

            CG.SetUp();
        }

        EffectManager.Instance.StartPollEffects();

            //Inica A historia 
            if (GameManagerScenes._gms.GameMode == Game_Mode.History &&
                HudHistory.Instance != null && 
                !GameManagerScenes._gms.CheckCompleteFase(-1, -1, -1))
                HudHistory.Instance.StartChat();
            else
                GetComponent<RespawMob>().EventRespawMobFase();
    }

    public void FreeHex(GameObject obj,bool free=true)
    {
        foreach (var _hex in hexManager)
        {
            if (_hex.currentMob == gameObject)
            {
                _hex.currentMob = null;
                _hex.free = free;
            }
        }
    }

    void CalcStartPos()
    {
        float offset = 0;
        if (width / 2 % 2 != 0)
            offset = height / 2;

        float x = -height * (height / 2) - offset + 60;
        float z = width * 0.75f * (width / 2);

        _startPos = new Vector3(x, 0, z);
    }

    void Gap()
    {
        _height += _height * _gap;
        _width += _width * _gap;
    }

    Vector3 WorldPosition(Vector2 _gridPosition)// burracos nas pontas das linhas da grid
    {
        float offset = 0;
        if (_gridPosition.y % 2 == 1)
            offset = _width / 2;

        float x = _startPos.x + _gridPosition.x * _width + offset;
        float y = _startPos.y + _gridPosition.y * _height * 0.75f;

        return new Vector3(x, 0, y);
    }
}
