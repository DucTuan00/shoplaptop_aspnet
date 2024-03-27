using System.ComponentModel.DataAnnotations;

namespace Ictshop.Models {
    public class MailModel
    {
        public string From
        {
            get;
            set;
        }
        [StringLength(50)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email bắt buộc nhập")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail sai định dạng")]
        public string To
        {
            get;
            set;
        }
        public string Subject
        {
            get;
            set;
        }
        public string Body
        {
            get;
            set;
        }
        [Display(Name = "Mã xác nhận")]
        [Required(ErrorMessage = "Mã xác nhận bắt buộc nhập")]
        public int code { get; set; }
    }
}

