/// <author>Thoams Krahl</author>

using ProjectGTA2_Unity;
using ProjectGTA2_Unity.Weapons;

public interface IDamagable
{
   public void TakeDamage(float damageAmount, DamageType damageType, string character);
}


