namespace Foundation.Architecture
{
    /// <summary>
    /// Utility for generating Views
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Generates a new view
        /// </summary>
        /// <param name="viewKey"></param>
        /// <returns></returns>
        IView Get(string viewKey);
    }
}