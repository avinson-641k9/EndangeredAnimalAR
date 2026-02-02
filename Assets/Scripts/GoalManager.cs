using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 教育目标管理器 - 管理学习目标、进度和成就系统
/// </summary>
public class GoalManager : MonoBehaviour
{
    [Header("教育目标配置")]
    public List<EducationGoal> educationGoals = new List<EducationGoal>();
    
    [Header("进度回调")]
    public UnityEvent<int> onGoalProgressChanged;  // 参数: 目标ID
    public UnityEvent<int> onGoalCompleted;        // 参数: 目标ID
    public UnityEvent<int> onAchievementUnlocked;  // 参数: 成就ID
    
    // 当前进度
    private Dictionary<int, float> goalProgress = new Dictionary<int, float>();
    private HashSet<int> completedGoals = new HashSet<int>();
    private HashSet<int> unlockedAchievements = new HashSet<int>();
    
    // 学习统计数据
    private LearningStats stats = new LearningStats();
    
    [System.Serializable]
    public class EducationGoal
    {
        public int id;
        public string title;
        public string description;
        public GoalType type;
        public float targetValue;
        public string unlockMessage;
        public Sprite icon;
        public Color color;
    }
    
    [System.Serializable]
    public class Achievement
    {
        public int id;
        public string title;
        public string description;
        public string unlockMessage;
        public Sprite icon;
        public AchievementTier tier;
    }
    
    public enum GoalType
    {
        KnowledgeQuiz,      // 知识问答
        InteractionCount,   // 互动次数
        TimeSpent,         // 使用时长
        SpeciesLearned,    // 学习物种数
        ShareCount,        // 分享次数
        ConservationAction // 保护行动
    }
    
    public enum AchievementTier
    {
        Bronze,
        Silver,
        Gold,
        Platinum
    }
    
    [System.Serializable]
    public class LearningStats
    {
        public int totalKnowledgeQuizzes = 0;
        public int correctAnswers = 0;
        public float totalInteractionTime = 0f;
        public int totalInteractions = 0;
        public int speciesLearned = 0;
        public int totalShares = 0;
        public int conservationActions = 0;
        public float learningStreak = 0f;  // 连续学习天数
    }
    
    void Start()
    {
        InitializeGoals();
        LoadProgress();
    }
    
    /// <summary>
    /// 初始化目标系统
    /// </summary>
    private void InitializeGoals()
    {
        // 初始化进度字典
        foreach (var goal in educationGoals)
        {
            if (!goalProgress.ContainsKey(goal.id))
            {
                goalProgress[goal.id] = 0f;
            }
        }
        
        // 设置默认教育目标
        if (educationGoals.Count == 0)
        {
            SetupDefaultGoals();
        }
    }
    
    /// <summary>
    /// 设置默认教育目标
    /// </summary>
    private void SetupDefaultGoals()
    {
        // 目标1: 完成知识问答
        educationGoals.Add(new EducationGoal
        {
            id = 1,
            title = "知识达人",
            description = "完成10次动物知识问答",
            type = GoalType.KnowledgeQuiz,
            targetValue = 10f,
            unlockMessage = "恭喜！你已经成为动物知识达人！",
            color = Color.blue
        });
        
        // 目标2: 互动次数
        educationGoals.Add(new EducationGoal
        {
            id = 2,
            title = "动物朋友",
            description = "与动物互动50次",
            type = GoalType.InteractionCount,
            targetValue = 50f,
            unlockMessage = "你已经是动物们的好朋友了！",
            color = Color.green
        });
        
        // 目标3: 学习时长
        educationGoals.Add(new EducationGoal
        {
            id = 3,
            title = "环保卫士",
            description = "累计学习30分钟",
            type = GoalType.TimeSpent,
            targetValue = 30f, // 分钟
            unlockMessage = "你正在成为一名优秀的环保卫士！",
            color = Color.yellow
        });
        
        // 目标4: 学习物种数
        educationGoals.Add(new EducationGoal
        {
            id = 4,
            title = "生物多样性探索者",
            description = "学习5种不同濒危动物",
            type = GoalType.SpeciesLearned,
            targetValue = 5f,
            unlockMessage = "你已经了解了多种濒危动物！",
            color = Color.magenta
        });
        
        // 目标5: 分享次数
        educationGoals.Add(new EducationGoal
        {
            id = 5,
            title = "环保传播者",
            description = "分享体验3次",
            type = GoalType.ShareCount,
            targetValue = 3f,
            unlockMessage = "感谢你传播环保理念！",
            color = Color.cyan
        });
    }
    
    /// <summary>
    /// 记录知识问答完成
    /// </summary>
    public void RecordKnowledgeQuiz(bool isCorrect)
    {
        stats.totalKnowledgeQuizzes++;
        if (isCorrect) stats.correctAnswers++;
        
        UpdateGoalProgress(GoalType.KnowledgeQuiz, 1f);
        
        // 计算知识准确率
        float accuracy = stats.totalKnowledgeQuizzes > 0 ? 
            (float)stats.correctAnswers / stats.totalKnowledgeQuizzes : 0f;
        
        Debug.Log($"[GoalManager] 知识问答完成，准确率: {(accuracy * 100):F1}%");
    }
    
    /// <summary>
    /// 记录互动
    /// </summary>
    public void RecordInteraction()
    {
        stats.totalInteractions++;
        
        UpdateGoalProgress(GoalType.InteractionCount, 1f);
        
        Debug.Log($"[GoalManager] 互动记录，总次数: {stats.totalInteractions}");
    }
    
    /// <summary>
    /// 记录学习时长
    /// </summary>
    public void RecordLearningTime(float minutes)
    {
        stats.totalInteractionTime += minutes;
        
        UpdateGoalProgress(GoalType.TimeSpent, minutes);
        
        Debug.Log($"[GoalManager] 学习时长记录: {minutes:F1}分钟，总计: {stats.totalInteractionTime:F1}分钟");
    }
    
    /// <summary>
    /// 记录学习物种
    /// </summary>
    public void RecordSpeciesLearned(string speciesName)
    {
        if (!IsSpeciesAlreadyLearned(speciesName))
        {
            stats.speciesLearned++;
            UpdateGoalProgress(GoalType.SpeciesLearned, 1f);
            
            Debug.Log($"[GoalManager] 新物种学习: {speciesName}，总计: {stats.speciesLearned}");
        }
    }
    
    /// <summary>
    /// 检查物种是否已学习
    /// </summary>
    private bool IsSpeciesAlreadyLearned(string speciesName)
    {
        // 这里可以实现更复杂的逻辑来跟踪已学习的物种
        return stats.speciesLearned >= GetSpeciesCount(speciesName);
    }
    
    /// <summary>
    /// 获取物种计数（简化版）
    /// </summary>
    private int GetSpeciesCount(string speciesName)
    {
        // 简化实现，实际项目中应该有更复杂的逻辑
        return stats.speciesLearned;
    }
    
    /// <summary>
    /// 记录分享
    /// </summary>
    public void RecordShare()
    {
        stats.totalShares++;
        
        UpdateGoalProgress(GoalType.ShareCount, 1f);
        
        Debug.Log($"[GoalManager] 分享记录，总次数: {stats.totalShares}");
    }
    
    /// <summary>
    /// 记录保护行动
    /// </summary>
    public void RecordConservationAction()
    {
        stats.conservationActions++;
        
        UpdateGoalProgress(GoalType.ConservationAction, 1f);
        
        Debug.Log($"[GoalManager] 保护行动记录，总次数: {stats.conservationActions}");
    }
    
    /// <summary>
    /// 更新目标进度
    /// </summary>
    private void UpdateGoalProgress(GoalType type, float value)
    {
        foreach (var goal in educationGoals)
        {
            if (goal.type == type)
            {
                if (goalProgress.ContainsKey(goal.id))
                {
                    goalProgress[goal.id] += value;
                    
                    // 确保进度不超过目标值
                    goalProgress[goal.id] = Mathf.Min(goalProgress[goal.id], goal.targetValue);
                    
                    // 检查是否完成目标
                    if (goalProgress[goal.id] >= goal.targetValue && !completedGoals.Contains(goal.id))
                    {
                        CompleteGoal(goal.id);
                    }
                    
                    // 触发进度更新事件
                    onGoalProgressChanged?.Invoke(goal.id);
                    
                    Debug.Log($"[GoalManager] 目标 {goal.title} 进度: {goalProgress[goal.id]:F1}/{goal.targetValue:F1}");
                }
            }
        }
    }
    
    /// <summary>
    /// 完成目标
    /// </summary>
    private void CompleteGoal(int goalId)
    {
        completedGoals.Add(goalId);
        
        // 触发目标完成事件
        onGoalCompleted?.Invoke(goalId);
        
        // 查找目标并显示解锁消息
        var goal = educationGoals.Find(g => g.id == goalId);
        if (goal != null)
        {
            Debug.Log($"[GoalManager] 恭喜！目标完成: {goal.unlockMessage}");
            
            // 解锁相关成就
            UnlockRelatedAchievements(goalId);
        }
    }
    
    /// <summary>
    /// 解锁相关成就
    /// </summary>
    private void UnlockRelatedAchievements(int goalId)
    {
        // 根据完成的目标解锁相应成就
        switch (goalId)
        {
            case 1: // 知识达人
                UnlockAchievement(101, "知识新星");
                break;
            case 2: // 动物朋友
                UnlockAchievement(102, "动物守护者");
                break;
            case 3: // 环保卫士
                UnlockAchievement(103, "环保先锋");
                break;
            case 4: // 生物多样性探索者
                UnlockAchievement(104, "生态使者");
                break;
            case 5: // 环保传播者
                UnlockAchievement(105, "理念传播者");
                break;
        }
    }
    
    /// <summary>
    /// 解锁成就
    /// </summary>
    private void UnlockAchievement(int achievementId, string achievementName)
    {
        if (!unlockedAchievements.Contains(achievementId))
        {
            unlockedAchievements.Add(achievementId);
            
            // 触发成就解锁事件
            onAchievementUnlocked?.Invoke(achievementId);
            
            Debug.Log($"[GoalManager] 成就解锁: {achievementName}!");
        }
    }
    
    /// <summary>
    /// 获取目标进度
    /// </summary>
    public float GetGoalProgress(int goalId)
    {
        return goalProgress.ContainsKey(goalId) ? goalProgress[goalId] : 0f;
    }
    
    /// <summary>
    /// 获取目标完成百分比
    /// </summary>
    public float GetGoalCompletionPercentage(int goalId)
    {
        var goal = educationGoals.Find(g => g.id == goalId);
        if (goal != null)
        {
            return goalProgress.ContainsKey(goalId) ? 
                Mathf.Clamp01(goalProgress[goalId] / goal.targetValue) : 0f;
        }
        return 0f;
    }
    
    /// <summary>
    /// 检查目标是否完成
    /// </summary>
    public bool IsGoalCompleted(int goalId)
    {
        return completedGoals.Contains(goalId);
    }
    
    /// <summary>
    /// 检查成就是否解锁
    /// </summary>
    public bool IsAchievementUnlocked(int achievementId)
    {
        return unlockedAchievements.Contains(achievementId);
    }
    
    /// <summary>
    /// 获取学习统计
    /// </summary>
    public LearningStats GetLearningStats()
    {
        return stats;
    }
    
    /// <summary>
    /// 重置进度（调试用）
    /// </summary>
    public void ResetProgress()
    {
        goalProgress.Clear();
        completedGoals.Clear();
        unlockedAchievements.Clear();
        
        foreach (var goal in educationGoals)
        {
            goalProgress[goal.id] = 0f;
        }
        
        stats = new LearningStats();
        
        Debug.Log("[GoalManager] 进度已重置");
    }
    
    /// <summary>
    /// 保存进度
    /// </summary>
    public void SaveProgress()
    {
        PlayerPrefs.SetInt("GoalManager_SpeciesLearned", stats.speciesLearned);
        PlayerPrefs.SetFloat("GoalManager_TotalInteractionTime", stats.totalInteractionTime);
        PlayerPrefs.SetInt("GoalManager_TotalInteractions", stats.totalInteractions);
        PlayerPrefs.SetInt("GoalManager_TotalShares", stats.totalShares);
        PlayerPrefs.SetInt("GoalManager_ConservationActions", stats.conservationActions);
        PlayerPrefs.SetInt("GoalManager_TotalQuizzes", stats.totalKnowledgeQuizzes);
        PlayerPrefs.SetInt("GoalManager_CorrectAnswers", stats.correctAnswers);
        
        // 保存目标进度
        foreach (var kvp in goalProgress)
        {
            PlayerPrefs.SetFloat($"GoalProgress_{kvp.Key}", kvp.Value);
        }
        
        // 保存完成的目标
        string completedString = "";
        foreach (int id in completedGoals)
        {
            completedString += id + ",";
        }
        PlayerPrefs.SetString("CompletedGoals", completedString);
        
        // 保存解锁的成就
        string achievementsString = "";
        foreach (int id in unlockedAchievements)
        {
            achievementsString += id + ",";
        }
        PlayerPrefs.SetString("UnlockedAchievements", achievementsString);
        
        PlayerPrefs.Save();
        
        Debug.Log("[GoalManager] 进度已保存");
    }
    
    /// <summary>
    /// 加载进度
    /// </summary>
    private void LoadProgress()
    {
        stats.speciesLearned = PlayerPrefs.GetInt("GoalManager_SpeciesLearned", 0);
        stats.totalInteractionTime = PlayerPrefs.GetFloat("GoalManager_TotalInteractionTime", 0f);
        stats.totalInteractions = PlayerPrefs.GetInt("GoalManager_TotalInteractions", 0);
        stats.totalShares = PlayerPrefs.GetInt("GoalManager_TotalShares", 0);
        stats.conservationActions = PlayerPrefs.GetInt("GoalManager_ConservationActions", 0);
        stats.totalKnowledgeQuizzes = PlayerPrefs.GetInt("GoalManager_TotalQuizzes", 0);
        stats.correctAnswers = PlayerPrefs.GetInt("GoalManager_CorrectAnswers", 0);
        
        // 加载目标进度
        foreach (var goal in educationGoals)
        {
            float progress = PlayerPrefs.GetFloat($"GoalProgress_{goal.id}", 0f);
            goalProgress[goal.id] = progress;
            
            // 检查是否完成
            if (progress >= goal.targetValue)
            {
                completedGoals.Add(goal.id);
            }
        }
        
        // 加载完成的目标
        string completedString = PlayerPrefs.GetString("CompletedGoals", "");
        if (!string.IsNullOrEmpty(completedString))
        {
            string[] ids = completedString.Split(',');
            foreach (string idStr in ids)
            {
                if (int.TryParse(idStr, out int id))
                {
                    completedGoals.Add(id);
                }
            }
        }
        
        // 加载解锁的成就
        string achievementsString = PlayerPrefs.GetString("UnlockedAchievements", "");
        if (!string.IsNullOrEmpty(achievementsString))
        {
            string[] ids = achievementsString.Split(',');
            foreach (string idStr in ids)
            {
                if (int.TryParse(idStr, out int id))
                {
                    unlockedAchievements.Add(id);
                }
            }
        }
        
        Debug.Log("[GoalManager] 进度已加载");
    }
    
    void OnDestroy()
    {
        SaveProgress();
    }
    
    /// <summary>
    /// 获取总体完成度
    /// </summary>
    public float GetOverallCompletion()
    {
        if (educationGoals.Count == 0) return 0f;
        
        float totalProgress = 0f;
        foreach (var goal in educationGoals)
        {
            totalProgress += GetGoalCompletionPercentage(goal.id);
        }
        
        return totalProgress / educationGoals.Count;
    }
    
    /// <summary>
    /// 获取完成的目标数量
    /// </summary>
    public int GetCompletedGoalsCount()
    {
        return completedGoals.Count;
    }
    
    /// <summary>
    /// 获取总目标数量
    /// </summary>
    public int GetTotalGoalsCount()
    {
        return educationGoals.Count;
    }
}