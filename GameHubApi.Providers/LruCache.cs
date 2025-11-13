namespace GameHubApi.Providers
{
    /// <summary>
    /// A thread-safe, in-memory Least Recently Used (LRU) cache implementation with optional expiration.
    /// </summary>
    /// <typeparam name="TKey">The type of the cache key. Must be non-nullable.</typeparam>
    /// <typeparam name="TValue">The type of the cache value. Must be a reference type.</typeparam>
    public class LruCache<TKey, TValue> : ILruCache<TKey, TValue>
        where TKey : notnull
        where TValue : class
    {
        private readonly int size;
        private readonly LinkedList<(TKey key, TValue value, DateTime? expiration)> list = new();
        private readonly Dictionary<TKey, LinkedListNode<(TKey key, TValue value, DateTime? expiration)>> dict = new();
        private readonly object syncRoot = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="LruCache{TKey, TValue}"/> class.
        /// </summary>
        /// <param name="size">The maximum number of items the cache can hold.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the size is less than 1.</exception>
        public LruCache(int size)
        {
            if (size < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(size));
            }

            this.size = size;
        }

        /// <inheritdoc />
        public TValue? Get(TKey key)
        {
            lock (syncRoot)
            {
                return this.GetInternal(key);
            }
        }

        /// <inheritdoc />
        public void Set(TKey key, TValue value, TimeSpan? timeToLive)
        {
            lock (syncRoot)
            {
                this.SetInternal(key, value, timeToLive);
            }
        }

        /// <inheritdoc />
        public void Remove(TKey key)
        {
            lock (syncRoot)
            {
                this.RemoveInternal(key);
            }
        }

        /// <inheritdoc />
        public void Clear()
        {
            lock (syncRoot)
            {
                this.ClearInternal();
            }
        }

        /// <summary>
        /// Retrieves a value from the cache without locking. Updates usage order if found and valid.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The cached value, or <c>null</c> if not found or expired.</returns>
        public TValue? GetInternal(TKey key)
        {
            if (dict.TryGetValue(key, out var node))
            {
                if (node.Value.expiration == null || DateTime.UtcNow < node.Value.expiration.Value)
                {
                    // Move accessed node to the front (most recently used)
                    list.Remove(node);
                    list.AddFirst(node);
                    return node.Value.value;
                }
                else
                {
                    // Expired entry
                    RemoveInternal(key);
                }
            }
            return null;
        }

        /// <summary>
        /// Adds or updates a value in the cache without locking. Evicts least recently used item if full.
        /// </summary>
        /// <param name="key">The key to store.</param>
        /// <param name="value">The value to store.</param>
        /// <param name="timeToLive">Optional expiration duration.</param>
        /// <exception cref="ArgumentNullException">Thrown if the key is null.</exception>
        /// <exception cref="NullReferenceException">Thrown if eviction fails due to an unexpected null node.</exception>
        public void SetInternal(TKey key, TValue value, TimeSpan? timeToLive)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            if (dict.ContainsKey(key))
            {
                list.Remove(dict[key]);
                dict.Remove(key);
            }
            else if (list.Count >= this.size)
            {
                var last = list.Last;
                if (last == null)
                {
                    throw new NullReferenceException("No value to evict!");
                }
                dict.Remove(last.Value.key);
                list.RemoveLast();
            }

            var node = new LinkedListNode<(TKey key, TValue value, DateTime? expiration)>(
                (key, value, timeToLive == null ? null : DateTime.UtcNow.Add(timeToLive.Value)));
            list.AddFirst(node);
            dict[key] = node;
        }

        /// <summary>
        /// Removes a specific key from the cache without locking.
        /// </summary>
        /// <param name="key">The key to remove.</param>
        public void RemoveInternal(TKey key)
        {
            if (dict.TryGetValue(key, out var node))
            {
                list.Remove(node);
                dict.Remove(key);
            }
        }

        /// <summary>
        /// Clears all entries from the cache without locking.
        /// </summary>
        private void ClearInternal()
        {
            list.Clear();
            dict.Clear();
        }
    }
}
