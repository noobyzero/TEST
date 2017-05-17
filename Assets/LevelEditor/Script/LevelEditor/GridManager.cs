  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {

    [Header("Grid References")]
    [Tooltip("The prefab for each \"NODE\"")]
    public GameObject nPrefab;
    [Tooltip("The reference for the \"CAMERAOBJECT\". *This is to show where the camera/player will be spawned in gameplay. NOT the actual camera")]
    public GameObject cameraPlacementObject;

    [Header("Grid Values")]
    [Tooltip("The number of rows")]
    public int gRow;
    [Tooltip("The number of columns")]
    public int gColumn;

    public Node[,] myGrid;
    public Node currentNode;
    public Node prevNode;
    public Node selectedNode;

    public Node pCam;

    Vector3 mouseHitPos;
    Node dNode;

    [System.NonSerialized]
    public bool isHovering;

    private static GridManager instance = null;
    
    public static GridManager GetInstance()
    {
        return instance;
    }

    UIManager uim;

    void Awake()
    {
        instance = this;
        CreateGrid();
        CreateGridCollision();
    }

    void Start()
    {
        uim = UIManager.GetInstance();
        uim.Status.text = "Started New Level";

        ResetCameraObject();
    }

    void Update()
    {
        UpdateMousePosition();
        UpdateMouseHover();
        UpdateSelectedNode();
        CameraPlacement();

        // ========== DEBUGGING USE ========== \\

        //if (currentNode != null)
        //{
        //    Debug.Log("CurrentNode : [" + currentNode.nPosX + ", " + currentNode.nPosZ + "]\r\ndNode : [" + dNode.nPosX + ", " + dNode.nPosZ + "]");
        //Debug.Log("CurrentNode : [" + currentNode.nPosX + ", " + currentNode.nPosZ + "]");
        //if (currentNode.nPlacedObject == null)
        //    Debug.Log("Empty");
        //else if (currentNode.nPlacedObject != null)
        //    Debug.Log("Has object");
        //Debug.Log("\r\nObject" + currentNode.nPlacedObject == null ? "Empty" : "Has Object");
        //}
        //if (prevNode != null)
        //    Debug.Log("PrevNode : [" + prevNode.nPosX + ", " + prevNode.nPosZ + "]");
        //if (selectedNode != null)
        //{
        //    Debug.Log("SelectedNode : [" + selectedNode.nPosX + ", " + selectedNode.nPosZ + "]");
        //    if (selectedNode.nPlacedObject == null)
        //        Debug.Log("SelectNodeObject : is null");
        //    else if (selectedNode.nPlacedObject != null)
        //        Debug.Log("SelectNodeObject : not null");
        //}

        // ========== DEBUGGING USE ========== \\

    }

    public void ResetCameraObject()
    {
        foreach (Node n in myGrid)
        {
            n.bFree = true;
        }

        pCam = myGrid[gRow/2, gColumn/2];
        pCam.bFree = false;
        cameraPlacementObject.transform.position = new Vector3(pCam.nPosX, 0, pCam.nPosZ);
    }

    void CameraPlacement()
    {
        if (Input.GetKey(KeyCode.C) && selectedNode != null)
        {
            if (selectedNode.nObjects.Count == 0)
            {
                pCam.bFree = true;
                selectedNode.bFree = false;
                cameraPlacementObject.transform.position = new Vector3(selectedNode.nPosX, 0, selectedNode.nPosZ);
                pCam = selectedNode;
            }
        }
    }

    void UpdateSelectedNode()
    {
        if (Input.GetMouseButton(0) && isHovering && !uim.mouseOverUI)
        {
            if (selectedNode != null)
            {
                //Check if there is a previously selected node and DO CHANGES HERE
                selectedNode.nVisualGrid.GetComponentInChildren<MeshRenderer>().material.color = new Color(1, 1, 1);
            }
            selectedNode = currentNode;
            //Edit the current selected node
            selectedNode.nVisualGrid.GetComponentInChildren<MeshRenderer>().material.color = new Color(255, 100, 0);
        }
        else if (Input.GetMouseButton(0) && !isHovering && selectedNode != null && !uim.mouseOverUI)
        {
            //Reset selected node to none if clicked outside graph
            selectedNode.nVisualGrid.GetComponentInChildren<MeshRenderer>().material.color = new Color(1, 1, 1);
            selectedNode = null;
        }
    }

    void UpdateMouseHover()
    {
        if (isHovering && currentNode == null && !uim.mouseOverUI)
        {
            currentNode = GetSelectedNode(mouseHitPos);
            dNode = currentNode;
        }
        else if (isHovering && !uim.mouseOverUI)
        {
            //When mouse is hovering
            currentNode = GetSelectedNode(mouseHitPos);
            if (currentNode != dNode)
            {
                prevNode = dNode;
                if (prevNode != selectedNode)
                {
                    //DO CHANGES WITH PREVIOUS NODE HERE
                    prevNode.nVisualGrid.GetComponentInChildren<MeshRenderer>().material.color = new Color(1, 1, 1);
                }
                dNode = currentNode;
            }
            else if (currentNode != selectedNode)
            {
				//DO CHANGES WITH CURRENT NODE HERE
			}
        }
        else if (currentNode != selectedNode)
        {
            //When mouse is not hovering
            //DO CHANGES TO LAST CURRENT NODE HERE
            currentNode.nVisualGrid.GetComponentInChildren<MeshRenderer>().material.color = new Color(1, 1, 1);
        }
    }

    void UpdateMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
        {
            isHovering = true; //true when cursor pointing at grid
			
            mouseHitPos = hit.point;
        }
        else
            isHovering = false; //false when cursor not pointing at grid
		
	}

    void CreateGrid()
    {
        myGrid = new Node[gRow, gColumn];
        
        for (int x = 0; x < gRow; x++)
        {
            for (int z = 0; z < gColumn; z++)
            {
                float posX = x * nPrefab.transform.localScale.x;
                float posZ = z * nPrefab.transform.localScale.z;

                GameObject gPrefab = Instantiate(nPrefab, new Vector3(posX, 0, posZ), Quaternion.identity) as GameObject;
                gPrefab.transform.parent = transform.FindChild("Grid").transform;

                Node node = new Node();
                node.nVisualGrid = gPrefab;
                node.nPosX = posX;
                node.nPosZ = posZ;
                myGrid[x, z] = node;
                
            }
        }
    }

    void CreateGridCollision()
    {
        GameObject gCollider = new GameObject("GridCollider");
        gCollider.layer = 8;
        gCollider.transform.parent = transform.FindChild("Grid").transform;
        gCollider.AddComponent<BoxCollider>();
        gCollider.GetComponent<BoxCollider>().size = new Vector3(gRow * nPrefab.transform.localScale.x, 0, gColumn * nPrefab.transform.localScale.z);
        gCollider.GetComponent<BoxCollider>().center = new Vector3((float)gRow/2 * nPrefab.transform.localScale.x - 0.5f * nPrefab.transform.localScale.x, 0, (float)gColumn/2 * nPrefab.transform.localScale.z);
    }

    public Node GetSelectedNode(Vector3 mouseHit)
    {
        int x = Mathf.RoundToInt(mouseHit.x / nPrefab.transform.localScale.x);
        int z = Mathf.RoundToInt((mouseHit.z / nPrefab.transform.localScale.z) - 0.5f);
		
        return myGrid[x, z];
    }

    public Vector3 GetCenterPosition()
    {
        return new Vector3((gRow * nPrefab.transform.localScale.x) /2, 0, (gColumn * nPrefab.transform.localScale.z) /2);
    }
    
    public Node FindNodeFromPos(float x, float z)
    {
        foreach (Node n in myGrid)
        {
            if (n.nPosX == x && n.nPosZ == z)
            {
                return n;
            }
        }
        return null;
    }

}
