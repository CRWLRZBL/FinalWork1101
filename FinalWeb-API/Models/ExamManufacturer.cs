using System;
using System.Collections.Generic;

namespace FinalWeb_API.Models;

public partial class ExamManufacturer
{
    public int ManufacturerId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ExamProduct> ExamProducts { get; set; } = new List<ExamProduct>();
}
