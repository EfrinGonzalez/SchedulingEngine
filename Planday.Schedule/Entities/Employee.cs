namespace Planday.Schedule.Domain.Entities
{
    public class Employee(long id, string name)
    {
        public long Id { get; } = id;
        public string Name { get; } = name;
    }    
}

