using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapContinuer : MonoBehaviour
{
    public static MapContinuer instance;

    public GameObject mapChunkPrefab;
    public int chunkSize = 76;
    public int renderChunksR = 1;

    // x, z -> Chunk
    public Dictionary<int, Dictionary<int, GameObject>> grid;

    private Player player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        grid = new Dictionary<int, Dictionary<int, GameObject>>();
        generateAt(0, 0);
        instance = this;
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
        OverlapWFC chunk = Instantiate(mapChunkPrefab).GetComponent<OverlapWFC>();
        chunk.transform.parent = transform;
        chunk.transform.position = new Vector3(x*chunkSize, 0, z*chunkSize);
        chunk.xloc = x;
        chunk.zloc = z;
        if (AddToPos(x, z, chunk.gameObject))
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

    public GameObject getChunkAt(int x, int z) {
        if (isPosFree(x,z))
            return null;
        return grid[x][z];
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
