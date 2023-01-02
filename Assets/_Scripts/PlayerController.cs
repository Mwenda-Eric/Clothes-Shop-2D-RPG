using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class PlayerController : MonoBehaviour
{
    private Vector2 _movementInput;
    public ContactFilter2D movementFilter;
    public float collisionOffset = 0.05f;
    public float moveSpeed = 1f;
    private readonly List<RaycastHit2D> _castCollisions = new List<RaycastHit2D>();
    public Animator playerAnimator;
    private Transform _playerTransform;

    private Rigidbody2D _rigidbody2D;

    private static readonly int IsPlayerWalkingAnimationId = Animator.StringToHash("isPlayerWalking");
    private static readonly int SwordAttackAnimationId = Animator.StringToHash("SlashMelee1H");
    
    private bool _isPlayerMove = true;//Set to force on Attack animation events.

    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_movementInput == Vector2.zero)
        {
            playerAnimator.SetBool(IsPlayerWalkingAnimationId, false);
        }
        if (!_isPlayerMove) return;//If the player Cant move don't proceed to try and move. Value changed by Animation events.

        if (_movementInput != Vector2.zero)//This means that the player wants to move.
        {
            playerAnimator.SetBool(IsPlayerWalkingAnimationId, true);
            //Try to move first with the given movement input.
            bool isMovementSuccessful = TryMove(_movementInput);
            
            //If the movement is not successful, it means we have collided with something.
            if (!isMovementSuccessful)
            {
                //Then just try to move along the x-axis to slide along the obstacle on the x-axis.
                isMovementSuccessful = TryMove(new Vector2(_movementInput.x, 0));
                
                //If Moving along X is not successful, now try moving along Y-axis to slide along y on the obstacle.
                if (!isMovementSuccessful)
                {
                    //Use y-axis as input to slide along the y - axis.
                    TryMove(new Vector2(0, _movementInput.y));
                }
            }
        }

        _playerTransform.rotation = _movementInput.x switch
        {
            //Rotate the Character for left and right movement.
            < 0 => Quaternion.Euler(0, 180, 0),
            > 0 => Quaternion.Euler(0, 0, 0),
            _ => _playerTransform.rotation
        };
    }
    
    //Try Move function will try to move the character and if the character can slide along the object, the character slides.
    private bool TryMove(Vector2 directionToMove)
    {
        //Don't go ahead to cast if we have no direction to move.
        if (directionToMove == Vector2.zero) return false;
        
        //This is bound to check for potential collisions.
        int numberOfCollisionsFound = _rigidbody2D.Cast(
            directionToMove, //X and Y values between -1 and 1, representing the direction from the body to look for collision.
            movementFilter, //Setting determining where a collision can occur on; Such as layers to collide with.
            _castCollisions, //A list of collisions to store the found collisions into after the cast is finished.
            moveSpeed * Time.fixedDeltaTime * collisionOffset //Amount to cast equal to equal to the movement plus an offset.
        );
            
        //If the numberOfCollisionsFound is zero - It means that there haven't been any collisions and the move is legal.
        if (numberOfCollisionsFound == 0)
        {
            _rigidbody2D.MovePosition(_rigidbody2D.position + directionToMove * (moveSpeed * Time.fixedDeltaTime));
            return true;
        }
        return false;
    }

    public Transform PlayerTransform => _playerTransform;

    //Functions called from Attack animation Events on first and last keyframes.
    public void LockMovement()
    {
        _isPlayerMove = false;
    }
    public void UnlockMovement()
    {
        _isPlayerMove = true;
    }
    
    //This Method is called(Will receive Message) from the PlayerInput Action Asset of the new Input System.
    void OnMove(InputValue movementValue)
    {
        //Get the Vector2 from the Input value.
        _movementInput = movementValue.Get<Vector2>();
    }
    
    void OnFire()
    {
        if (GameManager.Instance.isOutfitPanelActive) return;
        playerAnimator.Play(SwordAttackAnimationId);
    }

    void OnChangeOutfit()
    {
        GameManager.Instance.ChangeNextOutfit();
    }
}
