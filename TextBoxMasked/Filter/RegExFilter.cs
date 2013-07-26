using System;
using System.Text.RegularExpressions;

namespace TextBoxMasked.Filter
{
    public class RegExFilter : BaseFilter
    {
        public RegExFilter(String regExp, int? maxLength)
        {
            RegExp = regExp;
            AddTextValidator(CheckRegExp);
            MaxLength = maxLength;
            AddTextValidator(CheckMaxLength);
        }
        public RegExFilter(String regExp)
            : this(regExp, (int?)null)
        {
        }
        public RegExFilter(int? maxLength)
            : this(String.Empty, maxLength)
        {
        }
        
        private System.Text.RegularExpressions.Regex _regEx = null;
        private string _regExp = String.Empty;
        public string RegExp
        {
            protected set
            {
                _regExp = value;
                _regEx = (!String.IsNullOrEmpty(value)) ? new Regex(value) : null;
            }
            get { return _regExp; }
        }

        public int? MaxLength {  get; protected set;}

        private bool CheckRegExp(String newText)
        {
            return (_regEx == null) ||
                   _regEx.Match(newText).Success;

        }

        private bool CheckMaxLength(String newText)
        {
            return !MaxLength.HasValue || MaxLength.Value >= newText.Length;
        }

        #region StaticInstances

        public static readonly RegExFilter UNumberFilter = new RegExFilter("^\\d*$");
        public static readonly RegExFilter NumberFilter = new RegExFilter("^-?\\d*$");
        public static readonly RegExFilter UDecimalFilter = new RegExFilter("^\\d*([.,]\\d*)?$");
        public static readonly RegExFilter DecimalFilter = new RegExFilter("^-?\\d*([\\.,]\\d*)?$");

        #endregion
    }
}
