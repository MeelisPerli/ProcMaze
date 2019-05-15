using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMover : MonoBehaviour
{

    public Vector3 dist;
    public bool scaleBlock;
    public Vector3 scale;

    private void Start() {
        if (scaleBlock)
            transform.localScale = scale;
    }
}
