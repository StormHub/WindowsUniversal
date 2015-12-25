using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace PresentationToolkit.Core.ViewModels
{
    /// <summary>
    /// Base class for view model handles property change notifications.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Raised when property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the property change event.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression body.</param>
        protected void OnPropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (Equals(propertyExpression, null))
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var handler = PropertyChanged;

            //if is not null
            if (!Equals(handler, null))
            {
                var propertyName = GetPropertyName(propertyExpression);

                if (!Equals(propertyName, null))
                {
                    handler(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <summary>
        /// Extracts the property name from the expression.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="propertyExpression">The property expression.</param>
        /// <returns>The property name from the expression.</returns>
        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (Equals(propertyExpression, null))
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var body = propertyExpression.Body as MemberExpression;

            if (Equals(body, null))
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }

            var property = body.Member as PropertyInfo;

            if (Equals(property, null))
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }

            return property.Name;
        }
    }
}
