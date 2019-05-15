using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace WPFMidiBand 
{
    class TS : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; //通知属性发生改变，需要触发
        private string temperature;
        private string sunlight;
        public string Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                this.temperature = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Temperature"));// Age属性发生改变
                }
            }
        }
        public string Sunlight
        {
            get
            {
                return sunlight;
            }
            set
            {
                this.sunlight = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Sunlight"));// Age属性发生改变
                }
            }
        }
    }
}
