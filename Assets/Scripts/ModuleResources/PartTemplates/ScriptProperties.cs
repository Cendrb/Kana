namespace Assets.Scripts.ModuleResources.PartTemplates
{
    public class ScriptProperties
    {
        public float Mass { get; private set; }
        public int Health { get; private set; }
        public int DamageOnTouch { get; private set; }

        public ScriptProperties(float mass, int health, int damageOnTouch)
        {
            Mass = mass;
            Health = health;
            DamageOnTouch = damageOnTouch;
        }
    }
}
