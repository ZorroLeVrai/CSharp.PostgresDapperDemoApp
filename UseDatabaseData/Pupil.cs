using System.Globalization;

namespace UseDatabaseData;

public class Pupil
{
    string FirstName { get; }
    string LastName { get; }
    DateTime BirthDate { get; }

    //you have to use the same field names as in the database
    public Pupil(string prenom, string nom, DateTime date_naissance)
    {
        FirstName = prenom;
        LastName = nom;
        BirthDate = date_naissance;
    }

    public override string ToString()
    {
        return $"{FirstName} {LastName} - {BirthDate.ToString("d/M/yyyy", CultureInfo.InvariantCulture)}";
    }
}
