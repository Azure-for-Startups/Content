namespace VideoSharing.Web.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using DataAccess.Model;

    public static class VideoElementExtensions
    {
        private static string GetCurrentUserId => ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;

        public static IEnumerable<VideoElement> OwnItems(this IEnumerable<VideoElement> query)
        {
            if (query != null)
            {
                return query.Where(e => e.UserId.Equals(GetCurrentUserId, StringComparison.OrdinalIgnoreCase));
            }

            throw new ArgumentNullException(nameof(query));
        }

        public static IQueryable<VideoElement> OwnItems(this IQueryable<VideoElement> query)
        {
            if (query != null)
            {
                return query.Where(e => e.UserId.Equals(GetCurrentUserId, StringComparison.OrdinalIgnoreCase));
            }

            throw new ArgumentNullException(nameof(query));
        }

        public static IEnumerable<VideoElement> OwnAndSharedItems(this IEnumerable<VideoElement> query)
        {
            if (query != null)
            {
                return query.Where(e => e.UserId.Equals(GetCurrentUserId, StringComparison.OrdinalIgnoreCase) || e.IsPublic);
            }

            throw new ArgumentNullException(nameof(query));
        }

        public static IQueryable<VideoElement> OwnAndSharedItems(this IQueryable<VideoElement> query)
        {
            if (query != null)
            {
                return query.Where(e => e.UserId.Equals(GetCurrentUserId, StringComparison.OrdinalIgnoreCase) || e.IsPublic);
            }

            throw new ArgumentNullException(nameof(query));
        }

        public static IEnumerable<VideoElement> SharedItems(this IEnumerable<VideoElement> query)
        {
            if (query != null)
            {
                return query.Where(e => !e.UserId.Equals(GetCurrentUserId, StringComparison.OrdinalIgnoreCase) && e.IsPublic);
            }

            throw new ArgumentNullException(nameof(query));
        }
    }
}