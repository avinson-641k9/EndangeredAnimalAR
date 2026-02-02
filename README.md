# 🐼 Endangered Animal AR - 濒危动物AR交互科普系统

基于Unity AR与大语言模型的濒危动物交互科普系统，通过增强现实技术让用户与虚拟濒危动物互动，结合AI对话系统提供沉浸式科普教育体验。

## ✨ 核心功能

### 🎯 AR图像识别
- 扫描识别图生成3D濒危动物模型
- 实时追踪和位置更新
- 支持多种动物类型切换

### 🤖 AI智能对话
- 集成阿里通义千问大语言模型
- 动物角色化对话（熊猫、东北虎、雪豹、江豚）
- 个性化性格设定和知识科普

### 📱 移动端优化
- 支持iOS和Android平台
- 优化的AR性能和电池管理
- 响应式UI设计

## 🛠️ 技术栈

- **Unity 2022.3 LTS** - 游戏引擎
- **AR Foundation** - 跨平台AR框架
- **XR Interaction Toolkit** - AR交互组件
- **阿里通义千问API** - 大语言模型服务
- **C#** - 编程语言

## 🚀 快速开始

### 环境要求
- Unity 2022.3 LTS 或更高版本
- iOS 14+ 或 Android 8.0+ 设备
- 支持ARCore (Android) 或 ARKit (iOS)

### 安装步骤
1. **克隆仓库**
   ```bash
   git clone https://github.com/avinson-641k9/EndangeredAnimalAR.git
   ```

2. **打开Unity项目**
   - 使用Unity Hub打开项目文件夹
   - 等待依赖包自动导入

3. **配置API密钥**
   - 复制 `Assets/StreamingAssets/api_config.example.json` 为 `api_config.json`
   - 填入阿里云API密钥

4. **导入3D模型**
   - 下载濒危动物3D模型（FBX/OBJ格式）
   - 拖放到 `Assets/Models/` 文件夹
   - 创建Prefab并配置到 `AnimalARManager`

5. **创建AR识别图**
   - 准备512x512 PNG格式识别图
   - 在Unity中创建Reference Image Library
   - 配置图像名称与动物Prefab的映射

6. **构建项目**
   - 选择目标平台（iOS/Android）
   - 构建并运行到设备

## 📁 项目结构

```
EndangeredAnimalAR/
├── Assets/
│   ├── Scripts/              # C#脚本
│   │   ├── AnimalARManager.cs    # AR动物管理器
│   │   └── AnimalChatManager.cs  # AI对话管理器
│   ├── StreamingAssets/      # 配置文件
│   │   └── api_config.json   # API配置
│   ├── Models/              # 3D模型资源
│   ├── Scenes/              # Unity场景
│   └── Resources/           # 识别图等资源
├── ProjectSettings/         # Unity项目设置
├── Packages/               # Unity包管理
└── README.md              # 项目说明
```

## 🎮 使用指南

### 1. 扫描识别图
- 打开App，扫描预设的动物识别图
- 等待3D动物模型生成

### 2. 与动物互动
- 点击动物触发对话
- 通过语音或文字与动物交流
- 学习濒危动物保护知识

### 3. 切换动物
- 扫描不同的识别图切换动物
- 每种动物有独特的性格和知识

### 4. 学习模式
- 问答模式：向动物提问
- 故事模式：听动物讲述生存故事
- 挑战模式：完成保护知识问答

## 🔧 开发指南

### 添加新动物
1. 准备3D模型和识别图
2. 在 `AnimalARManager` 中添加动物映射
3. 在 `AnimalChatManager` 中添加性格配置
4. 测试AR识别和对话功能

### 扩展对话系统
- 修改 `AnimalPersonalities` 字典添加新性格
- 调整API参数优化响应质量
- 添加对话历史管理功能

### 性能优化
- 使用低多边形模型
- 优化材质和贴图
- 实现对象池管理

## 📚 教育价值

### 科普内容
- 濒危动物生态习性
- 栖息地保护知识
- 人类活动对生态的影响
- 保护措施和成功案例

### 学习目标
- 提高环保意识
- 了解生物多样性重要性
- 激发科学探索兴趣
- 培养责任感

## 🤝 贡献指南

欢迎贡献代码、模型、文档或建议！

1. Fork 本仓库
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 开启 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 致谢

- Unity Technologies - AR Foundation 和 XR Interaction Toolkit
- 阿里云 - 通义千问大语言模型API
- 开源社区 - 提供的3D模型和工具
- 所有濒危动物保护工作者

## 📞 联系方式

项目作者：David
- GitHub: [@avinson-641k9](https://github.com/avinson-641k9)
- 项目地址：https://github.com/avinson-641k9/EndangeredAnimalAR

---

**让我们一起用科技保护地球的珍贵生命！** 🌍🐾