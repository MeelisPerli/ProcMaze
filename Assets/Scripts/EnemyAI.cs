using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{

    public float turnSpeed;
    private bool turningDir;
    public float reloadTime;
    private float currentReloadTime;
    private bool focusPlayer;
    private LineRenderer lr;
    public Projectile projectile;
    private Player p;


    void Start() {
        p =  GameObject.Find("Player").GetComponent<Player>();
        if (Vector3.Distance(transform.position, p.transform.position) < 30f) {
            Destroy(gameObject);
            return;
        }
        turningDir = Random.Range(0, 100) < 50;
        currentReloadTime = 0;
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + new Vector3(0,0.25f,0));
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.other.tag == "Player") {
            UIController.instance.gameOver();
        }
    }


    void Update()
    {
        if (currentReloadTime >= 0)
            currentReloadTime -= Time.deltaTime;

        if (focusPlayer) {
            if (Vector3.Distance(p.transform.position, transform.position) > 30f)
                focusPlayer = false;
            transform.LookAt(p.transform.position);
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }

        RaycastHit hit;

        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(transform.position + transform.forward, transform.TransformDirection(Vector3.forward), out hit, 50f)) {
            lr.SetPosition(1, hit.point + new Vector3(0, 0.25f, 0));
            if (hit.transform.tag == "Player") {
                focusPlayer = true;
                if (currentReloadTime <= 0) {
                    shoot();
                }
            }
        } else {
            lr.SetPosition(1, transform.position + transform.forward*30 + new Vector3(0, 0.25f, 0));
        }

            
        


        if (turningDir)
            transform.Rotate(0, turnSpeed*Time.deltaTime, 0);
        else
            transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
    }

    private void shoot() {
        currentReloadTime = reloadTime;
        Projectile p = Instantiate(projectile);
        p.transform.rotation = transform.rotation;
        p.transform.position = transform.position + transform.forward;
    }
}
