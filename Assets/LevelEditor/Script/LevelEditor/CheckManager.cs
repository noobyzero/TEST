using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class CheckManager : MonoBehaviour
{

	GridManager gm;
	UIManager uim;
	ObjectManager objm;
	XMLManager xmlm;
	LevelManager lvlm;

	bool adjacentCheck = false;
	bool runwayCheck = false;
	bool rangeCheck = false;

	private static CheckManager instance = null;

	public static CheckManager GetInstance()
	{
		return instance;
	}

	void Awake()
	{
		instance = this;
	}

	// Use this for initialization
	void Start()
	{
		gm = GridManager.GetInstance();
		uim = UIManager.GetInstance();
		objm = ObjectManager.GetInstance();
		xmlm = XMLManager.GetInstance();
		lvlm = LevelManager.GetInstance();
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	void CheckAdjacent() //check the adjacent objects when trying to place a specified object
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ);
		Collider[] hitColliders = Physics.OverlapSphere(lastPos, 1); //cast a sphere with radius of 1 grid.
		foreach (Collider collided in hitColliders)
		{
			if (collided.gameObject.transform.position.x == lastPos.x || collided.gameObject.transform.position.z == lastPos.z) //check for same row or column, ignoring diagonal
			{
				if (lvlm.selectedObj.LObject.name == "RW_BlastPad" && collided.gameObject.tag == "taxiway") //if blastpad was selected and taxiway is around
				{
					adjacentCheck = false;
					print("You cannot place that near a taxiway");
					break;
				}
				else if (lvlm.selectedObj.LObject.tag == "taxiway" && collided.gameObject.name == "RW_BlastPad") //if taxyiway is selected and blastpad is around
				{
					adjacentCheck = false;
					print("You cannot place that near a blastpad");
					break;
				}
				else if (lvlm.selectedObj.LObject.tag == "runway" && collided.gameObject.name == "Roadway") //if runway is selected and roadway is around
				{
					adjacentCheck = false;
					print("You cannot place that near a vehicle roadway");
					break;
				}
				else if (lvlm.selectedObj.LObject.name == "Roadway" && collided.gameObject.tag == "runway") //if roadway is selected and runway is around
				{
					adjacentCheck = false;
					print("You cannot place that near a runway");
					break;
				}
				else if (lvlm.selectedObj.LObject.name == "TW_TaxiwayMarking") //if taxiway marking is selected, check for another taxiway marking and excludes itself
				{
					if ((collided.gameObject.transform.position.x != lastPos.x || collided.gameObject.transform.position.z != lastPos.z) && collided.gameObject.name == "TW_TaxiwayMarking")
					{
						adjacentCheck = true;
						break;
					}
					else
					{
						adjacentCheck = false;
						print("Connect the taxiway marking from a taxiway first");
					}
				}
				else if (lvlm.selectedObj.LObject.name == "HoldingShortLine") //if holding short line is selected and check for taxiway
				{
					if (collided.gameObject.tag == "taxiway")
					{
						adjacentCheck = true;
						break;
					}
					else
					{
						adjacentCheck = false;
					}
				}
				else
				{
					adjacentCheck = true;
				}
			}
		}
	}

	public void CheckRunway() //check the runway lane for correct order. This is super long because of all the double checking and in opposite order.
	{
		Vector3 lastPos = new Vector3(gm.currentNode.nPosX, lvlm.selectedObj.LObject.transform.position.y, gm.currentNode.nPosZ);
		RaycastHit hit;
		if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
			|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
		{
			if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker" && (hit.collider.gameObject.name == "RW_BlastPad" || hit.collider.gameObject.name == "RW_DisplacedThreshold"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				Debug.DrawRay(lastPos, transform.forward, Color.green, 5f);
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber" && hit.collider.gameObject.name == "RW_ThresholdMarker")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker" && hit.collider.gameObject.name == "RW_RunwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker" && hit.collider.gameObject.name == "RW_TouchdownZoneMarker")
			{ //selected object is AimingPointMarker and ray cast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber")) 
					|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast right and forward
				{
					if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker" && (hit.collider.gameObject.name == "RW_BlastPad" || hit.collider.gameObject.name == "RW_DisplacedThreshold"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber" && hit.collider.gameObject.name == "RW_ThresholdMarker")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker" && hit.collider.gameObject.name == "RW_RunwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker" && hit.collider.gameObject.name == "RW_TouchdownZoneMarker")
					{ //selected object is AimingPointMarker and ray cast hits TouchdownZoneMarker
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else if (Physics.Raycast(lastPos, transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))
				|| Physics.Raycast(lastPos, transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast right and forward
		{
			if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker" && (hit.collider.gameObject.name == "RW_BlastPad" || hit.collider.gameObject.name == "RW_DisplacedThreshold"))
			//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
			{
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber" && hit.collider.gameObject.name == "RW_ThresholdMarker")
			{ //selected object is RunwayNumber and raycast hits ThresholdMarker
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker" && hit.collider.gameObject.name == "RW_RunwayNumber")
			{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
				runwayCheck = true;
			}
			else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker" && hit.collider.gameObject.name == "RW_TouchdownZoneMarker")
			{ //selected object is AimingPointMarker and ray cast hits TouchdownZoneMarker
				runwayCheck = true;
			}
			else
			{
				runwayCheck = false;
				if (Physics.Raycast(lastPos, -transform.right, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber")) 
					|| Physics.Raycast(lastPos, -transform.forward, out hit, Mathf.Infinity, LayerMask.GetMask("runwayMarkings", "runwayNumber"))) //raycast left and back
				{
					if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker" && (hit.collider.gameObject.name == "RW_BlastPad" || hit.collider.gameObject.name == "RW_DisplacedThreshold"))
					//selected object is ThresholdMarker and raycast hits BlastPad/DisplacedThreshold 
					{
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber" && hit.collider.gameObject.name == "RW_ThresholdMarker")
					{ //selected object is RunwayNumber and raycast hits ThresholdMarker
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker" && hit.collider.gameObject.name == "RW_RunwayNumber")
					{ //selected object is TouchdownZoneMarker and raycast hits RunwayNumber
						runwayCheck = true;
					}
					else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker" && hit.collider.gameObject.name == "RW_TouchdownZoneMarker")
					{ //selected object is AimingPointMarker and ray cast hits TouchdownZoneMarker
						runwayCheck = true;
					}
				}
				else
				{
					runwayCheck = false;
				}
			}
		}
		else
		{
			runwayCheck = false;
		}
	}

	void CheckAdjacentTrue() //build if CheckAdjacent passes
	{
		if (adjacentCheck == true)
		{
			lvlm.PlaceSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckAdjacentTrueClone()
	{
		if (adjacentCheck == true)
		{
			lvlm.CloneSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckAdjacentFalse() //nothing built and cancel selection if CheckAdjacent fails
	{
		adjacentCheck = false;
		lvlm.CancelSelect();
	}

	void CheckRunwayTrue() //place runway marking if runwayCheck returns true, cancel selection if returns false
	{
		if (runwayCheck == true)
		{
			lvlm.PlaceSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	void CheckRunwayTrueClone() //clone and place runway marking if runwayCheck returns true, cancel selection if returns false
	{
		if (runwayCheck == true)
		{
			lvlm.CloneSucceed();
		}
		else
		{
			lvlm.CancelSelect();
		}
	}

	public void PlaceObjectClone() //clone and place object
	{
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1 && gm.currentNode.nObjects.Last().LObjectType != 4) //if the top object is stackable
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
					{
						print("This can only be built in apron area");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "sign" && gm.currentNode.nObjects.Last().LObject.tag != "grass") //selected sign but not building on grass plain
					{
						print("Signs can only be placed on grass plains");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.name == "Hangar" && gm.currentNode.nObjects.Last().LObject.tag != "apron")
					{
						print("This can only be placed on apron");
						lvlm.CancelSelect();
					}
					else
					{
						lvlm.CloneSucceed();
					}
				}
				else if (lvlm.selectedObj.LObjectType == 2) //selected object is stackable
				{
					lvlm.CloneSucceed();
				}
				else if (lvlm.selectedObj.LObjectType == 3) //selected object is base-only
				{
					print("That can only be used at ground level");
					lvlm.CancelSelect();
				}
				else if (lvlm.selectedObj.LObjectType == 4) //selected object is lighting/marking
				{
					if (lvlm.selectedObj.LObject.tag == "waylight" && (gm.currentNode.nObjects.Last().LObject.tag != "runway" && gm.currentNode.nObjects.Last().LObject.tag != "taxiway"))
					{ //selected taxiway/runway light but not building on taxiway/runway
						print("Taxiway/Runway lightings can only be placed on taxiway/runway");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "markingRunway") //if anything with markingRunway tag is selected
					{
						if (lvlm.selectedObj.LObject.name == "RW_BlastPad") //selected BlastPad
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckAdjacent();
								CheckAdjacentTrueClone();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_DisplacedThreshold") //selected DisplacedThreshhold
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								lvlm.CloneSucceed();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker") //selected ThresholdMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrueClone();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber") //selected RunwayNumber
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrueClone();
							}
							else
							{
								lvlm.CancelSelect();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker") //selected TouchdownZoneMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrueClone();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker") //selected AimingPointMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrueClone();
							}
						}
						else
						{
							lvlm.CancelSelect();
							print("You can only build that on the runway.");
						}
					}
					else if (lvlm.selectedObj.LObject.tag == "markingTaxiway")  //selected anything with markingTaxiway tag
					{
						if (gm.currentNode.nObjects.Last().LObject.tag == "taxiway")
						{
							if (lvlm.selectedObj.LObject.name == "HoldingShortLine")
							{
								CheckAdjacent();
								CheckAdjacentTrueClone();
							}
							else
							{
								lvlm.CloneSucceed();
							}
						}
						else if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
						{
							CheckAdjacent();
							CheckAdjacentTrueClone();
						}
						else
						{
							lvlm.CancelSelect();
							print("You cannot place the marking here");
						}
					}
					else
					{
						lvlm.CloneSucceed();
					}
				}
			}
			else if (gm.currentNode.nObjects.Last().LObjectType == 4) //if the top object is marking/lighting
			{
				if (lvlm.selectedObj.LObjectType != 4) //if the selected object is not lighting/marking
				{
					print("You cannot build this on lighting/marking");
					lvlm.CancelSelect();
				}
				else if (lvlm.selectedObj.LObject.tag == "waylight" && gm.currentNode.nObjects.Last().LObject.tag != "markingRunway")
				{ //selected waylight but not placing on runway marking
					print("Lightings cannot stack");
					lvlm.CancelSelect();
				}
				else if (lvlm.selectedObj.LObject.tag == "markingRunway" && gm.currentNode.nObjects.Last().LObject.tag != "waylight")
				{ //selected runway marking but not placing on waylight
					print("Markings cannot stack");
					lvlm.CancelSelect();
				}
				else
				{
					lvlm.CloneSucceed();
				}
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2 || lvlm.selectedObj.LObjectType == 4)
			{ //if object type 1, 2, or 3 is selected 
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else if (lvlm.selectedObj.LObjectType == 3) //if object type 3 is selected
			{
				if (lvlm.selectedObj.LObject.tag == "taxiway") //specifically taxiway
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}
				else if (lvlm.selectedObj.LObject.tag == "runway") //specifically runway
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}

				else if (lvlm.selectedObj.LObject.name == "Roadway") //specifically roadway
				{
					CheckAdjacent();
					CheckAdjacentTrueClone();
				}
				else
				{
					lvlm.CloneSucceed();
				}
			}
		}
	}

	public void PlaceObjectSingle() //place ONE object and no clone
	{
		if (gm.currentNode.nObjects.Count > 0) //if there is at least an object on the grid
		{
			if (gm.currentNode.nObjects.Last().LObjectType != 1 && gm.currentNode.nObjects.Last().LObjectType != 4) //if the top object is stackable
			{
				if (lvlm.selectedObj.LObjectType == 1) //selected object is non-stackable
				{
					if (lvlm.selectedObj.LObject.tag == "apronOnly" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected apronOnly objects but not building on apron
					{
						print("This can only be built in apron area");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "sign" && gm.currentNode.nObjects.Last().LObject.tag != "grass") //selected sign but not building on grass plain
					{
						print("Signs can only be placed on grass plains");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.name == "Hangar" && gm.currentNode.nObjects.Last().LObject.tag != "apron") //selected hangar but not building on apron tile
					{
						print("This can only be placed on apron");
						lvlm.CancelSelect();
					}
					else
					{
						lvlm.PlaceSucceed();
					}
				}
				if (lvlm.selectedObj.LObjectType == 2) //selected object is stackable
				{
					lvlm.PlaceSucceed();
				}
				if (lvlm.selectedObj.LObjectType == 3) //selected object is base-only
				{
					print("That can only be used at ground level");
					lvlm.CancelSelect();
				}
				if (lvlm.selectedObj.LObjectType == 4) //selected object is lighting/marking
				{
					if (lvlm.selectedObj.LObject.tag == "waylight" && (gm.currentNode.nObjects.Last().LObject.tag != "runway" && gm.currentNode.nObjects.Last().LObject.tag != "taxiway"))
					{ //selected taxiway/runway light but not building on taxiway/runway
						print("Taxiway/Runway lightings can only be placed on taxiway/runway");
						lvlm.CancelSelect();
					}
					else if (lvlm.selectedObj.LObject.tag == "markingRunway") //if anything with markingRunway tag is selected
					{
						if (lvlm.selectedObj.LObject.name == "RW_BlastPad") //selected BlastPad
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckAdjacent();
								CheckAdjacentTrue();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_DisplacedThreshold") //selected DisplacedThreshhold
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								lvlm.PlaceSucceed();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_ThresholdMarker") //selected ThresholdMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrue();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_RunwayNumber") //selected RunwayNumber
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrue();
							}
							else
							{
								lvlm.CancelSelect();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_TouchdownZoneMarker") //selected TouchdownZoneMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrue();
							}
						}
						else if (lvlm.selectedObj.LObject.name == "RW_AimingPointMarker") //selected AimingPointMarker
						{
							if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
							{
								CheckRunway();
								CheckRunwayTrue();
							}
						}
						else
						{
							lvlm.CancelSelect();
							print("You can only build that on the runway.");
						}
					}
					if (lvlm.selectedObj.LObject.tag == "markingTaxiway")  //selected anything with markingTaxiway tag
					{
						if (gm.currentNode.nObjects.Last().LObject.tag == "taxiway")
						{
							if (lvlm.selectedObj.LObject.name == "HoldingShortLine")
							{
								CheckAdjacent();
								CheckAdjacentTrue();
							}
							else
							{
								lvlm.PlaceSucceed();
							}
						}
						else if (gm.currentNode.nObjects.Last().LObject.tag == "runway")
						{
							CheckAdjacent();
							CheckAdjacentTrue();
						}
						else
						{
							lvlm.CancelSelect();
							print("You cannot place the marking here");
						}
					}
					else
					{
						lvlm.PlaceSucceed();
					}
				}
			}
			else if (gm.currentNode.nObjects.Last().LObjectType == 4) //if the top object is marking/lighting
			{
				if (lvlm.selectedObj.LObjectType != 4) //if the selected object is not lighting/marking
				{
					print("You cannot build this on lighting/marking");
					lvlm.CancelSelect();
				}
				else if (lvlm.selectedObj.LObject.tag == "waylight" && gm.currentNode.nObjects.Last().LObject.tag != "markingRunway")
				{ //selected waylight but not placing on runway marking
					print("Lightings cannot stack");
					lvlm.CancelSelect();
				}
				else if (lvlm.selectedObj.LObject.tag == "markingRunway" && gm.currentNode.nObjects.Last().LObject.tag != "waylight")
				{ //selected runway marking but not placing on waylight
					print("Markings cannot stack");
					lvlm.CancelSelect();
				}
				else
				{
					lvlm.PlaceSucceed();
				}
			}
		}
		else //if there is no object on the grid
		{
			if (lvlm.selectedObj.LObjectType == 1 || lvlm.selectedObj.LObjectType == 2 || lvlm.selectedObj.LObjectType == 4)
			{ //if object type 1, 2, or 3 is selected 
				print("Place a ground object first");
				lvlm.CancelSelect();
			}
			else if (lvlm.selectedObj.LObjectType == 3) //if object type 3 is selected
			{
				if (lvlm.selectedObj.LObject.tag == "taxiway") //specifically taxiway
				{
					CheckAdjacent();
					CheckAdjacentTrue();
				}
				else if (lvlm.selectedObj.LObject.tag == "runway") //specifically runway
				{
					CheckAdjacent();
					CheckAdjacentTrue();
				}

				else if (lvlm.selectedObj.LObject.name == "Roadway") //specifically roadway
				{
					CheckAdjacent();
					CheckAdjacentTrue();
				}
				else
				{
					CheckAdjacent();
					lvlm.PlaceSucceed();
				}
			}
		}
	}

}
