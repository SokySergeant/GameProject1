using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinScript : MonoBehaviour
{
    public float spinSpeed = 150f;

    void Update()
    {
        transform.Rotate(new Vector3(0f, spinSpeed * Time.deltaTime, 0f));
    }
}
