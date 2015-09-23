namespace RoyaltyDataCalculator.Model
{
    /// <summary>
    /// Класс описания результатов метода Preview()
    /// </summary>
    public class DataPreviewRow
    {
        /// <summary>
        /// Адрес
        /// </summary>
        public Parser.Address Address { get; set; }
        /// <summary>
        /// Город
        /// </summary>
        public RoyaltyRepository.Models.City City { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public RoyaltyRepository.Models.Mark Mark { get; set; }
        /// <summary>
        /// Телефон
        /// </summary>
        public RoyaltyRepository.Models.Phone Phone { get; set; }
        /// <summary>
        /// Хост
        /// </summary>
        public RoyaltyRepository.Models.Host Host { get; set; }
        /// <summary>
        /// Улица
        /// </summary>
        public RoyaltyRepository.Models.Street Street { get; set; }
    }
}
