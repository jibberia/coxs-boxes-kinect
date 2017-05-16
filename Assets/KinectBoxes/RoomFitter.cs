using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomFitter : MonoBehaviour {

	BodyView bodyView;
	public Transform backWall, leftWall, rightWall, floor, ceiling;

	// Use this for initialization
	void Start () {
		bodyView = this.GetComponent<BodyView>();		
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject body in bodyView.Bodies) {
			foreach (Transform child in body.transform) {
				//print("child z: " + child.transform.position.z + " backWall z: " + backWall.transform.position.z);
				// BACK WALL
				if (child.transform.position.z < backWall.transform.position.z) {
					Vector3 pos = backWall.transform.position;
					pos.z = child.transform.position.z;
					backWall.transform.position = pos;
				}

				// LEFT WALL
				if (child.transform.position.x > leftWall.transform.position.x) {
					Vector3 pos = leftWall.transform.position;
					pos.x = child.transform.position.x;
					leftWall.transform.position = pos;
				}
				
				// RIGHT WALL
				if (child.transform.position.x < rightWall.transform.position.x) {
					Vector3 pos = rightWall.transform.position;
					pos.x = child.transform.position.x;
					rightWall.transform.position = pos;
				}

				// FLOOR
				if (child.transform.position.y < floor.transform.position.y) {
					Vector3 pos = floor.transform.position;
					pos.y = child.transform.position.y;
					floor.transform.position = pos;
				}
			}
			// body.transform

		}
	}

	public void TrimRoom() {
		Debug.Log("trimming room...");
		float back = backWall.position.z;
		Vector3 scale;

		scale = leftWall.transform.localScale;
		scale.x = Mathf.Abs((back / 10f) * 2f);
		scale.z += Mathf.Abs(floor.transform.position.y) * 2f;
		leftWall.transform.localScale = scale;

		scale = rightWall.transform.localScale;
		scale.x = Mathf.Abs((back / 10f) * 2f);
		scale.z += Mathf.Abs(floor.transform.position.y) * 2f;
		rightWall.transform.localScale = scale;

		scale = floor.transform.localScale;
		scale.x = Mathf.Abs((leftWall.transform.position.x / 10f) * 2f);
		scale.z = Mathf.Abs((back / 10f) * 2f);
		floor.transform.localScale = scale;

	}
}
