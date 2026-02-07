using System;
using System.Collections.Generic;

namespace TravelDataAccess;

public partial class Booking
{
    public int BookingId { get; set; }

    public int TripId { get; set; }

    public int CustomerId { get; set; }

    public DateOnly BookingDate { get; set; }

    public string Status { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Trip Trip { get; set; } = null!;
}
