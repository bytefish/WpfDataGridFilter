namespace WpfDataGridFilter.Infrastructure
{
    public static class PackUriUtils
    {
        public static Uri GetAbsoluteUri(string path)
        {
            return new Uri($"pack://application:,,,/WpfDataGridFilter;component/{path}");
        }
    }
}
