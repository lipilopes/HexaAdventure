using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public static ToolTip Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);
    }

    GameManagerScenes gms;
    [SerializeField] GameObject painel;
    [SerializeField] Text nameTarget;

    [Space(7), Header("Mob Painel"), SerializeField]
    GameObject painelMob;
    [SerializeField] Text classeMob;
    [SerializeField] Text walkMob;
    [SerializeField] Slider hpMob;
    [SerializeField] Slider hpDamageMob;
    [SerializeField] Text hpMobText;
    [Tooltip("Nome Vida da barra de vida")]
    [SerializeField] Text hpTargetText;
    //[SerializeField] GameObject skill1Mob;
    //[SerializeField] GameObject skill2Mob;
    //[SerializeField] GameObject skill3Mob;
    [SerializeField]
    List<GameObject> skillsMob = new List<GameObject>();

    [SerializeField] Image[] iconsDbuff;

    [Space(7), Header("Skill Detal Painel")]
    [SerializeField] GameObject skillDetalMob;
    [SerializeField, Tooltip("Text de Descrição da skill")]
    protected Text txtSkillDetalMob;
    [SerializeField, Tooltip("Text de Damage cooldown ...")]
    protected Text txtSkillDetalExtraMob;

    [Space(7), Header("Item Painel"), SerializeField]
    GameObject painelItem;
    [SerializeField]
    protected Text descricaoItem;

    public GameObject target;
    //float damage;

    //Fade Buton
    GameObject player;

    bool ShowRageSkill,
        /*ShowRageSkill1,
     ShowRageSkill2,
     ShowRageSkill3,*/
     clearColor;
    //End

    private void OnEnable()
    {
        if (TurnSystem.Instance != null)
        {
            //TurnSystem.DelegateTurnEnd   += () => AttTooltip(null);
            
            TurnSystem.DelegateTurnCurrent += () => AttTooltip(null);
        }
    }

    private void OnDisable()
    {
        if (TurnSystem.Instance != null)
        {
            //TurnSystem.DelegateTurnEnd -=() => AttTooltip(null);

            TurnSystem.DelegateTurnCurrent -= () => AttTooltip(null);
        }
    }


    private void Start()
    {
        gms = GameManagerScenes._gms;

        if (TurnSystem.Instance != null)
        {
            //TurnSystem.DelegateTurnEnd += () => AttTooltip(null);

            TurnSystem.DelegateTurnCurrent += () => AttTooltip(null);
        }


        hpTargetText.text = XmlMenuInicial.Instance.Get(75);//Vida
    }

    private void Update()
    {
        ClickTarget();
    }

    void ClickTarget()
    {
        if (GameManagerScenes._gms)
        if (GameManagerScenes._gms.Paused==true)
            return;

        Ray ray;

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
        else
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        else
            return;

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject hitObject = hit.collider.transform.gameObject;

            if (Input.GetMouseButtonDown(0))
            {
                #region Caso Click no Hexagono               
                #region ITEM
                if (hitObject.GetComponent<HexManager>() != null)
                    if (hitObject.GetComponent<HexManager>().currentItem != null)
                        if (hitObject.GetComponent<HexManager>().currentItem.GetComponent<ToolTipType>() != null)
                            hitObject = hitObject.GetComponent<HexManager>().currentItem;
                #endregion

                #region MOB
                if (hitObject.GetComponent<HexManager>() != null)
                    if (hitObject.GetComponent<HexManager>().currentMob != null)
                        if (hitObject.GetComponent<HexManager>().currentMob.GetComponent<ToolTipType>() != null)
                            hitObject = hitObject.GetComponent<HexManager>().currentMob;
                #endregion
                #endregion

                if (hitObject.GetComponent<ToolTipType>() != null)
                {
                    GetComponent<ButtonManager>().ClickAudio();

                    //Era False
                    CloseTargetDetalSkill();

                    TargetTooltip(hitObject);
                }
            }

            if (hitObject != null)
                if (Input.GetMouseButtonDown(1) && !gms.IsMobile ||
                    Input.touchCount == 2 && gms.IsMobile)
                    CameraOrbit.Instance.ChangeTarget(hitObject);
        }
    }//Escolhe na list um inimigo

    public void AttTooltip(GameObject who = null)
    {        
        if (gms == null)
            gms = GameManagerScenes._gms;

        if (who == null && target != null)
            who = target;

        if (who != target || who == null)
            return;

        if (!who.activeInHierarchy)
            return;

        //Debug.LogError("AttTooltip(" + who + ")");

        if (_targetDetalSkill!=null && _targetDetalSkill == who.GetComponent<DetalSkillToolTip>())
        {
            //Debug.LogError("TargetDetalSkill -> AttTooltip()");

            AttTargetDetalSkill();
        }

        TargetTooltip(who, 0, false);
    }

    public void TargetTooltip(GameObject Target, float Damage = 0, bool prop = true)
    {        
        if (Target == null || !Target.activeInHierarchy)
        {
            target = null;
            //damage = 0;
            return;
        }

        ToolTipType _target = Target.GetComponent<ToolTipType>();

        if (_target == null)
            return;

        if (player == null)
            player = RespawMob.Instance.Player;       

        //painelItem.GetComponent<CanvasGroup>().alpha = 0;
        //painelMob.GetComponent<CanvasGroup>().alpha  = 0;

        //painel.GetComponentsInChildren<Animator>()[0].SetBool("Show", false);

        //painel.SetActive(true);

        painel.GetComponentsInChildren<Animator>()[0].SetBool("Show", true);

        _target.AttToltip();

        target = Target;

        if(player!=null)
        if (target.GetComponent<MobManager>() != null && player.GetComponent<MobManager>() != null)
        {
            if (target.GetComponent<MobManager>().MesmoTime(RespawMob.Instance.PlayerTime))
            {
                Damage = 0;
            }
        }

        nameTarget.text = _target._name;

        //Debug.LogError("ToolTip Damage:"+Damage);

        #region MOB
        if ((int)_target._type == 1)//MOB
        {
            if (_target._hp <= -1)
            {
                FechaTarget();
                return;
            }

            float hp      = _target._hp;
            float defense = _target._defense;
            float armor   = _target._armor;
            float dodge   = _target._dodge;
            string infos  = "";

            painelMob.GetComponent<CanvasGroup>().alpha = 1;
            painelItem.GetComponent<CanvasGroup>().alpha = 0;

            hpDamageMob.gameObject.SetActive(true);
            hpMob.gameObject.SetActive(true);

            if (Damage > 0)
            {
                hpMob.gameObject.SetActive(false);

                if (armor > 0)               
                    Damage -= armor;              
                else
                    Damage += armor;

                if (_target._defense > 0)
                {
                    float damage   = Damage;
                    //float totalDef = defense;

                    Damage -= defense;

                    defense -= (damage);

                    if (defense < 0)
                        Damage = defense / -1;
                }


                #region Calcula Damage
                if (defense <= 0)
                {
                    hp = hp - Damage;
                    hpDamageMob.gameObject.SetActive(true);
                    hpMob.gameObject.SetActive(false);
                }
                else
                {
                    defense = _target._defense - Damage;
                    hpMob.gameObject.SetActive(true);
                    hpDamageMob.gameObject.SetActive(false);
                }

                if (hp <= -1)
                    hp = 0;
                #endregion

                hpDamageMob.value = hp / _target._maxHp;
            }
            else
                hpDamageMob.gameObject.SetActive(false);

            if (defense <= 0)
            {
                hpMobText.text = hp.ToString("F0") + " / " + _target._maxHp.ToString("F0");
            }
            else
                hpMobText.text = hp.ToString("F0") + "<color=blue>+(" + defense.ToString("F0") + ") </color> / " + _target._maxHp.ToString("F0");


            hpMob.value = _target._hp / _target._maxHp;

            classeMob.text = "<b>" + _target._classe + "</b>";

            #region Name Skills
            for (int i = 0; i < _target._nameSkills.Count; i++)
            {
                if (i < skillsMob.Count)
                {
                    if (_target._nameSkills[i] != "")
                    {
                        skillsMob[i].SetActive(true);

                        skillsMob[i].GetComponentInChildren<Text>().text =                            
                            (_target.GetComponent<SkillManager>().Skills[i].SilenceSkill ? 
                             "[<color=red>"+XmlMenuInicial.Instance.Get(196)+"</color>]\n" : "")//Silenciada
                             + _target._nameSkills[i];
                    }
                    else
                        skillsMob[i].SetActive(false);
                }
            }
            #endregion
            #endregion

            descricaoItem.text = _target._descricao;

            MobManager mob = Target.GetComponent<MobManager>();

            walkMob.text = _target._walk;

            if (_target.GetComponent<MoveController>() && mob.maxTimeWalk > 0)
                walkMob.text = "<color=green>" + _target.GetComponent<MoveController>().time + "</color>/" + _target._walk;

            
            if (target.GetComponent<MobDbuff>().enabled)
            {
                MobDbuff dbuff = target.GetComponent<MobDbuff>();

                Icons(0, dbuff.Fire   , dbuff.InfoFire).text    = "" + dbuff.FireTurn;
                Icons(1, dbuff.Poison , dbuff.InfoPoison).text  = "" + dbuff.PoisonTurn;
                Icons(2, dbuff.Petrify, dbuff.InfoPetrify).text = "" + dbuff.PetrifyTurn;
                Icons(3, dbuff.Stun   , dbuff.InfoStun).text    = "" + dbuff.StunTurn;
                Icons(4, dbuff.Bleed  , dbuff.InfoBleed).text   = "" + dbuff.BleedTurn;
            }
            else
            {
                Icons(-1);
            }


            if (mob != null && prop)
            {
                string _dodge = "<b>" + XmlMenuInicial.Instance.Get(250)/*Esquiva*/+ "</b>: <color=white><b>" + dodge.ToString("F0") + "</b></color>\n\n";
                if (dodge == 0)
                    _dodge = "";


                string _armor = "<b>" + XmlMenuInicial.Instance.Get(221)/*Armadura*/+ "</b>: <color=black><b>" + armor.ToString("F0") + "</b></color>\n\n";
                if (armor == 0)
                    _armor = "";

                string _chanceCrit = "<b>" + XmlMenuInicial.Instance.Get(161)/*Chance Critico*/+ "</b>: <color=magenta><b>" + (mob.chanceCritical>100 ? "100" : mob.chanceCritical.ToString("F0")) + "</b>%</color>\n\n";
                if (mob.chanceCritical <= 0)
                    _chanceCrit = "";

                string _resistenceFire = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire) + "</b>: <color=grey><b>" + mob.DamageResistenceFire + "</b>%</color>\n";
                if (mob.DamageResistenceFire <= 0)
                    _resistenceFire = "";
                else
                    if (mob.DamageResistenceFire >= 100)
                    _resistenceFire = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Fire) + "</b>: <color=blue>" + XmlMenuInicial.Instance.Get(100)/*Imuni*/+ "</color>\n";

                string _resistencePoison = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar) + "</b>: <color=grey><b>" + mob.DamageResistencePoison + "</b>%</color>\n";
                if (mob.DamageResistencePoison <= 0)
                    _resistencePoison = "";
                else
                    if (mob.DamageResistencePoison >= 100)
                    _resistencePoison = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Envenenar) + "</b>: <color=blue>" + XmlMenuInicial.Instance.Get(100)/*Imuni*/+ "</color>\n";

                string _resistencePetrify = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar) + "</b>: <color=grey><b>" + mob.DamageResistencePetrify + "</b>%</color>\n";
                if (mob.DamageResistencePetrify <= 0)
                    _resistencePetrify = "";
                else
                    if (mob.DamageResistencePetrify >= 100)
                    _resistencePetrify = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Petrificar) + "</b>: <color=blue>" + XmlMenuInicial.Instance.Get(100)/*Imuni*/+ "</color>\n";

                string _resistenceBleed = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed) + "</b>: <color=grey><b>" + mob.DamageResistenceBleed + "</b>%</color>";
                if (mob.DamageResistenceBleed <= 0)
                    _resistenceBleed = "";
                else
                    if (mob.DamageResistenceBleed >= 100)
                    _resistenceBleed = "<b>" + XmlMenuInicial.Instance.DbuffTranslate(Dbuff.Bleed) + "</b>: <color=blue>" + XmlMenuInicial.Instance.Get(100)/*Imuni*/+ "</color>\n";




                infos = "<b>" + _target._name + "</b>\n\n" +
                         _dodge +
                         _armor +
                        _chanceCrit;


                if (_resistenceFire != "" || _resistencePoison != "" || _resistencePetrify != "" || _resistenceBleed != "")
                    infos += XmlMenuInicial.Instance.Get(162) + ":\n" +//Resistencia a Dano [<color=green><b>DBUFF</b></color>]
                            _resistenceFire +
                            _resistencePoison +
                           _resistencePetrify +
                           _resistenceBleed;

                if (_target.GetComponent<MobHealth>())
                    if (!_target.GetComponent<MobHealth>().TakeCriticalDamage)
                        infos = infos + "\n*" + XmlMenuInicial.Instance.Get(200);//iMUNI a dano critico

                if (_target.GetComponent<PassiveManager>())
                {
                    string passiveTT =
                        /*(_target.GetComponent<PassiveManager>().SilenceTime > 0 ?
                        " [ " + XmlMenuInicial.Instance.Get(196)/*Silenciada*//* + " - x" + _target.GetComponent<PassiveManager>().SilenceTime + " ]" : "")+*/
                        _target.GetComponent<PassiveManager>().DescriptionToolType;

                    if (passiveTT != null || passiveTT != "")
                    {
                        infos += "\n" + _target._passiveDesc
                            + (_target.GetComponent<PassiveManager>().CooldownMax>0 ?
                             "\n"+XmlMenuInicial.Instance.Get(68)/*cooldown*/+ ": <color=blue>" + _target.GetComponent<PassiveManager>().CooldownCurrent + "</color> / "+ _target.GetComponent<PassiveManager>().CooldownMax : "")
                            + (_target.GetComponent<PassiveManager>().SilencePassive ?
                             "\n"+ XmlMenuInicial.Instance.Get(196)/*Silenciada*/+ ": x<color=red>" + _target.GetComponent<PassiveManager>().SilenceTime +"</color>" : "");

                    }
                }

                if (_target.extraInInfo.Length != 0)
                    infos = infos+ "\n\n" + _target.extraInInfo;               

                if (_armor !="" || _chanceCrit != "" || _resistenceFire != "" || _resistencePoison != "" || _resistencePetrify != "" || _resistenceBleed != "" || _target.extraInInfo.Length != 0 || _target._passiveDesc.Length !=0)
                    GetComponent<InfoTable>().NewInfo(infos, 10);
            }           
        }

        #region ITEM
        if (_target._type == 0)//Item
        {
            painelMob.GetComponent<CanvasGroup>().alpha  = 0;
            painelItem.GetComponent<CanvasGroup>().alpha = 1;

            descricaoItem.text = _target._descricao;

            if (_target.extraInInfo.Length != 0)
                GetComponent<InfoTable>().NewInfo(_target._name+ "\n" + _target.extraInInfo, 10);

            return;
        }
        #endregion
    }

    Text Icons(int index, bool active = true,string txt="")
    {
        if (index < 0)
        {
            for (int i = 0; i < iconsDbuff.Length; i++)
            {
                iconsDbuff[i].gameObject.SetActive(false);
            }

            return null;
        }

        iconsDbuff[index].gameObject.SetActive(active);

        iconsDbuff[index].GetComponent<ToolTipType>()._descricao = txt;

        return iconsDbuff[index].GetComponentInChildren<Text>();
    }

    public void TargetTooltip(ToolTipType Target)
    {              
        if (Target == null || !Target.gameObject.activeInHierarchy)
        {
            target = null;
            //damage = 0;
            return;
        }

        if (Target == null)
            return;

        CloseTargetDetalSkill();
        painelItem.GetComponent<CanvasGroup>().alpha = 0;
        painelMob.GetComponent<CanvasGroup>().alpha = 0;

        painel.GetComponentsInChildren<Animator>()[0].SetBool("Show", false);

        //painel.SetActive(true);

        painel.GetComponentsInChildren<Animator>()[0].SetBool("Show", true);

        Target.AttToltip();

        #region ITEM
        if (Target._type == 0)//Item
        {
            painelMob.GetComponent<CanvasGroup>().alpha  = 0;
            painelItem.GetComponent<CanvasGroup>().alpha = 1;

            nameTarget.text   = Target._name;
            descricaoItem.text = Target._descricao;
            return;
        }
        #endregion
    }

    public void TargetTooltipCanvas(ToolTipType Target)
    {       
        if (Target == null || !Target.gameObject.activeSelf)
        {
            target = null;
            //damage = 0;
            return;
        }

        if (Target._descricao == null)
        {
            FechaTarget();
            return;
        }

        CloseTargetDetalSkill();

        #region ITEM
        if (Target._type == 0)//Item
        {
            GetComponent<InfoTable>().NewInfo(Target._descricao, 5);
            return;
        }
        #endregion

    }

    public void TargetTooltipCanvas(string _name, string _descricao)
    {
        CloseTargetDetalSkill();

        target = null;

        #region ITEM
        painelMob.GetComponent<CanvasGroup>().alpha  = 0;
        painelItem.GetComponent<CanvasGroup>().alpha = 1;


        nameTarget.text    = _name;
        descricaoItem.text = _descricao;
        #endregion
    }

    public DetalSkillToolTip _targetDetalSkill;
    public int _skillTargetDetalSkill=-1;
    public void TargetDetalSkill(int skill)
    {
        if (target == null)
            return;
       
        _targetDetalSkill = target.GetComponent<DetalSkillToolTip>();

        if (_targetDetalSkill == null)
            return;      

        _skillTargetDetalSkill = skill;

        CloseTargetDetalSkill();
        skillDetalMob.GetComponent<Animator>().SetBool("Show", true);

        string _damage       = XmlMenuInicial.Instance.Get(70)+": <color=red>D%</color>\n";//Dano
        string _cooldown     = XmlMenuInicial.Instance.Get(68)+": <color=green>C%</color> / CM%";//Cooldown
        string _dbuffNaoAlvo = "";

        int dS     = 0;
        int cdS    = 0;
        int maxCdS = 0;

        SkillManager sk = target.GetComponent<SkillManager>();

        if (sk != null && (skill - 1) < sk.Skills.Count)
        {
            _damage = XmlMenuInicial.Instance.Get(70)+": <color=" + (sk.Skills[skill - 1].RealDamage ? "yellow" : "red") + ">D%</color>\n";

            if (sk.Skills[skill - 1].SkillType != Skill_Type.Special || sk.Skills[skill - 1].NeedTarget && sk.Skills[skill - 1].TargetFriend)
                dS = sk.Skills[skill - 1].CurrentDamage;        

            cdS    = sk.Skills[skill - 1].CooldownCurrent;

            maxCdS = sk.Skills[skill - 1].CooldownMax;

            if (sk.Skills[skill - 1].DividedDamage > 1)
            {
                _damage = gms.AttDescrição(_damage,
                    "D%",
                    GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(166),""+(sk.Skills[skill - 1].DividedDamage)));//{0} hit's de D%
            }

            if (sk.Skills[skill - 1].AreaDamage)
            {
                if (dS != 0 && sk.Skills[skill - 1].DamageAreaDamage > 0 && sk.Skills[skill - 1].DamageAreaDamage != 100)
                {
                    _damage += XmlMenuInicial.Instance.Get(163)/*Dano[Não Alvos]*/+ ": <color="+ (sk.Skills[skill - 1].AreaRealDamage ? "yellow" : "red")+ ">" + ((dS * sk.Skills[skill - 1].DamageAreaDamage) / 100) + "</color>\n";
                }

                if (!sk.Skills[skill - 1].TargetMe && sk.Skills[skill - 1].OtherTargetDbuff && sk.Skills[skill - 1]._DbuffBuff.Count > 0)
                {
                    _dbuffNaoAlvo = "<color=blue>"+XmlMenuInicial.Instance.Get(164)+"</color>\n";//Chance Dbuff em Inimigos não Alvos
                }
            }

            txtSkillDetalMob.text = sk.Skills[skill - 1].Description;

            if (txtSkillDetalMob.text == null || txtSkillDetalMob.text == "")
                CloseTargetDetalSkill();
        }

        if (dS > 0)
            _damage = gms.AttDescrição(_damage, "D%", dS.ToString("F0"));
        else
            _damage = "";

        if (maxCdS > 0)
        {
            _cooldown = gms.AttDescrição(_cooldown, "C%", cdS.ToString("F0"));

            _cooldown = gms.AttDescrição(_cooldown, "CM%", maxCdS.ToString("F0"));
        }
        else
            if (maxCdS == 0)
            _cooldown = "<b>"+XmlMenuInicial.Instance.Get(165)+"</b>";//Sem Cooldown

        if (sk.Skills[skill - 1].SilenceSkill)
            _cooldown += "\n<color=red>"+XmlMenuInicial.Instance.Get(196)+": "+ sk.Skills[skill - 1].SilenceTime+ "</color>";

        if (sk.Skills[skill - 1].ChanceUseSkill < 1)
        {
            _cooldown += "\n<color=red>" + (gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(199), (100-(sk.Skills[skill - 1].ChanceUseSkill*100))+"")) + "</color>";
        }

        txtSkillDetalExtraMob.text = _dbuffNaoAlvo + _damage + _cooldown;

        if (txtSkillDetalExtraMob.text == null || txtSkillDetalExtraMob.text == "")
            CloseTargetDetalSkill();
    }

    void AttTargetDetalSkill()
    {
        //Debug.LogError("_targetDetalSkill -> "+ _targetDetalSkill + "/_skillTargetDetalSkill" + _skillTargetDetalSkill);

        if (!skillDetalMob.GetComponent<Animator>().GetBool("Show") ||
            _skillTargetDetalSkill <0                               ||
            _targetDetalSkill==null                                 ||
           !_targetDetalSkill.gameObject.activeInHierarchy)                  
            return;

        //Debug.LogError("AttTargetDetalSkill()");

        skillDetalMob.GetComponent<Animator>().SetBool("Show", false);
        skillDetalMob.GetComponent<Animator>().SetBool("Show", true);

        int skill = _skillTargetDetalSkill - 1;
        string _damage       = XmlMenuInicial.Instance.Get(70)+": <color=red>D%</color>\n";//Dano
        string _cooldown     = XmlMenuInicial.Instance.Get(68)+": <color=green>C%</color> / CM%";//Cooldown
        string _dbuffNaoAlvo = "";

        int dS     = 0;
        int cdS    = 0;
        int maxCdS = 0;

        SkillManager sk = _targetDetalSkill.GetComponent<SkillManager>();

        if (sk != null)
        {
            _damage = XmlMenuInicial.Instance.Get(70)/*Dano*/+": <color=" + (sk.Skills[skill].RealDamage ? "yellow" : "red") + ">D%</color>\n";

            dS = sk.Skills[skill].CurrentDamage;

            if (!sk.Skills[skill].NeedTarget && sk.Skills[skill].TargetFriend)
                dS = 0;

            cdS = sk.Skills[skill].CooldownCurrent;

            maxCdS = sk.Skills[skill].CooldownMax;

            if (sk.Skills[skill].DividedDamage > 1)
            {
                _damage = gms.AttDescrição(_damage,
                    "D%",
                    GameManagerScenes._gms.AttDescriçãoMult(XmlMenuInicial.Instance.Get(166), "" + (sk.Skills[skill].DividedDamage)));//{0} hit's de D%

            }

            if (sk.Skills[skill].AreaDamage)
            {
                if (dS != 0 && sk.Skills[skill].DamageAreaDamage > 0 && sk.Skills[skill].DamageAreaDamage != 100)
                {
                    _damage += XmlMenuInicial.Instance.Get(163)/*Dano[Não Alvos]*/+ ": <color=" + (sk.Skills[skill].AreaRealDamage ? "yellow" : "red") + ">" + ((dS * sk.Skills[skill].DamageAreaDamage) / 100) + "</color>\n";
                }

                if (!sk.Skills[skill].TargetMe && sk.Skills[skill].OtherTargetDbuff && sk.Skills[skill]._DbuffBuff.Count > 0)
                {
                    _dbuffNaoAlvo ="<color=blue>"+XmlMenuInicial.Instance.Get(164)+"</color>\n";//Chance Dbuff em Inimigos não Alvos
                }
            }

            txtSkillDetalMob.text = sk.Skills[skill].Description;

            if (txtSkillDetalMob.text == null || txtSkillDetalMob.text == "")
                CloseTargetDetalSkill();
        }

        if (dS > 0)
            _damage = gms.AttDescrição(_damage, "D%", dS.ToString("F0"));
        else
            _damage = "";

        if (maxCdS > 0)
        {
            _cooldown = gms.AttDescrição(_cooldown, "C%", cdS.ToString("F0"));

            _cooldown = gms.AttDescrição(_cooldown, "CM%", maxCdS.ToString("F0"));
        }
        else
            if (maxCdS == 0)
            _cooldown = "<b>"+XmlMenuInicial.Instance.Get(165)+"</b>";//Sem Cooldown

        txtSkillDetalExtraMob.text = _dbuffNaoAlvo + _damage + _cooldown;

        if (txtSkillDetalExtraMob.text == null || txtSkillDetalExtraMob.text == "")
            CloseTargetDetalSkill();
    }

    public void TargetDetalSkill(ToolTipType Target)
    {
        if (Target._descricao == null)
            return;

        CloseTargetDetalSkill();

        skillDetalMob.GetComponent<Animator>().SetBool("Show", true);

        skillDetalMob.GetComponentInChildren<Text>().text = Target._descricao;

        txtSkillDetalExtraMob.text = "";
    }

    public void FechaTarget(bool click=false)
    {
        CloseTargetDetalSkill();

        painel.GetComponentsInChildren<Animator>()[0].SetBool("Show", false);

        if(click)
        GetComponent<ButtonManager>().ClickAudio();
        //painel.SetActive(false);       

        target = null;
        //damage = 0;
    }

    public void CloseTargetDetalSkill()
    {
        Debug.LogErrorFormat("CloseTargetDetalSkill");

        //skillDetalMob.SetActive(false);

        skillDetalMob.GetComponent<Animator>().SetBool("Show", false);

        txtSkillDetalExtraMob.text = "";
        txtSkillDetalMob.text      = "";
    }

    #region Fade Button Skill
    public void FadeInSkill(int skill)
    {
        clearColor = false;

        if (target.GetComponent<EnemyAttack>() == null/* && target.GetComponent<PlayerAttack>()==null*/)
            return;

        if (!ShowRageSkill)
        {
            bool mesmoTime = target.GetComponent<MobManager>().MesmoTime(RespawMob.Instance.PlayerTime);

            if (mesmoTime)
            {
                if (target.GetComponent<SkillManager>().Skills[skill - 1].TargetFriend)
                    this.GetComponent<CheckGrid>().ColorGrid(2, 0, 0, true);
                else
                    this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
            }
            else//Não sao amigos
            {
                this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
            }

            EnemyAttack targetAttack = target.GetComponent<EnemyAttack>();
            targetAttack.CheckDistance(skill);

            ShowRageSkill = true;
        }
        /*
        switch (skill)
        {
            case 1:
                if (!ShowRageSkill1)
                {
                    ShowRageSkill1 = true;

                    if (mesmoTime)
                    {
                        if (target.GetComponent<SkillManager>().Skills[skill-1].TargetFriend)
                            this.GetComponent<CheckGrid>().ColorGrid(2, 0, 0, true);
                        else
                            this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }
                    else//Não sao amigos
                    {
                        this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }

                    EnemyAttack targetAttack = target.GetComponent<EnemyAttack>();
                    targetAttack.CheckDistance(skill);
                }
                break;

            case 2:
                if (!ShowRageSkill2)
                {
                    ShowRageSkill2 = true;

                    if (mesmoTime)
                    {
                        if (target.GetComponent<MobAttack>().Skill2TargetFriend)
                            this.GetComponent<CheckGrid>().ColorGrid(2, 0, 0, true);
                        else
                            this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }
                    else//Não sao amigos
                    {
                        this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }

                    EnemyAttack targetAttack = target.GetComponent<EnemyAttack>();
                    targetAttack.CheckDistance(skill);
                }
                break;

            case 3:
                if (!ShowRageSkill3)
                {
                    ShowRageSkill3 = true;

                    if (mesmoTime)
                    {
                        if (target.GetComponent<MobAttack>().Skill3TargetFriend)
                            this.GetComponent<CheckGrid>().ColorGrid(2, 0, 0, true);
                        else
                            this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }
                    else//Não sao amigos
                    {
                        this.GetComponent<CheckGrid>().ColorGrid(1, 0, 0, true);
                    }

                    EnemyAttack targetAttack = target.GetComponent<EnemyAttack>();
                    targetAttack.CheckDistance(skill);
                }
                break;
        }
        */
    }

    public void FadeOutSkill(int skill)
    {
        clearColor = true;

        if (target.GetComponent<EnemyAttack>() == null)
            return;

        if (ShowRageSkill)
        {
            ShowRageSkill = false;
            Invoke("ClearColorHex", 0.5f);
        }
        /*
        switch (skill)
        {
            case 1:
                if (ShowRageSkill1)
                {
                    ShowRageSkill1 = false;
                    Invoke("ClearColorHex", 0.5f);
                }
                break;

            case 2:
                if (ShowRageSkill2)
                {
                    ShowRageSkill2 = false;
                    Invoke("ClearColorHex", 0.5f);
                }
                break;

            case 3:
                if (ShowRageSkill3)
                {
                    ShowRageSkill3 = false;
                    Invoke("ClearColorHex", 0.5f);
                }
                break;
        }

        //TargetTooltip(target);
        */
    }

    void ClearColorHex()
    {
        if (clearColor)
            this.GetComponent<CheckGrid>().ColorGrid(0, 0, 0, true);
    }
    #endregion

}
