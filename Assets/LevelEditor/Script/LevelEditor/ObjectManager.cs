  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectManager : MonoBehaviour {

    [Header("ObjectManager References")]
    [Tooltip("The reference to the \"UIManager\"")]
    public UIManager uim;
    [Tooltip("The prefab for each \"TAB\"")]
    public GameObject tabPrefab;
    [Tooltip("The prefab for each \"TABBUTTON\"")]
    public GameObject tabButtonPrefab;

    [Header("OBJECT MANAGER")]
    [Tooltip("The number of tabs. *The catagories of objects")]
    public tabItems[] tabNumber;

    [System.Serializable]
    public class tabItems
    {
        [Tooltip("The name of the tab")]
        public string tabName;
        [Tooltip("The number of items in the current tab")]
        public itemObject[] tabItem;
    }

    [System.Serializable]
    public class itemObject
    {
        [Tooltip("The type of object. *Stackable/Non-Stackable")]
        public ObjectType objectType;
        [Tooltip("The prefab for the \"OBJECT\"")]
        public GameObject objectPrefab;
        [Tooltip("The image for the button of the object")]
        public Sprite objectImage;
        [Tooltip("The \'AssetBundles\' name of the prefab")]
        public string objectBundleName;
    }

    public enum ObjectType
    {
        NonStackable = 1,
        Stackable = 2,
		BaseOnly = 3,
		MarkingLighting = 4
    }


    LevelManager lm;


    private static ObjectManager instance = null;

    public static ObjectManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    void Start () {

        lm = LevelManager.GetInstance();
        uim = UIManager.GetInstance();
        CreateTabs();
        PopulateTabs();
	}

    void CreateTabs()
    {
        for (int i = tabNumber.Length -1; i >= 0; i--)
        {
            GameObject tab = Instantiate(tabPrefab, uim.UIall.transform) as GameObject;
            tab.name = "Tab " + (i+1);
            float w = (Screen.width * 0.95f) / tabNumber.Length;
            tab.transform.FindChild("TabPanel").GetComponent<RectTransform>().sizeDelta = new Vector2(w, Screen.height * 0.037f);
            tab.transform.FindChild("TabPanel").GetComponent<RectTransform>().position = new Vector3((i * w), Screen.height * 0.2f, 0);
            tab.transform.FindChild("TabPanel").transform.GetChild(0).GetComponent<Text>().text = "" + tabNumber[i].tabName;
            tab.transform.FindChild("MainPanel").GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height * 0.2f);
        }
    }

    void PopulateTabs()
    {
        for (int i = 0; i < tabNumber.Length; i++)
        {
            for (int j = 0; j < tabNumber[i].tabItem.Length; j++)
            {
                GameObject tObject = Instantiate(tabButtonPrefab, uim.UIall.transform.FindChild("Tab " + (i+1)).FindChild("MainPanel").GetChild(0)) as GameObject;
                tObject.GetComponent<Image>().sprite = tabNumber[i].tabItem[j].objectImage;
                tObject.name = tabNumber[i].tabItem[j].objectPrefab.name;
                tObject.GetComponent<Button>().onClick.AddListener(() => { GetSelectedObject(tObject.name); });
            }
        }
    }

    public void GetSelectedObject(string n)
    {
        uim.Status.text = "Editing ..";
        for (int i = 0; i < tabNumber.Length; i++)
        {
            for (int j = 0; j < tabNumber[i].tabItem.Length; j++)
            {
                if (tabNumber[i].tabItem[j].objectPrefab.name == n)
                {
					lm.selectedObj.LObject = tabNumber[i].tabItem[j].objectPrefab;
                    lm.selectedObj.LObjectType = (int)tabNumber[i].tabItem[j].objectType; 
					lm.selectedObj.bundleName = tabNumber[i].tabItem[j].objectBundleName;
					return;					
                }
				/*if (tabNumber[i].tabItem[j].objectPrefab.name != n)
				{
					lm.selectedObj.LObject = tabNumber[i].tabItem[j].objectPrefab;
					lm.selectedObj.LObjectType = (int)tabNumber[i].tabItem[j].objectType;
					lm.selectedObj.bundleName = tabNumber[i].tabItem[j].objectBundleName;
					return;
				}*/

			}
        }
    }
    
    public LevelObject GetLevelObject(string n)
    {
        for (int i = 0; i < tabNumber.Length; i++)
        {
            for (int j = 0; j < tabNumber[i].tabItem.Length; j++)
            {
                if (tabNumber[i].tabItem[j].objectPrefab.name == n)
                {
                    LevelObject obj = new LevelObject();
                    obj.LObject = tabNumber[i].tabItem[j].objectPrefab;
                    obj.LObjectType = (int)tabNumber[i].tabItem[j].objectType;
                    obj.bundleName = tabNumber[i].tabItem[j].objectBundleName;
                    return obj;
                }
            }
        }
        return null;
    }
}

public class LevelObject
{
    public GameObject LObject = null;
    public int LObjectType = 0;
    public string bundleName;
}