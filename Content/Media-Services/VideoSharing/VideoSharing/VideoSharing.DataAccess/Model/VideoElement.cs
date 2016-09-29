namespace VideoSharing.DataAccess.Model
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class VideoElement
    {
        public Guid Id { get; set; }

        [Required]
        public string UserId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string AssetId { get; set; }

        [Required]
        public string StreamingUrl { get; set; }

        [DisplayName("Shared")]
        public bool IsPublic { get; set; }
    }
}