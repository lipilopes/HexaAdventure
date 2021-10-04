using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectStigmaUltimoSuspiro : EffectStigma
{
    [Space]
    [Header("Ultimo suspiro")]
    [SerializeField, Tooltip("Caso for o player controla o clone")]
    protected bool    _controlTarget = true;
    [SerializeField, Tooltip("")]
    protected Material _ultimoSuspiroMat;
    [SerializeField, Tooltip("")]
    protected Color   _ultimoSuspiroColor = new Color();


    protected override void OnEnable()
    {
        base.OnEnable();

        if (TurnSystem.Instance)
        {
            if (!currentTurn)
                TurnSystem.DelegateTurnCurrent += Effect;
        }

        Effect();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (TurnSystem.Instance)
        {
            if (!currentTurn)
                TurnSystem.DelegateTurnCurrent -= Effect;
        }
    }

    public override void TimerStigma()
    {
        timer--;

        if (EffectManager.Instance != null && targetStigma != null)
            EffectManager.Instance.PopUpDamageEffect((duration - timer) + "/" + duration, targetStigma);

        if (targetStigma != null && !targetStigma.GetComponent<MobManager>().Alive)
        {
            Effect();
            return;
        }

        if (timer <= 0 && targetStigma != null)
            EndStigma();
    }

    protected override void EndStigma()
    {
        Effect();

        base.EndStigma();
    }

    public void Teste(GameObject _teste)
    {
        targetStigma = _teste;

        target = _teste;

        int count = targetStigma.GetComponent<MeshRenderer>().materials.Length;

        if (_ultimoSuspiroMat != null)
        {
            count += 1;

            Material[] mats = new Material[count];

            if (_ultimoSuspiroMat != null)
            {
                mats[0] = _ultimoSuspiroMat;

                for (int i = 0; i < count - 1; i++)
                {                 
                    mats[i + 1] = targetStigma.GetComponent<MeshRenderer>().materials[i];
                }
            }

            targetStigma.GetComponent<MeshRenderer>().materials = mats;
        }

        if (_ultimoSuspiroColor != new Color())
        {
            for (int i = 0; i < count; i++)
            {
                Material _Mat = /*new Material*/(targetStigma.GetComponent<MeshRenderer>().materials[i]);

                //_Mat.name = "New Mat(" + i + ")";

                if (_Mat.GetColor("Base Color") != new Color())
                {
                    _Mat.SetColor("Base Color", _ultimoSuspiroColor);

                    print("Base Color = "+ _Mat.GetColor("Base Color")+" "+ _Mat.shader);
                }
                else
                    if (_Mat.GetColor("_Color") != new Color())
                {
                    _Mat.SetColor("_Color", _ultimoSuspiroColor);

                    print("_Color = " + _Mat.GetColor("_Color") + " " + _Mat.shader);
                }

                targetStigma.GetComponent<MeshRenderer>().materials[i] = _Mat;
            }
        }
    }

    protected override void Effect()
    {
        if (targetStigma != null && !targetStigma.GetComponent<MobManager>().Alive)
        {
            if (GameManagerScenes._gms)
                GameManagerScenes._gms.NewInfo(targetStigma.name + " renascera como um aliado do thanatos", 3, true);

            targetStigma.GetComponent<MobManager>().TimeMob = user.GetComponent<MobManager>().TimeMob;

            targetStigma.GetComponent<MobManager>().getBonusPlayer = false;

            if (durationEffect>0)
            {
                targetStigma.AddComponent<EffectStigma>();
                targetStigma.GetComponent<EffectStigma>().desactiveThis = false;
                targetStigma.GetComponent<EffectStigma>().StartStigma(null,user,targetStigma,durationEffect,false);
            }

            #region Control
            if (user.GetComponent<MobManager>().isPlayer && _controlTarget)
            {
                targetStigma.GetComponent<MobManager>().isPlayer = true;
                targetStigma.AddComponent<PlayerControl>();
            }
            else
                if (targetStigma.GetComponent<PlayerControl>())
            {
                Destroy(targetStigma.GetComponent<PlayerControl>());
            }
            #endregion

            if (targetStigma.GetComponent<ToolTipType>())
            {
                targetStigma.GetComponent<ToolTipType>()._classe = targetStigma.GetComponent<MobManager>().classe.ToString() + " - " + (targetStigma.GetComponent<MobManager>().MesmoTime(user) ? "<color=green>Aliado</color>" : targetStigma.GetComponent<MobManager>().TimeMob.ToString());

                targetStigma.GetComponent<ToolTipType>().AttToltip();
            }

            if (RespawMob.Instance != null && targetStigma.GetComponent<MobManager>().TimeMob != (RespawMob.Instance.PlayerTime))
                if (targetStigma.GetComponentInChildren<Effects>())
                    targetStigma.GetComponentInChildren<Effects>().gameObject.SetActive((int)GameManagerScenes._gms.Dificuldade() >= 2 ? false : true);

            targetStigma.GetComponent<MobHealth>().ReBorn(true);

            if (targetStigma.GetComponent<MeshRenderer>())
            {
                int count = targetStigma.GetComponent<MeshRenderer>().materials.Length;

                if (_ultimoSuspiroMat != null)
                {
                    count += 1;

                    Material[] mats = new Material[count];

                    if (_ultimoSuspiroMat != null)
                    {
                        mats[0] = _ultimoSuspiroMat;

                        for (int i = 0; i < count - 1; i++)
                            mats[i + 1] = targetStigma.GetComponent<MeshRenderer>().materials[i];
                    }

                    targetStigma.GetComponent<MeshRenderer>().materials = mats;
                }

                if (_ultimoSuspiroColor != new Color())
                {
                    for (int i = 0; i < count; i++)
                    {
                        Material _Mat = /*new Material*/(targetStigma.GetComponent<MeshRenderer>().materials[i]);

                        //_Mat.name = "New Mat(" + i + ")";

                        if (_Mat.GetColor("Base Color") != new Color())
                        {
                            _Mat.SetColor("Base Color", _ultimoSuspiroColor);

                            print("Base Color = " + _Mat.GetColor("Base Color") + " " + _Mat.shader);
                        }
                        else
                            if (_Mat.GetColor("_Color") != new Color())
                        {
                            _Mat.SetColor("_Color", _ultimoSuspiroColor);

                            print("_Color = " + _Mat.GetColor("_Color") + " " + _Mat.shader);
                        }

                        //targetStigma.GetComponent<MeshRenderer>().materials[i] = _Mat;
                    }
                }
            }

            if (targetStigma.GetComponent<MoveController>().Solo != null)
            {
                targetStigma.GetComponent<MoveController>().Solo.free = false;
                targetStigma.GetComponent<MoveController>().Solo.currentMob = targetStigma;

                targetStigma.GetComponent<MoveController>().Solo.WalkInHere();

                targetStigma.transform.position = targetStigma.GetComponent<MoveController>().Solo.transform.position;
            }
            else
            {
                if (CheckGrid.Instance)
                {
                    List<HexManager> hex = CheckGrid.Instance.RegisterRadioHex(
                           user.GetComponent<MoveController>().hexagonX,
                           user.GetComponent<MoveController>().hexagonY,
                           colore: false);

                    foreach (var h in hex)
                    {
                        if (!h.free || h.currentMob != null)
                        {
                            hex.Remove(h);
                        }
                    }

                    if (hex.Count > 0)
                    {
                        HexManager _hex = hex[Random.Range(0, hex.Count)];

                        _hex.free = false;
                        _hex.currentMob = targetStigma;
                        _hex.WalkInHere();

                        targetStigma.GetComponent<MoveController>().hexagonX = _hex.x;
                        targetStigma.GetComponent<MoveController>().hexagonY = _hex.y;

                        targetStigma.transform.position = _hex.transform.position;
                    }
                }
            }

            targetStigma = null;

            timer = 0;

            if (gameObjScript != null)
                gameObjScript.StigmaTargets.Add(targetStigma);

            gameObject.SetActive(false);
        }
    }
}
