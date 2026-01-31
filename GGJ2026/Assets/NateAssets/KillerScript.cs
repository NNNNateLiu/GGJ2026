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
        // 先更新指示器，确保 _lastNearestAI 是最新的
        UpdateNearestIndicator();

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Time.time >= _nextKillTime)
            {
                // 直接尝试击杀指示器指向的那个 AI
                TryKillNearest();
            }
            else
            {
                Debug.Log($"击杀技能冷却中，还剩 {(_nextKillTime - Time.time):F1} 秒");
            }
        }
    }

    // 推荐做法：既然已经找到了最近的，直接对它下手
    void TryKillNearest()
    {
        if (_lastNearestAI != null)
        {
            // 1. 进入冷却
            _nextKillTime = Time.time + killCooldown;

            // 2. 玩家动画
            if (playerAnimator != null)
                playerAnimator.SetTrigger(killAnimationTrigger);

            // 3. 处理特效与死亡
            _lastNearestAI.transform.LookAt(new Vector3(transform.position.x, _lastNearestAI.transform.position.y, transform.position.z));
            
            if (bloodVFX != null)
            {
                // 获取模型骨骼/子物体生成特效
                GameObject vfx = Instantiate(bloodVFX, _lastNearestAI.gameObject.transform.GetChild(0));
                vfx.transform.localPosition = Vector3.zero;
                Destroy(vfx, 3f); // 别忘了销毁特效
            }

            // 4. 执行 AI 死亡
            _lastNearestAI.SetKillableIndicator(false); // 杀掉前先关掉提示
            _lastNearestAI.GetComponent<Common_WanderScript>().Die();

            // 5. 立即清空引用，防止下一帧 UpdateNearestIndicator 还没跑时就报错
            _lastNearestAI = null; 
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