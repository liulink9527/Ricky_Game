using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;

    [Header("基本参数")]
    public float moveSpeed;
    public float jumpForce;
    public float hurtForce;
    // 用于按住left shift 切换行走模式
    private float runSpeed;
    private float walkSpeed => moveSpeed / 2.5f;
    
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private CapsuleCollider2D coll;
    
    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;
    

    [Header("状态")]
    // 是否下蹲
    public bool isCrouch;
    public bool isHurt;
    public bool isDead;
    public bool isAttack;

    private void Awake() {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        coll = GetComponent<CapsuleCollider2D>();

        originalOffset = coll.offset;
        originalSize = coll.size;
        
        // 跳跃
        inputControl.Gameplay.Jump.started += Jump;

        #region 强制走路
        runSpeed = moveSpeed;
        // started代表触发
        // performed代表一直按住
        inputControl.Gameplay.WalkButton.performed += ctx => {
            if (physicsCheck.isGround) {
                moveSpeed = walkSpeed;
            } 
        };
        inputControl.Gameplay.WalkButton.canceled += ctx => {
            if (physicsCheck.isGround) {
                moveSpeed = runSpeed;
            }
        };
        #endregion
        
        // 攻击
        inputControl.Gameplay.Attack.started += PlayerAttack;

    }




    private void OnEnable() {
        Debug.Log("开启");
        inputControl.Enable();
    }

    private void Start() {
        
    }

    private void OnDisable() {
        inputControl.Disable();
        Debug.Log("关闭");
    }

    private void Update() {
        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate() {
        if (!isHurt && !isAttack) {
            Move();
        }
    }
    
    public void Move() {
        if (!isCrouch) {
            rb.velocity = new Vector2(inputDirection.x * moveSpeed * Time.deltaTime, rb.velocity.y);
        }

        // 人物朝向
        int facingDir = (int)transform.localScale.x;
        if (inputDirection.x > 0) {
            facingDir = 1;
        } else if (inputDirection.x < 0){
            facingDir = -1;
        }
        // 人物翻转
        transform.localScale = new Vector3(facingDir, 1, 1);
        
        // 第二种方式
        // if (inputDirection.x < 0) {
        //     sprite.flipX = true;
        // } else if (inputDirection.x > 0) {
        //     sprite.flipX = false;
        // }
        
        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch) {
            // 修改碰撞体大小和位移
            coll.size = new Vector2(0.95f, 1.7f);
            coll.offset = new Vector2( -0.13f, 0.8f);
        } else {
            coll.size = originalSize;
            coll.offset = originalOffset;
        }
    }
    
    private void Jump(InputAction.CallbackContext obj) {
        if (physicsCheck.isGround) {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
        }
    }
    
    private void PlayerAttack(InputAction.CallbackContext obj) {
        if (physicsCheck.isGround) {
            playerAnimation.PlayerAttack();
            isAttack = true;
        }
    }

    public void GetHurt(Transform attacker) {
        isHurt = true;
        rb.velocity = Vector2.one;
        // 方向
        Vector2 dir = new Vector2(transform.position.x - attacker.position.x, 0).normalized;
        // 添加一个反方向的力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        
    }

    public void PlayerDead() {
        isDead = true;
        inputControl.Gameplay.Disable();
    }

    private void CheckState() {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;
    }
    
}
