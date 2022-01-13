using UnityEngine;
public class Paddle : MonoBehaviour
{
  [SerializeField]
  private float _speed = 5;
  [SerializeField]
  private float _initialSpeed = 0;
  private float _movementLimit = 7;

  public float Speed => _speed;
  public float InitialSpeed => _initialSpeed;

  private Vector3 _targetPosition;
  private Camera _cam;
  private Camera Camera
  {
    get
    {
      if (_cam == null)
      {
        _cam = Camera.main;
      }
      return _cam;
    }
  }

  private void Start()
  {
    _initialSpeed = _speed;
  }

  void Update()
  {
    _targetPosition.x = Camera.ScreenToWorldPoint(Input.mousePosition).x;
    _targetPosition.x = Mathf.Clamp(_targetPosition.x, -_movementLimit, _movementLimit);
    _targetPosition.y = this.transform.position.y;

    transform.position = Vector3.Lerp(transform.position, _targetPosition, Time.deltaTime * _speed);
  }

  public void SetSpeed(float speed)
  {
    _speed = speed;
  }
}
