// Nicholas Ventimiglia 2016-09-05

using System;
using System.Collections;
using System.Collections.Generic;

#if UNITY
using UnityEngine;

#endif

namespace Foundation.Architecture
{
    /// <summary>
    /// For controllers with an observable model. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ObservableCollection<T> : IEnumerable<T>
    {
        public enum CollectionEvent
        {
            Add,
            Remove,
            Replace
        }

        public event Action<CollectionEvent, T> OnItem = delegate { };

        public event Action<CollectionEvent, IEnumerable<T>> OnItems = delegate { };

        public event Action OnClear = delegate { };

#if UNITY
        [SerializeField]
#endif
        public List<T> InternalList = new List<T>();

        public IComparer<T> Comperer;

        public ObservableCollection()
        {
        }

        public ObservableCollection(IEnumerable<T> set)
        {
            InternalList = new List<T>(set);
        }

        public virtual void Add(T model)
        {
            InternalList.Add(model);
            if (Comperer != null)
                InternalList.Sort(Comperer);
            OnItem(CollectionEvent.Add, model);
        }

        public virtual void AddRange(IEnumerable<T> models)
        {
            InternalList.AddRange(models);
            if (Comperer != null)
                InternalList.Sort(Comperer);
            OnItems(CollectionEvent.Add, models);
        }

        public virtual void Remove(T model)
        {
            InternalList.Remove(model);
            OnItem(CollectionEvent.Remove, model);
        }

        public virtual void Replace(T model)
        {
            InternalList.Remove(model);
            InternalList.Add(model);
            if (Comperer != null)
                InternalList.Sort(Comperer);
            OnItem(CollectionEvent.Replace, model);
        }

        public virtual void AddOrReplace(T model)
        {
            if (InternalList.Contains(model))
            {
                Replace(model);
            }
            else
            {
                Add(model);
            }
        }

        public void Clear()
        {
            InternalList.Clear();
            OnClear();
        }

        public void Sort()
        {
            InternalList.Sort();
        }

        public bool Contains(T model)
        {
            return InternalList.Contains(model);
        }

        public int Count
        {
            get { return InternalList.Count; }
        }

        public T this[int index]
        {
            get { return InternalList[index]; }
            set { InternalList[index] = value; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        public IEnumerable<T> GetEnumerable()
        {
            return InternalList;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}