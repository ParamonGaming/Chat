namespace Models
{
    public class ChanelModel
    {
        public int? Id { get; set; }
        public string GroupName { get; set; }
        public List<UserModel> Users { get; set; }
    }
}
