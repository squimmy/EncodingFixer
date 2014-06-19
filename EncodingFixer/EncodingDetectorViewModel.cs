using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Windows.Input;

namespace EncodingFixer
{
    sealed class EncodingDetectorViewModel : BindableBase
    {
        public ICommand DetectEncoding { get; private set; }
        public string TargetText { get; set; }
        public bool IsDetectingEncoding { get; set; }
        public bool IsEnabled { get { return !IsDetectingEncoding; } }
        public int Progress { get; set; }
        public int TargetProgress { get; set; }

        public EncodingDetectorViewModel(Action detectEncoding)
        {
            DetectEncoding = new DelegateCommand(detectEncoding);
            IsDetectingEncoding = false;
        }
    }
}
