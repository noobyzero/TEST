  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelCreator : MonoBehaviour {

    [Header("LevelCreator Reference")]
    [Tooltip("The prefab for the \"FLOORCOLLISION\"")]
    public GameObject floorColliderPrefab;
    [Tooltip("The reference for the \"CAMERAOBJECT\". *This is the camera/player. It will spawn the camera/player when the level loads")]
    public GameObject CameraObject;

    [Header("LevelCreator Values")]
    [Tooltip("The height the camera/player spawns at")]
    public float cameraHeight = 1.0f;
    [Tooltip("The height of the collider of the floor")]
    public float floorHeight = 0.1f;
    
    [System.NonSerialized]
    public int lLoad;

    [System.NonSerialized]
    public List<GameObject> loadedObjects = new List<GameObject>();

    AssetBundle assetBundle;
    AssetBundleRequest assetRequest;

    AWSscript AWS;

    public AndroidDatabase androidDatabase;

    private static LevelCreator instance = null;

    public static LevelCreator GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {

        AWS = AWSscript.GetInstance();
	}
    
    public void LoadObjects()
    {
#if UNITY_ANDROID
    string aeDir = Application.persistentDataPath;
#else
        string aeDir = Application.dataPath;
#endif
        if (androidDatabase != null)
        {
            if (lLoad < androidDatabase.objectBundleNames.Count)
            {
                LoadAssetBundle(aeDir + "/Serialization/XMLa/" + AWS.assetFolderName + "/" + androidDatabase.objectBundleNames[lLoad]);
            }
            else
            {
                //Finished loading objects
                AndroidLevel.GetInstance().UILoad.GetComponent<CanvasGroup>().alpha = 0;
                LoadLevel();
            }
        }
    }

    void LoadAssetBundle(string abPath)
    {
        assetBundle = AssetBundle.LoadFromFile(abPath);
        assetRequest = assetBundle.LoadAllAssetsAsync(typeof(GameObject));
        foreach (Object o in assetRequest.allAssets)
        {
            loadedObjects.Add(o as GameObject);
        }
        assetBundle.Unload(false);
        lLoad++;
        LoadObjects();
    }

    //Call loadLevel after selecting XML
    public void LoadLevel()
    {
        if (loadedObjects != null)
        {
            SpawnFloorCollision();

            CameraObject.transform.position = new Vector3(androidDatabase.cameraPosition.x, cameraHeight, androidDatabase.cameraPosition.z);

            foreach (AndroidNode lNode in androidDatabase.aList)
            {
                for (int i = 0; i < lNode.objectIDs.Count; i++)
                {
                    GameObject gObj = Instantiate(GetObject(lNode.objectIDs[i]), lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
                    gObj.transform.GetChild(0).transform.eulerAngles = lNode.objectRotations[i];
                    gObj.transform.localScale = androidDatabase.objectScale;
                    gObj.name = lNode.objectIDs[i];
                }
            }
        }
    }

    void SpawnFloorCollision()
    {
        List<float> xVal = new List<float>();
        List<float> zVal = new List<float>();
        foreach (AndroidNode an in androidDatabase.aList)
        {
            xVal.Add(an.objectPositions[0].x);
            zVal.Add(an.objectPositions[0].z);
        }
        Vector3 s = new Vector3(xVal.Max() - xVal.Min(), 0, zVal.Max() - zVal.Min());
        GameObject fObj = Instantiate(floorColliderPrefab, new Vector3(xVal.Min(), 0, zVal.Min()) + s/2, Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
        fObj.GetComponent<BoxCollider>().size = s + new Vector3(androidDatabase.objectScale.x, floorHeight, androidDatabase.objectScale.z);
        fObj.GetComponent<BoxCollider>().center = new Vector3(0 ,0 , 0.5f * androidDatabase.objectScale.z);
    }

    GameObject GetObject(string n)
    {
        foreach (GameObject go in loadedObjects)
        {
            if (go.name == n)
            {
                return go;
            }
        }
        return null;
    }
   

}
