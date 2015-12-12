﻿using UnityEngine;
using System.Collections;

public class MobMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Vector2 _forces = Vector2.zero;
    private bool _shouldStop = false;
    private bool _isStopping = false;
    private bool _isGrounded = false;
    private bool _isJumping = false;
    private bool _isDownJumping = false;
    private RaycastHit2D _hittedGround;
    private RaycastHit2D _savedHit = new RaycastHit2D();
    private RaycastHit2D _savedUpperHit = new RaycastHit2D();
    private GameObject _monster;
    private MonsterFX _monsterFx;
    private Collider2D _monsterCollider;

    private const float RAYCAST_DOWN_DISTANCE = 0.6f;
    private const int GROUND_MASK = 1 << 8;
    private const float POSITIVE_X_ACCELERATION = 100.0f;
    private const float NEGATIVE_X_ACCELERATION = -20.0f;
    private const float POSITIVE_Y_ACCELERATION = 1500f;
    private const float GRAVITY_FORCE = 50f;
    private const float MAX_X_FORCE = 400.0f;
    private const float STOP_VELOCITY = 1.0f;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _monster = GameObject.Find("Monster");
        _monsterFx = _monster.GetComponent<MonsterFX>();
        _monsterCollider = GetComponent<Collider2D>();
    }
	
	// Update is called once per frame
	void Update()
    {
        _hittedGround = Physics2D.Raycast(transform.position, Vector2.down, RAYCAST_DOWN_DISTANCE, GROUND_MASK);

        if (!_hittedGround.collider)
        {
            Vector2 position = transform.position;
            position.x += _monsterCollider.bounds.size.x / 2.0f;

            _hittedGround = Physics2D.Raycast(position, Vector2.down, RAYCAST_DOWN_DISTANCE, GROUND_MASK);

            if (!_hittedGround.collider)
            {
                position.x -= _monsterCollider.bounds.size.x;

                _hittedGround = Physics2D.Raycast(position, Vector2.down, RAYCAST_DOWN_DISTANCE, GROUND_MASK);
            }
        }

        bool hasHit = _hittedGround.collider != null;
        _isGrounded = !_isDownJumping && hasHit;

        if (_isJumping && hasHit)
        {
            if (_rigidBody.velocity.y <= 0)
            {
                _monsterCollider.enabled = true;
                _isJumping = false;
                _savedUpperHit = new RaycastHit2D();
            }
        }

        // Handle input now, before reactivating colliders
        HandlePlayerInput();

        if (_isDownJumping && hasHit && _savedHit.collider != _hittedGround.collider)
        {
            _monsterCollider.enabled = true;
            _isDownJumping = false;
        }

        if (_isJumping)
        {
            if (_savedUpperHit.collider == null)
            {
                RaycastHit2D upperHit;
                bool hit = tryHitInDirection(02f, GROUND_MASK, out upperHit);
                if (hit && upperHit.collider != null)
                {
                    _monsterCollider.enabled = false;
                    _savedUpperHit = upperHit;
                }
            }
            else if (transform.position.y > _savedUpperHit.transform.position.y + _savedUpperHit.collider.bounds.size.y)
            {
                _monsterCollider.enabled = true;
                _savedUpperHit = new RaycastHit2D();
            }
        }
    }

    void HandlePlayerInput()
    {
        #region X Forces Region
        if (_isGrounded)
        {
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                _forces += new Vector2(POSITIVE_X_ACCELERATION, 0);
                _shouldStop = false;
                _isStopping = false;
                _monster.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _forces += new Vector2(-POSITIVE_X_ACCELERATION, 0);
                _shouldStop = false;
                _isStopping = false;
                _monster.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
            else if (!_shouldStop && !_isStopping)
            {
                _shouldStop = true;
            }
        }

        // Invert forces once
        if (_shouldStop)
        {
            _forces.x = -_forces.x;
            _shouldStop = false;
            _isStopping = true;
        }

        // Add force to stop
        if (_isStopping)
        {
            _forces += new Vector2(NEGATIVE_X_ACCELERATION * Mathf.Sign(_forces.x) * -1f, 0);
        }

        // Clamp max force
        if (_forces.x > MAX_X_FORCE || _forces.x < -MAX_X_FORCE)
        {
            _forces.x = MAX_X_FORCE * Mathf.Sign(_forces.x);
        }
        #endregion 
        
        if (_isGrounded && _forces.y == 0 &&(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)))
        {
            _forces += new Vector2(-_forces.x, POSITIVE_Y_ACCELERATION);
            _isJumping = true;
        }
        else if (!_isGrounded)
        {
            _forces += new Vector2(0, -GRAVITY_FORCE);
        }
        else if (_forces.y <= 0 && _isGrounded)
        {
            _forces.y = 0;
            _rigidBody.velocity.Set(_rigidBody.velocity.x, 0);
        }

        if (_isGrounded && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            if (_hittedGround.collider.gameObject.tag != "BaseGround")
            {
                _monsterCollider.enabled = false;
                _savedHit = _hittedGround;
                _isDownJumping = true;
            }
        }

        if (_isStopping && Mathf.Abs(_rigidBody.velocity.x) <= STOP_VELOCITY)
        {
            _rigidBody.velocity.Set(0, _rigidBody.velocity.y);
            _forces.x = 0;
            _isStopping = false;
        }

        _rigidBody.AddForce(_forces * Time.deltaTime);


        if(_rigidBody.velocity.x != 0f)
        {
            _monsterFx.setState(MonsterFX.States.WALKING);
        }
        else
        {
            _monsterFx.setState(MonsterFX.States.IDLE);
        }
    }

    bool tryHitInDirection(float distance, int mask, out RaycastHit2D hit, int depth = 0)
    {
        Vector2 origin = Vector2.zero;
        Vector2 direction = Vector2.zero;

        switch (depth)
        {
            case 0:
            case 1:
            case 2:
                origin = transform.position;
                origin.y += _monsterCollider.bounds.size.y;
                direction = Vector2.up;

                if (depth == 1)
                {
                    origin.x -= _monsterCollider.bounds.size.x / 2;
                }
                else if (depth == 2)
                {
                    origin.x += _monsterCollider.bounds.size.x / 2;
                }
                break;
            case 3:
            case 4:
            case 5:
                origin = transform.position;
                origin.x += _monsterCollider.bounds.size.x * Mathf.Sign(_forces.x);
                direction = Mathf.Sign(_forces.x) > 0 ? Vector2.right : Vector2.left;

                if (depth == 4)
                {
                    origin.y += _monsterCollider.bounds.size.y / 2;
                }
                else if (depth == 5)
                {
                    origin.y += _monsterCollider.bounds.size.y;
                }

                break;
            default:
                hit = new RaycastHit2D();
                return false;
        }

        hit = Physics2D.Raycast(origin, direction, distance, mask);
        if (hit.collider)
        {
            return true;
        }

        return tryHitInDirection(distance, mask, out hit, depth + 1);
    }
}