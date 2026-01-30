using System;
using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using UnityEngine;

public class KillerScript : MonoBehaviour
{
    public Animator playerAnimator;   // 玩家的动画状态机
    public float killRange = 2.0f;    // 击杀距离
    public string killAnimationTrigger = "Kill"; // 玩家攻击动画的 Trigger 名
    public LayerMask aiLayer;         // 设置 AI 所在的层，优化性能

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryKill();
        }
    }

    void TryKill()
    {
        // 1. 尝试寻找前方的 AI (使用射线检测或球体覆盖检测)
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, killRange, aiLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            // 获取 AI 脚本（这里用基类 Common_WanderScript，这样动物和人都能杀）
            Common_WanderScript ai = hitCollider.GetComponent<Common_WanderScript>();

            if (ai != null)
            {
                // 2. 玩家播放攻击动画
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(killAnimationTrigger);
                }

                // 3. 让 AI 转向玩家（可选，增加真实感）
                ai.transform.LookAt(new Vector3(transform.position.x, ai.transform.position.y, transform.position.z));

                // 4. 触发 AI 死亡
                ai.Die();

                // 击杀一个后跳出循环，防止一键杀一群
                break; 
            }
        }
    }

    // 在编辑器里画出攻击范围
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}
