using UnityEngine;
using PolyPerfect;
using Unity.Mathematics;

public class KillerScript : MonoBehaviour
{
    public Animator playerAnimator;
    public float killRange = 2.0f;
    public string killAnimationTrigger = "Kill";
    public LayerMask aiLayer;
    public GameObject bloodVFX;

    [Header("Cooldown Settings")]
    public float killCooldown = 1.5f; // 冷却时间设置为 1.5 秒
    private float _nextKillTime = 0f; // 记录下一次允许击杀的时间点
    
    private CivilianScript _lastNearestAI; // 记录上一帧最近的 AI

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 检查当前系统时间是否已经超过了“下一次允许击杀的时间”
            if (Time.time >= _nextKillTime)
            {
                TryKill();
            }
            else
            {
                // 可选：在这里可以添加 UI 提示或音效，告诉玩家技能还在 CD 中
                float remainingCD = _nextKillTime - Time.time;
                Debug.Log($"击杀技能冷却中，还剩 {remainingCD:F1} 秒");
            }
        }
        
        UpdateNearestIndicator();
        
    }

    void TryKill()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, killRange, aiLayer);
        
        foreach (var hitCollider in hitColliders)
        {
            Common_WanderScript ai = hitCollider.GetComponent<Common_WanderScript>();

            if (ai != null)
            {
                // 1. 更新冷却时间：当前时间 + 冷却时长
                _nextKillTime = Time.time + killCooldown;

                // 2. 玩家播放动画
                if (playerAnimator != null)
                {
                    playerAnimator.SetTrigger(killAnimationTrigger);
                }

                // 3. AI 转向并执行死亡逻辑
                ai.transform.LookAt(new Vector3(transform.position.x, ai.transform.position.y, transform.position.z));
                GameObject vfx = Instantiate(bloodVFX, ai.gameObject.transform.GetChild(0));
                vfx.transform.localPosition = Vector3.zero;
                ai.Die();

                break; 
            }
        }
    }

    void UpdateNearestIndicator()
    {
        // 获取范围内所有 AI
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, killRange, aiLayer);
        
        CivilianScript nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            CivilianScript ai = hitCollider.GetComponent<CivilianScript>();
            
            // 确保 AI 存在且没死
            if (ai != null)
            {
                float dist = Vector3.Distance(transform.position, ai.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestAI = ai;
                }
            }
        }

        // 逻辑切换：如果最近的 AI 变了
        if (nearestAI != _lastNearestAI)
        {
            // 关闭上一个 AI 的提示
            if (_lastNearestAI != null)
            {
                _lastNearestAI.SetKillableIndicator(false);
            }

            // 开启当前最近 AI 的提示
            if (nearestAI != null)
            {
                nearestAI.SetKillableIndicator(true);
            }

            _lastNearestAI = nearestAI;
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
}