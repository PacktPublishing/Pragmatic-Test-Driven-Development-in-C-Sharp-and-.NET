public abstract class Bird
{
    public abstract void Walk();
}
public abstract class FlyingBird : Bird
{
    public abstract void Fly();
}

public class Robin : FlyingBird
{
    public override void Fly() => Console.WriteLine("fly");
    public override void Walk() => Console.WriteLine("walk");
}

public class Ostrich : Bird 
{
    public override void Walk() => Console.WriteLine("walk");
}

