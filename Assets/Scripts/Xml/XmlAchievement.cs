using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class AchievementXml
{
    public string  _nameX;
    public int     _ID;
    //[TextArea]
    public string _description;
    public string _type;
    public string _dlc;
}

[System.Serializable]
public class AchievementDatabase
{
    [XmlArray("Achievement")]//Nome da tag no XML
    public List<AchievementXml> list = new List<AchievementXml>();
}


public class XmlAchievement : XmlManager
{   
    public static XmlAchievement Instance;

    public AchievementDatabase ADB;

    protected override IEnumerator Start()
    {       
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        yield return new WaitForSeconds(3);

        Load();

       yield return base.Start();

        GameManagerScenes.DelegateChangeLanguageXml += Load;  
    }

    void Load()
    {
        if (GameManagerScenes._gms.IsMobile)
        {
            GameManagerScenes._gms.BuildAchievement();
            return;
        }

        ADB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(AchievementDatabase));     

        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Achievement/" + _nameXml + ".xml";
#elif UNITY_ANDROID
        nameStream = "jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Achievement/" + _nameXml + ".xml";

        //nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Achievement/" + _nameXml + ".xml";

         WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamMs = new MemoryStream(reader.bytes);

        if (streamMs != null)
        {
            ADB = serializer.Deserialize(streamMs) as AchievementDatabase;
            streamMs.Close();

            Debug.LogError("Android LoadXml- Achievement");
        }      
#elif UNITY_IPHONE
        nameStream =Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Achievement/" + _nameXml + ".xml";
#else
        nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Achievement/" + _nameXml + ".xml";
#endif


#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            ADB = serializer.Deserialize(stream) as AchievementDatabase;
            stream.Close();
            Debug.LogError("LoadXml- Achievement");
        }
        else
        {
            print("Conquista Não encontrada!!!");

            GameManagerScenes._gms.LoadingBar("<color=red>ERRO Load Achievement</color>", 1);
            return;
        }
#endif

        if (ADB.list.Count >= 1)
        {
            foreach (var s in ADB.list)
            {
                if (s._nameX != null)
                    s._nameX = CheckAndChangeWords(s._nameX);

                if (s._description != null)
                    s._description = CheckAndChangeWords(s._description);
            }

            print("Achievement Carregadas.");

            GameManagerScenes._gms.BuildAchievement();
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n"+nameStream,5);
        }
    }

    public AchievementXml GetAchievement(int ID)
    {
        AchievementXml _return = null;

        if (ADB.list.Count > 0)
        {
            if (ADB.list[ID]._ID == ID)
            {
                return ADB.list[ID];
            }

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();
        
          if (ADB.list.Count>0)                       
            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s;
                }
            }

          //  print("Achievement <b>" + ID + "</b> Carregada.");       

        return _return;
    }

    public string GetNameAchievement(int ID)
    {
        string _return = null;

        if (ADB.list.Count>0)
        {
            if (ADB.list[ID]._ID == ID)
            {
                return ADB.list[ID]._nameX;
            }

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s._nameX;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }


        Load();

        foreach (var s in ADB.list)
        {
            if (ID == s._ID)
                _return = CheckAndChangeWords(s._nameX);
        }

        return _return;
    }

    public string GetDescricaoAchievement(int ID)
    {
        string _return = null;

        if (ADB.list.Count > 0)
        {
            if (ADB.list[ID]._ID == ID)
            {
                return ADB.list[ID]._description;
            }

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s._description;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = CheckAndChangeWords(s._description);
                }
            }

        return _return;
    }

    public string GetTypeAchievement(int ID)
    {
        string _return = null;

        if (ADB.list.Count > 0)
        {
           // print("ID");
            if (ADB.list[ID]._ID==ID)
            {
                return ADB.list[ID]._type;
            }

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s._type;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = CheckAndChangeWords(s._type);
                }
            }

        return _return;
    }

    public string GetDlcAchievement(int ID)
    {
        string _return = null;

        if (ADB.list.Count > 0)
        {
            if (ADB.list[ID]._ID == ID)
            {
                return ADB.list[ID]._dlc;
            }

            foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = s._dlc;
                }
            }

            if (_return != null)
            {
                return _return;
            }
        }

        Load();

       foreach (var s in ADB.list)
            {
                if (ID == s._ID)
                {
                    _return = CheckAndChangeWords(s._dlc);
                }
            }

        return _return;
    }
}
