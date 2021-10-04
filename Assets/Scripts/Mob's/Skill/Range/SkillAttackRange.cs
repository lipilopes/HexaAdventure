using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Skill ataque com base em range
/// </summary>
public class SkillAttackRange : SkillAttack
{
    [Space]
    [Header("Skill Attack Range")]
    [SerializeField, Tooltip("Raio aparece")]
    protected bool radioUser = true;
    [SerializeField]
    protected int _range = -1;
    public int Range { set { _range = value; } get { return _range; } }

    public HexManager GroundCheck { set; get; }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!AreaDamage)
        {
            AreaDamage = true;
        }

        if (_range > 0)
        {
            if (GetComponent<BoxCollider>())
            {               
                Vector3 sizeCollider = new Vector3(0, 0, 0);
                GetComponent<BoxCollider>().center = new Vector3(999999, 999999, 99999);

                GetComponent<BoxCollider>().size = sizeCollider;
                GetComponent<BoxCollider>().enabled = false;
                //Debug.LogError("Ajustado BoxCollider[" + skillPrefab.name + "] para " + sizeCollider);
            }
            else
                     if (GetComponent<CapsuleCollider>())
            {
                GetComponent<CapsuleCollider>().radius = 0;
                GetComponent<CapsuleCollider>().center = new Vector3(999999, 999999, 99999);
                GetComponent<CapsuleCollider>().enabled = false;
                //Debug.LogError("Ajustado CapsuleCollider[" + skillPrefab.name + "] para " + valueSizeCollider);
            }
            else
                     if (GetComponent<SphereCollider>())
            {
                GetComponent<SphereCollider>().radius = 0;
                GetComponent<SphereCollider>().center = new Vector3(999999, 999999, 99999);
                GetComponent<SphereCollider>().enabled = false;
                //Debug.LogError("Ajustado SphereCollider[" + skillPrefab.name + "] para " + valueSizeCollider);
            }          
        }
    }


    public override void UseSkill(GameObject _target,MobSkillManager _skill)
    {
        base.UseSkill(_target, _skill);

        if (Range > 0)
            RangeDamage();
    }


    protected virtual void RangeDamage()
    {
        //Debug.LogError("RangeDamage()");

       // if (GameManagerScenes._gms.Adm)
       //     GameManagerScenes._gms.NewInfo("RangeDamage() -> "+ Range, 3);

        GameObject radio = radioUser ? who : target;

        int X = -1,
            Y = -1;

        if (/*!radioUser &&*/radio.GetComponent<MoveController>())
        {
            X = radio.GetComponent<MoveController>().hexagonX;
            Y = radio.GetComponent<MoveController>().hexagonY;
        }

        GroundCheck = CheckGrid.Instance.HexGround(X, Y);

        List <HexManager> hexRadio = CheckGrid.Instance.RegisterRadioHex(X, Y, Range, true, 3);

       // if (GameManagerScenes._gms.Adm && hexRadio.Count > 0)
       //     GameManagerScenes._gms.NewInfo("RangeDamage("+ hexRadio[0].currentMob+ ") -> Meio Hex" + X+"x"+Y+" sizeList "+hexRadio.Count, 5);

        bool _findMainTarget = false;

        foreach (var item in hexRadio)
        {
            //Debug.LogError("RangeDamage() -> Foreach = " + item.name);
            if (item != null && item.currentMob != null /*&& item.currentMob.GetComponent<MobManager>() && item.currentMob.GetComponent<MobManager>().Alive*/)
            {
                //Debug.LogError("RangeDamage() -> HITTED in " + item.currentMob.name);
              //  if (GameManagerScenes._gms.Adm)
              //      GameManagerScenes._gms.NewInfo("RangeDamage() -> HITTED in " + item.currentMob.name, 3);

                if (item.currentMob == target)
                    _findMainTarget = true;
                else
                    HittedSkill(item.currentMob);
            }
        }

        if (_findMainTarget)
            HittedSkill(target);
    }


    protected override IEnumerator WaitHitTargetCoroutine()
    {
        yield return waitHitTarget;
    }
}
