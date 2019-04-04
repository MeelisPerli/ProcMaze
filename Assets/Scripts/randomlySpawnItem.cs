using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class randomlySpawnItem : MonoBehaviour
{

    public float chanceToSpawn;
    public GameObject objToSpawn;
    public Vector3 offSet;
    // Start is called before the first frame update
    void Start()
    {
        if (Random.Range(0,1000)/1000f <= chanceToSpawn) {
            GameObject o = Instantiate(objToSpawn);
            o.transform.position += offSet + transform.position;
        }
    }

}
