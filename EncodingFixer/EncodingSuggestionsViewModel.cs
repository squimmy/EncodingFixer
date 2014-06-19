using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingFixer
{
    sealed class EncodingSuggestionsViewModel : BindableBase
    {
        public ObservableCollection<EncodingSuggestion> Suggestions { get; set; }
        public EncodingSuggestion SelectedSuggestion { get; set; }
        public bool IsVisible { get { return Suggestions.Count > 0; } }

        public EncodingSuggestionsViewModel()
        {
            Suggestions = new ObservableCollection<EncodingSuggestion>();
            Suggestions.CollectionChanged += (s, e) => OnPropertyChanged(() => IsVisible);
        }
    }
}
