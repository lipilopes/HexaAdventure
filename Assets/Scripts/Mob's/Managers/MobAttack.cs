using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class MobAttack : MonoBehaviour {

    [Header("Name")]
    [TextArea]public string nameSkill1;
    [TextArea]public string nameSkill2, nameSkill3;

    [Header("Distancia")]
    public int distanceSkill1;
    public int distanceSkill2, distanceSkill3;

    [HideInInspector]
    public int moreDistanceSkill;

    [Header("Dano")]
    [Range(0,150)][Tooltip("% do dano no MobManager")]
    public int damageSkill1;
    [Range(0,150)][Tooltip("% do dano no MobManager")]
    public int damageSkill2, damageSkill3;

    [Header("Cooldown")]
    [SerializeField] float maxCooldownSkill1;
    public float MaxCooldownSkill1
    {
        get
        {
            BonusCooldown();
            return maxCooldownSkill1;
        }

        set
        {
            maxCooldownSkill1 = value;
        }
    }

    [SerializeField] float maxCooldownSkill2;
    public float MaxCooldownSkill2
    {
        get
        {
            return maxCooldownSkill2;
        }

        set
        {
            maxCooldownSkill2 = value;
        }
    }

    [SerializeField] float maxCooldownSkill3;
    public float MaxCooldownSkill3
    {      
        get
        {
            return maxCooldownSkill3;
        }

        set
        {
            maxCooldownSkill3 = value;
        }
    }

    bool bonusCooldown = false;

    [Header("Other")]
    [SerializeField] bool skill1NeedTarget=true;
    [SerializeField] bool skill2NeedTarget=true;
    [SerializeField] bool skill3NeedTarget=true;

    public bool Skill1NeedTarget { get { return skill1NeedTarget; } set { skill1NeedTarget = value; } }
    public bool Skill2NeedTarget { get { return skill2NeedTarget; } set { skill2NeedTarget = value; } }
    public bool Skill3NeedTarget { get { return skill3NeedTarget; } set { skill3NeedTarget = value; } }
    [Space]
    [SerializeField] bool skill1TargetFriend=true;
    [SerializeField] bool skill2TargetFriend = true;
    [SerializeField] bool skill3TargetFriend = true;    

    public bool Skill1TargetFriend { get { return skill1TargetFriend; } set { skill1TargetFriend = value; } }
    public bool Skill2TargetFriend { get { return skill2TargetFriend; } set { skill2TargetFriend = value; } }
    public bool Skill3TargetFriend { get { return skill3TargetFriend; } set { skill3TargetFriend = value; } }

    [Space]
    [SerializeField] bool skill1TargetMe = false;
    [SerializeField] bool skill2TargetMe = false;
    [SerializeField] bool skill3TargetMe = false;  

    public bool Skill1TargetMe { get { return skill1TargetMe; } set { skill1TargetMe = value; } }
    public bool Skill2TargetMe { get { return skill2TargetMe; } set { skill2TargetMe = value; } }
    public bool Skill3TargetMe { get { return skill3TargetMe; } set { skill3TargetMe = value; } }

    /*[Header("Prefab Skill")]
    public List<MobSkillManager> skillList = new List<MobSkillManager>();*/
/*
    public void BonusCooldown()
    {
        if (bonusCooldown)
            return;

        bonusCooldown = true;

        if (RespawMob.Instance.Player != gameObject)
            return;

        //maxCooldownSkill1 -= GameManagerScenes._gms.BonusSkill(0);

        //maxCooldownSkill2 -= GameManagerScenes._gms.BonusSkill(1);

        MaxCooldownSkill3 -= GameManagerScenes._gms.BonusSkill(2);

        if (MaxCooldownSkill3 < 0)
            MaxCooldownSkill3 = 0;
    }

    public void CheckMoreDistanceSkill()
    {
        MobCooldown mcd = GetComponent<MobCooldown>();
        SkillManager sm = GetComponent<SkillManager>();

        //GetComponent<IaAttackMob>().AttAttack();

        if (sm != null)
        {
            sm.CheckMoreDistanceSkill();
            return;
        }

        if (mcd.timeCooldownSkill.Count!=0)
        {
            if (mcd.timeCooldownSkill[0] == 0)
                moreDistanceSkill = distanceSkill1;
            else
                moreDistanceSkill = 0;

            if (moreDistanceSkill < distanceSkill2 && mcd.timeCooldownSkill[1] == 0) { moreDistanceSkill = distanceSkill1; }
            if (moreDistanceSkill < distanceSkill3 && mcd.timeCooldownSkill[2] == 0) { moreDistanceSkill = distanceSkill2; }
        }

       // return moreDistanceSkill;
    }
}
*/