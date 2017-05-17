using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    public float nPosX;
    public float nPosZ;
    public bool bFree = true;
    public GameObject nVisualGrid;
    public List<LevelObject> nObjects = new List<LevelObject>();

}
