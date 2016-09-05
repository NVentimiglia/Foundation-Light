using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Represents a view instance
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Should Render
        /// </summary>
        bool IsVisible { get; }
        
        /// <summary>
        /// Initializes the view with a viewModel to bind to
        /// </summary>
        void Init(object viewModel);

        /// <summary>
        /// IsVisible = true
        /// </summary>
        void Show(bool transition = true, Action transitionComplete = null);

        /// <summary>
        /// IsVisible = false
        /// </summary>
        void Hide(bool transition = true, Action transitionComplete = null);
    }
}
