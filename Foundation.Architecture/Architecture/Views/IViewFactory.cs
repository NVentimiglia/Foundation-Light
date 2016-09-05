namespace Foundation.Architecture
{
    /// <summary>
    /// Utility for generating Views
    /// </summary>
    public interface IViewFactory
    {
        /// <summary>
        /// Generates a new view from a prefab or returns a static view (determined by view implementation).
        /// </summary>
        /// <param name="viewKey"></param>
        /// <returns></returns>
        IView Get(string viewKey);
    }
}
