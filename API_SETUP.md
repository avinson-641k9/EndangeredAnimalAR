# API 配置指南

## DeepSeek API 配置

本项目使用 DeepSeek API 来实现动物对话功能。以下是配置步骤：

### 1. 配置文件位置
- 配置文件路径：`Assets/StreamingAssets/api_config.json`
- 已经使用您提供的 API Key 配置完毕

### 2. 配置文件内容
```json
{
  "deepseek_api_key": "sk-4b16e6b8e1eb45cba6c9381a2d5740b0",
  "deepseek_model": "deepseek-chat",
  "deepseek_endpoint": "https://api.deepseek.com/chat/completions"
}
```

### 3. API 限制和注意事项
- DeepSeek API 有一定的请求频率限制
- 请确保网络连接稳定
- API Key 请勿公开分享

### 4. 错误处理
如果遇到 API 相关错误，请检查：
- API Key 是否正确
- 网络连接是否正常
- API 额度是否充足

### 5. 替换 API
如需更换其他 API，需要修改以下文件：
- `Assets/Scripts/AnimalChatManager.cs` - 修改 API 调用逻辑
- `Assets/StreamingAssets/api_config.json` - 更新配置信息