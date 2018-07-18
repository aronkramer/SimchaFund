using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimchaFund.Data
{
    public class DataBase
    {
        private string _connectionString;

        public DataBase(string ConnectionString)
        {
            _connectionString = ConnectionString;
        }

        public void AddContributor(Contributors contributor)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Contributors(firstname, lastname, cellnumber, date, alwaysInclude) " +
                                  "VALUES (@firstname, @lastname, @cell, @date, @alwaysInclude) SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@firstname", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastname", contributor.LastName);
                cmd.Parameters.AddWithValue("@cell", contributor.CellNumber);
                cmd.Parameters.AddWithValue("@date", contributor.Date);
                cmd.Parameters.AddWithValue("@alwaysInclude", contributor.AlwaysInclude);
                con.Open();
                contributor.Id = (int)(decimal)cmd.ExecuteScalar();
            }
        }

        public void Deposit(Deposits deposit)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO Deposits(contributorId, amount, date) " +
                                  "VALUES(@contributorId, @amount, @date)";
                cmd.Parameters.AddWithValue("@contributorId", deposit.ContributorsId);
                cmd.Parameters.AddWithValue("@amount", deposit.Amount);
                cmd.Parameters.AddWithValue("@date", deposit.Date);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<SimchaContributor> GetSimchaContributors()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"select * from Contributors";
                List<SimchaContributor> list = new List<SimchaContributor>();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    SimchaContributor c = new SimchaContributor();
                    c.ContributorId = (int)reader["Id"];
                    c.FirstName = (string)reader["FirstName"];
                    c.LastName = (string)reader["LastName"];
                    c.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    c.Balance = CurrentBalance(c.ContributorId);
                    list.Add(c);
                }
                return list;
            }
        }

        public IEnumerable<Contributors> GetContributors()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"select * from Contributors";
                List<Contributors> list = new List<Contributors>();
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Contributors c = new Contributors();
                    c.Id = (int)reader["Id"];
                    c.FirstName = (string)reader["FirstName"];
                    c.LastName = (string)reader["LastName"];
                    c.CellNumber = (string)reader["CellNumber"];
                    c.Date = (DateTime)reader["Date"];
                    c.AlwaysInclude = (bool)reader["AlwaysInclude"];
                    c.Amount = CurrentBalance(c.Id);
                    list.Add(c);
                }
                return list;
            }
        }

        public int ContributorCount()
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select count(*) from contributors";
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public void UpdateContributor(Contributors contributor)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"UPDATE Contributors SET Firstname = @firstname,
                                    Lastname = @lastname, cellnumber = @cell, date = @date,
                                    alwaysinclude = @alwaysinclude
                                    where id = @id";
                cmd.Parameters.AddWithValue("@firstname", contributor.FirstName);
                cmd.Parameters.AddWithValue("@lastname", contributor.LastName);
                cmd.Parameters.AddWithValue("@cell", contributor.CellNumber);
                cmd.Parameters.AddWithValue("@date", contributor.Date);
                cmd.Parameters.AddWithValue("@alwaysinclude", contributor.AlwaysInclude);
                cmd.Parameters.AddWithValue("@id", contributor.Id);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public IEnumerable<Deposits> GetDepositsById(int contribId)
        {
            List<Deposits> deposits = new List<Deposits>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Deposits WHERE ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", contribId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Deposits deposit = new Deposits();
                    deposit.Id = (int)reader["Id"];
                    deposit.Amount = (decimal)reader["Amount"];
                    deposit.Date = (DateTime)reader["Date"];
                    deposits.Add(deposit);
                }
            }

            return deposits;
        }

        public IEnumerable<Contribution> GetContributionsById(int contribId)
        {
            List<Contribution> contributions = new List<Contribution>();
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT c.*, s.Name, s.Date FROM Contributions c 
                                    JOIN Simchas s ON c.SimchaId = s.Id
                                    WHERE c.ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", contribId);
                connection.Open();
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var contribution = new Contribution();
                    contribution.ContributorId = (int)reader["ContributorId"];
                    contribution.Amount = (decimal)reader["Amount"];
                    contribution.SimchaId = (int)reader["SimchaId"];
                    contribution.SimchaName = (string)reader["Name"];
                    contribution.Date = (DateTime)reader["Date"];
                    contributions.Add(contribution);
                }
            }

            return contributions;
        }

        public decimal CurrentBalance(int id)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"select IsNull(sum(Amount),0) from Deposits
                                     where ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", id);
                con.Open();
                decimal depositTotal = (decimal)cmd.ExecuteScalar();
                cmd.Parameters.Clear();
                cmd.CommandText = "SELECT IsNull(SUM(Amount),0) FROM Contributions WHERE ContributorId = @contribId";
                cmd.Parameters.AddWithValue("@contribId", id);
                decimal contibutionsTotal = (decimal)cmd.ExecuteScalar();
                return depositTotal - contibutionsTotal;
            }
        }

        public void UpdateSimchaContributions(int simchaId, IEnumerable<ContributionInclusion> contributors)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM Contributions WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simchaId);

                connection.Open();
                cmd.ExecuteNonQuery();

                cmd.Parameters.Clear();
                cmd.CommandText = @"INSERT INTO Contributions (SimchaId, ContributorId, Amount)
                                    VALUES (@simchaId, @contributorId, @amount)";
                foreach (ContributionInclusion contributor in contributors.Where(c => c.Include))
                {
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@simchaId", simchaId);
                    cmd.Parameters.AddWithValue("@contributorId", contributor.ContributorId);
                    cmd.Parameters.AddWithValue("@amount", contributor.Amount);
                    cmd.ExecuteNonQuery();
                }

            }
        }

        public void AddSimcha(Simcha simcha)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Simchas(name, date)
                                    VALUES(@name, @date)";
                cmd.Parameters.AddWithValue("@name", simcha.SimchaName);
                cmd.Parameters.AddWithValue("@date", simcha.Date);
                con.Open();
                cmd.ExecuteNonQuery();
            }
        }

        private void SetSimchaTotals(Simcha simcha)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"SELECT ISNull(SUM(Amount), 0) as Total, Count(*) as ContributorAmount
                FROM Contributions
                WHERE SimchaId = @simchaId";
                cmd.Parameters.AddWithValue("@simchaId", simcha.Id);
                connection.Open();
                var reader = cmd.ExecuteReader();
                reader.Read();
                simcha.ContributorAmount = (int)reader["ContributorAmount"];
                simcha.Total = (decimal)reader["Total"];
            }
        }

        public IEnumerable<Simcha> GetSimcha()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM Simchas";
                List<Simcha> list = new List<Simcha>();
                con.Open();
                var reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    Simcha simcha = new Simcha();
                    simcha.Id = (int)reader["Id"];
                    simcha.SimchaName = (string)reader["name"];
                    simcha.Date = (DateTime)reader["date"];
                    SetSimchaTotals(simcha);
                    list.Add(simcha);
                }
                return list;
            }
        }

        public int GetPplAmount()
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = "select count(*) from Contributors";
                con.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

        public Simcha GetSimchaById(int ContributorId)
        {
            using (var con = new SqlConnection(_connectionString))
            using (var cmd = con.CreateCommand())
            {
                cmd.CommandText = @"select top 1 * from simchas
                                   where id = @id";
                cmd.Parameters.AddWithValue("@id", ContributorId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if(!reader.Read())
                {
                    return null;
                }
                Simcha simcha = new Simcha
                {
                    Id = (int)reader["Id"],
                    SimchaName = (string)reader["name"],
                    Date = (DateTime)reader["date"]
                };
                return simcha;
            }
        }

        public string GetContributorsName(int contribId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.CommandText = "select firstname, lastname from contributors where id = @id";
                cmd.Parameters.AddWithValue("@id", contribId);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                string first = (string)reader["firstname"];
                string last = (string)reader["lastname"];
                return $"{first} {last}";
            }
        }
    }
}
