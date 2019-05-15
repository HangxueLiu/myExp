using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Sanford.Multimedia.Midi;
using Sanford.Multimedia.Midi.UI;


namespace SequencerDemo
{
    public partial class Form1 : Form
    {
        AutoSizeFormClass asc = new AutoSizeFormClass();

        bool AutoLoop = true; //默认为单曲循环

        bool InOrder = false; //顺序播放

        bool AutoStart = true;

        bool randomOrder = false; // 随机播放

        private bool scrolling = false;//卷花样式

        private bool playing = false; //弹奏

        private bool closing = false; //结束

        /*基本上，你必须按照下面的步骤使用 C# Midi工具包来播放 Midi:
          实例化 OutputDevice
          实例化 Sequence
          实例化 Sequencer
          订阅 Sequencer.ChannelMessagePlayed 事件
          在 Sequencer.ChannelMessagePlayed 事件中，调用 outDevice.Send，将消息作为参数传递给设备
          将 Sequence 附加到 Sequencer 
          实例化 OutputDevice
          调用 Sequencer.LoadAsync 将文件 NAME 作为参数传递
          听音乐*/
        private OutputDevice outDevice;

        private int outDeviceID = 0;

        private OutputDeviceDialog outDialog = new OutputDeviceDialog();//输出设备对话框

        public Form1()
        {
            InitializeComponent();//窗体对象的初始化
        }

        protected override void OnLoad(EventArgs e)//EventArgs是包含事件数据的类的基类,用于传递事件的细节。
        {
            asc.controllInitializeSize(this);
            if (OutputDevice.DeviceCount == 0)
            {
                MessageBox.Show("No MIDI output devices available.", "Error!",
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);

                Close();
            }
            else
            {
                try
                {
                    outDevice = new OutputDevice(outDeviceID);

                    sequence1.LoadProgressChanged += HandleLoadProgressChanged;
                    sequence1.LoadCompleted += HandleLoadCompleted;
                    /*如果使用异步加载，还可以处理LoadProgressChanged事件，当加载的进度发生改变时就会引发该事件。当图像被加载改变或取消加载时会发生LoadCompleted事件。*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Close();
                }
            }

            base.OnLoad(e);//加载e
            /*基类中的OnLoad函数会引发事件调用Form1_Load。如果重写了OnLoad函数但是不调用base.OnLoad(e);而是定义自己在程序加载时的操作的话那么基类中引发事件的代码就不会被执行，也就是说Form1_Load将不会被执行。
             这样看来，也就是可以理解成OnLoad事件包含了Form1_Load事件，或者说先有OnLoad事件后才会触动Form1_Load事件。如果在override了OnLoad事件中提前于Form_Load写一些预处理就会先与窗口加载代码。下面让我们更加深入一点理解事件的具体调用情况，来看看VS中程序启动的事件顺序：
             1 - Form1 Constructor
             2 - OnLoad
             3 - Form1_Load
             4 - OnActivated
             5 - Form1_Activated*/
        }
  

        protected override void OnKeyDown(KeyEventArgs e)//midi获取按下的钢琴键
        {
            /*KeyCode： 获取 KeyDown 或 KeyUp 时按下键盘的 Keys 的枚举。
              KeyValue： 实际上等于 KeyCode， KeyCode是枚举，KeyValue是枚举对应的Integer值。
              KeyData： 获取 Keys 值，该值表示按下的键的键代码，以及修饰符标志（指示同时按下的 CTRL、SHIFT 和 ALT 键的组合）。所以当同时按下Shift和Enter时:    KeyData = CType(Keys.Shift & Keys.Enter, Keys)
            */
            pianoControl1.PressPianoKey(e.KeyCode);

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)//midi获取松开的钢琴键
        {
            pianoControl1.ReleasePianoKey(e.KeyCode);

            base.OnKeyUp(e);
        }

        protected override void OnClosing(CancelEventArgs e)//即将关闭
        {
            closing = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)//关闭
        {
            sequence1.Dispose();//释放资源

            if(outDevice != null)
            {
                outDevice.Dispose();
            }

            outDialog.Dispose();

            base.OnClosed(e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)//获取打开文件事件
        {
            if(openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                Open(fileName);
            }
        }

        public void Open(string fileName) //打开一个MiDi文件
        {
            try
            {
                sequencer1.Stop();
                playing = false;
                sequence1.LoadAsync(fileName);
                this.Cursor = Cursors.WaitCursor;
                startButton.Enabled = false;
                continueButton.Enabled = false;
                stopButton.Enabled = false;
                openToolStripMenuItem.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void outputDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) //打开关于的新窗体
        {
            AboutDialog dlg = new AboutDialog(); 

            dlg.ShowDialog();
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                playing = false;
                sequencer1.Stop();
                timer1.Stop();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                playing = true;
                sequencer1.Start();
                timer1.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void continueButton_Click(object sender, EventArgs e)
        {
            try
            {
                playing = true;
                sequencer1.Continue();
                timer1.Start();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void positionHScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if(e.Type == ScrollEventType.EndScroll)
            {
                sequencer1.Position = e.NewValue;

                scrolling = false;
            }
            else
            {
                scrolling = true;
            }
        }

        private void HandleLoadProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e) // 一首歌加载完毕
        {

         
            startButton_Click(sender,  e);
 
            this.Cursor = Cursors.Arrow;//改变箭头形状
            startButton.Enabled = true;
            continueButton.Enabled = true;
            stopButton.Enabled = true;
            openToolStripMenuItem.Enabled = true;
            toolStripProgressBar1.Value = 0;

            if(e.Error == null)
            {
                positionHScrollBar.Value = 0;
                positionHScrollBar.Maximum = sequence1.GetLength();
            }
            else
            {
                MessageBox.Show(e.Error.Message);
            }
        }

        private void HandleChannelMessagePlayed(object sender, ChannelMessageEventArgs e)
        {
            if(closing)
            {
                return;
            }

            outDevice.Send(e.Message);
            pianoControl1.Send(e.Message);
        }

        private void HandleChased(object sender, ChasedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
            }
        }

        private void HandleSysExMessagePlayed(object sender, SysExMessageEventArgs e)
        {
       //     outDevice.Send(e.Message); Sometimes causes an exception to be thrown because the output device is overloaded.
        }

        private void HandleStopped(object sender, StoppedEventArgs e)
        {
            foreach(ChannelMessage message in e.Messages)
            {
                outDevice.Send(message);
                pianoControl1.Send(message);
            }
        }

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

        private void pianoControl1_PianoKeyDown(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if(playing)
            {
                return;
            }

            #endregion

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOn, 0, e.NoteID, 127));
        }

        private void pianoControl1_PianoKeyUp(object sender, PianoKeyEventArgs e)
        {
            #region Guard

            if(playing)
            {
                return;
            }

            #endregion

            outDevice.Send(new ChannelMessage(ChannelCommand.NoteOff, 0, e.NoteID, 0));
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!scrolling)
            {
                positionHScrollBar.Value = Math.Min(sequencer1.Position, positionHScrollBar.Maximum);
            }
        }


        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            asc.controlAutoSize(this);
        }


        private void quitBtn_Click(object sender, EventArgs e) //退出窗体
        {
            DialogResult result= MessageBox.Show(this, "Are you sure you want to quit the program? ", "Closing prompt ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //this.Close();
                System.Environment.Exit(0);//这是最彻底的退出方式，不管什么线程都被强制退出，把程序结束的很干净。
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) // 拖拽播放
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            listBox1.Items.Add(path);
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
}
 