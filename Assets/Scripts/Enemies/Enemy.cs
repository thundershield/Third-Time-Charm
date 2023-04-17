using System;
using UnityEngine;
using Random = System.Random;

namespace Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class Enemy : MonoBehaviour
    {
        private const float FrameLength = 0.2f;
        private const float DirectionChangeTime = 1.5f;
        private const float MoveSpeed = 2.0f;
        private const int MaxHealth = 50;

        private static readonly Random Random = new();
        
        [SerializeField] private Sprite[] walkDownSprites;
        [SerializeField] private Sprite[] walkLeftSprites;
        [SerializeField] private Sprite[] walkRightSprites;
        [SerializeField] private Sprite[] walkUpSprites;
        
        private SpriteRenderer _spriteRenderer;
        private float _animationTimer;
        private float _directionChangeTimer;
        private Direction _direction;
        private Vector2 _directionVec;
        private Rigidbody2D _rigidbody2D;
        private int _health = MaxHealth;
        
        private void Start()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _rigidbody2D = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            _animationTimer += Time.deltaTime;
            _directionChangeTimer += Time.deltaTime;

            if (_directionChangeTimer > DirectionChangeTime)
            {
                _directionChangeTimer = 0f;
                ChangeDirection();
            }

            var sprites = GetSpritesForDirection(_direction);
            var frame = Mathf.FloorToInt(_animationTimer / FrameLength) % sprites.Length;
            _spriteRenderer.sprite = sprites[frame];
        }

        private void FixedUpdate()
        {
            _rigidbody2D.velocity = _directionVec * MoveSpeed;
        }

        private void ChangeDirection()
        {
            _direction = (Direction)Random.Next(4);
            _directionVec = _direction switch
            {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                _ => throw new ArgumentOutOfRangeException(nameof(_direction), _direction, null)
            };
        }

        private Sprite[] GetSpritesForDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => walkUpSprites,
                Direction.Down => walkDownSprites,
                Direction.Left => walkLeftSprites,
                Direction.Right => walkRightSprites,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
            };
        }

        public void TakeDamage(int damage)
        {
            _health -= damage;
            Debug.Log(_health);

            if (_health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}