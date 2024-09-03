﻿using System.ComponentModel.DataAnnotations;

namespace YongXing_Project.Models
{
    public class Booking
    {
        [Key]
        public int? BookingID { get; set; }
        public string? FacilityDescription { get; set; }
        public DateTime? BookingDateFrom { get; set; }
        public DateTime? BookingDateTo { get; set; }
        public string? BookedBy { get; set; }
        public string? BookingStatus { get; set; }
    }
}
