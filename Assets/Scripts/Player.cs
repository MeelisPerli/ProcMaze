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

    void Start() {
        rb = GetComponent<Rigidbody>();
        lastMousePos = new Vector3(-1000, -1000);
        cam = GetComponentInChildren<Camera>();
    }

    void Update() {
        move();
        cameraMovements();
        jumping();
        if (transform.position.y < -3)
            UIController.instance.gameOver();
    }

    private void move() {

        rb.velocity = new Vector3(0, rb.velocity.y, 0);
        Vector3 movementVec = new Vector3();

        if (Input.GetKey("w")) {
            movementVec += (transform.forward) * speed;
        }
        else if (Input.GetKey("s")) {
            movementVec += (-transform.forward) * speed;
        }
        if (Input.GetKey("a")) {
            movementVec += (-transform.right) * speed;
        }
        else if (Input.GetKey("d")) {
            movementVec += (transform.right) * speed;
        }
        rb.velocity += movementVec;
        waterTile.transform.position = new Vector3(transform.position.x, -1.5f, transform.position.z);

    }

    private void jumping() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Mathf.Abs(rb.velocity.y) < 0.1)
                rb.velocity = new Vector3(rb.velocity.x, jumpStr, rb.velocity.z);
        }
    }

    private void cameraMovements() {
        rb.angularVelocity = new Vector3();
        if (Input.GetMouseButton(1)) {
            Vector3 newPos = Input.mousePosition;
            if (lastMousePos == new Vector3(-1000, -1000)) {
                lastMousePos = newPos;
                return;
            }
            Vector3 delta = newPos - lastMousePos;
            lastMousePos = newPos;
            transform.rotation = Quaternion.Euler(0, delta.x / camSensitivity, 0) * transform.rotation;

            // y and z
            camAngle -= delta.y / (camSensitivity);
            if (camAngle > 70)
                camAngle = 70;
            else if (camAngle < -80) 
                camAngle = -80;
        } else {
            lastMousePos = new Vector3(-1000, -1000);
        }
        Vector3 camrot = new Vector3(camAngle, 0, 0);
        cam.transform.localRotation = Quaternion.Euler(camrot);

    }
}
