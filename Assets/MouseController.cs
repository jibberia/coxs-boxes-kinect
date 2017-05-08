using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

	public Camera camera;
	public Transform floor;

	private float height = 0f;
	public float heightDelta = 0.02f;

	private const int FLOOR_LAYER = 8;
	private const int FLOOR_MASK = 0 | (1 << FLOOR_LAYER);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit hit;
		Ray ray = camera.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray, out hit, 100f, FLOOR_MASK)) {
			if (hit.transform == floor) {
				Vector3 pos = new Vector3(
					hit.point.x,
					height,
					hit.point.z);
//				print(pos);
				this.gameObject.transform.position = pos;
//				gameObject.transform.position = hit.point;
			}
		}
		if (Input.GetKey(KeyCode.W)) {
			height += heightDelta;
			UpdateHeight();
		}
		if (Input.GetKey(KeyCode.S)) {
			height -= heightDelta;
			UpdateHeight();
		}
	}

	void UpdateHeight() {
		print(height);
		this.gameObject.transform.position = new Vector3(
			this.gameObject.transform.position.x,
			height,
			this.gameObject.transform.position.z);
	}
}
