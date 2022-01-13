using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUpType
{
  SlowPaddle,
  FastPaddle,
  MultiBalls,
  Points
}
public class PowerUp : MonoBehaviour
{
  private const string BLOCK_BIG_PATH = "Sprites/PowerUps/{0}{1}";
  private PowerUpType _type = PowerUpType.SlowPaddle;
  private int _points = 0;

  public PowerUpType Type => _type;
  public int Points => _points;

  private SpriteRenderer _renderer;
  private Collider2D _collider;

  public void Init()
  {
    _collider = GetComponent<Collider2D>();
    _collider.enabled = true;

    _renderer = GetComponentInChildren<SpriteRenderer>();
    _renderer.sprite = GetPowerUpSprite(_type, _points);
  }
  static Sprite GetPowerUpSprite(PowerUpType type, int points)
  {
    string path = string.Empty;

    if (type == PowerUpType.Points)
    {
      path = string.Format(BLOCK_BIG_PATH, type, points);
    }
    else
    {
      path = string.Format(BLOCK_BIG_PATH, type, "");
    }

    if (string.IsNullOrEmpty(path))
    {
      return null;
    }

    return Resources.Load<Sprite>(path);
  }
  public void SetData(PowerUpType type, int points)
  {
    _points = points;
    _type = type;
  }
  private void OnTriggerEnter2D(Collider2D col)
  {
    if (col.gameObject.name == "Paddle")
    {
      _collider.enabled = false;
      gameObject.SetActive(false);
      ArkanoidEvent.OnPowerUpEvent?.Invoke(this);
    }

    if (col.gameObject.name == "BottomWall")
    {
      _collider.enabled = false;
      gameObject.SetActive(false);
    }
  }
}
