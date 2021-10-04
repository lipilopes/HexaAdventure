using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class AchievementPanel : MonoBehaviour
{
    [SerializeField] GameObject painelModelo;
    [Space]
    [SerializeField] GameObject painelDlcModelo;
    [Space]
    [SerializeField] GameObject painelInf;
    [Space]
    [SerializeField] Color block;
    [Space]
    [SerializeField] List<GameObject> painelList = new List<GameObject>();
    [Space]
    [SerializeField] List<GameObject> painelDlcList = new List<GameObject>();

    GameManagerScenes gms;

    WaitForSeconds loadingWait = new WaitForSeconds(0.08f);

    private void OnEnable()
    {
        Ajust();
        Att();
        AttDlc();
        Ajust(true);
    }

    IEnumerator Start()
    {
        gms = GameManagerScenes._gms;

        painelModelo.SetActive(false);
        painelDlcModelo.SetActive(false);
        //gms.BuildAchievement();

        int count = gms._achievement.Count;

        for (int i = 0; i < count; i++)
        {
            //bool checkPainelDLC = CheckPainelDlc(gms._achievement[i]._dlc.ToString());

            string dlcName = (/*gms.IsMobile ? gms._achievement[i]._dlc.ToString() :*/ XmlAchievement.Instance.GetDlcAchievement(i));

            bool checkPainelDLC = CheckPainelDlc(gms._achievement[i]._dlc,i);//CheckPainelDlc(dlcName);            

            if (checkPainelDLC == false)
            {
                if ((painelDlcList.Count - 1) >= 0)
                    DlcPainelClick(painelDlcList.Count - 1,false);          

                GameObject P_dlc = Instantiate(painelDlcModelo, transform.parent);

                string dlc_name = DlcName(i);

                P_dlc.name      = "Painel ("+dlc_name+")";

                P_dlc.GetComponentInChildren<Text>().text = dlc_name;

                P_dlc.transform.SetParent(this.transform);                

                P_dlc.SetActive(true);

                painelDlcList.Add(P_dlc);

                int ID_P_Dlc = painelDlcList.Count - 1;

                painelDlcList[painelDlcList.Count-1].GetComponent<Button>().onClick.AddListener(() => DlcPainelClick(ID_P_Dlc));
            }

            GameObject P = Instantiate(painelModelo, transform.parent);

            P.transform.SetParent(this.transform);

            ///Muda posição na list 
            if (checkPainelDLC)
            {
               // Debug.LogWarning("ChangePositionInListAchievement(" + gms._achievement[i]._nameX + " - " + gms._achievement[i]._dlc + " )");
                P.transform.SetSiblingIndex(ChangePositionInListAchievement(dlcName));
            }

            painelList.Add(P);

            int ID = i;

            painelList[i].GetComponent<Button>().onClick.AddListener(() => SeeMoreAchievement(ID));                                  

            bool complete = gms._achievement[i]._complete;

            bool secret   = gms._achievement[i]._secret;

            float progess = (gms.AnchievementProgress(i));

            Image progresImg = GetProgess(i);

            Image img = GetIcon(i);

            Text name = GetName(i);

            P.name = gms._achievement[i]._name;

            GetTextProgess(i).text = (progess * 100).ToString("F0") + "%";

            if (progresImg != null)
                progresImg.rectTransform.offsetMax = new Vector2((progess*368 - 368), 0);

            name.text = "?????";

            if (img != null)
            {
                img.sprite = gms.secretAchievement;

                img.color = block;

                img.type          = Image.Type.Simple;
                img.preserveAspect = true;
            }

            if (!complete && !secret || gms.Adm)
            {
                name.text = NameColor(i) + gms._achievement[i]._name + "</color>";

                if (img != null)
                {
                    img.sprite = gms._achievement[i]._icon;
                    img.color  = block;
                }

            }
            else
            if (complete)
                DesBlock(i);

            gms.LoadingBar(name.text+"\n"+XmlMenuInicial.Instance.Get(186)+"...",i,count-1);//Loading...

            yield return loadingWait;

            P.SetActive(true);
            GetComponent<AjustarContent>().Alterar(true);
        }

        //Deixar os paineis de dlc abertos
        for (int i = 0; i < painelDlcList.Count; i++)
        {
            DlcPainelClick(i, true);
        }

        #region Manter a ordem original
        List<int> _dlcOk = new List<int>();

        for (int i = 0; i < painelList.Count; i++)
        {
            int _dlc = (int)gms._achievement[i]._dlc;

            if (!_dlcOk.Contains(_dlc))
            {
                _dlcOk.Add(_dlc);

                int indexSibling = painelDlcList[_dlc].transform.GetSiblingIndex() + 1;
                Debug.LogWarning("<color=red>ChangePositionInListAchievement(" + gms._achievement[i]._name + " - " + gms._achievement[i]._dlc + " ) [" + indexSibling + "]</color>");
                painelList[i].transform.SetSiblingIndex(indexSibling);
            }
        }
        #endregion

        Att();
        AttDlc();       

        //Ajust(true);
    }


    /// <summary>
    /// Checa se Painel de Dlc ja foi criado para evitar repetições
    /// </summary>
    /// <param name="_dlc"></param>
    /// <returns></returns>
    bool CheckPainelDlc(string _dlc)
    {
        if (painelDlcList.Count > 0)
        for (int i = 0; i < painelDlcList.Count; i++)       
            if (//painelDlcList[i].name == DlcName(_dlc) &&
                painelDlcList[i].name.Contains(DlcName(_dlc)))
                return true;

        return false;
    }

    bool CheckPainelDlc(Achievement._DLC _dlc, int indexAchievement)
    {
        if (painelDlcList.Count > 0)
        {
            string _dlcN = DlcName(XmlAchievement.Instance.GetDlcAchievement(indexAchievement));

            for (int i = 0; i < painelDlcList.Count; i++)
                if (painelDlcList[i].name.Contains(DlcName(_dlcN)) &&
                    _dlc == gms._achievement[indexAchievement]._dlc)
                    return true;
        }

        return false;
    }

    int ChangePositionInListAchievement(string _dlc)
    {        
        int index = painelList.Count;

        //for (int i = painelList.Count-1; i > 0; i--)
        for (int i = 0; i < painelList.Count; i++)
        {
            if ((/*!gms.IsMobile ?*/ XmlAchievement.Instance.GetDlcAchievement(i)/* : gms._achievement[i]._dlc.ToString()*/) == _dlc)
            {
                index = /*painelList.Count -*/painelList[i].transform.GetSiblingIndex();
                Debug.LogWarning("ChangePositionInListAchievement(" + _dlc + ") ->["+ index + "] "+ painelList[i].name);
                break;
            }         
        }

        return index;
    }

    public void Att()
    {
        if (painelList.Count <= 0)
            return;

        for (int i = 0; i < painelList.Count; i++)
        {
            if (!gms._achievement[i]._complete)
            {
                float progess = (gms.AnchievementProgress(i));

                GetTextProgess(i).text = (progess * 100).ToString("F0") + "%";

                GetProgess(i).rectTransform.offsetMax = new Vector2((progess * 368 - 368), 0);

                //print("Id Conquista "+i + " Progres:" + progess);
            }
            else
                DesBlock(i);
        }
    }

    public void AttDlc()
    {
        if (painelList.Count <= 0)
            return;

        print("AttDlc");

        for (int ID = 0; ID < painelDlcList.Count; ID++)
        {
            int total    = 0;
            int complete = 0;

            for (int i = 0; i < painelList.Count; i++)
            {
                if ((int)gms._achievement[i]._dlc == ID)
                {
                    total++;

                    if (gms._achievement[i]._complete || gms._achievement[i]._feito >= gms._achievement[i]._max)
                        complete++;
                }
            }

            print(DlcName(ID) + " " + complete + "/" + total);


            //Fecha aba da dlc caso esteja completo         
            if (total == complete && total != 0)
            {
                painelDlcList[ID].GetComponentsInChildren<Image>()[3].enabled = (true);

                if (ID > painelDlcList.Count - 1)
                    DlcPainelClick(ID, false);
            }
        }
    }

    void DesBlock(int index)
    {
        Image img = GetIcon(index);

        Text name = GetName(index);

        if (gms._achievement[index]._complete)
        {
            name.text          = NameColor(index)+"<b>"+gms._achievement[index]._name+"</b></color>";

            GetTextProgess(index).text = "100%";

            GetProgess(index).rectTransform.right = new Vector2(0, 0);

            img.sprite         = gms._achievement[index]._icon;
            img.color          = new Color(255, 255, 255, 255);
            img.type           = Image.Type.Simple;
            img.preserveAspect = true;
        }
    }

    Text GetName(int index)
    {
        Text txt = painelList[index].GetComponentsInChildren<Text>()[0];

        if (txt.name == "Text Name Achievement")
            return txt;
        else
            return null;
       
    }

    Text GetTextProgess(int index)
    {
        Text txt = painelList[index].GetComponentsInChildren<Text>()[1];

        if (txt.name == "Slider Text Progress")
            return txt;
        else
            return null;
    }

    Image GetIcon(int index)
    {
        Image img = painelList[index].GetComponentsInChildren<Image>()[1];

        if (img.name == "Icon Achievement")
            return img;
        else
            return null;
    }

    Image GetProgess(int index)
    {
        Image img = painelList[index].GetComponentsInChildren<Image>()[3];

        if (img.name == "Slider Progres")
            return img;
        else
            return null;
    }

    string NameColor(int index)
    {
        switch (gms._achievement[index]._type)
        {
            case Achievement._Type.Bronze:
               return "<color=#CD7F32>";

            case Achievement._Type.Prata:
                return "<color=#838383FF>";

            case Achievement._Type.Ouro:
                return "<color=#C3A607FF>";

            case Achievement._Type.Platina:
                return "<color=#2C4067FF>";
        }

        return "<color=black>";
    }

    string DlcName(int id)
    {
        //return gms.AttDescrição(gms._achievement[id]._dlc.ToString(), "_", " ", gms._achievement[id]._dlc.ToString());

        string dlc = XmlAchievement.Instance.GetDlcAchievement(id);

        return gms.AttDescrição(dlc, "_", " ", dlc);
    }

    string DlcName(string _dlc)
    {
        return gms.AttDescrição(_dlc, "_", " ", _dlc);
    }

    /// <summary>
    /// Click In Achievement
    /// </summary>
    /// <param name="ID"></param>
    public void SeeMoreAchievement(int ID)
    {
        int id = ID;

        if (id < 0 || id > painelList.Count)
            return;

        painelInf.GetComponent<Animator>().SetBool("Show", true);

        Image _Img  = painelInf.GetComponentsInChildren<Image>()[2];

        Text _Name = painelInf.GetComponentsInChildren<Text>()[0];

        Text _Descr = painelInf.GetComponentsInChildren<Text>()[1];

        bool complete = gms._achievement[id]._complete;

        bool secret = gms._achievement[id]._secret;

        string _name = "",
               _desc = "",
               _cor  = NameColor(ID),
               _type = gms.AttDescriçãoMult(/*!gms.IsMobile ? */XmlMenuInicial.Instance.Get(55)/* : "Tipo: {0}"*/,""),
               _dlc  = "",
               _progress = "(1/3)";

        if (gms._achievement[id]._feito >= gms._achievement[id]._max && !complete)
        {
            gms.CheckAchievement(id, gms._achievement[id]._feito,false);

            complete = true;
        }

        if ((int)gms._achievement[id]._dlc != 0)
        _dlc = "Dlc: " + "<color=black>" + DlcName(id) + "</color>" + "\n\n";

        _type += _cor + (/*!gms.IsMobile ?*/ XmlAchievement.Instance.GetTypeAchievement(ID)/* : gms._achievement[ID]._type.ToString()*/)+"</color>"+ "\n\n";        

        Sprite _icon = null;

        //_Img.color  = new Color(255, 255, 255, 255);

        _Img.color    = block;

        if (gms.Adm)
        {
            _progress = "\n\n(" + gms._achievement[id]._feito + "/" + gms._achievement[id]._max + ")";

            _name = gms._achievement[id]._name;

            string _secret = "";

            if (gms._achievement[id]._secret)
                _secret = "<color=black>"+(/*!gms.IsMobile ? */XmlMenuInicial.Instance.Get(51) /*: "Secreto"*/)+"</color>\n\n";

            _desc = _secret+_dlc + _type + gms._achievement[id]._descricao + _progress;

            _icon = gms._achievement[id]._icon;

            _Img.color = new Color(255, 255, 255, 255);

            _name = _cor + "<b>" + _name + "</b></color>";
        }
        else
        if (secret && !complete)
        {
            _name     = "?????";

            _type     = "<color=black>" + (/*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(51) /*: "Secreto"*/) + "</color>\n\n";

            _progress = "\n\n(" + gms._achievement[id]._feito+ "/???)";

            _desc     = _dlc + _type + (/*!gms.IsMobile ? */XmlMenuInicial.Instance.Get(52)/* : "Esse Achievement e Secreto, Desbloquei para mais informacoes"*/)+_progress;

            _icon     = gms.secretAchievement;
        }
        else
        if (!secret && !complete)
        {
            _name     = gms._achievement[id]._name;

            _progress = "\n\n(" + gms._achievement[id]._feito + "/"+gms._achievement[id]._max+")";

            _desc     = _dlc + _type + gms._achievement[id]._descricao + _progress;

            _icon     = gms._achievement[id]._icon;
        }
          else
        if (complete || secret && complete)
        {
            _progress  = "\n\n<color=green><size=15>✓</size> "+(/*!gms.IsMobile ?*/ XmlMenuInicial.Instance.Get(53) /*: "Completo"*/)+"</color>";

            _name      = gms._achievement[id]._name;

            _desc      = _dlc + _type + gms._achievement[id]._descricao + _progress;

            _icon      = gms._achievement[id]._icon;

            _Img.color = new Color(255, 255, 255, 255);

            _name       = _cor + "<b>" + _name + "</b></color>";
        }


        _Name.text          = _name;
        _Descr.text         = _desc;

        _Img.sprite         = _icon;
        _Img.type           = Image.Type.Simple;
        _Img.preserveAspect = true;
    }

    public void CloseSeeMoreAchievement()
    {
        painelInf.GetComponent<Animator>().SetBool("Show", false);
    }

    public void AnimAchievementOpen(Animator anim)
    {
        if (anim!=null)
            anim.SetBool("Open",true);
    }

    public void AnimAchievementClose(Animator anim)
    {
        if (anim != null)
            anim.SetBool("Open", false);
    }

    public void DlcPainelClick(int ID)
    {
        print("DlcPainelClick(" + ID + ")");

        Quaternion close = new Quaternion(180, 0, 0, 1);
        Quaternion open  = new Quaternion(  0, 0, 0, 1);

        if (painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation == open)
        {
            print("show Only Achievements dlc("+ID+")");

            for (int i = 0; i < painelList.Count; i++)
            {
                if ((int)gms._achievement[i]._dlc == ID)
                {
                    painelList[i].SetActive(true);
                }               
            }

            painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation = close;
            Ajust(true);
        }
        else
        if (painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation == close)
        {
            print("close Only Achievements dlc(" + ID + ")");

            for (int i = 0; i < painelList.Count; i++)
            {
                if ((int)gms._achievement[i]._dlc == ID)
                {
                    painelList[i].SetActive(false);
                }               
            }

            painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation = open;

            Ajust(false);
        }      
    }

    public void DlcPainelClick(int ID,bool _open = true)
    {
        print("DlcPainelClick(" + ID + ")");

        Quaternion close = new Quaternion(180, 0, 0, 1);
        Quaternion open = new Quaternion(0, 0, 0, 1);

        if (_open)
        {
            print("show Only Achievements dlc(" + ID + ")");

            for (int i = 0; i < painelList.Count; i++)
            {
                if ((int)gms._achievement[i]._dlc == ID)
                {
                    painelList[i].SetActive(true);
                }
            }

            painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation = close;           
        }
        else
        {
            print("close Only Achievements dlc(" + ID + ")");

            for (int i = 0; i < painelList.Count; i++)
            {
                if ((int)gms._achievement[i]._dlc == ID)
                {
                    painelList[i].SetActive(false);
                }
            }

            painelDlcList[ID].GetComponentsInChildren<RectTransform>()[1].rotation = open;          
        }

        Ajust(_open);
    }

    void Ajust(bool open = false)
    {
        int count = 0;

        foreach (var item in painelList)
            if (item.activeInHierarchy/* && open*/)
                count++;/*
        else if (item.activeInHierarchy && !open)
                count--;*/

        if (count < 0)
            count = 0;

        GetComponent<AjustarContent>().Alterar(count);
    }
}
