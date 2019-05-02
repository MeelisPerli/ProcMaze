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
    private Dictionary<int, Dictionary<int, bool>> gridBooleans;

    private Player player;

    private void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        grid = new Dictionary<int, Dictionary<int, GameObject>>();
        gridBooleans = new Dictionary<int, Dictionary<int, bool>>();
        generateAt(0, 0);
        OverlapWFC chunk = getChunkAt(0, 0).GetComponent<OverlapWFC>();
        StartCoroutine(spawnPlayer(chunk));
        instance = this;
    }


    void Update() {

        Vector3 pPos = player.transform.position;
        int pX = (int)Mathf.Floor(pPos.x / chunkSize);
        int pZ = (int)Mathf.Ceil(pPos.z / chunkSize);
        //resetGridBooleans();
        gridBooleans[pX][pZ] = true; // player's position must always be active
        for (int i = 0; i < renderChunksR; i++) {
            checkChunkAt(pX + i, pZ);
            checkChunkAt(pX - i, pZ);
            checkChunkAt(pX, pZ + i);
            checkChunkAt(pX, pZ - i);
            for (int j = 1; j <= i; j++) {
                checkChunkAt(pX + i, pZ + j);
                checkChunkAt(pX - i, pZ + j);
                checkChunkAt(pX + j, pZ + i);
                checkChunkAt(pX + j, pZ - i);

                checkChunkAt(pX + i, pZ - j);
                checkChunkAt(pX - i, pZ - j);
                checkChunkAt(pX - j, pZ + i);
                checkChunkAt(pX - j, pZ - i);
            }
        }
        renderActiveChunks();
    }

    IEnumerator spawnPlayer(OverlapWFC chunk) {
        while (chunk.model == null) {
            Debug.Log("waiting");
            yield return new WaitForSeconds(0.1f);
        }
        GameObject tile = chunk.findGoodTileForSpawning(30);
        player.transform.position = tile.transform.position + new Vector3(0, 4, 0);
        Debug.Log(tile.transform.position);
        Debug.DrawLine(tile.transform.position, tile.transform.position + new Vector3(0, 10, 0), Color.red, 1000);
    }

    private void renderActiveChunks() {
        foreach (int x in gridBooleans.Keys) {
            List<int> r = new List<int>(gridBooleans[x].Keys);
            foreach (int y in r) {
                GameObject o = grid[x][y];

                if (gridBooleans[x][y] == false && o.activeSelf == true)
                    o.SetActive(false);
                if (gridBooleans[x][y] == true && o.activeSelf == false)
                    o.SetActive(true);
                gridBooleans[x][y] = false;
                
            }
        }
    }

    private void checkChunkAt(int x, int z) {
        if (isPosFree(x, z)) {
            generateAt(x, z);
        }
        gridBooleans[x][z] = true;
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
        Dictionary<int, bool> rowB;
        if (grid.ContainsKey(x)) {
            row = grid[x];
            rowB = gridBooleans[x];
        } else {
            row = new Dictionary<int, GameObject>();
            grid.Add(x, row);
            rowB = new Dictionary<int, bool>();
            gridBooleans.Add(x, rowB);
        }

        if (row.ContainsKey(z))
            return false;
        row.Add(z, chunk);
        rowB.Add(z, true);
        return true;
    }

    private void resetGridBooleans() {
        foreach (int x in gridBooleans.Keys) {
            foreach(int y in gridBooleans[x].Keys) {
                gridBooleans[x][y] = false;
            }
        }
    }
}

