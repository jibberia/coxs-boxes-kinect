using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

    //public GameObject prefab;
    public GameObject[] prefabs;

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
        go.transform.parent = this.transform;

        go.transform.rotation = Quaternion.identity;
        go.transform.position = new Vector3(
            Random.Range(-1.5f, 1.5f),
            Random.Range(2, 4),
            Random.Range(-1.5f, 1.5f));
        float s = Random.Range(minimumScale, maximumScale);
        go.transform.localScale = new Vector3(s, s, s);
        /*
        go.transform.localScale = new Vector3(
            Random.Range(minimumScale, maximumScale),
            Random.Range(minimumScale, maximumScale),
            Random.Range(minimumScale, maximumScale));
        */
        go.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        go.SetActive(true);
    }

    private GameObject RandomPrefab()
    {
        return prefabs[(int)(Random.value * prefabs.Length)];
    }
}
