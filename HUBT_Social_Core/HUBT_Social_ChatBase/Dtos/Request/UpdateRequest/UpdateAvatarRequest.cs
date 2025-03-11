namespace ChatBase.Dtos.Request.UpdateRequest
{
    public class UpdateAvatarRequest
    {
        public string Id { get; set; } = String.Empty;
        public FormFile file { get; set; }
    }
}
