using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MVC.Models
{
    public class Color
    {
        public int Id { get; set; }

        [DisplayName("Tên Màu")]
        public string Colors { get; set; }
        // Collection reference property cho khóa ngoại từ productdetail
        public List<ProductDetail> ProductDetail { get; set; }
    }
}
