namespace GameHubApi.Providers
{
    /// <summary>
    /// Defines a generic interface for an in-memory Least Recently Used (LRU) cache.
    /// </summary>
    /// <typeparam name="TKey">The type of keys used to identify cached items. Must be non-nullable.</typeparam>
    /// <typeparam name="TValue">The type of values stored in the cache. Must be a reference type.</typeparam>
    public interface ILruCache<TKey, TValue>
        where TKey : notnull
        where TValue : class
    {
        /// <summary>
        /// Retrieves a cached value by its key.
        /// </summary>
        /// <param name="key">The key associated with the cached item.</param>
        /// <returns>The cached value if found; otherwise, <c>null</c>.</returns>
        TValue? Get(TKey key);

        /// <summary>
        /// Stores a value in the cache with an optional time-to-live (TTL).
        /// </summary>
        /// <param name="key">The key to associate with the value.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="timeToLive">Optional duration to keep the item in cache before expiration.</param>
        void Set(TKey key, TValue value, TimeSpan? timeToLive);

        /// <summary>
        /// Removes a specific item from the cache.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        void Remove(TKey key);

        /// <summary>
        /// Clears all items from the cache.
        /// </summary>
        void Clear();
    }
}
