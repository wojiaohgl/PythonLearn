import jieba
f=open("data.txt",'r')
for lines in f.readlines():
	seg_list = jieba.cut(lines, cut_all=False)
	print("Default Mode: " + "/ ".join(seg_list))
