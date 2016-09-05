using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// Controls the logical presentation of views
    /// </summary>
    public interface INavigationService
    {
        /// <summary>
        /// Presentation Stack 
        /// </summary>
        Stack<IView> NavigationStack { get; }

        /// <summary>
        /// Presentation Stack 
        /// </summary>
        Stack<IView> PopupStack { get; }

        //

        /// <summary>
        /// Back 
        /// </summary>
        void NavigatePop();

        /// <summary>
        /// Present a new view
        /// </summary>
        /// <param name="view"></param>
        void NavigatePush(IView view);

        /// <summary>
        /// Pops all views, clears view stack
        /// </summary>
        void NavigateClear();

        //

        /// <summary>
        /// adds a popup view
        /// </summary>
        /// <param name="view"></param>
        void PopupShow(IView view);

        /// <summary>
        /// Shows a popup
        /// </summary>
        IView PopupShow(string viewId, object model);

        /// <summary>
        /// Pops all views, clears view stack
        /// </summary>
        void PopupClear();

    }
}