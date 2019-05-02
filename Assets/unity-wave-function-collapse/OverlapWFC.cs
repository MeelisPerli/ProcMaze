using System;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
class OverlapWFC : MonoBehaviour{
	public Training training = null;
	public int gridsize = 1;
	public int width = 20;
	public int depth = 20;
	public int seed = 0;
    //[HideInInspector]
    public int xloc;
    public int zloc;
    public int N = 2;
	public bool periodicInput = false;
	public bool periodicOutput = false;
	public int symmetry = 1;
	public int foundation = 0;
	public int iterations = 0;
	public bool incremental = false;
	public OverlappingModel model = null;
	public GameObject[,] rendering;
	public GameObject output;
	private Transform group;
    private bool undrawn = true;

	public static bool IsPrefabRef(UnityEngine.Object o){
		#if UNITY_EDITOR
		return PrefabUtility.GetPrefabParent(o) == null && PrefabUtility.GetPrefabObject(o) != null;
		#endif
		return true;
	}

	static GameObject CreatePrefab(UnityEngine.Object fab, Vector3 pos, Quaternion rot) {
		#if UNITY_EDITOR
		GameObject e = PrefabUtility.InstantiatePrefab(fab as GameObject) as GameObject; 
		e.transform.position = pos;
		e.transform.rotation = rot;
		return e;
		#endif
		GameObject o = GameObject.Instantiate(fab as GameObject) as GameObject; 
		o.transform.position = pos;
		o.transform.rotation = rot;
		return o;
	}

	public void Clear(){
		if (group != null){
			if (Application.isPlaying){Destroy(group.gameObject);} else {
				DestroyImmediate(group.gameObject);
			}	
			group = null;
		}
	}

	void Awake(){}

	void Start(){
		Generate();
	}

	void Update(){
		if (incremental){
			Run();
		}
	}

	public void Generate() {
		if (training == null){Debug.Log("Can't Generate: no designated Training component");}
		if (IsPrefabRef(training.gameObject)){
			GameObject o = CreatePrefab(training.gameObject, new Vector3(0,99999f,0f), Quaternion.identity);
			training = o.GetComponent<Training>();
		}
		if (training.sample == null){
			training.Compile();
		}
		if (output == null){
			Transform ot = transform.Find("output-overlap");
			if (ot != null){output = ot.gameObject;}}
		if (output == null){
			output = new GameObject("output-overlap");
			output.transform.parent = transform;
			output.transform.position = this.gameObject.transform.position;
			output.transform.rotation = this.gameObject.transform.rotation;}
		for (int i = 0; i < output.transform.childCount; i++){
			GameObject go = output.transform.GetChild(i).gameObject;
			if (Application.isPlaying){Destroy(go);} else {DestroyImmediate(go);}
		}
		group = new GameObject(training.gameObject.name).transform;
		group.parent = output.transform;
		group.position = output.transform.position;
		group.rotation = output.transform.rotation;
        group.localScale = new Vector3(1f, 1f, 1f);
        rendering = new GameObject[width, depth];
		model = new OverlappingModel(training.sample, N, width, depth, periodicInput, periodicOutput, symmetry, foundation, xloc, zloc);
        undrawn = true;
        Run();
    }


    void OnDrawGizmos(){
		Gizmos.color = Color.cyan;
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.DrawWireCube(new Vector3(width*gridsize/2f-gridsize*0.5f, depth*gridsize/2f-gridsize*0.5f, 0f),
							new Vector3(width*gridsize, depth*gridsize, gridsize));
	}

	public void Run(){
		if (model == null){
            Debug.Log("model was null");
            return;
        }
        if (undrawn == false) {
            return;
        }
        if (model.Run(seed, iterations)){
			Draw();
		}
        else {
            Generate();
        }
	}

	public GameObject GetTile(int x, int y){
		return rendering[x,y];
	}

    public bool isModelGood() {
        int count = 0;
        for (int i = 0; i < width; i++) {
            if (model.Sample(i, i) == 0) count++;
        }
        return count < width / 2;
        
    }

	public void Draw(){
		if (output == null){
            Debug.Log("output was null");
            Generate();
            Draw();
        }
		if (group == null){
            Debug.Log("group was null");
            return;
        }

        undrawn = false;
		try{
			for (int y = 0; y < depth; y++){
				for (int x = 0; x < width; x++){
					if (rendering[x,y] == null){
						int v = (int)model.Sample(x, y);
						if (v != 99 && v < training.tiles.Length){
							Vector3 pos = new Vector3(x*gridsize, y*gridsize, 0f);
							int rot = (int)training.RS[v];
							GameObject fab = training.tiles[v] as GameObject;
							if (fab != null){
								GameObject tile = (GameObject)Instantiate(fab, new Vector3() , Quaternion.identity);
								Vector3 fscale = tile.transform.localScale;
								tile.transform.parent = group;
								tile.transform.localPosition = pos;
                                BlockMover bm = tile.GetComponent<BlockMover>();
                                if (bm != null)
                                    tile.transform.localPosition += bm.dist;
								tile.transform.localEulerAngles = new Vector3(0, 0, 360 - (rot * 90));
								tile.transform.localScale = fscale;
								rendering[x,y] = tile;
							}
						} else
                        {
                            undrawn = true;
                        }
					}
				}
	  		}
	  	} catch (IndexOutOfRangeException e) {
	  		model = null;
	  		return;
	  	}
	}

    internal GameObject findGoodTileForSpawning(int minSize) {
        int padding = 5;
        for (int i = padding; i < width- padding; i++) {
            for (int j = padding; j < depth- padding; j++) {
                if (isAreaBigEnough(minSize, i, j)) {
                    GameObject tile = rendering[i,j];
                    Debug.Log(tile.transform.position);
                    return tile;
                }
            }
        }
        return null;
    }

    public bool isAreaBigEnough(int minSize, int x, int y) {
        bool[][] grid = new bool[width][];
        // grid initialization.
        for (int i = 0; i < width; i++) {
            bool[] row = new bool[depth];
            for (int j = 0; j < depth; j++) {
                row[j] = isTileWalkable(i, j);
            }
            grid[i] = row;
        }

        bool target = true;
        bool replacement = false;
        // flood-fill (forest fire implementation)
        if (!grid[x][y])
            return false;
        grid[x][y] = false;
        int areaSize = 1;
        Queue<Vector2> Q = new Queue<Vector2>();
        Q.Enqueue(new Vector2(x, y));
        while (Q.Count > 0) {
            Vector2 n = Q.Dequeue();
            int nX = (int)n.x;
            int nY = (int)n.y;
            if (nX + 1 < width && grid[nX + 1][nY]) {
                areaSize++;
                grid[nX + 1][nY] = false;
                Q.Enqueue(new Vector2(nX + 1, nY));
            }
            if (nX - 1 >= 0 && grid[nX - 1][nY]) {
                areaSize++;
                grid[nX - 1][nY] = false;
                Q.Enqueue(new Vector2(nX - 1, nY));
            }
            if (nY + 1 < width && grid[nX][nY + 1]) {
                areaSize++;
                grid[nX][nY + 1] = false;
                Q.Enqueue(new Vector2(nX, nY + 1));
            }
            if (nY - 1 >= 0 && grid[nX][nY - 1]) {
                areaSize++;
                grid[nX][nY - 1] = false;
                Q.Enqueue(new Vector2(nX, nY - 1));
            }
        }
        return minSize < areaSize;
    }

    private GameObject getTile(int x, int y) {
        int v = (int)model.Sample(x, y);
        if (v == 99 || v >= training.tiles.Length)
            return null;

        GameObject fab = training.tiles[v] as GameObject;
        return fab;
    }

    public bool isTileWalkable(int x, int y) {
        GameObject tile = getTile(x, y);
        if (tile == null)
            return false;

        string name = tile.name;
        return name == "GrassBlock" || name == "Road" || name == "RoadCurve" || name == "RoadIntersection" || name == "BridgeStart Variant" || name == "bridgeMid Variant";
    }
}

#if UNITY_EDITOR
[CustomEditor (typeof(OverlapWFC))]
public class WFCGeneratorEditor : Editor {
	public override void OnInspectorGUI () {
		OverlapWFC generator = (OverlapWFC)target;
		if (generator.training != null){
			if(GUILayout.Button("generate")){
				generator.Generate();
			}
			if (generator.model != null){
				if(GUILayout.Button("RUN")){
					generator.Run();
				}
			}
		}
		DrawDefaultInspector ();
	}
}
#endif