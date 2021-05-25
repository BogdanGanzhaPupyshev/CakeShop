using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;

namespace CakeStore.Models
{
    public class CakeIngridients
    {
        public int Id { get; set; }

        [DisplayName("Select Cake")]
        public int CakeId { get; set; }

        [DisplayName("Select Ingredient")]
        public int IngredientId { get; set; }
        public virtual Ingredients Ingredient { get; set; }
        public virtual Cakes Cake { get; set; }

    }
}
