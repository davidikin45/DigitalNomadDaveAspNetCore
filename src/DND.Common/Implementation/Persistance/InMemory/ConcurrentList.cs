using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace DND.Common.Implementation.Persistance.InMemory
{
    public class ConcurrentList<TEntity> : IList<TEntity>
    {
        private readonly List<TEntity> _underlyingList = new List<TEntity>();
        private readonly object _syncRoot = new object();
        private readonly ConcurrentQueue<TEntity> _underlyingQueue;
        private bool _requiresSync;
        private bool _isDirty;

        public ConcurrentList()
        {
            _underlyingQueue = new ConcurrentQueue<TEntity>();
        }

        public ConcurrentList(IEnumerable<TEntity> items)
        {
            _underlyingQueue = new ConcurrentQueue<TEntity>(items);
        }

        private void UpdateLists()
        {
            if (!_isDirty)
                return;
            lock (_syncRoot)
            {
                _requiresSync = true;
                TEntity temp;
                while (_underlyingQueue.TryDequeue(out temp))
                    _underlyingList.Add(temp);
                _requiresSync = false;
            }
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TEntity item)
        {
            if (_requiresSync)
                lock (_syncRoot)
                    _underlyingQueue.Enqueue(item);
            else
                _underlyingQueue.Enqueue(item);
            _isDirty = true;
        }

        public int Add(object value)
        {
            if (_requiresSync)
                lock (_syncRoot)
                    _underlyingQueue.Enqueue((TEntity)value);
            else
                _underlyingQueue.Enqueue((TEntity)value);
            _isDirty = true;
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.IndexOf((TEntity)value);
            }
        }

        public bool Contains(object value)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.Contains((TEntity)value);
            }
        }

        public int IndexOf(object value)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.IndexOf((TEntity)value);
            }
        }

        public void Insert(int index, object value)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.Insert(index, (TEntity)value);
            }
        }

        public void Remove(object value)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.Remove((TEntity)value);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.RemoveAt(index);
            }
        }

        public TEntity this[int index]
        {
            get
            {
                lock (_syncRoot)
                {
                    UpdateLists();
                    return _underlyingList[index];
                }
            }
            set
            {
                lock (_syncRoot)
                {
                    UpdateLists();
                    _underlyingList[index] = value;
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public void Clear()
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.Clear();
            }
        }

        public bool Contains(TEntity item)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.Contains(item);
            }
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(TEntity item)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.Remove(item);
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.CopyTo((TEntity[])array, index);
            }
        }

        public int Count
        {
            get
            {
                lock (_syncRoot)
                {
                    UpdateLists();
                    return _underlyingList.Count;
                }
            }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public int IndexOf(TEntity item)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                return _underlyingList.IndexOf(item);
            }
        }

        public void Insert(int index, TEntity item)
        {
            lock (_syncRoot)
            {
                UpdateLists();
                _underlyingList.Insert(index, item);
            }
        }
    }
}
