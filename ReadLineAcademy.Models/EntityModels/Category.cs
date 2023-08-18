using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Models.EntityModels
{
    public class Category
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(1, 500)]
        [DisplayName("Display Order")]
        public int DisplayOrder { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
    }
}
