  // DONE BY \\
 //  ABRAHAM  \\
//     SZZ     \\

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [Header("Camera Values")]
    [Tooltip("Control the max zoom")]
    public float cMaxZoom = -60;
    [Tooltip("Control the min zoom")]
    public float cMinZoom = -3;
    float cZoomOffset = 1.0f;

    [Tooltip("Control the rotataion speed")]
    public float cRotationSpeed = 4;
    [Tooltip("Control the max yaw rotation")]
    public float cMaxRotationY = 80;
    [Tooltip("Control the min yaw rotation")]
    public float cMinRotationY = 10;
    float currentPitch = 0.0f;
    float currentYaw = 0.0f;

    [Tooltip("Control the max speed")]
    public float cMaxSpeed = 45;
    [Tooltip("Control the min speed")]
    public float cMinSpeed = 10;

    [System.NonSerialized]
    public bool cAllowCameraControls = true;
    float cMovementSideSize = 10;

    [Header("Camera Toggles")]
    [Tooltip("Toggle bewteen the 2 dfferent movement style")]
    public bool useDragPanning = false;
    [Tooltip("Invert the pitch movement")]
    public bool invertPitch = true;
    [Tooltip("Invert the yaw movement")]
    public bool invertYaw = false;
    
    Transform cRotator;
    Transform cZoomer;
    GridManager gm;

    private static CameraController instance = null;

    public static CameraController GetInstance()
    {
        return instance;
    }

    void Awake()
    {
        instance = this;
        cRotator = transform.GetChild(0);
        cZoomer = cRotator.transform.GetChild(0);
    }

    void Start()
    {
        gm = GridManager.GetInstance();
        transform.position = gm.GetCenterPosition();
    }
	
	void Update ()
    { 
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            useDragPanning = !useDragPanning;
        }

        float zoomControl = Input.GetAxis("Mouse ScrollWheel");

        if (cAllowCameraControls)
        {
            if (zoomControl != 0.0f)
            {
                AdjustZoom(zoomControl);
            }

            if (!useDragPanning)
            {
                //Rotating with middle mouse and movement with borders
                if (Input.GetMouseButton(2))
                {
                    AdjustRotation();
                }
                else
                {
                    AdjustMovement();
                }
            }
            else
            {
                //Rotating and movement with middle mouse
                if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftAlt))
                {
                    AdjustRotation();
                }
                else if (Input.GetMouseButton(2))
                {
                    AdjustMovement2();
                }
            }

            //Reset Camera to center
            if (Input.GetKey(KeyCode.Space))
            {
                transform.position = gm.GetCenterPosition();
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.position = new Vector3(0,0,0);
            }
        }
        
    }

    void AdjustZoom(float zoom)
    {
        cZoomOffset = Mathf.Clamp01(cZoomOffset + zoom);

        float cZoomed = Mathf.Lerp(cMinZoom, cMaxZoom, cZoomOffset); 
        cZoomer.localPosition = new Vector3(0, 0, cZoomed);
        
    }

    //Movement with borders
    void AdjustMovement()
    {
        Rect lowerBound = new Rect(0, 0, Screen.width, cMovementSideSize);
        Rect upperBound = new Rect(0, Screen.height - cMovementSideSize, Screen.width, cMovementSideSize);
        Rect leftBound = new Rect(0, 0, cMovementSideSize, Screen.height);
        Rect rightBound = new Rect(Screen.width - cMovementSideSize, 0, cMovementSideSize, Screen.height);

        float cFinalSpeed = cMinSpeed + ((cMaxSpeed-cMinSpeed) * cZoomOffset);

        if (lowerBound.Contains(Input.mousePosition))
            transform.localPosition += -transform.forward * cFinalSpeed * Time.deltaTime;
        if (upperBound.Contains(Input.mousePosition))
            transform.localPosition += transform.forward * cFinalSpeed * Time.deltaTime;
        if (leftBound.Contains(Input.mousePosition))
            transform.localPosition += -cRotator.transform.right * cFinalSpeed * Time.deltaTime;
        if (rightBound.Contains(Input.mousePosition))
            transform.localPosition += cRotator.transform.right * cFinalSpeed * Time.deltaTime;
    }

    //Movement with dragging
    void AdjustMovement2()
    {
        float cFinalSpeed = cMinSpeed + ((cMaxSpeed - cMinSpeed) * cZoomOffset);
        Vector3 nVec = transform.rotation * new Vector3(-Input.GetAxis("Mouse X") * cFinalSpeed * Time.deltaTime, 0, -Input.GetAxis("Mouse Y") * cFinalSpeed * Time.deltaTime);
        transform.localPosition += nVec;
    }

    void AdjustRotation()
    {
        currentPitch = Input.GetAxis("Mouse Y") * (invertPitch ? -cRotationSpeed : cRotationSpeed);
        currentYaw = Input.GetAxis("Mouse X") * (invertYaw ? -cRotationSpeed : cRotationSpeed);
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, currentYaw, 0));
        currentPitch = cRotator.transform.localRotation.eulerAngles.x + currentPitch;
        cRotator.transform.localRotation = Quaternion.Euler(new Vector3(Mathf.Clamp(currentPitch, cMinRotationY, cMaxRotationY), 0, 0));
    }
}
