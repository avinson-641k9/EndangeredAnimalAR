# 🐼 Endangered Animal AR - 濒危动物AR交互科普系统

基于Unity AR与大语言模型的濒危动物交互科普系统，通过增强现实技术让用户与虚拟濒危动物互动，结合AI对话系统提供沉浸式科普教育体验。

## ✨ 核心功能

### 🎯 AR图像识别
- 扫描识别图生成3D濒危动物模型
- 实时追踪和位置更新
- 支持多种动物类型切换

### 🤖 AI智能对话
- 集成 DeepSeek 大语言模型
- 动物角色化对话（熊猫、东北虎、雪豹、江豚）
- 个性化性格设定和知识科普

### 📱 移动端优化
- 支持iOS和Android平台
- 优化的AR性能和电池管理
- 响应式UI设计

### 🎯 教育目标系统
- 互动进度追踪
- 学习成就解锁
- 知识掌握评估

## 🛠️ 技术栈

- **Unity 2022.3 LTS** - 游戏引擎
- **AR Foundation** - 跨平台AR框架
- **XR Interaction Toolkit** - AR交互组件
- **DeepSeek API** - 大语言模型服务
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

3. **配置API密钥**
   - 在 `StreamingAssets/api_config.json` 中添加DeepSeek API密钥

4. **运行LLM服务器**
   ```bash
   cd LLM_Server
   pip install -r requirements.txt
   python app.py
   ```

5. **构建和运行**
   - 选择目标平台（iOS/Android）
   - 构建项目并安装到设备

## 📁 项目结构

```
EndangeredAnimalAR/
├── Assets/
│   ├── Scenes/              # Unity场景文件
│   │   ├── UI.unity         # 主UI场景
│   │   └── EndangeredAnimalAR1.unity  # AR主场景
│   ├── Scripts/            # C#脚本
│   │   ├── AR/             # AR相关脚本
│   │   ├── LLM/            # LLM集成脚本
│   │   ├── UI/             # UI管理脚本
│   │   └── Gameplay/       # 游戏逻辑脚本
│   ├── XR/                 # XR配置
│   └── Resources/          # 资源文件
├── LLM_Server/             # Python LLM服务器
│   ├── app.py              # Flask服务器
│   └── requirements.txt    # Python依赖
├── Packages/               # Unity包配置
└── ProjectSettings/        # Unity项目设置
```

## 🔧 开发指南

### AR功能开发
1. 使用 `ARAnimalController.cs` 管理AR动物
2. 配置 `ImageLibraryManager.cs` 设置识别图库
3. 使用 `AnimalPrefabManager.cs` 管理动物预制件

### UI系统开发
1. 使用 `MainMenuManager.cs` 管理主菜单
2. 通过 `AnimalUIManager.cs` 控制动物UI
3. 使用 `UIPrefabCreator.cs` 创建UI元素

### LLM集成
1. 修改 `LLMClient.cs` 调整API设置
2. 配置 `ConfigLoader.cs` 加载API密钥
3. 扩展 `app.py` 添加新的对话功能

## 📚 文档

- [API设置指南](API_SETUP.md) - 如何配置DeepSeek API
- [UI实现指南](UI_IMPLEMENTATION_GUIDE.md) - UI系统开发指南
- [AR开发指南](AR_DEVELOPMENT_GUIDE.md) - AR功能开发指南

## 🤝 贡献

欢迎提交Issue和Pull Request！

1. Fork项目
2. 创建功能分支 (`git checkout -b feature/新功能`)
3. 提交更改 (`git commit -m '添加新功能'`)
4. 推送到分支 (`git push origin feature/新功能`)
5. 创建Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情

## 🙏 致谢

- Unity Technologies - AR Foundation
- DeepSeek - 大语言模型API
- 所有贡献者和测试者

---

**让科技为生态保护赋能，用AR唤醒对濒危动物的关爱！** 🌍🐾