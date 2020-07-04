
public class Witch : RangedEnemy
{
    protected override void Init()
    {
        life = maxHealth = 3f;
        sensingRange = attackRange;
    }
}

 