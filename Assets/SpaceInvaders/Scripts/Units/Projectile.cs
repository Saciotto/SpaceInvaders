using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Projectile : MonoBehaviour
{
    private bool _destroy = false;
    private float _destroyTimer = 0.0f;
    private Transform _topLimit;
    private Transform _bottomLimit;
    private float _speed;

    public int Direction;
    public float Speed;
    public string Origin;

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
            ScheduleDestruction();
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

    private void ScheduleDestruction()
    {         
        _speed = 0;
        _destroy = true;
        _destroyTimer = 0.0f;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<Animator>().SetBool("Destroy", true);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(Origin)) {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        } else if (collision.gameObject.CompareTag("Enemy")) {
            EnemiesManager.Instance.Kill(collision.gameObject);
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Player")) {
            GameManager.Instance.SetGameState(GameState.Die);
            Destroy(gameObject);
        } else if (collision.gameObject.CompareTag("Shield")) {
            ScheduleDestruction();
            Tilemap tilemap = collision.gameObject.GetComponent<Tilemap>();
            Vector3 hitPosition = Vector3.zero;
            foreach (ContactPoint2D hit in collision.contacts) {
                hitPosition.x = hit.point.x - 0.01f * hit.normal.x;
                hitPosition.y = hit.point.y - 0.01f * hit.normal.y;
                Vector3Int hitCell = tilemap.WorldToCell(hitPosition);
                tilemap.SetTile(hitCell, null);
                Vector3Int[] neighbors = {
                    new Vector3Int(hitCell.x + 1, hitCell.y, hitCell.z),
                    new Vector3Int(hitCell.x - 1, hitCell.y, hitCell.z),
                    new Vector3Int(hitCell.x, hitCell.y + 1, hitCell.z),
                    new Vector3Int(hitCell.x, hitCell.y - 1, hitCell.z),
                };
                foreach (Vector3Int neighbor in neighbors) {
                    tilemap.SetTile(neighbor, null);
                }
            }
        } else if (collision.gameObject.CompareTag("Projectile")){
            if (Origin == "Player") {
                ScheduleDestruction();
            } else {
                Destroy(gameObject);
            }
        } else {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        }
    }
}
