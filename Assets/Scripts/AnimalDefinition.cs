using UnityEngine;

/// <summary>
/// 动物定义脚本 - 定义单个动物的属性和行为
/// </summary>
[CreateAssetMenu(fileName = "NewAnimal", menuName = "Endangered Animals/New Animal Definition")]
public class AnimalDefinition : ScriptableObject
{
    [Header("动物基本信息")]
    public string animalName;           // 动物名称
    public string scientificName;       // 学名
    public string animalType;          // 动物类型（熊猫、老虎等）
    
    [Header("生物学信息")]
    public string habitat;             // 栖息地
    public string diet;                // 食物
    public string conservationStatus;  // 保护状态
    [TextArea] public string funFacts; // 趣味知识
    
    [Header("AR显示设置")]
    public float defaultScale = 1.0f;      // 默认缩放
    public Vector3 spawnOffset = Vector3.zero; // 生成偏移
    public float recognitionDistance = 5f;  // 识别距离
    
    [Header("AI对话设置")]
    [TextArea] public string personalityDescription; // 性格描述
    [TextArea] public string welcomeMessage;        // 欢迎消息
    public string voiceType = "friendly";           // 音色类型
    
    [Header("动画设置")]
    public string idleAnimation = "idle";    // 待机动画
    public string interactionAnimation = "wave"; // 互动动画
    public float animationSpeed = 1.0f;      // 动画速度
    
    [Header("音效设置")]
    public AudioClip ambientSound;    // 环境音效
    public AudioClip interactionSound; // 互动音效
    [Range(0f, 1f)] public float soundVolume = 0.7f; // 音量
    
    [Header("教育内容")]
    [TextArea] public string conservationTips; // 保护建议
    [TextArea] public string educationalContent; // 教育内容
    
    /// <summary>
    /// 获取动物显示名称
    /// </summary>
    public string GetDisplayName()
    {
        return string.IsNullOrEmpty(animalName) ? name : animalName;
    }
    
    /// <summary>
    /// 获取动物简介
    /// </summary>
    public string GetIntroduction()
    {
        return $"我是{GetDisplayName()}，学名{scientificName}。{funFacts}";
    }
    
    /// <summary>
    /// 获取保护信息
    /// </summary>
    public string GetConservationInfo()
    {
        return $"保护状态：{conservationStatus}\n栖息地：{habitat}\n食物：{diet}\n{conservationTips}";
    }
}