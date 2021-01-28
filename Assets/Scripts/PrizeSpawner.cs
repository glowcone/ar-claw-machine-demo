using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PrizeSpawner : MonoBehaviour
{
    [SerializeField] Transform spawnPoint;
    [SerializeField] private float spawnRadius;
    [SerializeField] private int spawnAmount;
    [SerializeField] private int minPrizes;
    [SerializeField] private GameObject[] prizes;
    [SerializeField] private GameObject spawnVfx;
    [SerializeField] private float spawnCooldown;

    public List<GameObject> prizeInstances = new List<GameObject>();
    private void Start()
    {
        Spawn();
        Claw.Instance.OnGetPrize += RemovePrizeInstance;
    }

    public void Spawn()
    {
        StartCoroutine(DoSpawnCoroutine());
    }

    IEnumerator DoSpawnCoroutine()
    {
        var existing = prizeInstances.Count;
        for (int i = 0; i < spawnAmount - existing; i++)
        {
            var prize = prizes[Random.Range(0, prizes.Length)];
            var point = Random.insideUnitCircle * spawnRadius + new Vector2(spawnPoint.position.x, spawnPoint.position.y);
            var rotation = Random.Range(0, 360);
            Instantiate(spawnVfx, point, Quaternion.identity);
            var inst = Instantiate(prize, point, Quaternion.AngleAxis(rotation, Vector3.forward));
            prizeInstances.Add(inst);
            yield return new WaitForSeconds(spawnCooldown);
        }
    }

    public void RemovePrizeInstance(GameObject prize)
    {
        prizeInstances.Remove(prize);
        if (prizeInstances.Count < minPrizes)
            Spawn();
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(spawnPoint.position, spawnRadius);
    }
}