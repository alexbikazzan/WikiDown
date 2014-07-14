using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace WikiDown.Markdown
{
    public class ConverterHook
    {
        private readonly Regex regex;

        public ConverterHook(
            string regexPattern,
            string regexReplace,
            ConverterHookType type,
            bool ignoreCase = true,
            bool multiline = true)
        {
            this.RegexPattern = regexPattern;
            this.RegexReplace = regexReplace;
            this.Type = type;
            this.IgnoreCase = ignoreCase;
            this.Multiline = multiline;

            var regexOptions = this.GetRegexOptions();

            this.regex = new Regex(this.RegexPattern, regexOptions);

            this.RegexFlags = string.Format("g{0}{1}", this.IgnoreCase ? "i" : null, this.Multiline ? "m" : null);
        }

        public bool IgnoreCase { get; private set; }

        public bool Multiline { get; private set; }

        public string RegexFlags { get; private set; }

        public string RegexPattern { get; private set; }

        public string RegexReplace { get; private set; }

        public ConverterHookType Type { get; private set; }

        public virtual string Apply(string input)
        {
            string replacedText = this.regex.Replace(input, this.RegexReplace);
            return replacedText;
        }

        private RegexOptions GetRegexOptions()
        {
            var regexOptionList = new List<RegexOptions> { RegexOptions.ECMAScript };
            if (this.IgnoreCase)
            {
                regexOptionList.Add(RegexOptions.IgnoreCase);
            }
            if (this.Multiline)
            {
                regexOptionList.Add(RegexOptions.Multiline);
            }

            return regexOptionList.Aggregate((current, option) => current | option);
        }
    }
}