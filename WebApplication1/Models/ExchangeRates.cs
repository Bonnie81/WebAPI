namespace WebApplication1.Models
{
        public class ExchangeRates
        {
            public int RateId { get; set; }
            public string SourceCurrencyCode { get; set; }

            public string TargetCurrencyCode { get; set; }
            public decimal Rate { get; set; }
            public DateTime ExchangeDate { get; set; }
        }
    }