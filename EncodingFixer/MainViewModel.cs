using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace EncodingFixer
{
    sealed class MainViewModel : BindableBase
    {
        public ObservableCollection<string> SelectedFiles { get; private set; }
        public ObservableCollection<string> ConvertedFiles { get; private set; }

        public ICommand ChooseFiles { get; private set; }
        public ICommand ConvertFileNames { get; private set; }
        public ICommand SelectEncodings { get; private set; }
        public ICommand Quit { get; private set; }
        public EncodingConverterViewModel EncodingConverter { get; private set; }
        public EncodingDetectorViewModel EncodingDetector { get; private set; }
        public EncodingSuggestionsViewModel EncodingSuggestions { get; private set; }

        public MainViewModel(Action chooseFiles,
                             Action quit,
                             Action convertFileNames,
                             EncodingConverterViewModel converter,
                             EncodingDetectorViewModel detector,
                             EncodingSuggestionsViewModel suggestions)
        {
            ChooseFiles = new DelegateCommand(chooseFiles);
            Quit = new DelegateCommand(quit);
            ConvertFileNames = new DelegateCommand(convertFileNames);
            EncodingConverter = converter;
            EncodingDetector = detector;
            EncodingSuggestions = suggestions;

            SelectedFiles = new ObservableCollection<string>();
            ConvertedFiles = new ObservableCollection<string>();
        }
    }
}
