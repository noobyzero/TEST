using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AndroidDatabase
{
    public List<AndroidNode> aList = new List<AndroidNode>();
    public Vector3 cameraPosition;
    public Vector3 objectScale;
    public List<string> objectBundleNames = new List<string>();
}

public class AndroidNode
{
    public List<Vector3> objectPositions = new List<Vector3>();
    public List<Vector3> objectRotations = new List<Vector3>();
    public List<string> objectIDs = new List<string>();
}

