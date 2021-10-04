using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

[System.Serializable]
public class MobXml
{
    public string _nameX;
    public string _class;
    public string _description;
    [XmlArray("_skinsName")]
    [XmlArrayItem("_skinName")]
    public List<string> _skinName = new List<string>();
}

    [System.Serializable]
    public class MobManagerDatabase
    {
        [XmlArray("Mob")]//Nome da tag no XML
        public List<MobXml> list = new List<MobXml>();
    }

public class XmlMobManager : XmlManager
{
    public static XmlMobManager Instance;

    public MobManagerDatabase MMDB;

    protected override IEnumerator Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
            Destroy(this);       

        yield return base.Start();

        Load();

        GameManagerScenes.DelegateChangeLanguageXml += Load;
    }

    void Load()
    {
        if (GameManagerScenes._gms.IsMobile)
        {

            return;
        }

        MMDB.list.Clear();

        string language = GameManagerScenes._gms.Language().ToString();

        XmlSerializer serializer = new XmlSerializer(typeof(MobManagerDatabase));        

        string nameStream = "";

#if UNITY_EDITOR
        nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Mob/" + _nameXml + ".xml";
#elif UNITY_ANDROID
        nameStream ="jar:file://" + Application.dataPath + "!/assets/StreamingAssets/Xml/" + language + "/Mob/" + _nameXml + ".xml";       
        //nameStream = Application.streamingAssetsPath + "/StreamingAssets/Xml/" + language + "/Mob/" + _nameXml + ".xml";

        WWW reader = new WWW(nameStream);

        while (!reader.isDone)
        {
            //wait for the reader to finish downloading        
        }

        MemoryStream streamMs = new MemoryStream(reader.bytes);

        if (streamMs.CanRead)
        {
           MMDB = serializer.Deserialize(streamMs) as MobManagerDatabase;

            streamMs.Close();
  
            Debug.LogError("Android LoadXml-Mob's Carregadas.");
        }
#elif UNITY_IPHONE
        nameStream =Application.dataPath + "/Raw/StreamingAssets/Xml/" + language + "/Mob/" + _nameXml + ".xml";
#else
      nameStream = Application.streamingAssetsPath + "/Xml/" + language + "/Mob/" + _nameXml + ".xml";
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
        FileStream stream = new FileStream(nameStream, FileMode.Open);

        if (stream.CanRead)
        {
            print("LoadXml-Mob's Carregadas.");
            MMDB = serializer.Deserialize(stream) as MobManagerDatabase;
            stream.Close();
        }
        else
            return;
#endif

        if (MMDB.list.Count > 0)
        {
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);

                for (int i = 0; i < s._skinName.Count; i++)
                    s._skinName[i] = CheckAndChangeWords(s._skinName[i]);
            }


            int _countMob = GameManagerScenes._gms.Mob.Count,
                _countList = MMDB.list.Count;

            if (_countMob > _countList)
                Debug.LogError("<color=yellow><b>Talvez Possa conter Mobs Não registrados(Count: " + _countMob + "-" + _countList + ", Language: " + GameManagerScenes._gms.Language().ToString() + ")</b></color>");

            for (int i = 0; i < _countMob; i++)
            {
                if (MMDB.list.Count > i)
                    GameManagerScenes._gms.Mob[i]._nameHero = MMDB.list[i]._nameX;
                else
                    Debug.LogError("<color=red><b>Mob Não Registrado Encontrado (HeroID: [" + i + "], Language: " + GameManagerScenes._gms.Language().ToString() + ")</b></color>");

                int _countSkin = GameManagerScenes._gms.Mob[i]._skinHero.Count;
                if (_countSkin > 0)
                    for (int j = 0; j < _countSkin; j++)
                    {
                        if (MMDB.list[i]._skinName.Count > j)
                            GameManagerScenes._gms.Mob[i]._skinHero[j]._nameSkin = MMDB.list[i]._skinName[j];
                        else
                            Debug.LogError("<color=red><b>Skin Não Registrada Encontrada (Hero[" + i + "]: " + MMDB.list[i]._nameX + ", Skin: " + (1 + j) + ", Language: " + GameManagerScenes._gms.Language().ToString() + ")</b></color>");
                    }
            }

            print("Mob's Carregadas.");
        }
        else
        {
            GameManagerScenes._gms.NewInfo("Arquivo não encontrado\n" + nameStream, 5);
        }
    }

    public MobXml Mob(int ID)
    {
        if (MMDB.list.Count > 0)
        {
            if (ID > -1 && ID < MMDB.list.Count)
            {
                return MMDB.list[ID];
            }
        }

        MobXml _return = null;

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);

                for (int i = 0; i < s._skinName.Count; i++)
                    s._skinName[i] = CheckAndChangeWords(s._skinName[i]);                                    
            }

            if (MMDB.list.Count > -1 && ID < MMDB.list.Count)
                _return = MMDB.list[ID];        

        return _return;
    }

    public string Name(int ID)
    {
        if (MMDB.list.Count > 0)
        {
            if (ID >= 0 && ID < MMDB.list.Count)
            {
                return MMDB.list[ID]._nameX;
            }
        }

        string _return = "";

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);
            }

        if (ID >=0 && ID < MMDB.list.Count)
            _return = MMDB.list[ID]._nameX;

        return _return;
    }

    public string Name(float ID)
    {
        int index = (int)ID;
        float idSkin = 10*ID;

        Debug.LogWarning("name ID:"+index+" / Skin:"+idSkin);

        if (MMDB.list.Count > 0)
        {
            if (index >= 0 && index < MMDB.list.Count)
            {
                int count = MMDB.list[index]._skinName.Count;
                if (ID >= 0 && ID < count)
                {
                    return MMDB.list[index]._skinName[(int)idSkin];
                }
            }
        }

        string _return = "";

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);
            }

        if (MMDB.list.Count > 0)
        {
            if (index >= 0 && index < MMDB.list.Count)
            {
                int count = MMDB.list[index]._skinName.Count;
                if (count <= ID && ID < count)
                {
                    _return = MMDB.list[index]._skinName[(int)idSkin];
                }
            }
        }

        return _return;
    }

    public string Class(int ID)
    {
        if (MMDB.list.Count > 0)
        {
            if (ID > -1 && ID < MMDB.list.Count)
            {
                return MMDB.list[ID]._class;
            }
        }

        string _return = "";

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);
            }

        if (ID > -1 && ID < MMDB.list.Count)
            _return = MMDB.list[ID]._class;

        return _return;
    }

    public string Description(int ID)
    {
        if (MMDB.list.Count > 0)
        {
            if (ID > -1 && ID < MMDB.list.Count)
            {
                return MMDB.list[ID]._class;
            }
        }

        string _return = "";

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);
            }

        if (ID > -1 && ID < MMDB.list.Count)
            _return = MMDB.list[ID]._class;

        return _return;
    }

    public string SkinName(int ID,int IDSkin)
    {
        if (MMDB.list.Count > 0)
        {
            if (ID > -1 && ID < MMDB.list.Count)
            {
                if (IDSkin > -1 && IDSkin < MMDB.list[ID]._skinName.Count)
                {
                    return MMDB.list[ID]._skinName[IDSkin];
                }             
            }
        }

        string _return = "";

        Load();

        if (MMDB.list.Count > 0)
            foreach (var s in MMDB.list)
            {
                s._nameX = CheckAndChangeWords(s._nameX);
                s._class = CheckAndChangeWords(s._class);
                s._description = CheckAndChangeWords(s._description);
            }

        if (ID > -1 && ID < MMDB.list.Count)
            if (IDSkin > -1 && IDSkin < MMDB.list[ID]._skinName.Count)
                _return = MMDB.list[ID]._skinName[IDSkin];

        return _return;
    }
}
