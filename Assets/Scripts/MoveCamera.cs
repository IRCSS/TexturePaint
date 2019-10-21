using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {


    public float HorizontalVerticalSensitivity = 0.1f;
    public float UpDownSensitivity             = 0.015f;
    public bool  InvertedY                     = false;
    public float SpeedMultiplyerOnShiftPressed = 4f;

    private float moveUpDown;
    private float speedMultiplyer = 1f;
    private float inversion       = -1f;
    void Start()
    {
        if (InvertedY) inversion = 1f;
        else           inversion = -1f;

    }
    
    void Update () {

        if (!Input.GetKey(KeyCode.Mouse1)) return;
       
        float x      = Input.GetAxis("Horizontal");
        float y      = Input.GetAxis("Vertical");
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        speedMultiplyer = Mathf.Lerp(speedMultiplyer, 1f, 0.1f);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speedMultiplyer += 0.2f;
            speedMultiplyer = Mathf.Clamp(speedMultiplyer, 1f, SpeedMultiplyerOnShiftPressed);
        }

        moveUpDown = Mathf.Lerp(moveUpDown, 0f, 0.1f);
        if (Input.GetKey(KeyCode.E)) moveUpDown += UpDownSensitivity;
        if (Input.GetKey(KeyCode.Q)) moveUpDown -= UpDownSensitivity;

        this.transform.Rotate(Vector3.up * mouseX, Space.World);
        this.transform.Rotate(this.transform.right *-1f * mouseY, Space.World);
        this.transform.position += this.transform.right * x* HorizontalVerticalSensitivity * speedMultiplyer;
        this.transform.position += this.transform.forward * y* HorizontalVerticalSensitivity * speedMultiplyer;
        this.transform.position += this.transform.up * moveUpDown* speedMultiplyer;


    }
}

