using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
[System.Serializable]
public class levelState
{
	public GameState gameState { get;private set; }
	public List<Transform> statePoints;
	public int enemyCount;
	public List<EnemyBehaviour> enemyBehaviours;
	public void OnRemovePoint(int pointIndex)
	{
		if (pointIndex < statePoints.Count)
		{
			statePoints.RemoveAt (pointIndex);
			if (statePoints.Count == 0)
			{
				gameState = GameState.StartNewState;
			}
		}
	}
	public void OnRemoveEnemy(EnemyBehaviour enemyBehaviour)
	{
		enemyBehaviours.Remove (enemyBehaviour);
		gameState = enemyBehaviours.Count > 0 ? GameState.Attacking : GameState.MoveToState;
	}
	public Transform GetNearestEnemyToPlayer(Vector3 playerPosition)
	{
		if (enemyBehaviours.Count == 0)
		{
			return null;
		}
		var nearestEnemy = enemyBehaviours[0]._hip;
		for (int i = 1; i < enemyBehaviours.Count; i++)
		{
			if ((enemyBehaviours[i]._hip.position - playerPosition).sqrMagnitude
				< (nearestEnemy.position - playerPosition).sqrMagnitude)
			{
				nearestEnemy = enemyBehaviours[i]._hip;
			}
		}
		return nearestEnemy.transform;
	}
	public void OnStartNewState()
	{
		gameState = GameState.Attacking;
		enemyBehaviours.ForEach (x => x.StartEnemyFight ());
	}
}

public enum GameState
{
	Attacking, MoveToState, StartNewState
}
public class GameManager : MonoBehaviour
{
	public static GameManager instance;
	[SerializeField]
	private List<levelState> levelStates;
	[SerializeField]
	private PlayerController playerController;
	[SerializeField]
	private GameObject winMenu;
	private int stateIndex = 0;
	public PlayerController _playerController { get { return playerController; } }
	public levelState _levelState { get { return levelStates[Mathf.Clamp(stateIndex, 0, levelStates.Count-1) ]; } }
    void Start()
    {
		if (instance == null)
		{
			instance = this;
		} else
		{
			Destroy (this);
		}
		_levelState.OnStartNewState ();
    }
	public void OnEnemyDie(EnemyBehaviour enemy)
	{
		_levelState.OnRemoveEnemy (enemy);
		if (_levelState.gameState == GameState.MoveToState && stateIndex >= levelStates.Count-1)
		{
			ShowWinMenu ();
		}
		playerController.checkForNearestEnemy ();
	}
	public void OnMovingToNextState()
	{
		stateIndex++;
		playerController.checkForNearestEnemy ();
		_levelState.OnStartNewState ();
	}

	public void LevelRestart()
	{
		Time.timeScale = 1;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}

	public void ShowWinMenu()
	{
		Time.timeScale = 0;
		winMenu.SetActive (true);
	}
}
