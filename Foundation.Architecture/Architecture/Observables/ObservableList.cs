// Nicholas Ventimiglia 2016-09-05

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY
using UnityEngine;
#endif

namespace Foundation.Architecture
{
    /// <summary>
    /// For controllers with an observable model. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    // WEBGL [Serializable]
    public class ObservableList<T> : IEnumerable<T>, IObservable<ListEvent> where T : class
    {
        #region IObservable
        public event Action<ListEvent> OnPublish = delegate { };

        public void Subscribe(Action<ListEvent> handler)
        {
            OnPublish += handler;
            handler(new ListEvent
            {
                Event = ListChangedEventType.Add,
#if UNITY
                Items = InternalList.ToArray(),
#else
                Items = InternalList,
#endif
            });
        }

        public void Unsubscribe(Action<ListEvent> handler)
        {
            OnPublish -= handler;
        }

        public void Publish(ListEvent model)
        {
            OnPublish(model);
        }

        public void Publish(ListChangedEventType e, object item)
        {
            OnPublish(new ListEvent
            {
                Event = e,
                Items = new []{ item },
            });
        }

        public void Publish(ListChangedEventType e, IEnumerable<T> items)
        {
            OnPublish(new ListEvent
            {
                Event = e,
                Items = (object[])items,
            });
        }


        public void Dispose()
        {
            if (CanRefresh)
            {
                for (int i = 0; i < InternalList.Count(); i++)
                {
                    UnbindRefresh(InternalList.ElementAt(i) as IObservable<PropertyEvent>);
                }
            }

            InternalList.Clear();
            OnPublish = delegate { };
        }
        #endregion

        #region  IEnumerable

#if UNITY
        [SerializeField]
#endif
        public List<T> InternalList = new List<T>();

        public IComparer<T> Comperer;

        public ObservableList()
        {
        }

        public ObservableList(IEnumerable<T> set)
        {
            InternalList = new List<T>(set);
        }

        public virtual void Add(T model)
        {
            if (CanRefresh)
            {
                BindRefresh(model as IObservable<PropertyEvent>);
            }

            InternalList.Add(model);
            if (Comperer != null)
                InternalList.Sort(Comperer);
            Publish(ListChangedEventType.Add, model);
        }

        public virtual void AddRange(IEnumerable<T> models)
        {
            if (CanRefresh)
            {
                for (int i = 0; i < models.Count(); i++)
                {
                    BindRefresh(models.ElementAt(i) as IObservable<PropertyEvent>);
                }
            }

            InternalList.AddRange(models);
            if (Comperer != null)
                InternalList.Sort(Comperer);
            Publish(ListChangedEventType.Add, models);
        }

        public virtual void Insert(int index, T model)
        {
            if (CanRefresh)
            {
                UnbindRefresh(model as IObservable<PropertyEvent>);
            }

            InternalList.Insert(index, model);

            Publish(new ListEvent
            {
                Event = ListChangedEventType.Insert,
                Index = index,
                Items = new[] { model }
            });
        }

        public virtual void Remove(T model)
        {
            if (CanRefresh)
            {
                UnbindRefresh(model as IObservable<PropertyEvent>);
            }

            InternalList.Remove(model);
            Publish(ListChangedEventType.Remove, model);
        }

        public virtual void RemoveRange(IEnumerable<T> models)
        {
            if (CanRefresh)
            {
                for (int i = 0; i < models.Count(); i++)
                {
                    UnbindRefresh(models.ElementAt(i) as IObservable<PropertyEvent>);
                }
            }

            for (int i = 0; i < models.Count(); i++)
            {
                InternalList.Remove(models.ElementAt(i));
            }

            Publish(ListChangedEventType.Remove, models);
        }

        public virtual void Replace(T model)
        {
            
            Remove(model);
            Add(model);

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

        public virtual int IndexOf(T model)
        {
            return InternalList.IndexOf(model);
        }

        public void Clear()
        {
            InternalList.Clear();
            Publish(ListChangedEventType.Clear, null);
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
            set
            {
                InternalList[index] = value;
                Publish(new ListEvent
                {
                    Event = ListChangedEventType.Replace,
                    Index = index,
                    Items = new []{value}
                });
            }
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

        #endregion

        #region Refresh Binding

        // Subscribes to the item in the collection.
        // Raises a 'item refreshed' when that item says to.

        bool CanRefresh
        {
            get { return typeof(T).IsAssignableFrom(typeof(IObservable<PropertyEvent>)); }
        }

        void BindRefresh(IObservable<PropertyEvent> model)
        {
            model.OnPublish += OnRefresh;
        }
        void UnbindRefresh(IObservable<PropertyEvent> model)
        {
            model.OnPublish -= OnRefresh;
        }

        void OnRefresh(PropertyEvent args)
        {
            if (!args.IsRefreshAll)
                return;

            Publish(ListChangedEventType.Refresh, args.Sender);
        }

        #endregion

    }
}