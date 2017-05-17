// DONE BY \\
//  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class LevelManager : MonoBehaviour
{

	GridManager gm;
	UIManager uim;
	ObjectManager objm;
	XMLManager xmlm;
	CheckManager cm;

	LevelObject hObject;

	[Header("LevelManager References")]
	[Tooltip("The prefab for each \"LEVELBUTTON\"")]
	public GameObject levelButtonPrefab;
	[Header("LevelManager Values")]
	[Tooltip("The rotation angle in which the object will rotate")]
	public int rAngle = 90;

	public LevelObject selectedObj = new LevelObject();
	[System.NonSerialized]
	public bool bHoldingObject = false;

	string levelSelected;

	int userNumber;
	int sameLaneNumber;
	string runwayNumber;

	GameObject numberPrefab;
	GameObject numberCanvas;
	GameObject numberCanvasTwo;
	public GameObject SetNumberGO;
	public GameObject NumberOnlyGO;
	public GameObject NumberLCRGO;
	public GameObject userTextboxGO;
	public InputField userTextbox;
	public InputField userTextboxLCR;
	public GameObject userTextboxLCRGO;
	private Text lane_text;

	bool adjacentCheck = false;
	bool runwayCheck = false;
	bool rangeCheck = false;

	private static LevelManager instance = null;

	public static LevelManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		instance = this;
	}

	void Start()
	{

		gm = GridManager.GetInstance();
		uim = UIManager.GetInstance();
		objm = ObjectManager.GetInstance();
		xmlm = XMLManager.GetInstance();
		cm = CheckManager.GetInstance();
		userTextboxGO.SetActive(false);
		userTextboxLCRGO.SetActive(false);
		NumberOnlyGO.SetActive(false);
		NumberLCRGO.SetActive(false);
	}

	void Update()
	{
		SpawnAndHoverObject();
		InputHandler();
	}

	void SpawnAndHoverObject()
	{
		if (selectedObj.LObject != null)
		{
			if (gm.isHovering && !uim.mouseOverUI && gm.currentNode.bFree)
			{
				Vector3 pos = new Vector3(gm.currentNode.nPosX, 0, gm.currentNode.nPosZ);
				if (gm.currentNode.nObjects.Count > 0)
				{
					foreach (LevelObject go in gm.currentNode.nObjects)
					{
						pos.y += go.LObject.transform.GetComponentInChildren<MeshRenderer>().bounds.size.y;

					}
				}

				if (!bHoldingObject)
				{
					//Spawn object if not yet in scene
					hObject = new LevelObject();
					hObject.LObject = Instantiate(selectedObj.LObject, pos, Quaternion.identity) as GameObject;
					hObject.LObject.transform.parent = transform.FindChild("LevelObjects").transform;
					hObject.LObject.transform.localScale = gm.currentNode.nVisualGrid.transform.localScale;
					hObject.LObject.name = selectedObj.LObject.name;
					hObject.LObjectType = selectedObj.LObjectType;
					hObject.bundleName = selectedObj.bundleName;
					bHoldingObject = true;
					uim.holdingObject = true;
				}
				else
				{
					//Update position of object after spawning
					if (gm.currentNode != gm.prevNode)
					{
						hObject.LObject.transform.position = pos;
					}
				}
			}
		}
	}

	void InputHandler()
	{
		//Clones objects on button down
		if (Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonDown(0) && bHoldingObject)
		{
			PlaceObject(true);
		}
		//Delete objects on button down
		else if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0) && !bHoldingObject || Input.GetKeyDown(KeyCode.Mouse4))
		{
			DeleteObject();
		}
		//Rotate object on button down
		else if (Input.GetMouseButtonDown(1) && bHoldingObject)
		{
			RotateObject();
		}
		//Get object that is placed on click
		else if (Input.GetMouseButtonDown(1) && !bHoldingObject && !Input.GetKey(KeyCode.LeftShift))
		{
			GetObject();
		}
		//Places object once on click
		else if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift) && bHoldingObject)
		{
			PlaceObject(false);
		}
		//Cancels selected object on key F
		else if (Input.GetKey(KeyCode.F) || Input.GetKeyDown(KeyCode.Mouse3) && bHoldingObject)
		{
			CancelSelect();
		}
	}


	public void CloneSucceed() 
	{
		gm.currentNode.nObjects.Add(hObject);
		bHoldingObject = false;
		uim.holdingObject = false;
	}

	public void PlaceSucceed() 
	{
		gm.currentNode.nObjects.Add(hObject);
		bHoldingObject = false;
		uim.holdingObject = false;
		hObject = new LevelObject();
		selectedObj = new LevelObject();
	}

	public void CancelSelect() //destroys hovering object and resets object selection
	{
		Destroy(hObject.LObject);
		selectedObj.LObject = null;
		selectedObj.LObjectType = 0;
		bHoldingObject = false;
		uim.holdingObject = false;
		hObject = new LevelObject();
		selectedObj = new LevelObject();
	}

	void PlaceObject(bool cloneOrPlace)
	{
		if (cloneOrPlace) //if PlaceObject is true
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
				cm.PlaceObjectClone();
			}
		}
		else if (!cloneOrPlace) //if not cloning object
		{
			if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null && gm.currentNode.bFree) //cursor on grid not on UI, level object selected, camera not on selected grid 
			{
				cm.PlaceObjectSingle();
			}
		}
	}

	void DeleteObject()
	{
		if (gm.isHovering && !uim.mouseOverUI)
		{
			if (gm.currentNode.nObjects.Count > 0)
			{
				Destroy(gm.currentNode.nObjects.Last().LObject);
				gm.currentNode.nObjects.RemoveAt(gm.currentNode.nObjects.Count - 1);
			}
		}
	}

	void RotateObject()
	{
		if (gm.isHovering && !uim.mouseOverUI && hObject.LObject != null)
		{
			hObject.LObject.transform.GetChild(0).transform.eulerAngles += new Vector3(0, rAngle, 0);
		}
	}

	void GetObject()
	{
		if (gm.isHovering && !uim.mouseOverUI && gm.selectedNode != null)
		{
			if (gm.selectedNode == gm.currentNode && gm.selectedNode.nObjects.Count > 0)
			{
				hObject = new LevelObject();
				hObject = gm.selectedNode.nObjects.Last();
				gm.selectedNode.nObjects.RemoveAt(gm.selectedNode.nObjects.Count - 1);
				bHoldingObject = true;
				uim.holdingObject = true;
				selectedObj = hObject;
			}
		}
	}

	public void ShowSetButtons()
	{
		SetNumberGO.SetActive(false);
		userTextboxGO.SetActive(false);
		NumberOnlyGO.SetActive(true);
		NumberLCRGO.SetActive(true);
	}

	public void GetUserInputNum() //get user input from Set Runway Number (Number Only) input textfield, check for range and same lane number
	{
		NumberOnlyGO.SetActive(false);
		NumberLCRGO.SetActive(false);
		userTextboxGO.SetActive(true);
		if (Input.GetKey(KeyCode.Return) && userTextbox.text != "") //if user press Return(Enter) key and the inputfield is not left empty
		{
			runwayNumber = userTextbox.text;
			int.TryParse(runwayNumber, out userNumber); //read the string/text and acquire numerical value
			SetNumberGO.SetActive(true);
			userTextboxGO.SetActive(false);
			if (userNumber > 0 && userNumber < 37)
			{
				if (gm.selectedNode.nObjects.Last().LObject.name == "RW_RunwayNumber")
				{
					numberCanvas = gm.selectedNode.nObjects.Last().LObject.transform.Find("UICanvas").gameObject;
					if (numberCanvas != null) //if UICanvas is found, raycast forward, right, back and left
					{
						Vector3 lastPos = new Vector3(gm.selectedNode.nPosX, gm.selectedNode.nObjects.Last().LObject.transform.position.y, gm.selectedNode.nPosZ);
						RaycastHit hit;
						if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
							|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
							|| Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
							|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
						{
							if (hit.collider)
							{
								numberCanvasTwo = hit.collider.gameObject.transform.Find("UICanvas").transform.Find("lane_text").gameObject;
								if (numberCanvasTwo != null)
								{ //if another RW_RunwayNumber is found on the same X or Z axis, check and compare their values
									int.TryParse(numberCanvasTwo.GetComponent<Text>().text, out sameLaneNumber);
									if (sameLaneNumber == 0)
									{
										lane_text = numberCanvas.GetComponentInChildren<Text>();
										lane_text.text = "" + userNumber;
									}
									else if (userNumber > sameLaneNumber)
									{
										if (userNumber - sameLaneNumber == 18)
										{
											lane_text = numberCanvas.GetComponentInChildren<Text>();
											lane_text.text = "" + userNumber;
										}
										else
										{
											print("The numbers must have a difference of 18. Opposite in a compass.");
										}
									}
									else if (userNumber < sameLaneNumber)
									{
										if (sameLaneNumber - userNumber == 18)
										{
											lane_text = numberCanvas.GetComponentInChildren<Text>();
											lane_text.text = "" + userNumber;
										}
										else
										{
											print("The numbers must have a difference of 18. Opposite in a compass");
										}
									}
									else
									{
										print("The numbers cannot be equal");
									}
								}
							}
							else
							{
								lane_text = numberCanvas.GetComponentInChildren<Text>();
								lane_text.text = "" + userNumber;
							}
						}
						else
						{
							lane_text = numberCanvas.GetComponentInChildren<Text>();
							lane_text.text = "" + userNumber;
						}
					}
					else
					{
						print("Please select the grid with the runway number that you wish to edit.");
					}
				}
				else
				{
					print("Please select the grid with the runway number that you wish to edit.");
				}
			}
			else
			{
				print("Number is out of range.");
			}
		}
		else
		{
			SetNumberGO.SetActive(true);
			print("Please enter a runway number.");
		}
	}

	public void GetUserInputNumLCR() //get user input from Set Runway Number (Number & L/C/R) input textfield, check for range and same lane number
	{
		NumberOnlyGO.SetActive(false);
		NumberLCRGO.SetActive(false);
		userTextboxLCRGO.SetActive(true);
		if (Input.GetKey(KeyCode.Return) && userTextboxLCR.text != "") //if user press Return(Enter) key and the inputfield is not left empty
		{
			string[] runwayNumber = Regex.Split(userTextboxLCR.text, @"\D+"); //filter out non-numeric value
			foreach (string value in runwayNumber)
			{
				int userNumber;
				if (int.TryParse(value, out userNumber)) //read the string "value" and acquire numerical value
				{
					SetNumberGO.SetActive(true);
					userTextboxLCRGO.SetActive(false);
					if (userNumber > 0 && userNumber < 37)
					{
						if (gm.selectedNode.nObjects.Last().LObject.name == "RW_RunwayNumber")
						{
							numberCanvas = gm.selectedNode.nObjects.Last().LObject.transform.Find("UICanvas").gameObject;
							if (numberCanvas != null)
							{//if UICanvas is found, raycast forward, right, back and left
								Vector3 lastPos = new Vector3(gm.selectedNode.nPosX, gm.selectedNode.nObjects.Last().LObject.transform.position.y, gm.selectedNode.nPosZ);
								RaycastHit hit;
								if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
									|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
									|| Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber"))
									|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayNumber")))
								{
									if (hit.collider)
									{
										numberCanvasTwo = hit.collider.gameObject.transform.Find("UICanvas").transform.Find("lane_text").gameObject;
										if (numberCanvasTwo != null)
										{//if another RW_RunwayNumber is found on the same X or Z axis, check and compare their values
											string[] runwayNumberTwo = Regex.Split(numberCanvasTwo.GetComponent<Text>().text, @"\D+");
											foreach (string valueTwo in runwayNumberTwo)
											{
												int sameLaneNumber;
												int.TryParse(valueTwo, out sameLaneNumber);
												if (sameLaneNumber == 0)
												{
													lane_text = numberCanvas.GetComponentInChildren<Text>();
													lane_text.text = userTextboxLCR.text;
												}
												else if (userNumber > sameLaneNumber)
												{
													if (userNumber - sameLaneNumber == 18)
													{
														lane_text = numberCanvas.GetComponentInChildren<Text>();
														lane_text.text = userTextboxLCR.text;
													}
													else
													{
														print("The numbers must have a difference of 18. Opposite in a compass.");
													}
													break;
												}
												else if (userNumber < sameLaneNumber)
												{
													if (sameLaneNumber - userNumber == 18)
													{
														lane_text = numberCanvas.GetComponentInChildren<Text>();
														lane_text.text = userTextboxLCR.text;
													}
													else
													{
														print("The numbers must have a difference of 18. Opposite in a compass");
													}
													break;
												}
												else
												{
													print("The numbers cannot be equal");
													break;
												}
											}
										}
										else
										{
											print("There is something wrong with the opposite runway number");
										}
									}
									else
									{
										lane_text = numberCanvas.GetComponentInChildren<Text>();
										lane_text.text = userTextboxLCR.text;
									}
								}
								else
								{
									lane_text = numberCanvas.GetComponentInChildren<Text>();
									lane_text.text = userTextboxLCR.text;
								}
							}
							else
							{

								print("Please select the grid with the runway number that you wish to edit.");
							}
						}
						else
						{
							print("Please select the grid with the runway number that you wish to edit.");
						}
					}
					else
					{
						print("Number is out of range.");
					}
				}
				else
				{
					print("Please enter at least one number");
				}
			}
		}
		else
		{
			SetNumberGO.SetActive(true);
			print("Please enter a runway number.");
		}
	}

	//Button Functions 
	public void NewLevelButton()
	{
		if (uim.mouseOverUI && !bHoldingObject)
		{
			gm.ResetCameraObject();
			foreach (Node n in gm.myGrid)
			{
				if (n.nObjects.Count > 0)
				{
					foreach (LevelObject lo in n.nObjects)
					{
						Destroy(lo.LObject);
					}
					n.nObjects.Clear();
					uim.Status.text = "New Level";
				}
			}
		}
	}

	public void SaveLevelButton()
	{
		LevelDatabase levelDB = new LevelDatabase();
		string input = uim.UIsave.transform.GetChild(0).GetChild(0).FindChild("SaveInput").GetComponent<InputField>().text;
		if (input != "")
		{
			levelDB.cameraPosition = gm.cameraPlacementObject.transform.position;
			levelDB.objectScale = gm.myGrid[0, 0].nVisualGrid.transform.localScale;
			List<string> myBundle = new List<string>();
			foreach (Node n in gm.myGrid)
			{
				if (n.nObjects.Count > 0)
				{
					LevelNode levelNode = new LevelNode();
					levelNode.nodePositionX = n.nPosX;
					levelNode.nodePositionZ = n.nPosZ;
					for (int i = 0; i < n.nObjects.Count; i++)
					{
						levelNode.objectPositions.Add(n.nObjects[i].LObject.transform.position);
						levelNode.objectRotations.Add(n.nObjects[i].LObject.transform.GetChild(0).transform.eulerAngles);
						levelNode.objectTypes.Add(n.nObjects[i].LObjectType);
						levelNode.objectIDs.Add(n.nObjects[i].LObject.name);
						if (n.nObjects[i].LObject.name == "RW_RunwayNumber")
						{
							for (int j = 0; j < n.nObjects.Count; j++)
							{
								levelNode.numberStrings.Add(n.nObjects[i].LObject.transform.Find("UICanvas").transform.Find("lane_text").GetComponent<Text>().text);
							}	
						}
						//levelDB.objectBundleNames.Add(n.nObjects[i].bundleName);
						myBundle.Add(n.nObjects[i].bundleName);
					}
					levelDB.dbList.Add(levelNode);
				}
			}
			levelDB.objectBundleNames = myBundle.Distinct().ToList();
			xmlm.SaveLevel(input, levelDB);
			uim.Status.text = "Saved Level";

			if (uim.UIsave.transform.GetChild(0).GetChild(0).FindChild("UploadToggle").GetComponent<Toggle>().isOn)
			{
				AndroidDatabase androidDB = new AndroidDatabase();
				androidDB.cameraPosition = gm.cameraPlacementObject.transform.position;
				androidDB.objectScale = gm.myGrid[0, 0].nVisualGrid.transform.localScale;
				List<string> myBundleA = new List<string>();
				foreach (Node n in gm.myGrid)
				{
					if (n.nObjects.Count > 0)
					{
						AndroidNode androidNode = new AndroidNode();
						for (int i = 0; i < n.nObjects.Count; i++)
						{
							androidNode.objectPositions.Add(n.nObjects[i].LObject.transform.position);
							androidNode.objectRotations.Add(n.nObjects[i].LObject.transform.GetChild(0).transform.eulerAngles);
							androidNode.objectIDs.Add(n.nObjects[i].LObject.name);
							myBundleA.Add(n.nObjects[i].bundleName);
						}
						androidDB.aList.Add(androidNode);
					}
				}
				androidDB.objectBundleNames = myBundle.Distinct().ToList();
				xmlm.UploadLevel(input, androidDB);
				uim.Status.text = "Saved Level\nUploading ..";
			}
			uim.CancelSaveScreen();
		}

	}

	public void LoadLevelScreenButton()
	{
		levelSelected = null;
		if (Directory.Exists(Application.dataPath + "/Serialization/XML"))
		{
			string[] loadedXMLs = Directory.GetFiles(Application.dataPath + "/Serialization/XML/", "*.xml");
			//Debug.Log("Loaded Length : " + loadedXMLs.Length);
			Transform t = uim.UIload.transform.GetChild(0).GetChild(0).FindChild("LevelPanel").GetChild(0);
			if (t.childCount > 0)
			{
				foreach (Transform child in t.transform)
				{
					Destroy(child.gameObject);
				}
			}
			foreach (string f in loadedXMLs)
			{
				GameObject levelButton = Instantiate(levelButtonPrefab, t) as GameObject;
				levelButton.transform.GetChild(0).GetComponent<Text>().text = Path.GetFileName(f).Substring(0, Path.GetFileName(f).Length - 4);
				levelButton.name = Path.GetFileName(f);
				levelButton.GetComponent<Button>().onClick.AddListener(() => { SelectLevel(levelButton.name); });
			}

		}
	}


	public void SelectLevel(string n)
	{
		levelSelected = n;
	}

	public void LoadSelectedLevel()
	{
		if (levelSelected != null)
		{
			NewLevelButton();
			LevelDatabase levelData = xmlm.LoadLevel(levelSelected);
			gm.cameraPlacementObject.transform.position = levelData.cameraPosition;
			gm.pCam = gm.FindNodeFromPos(levelData.cameraPosition.x, levelData.cameraPosition.z);
			gm.pCam.bFree = false;

			foreach (LevelNode lNode in levelData.dbList)
			{
				for (int i = 0; i < lNode.objectIDs.Count; i++)
				{
					LevelObject lObj = new LevelObject();
					lObj.LObject = Instantiate(objm.GetLevelObject(lNode.objectIDs[i]).LObject, lNode.objectPositions[i], Quaternion.identity, transform.FindChild("LevelObjects")) as GameObject;
					lObj.LObject.transform.GetChild(0).transform.eulerAngles = lNode.objectRotations[i];
					lObj.LObject.transform.localScale = levelData.objectScale;
					lObj.LObject.name = lNode.objectIDs[i];
					if (lObj.LObject.name == "RW_RunwayNumber"){
						for (int j = 0; j < lNode.objectIDs.Count; j++)
						{
							lObj.LObject.transform.Find("UICanvas").transform.Find("lane_text").gameObject.GetComponent<Text>().text = lNode.numberStrings[j];
						}			
					}
					lObj.LObjectType = lNode.objectTypes[i];
					gm.FindNodeFromPos(lNode.nodePositionX, lNode.nodePositionZ).nObjects.Add(lObj);
				}
			}
			uim.CancelLoadScreen();
			uim.Status.text = "Loaded level";
		}
	}

	public void DeleteSelectedLevel()
	{
		if (levelSelected != null)
		{
			File.Delete(Application.dataPath + "/Serialization/XML/" + levelSelected);
			Destroy(uim.UIload.transform.GetChild(0).GetChild(0).FindChild("LevelPanel").GetChild(0).FindChild(levelSelected).gameObject);
			levelSelected = null;
			uim.Status.text = "Deleted level";
		}
	}

	void AddBundleName(List<string> myBundle, string b)
	{
		if (myBundle.Count == 0)
		{
			myBundle.Add(b);
		}
		else
		{
			foreach (string s in myBundle)
			{
				if (s != b)
				{
					myBundle.Add(b);
				}
			}
		}
	}

}
