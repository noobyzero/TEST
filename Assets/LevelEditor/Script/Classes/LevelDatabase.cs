using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelDatabase
{
    public List<LevelNode> dbList = new List<LevelNode>();
    public Vector3 cameraPosition;
    public Vector3 objectScale;
    public List<string> objectBundleNames = new List<string>();
}

public class LevelNode
{
    public float nodePositionX;
    public float nodePositionZ;
    public List<Vector3> objectPositions = new List<Vector3>();
    public List<Vector3> objectRotations = new List<Vector3>();
    public List<int> objectTypes = new List<int>();
    public List<string> objectIDs = new List<string>();
	public List<string> numberStrings = new List<string>();
}
