  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    [Header("UI References")]
    [Tooltip("Main canvas for all UI elements")]
    public Canvas UIall;
    [Tooltip("UI canvas for \"SAVE\" UI")]
    public Canvas UIsave;
    [Tooltip("UI canvas for \"LOAD\" UI")]
    public Canvas UIload;
    [Tooltip("UI canvas for \"HELP\" UI")]
    public Canvas UIHelp;

    [Header("Other References")]
    [Tooltip("A Text for the \"STATUS BAR\"")]
    public Text Status;

    [Tooltip("The sprite for the \"MOUSE ICON\"")]
    public Sprite mouseIcon;
    [Tooltip("The sprite for the \"PANNING ICON\"")]
    public Sprite panIcon;

    [System.NonSerialized]
    public bool mouseOverUI;

    [System.NonSerialized]
    public bool holdingObject = false;

    CameraController cc;

    private static UIManager instance = null;

    public static UIManager GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
    }

    public void MouseEnter()
    {
        mouseOverUI = true;
    }

    public void MouseExit()
    {
        mouseOverUI = false;
    }

	void Start ()
    {
        cc = CameraController.GetInstance();   
	}
	
	void Update () {
		
        if (holdingObject)
        {
            UIall.transform.FindChild("MouseIcon").GetComponent<Image>().sprite = panIcon;
        }
        else
        {
            UIall.transform.FindChild("MouseIcon").GetComponent<Image>().sprite = mouseIcon;
        }
	}

    public void OpenSaveScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            UIsave.transform.GetChild(0).GetChild(0).FindChild("SaveInput").GetComponent<InputField>().text = "";
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = false;
            StartCoroutine(FadeCanvas(UIsave, true));
            UIsave.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = false;
        }
    }

    public void CancelSaveScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            StartCoroutine(FadeCanvas(UIsave, false));
            UIsave.GetComponent<CanvasGroup>().blocksRaycasts = false;
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = true;
        }
    }

    public void OpenLoadScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = false;
            StartCoroutine(FadeCanvas(UIload, true));
            UIload.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = false;
        }
    }

    public void CancelLoadScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            StartCoroutine(FadeCanvas(UIload, false));
            UIload.GetComponent<CanvasGroup>().blocksRaycasts = false;
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = true;
        }
    }

    public void OpenHelpScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = false;
            StartCoroutine(FadeCanvas(UIHelp, true));
            UIHelp.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = false;
        }
    }

    public void CancelHelpScreen()
    {
        if (mouseOverUI && !holdingObject)
        {
            StartCoroutine(FadeCanvas(UIHelp, false));
            UIHelp.GetComponent<CanvasGroup>().blocksRaycasts = false;
            UIall.GetComponent<CanvasGroup>().blocksRaycasts = true;
            cc.cAllowCameraControls = true;
        }
    }

    IEnumerator FadeCanvas(Canvas c, bool r)
    {
        if (!r)
        {
            while (c.GetComponent<CanvasGroup>().alpha > 0)
            {
                c.GetComponent<CanvasGroup>().alpha -= Time.deltaTime *2;
                yield return null;
            }
        }
        else
        {
            while (c.GetComponent<CanvasGroup>().alpha < 1)
            {
                c.GetComponent<CanvasGroup>().alpha += Time.deltaTime *2;
                yield return null;
            }
        }
        yield return null;
    }

}
