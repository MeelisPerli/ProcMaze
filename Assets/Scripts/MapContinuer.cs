using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapContinuer : MonoBehaviour
{

    public GameObject mapChunkPrefab;
    public int chunkSize = 76;
    public int renderChunksR = 1;

    // x, z -> Chunk
    Dictionary<int, Dictionary<int, GameObject>> grid;

    private Player player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        grid = new Dictionary<int, Dictionary<int, GameObject>>();
        generateAt(0, 0);
    }


    void Update() {

        Vector3 pPos = player.transform.position;
        int pX = (int)Mathf.Floor(pPos.x / chunkSize);
        int pZ = (int)Mathf.Ceil(pPos.z / chunkSize);

        for (int i = 0; i < renderChunksR; i++) {
            tryToGenAt(pX + i, pZ);
            tryToGenAt(pX - i, pZ);
            tryToGenAt(pX, pZ + i);
            tryToGenAt(pX, pZ - i);
            for (int j = 1; j <= i; j++) {
                tryToGenAt(pX + i, pZ + j);
                tryToGenAt(pX - i, pZ + j);
                tryToGenAt(pX + j, pZ + i);
                tryToGenAt(pX + j, pZ - i);

                tryToGenAt(pX + i, pZ - j);
                tryToGenAt(pX - i, pZ - j);
                tryToGenAt(pX - j, pZ + i);
                tryToGenAt(pX - j, pZ - i);
            }
        }
    }

    private void tryToGenAt(int x, int z) {
        if (isPosFree(x, z)) {
            generateAt(x, z);
        }
    }

    private void generateAt(int x, int z) {
        GameObject chunk = Instantiate(mapChunkPrefab);
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(x*chunkSize, 0, z*chunkSize);
        if (AddToPos(x, z, chunk))
            return;
        Destroy(chunk);
    }

    private bool isPosFree(int x, int z) {
        if (grid.ContainsKey(x)) {
            if (grid[x].ContainsKey(z))
                return false;
        }
        return true;
    }

    private bool AddToPos(int x, int z, GameObject chunk) {
        Dictionary<int, GameObject> row;
        if (grid.ContainsKey(x)) {
             row = grid[x];
        } else {
            row = new Dictionary<int, GameObject>();
            grid.Add(x, row);
        }

        if (row.ContainsKey(z))
            return false;
        row.Add(z, chunk);
        return true;
    }
}
