namespace nalim
{
    public enum NotificationType
    {
        Info,
        Warning,
        Error,
        System
    }

    public enum SortNotificationBy
    {
        Date,
        Priority,
        Title
    }

    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public NotificationType Type { get; set; } // Info, Warning, Error, System
        public DateTime CreatedAt { get; set; }
        public int Priority { get; set; } // 0 (низкий) до 5 (высокий)
        public bool IsRead { get; set; }
    }

    public class NotificationFilterOptions
    {
        public bool? IsRead { get; set; }
        public NotificationType[]? Types { get; set; } // список нужных типов
        public string? SearchText { get; set; }
        public int? MinPriority { get; set; }
        public SortNotificationBy? SortBy { get; set; } // по Date, Priority, Title
        public bool Descending { get; set; }
    }

    public class NotificationService
    {
        public IEnumerable<Notification> FilterAndSort(
            IEnumerable<Notification> notifications,
            NotificationFilterOptions options)
        {
            var filteredNotifications = notifications;

            if (options.IsRead.HasValue)
            {
                filteredNotifications = filteredNotifications
                    .Where(n => n.IsRead == options.IsRead.Value);
            }

            if (options.Types != null && options.Types.Length > 0)
            {
                filteredNotifications = filteredNotifications
                    .Where(n => options.Types.Contains(n.Type));
            }

            if (!string.IsNullOrEmpty(options.SearchText))
            {
                filteredNotifications = filteredNotifications
                    .Where(n => n.Title.Contains(options.SearchText, StringComparison.OrdinalIgnoreCase) ||
                                 (n.Content != null && n.Content.Contains(options.SearchText, StringComparison.OrdinalIgnoreCase)));
            }

            if (options.MinPriority.HasValue)
            {
                filteredNotifications = filteredNotifications
                    .Where(n => n.Priority >= options.MinPriority.Value);
            }

            filteredNotifications = options.SortBy switch
            {
                SortNotificationBy.Date => options.Descending ?
                    filteredNotifications.OrderByDescending(n => n.CreatedAt) :
                    filteredNotifications.OrderBy(n => n.CreatedAt),
                SortNotificationBy.Priority => options.Descending ?
                    filteredNotifications.OrderByDescending(n => n.Priority) :
                    filteredNotifications.OrderBy(n => n.Priority),
                SortNotificationBy.Title => options.Descending ?
                    filteredNotifications.OrderByDescending(n => n.Title) :
                    filteredNotifications.OrderBy(n => n.Title),
                _ => filteredNotifications
            };

            return filteredNotifications;
        }
    }

}
