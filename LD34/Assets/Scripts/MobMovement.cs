using UnityEngine;
using System.Collections;

public class MobMovement : MonoBehaviour
{
    public bool HandleInput = false;
    public float MaxForce_X = 400.0f;
    public float MaxForce_Y = 4000.0f;
    public float MinForce_Y = -2000.0f;
    public float MaxVelocity_X = 6f;
    public Vector2 InitialForce = Vector2.zero;

    private Rigidbody2D _rigidBody;
    private Vector2 _forces = Vector2.zero;
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

    public static readonly float RAYCAST_DOWN_DISTANCE = 0.6f;
    public static readonly float POSITIVE_X_ACCELERATION = 100.0f;
    public static readonly float NEGATIVE_X_ACCELERATION = -50.0f;
    public static readonly float POSITIVE_Y_ACCELERATION = 1300f;
    public static readonly float GRAVITY_FORCE = 50f;
    public static readonly float STOP_VELOCITY = 1.0f;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _monster = transform.Find("Agent").gameObject;
        _monsterCollider = _monster.GetComponent<Collider2D>();
        _monsterFx = _monster.GetComponent<MonsterFX>();

        ApplyInitialForces();
    }
	
    public void ApplyInitialForces()
    {
        _forces += InitialForce;
    }

	// Update is called once per frame
	void FixedUpdate()
    {
        _hittedGround = Physics2D.Raycast(transform.position, Vector2.down, RAYCAST_DOWN_DISTANCE, Constants.Layers.GROUND_MASK);

        if (!_hittedGround.collider && _monsterCollider != null)
        {
            Vector2 position = transform.position;
            position.x += _monsterCollider.bounds.size.x / 2.0f + 0.2f;

            _hittedGround = Physics2D.Raycast(position, Vector2.down, RAYCAST_DOWN_DISTANCE, Constants.Layers.GROUND_MASK);

            if (!_hittedGround.collider)
            {
                position.x -= _monsterCollider.bounds.size.x + 0.2f;

                _hittedGround = Physics2D.Raycast(position, Vector2.down, RAYCAST_DOWN_DISTANCE, Constants.Layers.GROUND_MASK);
            }
        }

        bool hasHit = _hittedGround.collider != null;
        _isGrounded = !_isDownJumping && hasHit;

        if (_isJumping && hasHit)
        {
            if (_rigidBody.velocity.y <= 0)
            {
                if (_monsterCollider)
                {
                    _monsterCollider.enabled = true;
                }

                _isJumping = false;
                _savedUpperHit = new RaycastHit2D();
            }
        }

        // Handle gravity
        if (!_isGrounded)
        {
            _forces += new Vector2(0, -GRAVITY_FORCE);
        }
        else if (_forces.y <= 0 && _isGrounded)
        {
            _forces.y = 0;
            _rigidBody.velocity = new Vector2(_rigidBody.velocity.x, 0);
        }

        // Handle input now, before reactivating colliders
        if (HandleInput)
        {
            HandlePlayerInput();
        }

        // Add force to stop
        if (_isStopping)
        {
            _forces += new Vector2(NEGATIVE_X_ACCELERATION * Mathf.Sign(_forces.x) * -1f, 0);
        }

        // Clamp max force
        if (_forces.x > MaxForce_X || _forces.x < -MaxForce_X)
        {
            _forces.x = MaxForce_X * Mathf.Sign(_forces.x);
        }
        if (_forces.y > MaxForce_Y)
        {
            _forces.y = MaxForce_Y;
        }
        else if (_forces.y < MinForce_Y)
        {
            _forces.y = MinForce_Y;
        }

        if (_rigidBody.velocity.x > MaxVelocity_X)
        {
            _rigidBody.velocity = new Vector2(MaxVelocity_X, _rigidBody.velocity.y);
        }
        else if (_rigidBody.velocity.x < -MaxVelocity_X)
        {
            _rigidBody.velocity = new Vector2(-MaxVelocity_X, _rigidBody.velocity.y);
        }

        // Stop if we are at stop velocity
        if (_isStopping && Mathf.Abs(_rigidBody.velocity.x) <= STOP_VELOCITY)
        {
            _rigidBody.velocity = new Vector2(0, _rigidBody.velocity.y);
            _forces.x = 0;
            _isStopping = false;

            if (_monsterFx != null)
            {
                _monsterFx.setState(MonsterFX.States.IDLE);
            }
        }

        // Apply forces
        _rigidBody.AddForce(_forces * Time.deltaTime);

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
                bool hit = tryHitInDirection(3f, Constants.Layers.GROUND_MASK, out upperHit);
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

        if (_monsterFx != null)
        {
            if (_rigidBody.velocity.x != 0f)
            {
                _monsterFx.setState(MonsterFX.States.WALKING);
            }
        }
    }

    public void Stop()
    {
        if (_rigidBody.velocity.x != 0)
        {
            if (Mathf.Sign(_rigidBody.velocity.x) == Mathf.Sign(_forces.x))
            {
                _forces.x = -_forces.x;
            }

            _isStopping = true;
        }
    }

    public void Jump()
    {
        if (_isGrounded && _forces.y == 0)
        {
            _forces += new Vector2(-_forces.x, POSITIVE_Y_ACCELERATION);
            _isJumping = true;
        }
    }

    public void ForceJump()
    {
        _forces += new Vector2(_forces.x, _forces.y + POSITIVE_Y_ACCELERATION / 2);
        _isJumping = true;
    }

    public void MoveRight()
    {
        if (_forces.x < 0)
        {
            _forces.x = 0;
        }

        _forces += new Vector2(POSITIVE_X_ACCELERATION, 0);
        _isStopping = false;
        _monster.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void MoveLeft()
    {
        if (_forces.x > 0)
        {
            _forces.x = 0;
        }

        _forces += new Vector2(-POSITIVE_X_ACCELERATION, 0);
        _isStopping = false;
        _monster.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }

    void HandlePlayerInput()
    {
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            MoveRight();
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        else if (_isGrounded && !_isStopping)
        {
            Stop();
        }
        
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
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
