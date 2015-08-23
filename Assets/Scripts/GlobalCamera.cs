using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GlobalCamera : Photon.PunBehaviour {

	public static List<Transform> Pawns = new List<Transform>();

	public static void UpdatePawnList() {
		Pawns = FindObjectsOfType<Pawn>().Select(e => e.transform).ToList();
	}

	public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer) {
		UpdatePawnList();
    }

	private Camera camera;

	// Use this for initialization
	void Start () {
		camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void LateUpdate () {
		UpdatePawnList();
		var centerPosition = Vector3.zero;

		if (Pawns.Count == 0) {
			transform.position = Vector3.Lerp(transform.position, centerPosition + new Vector3(0, 0, -5), Time.deltaTime * 4f);
			return;
		}

		// Calculate center position between all pawns
		
		foreach(var pawnTransform in Pawns) {
			centerPosition += pawnTransform.position;
        }
		
		centerPosition /= Pawns.Count;
		
		transform.position = Vector3.Lerp(transform.position, centerPosition + new Vector3(0, 0, -5), Time.deltaTime * 4f);

		// Calculate max distance from camera and adjust camera size to cover all pawns
		float largestDifference = 0;
		foreach(var pawnTransform in Pawns) {
			if (largestDifference == 0) {
				largestDifference = (centerPosition - pawnTransform.position).magnitude;
				continue;
			}
				
			if((centerPosition - pawnTransform.position).magnitude > largestDifference) {
				largestDifference = (centerPosition - pawnTransform.position).magnitude;
			}
		}

		camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, Mathf.Clamp(largestDifference, 3.5f, 8), Time.deltaTime * 4f);
    }
}
