using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class BetterCameraControllerScript : MonoBehaviour
{
    //enum VerticalState { Ascending, Descending, Steady };

    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera playerZoomCam;
    [SerializeField] private CinemachineVirtualCamera playerAscendCam;
    [SerializeField] private GameObject playerObject;

    public static bool zoomOn = false;
    private List<CinemachineVirtualCamera> allMyCams = new List<CinemachineVirtualCamera> ();
    private Keyboard mykeyboprad;

    private bool currentHorizontalInput = false;

    public static float currentPlayerY = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        //myRB = playerObject.GetComponent<Rigidbody>();
        allMyCams.Add(playerCam);
        allMyCams.Add (playerZoomCam);
        allMyCams.Add (playerAscendCam);
        mykeyboprad = Keyboard.current;
        currentPlayerY = playerObject.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        /*if (mykeyboprad.shiftKey.isPressed && mykeyboprad.zKey.wasPressedThisFrame)
        // dev commands...
        {
            SwitchToCamera(playerZoomCam);
        }
        if (mykeyboprad.shiftKey.isPressed && mykeyboprad.uKey.wasPressedThisFrame)
        {
            SwitchToCamera(playerCam);
        }
        */

        if(playerObject.GetComponent<PlayerMovement>().horizontalInput.x != 0)
        {
            currentHorizontalInput = true;
        } else
        {
            currentHorizontalInput = false;
        }
        

    }

    private void FixedUpdate()
    {
        if (!currentHorizontalInput)
        {
            if (playerObject.transform.position.y < currentPlayerY)
            {
                SwitchToCamera(playerZoomCam);
            }
            else if (playerObject.transform.position.y > currentPlayerY)
            {
                SwitchToCamera(playerAscendCam);
            }
            else
            {
                SwitchToCamera(playerCam);
            }
        }
        currentPlayerY = playerObject.transform.position.y;
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
