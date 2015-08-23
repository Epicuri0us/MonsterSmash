using UnityEngine;
using System.Collections;
using Prime31;

public class PawnMovement : Photon.MonoBehaviour {

	public float Gravity = -25f;
	public float MoveSpeed = 8f;
	public float GroundDirectionChangeSpeed = 20f;
	public float AirDirectionChangeSpeed = 5f;
	public float JumpHeight = 3f;

	private bool DidAirJump = false;

	public Animator Animator;

	public enum MovementState {
		Idle,
		Walking,
		Falling
	}

	public MovementState State = MovementState.Idle;

	protected CharacterController2D characterController;

	private Pawn pawn;

	[HideInInspector]
	public bool ControlledLocally = false;

	[HideInInspector]
	public Vector3 Velocity;

	private float normalizedHorizontalSpeed = 0;

	void Awake() {
		pawn = GetComponent<Pawn>();

		characterController = GetComponent<CharacterController2D>();

		// Set controller collision delegates
		characterController.onControllerCollidedEvent += OnContollerCollider;
    }

	void OnContollerCollider(RaycastHit2D hit) {
		/*if(hit.normal.y == 1f) {
			return;
		}*/
	}
	
	void Update () {
		if (characterController.isGrounded)
			DidAirJump = false;
		
		if (characterController.isGrounded && Velocity.y <= 0)
			Velocity.y = 0;

		if (ControlledLocally) {
			// Horizontal movement
			var horizontal = Input.GetAxis("Horizontal" + pawn.LocalPlayerNumber);
			if (horizontal > 0f) {
				normalizedHorizontalSpeed = 1;

				if (transform.localScale.x < 0f)
					transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

				// TODO: Play run animation

			} else if (horizontal < 0f) {
				normalizedHorizontalSpeed = -1;

				if (transform.localScale.x > 0f)
					transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

				// TODO: Play run animation

			} else {
				normalizedHorizontalSpeed = 0;

				// TODO: Play idle animation
			}
		}

		// Jump when grounded
		if (ControlledLocally && (characterController.isGrounded || DidAirJump == false) && Input.GetButtonDown("Jump" + pawn.LocalPlayerNumber)) {
			if (!characterController.isGrounded)
				DidAirJump = true;

			// Adjust vertical velocity to jump height using gravity
			Velocity.y = Mathf.Sqrt(2f * JumpHeight * -Gravity);

			// TODO: Play jump animation
		}

		// TODO: Horizontal spped smoothin
		var smoothedMovementFactor = characterController.isGrounded ? GroundDirectionChangeSpeed : AirDirectionChangeSpeed;
		Velocity.x = Mathf.Lerp(Velocity.x, normalizedHorizontalSpeed * MoveSpeed, smoothedMovementFactor * Time.deltaTime);

		// Apply gravity
		Velocity.y += Gravity * Time.deltaTime;

		// Fall through one-way platforms when holding down
		// TODO: Fiddle around with values, fix high jumps etc.
		if(ControlledLocally && characterController.isGrounded && Input.GetAxis("Vertical" + pawn.LocalPlayerNumber) < -0.5f) {
			//Velocity.y *= 3f;
			characterController.ignoreOneWayPlatformsThisFrame = true;
		}

		characterController.move(Velocity * Time.deltaTime);
		
		Animator.SetFloat("MoveSpeedX", Mathf.Abs(Velocity.x));
		Animator.SetBool("Falling", !characterController.isGrounded);

		Velocity = characterController.velocity;
	}
}
