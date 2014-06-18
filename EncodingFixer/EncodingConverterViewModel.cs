using Microsoft.Practices.Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EncodingFixer
{
    sealed class EncodingConverterViewModel : BindableBase
    {
        private readonly EncodingConverter converter;
        private readonly Lazy<IEnumerable<string>> allEncodings;

        public IEnumerable<string> AllEncodings { get { return allEncodings.Value; } }
        public string SourceEncoding
        {
            get { return converter.SourceEncoding.WebName; }
            set { converter.SourceEncoding = Encoding.GetEncoding(value); }
        }
        public string TargetEncoding
        {
            get { return converter.TargetEncoding.WebName; }
            set { converter.TargetEncoding = Encoding.GetEncoding(value); }
        }

        public EncodingConverterViewModel(EncodingConverter converter)
        {
            this.converter = converter;
            allEncodings = new Lazy<IEnumerable<string>>(
                () => from e in Encoding.GetEncodings()
                      orderby e.Name
                      select e.Name);

            SourceEncoding = converter.SourceEncoding.WebName;
            TargetEncoding = converter.TargetEncoding.WebName;
        }

    }
}
