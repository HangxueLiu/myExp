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

        bool AutoLoop = true; //Ĭ��Ϊ����ѭ��

        bool InOrder = false; //˳�򲥷�

        bool AutoStart = true;

        bool randomOrder = false; // �������

        private bool scrolling = false;//����ʽ

        private bool playing = false; //����

        private bool closing = false; //����

        /*�����ϣ�����밴������Ĳ���ʹ�� C# Midi���߰������� Midi:
          ʵ���� OutputDevice
          ʵ���� Sequence
          ʵ���� Sequencer
          ���� Sequencer.ChannelMessagePlayed �¼�
          �� Sequencer.ChannelMessagePlayed �¼��У����� outDevice.Send������Ϣ��Ϊ�������ݸ��豸
          �� Sequence ���ӵ� Sequencer 
          ʵ���� OutputDevice
          ���� Sequencer.LoadAsync ���ļ� NAME ��Ϊ��������
          ������*/
        private OutputDevice outDevice;

        private int outDeviceID = 0;

        private OutputDeviceDialog outDialog = new OutputDeviceDialog();//����豸�Ի���

        public Form1()
        {
            InitializeComponent();//�������ĳ�ʼ��
        }

        protected override void OnLoad(EventArgs e)//EventArgs�ǰ����¼����ݵ���Ļ���,���ڴ����¼���ϸ�ڡ�
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
                    /*���ʹ���첽���أ������Դ���LoadProgressChanged�¼��������صĽ��ȷ����ı�ʱ�ͻ��������¼�����ͼ�񱻼��ظı��ȡ������ʱ�ᷢ��LoadCompleted�¼���*/
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error!",
                        MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    Close();
                }
            }

            base.OnLoad(e);//����e
            /*�����е�OnLoad�����������¼�����Form1_Load�������д��OnLoad�������ǲ�����base.OnLoad(e);���Ƕ����Լ��ڳ������ʱ�Ĳ����Ļ���ô�����������¼��Ĵ���Ͳ��ᱻִ�У�Ҳ����˵Form1_Load�����ᱻִ�С�
             ����������Ҳ���ǿ�������OnLoad�¼�������Form1_Load�¼�������˵����OnLoad�¼���Żᴥ��Form1_Load�¼��������override��OnLoad�¼�����ǰ��Form_LoadдһЩԤ����ͻ����봰�ڼ��ش��롣���������Ǹ�������һ������¼��ľ�����������������VS�г����������¼�˳��
             1 - Form1 Constructor
             2 - OnLoad
             3 - Form1_Load
             4 - OnActivated
             5 - Form1_Activated*/
        }
  

        protected override void OnKeyDown(KeyEventArgs e)//midi��ȡ���µĸ��ټ�
        {
            /*KeyCode�� ��ȡ KeyDown �� KeyUp ʱ���¼��̵� Keys ��ö�١�
              KeyValue�� ʵ���ϵ��� KeyCode�� KeyCode��ö�٣�KeyValue��ö�ٶ�Ӧ��Integerֵ��
              KeyData�� ��ȡ Keys ֵ����ֵ��ʾ���µļ��ļ����룬�Լ����η���־��ָʾͬʱ���µ� CTRL��SHIFT �� ALT ������ϣ������Ե�ͬʱ����Shift��Enterʱ:    KeyData = CType(Keys.Shift & Keys.Enter, Keys)
            */
            pianoControl1.PressPianoKey(e.KeyCode);

            base.OnKeyDown(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)//midi��ȡ�ɿ��ĸ��ټ�
        {
            pianoControl1.ReleasePianoKey(e.KeyCode);

            base.OnKeyUp(e);
        }

        protected override void OnClosing(CancelEventArgs e)//�����ر�
        {
            closing = true;

            base.OnClosing(e);
        }

        protected override void OnClosed(EventArgs e)//�ر�
        {
            sequence1.Dispose();//�ͷ���Դ

            if(outDevice != null)
            {
                outDevice.Dispose();
            }

            outDialog.Dispose();

            base.OnClosed(e);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)//��ȡ���ļ��¼�
        {
            if(openMidiFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = openMidiFileDialog.FileName;
                Open(fileName);
            }
        }

        public void Open(string fileName) //��һ��MiDi�ļ�
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

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e) //�򿪹��ڵ��´���
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

        private void HandleLoadCompleted(object sender, AsyncCompletedEventArgs e) // һ�׸�������
        {

         
            startButton_Click(sender,  e);
 
            this.Cursor = Cursors.Arrow;//�ı��ͷ��״
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
                Open(path); // ѡ�в���
            }
            else if(InOrder)
            {
                listBox1.SelectedIndex = (listBox1.SelectedIndex + 1) % (listBox1.Items.Count); //������һ��
            }else
            {
                int i = listBox1.SelectedIndex;
                Random rd = new Random();
                int r = rd.Next(0,listBox1.Items.Count);
                if(r==i)
                {
                    string path = listBox1.SelectedItem.ToString();
                    Open(path); // ѡ�в���
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


        private void quitBtn_Click(object sender, EventArgs e) //�˳�����
        {
            DialogResult result= MessageBox.Show(this, "Are you sure you want to quit the program? ", "Closing prompt ", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (result == DialogResult.OK)
            {
                //this.Close();
                System.Environment.Exit(0);//������׵��˳���ʽ������ʲô�̶߳���ǿ���˳����ѳ�������ĺܸɾ���
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) // ��ק����
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            listBox1.Items.Add(path);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //listBox1.SelectionMode =;
            rb1.Checked = true; // Ĭ�ϵ���ѭ��
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
                string path = listBox1.SelectedItem.ToString();
                Open(path); // ѡ�в���
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
 