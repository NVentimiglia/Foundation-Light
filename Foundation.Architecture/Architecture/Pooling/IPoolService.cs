namespace Foundation.Architecture.Misc
{
    /// <summary>
    /// Pool Manager for CLR objects
    /// </summary>
    /// <remarks>
    /// Thread Safe
    /// </remarks>
    public interface IPoolService
    {
        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        T Rent<T>() where T : new();

        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        /// <param name="item"></param>
        void Return<T>(T item) where T : new();
    }
}