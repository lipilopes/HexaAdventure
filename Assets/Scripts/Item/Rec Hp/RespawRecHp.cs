using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawRecHp : RespawItem
{
    public static RespawRecHp Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

    public override void ConfigItem()
    {
        StartRespaw();

        StartCoroutine(ConfigCoroutine());
    }
    IEnumerator ConfigCoroutine()
    {       
            while (respawmob.Player == null)
            {
                yield return null;
            }

        yield return new WaitForSeconds(1);

        //gms.NewInfo("Health player is " + grid.player.GetComponent<MobManager>().health, 10);

        needToShow = (int)((int)respawmob.Player.GetComponent<MobManager>().health - (((int)respawmob.Player.GetComponent<MobManager>().health * _respawItemSetUps[gms.FaseAtual]._porcentToShow) / 100));

        maxRespawPorFase = _respawItemSetUps[gms.FaseAtual]._maxRespawporfase;
        maxItemInFase    = _respawItemSetUps[gms.FaseAtual]._maxRespawporfase;
        chanceToShow     = _respawItemSetUps[gms.FaseAtual]._chanceShow / 100;

        AtivedHex();

        string RecHP = itemList[0].GetComponent<ItemRecHp>().RecHpPlayer.ToString("F0");

        if (RecHP == "0")
            RecHP = (((int)respawmob.Player.GetComponent<MobManager>().health * itemList[0].GetComponent<ItemRecHp>().RecHpPlayerPorc) / 100) + " de Hp";
        else
            RecHP += " de Hp";

        infoTable.NewInfo(
            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(187)//"Item _b;Rec Hp_/b;, tem _b;{0}%_/b; de chance de aparecer quando seu Hp estiver inferior a _b;{1}_/b;  Total de _b;{2}_/b; Podendo ter no maximo _b;{3}_/b; no mapa.."
                ,"" + chanceToShow * 100
                ,""+ needToShow
                ,""+ maxItemInFase
                ,""+ maxRespawPorFase)
            , 10);

        AttIconDesc(
            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(188)//Item <b>Rec Hp</b>, \n tem <b>{0}% </b> de chance de aparecer \n quando seu Hp estiver inferior a <b>{1}</b> \nTotal de <b>{2}</b> Podendo ter no maximo <b>{3}</b> no mapa.\nRecupera: {4}\nRestam: <b>{5}</b>.
                , "" + chanceToShow * 100
                , "" + needToShow
                , "" + maxItemInFase
                , "" + maxRespawPorFase
                , "" + RecHP
                , "" + (maxItemInFase - currentRespawPorFase)));
    }


    public override void CheckItem()
    {
        if (GameManagerScenes.BattleMode)
            return;

        base.CheckItem();

        if (respawmob.Player == null)
            return;

        if (respawmob.Player.GetComponent<MobHealth>().Health > needToShow || (maxItemInFase - currentRespawPorFase)<=0)
            return;

        float V = Random.value;

        if (V <= chanceToShow)
        {
            for (int i = 0; i < itemList.Count; i++)
            {
                if (!itemList[i].activeSelf)
                {
                    if (MoveItem(itemList[i], Random.Range(0, 12), Random.Range(0, 12)))
                    {
                        itemList[i].SetActive(true);

                        respawPorFase++;

                        currentRespawPorFase++;

                        Debug.LogWarning("Rec hp Respawnou");

                        infoTable.NewInfo(msgRespaw[Random.Range(0, msgRespaw.Length)] + " \n "+ XmlMenuInicial.Instance.Get(189)+": " + (maxItemInFase - currentRespawPorFase) + ".", 5);//Item _b;Rec Hp_/b; Apareceu.Restam

                        AttIconDesc(
                            GameManagerScenes._gms.AttDescriçãoMult(
                XmlMenuInicial.Instance.Get(188)//Item <b>Rec Hp</b>, \n tem <b>{0}% </b> de chance de aparecer \n quando seu Hp estiver inferior a <b>{1}</b> \nTotal de <b>{2}</b> Podendo ter no maximo <b>{3}</b> no mapa.\nRecupera: {4}\nRestam: <b>{5}</b>.
                , "" + chanceToShow * 100
                , "" + needToShow
                , "" + maxItemInFase
                , "" + maxRespawPorFase
                , "" + itemList[i].GetComponent<ItemRecHp>().RecHpPlayer.ToString("F0")
                , "" + (maxItemInFase - currentRespawPorFase)));

                        
            return;
                    }
                    else
                    {
                        CheckItem();
                        return;
                    }
                }
            }
        }
        else
            Debug.LogWarning("Iten Nao respawnou por q você esta com azar");
    }
}
