using Assets.Scripts;
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
    public AudioClipGroup fireSound;
    public AudioClipGroup deathSound;
    public Projectile projectile;
    private Player p;
    public bool dead;


    void Start() {
        p =  GameObject.Find("Player").GetComponent<Player>();
        if (Vector3.Distance(transform.position, new Vector3(p.transform.position.x, 0, p.transform.position.z)) < 60f) {
            Destroy(gameObject);
            return;
        }
        turnSpeed += Random.Range(0, 20);
        turningDir = Random.Range(0, 100) < 50;
        currentReloadTime = 0;
        lr = GetComponent<LineRenderer>();
        lr.SetPosition(0, transform.position + new Vector3(0,0.25f,0));
        
    }

    private void OnCollisionEnter(Collision collision) {
        if (!dead) {
            gameObject.layer = 11;
            dead = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.None;
            UIController.instance.increaseScore();
            lr.enabled = false;
            deathSound.play();
        }
    }


    void Update()
    {
        if (dead)
            return;


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
        fireSound.play();
        currentReloadTime = reloadTime;
        Projectile p = Instantiate(projectile);
        p.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0, Random.Range(-10, 10), 0));
        p.transform.position = transform.position + transform.forward;
    }
}
