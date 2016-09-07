// Nicholas Ventimiglia 2016-09-05
namespace Foundation.Architecture
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
        /// Get the pool
        /// </summary>
        Pool<TObject> GetPool<TObject>() where TObject : new();

        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        TObject Rent<TObject>() where TObject : new();

        /// <summary>
        /// Returns an object to the correct pool
        /// </summary>
        void Return<TObject>(TObject item) where TObject : new();
    }
}