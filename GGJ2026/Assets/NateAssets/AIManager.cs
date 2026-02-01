using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AIManager : MonoBehaviour
{
    // 静态实例，全局唯一访问点
    public static AIManager Instance { get; private set; }

    [Header("Settings")]
    public string civilianTag = "Civilian";
    
    private List<CivilianScript> _allCivilians = new List<CivilianScript>();

    private void Awake()
    {
        // --- 单例模式核心逻辑 ---
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 如果场景中已存在实例，销毁新的，保持唯一
            return;
        }

        Instance = this;
        
        // 如果你希望这个管理器在切换场景时不被销毁，可以取消下面这行的注释
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CivilianScript[] foundCivilians = Object.FindObjectsByType<CivilianScript>(FindObjectsSortMode.None);
        _allCivilians = new List<CivilianScript>(foundCivilians);
        _allCivilians = _allCivilians.OrderBy(x => Random.value).ToList();

        foreach (var civ in _allCivilians)
        {
            if (!civ.isPolice && !civ.gameObject.GetComponent<PlayerGameplay>().enabled)
            {
                civ.isKiller = true;
                civ.gameObject.AddComponent<KillerScript>();

                break;
            }
        }
    }
    
    /// <summary>
    /// 强制执行脱离附身逻辑
    /// </summary>
    public void ForceUnpossess()
    {
        Debug.Log("AI Manager: 收到强制脱离指令。正在寻找最近的 Civilian AI...");
        
        // 这里后续可以调用之前提到的：
        // 1. 找到最近的 AI
        // 2. 将控制权切回该 AI
        // 3. 将杀手（Killer）状态重置
    }

    /// <summary>
    /// 获取场景中距离指定点最近的 CivilianAI
    /// </summary>
    public GameObject GetNearestCivilian(Vector3 position)
    {
        GameObject[] civilians = GameObject.FindGameObjectsWithTag(civilianTag);
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (GameObject civ in civilians)
        {
            float dist = Vector3.Distance(position, civ.transform.position);
            if (dist < minDistance && !civ.GetComponent<PlayerGameplay>().enabled)
            {
                minDistance = dist;
                nearest = civ;
            }
        }
        return nearest;
    }
    
    public void SetAllAIMovement(bool canMove)
    {
        CivilianScript[] foundCivilians = Object.FindObjectsByType<CivilianScript>(FindObjectsSortMode.None);
        _allCivilians = new List<CivilianScript>(foundCivilians);
        
        foreach (var ai in _allCivilians)
        {
            if (ai != null)
            {
                ai.SetMoving(canMove);
            }
        }
    }
}