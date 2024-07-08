using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _cooldownTime = 1f;
    [SerializeField] private Projectile _projectilePrefab;
    [SerializeField] private float _projectileSpeed = 10f;

    private float _horizontal = 0;
    private Rigidbody2D _body;

    private bool _cooldown = false;
    private float _cooldownTimer = 0.0f;

    private void Awake()
    {
        GameManager.Instance.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_cooldown) {
            _cooldownTimer += Time.deltaTime;
            if (_cooldownTimer > _cooldownTime) {
                _cooldown = false;
                _cooldownTimer = 0.0f;
            }
        }

        if (GameManager.Instance.CurrentGameState == GameState.Playing) {
            _horizontal = Input.GetAxis("Horizontal");

            if (Input.GetKeyDown(KeyCode.Space) && !_cooldown) {
                float y = GetComponent<SpriteRenderer>().bounds.size.y / 2 + _projectilePrefab.GetComponent<SpriteRenderer>().bounds.size.y;
                Projectile projectile = Instantiate(_projectilePrefab, transform.position + Vector3.up * y, Quaternion.identity);
                projectile.Speed = _projectileSpeed;
                projectile.Direction = 1;
                projectile.Origin = gameObject.tag;
                _cooldown = true;
            }   
        }
    }

    private void FixedUpdate()
    {
        if (_horizontal != 0) {
            _body.MovePosition(_body.position + Vector2.right * _horizontal * _speed * Time.fixedDeltaTime);
        }
    }

    private void OnGameStateChanged(GameState newState)
    {
        if (newState == GameState.GameOver) {
            Destroy(gameObject);
        }
    }
}
