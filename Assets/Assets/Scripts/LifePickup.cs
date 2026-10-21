using UnityEngine;

public class LifePickup : MonoBehaviour
{
    public int lifeAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.GetComponent<PlayerControl>() != null)
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            player.AddLife(lifeAmount);
            Destroy(gameObject);
        }
    }
}
