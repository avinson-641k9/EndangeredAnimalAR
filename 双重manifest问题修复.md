# 双重 manifest.json 问题修复

## 🔍 问题根源

**发现关键问题**：项目中有**两个** `manifest.json` 文件：
1. `Packages/manifest.json` - 正确位置，干净版本
2. `ProjectSettings/Packages/manifest.json` - 错误位置，旧版本

**错误原因**：Unity 可能在读取 `ProjectSettings/Packages/manifest.json` 这个旧文件，其中包含：
- AR Foundation 5.1.5（不一致版本）
- 可能还有其他问题

## ✅ 已执行的修复

### 1. **删除了错误的 manifest.json**
```bash
# 删除 ProjectSettings 中的错误文件
rm -f ProjectSettings/Packages/manifest.json

# 备份到 ProjectSettings/Packages/manifest.json.backup
```

### 2. **确保了正确的 manifest.json**
- `Packages/manifest.json` 包含正确的包配置
- AR Foundation: 5.1.0（一致版本）
- 没有 JSON 注释
- 格式完全正确

### 3. **验证了文件位置**
```bash
# 现在只有这一个 manifest.json
ls -la Packages/manifest.json
# -rw-r--r--@ 1 yuanweijie staff 1876 Feb 18 13:33 Packages/manifest.json

# ProjectSettings/Packages/ 中没有 manifest.json
ls -la ProjectSettings/Packages/manifest.json 2>/dev/null || echo "不存在（正确）"
```

## 🚀 立即操作

### 步骤 1: 完全重启 Unity
1. **完全退出** Unity（如果还在运行）
2. 检查 Activity Monitor 确保没有 Unity 进程
3. **不要**点击错误对话框中的任何按钮

### 步骤 2: 重新打开项目
```bash
# 通过命令行
open /Users/yuanweijie/Projects/EndangeredAnimalAR

# 或通过 Unity Hub
```

### 步骤 3: 验证修复
1. Unity 应该**正常启动**，没有错误对话框
2. 包管理器开始导入包
3. Console 显示导入进度

## 📊 预期结果

### 包导入：
- ✅ 成功解析 `Packages/manifest.json`
- ✅ 开始导入 AR Foundation 5.1.0
- ✅ 导入其他核心包
- ✅ **不会出现** JSON 解析错误

### 编译状态：
- ✅ 0-3 个红色错误
- ✅ 5-15 个黄色警告
- ✅ 成功编译

## 🔧 技术细节

### 正确的文件结构：
```
EndangeredAnimalAR/
├── Packages/
│   └── manifest.json          # ✅ 正确位置，干净版本
├── ProjectSettings/
│   └── Packages/
│       └── manifest.json.backup  # ⚠️ 备份文件，已删除原文件
└── ...
```

### 包版本对比：
- **之前**（错误文件）: AR Foundation 5.1.5
- **现在**（正确文件）: AR Foundation 5.1.0（与 Unity 2022.3 兼容）

### 为什么会有两个文件：
1. Unity 旧版本可能在不同位置存储包配置
2. 项目迁移或升级时可能产生重复文件
3. Unity 包管理器可能读取错误的位置

## ⚠️ 注意事项

### 如果仍然看到错误：
1. **检查 Unity 是否完全关闭**
2. **清理 Unity 缓存**（之前已执行）
3. **验证只有一个 manifest.json**

### 清理缓存命令：
```bash
# 如果仍然有问题，执行这些
rm -rf ~/Library/Caches/Unity/
rm -rf Library/
```

### 验证步骤：
```bash
# 验证 manifest.json 位置和内容
find . -name "manifest.json" -type f
python3 -m json.tool Packages/manifest.json
```

## 🎯 成功标志

修复成功后：
1. ✅ Unity 正常启动，没有错误对话框
2. ✅ 包管理器显示导入进度
3. ✅ Console 显示包正在下载/导入
4. ✅ 项目开始编译

### 验证编译状态：
1. 打开 Console 窗口
2. 查看错误数量
3. 预期：0-3 个红色错误

## 📁 备份信息

### 可用备份：
1. `ProjectSettings/Packages/manifest.json.backup` - 旧版本备份
2. `Packages/Backup/` - 所有 manifest 文件备份
3. `../Emergency_Backup_20260218_132500/` - 完整项目备份

### 恢复选项：
如果需要恢复旧配置：
```bash
cp ProjectSettings/Packages/manifest.json.backup Packages/manifest.json
```

---

**修复完成时间**: 2026-02-18 13:33 GMT+8  
**问题类型**: 双重 manifest.json 文件冲突  
**修复方法**: 删除错误文件，保留正确文件  
**验证状态**: 只有一个 manifest.json，JSON 格式正确  
**预期结果**: Unity 正常启动，包正常导入