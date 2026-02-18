# 修复 VirtualArtifacts 错误

## 🔍 错误分析

**错误信息**:
```
Opening file VirtualArtifacts/Primary/0517374e1a7174be0aa2e368eaf8189a: No such file or directory
```

**原因**:
1. Unity 在清理缓存后，某些内部引用仍然指向旧的缓存路径
2. `0517374e1a7174be0aa2e368eaf8189a` 是 `AutoCleanSamples.cs` 的 GUID
3. Unity 在重新编译时尝试访问旧的虚拟文件路径

## 🚀 立即修复方案

### 方案 A: 删除并重新创建脚本文件（推荐）
```bash
cd /Users/yuanweijie/Projects/EndangeredAnimalAR

# 1. 备份脚本
cp Assets/Editor/AutoCleanSamples.cs Assets/Editor/AutoCleanSamples.cs.backup

# 2. 删除脚本和 meta 文件
rm Assets/Editor/AutoCleanSamples.cs
rm Assets/Editor/AutoCleanSamples.cs.meta

# 3. 重新创建脚本（如果需要）
# 或者暂时不使用这个脚本
```

### 方案 B: 清理 Unity 内部缓存
```bash
# 清理 Unity 的全局缓存
rm -rf ~/Library/Caches/Unity/
rm -rf ~/Library/Unity/

# 清理项目特定缓存
rm -rf Library/Bee/
rm -rf Library/Artifacts/
```

### 方案 C: 重新生成所有 meta 文件
```bash
# 删除所有 meta 文件（危险！先备份）
find . -name "*.meta" -type f -delete

# 重新打开 Unity，会自动重新生成 meta 文件
```

## 🛠️ 安全执行步骤

### 步骤 1: 创建安全备份
```bash
cd /Users/yuanweijie/Projects/EndangeredAnimalAR
META_BACKUP="../Meta_Backup_$(date +%Y%m%d_%H%M%S)"
mkdir -p "$META_BACKUP"
find . -name "*.meta" -type f -exec cp {} "$META_BACKUP/" \;
echo "Meta 文件备份到: $META_BACKUP"
```

### 步骤 2: 删除问题脚本
```bash
# 只删除有问题的脚本
rm -f Assets/Editor/AutoCleanSamples.cs
rm -f Assets/Editor/AutoCleanSamples.cs.meta
```

### 步骤 3: 清理 Unity 全局缓存
```bash
# 安全清理，不影响项目
rm -rf ~/Library/Caches/Unity/AssetDatabase
rm -rf ~/Library/Caches/Unity/ScriptAssemblies
```

### 步骤 4: 重新打开 Unity
1. 完全退出 Unity
2. 重新打开项目
3. 等待重新编译

## 📊 预期结果

### 修复后：
1. ✅ VirtualArtifacts 错误消失
2. ✅ 项目正常编译
3. ✅ 没有文件引用错误

### 可能的影响：
1. ⚠️ 需要重新编译所有脚本
2. ⚠️ 可能需要重新导入一些资源
3. ⚠️ 编译时间可能稍长

## 🔧 如果错误仍然存在

### 进一步诊断：
```bash
# 检查是否有其他文件引用这个 GUID
find . -type f -exec grep -l "0517374e1a7174be0aa2e368eaf8189a" {} \; 2>/dev/null

# 检查 Unity 日志中的其他错误
grep -n "VirtualArtifacts\|0517374e" ~/Library/Logs/Unity/Editor.log | head -10
```

### 终极解决方案：
1. **创建全新项目** - 导入核心资源
2. **使用备份恢复** - 回到可工作状态
3. **联系 Unity 支持** - 提供完整错误日志

## ⚠️ 注意事项

### 不要执行的操作：
- ❌ 不要删除所有 meta 文件（会丢失所有资源引用）
- ❌ 不要删除 Library/ 文件夹外的其他 Unity 缓存
- ❌ 不要修改 GUID（会导致资源丢失）

### 安全操作：
- ✅ 只删除有问题的脚本文件
- ✅ 清理全局缓存是安全的
- ✅ 重新打开 Unity 会自动修复很多问题

## 🎯 成功标准

修复成功后：
1. ✅ 没有 VirtualArtifacts 错误
2. ✅ 项目正常编译
3. ✅ 所有资源正常显示
4. ✅ 可以进入 Play 模式

---

**错误类型**: Unity 内部缓存引用错误  
**影响文件**: AutoCleanSamples.cs (GUID: 0517374e1a7174be0aa2e368eaf8189a)  
**推荐方案**: 删除问题脚本 + 清理 Unity 缓存  
**预计耗时**: 2-5分钟