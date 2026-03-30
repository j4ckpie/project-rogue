using Godot;

public interface IDamageable
{
    void TakeDamage(float amount);
    void ApplyKnockback(float amount, Vector2 direction);
    void ApplySlowness(float amount);
}