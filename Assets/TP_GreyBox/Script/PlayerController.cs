using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private FreeFollowView _followView;

	public float speed = 10.0f;

	Rigidbody _rigidbody = null;
	protected bool IsActive { get; private set; }

	public void Awake()
	{
		_rigidbody = GetComponent<Rigidbody>();
	}

	void FixedUpdate()
    {
		//Movement
		Vector3 direction = Vector3.zero;
		direction += Input.GetAxisRaw("Horizontal") * Vector3.right;
		direction += Input.GetAxisRaw("Vertical") * Vector3.forward;
        direction = Camera.main.transform.TransformDirection(direction);
        direction.y = 0; // pour éviter de voler
        direction.Normalize();

        float cameraSpeed = speed * Time.fixedDeltaTime;
		_rigidbody.AddForce(direction * cameraSpeed);

		//Camera
		Vector3 mouseMovement = Vector2.zero;
		mouseMovement += Input.GetAxisRaw("Mouse X") * Vector3.right;
        mouseMovement += Input.GetAxisRaw("Mouse Y") * Vector3.up;
		mouseMovement.Normalize();
		_followView.OnCameraInputs(new Vector2(mouseMovement.x, mouseMovement.y), Time.fixedDeltaTime);


		//Saut
		if (Input.GetButton("Jump"))
		{
			_rigidbody.AddForce(Vector3.up * 50 * Time.fixedDeltaTime, ForceMode.Impulse);
		}
    }
}
