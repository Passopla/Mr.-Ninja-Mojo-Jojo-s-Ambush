using System.Collections;
using UnityEngine;

public class CanonController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartShooting()
    {
        StartCoroutine(ShootRoutine());
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fireRate);
            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        }
    }
}
