using UnityEngine;
using System.Collections;

public class BoundsManager : MonoBehaviour {

	// Get the singleton instance
	private static BoundsManager instance;
	public static BoundsManager Instance {
		get {
			if (instance != null)
				return instance;

			instance = FindObjectOfType<BoundsManager>();
			return instance;
		}
	}

	public float RightBounds = 10f;
	public float LeftBounds = 10f;
	public float UpperBounds = 10f;
	public float LowerBounds = 10f;

	public Transform OutOfBoundsEffect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(new Vector3((RightBounds - LeftBounds)/2, (UpperBounds - LowerBounds)/2), new Vector3(LeftBounds + RightBounds, UpperBounds + LowerBounds));
	}

	public static bool CheckBounds(Vector3 position) {
		var manager = Instance;
		if(position.x < -manager.LeftBounds || position.x > manager.RightBounds || position.y < -manager.LowerBounds || position.y > manager.UpperBounds) {
			return true;
		}

		return false;
	}
}
