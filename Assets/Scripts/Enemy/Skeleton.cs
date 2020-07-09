
namespace Enemy
{
    public class Skeleton: MeleeEnemy
    {
        protected override void Init()
        {
            life = maxHealth = 3f;
            sensingRange = 10f;
        }
    }
}
