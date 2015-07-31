# coding: utf-8
import csv
csvfile = file('day6.10  after order.csv', 'rb')
reader = csv.reader(csvfile)
i=0
for line in reader:
	f=open(str(i)+'.txt','w')
	j=0
	for elem in line:
		if(j!=0):
			f.write(',')
		f.write(elem)
		j+=1
	i+=1
	f.close()
	#print type(line)
csvfile.close() 
raw_input()
