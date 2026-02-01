using System;
using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using StarterAssets;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PoliceScript : MonoBehaviour
{
    public float arrestRange = 3.0f;
    public LayerMask aiLayer;
    public float freezeDuration = 20f; // 停止移动的持续时间
    
    private float _freezeTimer = 0f;
    private bool _isFreezing = false;
    private CivilianScript _nearestAI;

    public GameObject policeRemaind;

    private void Start()
    {
        policeRemaind = Instantiate(Resources.Load<GameObject>("Prefab/CountDown"), this.transform);
        policeRemaind.GetComponent<CountDown>().contextText = " s to unfreeze";
    }

    void Update()
    {
        // 1. 按下 E 触发全体禁足
        if ((Input.GetKeyDown(KeyCode.E) && this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1) || (Input.GetKeyDown(KeyCode.K) && !this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1))
        {
            if (!_isFreezing)
            {
                StartFreeze();
            }
        }

        // 2. 禁足倒计时处理
        if (_isFreezing)
        {
            _freezeTimer -= Time.deltaTime;
            policeRemaind.GetComponent<CountDown>().SetText(_freezeTimer);
            
            // 禁足期间寻找最近的 AI 进行逮捕交互提示
            UpdateArrestIndicator();

            // 逮捕交互：禁足期间再次按 E（或你指定的键）进行逮捕
            if ((Input.GetKeyDown(KeyCode.Q) && this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1) || (Input.GetKeyDown(KeyCode.L) && !this.gameObject.GetComponent<ThirdPersonController>().IsPlayer1))
            {
                TryArrest();
            }

            if (_freezeTimer <= 0)
            {
                EndFreeze();
                policeRemaind.GetComponent<CountDown>().ClearText();
            }
        }
    }

    void StartFreeze()
    {
        _isFreezing = true;
        _freezeTimer = freezeDuration;
        AIManager.Instance.SetAllAIMovement(false);
        AIManager.Instance.isPoliceStartArrest = true;
        
        CivilianScript[] foundCivilians = FindObjectsByType<CivilianScript>(FindObjectsSortMode.None);
        List<CivilianScript> _allCivilians = new List<CivilianScript>(foundCivilians);

        foreach (var civ in _allCivilians)
        {
            if (civ.killableIndicator != null)
            {
                civ.killableIndicator.transform.GetChild(0).gameObject.SetActive(false);
                civ.killableIndicator.transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        
        Debug.Log("警察指令：全体停止移动！持续20秒。");
    }

    void EndFreeze()
    {
        _isFreezing = false;
        AIManager.Instance.SetAllAIMovement(true);
        
        CivilianScript[] foundCivilians = FindObjectsByType<CivilianScript>(FindObjectsSortMode.None);
        List<CivilianScript> _allCivilians = new List<CivilianScript>(foundCivilians);

        foreach (var civ in _allCivilians)
        {
            if (civ.killableIndicator != null)
            {
                civ.killableIndicator.transform.GetChild(0).gameObject.SetActive(true);
                civ.killableIndicator.transform.GetChild(1).gameObject.SetActive(false);
            }
        }

        AIManager.Instance.isPoliceStartArrest = false;
        policeRemaind.GetComponent<CountDown>().ClearText();
        
        Debug.Log("禁足结束，AI 恢复移动。");
    }

    void UpdateArrestIndicator()
    {
        // 复用你之前的寻找最近 AI 的逻辑
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, arrestRange, aiLayer);
        float minDist = Mathf.Infinity;
        CivilianScript target = null;

        foreach (var hit in hitColliders)
        {
            CivilianScript civ = hit.GetComponent<CivilianScript>();
            if (civ != null)
            {
                float d = Vector3.Distance(transform.position, civ.transform.position);
                if (d < minDist)
                {
                    minDist = d;
                    target = civ;
                }
            }
        }

        // 这里的逻辑与 KillerScript 类似，显示指示器
        if (target != _nearestAI)
        {
            if (_nearestAI != null) _nearestAI.SetKillableIndicator(false); // 复用之前的 UI 接口
            if (target != null) target.SetKillableIndicator(true);
            _nearestAI = target;
        }
    }

    void TryArrest()
    {
        if (_nearestAI != null)
        {
            Debug.Log($"已逮捕 AI: {_nearestAI.name}");
            // 执行逮捕逻辑：例如销毁 AI，或者播放带走动画
            Destroy(_nearestAI.gameObject);
            _nearestAI = null;
            
            //1s 后强制转移到其他非玩家非killer的AI身上
            Invoke("ForceUnPossess",1f);
            
            // 逮捕成功后是否立即结束禁足？可选：
            EndFreeze(); 
        }
    }

    public void ForceUnPossess()
    {
        CivilianScript nearestCiv = AIManager.Instance.GetNearestCivilian(gameObject.transform.position)
            .GetComponent<CivilianScript>();
            
        gameObject.GetComponent<PlayerGameplay>().UnPossessed(nearestCiv);
        bool isPlayer1 = gameObject.GetComponent<ThirdPersonController>().IsPlayer1;
        nearestCiv.BePossessed(isPlayer1);
    }
    
}
