using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private Projectile _projectilePrefab;

    private float _horizontal = 0;
    private Rigidbody2D _body;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Playing) {
            _horizontal = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space)) {
                Projectile projectile = Instantiate(_projectilePrefab, transform.position, Quaternion.identity);
                projectile.Direction = 1;
            }   
        }
    }

    private void FixedUpdate()
    {
        if (_horizontal != 0) {
            _body.MovePosition(_body.position + Vector2.right * _horizontal * _speed * Time.fixedDeltaTime);
        }
    }
}
