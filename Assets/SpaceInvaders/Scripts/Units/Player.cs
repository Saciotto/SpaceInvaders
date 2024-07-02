using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 5f;

    private float _horizontal = 0;
    private Rigidbody2D _body;

    private void Start()
    {
        _body = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");
    }

    private void FixedUpdate()
    {
        if (_horizontal != 0) {
            _body.MovePosition(_body.position + Vector2.right * _horizontal * _speed * Time.fixedDeltaTime);
        }
    }
}
