using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MidiWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        ///  inForm程序的启动代码
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();//启用应用程序的可视样式
            Application.SetCompatibleTextRenderingDefault(false);
            /*1.作用:在应用程序范围内设置控件显示文本的默认方式(可以设为使用新的GDI + , 还是旧的GDI)
            true使用GDI + 方式显示文本, 
            false使用GDI方式显示文本.
            2.只能在单独运行窗体的程序中调用该方法; 不能在插件式的程序中调用该方法.
            3.只能在程序创建任何窗体前调用该方法，否则会引发InvalidOperationException异常.*/
            Application.Run(new Form1());//表示在当前线程上开始运行标准应用程序消息循环，并使指定窗体可见
        }
    }
}