using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WPFMidiBand
{
    class myColor : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged; //通知属性发生改变，需要触发
        private string mycolor;
        public string Mycolor
        {
            get
            {
                return mycolor;
            }
            set
            {
                this.mycolor = value;
                if (PropertyChanged != null)
                {

                    PropertyChanged.Invoke(this, new PropertyChangedEventArgs("Mycolor"));// Age属性发生改变
                }
            }


        }
    }
}
