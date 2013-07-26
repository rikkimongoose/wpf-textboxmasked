using System;
using System.Linq;

namespace TextBoxMasked.Filter
{
    public class BaseFilter
    {
        public BaseFilter()
        { }
        public BaseFilter(IsTextValidDelegate additionalValidator)
        {
            AddTextValidator(additionalValidator);
        }

        protected void AddTextValidator(IsTextValidDelegate additionalValidator)
        {
            if (IsTextValidCheckers == null || !IsTextValidCheckers.GetInvocationList().Contains(additionalValidator))
            {
                IsTextValidCheckers += additionalValidator;
            }
        }

        public delegate bool IsTextValidDelegate(String newText);

        public virtual bool IsTextValid(String newText)
        {
            if (IsTextValidCheckers == null)
                return true;
            foreach (IsTextValidDelegate isTextValidChecker in IsTextValidCheckers.GetInvocationList())
            {
                try
                {
                    if (!isTextValidChecker(newText))
                        return false;
                }
                catch
                {
                    throw;
                }
            }
            return true;
        }

        protected IsTextValidDelegate IsTextValidCheckers = null;

        #region StaticInstances

        public static readonly BaseFilter IntegerFilter = new BaseFilter(item =>
        {
            bool result = false;
            int conversionResult = 0;
            result = Int32.TryParse(item, out conversionResult);
            return result;
        });

        public static readonly BaseFilter NullFilter = new BaseFilter();

        #endregion
    }
}
