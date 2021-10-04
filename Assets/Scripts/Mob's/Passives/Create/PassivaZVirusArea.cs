using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassivaZVirusArea : PassivaZVirus
{
    [Space]
    [Header("Area")]
    [SerializeField,Tooltip("Area em que o clone pode aparecer $P4")]
    protected int range;

   
    private void Awake()
    {
        if (range <= 0)
        {
            if (GetComponent<MobSkillManager>() != null)
                range = GetComponent<MobSkillManager>().RangeSkillAttackRange;
            else
            if (GetComponent<MobSkillAreaDamage>() != null)
                range = GetComponent<MobSkillAreaDamage>().Range;
        }
    }

    public override void StartPassive(GameObject target, params Passive[] passive)
    {
        if (CooldownCurrent > 0 || CooldownCurrent == -2 || SilencePassive)
        {
            Debug.LogError(SilencePassive ? _Nome + " Esta Silenciada por " + SilenceTime + " turnos"
                : (cooldownCurrent > 0 ? "Em Espera" : "Passiva já foi usada"));
            return;
        }

        foreach (var i in passive)
        {
            //Debug.LogError("StartPassive "+i);

            if (i == Passive.EndTurn)
            {
                TimerClones();
            }

            if (i == Passive.Kill)
            {
                KillAllClones();
                break;
            }

            foreach (var y in _Passive)
            {
                if (_Passive.Length > 0)
                    if (y._startPassive == i && _mobSkillManager != null && _mobSkillManager.useSkill/**/ ||
                        y._startPassive == i && _mobSkillManager == null)
                    {
                        Debug.LogError("StartPassiveZVirus(" + target + ")");

                        //GameManagerScenes._gms.NewInfo("StartPassiveZVirus - [" + target + "]", 3, true);

                        if (ChanceActivePassive() && CountClonesActives() <= _maxClonesActive)
                        {
                            if (target.GetComponent<MoveController>())
                            {
                                EffectManager.Instance.PopUpDamageEffect(_Nome + " Active", User);
                                CreateAreaClone(target.GetComponent<MoveController>().Solo);
                            }
                            else
                            if (User != null)
                            {
                                EffectManager.Instance.PopUpDamageEffect(_Nome + " Active", User);
                                CreateAreaClone(User.GetComponent<MoveController>().Solo);
                            }
                        }
                        else
                            EffectManager.Instance.PopUpDamageEffect(_Nome + " Maximo", User);
                    }
            }
        }
    }

    protected virtual void CreateAreaClone(HexManager targetSolo)
    {            
        Debug.LogError("CreateAreaClone - " + _Nome + " [" + targetSolo + "] Range - " + range);

        if (CountClonesActives() >= _maxClonesActive)
            return;

        CooldownReset();

        //GameManagerScenes._gms.NewInfo("CreateAreaClone - " + _Nome + " [" + targetSolo + "] Range - " + range, 3,true);

        HexManager solo = targetSolo;

        if (_mobSkillManager != null)
        {
            if (_mobSkillManager.Target!=null && _mobSkillManager.Target.GetComponent<MoveController>())
                solo = _mobSkillManager.Target.GetComponent<MoveController>().Solo;
        }

        List<HexManager> listSolo =  CheckGrid.Instance.RegisterRadioHex(solo.x, solo.y, range, false, 0);

        RulesCreateArea(listSolo);

        if (listSolo.Count > 0)
        {
            solo = listSolo[Random.Range(0, listSolo.Count)];

            StartCoroutine(RespawClone(solo));
        }
    }

    public virtual void CreateAreaClone()
    {
        GameManagerScenes._gms.NewInfo("CreateAreaClone()", 3, true);

        if (ChanceActivePassive())
        {
            if (CountClonesActives() >= _maxClonesActive)
                return;

            CooldownReset();

            HexManager solo = null;

            if (GetComponent<MobSkillManager>() != null &&
            GetComponent<MobSkillManager>().Target != null &&
            GetComponent<MobSkillManager>().Target.GetComponent<MoveController>() != null)
            {
                solo = GetComponent<MobSkillManager>().Target.GetComponent<MoveController>().Solo;
            }
            else
            if (GetComponent<MobSkillAreaDamage>() != null)
            {
                solo = GetComponent<MobSkillAreaDamage>().Solo;
            }

            Debug.LogError("CreateAreaClone - " + _Nome + " [" + solo + "] Range - "+range);

            //GameManagerScenes._gms.NewInfo("CreateAreaClone - " + _Nome + " [" + solo + "] Range - " + range, 3, true);

            if (solo != null)
            {
                List<HexManager> listSolo = CheckGrid.Instance.RegisterRadioHex(solo.x, solo.y, range, false, 0);

                RulesCreateArea(listSolo);

                //GameManagerScenes._gms.NewInfo("CreateAreaClone - " + solo + " -> " + listSolo.Count, 3, true);
                Debug.LogError("CreateAreaClone - " + solo + " -> " + listSolo.Count);

                if (listSolo.Count > 0)
                {
                    solo = listSolo[Random.Range(0, listSolo.Count)];

                    // GameManagerScenes._gms.NewInfo("CreateAreaClone Respaw solo - " + solo, 3, true);

                    EffectManager.Instance.PopUpDamageEffect(_Nome + " Active", User);

                    StartCoroutine(RespawClone(solo));
                }
            }
        }
    }

    protected virtual List<HexManager> RulesCreateArea(List<HexManager> list)
    {
        Debug.LogError("RulesCreateArea ("+list.Count+")");

        //GameManagerScenes._gms.NewInfo("RulesCreateArea (" + list.Count + ")", 3, true);

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].free || list[i].currentMob != null)
            {
                Debug.LogError("RulesCreateArea Remove -> " + list[i]);
               // GameManagerScenes._gms.NewInfo("RulesCreateArea Remove -> " + list[i], 3, true);

                list.Remove(list[i]);
            }
        }

       // GameManagerScenes._gms.NewInfo("RulesCreateArea - Return (" + list.Count + ")", 3, true);

        return list;
    }
}
