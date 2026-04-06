using Godot;

public interface IDamageable
{
    float TakeDamage(float amount);
    void ApplyKnockback(float amount, Vector2 direction);
    void ApplySlowness(float amount);
}