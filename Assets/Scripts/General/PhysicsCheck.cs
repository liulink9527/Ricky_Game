using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour {
    
    public CapsuleCollider2D coll;
    
    [Header("碰撞检测参数")] 
    public bool manual;
    public float checkRadius = 2f;
    public LayerMask groundLayer;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    [Header("状态")]
    // 地面检测
    public bool isGround;

    public bool touchLeftWall;
    public bool touchRightWall;

    private void Awake() {
        coll = GetComponent<CapsuleCollider2D>();
        
        if (!manual) {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
        
    }

    void Start() {
    }

    // Update is called once per frame
    void Update() {
        Check();
    }

    public void Check() {
        // 检测地面
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);
        
        // 墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);
    }
}