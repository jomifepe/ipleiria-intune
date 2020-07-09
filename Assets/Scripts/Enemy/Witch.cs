namespace Enemy
{
    public class Witch : RangedEnemy
    {
        protected override void Init()
        {
            life = maxHealth = 2f;
            sensingRange = attackRange;
        }
    }
}

 