using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRecHp : MonoBehaviour
{
   public HexManager Here;

    [SerializeField][Range(1,100)]
    float recHpPlayer,
          recHpMob=25;

    public float RecHpPlayer
    {
        get
        {
            if (RespawMob.Instance.Player != null)
                return RespawMob.Instance.Player.GetComponent<MobManager>().health * recHpPlayer / 100;
            else
                return 0;
        }
    }

    public float RecHpPlayerPorc { get { return recHpPlayer; }  set { recHpPlayer = value; } }
    public float RecHpMobPorc    { get { return recHpMob;    }  set { recHpMob    = value; } }

    public void AttPosition(HexManager obj)
    {
        if (obj == null)
            return;

        if (obj.currentItem==null && obj.free)
        {
            if (Here != null)
            {
                Here.currentItem = null;
                Here.puxeItem = false;           
            }
            

            Here               = obj;
            Here.currentItem   = gameObject;
            Here.puxeItem      = true;
            transform.position = Here.transform.position;
        }        
    }

    public void Pull(HexManager obj)
    {
        if (Here != null)
        {
            Here.currentItem = null;
            Here.puxeItem    = false;
        }

        Here               = obj;
        Here.currentItem   = gameObject;
        Here.puxeItem      = true;
        transform.position = Here.transform.position;

        CheckInHere();
    }

    void OnDisable()
    {
        if (Here != null)
        {
            Here.currentItem = null;
            Here.puxeItem    = false;
        }
    }

    public void CheckInHere()
    {
        if (Here == null)
            return;
        if (Here.GetComponent<HexManager>().free)
            return;
        RecHp();
    }

    void RecHp()
    {
        if (Here.currentMob == null)
            return;

        GameObject obj = Here.currentMob;

        if (obj.GetComponent<MobHealth>() == null)
            return;
        float recHp = 0;

        if (!obj.GetComponent<MobManager>().isPlayer)
        {
            recHp = obj.GetComponent<MobManager>().health * recHpMob / 100;
        }
        else
        {
            recHp = obj.GetComponent<MobManager>().health * recHpPlayer / 100;
            obj.GetComponent<MobDbuff>().ClearDbuff();
        }
            

        obj.GetComponent<MobHealth>().RecHp(null,(int)recHp);

        Here.puxeItem        = false;
        Here.currentItem     = null;
        gameObject.SetActive(false);
    }

}
