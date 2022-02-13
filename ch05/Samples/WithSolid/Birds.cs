public abstract class Bird
{
    public abstract void Fly();
    public abstract void Walk();
}

public class Robin : Bird
{
    public override void Fly() => Console.WriteLine("fly");
    public override void Walk() => Console.WriteLine("walk");
}

public class Ostrich : Bird 
{
    public override void Fly() => throw new InvalidOperationException();
    public override void Walk() => Console.WriteLine("walk");
}

