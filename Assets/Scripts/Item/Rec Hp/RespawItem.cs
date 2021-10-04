using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RespawItemSetUp
{
    [Tooltip("% para aparecer o item"), Range(0, 100)]
    public float _porcentToShow;

    [Tooltip("% de chance de aparecer"), Range(0, 100)]
    public float _chanceShow;

    [Tooltip("max de respaw por fase")]
    public int   _maxRespawporfase;

    [Tooltip("max de item na fase")]
    public int   _maxitem;
}

public class RespawItem : MonoBehaviour
{
    protected InfoTable     infoTable;
    protected EffectManager effectManager;
    protected ButtonManager grid;
    protected RespawMob     respawmob;
    protected GameManagerScenes gms;

    [SerializeField]
    protected List<RespawItemSetUp> _respawItemSetUps = new List<RespawItemSetUp>();
    [Space]
    [SerializeField]
    protected GameObject _iconItem;
    [SerializeField]
    protected string[] msgRespaw;

    [Header("Item")]
    [SerializeField]
    protected GameObject       itemPrefab;
    [SerializeField]
    protected List<GameObject> itemList;

    [Header("Propriedades")]
    [Range(1, 100)][Tooltip("Player Need to show item")]
    protected float needToShow;
    [Range(0, 1)][Tooltip("Chance Do item Aparece na cena")]
    protected float chanceToShow;

    protected int   respawPorFase,       ///quantas vezes vc ja respawnou esse item na fase
                    currentRespawPorFase,///Contagem de quantas vezes vc ja respawnou esse item na fase
                    maxHexInMap;        ///quantos hex podem receber o item rec hp

    protected int maxRespawPorFase, ///max de respaw por fase
                 maxItemInFase;    ///max de item na fase
    //public bool PreencherMap;

    public virtual void StartRespaw()
    {
        if (GameManagerScenes.BattleMode)
        {
            return;
        }

        grid          = ButtonManager.Instance;
        respawmob     = RespawMob.Instance;
        infoTable     = InfoTable.Instance;
        effectManager = EffectManager.Instance;
        gms = GameManagerScenes._gms;

        if (gms.FaseCount != _respawItemSetUps.Count)
            Debug.LogError("Respaw Item SetUp não contem todas as fases");
        else
        maxItemInFase = _respawItemSetUps[gms.FaseAtual]._maxRespawporfase;

        if (itemList.Count < maxItemInFase)
        {
            for (int i = 0; i < maxItemInFase; i++)
            {
                GameObject obj = Instantiate(itemPrefab);

                itemList.Add(obj);

                obj.name = itemPrefab.name + i;

                obj.transform.SetParent(transform);

                respawmob.allRespaws.Add(obj);

                //MoveItem(obj,Random.Range(0,12), Random.Range(0, 12),false);

                obj.SetActive(false);
            }
        }
    }

    public virtual void ConfigItem()
    {
        StartRespaw();

        if (respawmob.Player == null)
        {
            while (respawmob.Player != null)
            {

            }       
        }    

       maxRespawPorFase = _respawItemSetUps[gms.FaseAtual]._maxRespawporfase;
       maxItemInFase    = _respawItemSetUps[gms.FaseAtual]._maxRespawporfase;
       chanceToShow     = _respawItemSetUps[gms.FaseAtual]._chanceShow/100;

       AtivedHex();     
    }

    public virtual void CheckItem()
    {
        if (GameManagerScenes.BattleMode)
            return;

        if (!AtivedHex())
        {
            Debug.LogWarning("Itens Ativos na cena atingiu o limite de casas");
            return;
        }

        int itensAtivos = 0;

        for (int i = 0; i < itemList.Count; i++)
        {
            if (itemList[i].activeSelf)
            {
                itensAtivos++;                
            }
        }

        _iconItem.SetActive(!(itensAtivos >= maxItemInFase || respawPorFase >= maxRespawPorFase));

        if (!_iconItem.activeInHierarchy)       
            return; 
    }

    protected virtual bool AtivedHex()
    {
        GridMap HexInMap = (GridMap)FindObjectOfType(typeof(GridMap));

        maxHexInMap = HexInMap.hex.Count;

        for (int i = 0; i < HexInMap.hex.Count; i++)
        {
            HexManager hex = HexInMap.hexManager[i];

            if (!hex.free || hex.currentItem!=null)
            {
                maxHexInMap--;
            }
        }

        return maxHexInMap != 0;
    }

    protected virtual bool MoveItem(GameObject obj, int X, int Y,bool reg=true)
    {

        GameObject Here = GameObject.Find("Hex" + X + "x" + Y);

        if (Here != null)
        {
            if (Here.GetComponent<HexManager>().free && Here.GetComponent<HexManager>().currentItem==null)
            {
                obj.GetComponent<ItemRecHp>().AttPosition(Here.GetComponent<HexManager>());

                Here.GetComponent<HexManager>().puxeItem = true;

                if (reg)//registrar
                    Here.GetComponent<HexManager>().currentItem = obj;

                obj.transform.position = new Vector3(Here.GetComponent<Transform>().position.x, 0, Here.GetComponent<Transform>().position.z);
                return true;
            }
        }

        return false;
    }

    protected virtual void AttIconDesc(string desc)
    {
        _iconItem.GetComponent<ToolTipType>()._descricao = desc;
    }

    //void Update()
    //{
    //    if (PreencherMap)
    //    {
    //        if (AtivedHex())
    //            CheckItem();
    //        //else
    //        //    PreencherMap = false;
    //    }
    //}
}
