using System;

namespace HomeWeather.Domain.DTO.TelegramBot
{
    internal record UntrustedUserDTO : UserDTO
    {
        public DateTime TimeLastRequest { get; init; }
        public bool IsNotified { get; set; }
    }
}
