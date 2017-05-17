  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;

public class XMLManager : MonoBehaviour {

    AWSscript AWS;

    private static XMLManager instance = null;

    public static XMLManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        AWS = AWSscript.GetInstance();
    }

    public void SaveLevel(string fileName, LevelDatabase levelDB)
    {
        if (!Directory.Exists(Application.dataPath + "/Serialization/XML"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Serialization/XML");
        }
        XmlSerializer serilizer = new XmlSerializer(typeof(LevelDatabase));
        StreamWriter stream = new StreamWriter(Application.dataPath + "/Serialization/XML/" + fileName + ".xml", false, Encoding.GetEncoding("UTF-8"));
        serilizer.Serialize(stream, levelDB);
        stream.Close();
    }

    public void UploadLevel(string fileName, AndroidDatabase androidDB)
    {
        if (!Directory.Exists(Application.dataPath + "/Serialization/XMLa/temp"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Serialization/XMLa/temp");
        }
        XmlSerializer serilizer = new XmlSerializer(typeof(AndroidDatabase));
        StreamWriter stream = new StreamWriter(Application.dataPath + "/Serialization/XMLa/temp/" + fileName + ".xml", false, Encoding.GetEncoding("UTF-8"));
        serilizer.Serialize(stream, androidDB);
        stream.Close();
        //uploading
        AWS.AWSUploadXML(fileName);
        StartCoroutine(UploadingXML(fileName));
    }

    public LevelDatabase LoadLevel(string fileName)
    {
        XmlSerializer serilizer = new XmlSerializer(typeof(LevelDatabase));
        FileStream stream = new FileStream(Application.dataPath + "/Serialization/XML/" + fileName, FileMode.Open);
        LevelDatabase levelDB = serilizer.Deserialize(stream) as LevelDatabase;
        stream.Close();
        return levelDB;
    }

    IEnumerator UploadingXML(string fileName)
    {
        while (AWS.uploadingXML)
        {
            Debug.Log("Uploading ~");
            if (AWS.uploadedXML && Directory.Exists(Application.dataPath + "/Serialization/XMLa/temp"))
            {
                Debug.Log("Finished uploading");
                Directory.Delete(Application.dataPath + "/Serialization/XMLa/temp", true);
                AWS.uploadingXML = false;
                AWS.uploadedXML = false;
                UIManager.GetInstance().Status.text = "Uploaded";
                yield break;
            }
            yield return null;
        }
    }
}


