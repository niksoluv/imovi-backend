using System;
using System.Collections.Generic;

namespace imovi_backend.Models
{
    public class CustomList:BaseEntity
    {
        public string ListName { get; set; }
        public Guid UserId { get; set; }
        public bool IsVisible { get; set; }
        public IEnumerable<CustomListMovie> RelatedMovies { get;set;}

    }
}
