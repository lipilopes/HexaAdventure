using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerGUI : MonoBehaviour
{
    public static PlayerGUI Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        if (_Awake)
        {
            SpritePerfil.sprite = GameManagerScenes._gms.SpritePerfilPlayer;

            SpritePerfil.type = Image.Type.Simple;

            SpritePerfil.preserveAspect = true;
        }        
    }

    [SerializeField] bool _Awake;
    [SerializeField] Image SpritePerfil;

    MobHealth player;

    [Header("HP")]
    [SerializeField] GameObject hp;
    [SerializeField] Image hpBarCurrent;
    [SerializeField] Image hpBackBar;
    [Header("Defense")]
    [SerializeField] GameObject defense;
    [SerializeField] Image defenseBarCurrent;
    [SerializeField] Image defenseBackBar;

    WaitForSeconds wait   = new WaitForSeconds(0.05f);

    public void SpritePerfilChange(Sprite s)
    {
        SpritePerfil.sprite = s;

        SpritePerfil.preserveAspect = true;

        SpritePerfil.type = Image.Type.Simple;
    }

    public void GetPlayer()
    {
        if (RespawMob.Instance != null)
        {
            player = RespawMob.Instance.Player.GetComponent<MobHealth>();

            defense.SetActive(player.defense > 0);

            hp.SetActive(player.Health > 0);

           defenseBarCurrent.fillAmount      = player.defense / player.maxDefense;
           defenseBackBar.fillAmount        = defenseBarCurrent.fillAmount;

            hpBarCurrent.fillAmount = player.Health / player.MaxHealth;
            hpBackBar.fillAmount    = hpBarCurrent.fillAmount;

            SpritePerfilChange(GameManagerScenes._gms.SpritePerfil(player.GetComponent<ToolTipType>()._name));

            if (GameManagerScenes._gms.Adm)
                InfoTable.Instance.NewInfo("Player Hud alterado para " + player.name, 3);
        }      
    }

    public void GetPlayer(GameObject _player)
    {
        if (_player != null && _player.GetComponent<MobHealth>())
        {
            player = _player.GetComponent<MobHealth>();

            defense.SetActive(player.defense > 0);

            hp.SetActive(player.Health > 0);

            //defenseBarCurrent.fillAmount      = player.defense / player.maxDefense;
            // defenseBackBar.fillAmount         = defenseBarCurrent.fillAmount;

            // hpBarCurrent.fillAmount = player.Health / player.MaxHealth;
            // hpBackBar.fillAmount    = hpBarCurrent.fillAmount;            

            /* if (GameManagerScenes._gms.Adm)
                 InfoTable.Instance.NewInfo("Player Hud alterado para " + player.name, 3);*/

            SpritePerfilChange(GameManagerScenes._gms.SpritePerfil(_player.GetComponent<ToolTipType>()._name));

            print("Player Hud alterado para " + player.name);

            AttBarHP();
        }
    }

    public void AttBarHP()
    {
        if (player == null)
            GetPlayer();

        defense.SetActive(player.defense > 0);

        if (player.Health > 0)
            hp.SetActive(true);

        if (defense.activeInHierarchy)
        {
            defenseBarCurrent.fillAmount = player.defense / player.maxDefense;
            StartCoroutine(BackDefenseBarAtt());
        }

      
        if (hp.activeInHierarchy)
        {
            hpBarCurrent.fillAmount = player.Health / player.MaxHealth;
            StartCoroutine(BackHpBarAtt());
        }
    }

    IEnumerator BackHpBarAtt()
    {
        yield return wait;

        while (hpBarCurrent.fillAmount != hpBackBar.fillAmount)
        {
            if (hpBarCurrent.fillAmount > hpBackBar.fillAmount)
            {
                hpBackBar.fillAmount = hpBarCurrent.fillAmount;
                break;
            }
            else
                if (hpBarCurrent.fillAmount < hpBackBar.fillAmount)
            {
                if ((hpBarCurrent.fillAmount - hpBackBar.fillAmount) <= 0.77f)
                    hpBackBar.fillAmount -= 0.001f;
                else
                    hpBackBar.fillAmount -= 0.0008f;

                yield return null;
            }
        }

        hp.SetActive(player.Health > 0);
    }
    IEnumerator BackDefenseBarAtt()
    {
        yield return wait;

        while (defenseBarCurrent.fillAmount != defenseBackBar.fillAmount)
        {
            if (defenseBarCurrent.fillAmount > defenseBackBar.fillAmount)
            {
                defenseBackBar.fillAmount = defenseBarCurrent.fillAmount;
                break;
            }
            else
                if (defenseBarCurrent.fillAmount < defenseBackBar.fillAmount)
            {
                if ((defenseBarCurrent.fillAmount - defenseBackBar.fillAmount) <= 0.77f)
                    defenseBackBar.fillAmount -= 0.001f;
                else
                    defenseBackBar.fillAmount -= 0.0008f;

                yield return null;
            }
        }

        defense.SetActive(player.defense > 0);
    }
}
