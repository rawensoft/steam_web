namespace SteamWeb.API.Models
{
    public class Response<T>
    {
        public bool success { get; internal set; } = false;
        /// <summary>
        /// default(T)
        /// </summary>
        public T response { get; init; } = default;
    }
    public class Result<T>
    {
        public bool success { get; internal set; } = false;
        /// <summary>
        /// default(T)
        /// </summary>
        public T result { get; init; } = default(T);
    }
    public class Response
    {
        public bool success { get; init; } = false;
    }
}
