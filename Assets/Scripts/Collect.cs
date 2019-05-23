using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collect : MonoBehaviour
{
    private void OnTriggerEnter(Collider other) {
        if (other.GetComponent<Player>() != null) {
            UIController.instance.increaseScore(5);
            Destroy(gameObject);
        }
    }
}
