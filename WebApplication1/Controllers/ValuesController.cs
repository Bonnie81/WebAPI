using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ValuesController : ControllerBase
    {

        private readonly string _connectionString;

        public ValuesController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("MyDatabaseConnection");
        }

        [HttpGet]
        public IActionResult GetQuery(String sourceCurrencyCode, DateTime querySDate, DateTime queryEDate)
        {
            if (querySDate > queryEDate)
            {
                return BadRequest("�L�Ī��d�߽d��");
            }

            if ((queryEDate - querySDate).TotalDays > 365)
            {
                return BadRequest("�d�߰϶����i�W�L�@�~");
            }

            if (queryEDate > DateTime.Today)
            {
                return BadRequest("�d�ߨ���ݤp����");
            }

            List<ExchangeRates> exchangeRatesList = new List<ExchangeRates>();
            StringBuilder str = new StringBuilder();

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                str.AppendLine("SELECT RateId,SourceCurrencyCode +'('+B.CurrencyName +')' AS SourceCurrencyCode, TargetCurrencyCode +'('+ C.CurrencyName +')' AS TargetCurrencyCode,Rate,ExchangeDate ");
                str.AppendLine("  FROM ExchangeRate A,Currency B,Currency C ");
                str.AppendLine(" WHERE A.SourceCurrencyCode = B.CurrencyCode ");
                str.AppendLine("   AND A.TargetCurrencyCode = C.CurrencyCode ");
                str.AppendLine("   AND A.SourceCurrencyCode = @SourceCurrencyCode ");
                str.AppendLine("   AND ExchangeDate Between @QuerySDate AND @QueryEDate ");

                using (var command = new SqlCommand(str.ToString(), conn))
                {
                    command.Parameters.AddWithValue("@SourceCurrencyCode", sourceCurrencyCode);
                    command.Parameters.AddWithValue("@QuerySDate", querySDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@QueryEDate", queryEDate.ToString("yyyy-MM-dd"));

                    using (var reader = command.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return NotFound();

                        while (reader.Read())
                        {
                            ExchangeRates exchangeRates = new ExchangeRates();
                            exchangeRates.RateId = reader.GetInt32(0);
                            exchangeRates.SourceCurrencyCode = reader.GetString(1);
                            exchangeRates.TargetCurrencyCode = reader.GetString(2);
                            exchangeRates.Rate = reader.GetDecimal(3);
                            exchangeRates.ExchangeDate = reader.GetDateTime(4);
                            exchangeRatesList.Add(exchangeRates);
                        }

                        return Ok(exchangeRatesList);
                    }
                }
            }
        }


        //�s�W���O���
        [HttpPost]
        public string Post(Currency currency)
        {
            StringBuilder str = new StringBuilder();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                str.AppendLine("INSERT INTO Currency");
                str.AppendLine("            (CurrencyCode,");
                str.AppendLine("             CurrencyName)");
                str.AppendLine("     VALUES");
                str.AppendLine("           (@CurrencyCode,");
                str.AppendLine("            @CurrencyName)");

                SqlCommand cmd = new SqlCommand(str.ToString(), conn);
                cmd.Parameters.AddWithValue("@CurrencyCode", currency.CurrencyCode);
                cmd.Parameters.AddWithValue("@CurrencyName", currency.CurrencyName);

                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i > 0)
                {
                    return "�s�W���O��Ʀ��\";
                }
                else
                {
                    return "�s�W���O��ƥ���";
                }
            }
        }

        //�ק���O���
        [HttpPut]
        public string Put(Currency currency)
        {
            StringBuilder str = new StringBuilder();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                str.AppendLine("UPDATE Currency");
                str.AppendLine("   SET CurrencyName = @CurrencyName");
                str.AppendLine(" WHERE CurrencyCode = @CurrencyCode");

                SqlCommand cmd = new SqlCommand(str.ToString(), conn);
                cmd.Parameters.AddWithValue("@CurrencyName", currency.CurrencyName);
                cmd.Parameters.AddWithValue("@CurrencyCode", currency.CurrencyCode);

                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i > 0)
                {
                    return "��s���O��Ʀ��\";
                }
                else
                {
                    return "��s���O��ƥ���";
                }
            }
        }

        //�R�����O���
        [HttpDelete]
        public string Delete(Currency currency)
        {
            StringBuilder str = new StringBuilder();
            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                str.AppendLine("DELETE FROM Currency ");
                str.AppendLine(" WHERE CurrencyCode = @CurrencyCode ");
                str.AppendLine("   AND CurrencyName = @CurrencyName ");

                SqlCommand cmd = new SqlCommand(str.ToString(), conn);
                cmd.Parameters.AddWithValue("@CurrencyCode", currency.CurrencyCode);
                cmd.Parameters.AddWithValue("@CurrencyName", currency.CurrencyName);

                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i > 0)
                {
                    return "�R�����O��Ʀ��\";
                }
                else
                {
                    return "�R�����O��ƥ���";
                }
            }
        }
    }
}


