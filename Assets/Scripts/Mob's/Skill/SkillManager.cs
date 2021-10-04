using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    [SerializeField]
    List<GameObject>        prefabSkills = new List<GameObject>() { null, null, null };
    public List<GameObject> PrefabSkills { get { return prefabSkills; } set { prefabSkills = value; } }

    List<MobSkillManager>        skills = new List<MobSkillManager>() { null, null, null };
    public List<MobSkillManager> Skills { get { return skills; } set { skills = value; } }

    List<PassiveManager> passives   = new List<PassiveManager>();
    public List<PassiveManager> Passives { get { CheckPassives(); return passives; } set { checkPassives = true; passives = value; } }

    [HideInInspector]
    public MobSkillManager SkillsReserva;

    public int[] sequenceSkill = { 2, 1, 0 };
    
    bool bonusCooldown = false;

    WaitForSeconds wActiveSkill = new WaitForSeconds(0.2f);

   
    bool checkPassives = true;
    /// <summary>
    /// Remove Objs nulos da lista
    /// </summary>
    void CheckPassives()
    {
        if (!checkPassives)
            return;

        int count = passives.Count;
        for (int i = 0; i < count; i++)
        {
            if (passives[i]==null)
            {
                passives.Remove(passives[i]);
                CheckPassives();
                break;
            }
        }
        checkPassives = false;
    }

    public bool SkillInUse
    {
        get
        {
            if (SkillsReserva != null)
            {
                if (SkillsReserva.useSkill)
                {
                    //Debug.LogError("Skill in Use -> True");
                    return true;
                }
            }

            if (Skills.Count != 0)
                for (int i = 0; i < Skills.Count; i++)
                {
                    if (Skills[i] != null)
                        if (Skills[i].useSkill)
                        {
                          //  Debug.LogError("Skill in Use -> True");
                            return true;
                        }
                }


            //Debug.LogError("Skill in Use -> False");
            return false;
        }
    }

    public void ActiveSkill()
    {
        StartCoroutine(ActiveSkillCoroutine());
    }

    IEnumerator ActiveSkillCoroutine()
    {
        int count = prefabSkills.Count;

        for (int i = 0; i < count; i++)
        {
            if (Skills[i] == null)
            {
                MobSkillManager skill = null;

                GameObject Prefab = PrefabSkills[i];

                if (Prefab != null)//Pra dar tempo de apagar a do modo errado
                {
                    GameObject _skill = Instantiate(Prefab, new Vector3(0, 0.057f, 0), new Quaternion());

                    yield return wActiveSkill;

                    skill = _skill.GetComponent<MobSkillManager>();
                }


                if (skill != null)
                {
                    //skill.name = skill.Nome + " - " + name;
                    skill.User = gameObject;
                    skill.skillManager = this;                  
                    skills[i] = skill;
                    skills[i].transform.SetParent(transform);
                    skills[i].gameObject.SetActive(true);

                   // if (i != 0)
                    {
                        //skill.CooldownCurrent = skill.CooldownMax;
                        GetComponent<MobCooldown>().timeCooldownSkill.Add(skill.CooldownMax);
                        GetComponent<MobCooldown>().AttCooldown(skill.CooldownMax, i);
                    }
                   /* else
                    {
                        //skill.CooldownCurrent = 0;
                        GetComponent<MobCooldown>().AttCooldown(0, i);
                    }*/

                    Debug.LogError(skill.name + " Foi criado!!!");
                }
                else
                {
                    if (Prefab != null)
                        Debug.LogError("Errou ao ActivarSkill(" + Prefab.name + ")[" + i + "] do " + gameObject);
                    else
                        Debug.LogError("Errou ao ActivarSkill[" + i + "] do " + gameObject);
                }
            }
        }

        AttDamageSkills();

        //if (GetComponent<DetalSkillToolTip>() != null)
        //    GetComponent<DetalSkillToolTip>().AttDamage();
    }

    public void AttDamageSkills()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if(Skills[i]!=null)
            Skills[i].AttDamage();
        }

        if (SkillsReserva != null)
        {
            SkillsReserva.AttDamage();
        }
    }

    /// <summary>
    /// Atualiza dano da skill e afins quando ganha um bonus
    /// </summary>
    public void AttStatusSkills()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i] != null)
                Skills[i].AttDamageAndDescription();
        }

        if (SkillsReserva != null)
        {
            SkillsReserva.AttDamageAndDescription();
        }
    }

    private void Awake()
    {
        //ActiveSkill();
    }

    public void UseSkill(int index)
    {
        Debug.LogError("Count[" + Skills.Count+"] - index("+index+")");

        if (index < Skills.Count && index > -1)
        {
            if (Skills[index] != null)
               Skills[index].UseSkill();          
        }

        if (SkillsReserva != null && index==-1)
        {
            SkillsReserva.UseSkill();
        }
    }

    /*public void HitSkill(bool endTurn,GameObject target)
    {
        Debug.LogError("HitSkill(" + endTurn + ") - "+ target);

        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i]!=null && Skills[i].useSkill)
            {
                Debug.LogError(Skills[i].Nome+" HitSkill(" + endTurn + ")");
                Skills[i].Hit(endTurn, target);
                return;
            }

            Debug.LogError(Skills[i].Nome + " Foi rejeitado para o HitSkill");
        }

        if (SkillsReserva != null)
        {
            SkillsReserva.Hit(endTurn, target);
        }
    }*/

    /*public void HitSkill(GameObject target)
    {
        Debug.LogError("HitSkill() - " + target);

        foreach (var s in Skills)
        {
            if (s!=null && s.useSkill)
            {
                s.Hit(false,target);
                return;
            }
        }

        if (SkillsReserva != null)
        {
            SkillsReserva.Hit(false, target);
        }
    }*/

    public void CheckAreaDamage()
    {
        //GameManagerScenes._gms.NewInfo("CheckAreaDamage - " + name, 3, true);

        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i] != null && Skills[i].gameObject.activeInHierarchy)
            {
                //GameManagerScenes._gms.NewInfo("CheckAreaDamage ["+ Skills[i].Nome+ "]- " + name, 3, true);
                Skills[i].EndTurnAttack();
            }
        }

        if (SkillsReserva != null && SkillsReserva.gameObject.activeInHierarchy)
        {
            SkillsReserva.EndTurnAttack();
        }
    }

    public void DesactiveAreaDamage()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i] != null)
                Skills[i].DesactiveTurnAttack();
        }

        if (SkillsReserva != null)
        {
            SkillsReserva.DesactiveTurnAttack();
        }
    }

    public void ResetUseSkill()
    {
        for (int i = 0; i < Skills.Count; i++)
        {
            if (Skills[i] != null)
            {
                Skills[i].useSkill = false;
                Skills[i].HexList.Clear();
            }
        }

        if (SkillsReserva!=null)
        {
            SkillsReserva.useSkill = false;
            SkillsReserva.HexList.Clear();
        }
    }

    public void BonusCooldown()
    {
        if (bonusCooldown)
            return;

        bonusCooldown = true;

        if (RespawMob.Instance==null || RespawMob.Instance.Player != gameObject || !GetComponent<MobManager>().getBonusPlayer)
            return;

        int count = Skills.Count;

        for (int i = 0; i < count; i++)
        {
            if (Skills[i] != null)
            {
                Skills[i].CooldownMax -= GameManagerScenes._gms.BonusSkill(i);

                if (Skills[i].CooldownMax < 0)
                    Skills[i].CooldownMax = 0;

                Skills[i].CooldownCurrent = Skills[i].CooldownMax;

                if (i != 0)
                    GetComponent<MobCooldown>().timeCooldownSkill[i] = Skills[i].CooldownCurrent;
            }
        }      
    }

    public int CheckMoreDistanceSkill()
    {
      int  moreDistanceSkill = 0;
        int count = Skills.Count;

        for (int i = 0; i < count; i++)
        {
            if (Skills[i] != null)
            {
                if (moreDistanceSkill < Skills[i].Range &&
                    Skills[i].CooldownCurrent <= 0)
                {
                    moreDistanceSkill = Skills[i].Range;
                }
            }
        }

        if (SkillsReserva != null)
        {
            if (moreDistanceSkill < SkillsReserva.Range &&
                       SkillsReserva.CooldownCurrent <= 0)
                moreDistanceSkill = SkillsReserva.Range;
        }

        return moreDistanceSkill;
    }

    public int CooldownCurrentSkill(int index)
    {
        if (SkillsReserva != null && index ==-1)
        {
            return SkillsReserva.CooldownCurrent;
        }

        if (Skills.Count < index && index >= 0 && Skills[index] != null)
            return Skills[index].CooldownCurrent;

        return 99;
    }

    public int CooldownMaxSkill(int index)
    {
        if (SkillsReserva != null && index == -1)
        {
            return SkillsReserva.CooldownMax;
        }

        if (Skills[index] != null)
            return Skills[index].CooldownMax;

        return 99;
    }

    public float CalculePorcent(float value,float porcent)
    {
        return (value * porcent) / 100;
    }

    public int CalculePorcent(int value, int porcent,int divided=1,int baseValue=0, int HpMaxPorcent=0)
    {
        print(" Damage(("+value+") * ("+porcent+ "%)) / Divided("+divided+")) + Base Damage("+baseValue+")  + Max Hp Porcent("+ HpMaxPorcent + ") = "+ ((((value * porcent) / 100) / divided)+baseValue+ HpMaxPorcent));
        return ((((value * porcent) / 100)/ divided)+baseValue + HpMaxPorcent);
    }
    public int CalculeTotalDamage(int value, int porcent, int divided = 1, int baseValue = 0, int HpMaxPorcent = 0,float dividDamage=1)
    {
        print(" Damage((" + value + ") * (" + porcent + "%)) / Divided(" + divided + ")) + Base Damage(" + baseValue + ")  + Max Hp Porcent(" + HpMaxPorcent + ") * DividDamage("+ dividDamage + ") = " + (((((value * porcent) / 100) / divided) + baseValue + HpMaxPorcent)* dividDamage));
        return (int)(((((value * porcent) / 100) / divided) + baseValue + HpMaxPorcent)* dividDamage);
    }

    public void ActivePassive(Passive _passive, GameObject gO, float value)
    {
        foreach (var s in Skills)
        {
            if (s !=null)
            s.ActivePassive(_passive, value, gO);
        }
    }
    public void ActivePassive(Passive _passive, GameObject gO)
    {
        foreach (var s in Skills)
        {
            if (s != null)
                s.ActivePassive(_passive, gO);
        }
    }


    public MobSkillManager SkillInUseWho()
    {
        foreach (var i in Skills)
        {
            if (i.useSkill)
            {
                Debug.LogError("SkillInUseWho() => "+i.Nome);
                return i;
            }
        }

        Debug.LogError("SkillInUseWho() => null");

        return null;
    }

    ///// <summary>
    ///// Silencia Skill()
    ///// </summary>
    ///// <param name="_time">tempo da duração</param>
    ///// <param name="_index">index da skill [-2: Todas, -3 Menor cooldown]</param>
    //public void AtiveDbuffSilenceSkill(int _time, int _index)
    //{
    //    Debug.LogError("AtiveDbuffSilenceSkill(Time:" + _time+", index:"+_index+")");

        //int menorCooldown      = 99999,
        //    indexMenorCooldown = 0;

        //for (int i = 0; i < Skills.Count; i++)
        //{
        //    if (Skills[i] != null)
        //    {
        //        if (i      == _index || 
        //            _index == -2)//index iguais ou todos
        //        {
        //            Debug.LogError("Skill["+i+"] silenciada");
        //            //Skills[i].AtiveDbuffSilenceSkill(_time);
        //            Skills[i].SilenceSkill     = true;
        //            Skills[i].SilenceTime = _time;
        //        }
        //        else
        //            if (_index == -3)//Menor Cooldown
        //        {
        //            if (Skills[i].CooldownCurrent <= menorCooldown)
        //            {
        //                Debug.LogError("Skill[" + i + "] Esta com o menor cooldown");
        //                menorCooldown      = Skills[i].CooldownCurrent;
        //                indexMenorCooldown = i;
        //            }
        //        }
        //    }
        //}

        //if (_index==-3)//Menor Cooldown
        //{
        //    Skills[indexMenorCooldown].SilenceSkill = true;
        //    Skills[indexMenorCooldown].SilenceTime = _time;
        //   // Skills[indexMenorCooldown].AtiveDbuffSilenceSkill(_time);
        //}
                 

        //if (SkillsReserva != null && _index==-1)
        //{
        //    // SkillsReserva.AtiveDbuffSilenceSkill(_time);
        //    SkillsReserva.SilenceSkill     = true;
        //    SkillsReserva.SilenceTime = _time;
        //}
    //}
}
