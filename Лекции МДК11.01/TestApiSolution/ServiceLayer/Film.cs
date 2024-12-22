using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer
{
    public partial class Film
    {
        public int FilmId { get; set; }

        public string Name { get; set; } = null!;

        public short ReleaseDate { get; set; }

        public short Duration { get; set; }

        public string? Description { get; set; }

        public string? Poster { get; set; }

        public string? AgeYear { get; set; }

        public DateOnly? RentalStart { get; set; }

        public DateOnly? RentalEnd { get; set; }
    }
}
