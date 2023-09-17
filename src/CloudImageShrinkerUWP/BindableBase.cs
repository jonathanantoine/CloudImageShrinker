using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace CloudImageShrinkerUWP
{
    /// <summary>
    /// Base class for implementing INotifyPropertyChanged
    /// </summary>
    public class BindableBase : INotifyPropertyChanged
    {
        /// <summary>
        /// potential perf improvement by Simon, inspired by work on Motivation Factory to reduce memory allocations when many PropertyChanged events
        /// are raised in a short amount of time.
        /// PropertyChanged calls are always done on UI thread, so no inter-thread sync is need for this pcea cache
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        private static readonly Dictionary<string, PropertyChangedEventArgs> _PceaCache = new Dictionary<string, PropertyChangedEventArgs>();

        /// <summary>
        /// The property changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise the PropertyChanged event
        /// </summary>
        /// <param name="propertyName">the name of the targeted property</param>
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler == null)
            {
                return;
            }

            PropertyChangedEventArgs args;
            if (!_PceaCache.TryGetValue(propertyName, out args))
            {
                args = new PropertyChangedEventArgs(propertyName);
                _PceaCache.Add(propertyName, args);
            }

            handler(this, args);
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="expression">
        /// The expression.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            if (expression.Body is MemberExpression body)
            {
                RaisePropertyChanged(body.Member.Name);
            }
        }

        /// <summary>
        /// Checks if a property already matches a desired value.  Sets the property and
        /// notifies listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">Name of the property used to notify listeners.  This
        /// value is optional and can be provided automatically when invoked from compilers that
        /// support CallerMemberName.</param>
        /// <returns>True if the value was changed, false if the existing value matched the
        /// desired value.</returns>
        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// The _is loading.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        /// Is this current object loading something ? Default value is false.
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                SetProperty(ref _isLoading, value);
                OnIsLoadingChanged();
            }
        }

        private string _loadingText;

        /// <summary>
        /// Text which can be used to describes the loading process.
        /// </summary>
        public string LoadingText
        {
            get { return _loadingText; }
            set { SetProperty(ref _loadingText, value); }
        }

        /// <summary>
        /// The on is loading changed.
        /// </summary>
        protected virtual void OnIsLoadingChanged()
        {
        }
    }
}