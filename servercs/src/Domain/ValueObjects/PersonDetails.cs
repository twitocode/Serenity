using Microsoft.EntityFrameworkCore;

namespace Serenity.Domain.ValueObjects;

[Owned]
public class PersonDetails : ValueObject
{
    public string Gender { get; private set; }
    public string Pronouns { get; private set; }
    public PersonDetails() { }

    public PersonDetails(string gender, string pronouns)
    {
        Gender = gender;
        Pronouns = pronouns;
    }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Gender;
        yield return Pronouns;
    }
}