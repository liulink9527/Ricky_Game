using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState {
    
    
    public override void OnEnter(Enemy enemy) {
        currentEnemy = enemy;
        
    }

    public override void LogicUpdate() {
        // todo 发现player切换到Chase状态
        
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDir.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDir.x > 0)) {
            currentEnemy.wait = true;
            currentEnemy.anim.SetBool("walk", false);
        } else {
            currentEnemy.anim.SetBool("walk", true);
        }
    }

    public override void PhysicsUpdate() {
    }

    public override void OnExit() {
    }
}
