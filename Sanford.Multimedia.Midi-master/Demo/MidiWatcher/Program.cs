using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MidiWatcher
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        ///  inForm�������������
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();//����Ӧ�ó���Ŀ�����ʽ
            Application.SetCompatibleTextRenderingDefault(false);
            /*1.����:��Ӧ�ó���Χ�����ÿؼ���ʾ�ı���Ĭ�Ϸ�ʽ(������Ϊʹ���µ�GDI + , ���Ǿɵ�GDI)
            trueʹ��GDI + ��ʽ��ʾ�ı�, 
            falseʹ��GDI��ʽ��ʾ�ı�.
            2.ֻ���ڵ������д���ĳ����е��ø÷���; �����ڲ��ʽ�ĳ����е��ø÷���.
            3.ֻ���ڳ��򴴽��κδ���ǰ���ø÷��������������InvalidOperationException�쳣.*/
            Application.Run(new Form1());//��ʾ�ڵ�ǰ�߳��Ͽ�ʼ���б�׼Ӧ�ó�����Ϣѭ������ʹָ������ɼ�
        }
    }
}