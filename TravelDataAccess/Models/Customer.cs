using System;
using System.Collections.Generic;

namespace TravelDataAccess;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Code { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Email { get; set; }

    public int? Age { get; set; }

    public string Password { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
