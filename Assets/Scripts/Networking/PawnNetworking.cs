using UnityEngine;
using System.Collections;

public class PawnNetworking : Photon.MonoBehaviour {

	private Vector3 correctPlayerPos;
	private Quaternion correctPlayerRot;

	private PawnMovement pawnMovement;

	public Vector3 positionAtLastPacket = Vector3.zero;
	public double currentTime = 0.0;
	public double currentPacketTime = 0.0;
	public double lastPacketTime = 0.0;
	public double timeToReachGoal = 0.0;

	void Awake() {
		pawnMovement = GetComponent<PawnMovement>();
    }

	// TODO: Send velocity to be able to predict movement
	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
		if(stream.isWriting) {
			stream.SendNext(transform.position);
			stream.SendNext(transform.localScale);
			stream.SendNext(pawnMovement.Velocity);
		} else {
			currentTime = 0.0;
			positionAtLastPacket = transform.position;
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			
			transform.localScale = (Vector3)stream.ReceiveNext();
			pawnMovement.Velocity = (Vector3)stream.ReceiveNext();

			lastPacketTime = currentPacketTime;
			currentPacketTime = info.timestamp;
		}
	}

	void Update() {
		// Smooth movement for remote players
		if(!photonView.isMine) {
			timeToReachGoal = currentPacketTime - lastPacketTime;
			currentTime += Time.deltaTime;

			transform.position = Vector3.Lerp(positionAtLastPacket, correctPlayerPos, (float)(currentTime / timeToReachGoal));
		}
	}
}
