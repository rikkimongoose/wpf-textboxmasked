using System.Collections.Generic;

namespace TextBoxMasked.Filter
{
    public class TextBoxMaskedFilterProvider
    {
        private readonly Dictionary<TextBoxMaskedFilterType, BaseFilter> _predefinedFilters = new Dictionary<TextBoxMaskedFilterType, BaseFilter>(); 
        private TextBoxMaskedFilterProvider()
        {
            this._loadValues();
        }
        private void _loadValues()
        {
            _predefinedFilters.Add(TextBoxMaskedFilterType.Any, BaseFilter.NullFilter);
            _predefinedFilters.Add(TextBoxMaskedFilterType.Number, RegExFilter.NumberFilter);
            _predefinedFilters.Add(TextBoxMaskedFilterType.Decimal, RegExFilter.DecimalFilter);
            _predefinedFilters.Add(TextBoxMaskedFilterType.UNumber, RegExFilter.UNumberFilter);
            _predefinedFilters.Add(TextBoxMaskedFilterType.UDecimal, RegExFilter.UDecimalFilter);
        }
        public BaseFilter FilterForMaskedType(TextBoxMaskedFilterType textBoxMaskedFilterType)
        {
            var filter = _predefinedFilters[textBoxMaskedFilterType];
            return filter ?? BaseFilter.NullFilter;
        }

        #region Singleton

        private static TextBoxMaskedFilterProvider _instance = null;

        public static TextBoxMaskedFilterProvider Instance
        {
            get { return _instance ?? (_instance = new TextBoxMaskedFilterProvider()); }
        }
        #endregion
    }
}
