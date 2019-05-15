using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPFMidiBand
{
    public class MidiData : INotifyPropertyChanged
    {
        private byte[] serialdatas;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        public byte[] SerialDatas
        {
            get
            {
                return serialdatas;
            }
            set
            {
                serialdatas = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("SerialDatas"));// Age属性发生改变
                }
            }
        }


        public int DataIdx { get; set; }

        public BindingList<string> framemsg = new BindingList<string>();
        public BindingList<string> FrameMsg
        {
            get
            {
                return framemsg;
            }
            set
            {
                if (framemsg != value)
                {
                    framemsg = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged.Invoke(this, new PropertyChangedEventArgs("FrameMsg"));// Age属性发生改变
                    }

                }
            }
        }

        public int RealData
        {
            get
            {
                return ((int)serialdatas[2] << 7) + serialdatas[1];
            }

        }
        public MidiData()
        {
            serialdatas = new byte[3];
            serialdatas[0] = 0;
            serialdatas[1] = 0;
            serialdatas[2] = 0;
            DataIdx = 0;
        }
    }
}
