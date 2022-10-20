using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BetterCameraControllerScript : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private CinemachineVirtualCamera playerZoomCam;
    [SerializeField] private CinemachineVirtualCamera playerAscendCam;
    [SerializeField] private PlayerMovement playerMovement;

    private List<CinemachineVirtualCamera> allMyCams = new List<CinemachineVirtualCamera>();

    

    void Awake()
    {
        allMyCams.Add(playerCam);
        allMyCams.Add(playerZoomCam);
        allMyCams.Add(playerAscendCam);
    }



    void Update()
    {

        if (playerMovement.horizontalInput.x == 0){

            if(playerMovement.velocity.y > 1){
                SwitchToCamera(playerAscendCam);
            }else if(playerMovement.velocity.y < -1){
                SwitchToCamera(playerZoomCam);
            }else{
                SwitchToCamera(playerCam);
            }

        }
    }


   
    private void SwitchToCamera(CinemachineVirtualCamera activeCam)
    {
        if(activeCam.Priority != 10){
            foreach(CinemachineVirtualCamera cammy in allMyCams)
            {
                cammy.Priority = 0;
            }
            activeCam.Priority = 10;
        }
    }



}
