using System.Text;

namespace EncodingFixer
{
    sealed class EncodingConverter
    {
        public Encoding SourceEncoding { get; set; }
        public Encoding TargetEncoding { get; set; }

        public EncodingConverter(Encoding sourceEncoding, Encoding targetEncoding)
        {
            SourceEncoding = sourceEncoding;
            TargetEncoding = targetEncoding;
        }

        public string Convert(string text)
        {
            var rawBytes = SourceEncoding.GetBytes(text);
            var converted = Encoding.Convert(TargetEncoding, Encoding.UTF8, rawBytes);
            return Encoding.UTF8.GetString(converted);
        }
    }
}
