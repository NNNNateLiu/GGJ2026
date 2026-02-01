using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using StarterAssets;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;

public class KillerScript : MonoBehaviour
{
    public Animator playerAnimator;
    public float killRange = 2.0f;
    public string killAnimationTrigger = "Kill";
    public GameObject bloodVFX;
    public GameObject killStrikeVFX;
    public GameObject killStrikeVFXInstance;
    
    public string aiLayerName = "AI";
    private LayerMask aiLayer;

    [Header("Cooldown Settings")]
    public float killCooldown = 1.5f;
    private float _nextKillTime = 0f;
    
    [Header("Combo Timer (Combo System)")]
    [Tooltip("杀人后的宽限时间（X秒）")]
    public float comboDuration = 5.0f; 
    private float _currentComboTimer = 0f;
    private bool _isTimerActive = false;

    private CivilianScript _lastNearestAI;
    public GameObject killrRemaind;
    
    private void Start()
    {
        playerAnimator = GetComponent<Animator>();
        bloodVFX = Resources.Load("Prefab/FX_BloodSplat_01") as GameObject;
        killStrikeVFX = Resources.Load("Prefab/KillerRemind") as GameObject;
        killrRemaind = Instantiate(Resources.Load<GameObject>("Prefab/CountDown"), this.transform);
        killrRemaind.GetComponent<CountDown>().contextText = " s to leave";
    }

    void Update()
    {
        // 1. 更新击杀指示器
        UpdateNearestIndicator();

        // 2. 处理按键击杀
        if ((Input.GetKeyDown(KeyCode.E) && this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1) || (Input.GetKeyDown(KeyCode.K) && !this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1))
        {
            if (Time.time >= _nextKillTime)
            {
                TryKillNearest();
            }
        }

        // 3. 处理倒计时逻辑
        HandleComboTimer();
    }

    void HandleComboTimer()
    {
        if (!_isTimerActive) return;

        _currentComboTimer -= Time.deltaTime;
        killrRemaind.GetComponent<CountDown>().SetText(_currentComboTimer);

        // 调试用：在控制台显示剩余时间
        // Debug.Log($"剩余暗杀时间: {_currentComboTimer:F1}");

        if (_currentComboTimer <= 0)
        {
            _isTimerActive = false;
            killrRemaind.GetComponent<CountDown>().ClearText();
            Debug.Log("<color=red>时间到！由于未能及时击杀，被强制脱离附身！</color>");

            Destroy(killStrikeVFXInstance);
            
            CivilianScript nearestCiv = AIManager.Instance.GetNearestCivilian(gameObject.transform.position)
                .GetComponent<CivilianScript>();
            
            gameObject.GetComponent<PlayerGameplay>().UnPossessed(nearestCiv);
            bool isPlayer1 = gameObject.GetComponent<ThirdPersonController>().IsPlayer1;
            nearestCiv.BePossessed(isPlayer1);
        }
    }

    void TryKillNearest()
    {
        if (_lastNearestAI != null && !_lastNearestAI.isPolice)
        {
            _nextKillTime = Time.time + killCooldown;

            // --- 倒计时核心逻辑开始 ---
            // 只要完成击杀，就重置计时器并激活它
            _currentComboTimer = comboDuration;

            if (killStrikeVFXInstance == null)
            {
                killStrikeVFXInstance = Instantiate(killStrikeVFX, this.transform);
            }
            else
            {
                killStrikeVFXInstance.transform.GetChild(0).GetComponent<KillerRemind>().IncreaseEffect();
            }
            
            _isTimerActive = true;
            // --- 倒计时核心逻辑结束 ---

            if (playerAnimator != null)
                playerAnimator.SetTrigger(killAnimationTrigger);

            _lastNearestAI.transform.LookAt(new Vector3(transform.position.x, _lastNearestAI.transform.position.y, transform.position.z));
            
            if (bloodVFX != null)
            {
                GameObject vfx = Instantiate(bloodVFX, _lastNearestAI.gameObject.transform.GetChild(0));
                vfx.transform.localPosition = Vector3.zero;
                Destroy(vfx, 3f);
            }

            _lastNearestAI.SetKillableIndicator(false);
            _lastNearestAI.GetComponent<Common_WanderScript>().Die();

            if (_lastNearestAI)
            {
                if (_lastNearestAI.GetComponent<ThirdPersonController>().enabled == true)
                {
                    Application.Quit();
                }
            }
            _lastNearestAI = null;
        }
        else if (_lastNearestAI != null && !_lastNearestAI.isPolice)
        {
            Application.Quit();
        }
    }

    // ... UpdateNearestIndicator 和 OnDrawGizmosSelected 保持不变 ...
    void UpdateNearestIndicator() 
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, killRange, LayerMask.GetMask("AI", "Ignore Raycast"));
        CivilianScript nearestAI = null;
        float minDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            CivilianScript ai = hitCollider.GetComponent<CivilianScript>();
            if (ai != null && !ai.isPolice && ai.gameObject != this.gameObject)
            {
                float dist = Vector3.Distance(transform.position, ai.transform.position);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    nearestAI = ai;
                }
            }
        }

        if (nearestAI != _lastNearestAI)
        {
            if (_lastNearestAI != null) _lastNearestAI.SetKillableIndicator(false);
            if (nearestAI != null) nearestAI.SetKillableIndicator(true);
            _lastNearestAI = nearestAI;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, killRange);
    }
    
    private void SetLayerRecursive(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursive(child.gameObject, newLayer);
        }
    }
}