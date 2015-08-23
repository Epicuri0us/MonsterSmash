using UnityEngine;
using System.Collections;

public class DamageTrigger : Photon.MonoBehaviour {

	private int PlayerLayer;

	public float StartUpTime = 0.1f;
	public float HitDamage = 0.1f;
	public float HitPower = 80f;
	public float Hitstun = 0.45f;
	public Vector3 HitDirection = new Vector3(1, 0, 0);
	public bool RespectPlayerDirection = true;
	public float ActiveDuration = 0.2f;

	public Transform HitEffectPrefab;

	void Start() {
		PlayerLayer = LayerMask.NameToLayer("Player");
    }

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.layer == PlayerLayer && collider.transform.root != transform.root) {
			PhotonNetwork.RPC(collider.GetComponent<PhotonView>(), "Hit", PhotonTargets.All, false, RespectPlayerDirection ? Mathf.Sign(transform.root.localScale.x) * new Vector3(HitDirection.x, 0, 0) + new Vector3(0, HitDirection.y, 0) : HitDirection, HitPower, HitDamage, Hitstun);
			PhotonNetwork.RPC(photonView, "HitEffect", PhotonTargets.All, false, transform.position + new Vector3(0, 0, -1), transform.rotation);
		}
	}

	[PunRPC]
	public void HitEffect(Vector3 position, Quaternion rotation) {
		Instantiate(HitEffectPrefab, position, rotation);
	}

	IEnumerator DisableAfterTime(float time) {
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
	}

	void OnEnable() {
		StartCoroutine(DisableAfterTime(ActiveDuration));
	}
}
