using System;

namespace imovi_backend.Models
{
    public class CustomList:BaseEntity
    {
        public string ListName { get; set; }
        public Guid UserId { get; set; }
        public bool IsVisible { get; set; }

    }
}
