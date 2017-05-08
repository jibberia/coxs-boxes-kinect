using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    //public GameObject prefab;
    public GameObject[] prefabs;
    public Transform parentTransform;

    public int num = 10;
    public float minimumScale = .1f, maximumScale = .6f;

    private List<GameObject> objects;

    // Use this for initialization
    void Start() {
        objects = new List<GameObject>();

        foreach (GameObject prefab in prefabs) {
            prefab.SetActive(false);
        }

        for (int i=0; i < num; i++) {
            GameObject prefab = RandomPrefab();
            GameObject go = Instantiate(prefab) as GameObject;
            SetInitialPosition(go);
            objects.Add(go);
        }

        GetComponent<MeshRenderer>().enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		foreach (var go in objects) {
            if (go.transform.position.y < -20) {
                SetInitialPosition(go);
            }
        }
	}

    void SetInitialPosition(GameObject go)
    {
        // go.transform.parent = this.transform;
        go.transform.SetParent(parentTransform, false);

        Vector3 ls = this.transform.localScale;

        go.transform.rotation = Quaternion.identity;
        Vector3 randPos = new Vector3(
            Random.Range(-ls.x/2f, ls.x/2f),
            Random.Range(-ls.y/2f, ls.y/2f),
            Random.Range(-ls.z/2f, ls.z/2f));
        go.transform.position = this.transform.position + randPos;
        float s = Random.Range(minimumScale, maximumScale);
        Vector3 worldNormalizedScale = new Vector3(
            s / ls.x,
            s / ls.x,
            s / ls.x);
            //(s / ls.x) * (ls.x / ls.z));
        go.transform.localScale = Vector3.one * s;//worldNormalizedScale;//new Vector3(s, s, s);
        /*
        go.transform.localScale = new Vector3(
            Random.Range(minimumScale, maximumScale),
            Random.Range(minimumScale, maximumScale),
            Random.Range(minimumScale, maximumScale));
        */
        // go.transform.SetParent(this.transform, false);

        go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        go.SetActive(true);
//        print(go.transform.lossyScale);
    }

    private GameObject RandomPrefab()
    {
        return prefabs[(int)(Random.value * prefabs.Length)];
    }
}
