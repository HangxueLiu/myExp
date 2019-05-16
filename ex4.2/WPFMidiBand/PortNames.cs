using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace WPFMidiBand
{
    class PortNames : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; //通知属性发生改变，需要触发
        private string[] portNames;
        private BindingList<Port> port = new BindingList<Port>(); //List不支持数据刷新，要用BindingList!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!11
        public PortNames()
        {
            this.portNames = System.IO.Ports.SerialPort.GetPortNames();
            for (int i = 0; i < portNames.Count(); i++)
            {
                this.port.Add(new Port { portName = portNames[i] });
            }

        }
        public string[] PortName
        {
            get
            {
                return portNames;
            }
            set
            {
                this.portNames = value;

            }
        }
        public BindingList<Port> Ports
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Ports"));// Age属性发生改变
                }
            }
        }
    }
}
