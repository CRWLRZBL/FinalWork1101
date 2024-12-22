using System;
using System.Collections.Generic;

namespace ServicesLibrary.Models;

public partial class ExamPickupPoint
{
    public int PickupPointId { get; set; }

    public int Index { get; set; }

    public string City { get; set; } = null!;

    public string Street { get; set; } = null!;

    public byte HomeNumber { get; set; }

    public virtual ICollection<ExamOrder> ExamOrders { get; set; } = new List<ExamOrder>();
}
