using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class BetterCameraControllerScript : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera playerZoomCam;
    [SerializeField] private GameObject playerObject;

    public float zoomIncrementDelay = .01f;
    public float unzoomIncrementDelay = .01f;
    public float zoomVelocity = 20f;
    public float unzoomVelocity = 15f;
    public float zoomMultiplier = 1.1f;

    private Rigidbody myRB;
    public static bool zoomOn = false;
    private List<CinemachineVirtualCamera> allMyCams = new List<CinemachineVirtualCamera> ();
    private Keyboard mykeyboprad;
    

    // Start is called before the first frame update
    void Start()
    {
        myRB = playerObject.GetComponent<Rigidbody>();
        allMyCams.Add(playerCam);
        allMyCams.Add (playerZoomCam);
        mykeyboprad = Keyboard.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (mykeyboprad.shiftKey.isPressed && mykeyboprad.zKey.wasPressedThisFrame)
        {
            SwitchToCamera(playerZoomCam);
        }
        if (mykeyboprad.shiftKey.isPressed && mykeyboprad.uKey.wasPressedThisFrame)
        {
            SwitchToCamera(playerCam);
        }
    }

    private void FixedUpdate()
    {

        // uncomment this when we sort the switch condition
        /*if (zoomOn)
        {
            SwitchToCamera(playerZoomCam);
           
        }

        if (myRB.velocity.magnitude < unzoomVelocity)
        {
            SwitchToCamera(playerCam);

        }*/
        
    }

    private IEnumerator TightenUp()
    {
        int index = 0;

        while (index < 100)
        {
            playerCam.m_Lens.FieldOfView -= .25f * zoomMultiplier;
            yield return new WaitForSecondsRealtime(zoomIncrementDelay);
            index++;
        }
    }

    private IEnumerator LoosenUp()
    {
        int index = 0;

        while (index < 100)
        {
            playerCam.m_Lens.FieldOfView += .25f * zoomMultiplier;
            yield return new WaitForSecondsRealtime(unzoomIncrementDelay);
            index++;
        }
    }

    private void SwitchToCamera(CinemachineVirtualCamera activeCam)
    {
        if(activeCam.Priority != 10)
        {
            foreach(CinemachineVirtualCamera cammy in allMyCams)
            {
                cammy.Priority = 0;
            }
            activeCam.Priority = 10;
        }
    }



}
