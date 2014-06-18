using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EncodingFixer
{
    sealed class MainController
    {
        private readonly EncodingConverter converter;
        public MainViewModel ViewModel { get; private set; }

        public MainController()
        {
            converter = new EncodingConverter(
                Encoding.GetEncoding(@"shift_jis"),
                Encoding.GetEncoding(@"utf-8"));
            var converterVM = new EncodingConverterViewModel(converter);
            converterVM.PropertyChanged += (s, e) => updateConvertedFiles();

            ViewModel = new MainViewModel(chooseFiles, quit, convertFileNames, converterVM);
        }

        private void chooseFiles()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog() { Multiselect = true, };

            if (Nullable.Equals(dialog.ShowDialog(), true))
            {
                ViewModel.SelectedFiles.Clear();
                foreach (var item in dialog.FileNames)
                    ViewModel.SelectedFiles.Add(item);
                updateConvertedFiles();
            }
        }

        private void updateConvertedFiles()
        {
            ViewModel.ConvertedFiles.Clear();
            foreach (var item in ViewModel.SelectedFiles)
                ViewModel.ConvertedFiles.Add(converter.Convert(item));
        }

        private void convertFileNames()
        {
            throw new NotImplementedException();
        }

        private void quit()
        {
            Application.Current.Shutdown();
        }
    }
}
