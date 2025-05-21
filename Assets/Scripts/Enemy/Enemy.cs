using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
    
    [Header("基本参数")]
    public float moveSpeed;
    public float chaseSpeed;
    [HideInInspector] public float currentSpeed;
    public float hurtForce;
    [HideInInspector] public Transform attacker;
    [HideInInspector] public Vector3 faceDir;
    
    [Header("计时器")] 
    public float waitTime;
    public float waitTimeCounter;
    public bool wait;

    [Header("状态")] 
    public bool isHurt;
    public bool isDead;

    
    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public CapsuleCollider2D coll;
    [HideInInspector] public PhysicsCheck physicsCheck;


    protected virtual void Awake() {
        this.rb = GetComponent<Rigidbody2D>();
        this.anim = GetComponent<Animator>();
        this.coll = GetComponent<CapsuleCollider2D>();
        this.physicsCheck = GetComponent<PhysicsCheck>();
        
        this.currentSpeed = moveSpeed;
    }

    protected virtual void OnEnable() {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    protected virtual void Update() {
        faceDir = new Vector3(-transform.localScale.x, 1, 1);
        currentState.LogicUpdate();
        TimeCounter();
    }

    protected virtual void FixedUpdate() {
        if (!isHurt & !isDead && !wait) {
            Move();
        }
        
        currentState.PhysicsUpdate();
    }

    protected virtual void OnDisable() {
        currentState.OnExit();
    }

    public virtual void Move() {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    // 转向等待计时
    public void TimeCounter() {
        if (wait) {
            waitTimeCounter -= Time.deltaTime;
            if (waitTimeCounter <= 0) {
                wait = false;
                waitTimeCounter = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }
    }

    public void OnTakeDamage(Transform attackTrans) {
        attacker = attackTrans;
        // 转身
        if (attackTrans.position.x - transform.position.x > 0) {
            transform.localScale = new Vector3(-1, 1, 1);

        }

        if (attackTrans.position.x - transform.position.x < 0) {
            transform.localScale = new Vector3(1, 1, 1);
        }
        
        // 受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;

        StartCoroutine(OnHurt(dir));
    }
    private IEnumerator OnHurt(Vector2 dir) {
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.5f);
        isHurt = false;
    }

    public void OnDead() {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterAnimation() {
        Destroy(this.gameObject);
    }



}
