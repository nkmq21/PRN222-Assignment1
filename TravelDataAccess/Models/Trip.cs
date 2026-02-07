using System;
using System.Collections.Generic;

namespace TravelDataAccess;

public partial class Trip
{
    public int TripId { get; set; }

    public string Code { get; set; } = null!;

    public string Destination { get; set; } = null!;

    public decimal Price { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
