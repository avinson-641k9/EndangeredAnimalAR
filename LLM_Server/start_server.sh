#!/bin/bash
# 启动濒危动物 LLM 服务器

echo "🐼 启动濒危动物交互科普系统 - LLM 服务器"
echo "========================================"

# 检查是否在虚拟环境中
if [ -z "$VIRTUAL_ENV" ]; then
    echo "🔧 激活 Python 虚拟环境..."
    source venv/bin/activate
fi

# 检查依赖是否安装
echo "📦 检查 Python 依赖..."
pip list | grep -E "(flask|torch|transformers)" || {
    echo "⚠️  依赖未完全安装，正在安装..."
    pip install -r requirements.txt
}

# 设置环境变量
export FLASK_APP=app.py
export FLASK_ENV=development
export PORT=5000

echo "🚀 启动服务器..."
echo "📡 服务地址: http://localhost:5000"
echo "📚 可用端点:"
echo "  - GET  /health         健康检查"
echo "  - GET  /animals        列出所有动物"
echo "  - GET  /animal_info/<name> 获取动物信息"
echo "  - POST /chat           对话接口"
echo ""
echo "按 Ctrl+C 停止服务器"

# 启动 Flask 服务器
python app.py