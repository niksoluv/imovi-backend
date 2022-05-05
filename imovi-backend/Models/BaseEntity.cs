﻿using System;

namespace imovi_backend.Models
{
    public class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
