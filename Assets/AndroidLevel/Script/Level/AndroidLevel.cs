  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AndroidLevel : MonoBehaviour {

    [Header("AndroidLevel References")]
    [Tooltip("The reference for the \"LEVEL\" canvas")]
    public Canvas UILevel;
    [Tooltip("The reference for the \"START\" canvas")]
    public Canvas UIStart;
    [Tooltip("The reference for the \"LOAD\" canvas")]
    public Canvas UILoad;
    [Tooltip("The prefab for each \"LEVELBUTTON\"")]
    public GameObject LevelButtonPrefab;

    AWSscript AWS;

    string[] loadedXMLs;

    LevelCreator lvlCreator;

    private static AndroidLevel instance = null;

    public static AndroidLevel GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {

        AWS = AWSscript.GetInstance();
        lvlCreator = LevelCreator.GetInstance();
	}

    public void StartButton()
    {
        UIStart.GetComponent<CanvasGroup>().alpha = 0;
        UIStart.GetComponent<CanvasGroup>().blocksRaycasts = false;
        UILoad.GetComponent<CanvasGroup>().alpha = 1;
        AWS.AWSListLevels();
        StartCoroutine(FetchXMLs());
    }

    void CreateLevelButtons()
    {
        for (int i = 0; i < loadedXMLs.Length; i++)
        {
            GameObject lvlButton = Instantiate(LevelButtonPrefab, UILevel.transform.GetChild(0).GetChild(0).FindChild("LevelPanel").GetChild(0).transform) as GameObject;
            lvlButton.name = loadedXMLs[i];
            lvlButton.transform.GetChild(0).GetComponent<Text>().text = loadedXMLs[i].Replace(".xml", ""); ;
            lvlButton.GetComponent<Button>().onClick.AddListener(() => { LoadSelectedLevel(lvlButton.name); });
        }
    }

    void LoadSelectedLevel(string XMLn)
    {
        UILevel.GetComponent<CanvasGroup>().alpha = 0;
        UILevel.GetComponent<CanvasGroup>().blocksRaycasts = false;
        XMLAndroid.GetInstance().DownloadLevel(XMLn);
        UILoad.GetComponent<CanvasGroup>().alpha = 1;
    }

    IEnumerator FetchXMLs()
    {
        while (AWS.fetchingList)
        {
            Debug.Log("Fetching ~");
            if (AWS.fetchedList)
            {
                Debug.Log("Finsihed Fetch");
                loadedXMLs = AWS.xmlList.ToArray();
                UILoad.GetComponent<CanvasGroup>().alpha = 0;
                UILevel.GetComponent<CanvasGroup>().alpha = 1;
                UILevel.GetComponent<CanvasGroup>().blocksRaycasts = true;
                CreateLevelButtons();
                AWS.fetchingList = false;
                AWS.fetchedList = false;
                yield break;
            }
            yield return null;
        }
    }

}
