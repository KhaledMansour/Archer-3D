using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerController : MonoBehaviour
{
	public GameObject bullet;
	public float forceSpeed;
	public float cameraSpeed;
	private Transform target;
	public float moveSpeed;
	public List<Transform> movePoints;
	public float fireRate=0.2f;
	private float currentFireRate=0;

	void Start()
	{
		checkForNearestEnemy ();
	}

	void Update()
	{
		currentFireRate += Time.deltaTime;
		if (!bullet.activeSelf && currentFireRate >= fireRate)
		{
			bullet.SetActive (true);
		}
		if (Input.GetMouseButtonDown (0) && bullet.activeSelf)
		{
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			var bul = Instantiate (bullet, bullet.transform.position, Quaternion.identity);
			var rb = bul.GetComponent<Rigidbody> ();
			bul.GetComponent<BulletBehaviour> ().direction = ray.direction;
			rb.AddForce (ray.direction * forceSpeed, ForceMode.Force);
			bullet.gameObject.SetActive (false);
			currentFireRate = 0;
		}
		if (GameManager.instance._levelState.gameState == GameState.MoveToState)
		{
			if (GameManager.instance._levelState.statePoints.Count  == 0)
			{
				return;
			}
			if (transform.position != GameManager.instance._levelState.statePoints[0].position)
			{
				transform.position = Vector3.MoveTowards (transform.position, GameManager.instance._levelState.statePoints[0].position, Time.deltaTime * moveSpeed);
				var rotation = Quaternion.LookRotation (GameManager.instance._levelState.statePoints[0].position);
				transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * cameraSpeed);

			}
			else
			{
				GameManager.instance._levelState.OnRemovePoint (0);
				if (GameManager.instance._levelState.gameState == GameState.StartNewState)
				{
					GameManager.instance.OnMovingToNextState ();
				}
			}
		}
		if (GameManager.instance._levelState.gameState == GameState.Attacking && target != null)
		{
			var lookPos = target.transform.position - transform.position;
			lookPos.y = 0;
			var rotation = Quaternion.LookRotation (lookPos);
			transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * cameraSpeed);
		}
	}

	public void checkForNearestEnemy()
	{
	
		target = GameManager.instance._levelState.GetNearestEnemyToPlayer(transform.position);
	}
}
