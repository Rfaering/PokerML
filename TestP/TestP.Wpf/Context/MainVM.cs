using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Media;
using TestP.Ocr;
using TestP.Ocr.Data;

namespace TestP.Wpf.Context
{
    public class MainVM : ViewModelBase
    {
        private ImageSource  _mainImage;

        public Command SetImageCommand { get; }

        public ImageSource MainImage
        {
            get { return _mainImage; }
            set 
            {
                _mainImage = value;
                OnPropertyChanged();
            }
        }



        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public float ToleranceAlgoValue => (_tolerance / 100.0f);

        private int _tolerance;
        private string _text;

        public int Tolerance
        {
            get { return _tolerance; }
            set {                 
                _tolerance = value;
                OnPropertyChanged();
                OnPropertyChanged("ToleranceAlgoValue");
            }
        }


        public MainVM()
        {
            MainImage = ToBitmapImage(Images.Test1);
            SetImageCommand = new Command(SetImage);
        }

        public void SetImage()
        {
            var extract = Extract.FindNameImage(ToleranceAlgoValue);

            Text = extract.ToString();
            MainImage = ToBitmapImage(extract.Image);
        }
    }
}
