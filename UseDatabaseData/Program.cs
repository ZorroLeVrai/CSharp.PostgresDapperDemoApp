using Npgsql;
using UseDatabaseData;
using Dapper;
using System.Xml.Linq;

const string CONNECTION_STRING = "Server=127.0.0.1;Port=5432;Database=Exercices;User Id=postgres;Password=pass123;";

await using var connection = new NpgsqlConnection(CONNECTION_STRING);

//open connection
await connection.OpenAsync();

bool tomSelectExists = await CheckIfPupilExistsWithDapper("Tom", "Select");
if (!tomSelectExists)
    InsertPupilDataWithDapper("Tom", "Select", new DateOnly(2001, 1, 1));
var pupils = await ReadPupilDataWithDapper();
foreach (var pupil in pupils)
    Console.WriteLine(pupil);

//await foreach (var pupil in ReadPupilDataWithoutDapper())
//{
//    Console.WriteLine(pupil);
//}

Console.Read();

async void InsertPupilDataWithoutDapper(string firstName, string lastName, DateOnly birthDate)
{
    //insert some data
    await using (var cmd = new NpgsqlCommand("insert into eleve (nom, prenom, date_naissance) values (@lname, @fname, @birthdate)", connection))
    {
        cmd.Parameters.AddWithValue("lname", lastName);
        cmd.Parameters.AddWithValue("fname", firstName);
        cmd.Parameters.AddWithValue("birthdate", birthDate);
        await cmd.ExecuteNonQueryAsync();
    }
}

async IAsyncEnumerable<Pupil> ReadPupilDataWithoutDapper()
{
    await using (var cmd = new NpgsqlCommand("SELECT prenom, nom, date_naissance FROM eleve", connection))
    await using (var reader = await cmd.ExecuteReaderAsync())
    {
        while (await reader.ReadAsync())
        {
            var firstName = reader.GetString(0);
            var lastName = reader.GetString(1);
            var birthDate = reader.GetDateTime(2);

            yield return new Pupil(firstName, lastName, birthDate);
        }
    }
}

async void InsertPupilDataWithDapper(string firstName, string lastName, DateOnly birthDate)
{
    if (connection == null)
        throw new ArgumentNullException(nameof(connection));

    var sql = "insert into eleve (nom, prenom, date_naissance) values (@lname, @fname, @birthdate)";
    object[] parameters = { new { lname = lastName, fname = firstName, birthdate = birthDate.ToDateTime(new TimeOnly(0,0,0)) } };
    connection.Execute(sql, parameters);
}

async Task<IEnumerable<Pupil>> ReadPupilDataWithDapper()
{
    if (connection == null)
        throw new ArgumentNullException(nameof(connection));
    return await connection.QueryAsync<Pupil>("SELECT prenom, nom, date_naissance FROM eleve");
}

async Task<bool> CheckIfPupilExistsWithDapper(string firstName, string lastName)
{
    if (connection == null)
        throw new ArgumentNullException(nameof(connection));
    var sql = @"SELECT count(1)
        FROM eleve
        WHERE prenom = @fname
        AND nom = @lname";

    var parameters = new DynamicParameters();
    parameters.Add("@fname", firstName);
    parameters.Add("@lname", lastName);
    int count = await connection.ExecuteScalarAsync<int>(sql, parameters);
    return count > 0;
}
