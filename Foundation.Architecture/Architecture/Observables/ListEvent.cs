using System.Collections.Generic;

namespace Foundation.Architecture
{
    /// <summary>
    /// List event type
    /// </summary>
    public enum ListChangedEventType
    {
        /// <summary>
        /// List.Add(index, value);
        /// </summary>
        Add,
        /// <summary>
        /// List.Remove(index, value);
        /// </summary>
        Remove,
        /// <summary>
        /// List[i].RaiseChange();
        /// </summary>
        Refresh,
        /// <summary>
        /// List.Add(index, value)
        /// </summary>
        Insert,
        /// <summary>
        /// List[i] = value;
        /// </summary>
        Replace,
        /// <summary>
        /// List.Clear();
        /// </summary>
        Clear
    }

    /// <summary>
    /// List Changed Args
    /// </summary>
    public struct ListEvent
    {
        public ListChangedEventType Event;

        public IEnumerable<object> Items;

        /// <summary>
        /// Used for replace and Insert
        /// </summary>
        public int Index;
    }
}