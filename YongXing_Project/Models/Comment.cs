using System.ComponentModel.DataAnnotations;

namespace YongXing_Project.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public int BookingID { get; set; }
        public string Text { get; set; }
    }

}
