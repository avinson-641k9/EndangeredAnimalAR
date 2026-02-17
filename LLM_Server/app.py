#!/usr/bin/env python3
"""
濒危动物交互科普系统 - LLM 服务器
提供自然语言对话接口给 Unity AR 客户端
"""

from flask import Flask, request, jsonify
from flask_cors import CORS
import os
from dotenv import load_dotenv

# 加载环境变量
load_dotenv()

app = Flask(__name__)
CORS(app)  # 允许跨域请求

# 动物知识库（示例数据）
ANIMAL_KNOWLEDGE = {
    "大熊猫": {
        "description": "大熊猫是中国的国宝，主要生活在四川、陕西和甘肃的山区竹林。",
        "status": "易危",
        "population": "约1864只野生大熊猫",
        "threats": "栖息地碎片化、气候变化、竹子开花",
        "conservation": "建立自然保护区、人工繁殖、国际合作"
    },
    "东北虎": {
        "description": "东北虎是世界上最大的猫科动物，主要分布在中国东北和俄罗斯远东地区。",
        "status": "濒危",
        "population": "约500只野生东北虎",
        "threats": "栖息地丧失、盗猎、猎物减少",
        "conservation": "反盗猎巡逻、栖息地恢复、跨国保护"
    },
    "长江江豚": {
        "description": "长江江豚是中国特有的淡水豚类，生活在长江中下游干流及洞庭湖、鄱阳湖。",
        "status": "极危",
        "population": "约1012头",
        "threats": "航运、污染、非法捕捞、水利工程",
        "conservation": "迁地保护、建立保护区、人工繁殖"
    }
}

@app.route('/health', methods=['GET'])
def health_check():
    """健康检查端点"""
    return jsonify({
        "status": "healthy",
        "service": "Endangered Animal LLM Server",
        "version": "1.0.0"
    })

@app.route('/chat', methods=['POST'])
def chat():
    """处理用户对话请求"""
    try:
        data = request.json
        user_message = data.get('message', '')
        animal_name = data.get('animal', '')
        
        if not user_message:
            return jsonify({"error": "消息不能为空"}), 400
        
        # 简单的关键词匹配回复（实际应使用 LLM）
        response = generate_response(user_message, animal_name)
        
        return jsonify({
            "response": response,
            "animal": animal_name if animal_name else "未知",
            "timestamp": os.times().elapsed
        })
        
    except Exception as e:
        return jsonify({"error": str(e)}), 500

@app.route('/animal_info/<animal_name>', methods=['GET'])
def get_animal_info(animal_name):
    """获取特定动物的信息"""
    animal_name_cn = animal_name
    if animal_name_cn in ANIMAL_KNOWLEDGE:
        return jsonify(ANIMAL_KNOWLEDGE[animal_name_cn])
    else:
        return jsonify({
            "error": f"未找到动物 '{animal_name_cn}' 的信息",
            "available_animals": list(ANIMAL_KNOWLEDGE.keys())
        }), 404

@app.route('/animals', methods=['GET'])
def list_animals():
    """列出所有可用的动物"""
    return jsonify({
        "animals": list(ANIMAL_KNOWLEDGE.keys()),
        "count": len(ANIMAL_KNOWLEDGE)
    })

def generate_response(user_message, animal_name):
    """生成回复（简化版，实际应集成 LLM）"""
    user_message_lower = user_message.lower()
    
    # 问候语
    if any(word in user_message_lower for word in ['你好', '嗨', 'hello', 'hi']):
        return f"你好！我是{animal_name if animal_name else '濒危动物'}的虚拟助手。有什么可以帮助你的吗？"
    
    # 关于动物的问题
    if animal_name and animal_name in ANIMAL_KNOWLEDGE:
        animal_info = ANIMAL_KNOWLEDGE[animal_name]
        
        if any(word in user_message_lower for word in ['介绍', '是什么', 'describe', 'what']):
            return f"{animal_name}：{animal_info['description']}"
        
        if any(word in user_message_lower for word in ['状态', 'status', '保护级别']):
            return f"{animal_name}的保护状态是：{animal_info['status']}"
        
        if any(word in user_message_lower for word in ['数量', 'population', '有多少']):
            return f"目前野生{animal_name}的数量约为：{animal_info['population']}"
        
        if any(word in user_message_lower for word in ['威胁', 'threats', '危险']):
            return f"{animal_name}面临的主要威胁包括：{animal_info['threats']}"
        
        if any(word in user_message_lower for word in ['保护', 'conservation', '措施']):
            return f"保护{animal_name}的主要措施有：{animal_info['conservation']}"
    
    # 默认回复
    return "我对濒危动物保护很感兴趣！你可以问我关于大熊猫、东北虎或长江江豚的信息。"

if __name__ == '__main__':
    # 启动服务器
    port = int(os.getenv('PORT', 5000))
    debug = os.getenv('DEBUG', 'False').lower() == 'true'
    
    print(f"🚀 启动濒危动物 LLM 服务器...")
    print(f"📡 服务地址: http://localhost:{port}")
    print(f"🔧 调试模式: {debug}")
    print(f"🐼 可用动物: {', '.join(ANIMAL_KNOWLEDGE.keys())}")
    
    app.run(host='0.0.0.0', port=port, debug=debug)