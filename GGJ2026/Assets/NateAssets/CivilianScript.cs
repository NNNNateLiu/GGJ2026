using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using StarterAssets;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;

public class CivilianScript : MonoBehaviour
{
    [SerializeField] private GameObject outlookPool;

    [SerializeField] private GameObject aimedIndicator;

    [SerializeField] private GameObject killableIndicator;
    
    [SerializeField] private Animator animator;
    [SerializeField] private AnimatorOverrideController aiAnimatorController;
    [SerializeField] private AnimatorOverrideController playerAnimatorController;
    
    [Header("Layer Settings")]
    public string defaultLayerName = "Default";
    public string ignoreRaycastLayerName = "Ignore Raycast";
    
    [Header("AI Content")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private People_WanderScript people_WanderScript;
    [SerializeField] private CivilianScript civilianScript;

    [Header("Player Content")]
    [SerializeField] private ThirdPersonController thirdPersonController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private PlayerGameplay playerGameplay;
    [SerializeField] private GameObject playerCameraRoot;
    
    public CinemachineVirtualCamera virtualCamera;

    public bool isKiller = false;
    
    // Start is called before the first frame update
    void Start()
    {
        int randomNumber = Random.Range(0,outlookPool.transform.childCount);
        outlookPool.transform.GetChild(0).gameObject.SetActive(false);
        outlookPool.transform.GetChild(randomNumber).gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetAimedFeedback(bool isAimed)
    {
        if (aimedIndicator != null && aimedIndicator.activeSelf != isAimed)
        {
            aimedIndicator.SetActive(isAimed);
        }
    }
    
    public void SetKillableIndicator(bool visible)
    {
        if (killableIndicator != null && killableIndicator.activeSelf != visible)
        {
            killableIndicator.SetActive(visible);
        }
    }

    public void BePossessed()
    {
        Debug.Log("BePossessed");
        
        // 停止 PolyPerfect 所有的协程，防止它自己乱走
        StopAllCoroutines();
    
        // 禁用所有AI逻辑和组件
        navMeshAgent.isStopped = true;
        people_WanderScript.enabled = false;
        navMeshAgent.enabled = false;
        
        // 激活所有Player逻辑和组件；切换摄像机；切换动画机
        thirdPersonController.enabled = true;
        playerInput.enabled = true;
        playerInput.ActivateInput();
        playerGameplay.enabled = true;
        
        animator.runtimeAnimatorController = playerAnimatorController;
        
        virtualCamera.Follow = playerCameraRoot.transform;
        
        SetAimedFeedback(false);

        if (isKiller)
        {
            gameObject.GetComponent<KillerScript>().enabled = true;
        }
        
        // 禁用 AI 脚本，防止干扰 ThirdPersonController 的 Move 调用
        this.enabled = false;
    }

    public void isKilled()
    {
        Destroy(gameObject.GetComponent<Common_WanderScript>());
    }
}
