using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
	public float exp;
	public Animator anim;
	public Vector3 direction;
	public float bulletSpeed;
	private Rigidbody rg;
	public float head;
	private bool isTriggered = false;
	private void FixedUpdate()
	{
		if (!isTriggered && direction != Vector3.zero)
		{
			transform.position += direction * Time.deltaTime * bulletSpeed;
			//transform.LookAt(direction, Vector3.forward);
			transform.rotation = Quaternion.LookRotation (direction, Vector3.up);
			Debug.DrawLine (transform.position, transform.position + transform.forward, Color.red);
		}
	}
	private void OnTriggerEnter(Collider other)
	{
		var enemyBehaviour = other.transform.root.GetComponentInChildren<EnemyBehaviour> ();
		if (!enemyBehaviour || !other.gameObject.GetComponent<Rigidbody> ())
		{
			return;
		}
		//Debug.LogError (other.transform.root.GetChild (0).name);
		var rg = other.transform.gameObject.GetComponent<Rigidbody> ();
		var forc = 0.0f;
		if (other.gameObject.name =="Head")
		{
			forc = head;
			enemyBehaviour.OnTakeDamage (true);
		}
		else
		{
			forc = exp;
			enemyBehaviour.OnTakeDamage (false);
		}
		rg.AddForce (other.transform.forward * forc, ForceMode.Impulse);
		//Debug.LogError (other.transform.gameObject.name);
		isTriggered = true;
		GetComponent<Collider> ().enabled = false;
		Destroy (GetComponent<Rigidbody> ());
		transform.parent = other.transform.parent;
	}
}
