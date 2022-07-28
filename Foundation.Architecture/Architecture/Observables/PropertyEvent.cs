namespace Foundation.Architecture
{
    /// <summary>
    /// MVVM Event Arguments
    /// </summary>
    public struct PropertyEvent
    {
        /// <summary>
        /// The Model
        /// </summary>
        public object Sender;

        /// <summary>
        /// Member name (property / method)
        /// </summary>
        public string Name;

        /// <summary>
        /// New Value
        /// </summary>
        public object Value;

        /// <summary>
        /// Refresh root
        /// </summary>
        public bool IsRefreshAll
        {
            get { return string.IsNullOrEmpty(Name); }
        }
    }
}