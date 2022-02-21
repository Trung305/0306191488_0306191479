using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MVC.Models
{
    public class ProductDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DisplayName("Sản phẩm")]
        public int ProductId { get; set; }
        // Navigation reference property cho khóa ngoại đến Product
        public Product Product { get; set; }

        [DisplayName("Size")]
        [ForeignKey("Size")]
        public int? Size_id { get; set; }
        public Size Size { get; set; }

        [DisplayName("Mau")]
        [ForeignKey("Color")]
        public int? Color_id { get; set; }
        public Color Color { get; set; }
        public int Quantity { get; set; }







    }
}
