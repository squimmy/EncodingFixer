using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncodingFixer
{
    sealed class EncodingSuggestion
    {
        public readonly string Source;
        public readonly string Target;

        public EncodingSuggestion(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public override string ToString()
        {
            return string.Format("{0} --> {1}", Source, Target);
        }
    }
}
