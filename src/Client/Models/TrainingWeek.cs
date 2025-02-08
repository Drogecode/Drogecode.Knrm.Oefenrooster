namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public class TrainingWeek
{
    public LinkedList<Training> Trainings { get; set; } = new ();
    public DateOnly From { get; set; }
    public DateOnly Till { get; set; }
}
