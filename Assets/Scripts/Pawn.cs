using UnityEngine;
using System.Collections;
using System.Linq;

public class Pawn : Photon.MonoBehaviour {

	public float Damage = 0;
	public string Name = "Yeti";

	public int LocalPlayerNumber = 0;

	private float recoveryDuration = 0;
	private float recoveryStartTime = 0;
	private float hitstunDuration = 0;
	private float hitstunStartTime = 0;

	private PawnMovement pawnMovement;

	public DamageTrigger BasicAttackTrigger;
	public DamageTrigger UpAttackTrigger;
	public DamageTrigger AirAttackTrigger;

	void Start() {
		FindObjectOfType<PlayerPanelGrid>().FillEmptyPanel(this);
		pawnMovement = GetComponent<PawnMovement>();
    }
	
	// Update is called once per frame
	void Update () {
		if (!photonView.isMine)
			return;

		// Check if player is out of bounds
		if(BoundsManager.CheckBounds(transform.position)) {
			PhotonNetwork.RPC(photonView, "Die", PhotonTargets.All, false, transform.position);
			
			NetworkInitializer.RespawnAfterTime(Name, LocalPlayerNumber);
			PhotonNetwork.Destroy(gameObject);
		}

		if(Input.GetButtonDown("Basic Attack" + LocalPlayerNumber) && recoveryStartTime + recoveryDuration < Time.time && hitstunStartTime + hitstunDuration < Time.time) {
			BasicAttack();
		}

		// Cycle through characters
		if (Input.GetButtonDown("ChangeCharacter" + LocalPlayerNumber)) {
			var currentIndex = NetworkInitializer.Characters.IndexOf(Name);
            var nextCharacter = NetworkInitializer.Characters.Count > currentIndex + 1 ? NetworkInitializer.Characters[currentIndex + 1] : NetworkInitializer.Characters[0];
			NetworkInitializer.Respawn(this, nextCharacter, LocalPlayerNumber);
		}
	}

	public virtual void BasicAttack() {
		if (Input.GetAxis("Vertical" + LocalPlayerNumber) > 0.1f) {
			pawnMovement.Animator.SetTrigger("UpAttack");
			PhotonNetwork.RPC(photonView, "BasicAttackAnimation", PhotonTargets.Others, false, "UpAttack");
            recoveryDuration = 0.6f;

			StartCoroutine(StartAttack(UpAttackTrigger));
		} else if(pawnMovement.Animator.GetBool("Falling")) {
			pawnMovement.Animator.SetTrigger("AirAttack");
			PhotonNetwork.RPC(photonView, "BasicAttackAnimation", PhotonTargets.Others, false, "AirAttack");
			recoveryDuration = 0.4f;

			StartCoroutine(StartAttack(AirAttackTrigger));
		} else {
			pawnMovement.Animator.SetTrigger("BasicAttack");
			PhotonNetwork.RPC(photonView, "BasicAttackAnimation", PhotonTargets.Others, false, "BasicAttack");
			recoveryDuration = 0.4f;

			StartCoroutine(StartAttack(BasicAttackTrigger));
		}

		recoveryStartTime = Time.time;
    }

	IEnumerator StartAttack(DamageTrigger trigger) {
		yield return new WaitForSeconds(trigger.StartUpTime);
		trigger.gameObject.SetActive(true);
	}

	[PunRPC]
	public virtual void BasicAttackAnimation(string triggerName) {
		pawnMovement.Animator.SetTrigger(triggerName);
	}

	[PunRPC]
	public void Hit(Vector3 direction, float power, float damage, float hitstun) {
		Damage += damage;
		hitstunDuration = hitstun;
		hitstunStartTime = Time.time;
		
		if(direction == Vector3.up) {
			pawnMovement.Velocity.y = 0;
		}
		pawnMovement.Velocity += direction * power * Damage;
	}

	[PunRPC]
	public virtual void SpecialAttack() {

	}

	[PunRPC]
	public void Die(Vector3 position) {
		Damage = 0;
		pawnMovement.Velocity = Vector3.zero;

		var boundsManager = BoundsManager.Instance;
		Instantiate(boundsManager.OutOfBoundsEffect, transform.position + new Vector3(0, 0, -2), Quaternion.LookRotation(Vector3.zero - transform.position + new Vector3(0, 0, -2)));
	}
}
