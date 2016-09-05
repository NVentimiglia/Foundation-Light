using System;

namespace Foundation.Architecture
{
    /// <summary>
    /// Represents a view instance
    /// </summary>
    public interface IView : IDisposable
    {
        /// <summary>
        /// Initializes the view with a viewModel to bind to
        /// </summary>
        void Init(object viewModel);
    }
}
