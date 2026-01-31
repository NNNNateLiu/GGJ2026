using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using StarterAssets;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;

public class PlayerGameplay : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("射线的最大长度")]
    public float rayDistance = 10f;
    public float sphereRadius = .5f;
    
    [Tooltip("AI 所在的层级")]
    public LayerMask aiLayer;
    [Tooltip("射线在编辑器中的颜色")]
    public Color debugRayColor = Color.green;
    
    [Header("调试视觉设置")]
    public bool showRayInScene = true;
    public Color rayColorWhileAiming = Color.red;
    public Color rayColorIdle = Color.yellow;

    private Camera _mainCam;
    private CivilianScript _currentHitAI;
    
    [SerializeField] private GameObject outlookPool;
    
    [Header("----------Core----------")]
    
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController aiAnimatorController;
    [SerializeField] private AnimatorOverrideController playerAnimatorController;
    
    [Header("Layer Settings")]
    public string defaultLayerName = "Default";
    public string ignoreRaycastLayerName = "Ignore Raycast";
    
    [Header("AI Content")]
    [SerializeField] private UnityEngine.AI.NavMeshAgent navMeshAgent;
    [SerializeField] private People_WanderScript people_WanderScript;
    [SerializeField] private CivilianScript civilianScript;

    [Header("Player Content")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerGameplay playerGameplay;
    
    public CinemachineVirtualCamera virtualCamera;

    void Start()
    {
        _mainCam = Camera.main;
    }

    void Update()
    {
        // 1. 当按下 Space 时，持续发射射线进行预览或检测
        if (Input.GetKey(KeyCode.Space))
        {
            PerformRaycast();
        }

        // 2. 当松开 Space 时，如果射中了 AI，则执行附身
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_currentHitAI != null)
            {
                UnPossessed(_currentHitAI);
                _currentHitAI.BePossessed();
                _currentHitAI = null; // 执行完后清空记录
                
            }
        }
    }

    void PerformRaycast()
    {
        Ray ray = _mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * rayDistance, debugRayColor);

        if (Physics.SphereCast(ray.origin, sphereRadius, ray.direction, out hit, rayDistance, aiLayer))
        {
            CivilianScript hitAI = hit.collider.GetComponent<CivilianScript>();

            if (hitAI != null)
            {
                if (_currentHitAI != hitAI)
                {
                    if (_currentHitAI != null) _currentHitAI.SetAimedFeedback(false);
                    _currentHitAI = hitAI;
                    _currentHitAI.SetAimedFeedback(true);
                }
                return;
            }
        }

        // 如果射线没打到任何 AI，但之前有 AI 被激活了
        if (_currentHitAI != null)
        {
            _currentHitAI.SetAimedFeedback(false);
            _currentHitAI = null;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showRayInScene) return;

        // 如果没有获取到主摄像机（比如在非运行模式下），尝试获取
        Camera cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;
        if (cam == null) return;

        Gizmos.color = rayColorIdle;
        Vector3 forward = cam.transform.forward;
        Vector3 origin = cam.transform.position;

        // 画出代表射线的直线
        Gizmos.DrawLine(origin, origin + forward * rayDistance);
        
        // 在射线末端画一个半透明小球，方便确认长度 X 的终点
        Gizmos.DrawWireSphere(origin + forward * rayDistance, 0.2f);
    }

    public void UnPossessed(CivilianScript possessTarget)
    {
        // 激活所有AI逻辑和组件
        navMeshAgent.enabled = true;
        //StopAllCoroutines();
        people_WanderScript.enabled = true;
        
        // 禁用所有Player逻辑和组件；切换摄像机；切换动画机
        thirdPersonController.enabled = false;
        playerInput.enabled = false;
        playerGameplay.enabled = false;
        
        animator.runtimeAnimatorController = aiAnimatorController;
        
        int defaultLayer = LayerMask.NameToLayer(defaultLayerName);
        SetLayerRecursive(gameObject.gameObject, defaultLayer);
        
        int ignoreLayer = LayerMask.NameToLayer(ignoreRaycastLayerName);
        SetLayerRecursive(possessTarget.gameObject, ignoreLayer);
        
        // 禁用 PlayerGameplay 脚本
        this.enabled = false;
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
