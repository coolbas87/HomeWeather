namespace HomeWeather.Domain.DTO.TelegramBot
{
    public record TrustedUserDTO : UserDTO
    {
        public string Username { get; init; }
        public string FirstName { get; init; }
    }
}
