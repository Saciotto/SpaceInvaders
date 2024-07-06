using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private bool _destroy = false;
    private float _destroyTimer = 0.0f;
    private Transform _topLimit;
    private Transform _bottomLimit;
    private float _speed;

    public int Direction;
    public float Speed;

    private void Start()
    {
        _speed = Speed;
        _topLimit = GameManager.Instance.TopLimit;
        _bottomLimit = GameManager.Instance.BottomLimit;
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + Vector2.up * _speed * Direction * Time.fixedDeltaTime);

        if (transform.position.y > _topLimit.position.y && !_destroy) {
            _speed = 0;
            _destroy = true;
            _destroyTimer = 0.0f;
            GetComponent<Animator>().SetBool("Destroy", true);
        }

        if (transform.position.y < _bottomLimit.position.y) {
            Destroy(gameObject);
        }

        if (_destroy) {
            _destroyTimer += Time.fixedDeltaTime;
            if (_destroyTimer > 0.5f) {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) {
            EnemiesManager.Instance.Kill(collision.gameObject);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.SetGameState(GameState.GameOver);
        }
    }
}
