using UnityEngine;

public class DamageOnCollide : MonoBehaviour
{
    public int damageDealt = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerControl>() != null) 
        {
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            player.TakeDamage(damageDealt);
        }

        Destroy(gameObject);
    }
}
