using System.Collections;
using System.Collections.Generic;
using PolyPerfect;
using StarterAssets;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using Cinemachine;

public class PoliceScript : MonoBehaviour
{
    public float arrestRange = 3.0f;
    public LayerMask aiLayer;
    public float freezeDuration = 20f; // 停止移动的持续时间
    
    private float _freezeTimer = 0f;
    private bool _isFreezing = false;
    private CivilianScript _nearestAI;

    void Update()
    {
        // 1. 按下 E 触发全体禁足
        if (Input.GetKeyDown(KeyCode.E) && !_isFreezing)
        {
            StartFreeze();
        }

        // 2. 禁足倒计时处理
        if (_isFreezing)
        {
            _freezeTimer -= Time.deltaTime;
            
            // 禁足期间寻找最近的 AI 进行逮捕交互提示
            UpdateArrestIndicator();

            // 逮捕交互：禁足期间再次按 E（或你指定的键）进行逮捕
            if (Input.GetKeyDown(KeyCode.E))
            {
                TryArrest();
            }

            if (_freezeTimer <= 0)
            {
                EndFreeze();
            }
        }
    }

    void StartFreeze()
    {
        _isFreezing = true;
        _freezeTimer = freezeDuration;
        AIManager.Instance.SetAllAIMovement(false);
        Debug.Log("警察指令：全体停止移动！持续20秒。");
    }

    void EndFreeze()
    {
        _isFreezing = false;
        AIManager.Instance.SetAllAIMovement(true);
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
            
            // 逮捕成功后是否立即结束禁足？可选：
            // EndFreeze(); 
        }
    }
    
}
