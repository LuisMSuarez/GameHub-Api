namespace GameHubApi
{
    public class CollectionResult<T>
    {
        public int count { get; set; }
        public string? next { get; set; }
        public string? previous { get; set; }
        public List<T> results { get; set; } = new List<T>();
    }
}
