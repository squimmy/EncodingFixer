using System;
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

            var suggestionsVM = new EncodingSuggestionsViewModel();
            suggestionsVM.PropertyChanged += (s, e) =>
            {
                if (suggestionsVM.SelectedSuggestion != null)
                {
                    ViewModel.EncodingConverter.SourceEncoding = suggestionsVM.SelectedSuggestion.Source;
                    ViewModel.EncodingConverter.TargetEncoding = suggestionsVM.SelectedSuggestion.Target;
                }
            };


            ViewModel = new MainViewModel(chooseFiles, quit, convertFileNames, converterVM, detectorVM, suggestionsVM);
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
                resetSuggestions();
                ViewModel.EncodingDetector.IsDetectingEncoding = false;
                Mouse.OverrideCursor = null;
                return;
            }

            ViewModel.EncodingDetector.TargetProgress = allEncodings.Count();
            ViewModel.EncodingDetector.Progress = 0;

            var reporter = new ProgressReporter(
                allEncodings.Count(),
                () => Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => ++ViewModel.EncodingDetector.Progress)));

            var encodings = await Task.Run(() =>
            {
                return (from sourceEncoding in allEncodings.AsParallel()
                        from targetEncoding in allEncodings
                        let c = new EncodingConverter(sourceEncoding, targetEncoding)
                        let converted = from source in sourceTexts
                                        select c.Convert(source)
                        where reporter.DoWork(() => converted.Any(x => x.Contains(targetText)))
                        select new EncodingSuggestion(sourceEncoding.WebName, targetEncoding.WebName)).ToList();
            });

            if (encodings.Count == 0)
            {
                resetSuggestions();
            }
            else
            {
                ViewModel.EncodingSuggestions.Suggestions.Clear();
                foreach (var item in encodings)
                    ViewModel.EncodingSuggestions.Suggestions.Add(item);
                ViewModel.EncodingSuggestions.SelectedSuggestion = encodings.First();
            }

            ViewModel.EncodingDetector.IsDetectingEncoding = false;
            Mouse.OverrideCursor = null;
            return;
        }

        private void resetSuggestions()
        {
            ViewModel.EncodingConverter.SourceEncoding = Encoding.UTF8.WebName;
            ViewModel.EncodingConverter.TargetEncoding = Encoding.UTF8.WebName;
            ViewModel.EncodingSuggestions.Suggestions.Clear();
        }
    }
}
