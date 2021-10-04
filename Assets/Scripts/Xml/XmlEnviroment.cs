using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class EnviromentXml
{
    public string _name;
    public string _extraInfo;
    [XmlArray("_descriptions")]
    [XmlArrayItem("_description")]
    public List<string> _description = new List<string>();
}

[System.Serializable]
public class EnviromentDatabase
{
    [XmlArray("Enviroment")]//Nome da tag no XML
    public List<EnviromentXml> list = new List<EnviromentXml>();
}

public class XmlEnviroment : XmlManager
{
    public static XmlEnviroment Instance;

    public EnviromentDatabase IDB;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);

        Load();

        yield return base.Start();

        GameManagerScenes.DelegateChangeLanguageXml += Load;
    }

    void Load()
    {
        if (GameManagerScenes._gms.IsMobile)
        {

            return;
        }

        IDB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(EnviromentDatabase));

        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Environment/" + _nameXml + ".xml";
#elif UNITY_ANDROID
        nameStream ="jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Environment/" + _nameXml + ".xml";       
        //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Environment/" + _nameXml + ".xml";

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamb = new MemoryStream(reader.bytes);

        if (streamb != null && streamb.CanRead)
        {
            IDB = serializer.Deserialize(streamb) as EnviromentDatabase;
            streamb.Close();
            print("Android LoadXml-Envoriment's Carregadas.");
        }         
#elif UNITY_IPHONE
        nameStream =Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Environment/" + _nameXml + ".xml";
#else
      nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Environment/" + _nameXml + ".xml";
#endif


#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            IDB = serializer.Deserialize(stream) as EnviromentDatabase;
            stream.Close();
        }
        else
        {
            print("Passiva DB Não encontrada!!!");
            return;
        } 
#endif


        if (IDB.list.Count > 0)
            {
                foreach (var s in IDB.list)
                {
                    s._name      = CheckAndChangeWords(s._name);
                    s._extraInfo = CheckAndChangeWords(s._extraInfo);                 

                    int count = s._description.Count;

                    if (count > 0)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            s._description[i] = (CheckAndChangeWords(s._description[i]));
                        }
                    }
                }              
           

            print("Envoriment's Carregadas.");
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
    }

    public string[] Get(int ID)
    {
        if (IDB.list.Count > 0)
        {
            if (IDB.list.Count > -1 && ID < IDB.list.Count)
            {
                return IDB.list[ID]._description.ToArray();
            }
        }

        string[] _return = { };


        Load();

        if (IDB.list.Count > 0)
        {
            foreach (var s in IDB.list)
            {
                for (int i = 0; i < s._description.Count; i++)
                {
                    if (i == ID)
                        s._description[i] = (CheckAndChangeWords(s._description[i]));
                }
            }
        }

        if (ID<IDB.list.Count)              
        _return = IDB.list[ID]._description.ToArray();

        return _return;
    }

    public string GetName(int ID)
    {
        if (IDB.list.Count > 0)
        {
            if (IDB.list.Count > -1 && ID < IDB.list.Count)
            {
                return IDB.list[ID]._name;
            }
        }

        string _return = "";

        Load();

        if (IDB.list.Count > 0)
        {
            foreach (var s in IDB.list)
            {
                _return = s._name = CheckAndChangeWords(s._name);
            }
        }

        return _return;
    }

    public string GetExtraInf(int ID)
    {
        if (IDB.list.Count > 0)
        {
            if (IDB.list.Count > -1 && ID < IDB.list.Count)
            {
                return IDB.list[ID]._extraInfo;
            }
        }

        string _return = "";

        Load();

        if (IDB.list.Count > 0)
            {
                foreach (var s in IDB.list)
                {
                    _return = s._extraInfo = CheckAndChangeWords(s._extraInfo);
                }
            }

        return _return;
    }
}
