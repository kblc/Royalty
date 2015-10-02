namespace RoyaltyDataCalculator.Model
{
    /// <summary>
    /// Класс описания результатов метода Preview()
    /// </summary>
    public class DataPreviewRow
    {
        /// <summary>
        /// 
        /// </summary>
        public RoyaltyRepository.Models.AccountDataRecord DataRecord { get; set; }
        /// <summary>
        /// Метка
        /// </summary>
        public RoyaltyRepository.Models.AccountDataRecordAdditional DataRecordAdditional { get; set; }
    }
}
