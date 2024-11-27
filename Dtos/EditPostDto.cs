namespace DotnetAPI.Dtos
{
    public class EditPostDto
    {
        public int PostId { get; set; }
        public string PostTitle { get; set; } = "";
        public string PostContent { get; set; } = "";
    }
}