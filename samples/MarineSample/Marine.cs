namespace MarineSample;

public class Marine
{
    public string Name { get; } = "Jim Raynor";
    public int Health { get; set; } = 40;
    public int MaxHealth { get; } = 40;
    public int AttackPower { get; set; } = 6;
    public int Armor { get; set; } = 1;
    public int Kills { get; set; }
    public (int X, int Y) Position { get; set; } = (0, 0);
    public int StimpackCharges { get; private set; } // Mocking a time-based stimpack

    public void ApplyStimpack()
    {
        Health -= 10;
        AttackPower += 3;
        StimpackCharges += 10;
    }

    public void UseStimpackCharge()
    {
        if (StimpackCharges > 0)
        {
            StimpackCharges--;

            if (StimpackCharges == 0)
            {
                AttackPower -= 3;
            }
        }
    }

    public void MoveRandom(Random rng)
    {
        var dx = rng.Next(-3, 4);
        var dy = rng.Next(-3, 4);
        Position = (Position.X + dx, Position.Y + dy);
    }
}
