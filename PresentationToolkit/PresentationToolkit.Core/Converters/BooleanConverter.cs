using System;
using Windows.UI.Xaml.Data;

namespace PresentationToolkit.Core.Converters
{
    /// <summary>
    /// Converts boolean value to corresponding target type values.
    /// </summary>
    /// <typeparam name="T">
    /// The type value type.
    /// </typeparam>
    public class BooleanConverter<T> : IValueConverter
    {
        /// <summary>
        /// Initializes a new instance of BooleanConverter.
        /// </summary>
        /// <param name="trueValue">Value to return if true.</param>
        /// <param name="falseValue">Value to return if false.</param>
        public BooleanConverter(T trueValue, T falseValue)
        {
            TrueValue = trueValue;
            FalseValue = falseValue;
        }

        /// <summary>
        /// Target value for true.
        /// </summary>
        public T TrueValue
        {
            get;
            set;
        }

        /// <summary>
        /// Target value for false.
        /// </summary>
        public T FalseValue
        {
            get;
            set;
        }

        /// <summary>
        /// Converts the boolean value to the target type.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language name.</param>
        /// <returns>
        /// The target type from boolean conversion.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool result;

            if (value is bool)
            {
                result = (bool)value;
            }
            else
            {
                bool? nullable = (bool?)value;
                result = nullable.HasValue && nullable.Value;
            }

            return result ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts the target value back to boolean.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type to convert to.</param>
        /// <param name="parameter">The converter parameter.</param>
        /// <param name="language">The language name.</param>
        /// <returns>
        /// The boolean value from the target type.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is T)
            {
                return Equals(TrueValue, value);
            }

            return false;
        }
    }
}
