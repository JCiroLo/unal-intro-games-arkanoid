using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPowerUp : MonoBehaviour
{
  private Slider _slider;
  private CanvasGroup _canvasGroup;
  void Start()
  {
    _canvasGroup = GetComponent<CanvasGroup>();
    _slider = GetComponent<Slider>();
    _slider.value = 0f;
    _canvasGroup.alpha = 0;

    ArkanoidEvent.OnPowerUpEvent += OnPowerUp;
    ArkanoidEvent.OnEndPowerUpEvent += OnEndPowerUp;
  }
  private void OnPowerUp(PowerUp powerUp)
  {
    if (powerUp.Type == PowerUpType.FastPaddle || powerUp.Type == PowerUpType.SlowPaddle)
    {
      _canvasGroup.alpha = 1;
    }
  }
  private void OnEndPowerUp()
  {
    _canvasGroup.alpha = 0;
  }
  private void OnDestroy()
  {
    ArkanoidEvent.OnPowerUpEvent -= OnPowerUp;
    ArkanoidEvent.OnEndPowerUpEvent -= OnEndPowerUp;
  }

  public void SetTimer(float time)
  {
    _slider.value = time;
  }
}