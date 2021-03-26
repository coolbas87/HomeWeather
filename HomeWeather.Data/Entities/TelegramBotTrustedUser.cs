namespace HomeWeather.Data.Entities
{
    public class TelegramBotTrustedUser : IEntity
    {
        public long tbtuID { get; set; }
        public int userID { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
    }
}
