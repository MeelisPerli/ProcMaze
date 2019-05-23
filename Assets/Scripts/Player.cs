using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed;
    public float jumpStr;
    private Camera cam;
    private Rigidbody rb;
    private Vector3 lastMousePos;
    public float camAngle;
    public float camSensitivity;
    public GameObject waterTile;
    private bool cursorLocked;
    public int joystickNumber;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        cursorLocked = true;
        rb = GetComponent<Rigidbody>();
        lastMousePos = new Vector3(-1000, -1000);
        cam = GetComponentInChildren<Camera>();

    }

    void Update() {

        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        move();
        moveXBox();
        cameraMovements();
        jumping();
        if (transform.position.y < -3)
            UIController.instance.gameOver();

        if (Input.GetKeyDown("m")) {
            if (cursorLocked) {
                Cursor.lockState = CursorLockMode.None;
                cursorLocked = false;
            }
            else {
                Cursor.lockState = CursorLockMode.Locked;
                cursorLocked = true;
            }
        }
    }

    private void move() {

        Vector3 movementVec = new Vector3();

        if (Input.GetKey("w")) {
            movementVec += (transform.forward);
        }
        else if (Input.GetKey("s")) {
            movementVec += (-transform.forward);
        }
        if (Input.GetKey("a")) {
            movementVec += (-transform.right);
        }
        else if (Input.GetKey("d")) {
            movementVec += (transform.right);
        }
        movementVec = movementVec.normalized*speed;
        rb.velocity += movementVec;
        waterTile.transform.position = new Vector3(transform.position.x, -1.7f, transform.position.z);
        
        
    }

    private void moveXBox() {
        Vector3 v = transform.right * Input.GetAxis("LJoystickHorizontal") - transform.forward * Input.GetAxis("LJoystickVertical");
        if (v.magnitude < 0.25f)
            return;
        rb.velocity += v*speed;
        
    }

    private void jumping() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Mathf.Abs(rb.velocity.y) < 0.1)
                rb.velocity = new Vector3(rb.velocity.x, jumpStr, rb.velocity.z);
        }
    }

    private void cameraMovements() {
        rb.angularVelocity = new Vector3();

        if (!cursorLocked)
            return;

        Vector3 delta = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        Vector3 consoleDelta = new Vector3(Input.GetAxis("RJoystickHorizontal"), + Input.GetAxis("RJoystickVertical"), 0);
        if (consoleDelta.magnitude > 0.1f)
            delta += consoleDelta * 0.45f;
        transform.rotation = Quaternion.Euler(0, delta.x * camSensitivity, 0) * transform.rotation;

        // y and z
        camAngle -= delta.y * (camSensitivity);
        if (camAngle > 70)
            camAngle = 70;
        else if (camAngle < -80) 
            camAngle = -80;
        

        Vector3 camrot = new Vector3(camAngle, 0, 0);
        cam.transform.localRotation = Quaternion.Euler(camrot);
    }

    
}
