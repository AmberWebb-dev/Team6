using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] int sensitivity;
    [SerializeField] float lockVertMin;
    [SerializeField] float lockVertMax;
    [SerializeField] bool invertY;

    float rotateX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get Input
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") *sensitivity * Time.deltaTime;

        if (invertY)
        {
            rotateX += mouseY;
        }
        else
        {
            rotateX -= mouseY;
        }

        // Clamp the cameras X rotation
        rotateX = Mathf.Clamp(rotateX, lockVertMin, lockVertMax);

        // Rotate camera on the X axis
        transform.localRotation = Quaternion.Euler(rotateX, 0, 0);

        // Rotate the player on the Y axis
        transform.parent.Rotate(Vector3.up * mouseX);
            
    }
}
