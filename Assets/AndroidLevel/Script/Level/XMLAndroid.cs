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

public class XMLAndroid : MonoBehaviour {

    AWSscript AWS;
    LevelCreator lvlCreator;

    AndroidDatabase androidDB;
    int aDownloads;
    int mDownloads;

    AndroidDatabase aDB = new AndroidDatabase();

    private static XMLAndroid instance = null;

    public static XMLAndroid GetInstance()
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
        lvlCreator = LevelCreator.GetInstance();
    }
    
    public void DownloadLevel(string fileName)
    {
        // Download the selected XML
        aDownloads = 0;
        mDownloads = 0;
        AWS.AWSDownload(fileName, AWS.levelFolderName);
        StartCoroutine(DownloadingXML(fileName));
    }

    IEnumerator DownloadingXML(string fileName)
    {
        while (AWS.downloadingStuff)
        {
            Debug.Log("Downloading XML ~");
#if UNITY_ANDROID
    string aeDir = Application.persistentDataPath;
#else
            string aeDir = Application.dataPath;
#endif
            if (AWS.downloadedStuff && File.Exists(aeDir + "/Serialization/XMLa/" + AWS.levelFolderName + "/" + fileName))
            {
                Debug.Log("Finished downloading XML");
                XmlSerializer serilizer = new XmlSerializer(typeof(AndroidDatabase));
                FileStream stream = new FileStream(aeDir + "/Serialization/XMLa/" + AWS.levelFolderName + "/" + fileName, FileMode.Open);
                androidDB = serilizer.Deserialize(stream) as AndroidDatabase;
                stream.Close();
                AWS.downloadingStuff = false;
                AWS.downloadedStuff = false;
                DownloadAsset();
                yield break;
            }
            yield return null;
        }
    }

    void DownloadAsset()
    {
        if (aDownloads < androidDB.objectBundleNames.Count)
        {
            //Debug.Log("BundleName : " + androidDB.objectBundleNames[aDownloads]);
            AWS.AWSDownload(androidDB.objectBundleNames[aDownloads], AWS.assetFolderName);
            StartCoroutine(DownloadingAssets(androidDB.objectBundleNames[aDownloads], true));
        }
        else if (mDownloads < androidDB.objectBundleNames.Count)
        {
            //Download manifest
            AWS.AWSDownload(androidDB.objectBundleNames[mDownloads] + ".manifest", AWS.assetFolderName);
            StartCoroutine(DownloadingAssets(androidDB.objectBundleNames[mDownloads] + ".manifest", false));
        }
        else
        {
            //Finished downloading everything
            lvlCreator.androidDatabase = androidDB;
            lvlCreator.LoadObjects();
            lvlCreator.lLoad = 0;
        }
    }

    IEnumerator DownloadingAssets(string fileName, bool bDownload)
    {
        while (AWS.downloadingStuff)
        {
            Debug.Log("Downloading Asset~");
#if UNITY_ANDROID
    string aeDir = Application.persistentDataPath;
#else
            string aeDir = Application.dataPath;
#endif
            if (AWS.downloadedStuff && File.Exists(aeDir + "/Serialization/XMLa/" + AWS.assetFolderName + "/" + fileName))
            {
                Debug.Log("Finished downloading Asset");
                if (bDownload)
                {
                    aDownloads++;
                }
                else
                {
                    mDownloads++;
                }
                AWS.downloadingStuff = false;
                AWS.downloadedStuff = false;
                DownloadAsset();
                yield break;
            }
            yield return null;
        }
    }

}


