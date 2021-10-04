using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]
public class PortalManager : MonoBehaviour
{
    GameObject Hexagon;
    [HideInInspector]
    public HexManager Here;
    MoveController moveController;
    AudioSource audioSource;
    [SerializeField]
    EffectManager effect;

    bool playerInTeleport=false;

    WaitForSeconds wait = new WaitForSeconds(2);

    public List<GameObject> Player = new List<GameObject>();

    public void Start()
    {
        effect         = EffectManager.Instance;

        audioSource    = GetComponent<AudioSource>();

        moveController = GetComponent<MoveController>();

        Hexagon        = GameObject.FindGameObjectWithTag("Manager");

        GameObject obj = GameObject.Find("Hex" + moveController.hexagonX + "x" + moveController.hexagonY);

        Here           = obj.GetComponent<HexManager>();

        Here.free      = true;

        Here.puxeItem = false;

        if (RespawMob.Instance.Player != null)
            LookAtPlayer(RespawMob.Instance.Player);
    }

    public void CheckInHere()
    {
        if (Here == null)
            return;       
          
        if (Here.currentMob!=null)
        {
            if (GetComponent<BoxCollider>().enabled)
                GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            if (!GetComponent<BoxCollider>().enabled)
                GetComponent<BoxCollider>().enabled = true;
            return;
        }

        if (Here.currentMob.GetComponent<MobManager>()==null)
            return;

        if (Here.currentMob.GetComponent<MobManager>().TimeMob != MobManager.MobTime.Player)
            return;

        if (!Player.Contains(Here.currentMob) /*&& GameManagerScenes._gms.GameMode == Game_Mode.History*/)
        {
            EffectManager.Instance.PopUpDamageEffect(
               XmlMenuInicial.Instance.Get(168)//Apenas o verdadeiro Player pode ativar o portal
              , Here.currentMob);
            return;
        }

        if (audioSource!=null && !audioSource.isPlaying)
            audioSource.Play();

        if (!playerInTeleport && effect!=null)
        {
            Here.currentMob.GetComponent<MobManager>().myTurn = false;  
                                 
            effect.TeleportEffect(Here.currentMob);

            Here.currentMob.transform.rotation = transform.rotation;

            playerInTeleport = true;

            Hexagon.GetComponent<ButtonManager>().ClearHUD();
        }

        StartCoroutine(PlayerInPortal());
    }

    IEnumerator PlayerInPortal()
    {
        print("Player Chegou no Portal");

        yield return wait;

        //effect.TeleportReset(Here.currentMob);

        Hexagon.GetComponent<TurnSystem>().GameOver(-1,
           XmlMenuInicial.Instance.Get(169) //"_b;PARABÉNS_/b;\n Você concluiu essa fase após entrar no portal."
            , false, false);
            
        playerInTeleport = false;
    }

    public void LookAtPlayer(GameObject target)
    {
        this.transform.LookAt(target.transform);
    }
}
