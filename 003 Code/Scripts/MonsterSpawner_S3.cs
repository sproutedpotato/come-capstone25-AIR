using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner_S3 : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Monsters;

    [SerializeField] private Transform arCameraTransform;
    private int totalCount, spawnCount, index;


    // Start is called before the first frame update
    void Start()
    {
        totalCount = 20;
        spawnCount = 0;
        Spawn();
    }

    private void Spawn()
    {
        StartCoroutine(MonsterSpawnRoutine());
    }

    IEnumerator MonsterSpawnRoutine(){
        float time = 3f;
        yield return new WaitForSeconds(3);
        for (int i = 0; i < totalCount; i++){
            spawnCount++;
            if (i == 0)
                SpawnMonsterInView(5f);
            else
            {
                if (spawnCount == 9)
                {
                    SpawnHydra();
                }
                else if(spawnCount == 19)
                {
                    SpawnDragon();
                }
                else
                {
                    int rand = Random.Range(0, 2);
                    if(rand == 0)
                    {
                        SpawnLizardWarrior();
                    }
                    else
                    {
                        SpawnDragonide();
                    }
                }
            }
            yield return new WaitForSeconds(time);
        }
    }

    private void SpawnMonster(int index, float minDistance = 10f, float maxDistance = 20f)
    {
        Vector3 playerPos = Camera.main.transform.position;

        Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(minDistance, maxDistance);
        Vector3 spawnPos = new Vector3(playerPos.x + circle.x, -1f, playerPos.z + circle.y);

        Instantiate(Monsters[index], spawnPos, Quaternion.identity);
    }

    private void SpawnMonsterInView(float distance)
    {
        if (arCameraTransform == null)
            arCameraTransform = Camera.main.transform;

        float vx = Random.Range(0.3f, 0.7f);
        float vy = Random.Range(0.3f, 0.7f);

        Vector3 viewPos = new Vector3(vx, vy, distance);
        Vector3 worldPos = Camera.main.ViewportToWorldPoint(viewPos);

        worldPos.y = -1f;

        Instantiate(Monsters[0], worldPos, Quaternion.identity);
    }

    private void SpawnDragonide()
    {
        SpawnMonster(0);
    }

    private void SpawnHydra()
    {
        SpawnMonster(1);
    }

    private void SpawnLizardWarrior()
    {
        SpawnMonster(2);
    }

    private void SpawnDragon()
    {
        SpawnMonster(3);
    }
}
