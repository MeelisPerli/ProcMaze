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

    void Start() {
        rb = GetComponent<Rigidbody>();
        lastMousePos = new Vector3(-1000, -1000);
        cam = GetComponentInChildren<Camera>();
    }

    void Update() {
        move();
        cameraMovements();
        jumping();
    }

    private void move() {
        rb.velocity = new Vector3(0, rb.velocity.y, 0);

        if (Input.GetKey("w")) {
            rb.velocity += (transform.forward) * speed;
        }
        else if (Input.GetKey("s")) {
            rb.velocity += (-transform.forward) * speed;
        }
        if (Input.GetKey("a")) {
            rb.velocity += (-transform.right) * speed;
        }
        else if (Input.GetKey("d")) {
            rb.velocity += (transform.right) * speed;
        }
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
            rb.angularVelocity = new Vector3(0, delta.x / camSensitivity, 0);

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
