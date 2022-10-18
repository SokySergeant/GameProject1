using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraControllerScript : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera playerCam;
    [SerializeField] private GameObject playerObject;

    public float zoomIncrementDelay = .01f;
    public float unzoomIncrementDelay = .01f;
    public float zoomVelocity = 20f;
    public float unzoomVelocity = 15f;
    public float zoomMultiplier = 1.1f;

    private Rigidbody myRB;
    private bool zoomOn = false;

    // Start is called before the first frame update
    void Start()
    {
        myRB = playerObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (myRB.velocity.magnitude > zoomVelocity)
        {
            if (!zoomOn)
            {
                zoomOn = true;
                StartCoroutine(TightenUp());
            }
        }

        if (myRB.velocity.magnitude < unzoomVelocity)
        {
            if (zoomOn)
            {
                zoomOn = false;
                StartCoroutine(LoosenUp());
            }
        }
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
}
