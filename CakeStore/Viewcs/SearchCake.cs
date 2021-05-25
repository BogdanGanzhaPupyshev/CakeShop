using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CakeStore.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CakeStore.Viewcs
{
    public class SearchCake
    {
        [Required]
        [DisplayName("Serach")]
        public string SearchText { get; set; }

        public IEnumerable<Cakes> CakeList { get; set; }

    }
}
