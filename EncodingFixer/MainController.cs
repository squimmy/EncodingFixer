using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

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

            var detectorVM = new EncodingDetectorViewModel(detectEncoding);

            ViewModel = new MainViewModel(chooseFiles, quit, convertFileNames, converterVM, detectorVM);
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

        private async void detectEncoding()
        {
            ViewModel.EncodingDetector.IsDetectingEncoding = true;
            Mouse.OverrideCursor = Cursors.AppStarting;

            var sourceTexts = ViewModel.SelectedFiles;
            var targetText = ViewModel.EncodingDetector.TargetText;
            var allEncodings = from e in Encoding.GetEncodings() select e.GetEncoding();

            if (sourceTexts.Count == 0
                || string.IsNullOrEmpty(targetText)
                || sourceTexts.Any(x => x.Contains(targetText)))
            {
                ViewModel.EncodingConverter.SourceEncoding = Encoding.UTF8.WebName;
                ViewModel.EncodingConverter.TargetEncoding = Encoding.UTF8.WebName;
                ViewModel.EncodingDetector.IsDetectingEncoding = false;
                Mouse.OverrideCursor = null;
                return;
            }

            var encodings = await Task.Run(() =>
            {
                return (from sourceEncoding in allEncodings.AsParallel()
                        from targetEncoding in allEncodings
                        let c = new EncodingConverter(sourceEncoding, targetEncoding)
                        let converted = from source in sourceTexts
                                        select c.Convert(source)
                        where converted.Any(x => x.Contains(targetText))
                        select Tuple.Create(sourceEncoding, targetEncoding)).ToList();
            });

            if (encodings.Count == 0)
            {
                ViewModel.EncodingConverter.SourceEncoding = Encoding.UTF8.WebName;
                ViewModel.EncodingConverter.TargetEncoding = Encoding.UTF8.WebName;
            }
            else
            {
                ViewModel.EncodingConverter.SourceEncoding = encodings.First().Item1.WebName;
                ViewModel.EncodingConverter.TargetEncoding = encodings.First().Item2.WebName;
            }
            ViewModel.EncodingDetector.IsDetectingEncoding = false;
            Mouse.OverrideCursor = null;
            return;
        }
    }
}
