using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadLineAcademy.Models.EntityModels
{
    public class CoverType
    {
        public int Id   { get; set; }
        [DisplayName("Cover Type")]
        [Required]
        [StringLength(25,ErrorMessage ="Please Give a CoverType within Range")]
        public string Name { get; set; }
    }
}
