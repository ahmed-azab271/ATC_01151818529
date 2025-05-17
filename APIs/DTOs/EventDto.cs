﻿using System.ComponentModel.DataAnnotations;

namespace APIs.DTOs
{
    public class EventDto
    {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Venue { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
        public string CategoryName { get; set; }
        public List<string> Tags { get; set; }
    }
}
