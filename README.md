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
## 五 知识点总结图（Concept MAP）  ![](https://github.com/HangxueLiu/myExp/blob/master/MyQrCode/picture/1.png)
## 六 结论
1.了解到了QrCode的编码原理  
2.学会使用第三方提供的包  
3.学会了程序对文件的读取和保存  
4.学会了程序与数据库和Excel表的连接  


# 实验二
实验名称
Windows Form实现MIDI音乐文件的播放APP


实验目的
1) 理解和掌握基于Windows Form的APP应用程序开发
2) 学习使用MIDI Toolkit完成Midi音乐文件的播放
3) 理解C# MIDI Toolkit内部的类、event、delegate构成机制
4) 理解基于Windows Form的event的GUI界面开发方法

实验内容
1) 使用C# MIDI Toolkit提供的源程序，在Visual Studio中建立相应的解决方案。
2) 能够成功编译C# MIDI Toolkit提供的演示程序。并能正常播放MIDI文件。
3) 理解演示程序的内部工作机制: 参照C# MIDI Toolkit文章内容，理解Event/Delegate方式实现的模块间的耦合机制，各种类的继承关系等。
4) 对GUI界面中的控件大小、位置进行完善，使之能够随APP界面大小自动调整其自身大小。需要使用相应的Event完成此项工作。
5) 其他GUI界面的用户体验提升：由同学们自己提出创新功能并实现。

实验要求
参实验内容完善详细的实验步骤，每位同学独立或两位同学结对编程完成实验。

实验分析
1.实验结果
1.1项目名称：实现空间大小的自适应
1.2操作步骤：

a.新建相关类AutoSizeFormClass
b.生成对象AutoSizeFormClass asc = new AutoSizeFormClass();
c.新增事件

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }
d.AutoSizeFormClass相关代码
 using System.Collections.Generic;
using System.Windows.Forms;

class AutoSizeFormClass
{
    //(1).声明结构,只记录窗体和其控件的初始位置和大小。
    public struct controlRect
    {
        public int Left;
        public int Top;
        public int Width;
        public int Height;
    }
    //(2).声明 1个对象
    //注意这里不能使用控件列表记录 List nCtrl;，因为控件的关联性，记录的始终是当前的大小。
    //      public List oldCtrl= new List();//这里将西文的大于小于号都过滤掉了，只能改为中文的，使用中要改回西文
    public List<controlRect> oldCtrl = new List<controlRect>();
    int ctrlNo = 0;//1;
                   //(3). 创建两个函数
                   //(3.1)记录窗体和其控件的初始位置和大小,
    public void controllInitializeSize(Control mForm)
    {
        controlRect cR;
        cR.Left = mForm.Left; cR.Top = mForm.Top; cR.Width = mForm.Width; cR.Height = mForm.Height;
        oldCtrl.Add(cR);//第一个为"窗体本身",只加入一次即可
        AddControl(mForm);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
                          //this.WindowState = (System.Windows.Forms.FormWindowState)(2);//记录完控件的初始位置和大小后，再最大化
                          //0 - Normalize , 1 - Minimize,2- Maximize
    }
    private void AddControl(Control ctl)
    {
        foreach (Control c in ctl.Controls)
        {  //**放在这里，是先记录控件的子控件，后记录控件本身
           //if (c.Controls.Count > 0)
           //    AddControl(c);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            controlRect objCtrl;
            objCtrl.Left = c.Left; objCtrl.Top = c.Top; objCtrl.Width = c.Width; objCtrl.Height = c.Height;
            oldCtrl.Add(objCtrl);
            //**放在这里，是先记录控件本身，后记录控件的子控件
            if (c.Controls.Count > 0)
                AddControl(c);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
        }
    }
    //(3.2)控件自适应大小,
    public void controlAutoSize(Control mForm)
    {
        if (ctrlNo == 0)
        { //*如果在窗体的Form1_Load中，记录控件原始的大小和位置，正常没有问题，但要加入皮肤就会出现问题，因为有些控件如dataGridView的的子控件还没有完成，个数少
          //*要在窗体的Form1_SizeChanged中，第一次改变大小时，记录控件原始的大小和位置,这里所有控件的子控件都已经形成
            controlRect cR;
            //  cR.Left = mForm.Left; cR.Top = mForm.Top; cR.Width = mForm.Width; cR.Height = mForm.Height;
            cR.Left = 0; cR.Top = 0; cR.Width = mForm.PreferredSize.Width; cR.Height = mForm.PreferredSize.Height;

            oldCtrl.Add(cR);//第一个为"窗体本身",只加入一次即可
            AddControl(mForm);//窗体内其余控件可能嵌套其它控件(比如panel),故单独抽出以便递归调用
        }
        float wScale = (float)mForm.Width / (float)oldCtrl[0].Width;//新旧窗体之间的比例，与最早的旧窗体
        float hScale = (float)mForm.Height / (float)oldCtrl[0].Height;//.Height;
        ctrlNo = 1;//进入=1，第0个为窗体本身,窗体内的控件,从序号1开始
        AutoScaleControl(mForm, wScale, hScale);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
    }
    private void AutoScaleControl(Control ctl, float wScale, float hScale)
    {
        int ctrLeft0, ctrTop0, ctrWidth0, ctrHeight0;
        //int ctrlNo = 1;//第1个是窗体自身的 Left,Top,Width,Height，所以窗体控件从ctrlNo=1开始
        foreach (Control c in ctl.Controls)
        { //**放在这里，是先缩放控件的子控件，后缩放控件本身
          //if (c.Controls.Count > 0)
          //   AutoScaleControl(c, wScale, hScale);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用
            ctrLeft0 = oldCtrl[ctrlNo].Left;
            ctrTop0 = oldCtrl[ctrlNo].Top;
            ctrWidth0 = oldCtrl[ctrlNo].Width;
            ctrHeight0 = oldCtrl[ctrlNo].Height;
            //c.Left = (int)((ctrLeft0 - wLeft0) * wScale) + wLeft1;//新旧控件之间的线性比例
            //c.Top = (int)((ctrTop0 - wTop0) * h) + wTop1;
            c.Left = (int)((ctrLeft0) * wScale);//新旧控件之间的线性比例。控件位置只相对于窗体，所以不能加 + wLeft1
            c.Top = (int)((ctrTop0) * hScale);//
            c.Width = (int)(ctrWidth0 * wScale);//只与最初的大小相关，所以不能与现在的宽度相乘 (int)(c.Width * w);
            c.Height = (int)(ctrHeight0 * hScale);//
            ctrlNo++;//累加序号
                     //**放在这里，是先缩放控件本身，后缩放控件的子控件
            if (c.Controls.Count > 0)
                AutoScaleControl(c, wScale, hScale);//窗体内其余控件还可能嵌套控件(比如panel),要单独抽出,因为要递归调用

            if (ctl is DataGridView)
            {
                DataGridView dgv = ctl as DataGridView;
                Cursor.Current = Cursors.WaitCursor;

                int widths = 0;
                for (int i = 0; i < dgv.Columns.Count; i++)
                {
                    dgv.AutoResizeColumn(i, DataGridViewAutoSizeColumnMode.AllCells);  // 自动调整列宽  
                    widths += dgv.Columns[i].Width;   // 计算调整列后单元列的宽度和                       
                }
                if (widths >= ctl.Size.Width)  // 如果调整列的宽度大于设定列宽  
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;  // 调整列的模式 自动  
                else
                    dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;  // 如果小于 则填充  

                Cursor.Current = Cursors.Default;
            }
        }
    }
}

1.3实际结果描述、结论：

成功实现了控件大小随页面的自适应变化

2.1项目名称： 添加listBox实现播放内容的可视化以及拖拽播放

2.2操作步骤：
a.拖拽控件增加listBox
b.新增事件
 private void Form1_DragEnter(object sender, DragEventArgs e) // 拖拽播放
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            listBox1.Items.Add(path);
        }


2.3实际结果描述、结论：
a.成功实现了添加listBox实现播放内容的可视化以及拖拽播放
b.listBox 每一行的内容一起组成了一个集合

3.1项目名称：实现音乐的单曲循环，顺序播放，和随机播放

3.2操作步骤：
a.拖拽控件添加
b.新增代码
   private void HandlePlayingCompleted(object sender, EventArgs e)
        {

            if (AutoLoop)
            {
                string path = listBox1.SelectedItem.ToString();
                Open(path); // 选中播放
            }
            else if(InOrder)
            {
                listBox1.SelectedIndex = (listBox1.SelectedIndex + 1) % (listBox1.Items.Count); //跳到下一个
            }else
            {
                int i = listBox1.SelectedIndex;
                Random rd = new Random();
                int r = rd.Next(0,listBox1.Items.Count);
                if(r==i)
                {
                    string path = listBox1.SelectedItem.ToString();
                    Open(path); // 选中播放
                }
                else
                {
                    listBox1.SelectedIndex = r;
                }
            }
        }
  private void Form1_Load(object sender, EventArgs e)
        {
            //listBox1.SelectionMode =;
            rb1.Checked = true; // 默认单曲循环
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                string path = listBox1.SelectedItem.ToString();
                Open(path); // 选中播放
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            AutoLoop = true;
            InOrder = false;
            randomOrder = false;
        }

        private void rb2_CheckedChanged(object sender, EventArgs e)
        {
            AutoLoop = false;
            InOrder = true;
            randomOrder = false;
        }

        private void rb3_CheckedChanged(object sender, EventArgs e)
        {
            AutoLoop = false;
            InOrder = false;
            randomOrder = true;
        }
    }

3.3实际结果描述、结论：

实现了音乐的单曲循环，顺序播放，随机播放
了解了线程保护，和跨线程调用的相关知识

4.1项目名称： 增加退出按钮
4.2操作步骤：

 private void quitBtn_Click(object sender, EventArgs e) //退出窗体
        {
            DialogResult result= MessageBox.Show(this, "Are you sure you want to quit the program? ", "Closing prompt ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
          {
                //this.Close();
                System.Environment.Exit(0);//这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            }
        }


4.3实际结果描述、结论：

实现了退出按钮

实验结论及心得体会
1) 理解和掌握了基于Windows Form的APP应用程序开发
2) 学会使用MIDI Toolkit完成Midi音乐文件的播放
3) 理解了C# MIDI Toolkit内部的类、event、delegate构成机制
4) 理解了基于Windows Form的event的GUI界面开发方法
5) 掌握了winform中各种控件的使用方法
6) 了解并掌握了C#中的线程保护和跨线程调用的相关知识。

