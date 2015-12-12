using UnityEngine;
using System.Collections;

public class MobMovement : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private Vector2 _forces = Vector2.zero;
    private bool _shouldStop = false;
    private bool _isStopping = false;
    private bool _isGrounded = false;
    private RaycastHit2D _hittedGround;
    private RaycastHit2D _savedHit = new RaycastHit2D();

    private const int GROUND_MASK = 1 << 8;
    private const float POSITIVE_X_ACCELERATION = 100.0f;
    private const float NEGATIVE_X_ACCELERATION = -10.0f;
    private const float POSITIVE_Y_ACCELERATION = 1300f;
    private const float GRAVITY_FORCE = 50f;
    private const float MAX_X_FORCE = 400.0f;
    private const float STOP_VELOCITY = 1.0f;

    // Use this for initialization
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update()
    {
        _hittedGround = Physics2D.Raycast(transform.position, Vector2.down, 0.3f, GROUND_MASK);
        _isGrounded = _hittedGround.collider != null;

        if (_hittedGround.collider != _savedHit.collider && _savedHit.collider != null &&  _hittedGround.collider != null)
        {
            _savedHit.collider.enabled = true;
            _savedHit = _hittedGround;
        }

        RaycastHit2D upperHit = Physics2D.Raycast(transform.position, Vector2.up, 1f, GROUND_MASK);
        if (upperHit.collider != null)
        {
            upperHit.collider.enabled = false;
            _savedHit = _hittedGround;
        }

        HandlePlayerInput();
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
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                _forces += new Vector2(-POSITIVE_X_ACCELERATION, 0);
                _shouldStop = false;
                _isStopping = false;
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
            _forces += new Vector2(0, POSITIVE_Y_ACCELERATION);
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
            _hittedGround.collider.enabled = false;
            _savedHit = _hittedGround;
        }

        if (_isStopping && Mathf.Abs(_rigidBody.velocity.x) <= STOP_VELOCITY)
        {
            _rigidBody.velocity.Set(0, _rigidBody.velocity.y);
            _forces.x = 0;
            _isStopping = false;
        }

        _rigidBody.AddForce(_forces * Time.deltaTime);
    }
}
