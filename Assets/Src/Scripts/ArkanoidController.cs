using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ArkanoidController : MonoBehaviour
{
  private const string BALL_PREFAB_PATH = "Prefabs/Ball";
  private readonly Vector2 BALL_INIT_POSITION = new Vector2(0, -0.86f);

  private bool _onPowerUp = false;
  [SerializeField]
  [Range(0, 100)]
  private float _powerUpSpawProbability = 100; // Percentage
  [SerializeField]
  private float _powerUpDuration = 5; // Seconds
  private float _powerUpTimer = 0;
  public float PowerUpTimer => _powerUpTimer;
  private PowerUpType _powerUpType = PowerUpType.SlowPaddle;

  [SerializeField]
  private Paddle _paddle;
  private Ball _ballPrefab = null;
  private List<Ball> _balls = new List<Ball>();

  [SerializeField]
  private GridController _gridController;

  [Space(20)]
  [SerializeField]
  private List<LevelData> _levels = new List<LevelData>();

  [SerializeField]
  private UIPowerUp powerUpUI = null;

  private int _currentLevel = 0;
  private int _totalScore = 0;

  private int[] Points = { 50, 100, 250, 500 };

  private void InitGame()
  {
    _currentLevel = 0;
    _totalScore = 0;
    _gridController.BuildGrid(_levels[0]);
    SetInitialBall();
    ArkanoidEvent.OnGameStartEvent?.Invoke();
    ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(0, _totalScore);
  }
  private void Start()
  {
    ArkanoidEvent.OnBallReachDeadZoneEvent += OnBallReachDeadZone;
    ArkanoidEvent.OnBlockDestroyedEvent += OnBlockDestroyed;
    ArkanoidEvent.OnPowerUpEvent += OnPowerUp;
  }
  private void Update()
  {
    if (Input.GetKeyDown(KeyCode.Space))
    {
      InitGame();
    }

    if (_onPowerUp)
    {
      if (_powerUpTimer > 0)
      {
        _powerUpTimer -= Time.deltaTime;
        powerUpUI.SetTimer(_powerUpTimer / _powerUpDuration);
      }
      else
      {
        _onPowerUp = false;
        ArkanoidEvent.OnEndPowerUpEvent?.Invoke();
        switch (_powerUpType)
        {
          case PowerUpType.SlowPaddle:
            _paddle.SetSpeed(_paddle.InitialSpeed);
            break;
          case PowerUpType.FastPaddle:
            _paddle.SetSpeed(_paddle.InitialSpeed);
            break;
        }
      }
    }
  }
  private void OnDestroy()
  {
    ArkanoidEvent.OnBallReachDeadZoneEvent -= OnBallReachDeadZone;
    ArkanoidEvent.OnBlockDestroyedEvent -= OnBlockDestroyed;
    ArkanoidEvent.OnPowerUpEvent -= OnPowerUp;
  }
  private void SetInitialBall()
  {
    ClearBalls();
    Ball ball = CreateBallAt(BALL_INIT_POSITION);
    ball.Init();
    _balls.Add(ball);
  }
  private Ball CreateBallAt(Vector2 position)
  {
    if (_ballPrefab == null)
    {
      _ballPrefab = Resources.Load<Ball>(BALL_PREFAB_PATH);
    }
    return Instantiate(_ballPrefab, position, Quaternion.identity);
  }
  private void ClearBalls()
  {
    for (int i = _balls.Count - 1; i >= 0; i--)
    {
      _balls[i].gameObject.SetActive(false);
      Destroy(_balls[i]);
    }

    _balls.Clear();
  }
  private void OnBallReachDeadZone(Ball ball)
  {
    ball.Hide();
    _balls.Remove(ball);
    Destroy(ball.gameObject);

    CheckGameOver();
  }
  private void CheckGameOver()
  {
    //Game over
    if (_balls.Count == 0)
    {
      ClearBalls();

      Debug.Log("Game Over: LOSE!!!");
      ArkanoidEvent.OnGameOverEvent?.Invoke();
    }
  }
  private void OnBlockDestroyed(int blockId)
  {
    BlockTile blockDestroyed = _gridController.GetBlockBy(blockId);
    if (blockDestroyed != null)
    {
      float probability = Random.Range(0f, 1f);
      if (probability > 1 - (_powerUpSpawProbability / 100))
      {
        SpawnPowerUp(blockDestroyed.transform.position);
      }
      _totalScore += blockDestroyed.Score;
      ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(blockDestroyed.Score, _totalScore);
    }
    if (_gridController.GetBlocksActive() == 0)
    {
      _currentLevel++;
      if (_currentLevel >= _levels.Count)
      {
        ClearBalls();
        ArkanoidEvent.OnGameOverEvent?.Invoke();
        Debug.LogError("Game Over: WIN!!!!");
      }
      else
      {
        SetInitialBall();
        _gridController.BuildGrid(_levels[_currentLevel]);
        ArkanoidEvent.OnLevelUpdatedEvent?.Invoke(_currentLevel);
      }
    }
  }
  void SpawnPowerUp(Vector2 position)
  {
    PowerUpType powerUpType = (PowerUpType)Random.Range(0, System.Enum.GetNames(typeof(PowerUpType)).Length);

    int points = 0;
    if (powerUpType == PowerUpType.Points)
    {
      points = Points[Random.Range(0, Points.Length)];
    }

    PowerUp powerUpPrefab = Resources.Load<PowerUp>("Prefabs/PowerUp");
    PowerUp powerUp = Instantiate<PowerUp>(powerUpPrefab, position, Quaternion.identity);
    powerUp.SetData(powerUpType, points);
    powerUp.Init();
  }
  private void OnPowerUp(PowerUp powerUp)
  {
    switch (powerUp.Type)
    {
      case PowerUpType.SlowPaddle:
        _onPowerUp = true;
        _powerUpTimer = _powerUpDuration;
        _powerUpType = powerUp.Type;
        _paddle.SetSpeed(0.5f);
        break;
      case PowerUpType.FastPaddle:
        _onPowerUp = true;
        _powerUpTimer = _powerUpDuration;
        _powerUpType = powerUp.Type;
        _paddle.SetSpeed(20f);
        break;
      case PowerUpType.MultiBalls:
        while (_balls.Count < 3)
        {
          Ball ball = CreateBallAt(_balls[0].transform.position);
          ball.Init();
          _balls.Add(ball);
        }

        break;
      case PowerUpType.Points:
        _totalScore += powerUp.Points;
        ArkanoidEvent.OnScoreUpdatedEvent?.Invoke(powerUp.Points, _totalScore);
        break;
    }
  }
}