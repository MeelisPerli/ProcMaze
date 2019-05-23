using Unity.Jobs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;


public struct MapContinuerJOb : IJob
{

    public GameObject chunk;

    public void Execute() {
        OverlapWFC c = chunk.GetComponent<OverlapWFC>();
        c.enabled = true;
    }
}
