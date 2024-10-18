namespace com.nemodouble.massiveGunner.Scripts
{
    public interface IBurnable
    {
        public void StartBurning(float percentDamage, float duration, float burnSpeed = 1f);
    }
}