using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;

namespace NetworkService.Model
{
    public class CircleMarker : ClassINotifyPropertyChanged
    {
        private int cmValue;
        private string cmDate;
        private string cmTime;
        private Brush cmColor;
        private int cmWidthAndHeight;

        public CircleMarker()
        {
            cmValue = 1;
            
        }

        public CircleMarker(int cmValue, string cmDate, string cmTime)
        {
            CmValue = cmValue;
            CmDate = cmDate;
            CmTime = cmTime;
        }


        public int CmValue
        {
            get { return cmValue; }
            set
            {
                cmValue = value;
                CmWidthAndHeight = (int)Math.Round(cmValue / 5.625);
                if (cmValue >= 250 & cmValue <= 350)
                {
                    CmColor = Brushes.Green;
                }
                else if ((cmValue > 0 && cmValue<250) || cmValue>350)
                {
                    CmColor = Brushes.Red;
                }
                else
                {                  
                    CmColor = Brushes.CadetBlue;   
                }
                OnPropertyChanged("CmValue");
            }
        }

        public string CmDate
        {
            get { return cmDate; }
            set
            {
                cmDate = value;
                OnPropertyChanged("CmDate");
            }
        }
        public int CmWidthAndHeight
        {
            get { return cmWidthAndHeight; }
            set
            {
                cmWidthAndHeight = value;
                OnPropertyChanged("CmWidthAndHeight");
            }
        }

        public string CmTime
        {
            get { return cmTime; }
            set
            {
                cmTime = value;
                OnPropertyChanged("CmTime");
            }
        }
        public Brush CmColor
        {
            get { return cmColor; }
            set
            {
                cmColor = value;
                OnPropertyChanged("CmColor");
            }
        }
    }
}
