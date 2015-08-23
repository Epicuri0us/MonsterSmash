using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class NetworkInitializer : Photon.PunBehaviour {

	// Get the singleton instance
	private static NetworkInitializer instance;
	public static NetworkInitializer Instance {
		get {
			if (instance != null)
				return instance;

			instance = FindObjectOfType<NetworkInitializer>();
			return instance;
		}
	}

	private static PhotonView ScenePhotonView;

	public static List<string> Characters = new List<string>() { "Yeti", "Gladiator", "Eye", "Bot" };

	// Use this for initialization
	void Start () {
		PhotonNetwork.sendRate = 30;
		PhotonNetwork.ConnectUsingSettings("0.1");
		ScenePhotonView = GetComponent<PhotonView>();
	}

	public override void OnConnectedToMaster() {
		PhotonNetwork.JoinLobby();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.F1)) {
            Respawn(FindPawnWithLocalPlayerNumber(1), Characters[Random.Range(0, Characters.Count)], 1);
		} else if (Input.GetKeyDown(KeyCode.F2)) {
			Respawn(FindPawnWithLocalPlayerNumber(2), Characters[Random.Range(0, Characters.Count)], 2);
		} else if (Input.GetKeyDown(KeyCode.F3)) {
			Respawn(FindPawnWithLocalPlayerNumber(3), Characters[Random.Range(0, Characters.Count)], 3);
		} else if (Input.GetKeyDown(KeyCode.F4)) {
			Respawn(FindPawnWithLocalPlayerNumber(4), Characters[Random.Range(0, Characters.Count)], 4);
		}
	}

	public static Pawn FindPawnWithLocalPlayerNumber(int playerNumber) {
		return FindObjectsOfType<Pawn>().FirstOrDefault(e => e.LocalPlayerNumber == playerNumber);
	}

	public override void OnJoinedLobby() {
		PhotonNetwork.JoinRandomRoom();
	}

	public override void OnPhotonRandomJoinFailed(object[] codeAndMsg) {
		PhotonNetwork.CreateRoom(null, new RoomOptions() { maxPlayers = 4 }, null);
	}

	public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer) {
		
    }

	public override void OnJoinedRoom() {
		Respawn(null, Characters[0], 1);
    }

	public static void Respawn(Pawn oldPawn, string character, int playerNumber) {
		if(oldPawn != null)
			PhotonNetwork.Destroy(oldPawn.photonView);

		GlobalCamera.UpdatePawnList();

		var player = PhotonNetwork.Instantiate(character, Vector3.zero, Quaternion.identity, 0);
		player.GetComponent<PawnMovement>().ControlledLocally = true;
		player.transform.FindChild("Arrow").gameObject.SetActive(true);
		var pawn = player.GetComponent<Pawn>();
		pawn.LocalPlayerNumber = playerNumber;

		GlobalCamera.UpdatePawnList();
	}

	public static void RespawnAfterTime(string name, int playerNumber) {
		Instance.StartCoroutine(Instance.CoRespawnAfterTime(name, playerNumber));
    }

	public IEnumerator CoRespawnAfterTime(string name, int playerNumber) {
		yield return new WaitForSeconds(4f);
		Respawn(null, name, playerNumber);
	}

	void OnGUI() {
		//GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
	}
}
