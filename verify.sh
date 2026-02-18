#!/bin/bash
echo "文件内容（前5行）:"
head -5 Packages/manifest.json
echo ""
echo "文件大小: $(wc -l < Packages/manifest.json) 行"
echo "文件大小: $(wc -c < Packages/manifest.json) 字节"
echo ""
echo "检查注释:"
grep -c "//" Packages/manifest.json | awk '{if($1==0) print "✅ 没有注释"; else print "❌ 发现注释"}'
