using UnityEngine;
using System.Collections.Generic;

public class MobCooldown : MonoBehaviour
{
    public List<float> timeCooldownSkill = new List<float>();

   // public float timeCooldownSkill1, timeCooldownSkill2, timeCooldownSkill3;

   protected SkillManager Sm;

    private void Awake()
    {
            timeCooldownSkill.Clear();
    }

    private void Start()
    {

    }

    private void OnEnable()
    {
        Sm = GetComponent<SkillManager>();

        if (TurnSystem.Instance != null)
        {
            TurnSystem.DelegateTurnEnd += CooldownSkills;
        }
    }

    private void OnDisable()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.DelegateTurnEnd -= CooldownSkills;
        }
    }

    private void OnDestroy()
    {
        if (TurnSystem.Instance != null)
        {
            TurnSystem.DelegateTurnEnd -= CooldownSkills;
        }
    }

    /// <summary>
    /// Ninguem Pode Chamar esse, Passiva Tbm
    /// </summary>
    public void CooldownSkills()
    {
        int count = timeCooldownSkill.Count;
     
        if (Sm != null)
        {
            for (int i = 0; i < Sm.Skills.Count; i++)
            {
                if (Sm.Skills[i]!=null)
                if (Sm.Skills[i].CooldownCurrent >= 1)
                {
                    Sm.Skills[i].CooldownCurrent--;

                    timeCooldownSkill[i] = Sm.Skills[i].CooldownCurrent;
                }
            }

            for (int i = 0; i < Sm.Passives.Count; i++)
            {
                if (Sm.Passives[i] != null)
                    Sm.Passives[i].Cooldown();
            }
        }   
    }

    public void RenewCooldown(params float[] arg)
    {
        int count = arg.Length;

        for (int i = 0; i < count; i++)
        {
            if (i < timeCooldownSkill.Count)
                timeCooldownSkill[i] = arg[i];
            else
                timeCooldownSkill.Add(arg[i]);
        }

        Debug.LogWarning("RENEW COOLDOW's");

     /*
        timeCooldownSkill1 = timeCooldownSkill[0];
        timeCooldownSkill2 = timeCooldownSkill[1];
        timeCooldownSkill3 = timeCooldownSkill[2];
        */
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">-2 para tirar um cooldown</param>
    /// <param name="index"></param>
    public void AttCooldown(int value, int index)
    {
        if (index < 0 || index >= timeCooldownSkill.Count)
            return;

        if (value == -2)
        {
            value = (int)timeCooldownSkill[index];
            value--;
        }
        else
            timeCooldownSkill[index] = value;

        SkillManager sm = GetComponent<SkillManager>();

        if (timeCooldownSkill[index] < 0)
        {
            timeCooldownSkill[index] = 0;
        }

        Debug.LogError("AttCooldown(Valor = " + value + ",index = " + index + ")");

        if (sm != null)
            if (index >= 0 && index <= sm.Skills.Count)
            {
                sm.Skills[index].CooldownCurrent = value;
                Debug.LogError("AttCooldown SM");
            }      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value">-2 para tirar um cooldown</param>
    /// <param name="index"></param>
    public void AttPassiveCooldown(int value, int index)
    {
        if (index < 0 && index < Sm.Passives.Count)
            return;

        if (value == -2 && Sm.Passives[index].CooldownCurrent != -2)
        {
            value = (int)Sm.Passives[index].CooldownCurrent;
            value--;
        }

        Debug.LogError("AttPassiveCooldown(Valor = " + value + ",index = " + index + ")");

        if (Sm != null)
            if (index >= 0 && index < Sm.Passives.Count 
                && Sm.Passives[index].CooldownCurrent != -2)
                Sm.Passives[index].CooldownCurrent = value;
    }
}
