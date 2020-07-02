
public class Witch : Enemy
{
    private float maxHealth = 3f;
    private int minCoinDrop = 10;
    private int maxCoinDrop = 15;
    private int minCoinCount = 1;
    private int maxCoinCount = 2;

    protected override float getMaxHealth() => maxHealth;
    protected override int getMinCoinDrop() => minCoinDrop;
    protected override int getMaxCoinDrop() => maxCoinDrop;
    protected override int getMinCoinCount() => minCoinCount;
    protected override int getMaxCoinCount() => maxCoinCount;
}

