# 实验报告一
学院：软件学院 班级：一班 学号：3017218060 姓名：刘杭学 日期：2019年 3月 14日  
## 一 功能概述
1.命令行可以是例如 -f D:\csharp\MyQrCode\MyQrCodin\Debug\text.txt，-f表示QrCode信息放在-f的后面的dataqrcode.txt文件中。  
2.也可以从数据库中读取 例如 -m server=localhost;database=gy1;uid=root;pwd=a1102134015 myTable  
3.还可以从excel表中读取 例如 -e D:\csharp\MyQrCode\MyQrCode\bin\Debug\test.xls  
4.二维码保存在bin目录下的 图片文件夹下  
5.如果不含参数则在控制台读取字符串，且在控制台输出二维码。  
## 二 项目特色
1.提供了纠错机制只能输出字符在（4-32）字符串的二维码，否则输出WrongMessage。  
2.可以从文本文件中批量读取数据，并将生成的二维码保存在文件夹中。  
3.可以从数据库中批量读取数据，并将生成的二维码保存在文件夹中。  
4.可以从Excel表中批量读取数据,并将生成的二维码保存在文件夹中.  
## 三 代码总量
231行  
## 四 工作时间
约14个小时
## 五 知识点总结图（Concept MAP） 
![]https://github.com/HangxueLiu/myExp/blob/master/MyQrCode/picture/1.png
## 六 结论
1.了解到了QrCode的编码原理  
2.学会使用第三方提供的包  
3.学会了程序对文件的读取和保存  
4.学会了程序与数据库和Excel表的连接  
