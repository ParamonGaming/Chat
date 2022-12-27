namespace Models
{
    public class MessageData
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public int? AuthorID { get; set; }
        public int? ChanelID { get; set; }
        public DateTime Time { get; set; }
    }
}
