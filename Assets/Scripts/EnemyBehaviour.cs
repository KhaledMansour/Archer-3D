using UnityEngine;

public enum EnemyState
{
	Idle, Attacking, Dead
}
public class EnemyBehaviour : MonoBehaviour
{
	[SerializeField] private ConfigurableJoint hipJoint;
	[SerializeField] private Rigidbody hip;
	public Rigidbody _hip { get { return hip; }  }
	private Rigidbody rb;
	public EnemyState enemyState { get; private set; }
	[SerializeField] private Animator targetAnimator;
	public GameObject target;
	public float movingSpeed;
	private static int maxEnemyHits = 4;
	public int currentEnemyHealth { get; private set; }
	void Awake()
	{
		rb = GetComponent<Rigidbody> ();
		enemyState = EnemyState.Idle;
	}

	// Update is called once per frame
	void Update()
	{
		if (enemyState == EnemyState.Attacking)
		{
			var directionn = target.transform.position - this.hip.position;
			directionn.y = 0;
			this.hip.position += directionn * movingSpeed * Time.deltaTime;
			this.targetAnimator.SetBool ("Walk", true);
			float targetAngle = Mathf.Atan2 (directionn.z, directionn.x) * Mathf.Rad2Deg;
			this.hipJoint.targetRotation = Quaternion.Euler (0f, targetAngle, 0f);
		}
		else
		{
			this.targetAnimator.SetBool ("Walk", false);
		}
	}
	public void OnTakeDamage(bool isHead)
	{
		if (enemyState == EnemyState.Dead)
		{
			return;
		}
		movingSpeed = 0;
		hip.mass = 100;
		hip.velocity = Vector3.zero ;
		Invoke ("Reset", 1f);
		if (isHead)
		{
			currentEnemyHealth = maxEnemyHits;
		}
		else
		{
			currentEnemyHealth++;
		}
		if (currentEnemyHealth >= maxEnemyHits)
		{
			Time.timeScale = 0.4f;
			enemyState = EnemyState.Dead;
			Invoke ("OnDie", 1f);
		}
	}
	private void Reset()
	{
		if (enemyState == EnemyState.Dead)
		{
			return;
		}
		GameManager.instance._playerController.checkForNearestEnemy ();
		movingSpeed = 1;
		hip.mass = 1;

	}
	public void OnDie()
	{
		rb.isKinematic = false;
		rb.useGravity = false;
		Time.timeScale = 1f;
		this.targetAnimator.SetBool ("Walk", false);
		GameManager.instance.OnEnemyDie (this);
	}
	public void StartEnemyFight()
	{
		enemyState = EnemyState.Attacking;
	}
}
