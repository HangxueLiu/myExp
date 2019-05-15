using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Sanford.Multimedia.Midi.UI;
using Sanford.Multimedia.Midi;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;

using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;
using System.Windows.Controls;
using System.Drawing;
using ZedGraph;
// mciSendString
namespace WPFMidiBand
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MidiData Rxdata = new MidiData(); // 返回数据'

        private PortNames portNames = new PortNames();

        private myColor mycolor = new myColor();

        bool color = false; //滑块颜色显示
        bool isConnected = false;//是否已经连接

        //string[] PortNames = System.IO.Ports.SerialPort.GetPortNames();    //获取本机串口名称，存入PortNames数组中, 要插上才有！！！！
        System.IO.Ports.SerialPort m_sp = new System.IO.Ports.SerialPort(); //声明串口
        private string lastFileName = "";// 上一个发送的文件
        string fileName = "";//当前文件

        TS ts=new TS();//返回的光照和温度
        ZedGraph.GraphPane myPane ;
        LineItem myCurveTem;
        LineItem myCurveSun;
        PointPairList listTem = new PointPairList();
        PointPairList listSun = new PointPairList();
        DateTime beginTime;

        public MainWindow()
        {
            InitializeComponent();
            /*System.Windows.Data.Binding myBinding = new System.Windows.Data.Binding("FrameMsg");
            myBinding.Source = Rxdata;
            backData.SetBinding(System.Windows.Controls.TextBox.TextProperty, myBinding);*/
            System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
            int[] a = { 9600, 19200, 38400, 57600 };
            cbxSerilBaudRate.ItemsSource = a;// Object型

            byte[] b = { 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };//设置引脚与读取引脚
            cmb3.ItemsSource = b;
            cmb4.ItemsSource = b;

            // sentData.DataContext = midiData;
            cbxSerilPort.ItemsSource = portNames.Ports;//绑定端口号

            //数据绑定温度
            System.Windows.Data.Binding bind = new System.Windows.Data.Binding();
            bind.Source = ts;
            bind.Path = new PropertyPath("Temperature");
            this.temptxt.SetBinding(System.Windows.Controls.TextBox.TextProperty, bind);
            //数据绑定光强
            System.Windows.Data.Binding bind2 = new System.Windows.Data.Binding();
            bind2.Source = ts;
            bind2.Path = new PropertyPath("Sunlight");
            this.suntxt.SetBinding(System.Windows.Controls.TextBox.TextProperty, bind2);

            System.Windows.Data.Binding bind3 = new System.Windows.Data.Binding();
            bind3.Source = mycolor;
            myPane = zedGraphControl1.GraphPane;
            myPane.XAxis.Title.Text = "时间";
            myPane.YAxis.Title.Text = "光照/温度";
            myCurveTem = myPane.AddCurve("温度", listTem, System.Drawing.Color.Red, SymbolType.Diamond);
            myCurveSun = myPane.AddCurve("光强", listSun, System.Drawing.Color.Blue, SymbolType.Diamond);
            
        }


        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (!isConnected)
            {
                System.Windows.MessageBox.Show("请先连接串口");
                rb1.IsChecked = false;
            }
            else
            {
                color = true;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void btnConnected_Click(object sender, RoutedEventArgs e) // 连接串口
        {
            if (!isConnected)
            {
                isConnected = true;
                Port myPort = cbxSerilPort.SelectedItem as Port;
                //强制类型转换，直接把原来类型按照指定类型，强制转移，如果类型不匹配会出现异常as 类型不匹配时，不会出现异常，接受的变量会是null
                if (myPort != null)
                {
                    m_sp.PortName = myPort.portName;  //串口
                    m_sp.BaudRate = (int)cbxSerilBaudRate.SelectedItem; // 波特率
                    m_sp.Parity = System.IO.Ports.Parity.None;    //校验法：无
                    m_sp.DataBits = 8;// 数据位
                    m_sp.StopBits = System.IO.Ports.StopBits.One;     //停止位：1

                    //m_sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);//委托
                    try
                    {
                        m_sp.Open();        //打开串口
                        beginTime = DateTime.Now;
                        Byte[] BSendTemp = new byte[3];
                        BSendTemp[0] = 0xF9;
                        BSendTemp[1] = 0;
                        BSendTemp[2] = 0;
                        m_sp.Write(BSendTemp, 0, 3);
                        for (int i = 0; i < 3; i++)//获取学号
                        {
                            int indata = m_sp.ReadByte();
                            //System.Windows.MessageBox.Show(indata.ToString());
                            //Console.Write("\n{0:X2}\n ", indata);
                            if ((indata & 0x80) != 0)
                            {
                                //Console.Write("\n New Data Frame:");
                                Rxdata.DataIdx = 0;
                                Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                                Rxdata.DataIdx++;
                            }
                            else if (Rxdata.DataIdx < Rxdata.SerialDatas.Length)
                            {
                                //Console.Write("{0:X2} ", indata);
                                Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                                Rxdata.DataIdx++;
                            }

                            if (Rxdata.DataIdx >= 3)
                            {
                                string msg = "学号为："+Rxdata.SerialDatas[1].ToString() + Rxdata.SerialDatas[2].ToString();
                                SetTextInTextBox(backData, msg);
                            }
                        }
                    }
                    catch (System.Exception ex)
                    {//打开串口出错，显示错误信息
                        System.Windows.MessageBox.Show(ex.Message);
                    }
                }
                else
                {
                    System.Windows.MessageBox.Show("请选择串口");
                }
            }
            else
            {
                System.Windows.MessageBox.Show("已经连接请先断开");
            }
        }

        private void btnDisconnected_Click(object sender, RoutedEventArgs e)//关闭串口
        {
            if (m_sp.IsOpen)
            {
                isConnected = false;
                m_sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);
                m_sp.Close();
            }
            else
            {
                System.Windows.MessageBox.Show("您未打开任何串口");
            }
        }

        private void cbxSerilPort_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)//自动更新可选择的串口号
        {
            portNames.PortName = System.IO.Ports.SerialPort.GetPortNames();
            if (portNames.PortName.Count() == 0)
            {
                portNames.Ports.Clear();
            }
            else
            {
                portNames.Ports.Clear();
                for (int i = 0; i < portNames.PortName.Count(); i++)
                {
                    portNames.Ports.Add(new Port { portName = portNames.PortName[i] });

                }
            }
        }
        static void Thread1(byte[] data,int start,int end)
        {

        }
        private void btnSent_Click(object sender, RoutedEventArgs e)// 打开需要播放的MIDI文件
        {
            //this.txb1.Visibile= System.Windows.Visibility.Visible;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            //openFileDialog.InitialDirectory = "C:\\";    //打开对话框后的初始目录
            openFileDialog.Filter = "文本文件|*.mid|所有文件|*.*";
            //openFileDialog.RestoreDirectory = false;    //若为false，则打开对话框后为上次的目录。若为true，则为初始目录      
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                fileName = openFileDialog.FileName;
                lastFileName = fileName;
                FilePath.Text = fileName;
                byte[] data = File.ReadAllBytes(fileName);
                /*FileStream fs = new FileStream(fileName,FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                byte[] file = new byte[fs.Length];
                fs.Read(file, 0, file.Length);*/
                //System.Windows.MessageBox.Show("开始");
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = 0xF8;
                BSendTemp[1] = 0;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp,0,3);
                //Thread thread1 = new Thread(new ParameterizedThreadStart(Thread1));
                m_sp.Write(data,0,data.Length);
                for (int i = 0; i < 1000; i++)
                {
                    string msg= string.Join(",", Convert.ToString(data[i], 16))+ ", ";
                    SetTextInTextBox(sentData,msg);
                    //sentData.Text += ", ";
                    //System.Windows.MessageBox.Show(Convert.ToString(data[i], 16));
                }
                PlayRepeat(fileName);
            }
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            Byte[] BSendTemp = new byte[3];
            BSendTemp[0] = 0xF7;
            BSendTemp[1] = 0;
            BSendTemp[2] = 0;
            m_sp.Write(BSendTemp, 0, 3);
            mciSendString("close media", null, 0, IntPtr.Zero);
        }

        private void sliderColor(byte cmd, Slider slider)
        {
            try
            {
                if (color == true)
                {
                    Byte[] BSendTemp = new byte[3];
                    BSendTemp[0] = cmd;
                    int a = (int)slider.Value * 25;
                    if ((a & 0x80) != 0)
                    {
                        BSendTemp[1] = (byte)(a & 0x7f);
                        BSendTemp[2] = 0x01;
                    }
                    else
                    {
                        BSendTemp[1] = (byte)a;
                        BSendTemp[2] = 0;
                    }
                    sentData.Text += string.Join(",", Convert.ToString(BSendTemp[0], 16));
                    sentData.Text += " ";
                    sentData.Text += string.Join(",", Convert.ToString(BSendTemp[1], 16));
                    sentData.Text += " ";
                    sentData.Text += string.Join(",", Convert.ToString(BSendTemp[2], 16));
                    sentData.Text += "\n";
                    m_sp.Write(BSendTemp, 0, 3);
                    int color1 = (int)RSlider.Value*25;
                    int color2 = (int)GSlieder.Value*25;
                    int color3 = (int)BSlider.Value*25;
                    //System.Windows.MessageBox.Show(color1.ToString());
                    string txt1;
                    string txt2;
                    string txt3;
                    if (color1 <= 5)
                    {
                        txt1 = Convert.ToString(color1, 16);
                        txt1 = "0" + txt1;
                    }
                    else
                    {
                        txt1 = Convert.ToString(color1, 16);
                    }

                    if (color2 <= 5)
                    {
                        txt2 = Convert.ToString(color2, 16);
                        txt2 = "0" + txt2;
                    }
                    else
                    {
                        txt2 = Convert.ToString(color2, 16);
                    }

                    if (color3 <= 5)
                    {
                         txt3 = Convert.ToString(color3, 16);
                        txt3 = "0" + txt3;
                    }
                    else
                    {
                         txt3 = Convert.ToString(color3, 16);
                    }
                    string myColor = "#" + txt1 + txt2 + txt3;
                    //Eli.Fill = System.Windows.Media.Brushes.Red;
                    //System.Windows.MessageBox.Show(myColor);
                    Eli.Fill = (SolidColorBrush)new BrushConverter().ConvertFromString(myColor);
                    mycolor.Mycolor = myColor;
                    //System.Windows.MessageBox.Show(mycolor.Mycolor);
                }
            }
            catch
            {
                System.Windows.MessageBox.Show("Error");
            }
        }


        private void RSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderColor(0xD9, RSlider);
        }

        private void GSlieder_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderColor(0xD3, GSlieder);
        }

        private void YSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderColor(0xD5, YSlider);
        }

        private void BSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderColor(0xD6, BSlider);
        }

        private void WSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderColor(0xDA, WSlider);
        }

        #region 播放MIDI文件
        [DllImport("winmm.dll")]
        private static extern long mciSendString(
            string command,      //MCI命令字符串
            string returnString, //存放反馈信息的缓冲区
            int returnSize,      //缓冲区的长度
            IntPtr hwndCallback  //回调窗口的句柄，一般为NULL
            );//若成功则返回0，否则返回错误码。
        private void PlayWait(string file)
        {
            /*
             * open device_name type device_type alias device_alias  打开设备
             * device_name　 　　　要使用的设备名，通常是文件名。
             * type device_type　　设备类型，例如mpegvideo或waveaudio，可省略。
             * alias device_alias　设备别名，指定后可在其他命令中代替设备名。
             */
            mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", file), null, 0, IntPtr.Zero);

            /*
             * play device_alias from pos1 to pos2 wait repeat  开始设备播放
             * 若省略from则从当前磁道开始播放。
             * 若省略to则播放到结束。
             * 若指明wait则等到播放完毕命令才返回。即指明wait会产生线程阻塞，直到播放完毕
             * 若指明repeat则会不停的重复播放。
             * 若同时指明wait和repeat则命令不会返回，本线程产生堵塞，通常会引起程序失去响应。
             */
            mciSendString("play media wait", null, 0, IntPtr.Zero);
            /*
             * close　　　 关闭设备
             */
            mciSendString("close media", null, 0, IntPtr.Zero);
        }
        //循环播放
        private void PlayRepeat(string file)
        {
            mciSendString(string.Format("open \"{0}\" type mpegvideo alias media", file), null, 0, IntPtr.Zero);
            //mciSendString("status cd current track", midiData.Date, 1, IntPtr.Zero);

            mciSendString("play media repeat", null, 0, IntPtr.Zero);
        }

        private Thread thread;
        /// <summary>
        /// 播放音频文件
        /// </summary>
        /// <param name="file">音频文件路径</param>
        /// <param name="times">播放次数，0：循环播放 大于0：按指定次数播放</param>
        public void Play(string file, int times)
        {
            //用线程主要是为了解决在播放的时候指定wait时产生线程阻塞,从而导致界面假死的现象
            thread = new Thread(() =>
            {
                if (times == 0)
                {
                    PlayRepeat(file);
                }
                else if (times > 0)
                {
                    for (int i = 0; i < times; i++)
                    {
                        PlayWait(file);
                    }
                }
            });

            //线程必须为单线程
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }
        /// <summary>
        /// 结束播放的线程
        /// </summary>
        public void Exit()
        {
            if (thread != null)
            {
                try
                {
                    thread.Abort();
                }
                catch { }
                thread = null;
            }
        }

        #endregion 播放MIDI文件

        private void readFoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte foot = (byte)cmb3.SelectedItem;
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = (byte)(0xC0 + foot);
                BSendTemp[1] = 0x66;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp, 0, 3);
                System.Windows.MessageBox.Show(m_sp.ReadByte().ToString());
            }
            catch
            {
                System.Windows.MessageBox.Show("请选择引脚号或连接串口");
            }

        }

        private void cmb4_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void writeFoot_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte foot = (byte)cmb3.SelectedItem;
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = (byte)(0x90 + foot);
                // System.Windows.MessageBox.Show(BSendTemp[0].ToString());
                BSendTemp[1] = 0;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp, 0, 3);
                System.Windows.MessageBox.Show("写入成功");
            }
            catch
            {
                System.Windows.MessageBox.Show("请选择引脚号或连接串口");
            }
        }

        private void writeFoot1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte foot = (byte)cmb3.SelectedItem;
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = (byte)(0x90 + foot);
                BSendTemp[1] = 0x01;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp, 0, 3);
                System.Windows.MessageBox.Show("写入成功");
            }
            catch
            {
                System.Windows.MessageBox.Show("请选择引脚号或连接串口");
            }
        }


      /*  private void DataReceivedHandler() //温度
         {
             if (m_sp == null)
             {
                 return;
             }
             int numberOfByte = m_sp.BytesToRead;
             //System.Windows.MessageBox.Show(numberOfByte.ToString());
             for (int i = 0; i < numberOfByte; i++)
             {
                 int indata = m_sp.ReadByte();
                 //System.Windows.MessageBox.Show(indata.ToString());
                 if ((indata & 0x80) != 0)
                 {
                     Rxdata.DataIdx = 0;//0,代表命令字节
                     Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                     Rxdata.DataIdx++;
                 }
                 else if (Rxdata.DataIdx < 3)
                 {
                     Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                     Rxdata.DataIdx++;
                 }
                 if (Rxdata.DataIdx >= 3)
                 {
                     string msg = string.Format("\n温度=0x{0:X4}={0:d}", 
                        Rxdata.RealData);
                         SetTextInTextBox(backData, msg);
                     //Rxdata.FrameMsg = Rxdata.FrameMsg.Add(string.Format("温度=0x{0:X4}={0:d}", Rxdata.RealData));
                     double hq = 5.0 * (Rxdata.RealData) / 1024; //读取传感器模拟值
                     double x = 51 * hq / (5.0 - hq); //计算当前电阻值
                     double hs = Math.Log(x / 50); //计算NTC对应阻值的对数值
                                                   //Series.println(long(&x),HEX);
                     double temp = 1 / (-hs / 4150 + 1 / 298.15) - 273.15;//计算当前的温度值
                     temptxt.Text = temp.ToString();
                 }
             }
         }*/
       /*  private void DataReceivedHandler2() //光照
         {
             if (m_sp == null)
             {
                 return;
             }
             int numberOfByte = m_sp.BytesToRead;
             //System.Windows.MessageBox.Show(numberOfByte.ToString());
             for (int i = 0; i < numberOfByte; i++)
             {
                 int indata = m_sp.ReadByte();
                 //System.Windows.MessageBox.Show(indata.ToString());
                 if ((indata & 0x80) != 0)
                 {
                     Rxdata.DataIdx = 0;//0,代表命令字节
                     Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                     Rxdata.DataIdx++;
                 }
                 else if (Rxdata.DataIdx < 3)
                 {
                     Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                     Rxdata.DataIdx++;
                 }
                 if (Rxdata.DataIdx >= 3)
                 {
                     string msg = string.Format("\n光强=0x{0:X4}={0:d}",
                        Rxdata.RealData);
                     SetTextInTextBox(backData, msg);
                     //Rxdata.FrameMsg = Rxdata.FrameMsg.Add(string.Format("温度=0x{0:X4}={0:d}", Rxdata.RealData));
                     suntxt.Text = Rxdata.RealData.ToString();


                 }
             }
         }*/
        private delegate void SetTextCallback(System.Windows.Controls.TextBox control, string text);

        public void SetTextInTextBox(System.Windows.Controls.TextBox control, string msg)//跨线程调用
        {

            if (backData.Dispatcher.CheckAccess())
            {
                backData.AppendText(msg);
                backData.ScrollToEnd();
            }
            else
            {
                SetTextCallback d = new SetTextCallback(SetTextInTextBox);
                Dispatcher.Invoke(d, new object[] { control, msg });
            }
        }
       /* private void rf_Click(object sender, RoutedEventArgs e)//刷新温度
        {
            try
            {
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = (byte)(0xE0);//温度
                BSendTemp[1] = 0;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp, 0, 3);
                DataReceivedHandler();
            }
            catch
            {
                System.Windows.MessageBox.Show("请连接串口");
            }
        }*/

       /* private void rf2_Click(object sender, RoutedEventArgs e)//刷新光照
        {
            try
            {
                Byte[] BSendTemp = new byte[3];
                BSendTemp[0] = (byte)(0xE1);//温度
                BSendTemp[1] = 0;
                BSendTemp[2] = 0;
                m_sp.Write(BSendTemp, 0, 3);
                DataReceivedHandler2();
            }
            catch
            {
                System.Windows.MessageBox.Show("请连接串口");
            }
        }*/

        private void sent_Click(object sender, RoutedEventArgs e)
        {
            byte[] SendBytes = null;
            string SendData = sentData.Text;
            //System.Windows.MessageBox.Show(SendData);
            //剔除所有空格
            SendData = SendData.Replace(" ", "");
            //每两个字符放进认为一个字节
            List<string> SendDataList = new List<string>();
            for (int i = 0; i < SendData.Length; i = i + 2)
            {
                SendDataList.Add(SendData.Substring(i, 2));
            }
            SendBytes = new byte[SendDataList.Count];
            for (int j = 0; j < SendBytes.Length; j++)
            {
                SendBytes[j] = (byte)(Convert.ToInt32(SendDataList[j], 16));
                //System.Windows.MessageBox.Show(SendBytes[j].ToString());
            }
            m_sp.Write(SendBytes, 0, SendBytes.Length);

        }


      private void DataReceivedHandler(
                  object sender,
                  SerialDataReceivedEventArgs e)
        {
            //SerialPort sp = (SerialPort)sender;
            if (m_sp == null)
            {
                return;
            }
            int numOfByte = m_sp.BytesToRead;
            for (int i = 0; i < numOfByte; i++)
            {

                int indata = m_sp.ReadByte();

                //Console.Write("\n{0:X2}\n ", indata);
                if ((indata & 0x80) != 0)
                {

                    //Console.Write("\n New Data Frame:");
                    Rxdata.DataIdx = 0;
                    Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                    Rxdata.DataIdx++;
                }
                else if (Rxdata.DataIdx < Rxdata.SerialDatas.Length)
                {
                    //Console.Write("{0:X2} ", indata);
                    Rxdata.SerialDatas[Rxdata.DataIdx] = (byte)indata;
                    Rxdata.DataIdx++;
                }

                if (Rxdata.DataIdx >= 3)
                {
                    if ((Rxdata.SerialDatas[0] & 0x0f) == 0)
                    {
                        string msg = string.Format("\n温度=0x{0:X4}={0:d}",
                       Rxdata.RealData);
                        SetTextInTextBox(backData, msg);
                        //Rxdata.FrameMsg = Rxdata.FrameMsg.Add(string.Format("温度=0x{0:X4}={0:d}", Rxdata.RealData));
                        double hq = 5.0 * (Rxdata.RealData) / 1024; //读取传感器模拟值
                        double x = 51 * hq / (5.0 - hq); //计算当前电阻值
                        double hs = Math.Log(x / 50); //计算NTC对应阻值的对数值
                                                      //Series.println(long(&x),HEX);
                        double temp = 1 / (-hs / 4150 + 1 / 298.15) - 273.15;//计算当前的温度值
                        ts.Temperature = temp.ToString();
                        //temptxt.Text = temp.ToString();
                        string msg2 = "\n温度：" + temp.ToString();
                        SetTextInTextBox(backData,msg2);
                        DateTime endTime = DateTime.Now;
                        TimeSpan oTime = endTime.Subtract(beginTime); //求时间差的函数 
                        listTem.Add(oTime.TotalSeconds, Rxdata.RealData);
                        // System.Windows.MessageBox.Show(listTem.Count.ToString());
                       
                        zedGraphControl1.AxisChange();//刷新界面
                        zedGraphControl1.Refresh();//重新刷新
                    }
                    else 
                    if((Rxdata.SerialDatas[0] & 0x0f) == 1)
                    {
                        string msg = string.Format("\n光强=0x{0:X4}={0:d}",
                      Rxdata.RealData);
                        SetTextInTextBox(backData, msg);
                        //Rxdata.FrameMsg = Rxdata.FrameMsg.Add(string.Format("温度=0x{0:X4}={0:d}", Rxdata.RealData));
                        //suntxt.Text = Rxdata.RealData.ToString();
                        ts.Sunlight= Rxdata.RealData.ToString();
                        DateTime endTime = DateTime.Now;
                        TimeSpan oTime = endTime.Subtract(beginTime); //求时间差的函数 
                        listSun.Add(oTime.TotalSeconds, Rxdata.RealData);
                        // System.Windows.MessageBox.Show(listTem.Count.ToString());

                        zedGraphControl1.AxisChange();//刷新界面
                        zedGraphControl1.Refresh();//重新刷新

                    }

                }
            }

        }

      

        private void rf_Click(object sender, RoutedEventArgs e)
        {
            Byte[] BSendTemp = new byte[3];
            BSendTemp[0] =0xE0;
            BSendTemp[1] = 0;
            BSendTemp[2] = 0;
            m_sp.Write(BSendTemp, 0, 3);
            m_sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);//委托
        }

        private void rf2_Click(object sender, RoutedEventArgs e)
        {
            Byte[] BSendTemp = new byte[3];
            BSendTemp[0] = 0xA0;


            BSendTemp[1] = 0;
            BSendTemp[2] = 0;
            m_sp.Write(BSendTemp, 0, 3);
            m_sp.DataReceived -= new SerialDataReceivedEventHandler(DataReceivedHandler);//委托
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog file = new SaveFileDialog();//定义新的文件保存位置控件 
            file.Filter = "txt文件(*.txt)|*.txt";//设置文件后缀的过滤    
            file.FileName = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            file.FileName = file.FileName.Replace(":", "-");
            //System.Windows.MessageBox.Show(file.FileName);
            if (file.ShowDialog() == System.Windows.Forms.DialogResult.OK)//如果有文件保存路径       
            {
                StreamWriter sw = File.CreateText(file.FileName);
                sw.Write("串口号" + cbxSerilPort.SelectedItem.ToString() + "\n");  //写入文件中    
                sw.Write("波特率" + cbxSerilBaudRate.SelectedItem.ToString() + "\n");
                sw.Write("AD转换值和温度\n" + backData.Text + "\n");
                sw.Flush();//清理缓冲区               
                sw.Close();//关闭文件          
            }
        }
    }
}

