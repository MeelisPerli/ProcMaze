using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed;
    public float lifeTime;

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0) {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            UIController.instance.gameOver();
        }
        Destroy(gameObject);
    }

    /*
    private void OnCollisionEnter(Collision collision) {
        if (collision.rigidbody.tag == "Player") {
            UIController.instance.gameOver();
        }
        Destroy(gameObject);
    }
    */
}
