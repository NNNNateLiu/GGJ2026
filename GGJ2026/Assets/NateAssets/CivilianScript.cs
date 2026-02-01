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
    [SerializeField] private GameObject currentOutlook;

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

    public bool isKiller = false;
    public bool isPolice = false;
    
    // Start is called before the first frame update
    void Start()
    {
        if (!isPolice)
        {
            int randomNumber = Random.Range(0,outlookPool.transform.childCount);
            outlookPool.transform.GetChild(0).gameObject.SetActive(false);
            currentOutlook = outlookPool.transform.GetChild(randomNumber).gameObject;
            currentOutlook.gameObject.SetActive(true);
        }
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

    public void BePossessed(bool isPlayer1)
    {
        Debug.Log("BePossessed");
        
        // 停止 PolyPerfect 所有的协程，防止它自己乱走
        StopAllCoroutines();
    
        // 禁用所有AI逻辑和组件
        navMeshAgent.isStopped = true;
        people_WanderScript.enabled = false;
        navMeshAgent.enabled = false;

        // 激活所有Player逻辑和组件；切换摄像机；切换动画机
        thirdPersonController.IsPlayer1 = isPlayer1;
        thirdPersonController.enabled = true;
        playerInput.ActivateInput();
        playerGameplay.enabled = true;
        
        gameObject.GetComponent<ThirdPersonController>()._mainCamera.SetActive(true);
        
        animator.runtimeAnimatorController = playerAnimatorController;
        
        SetAimedFeedback(false);

        if (isKiller)
        {
            gameObject.GetComponent<KillerScript>().enabled = true;
        }

        if (isPolice)
        {
            gameObject.GetComponent<PoliceScript>().enabled = true;
        }
        
        // 禁用 AI 脚本，防止干扰 ThirdPersonController 的 Move 调用
        this.enabled = false;
    }

    public void isKilled()
    {
        Destroy(gameObject.GetComponent<Common_WanderScript>());
        Invoke("DecayToSkeleton",1.5f);
    }

    private void DecayToSkeleton()
    {
        Destroy(navMeshAgent);
        Destroy(people_WanderScript);
        Destroy(thirdPersonController);
        Destroy(playerInput);
        Destroy(playerGameplay);
        Destroy(animator);
        Destroy(aimedIndicator);
        Destroy(killableIndicator);
        
        gameObject.tag = "Untagged";
        
        currentOutlook.SetActive(false);
        outlookPool.transform.GetChild(22).gameObject.SetActive(true);
        
        Destroy(civilianScript);
    }
    
    public void SetMoving(bool canMove)
    {
        // 如果你有 NavMeshAgent
        if (TryGetComponent<UnityEngine.AI.NavMeshAgent>(out var agent))
        {
            agent.isStopped = !canMove;
        }

        animator.SetBool("isWalking",false);
        animator.SetBool("isStanding",false);
        animator.SetBool("isTexting",false);
        animator.SetBool("isWaving",false);
        animator.SetBool("isPoliceOrdered",true);
        
        // 禁用或启用 Wander 逻辑脚本
        // 假设你的游走逻辑在 PolyPerfect.Common_WanderScript 中
        this.enabled = canMove;
        
    }
}
