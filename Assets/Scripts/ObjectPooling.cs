using UnityEngine;
using System.Collections.Generic;

public class ObjectPooling : MonoBehaviour
{
    public GameObject prefab;  // 풀링할 오브젝트의 원본 프리팹
    public int poolSize = 10;   // 미리 생성해 둘 오브젝트의 개수

		// 풀링된 오브젝트를 저장하는 큐 (FIFO)
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
		    // 풀사이즈만큼 오브젝트를 미리 생성하여 큐에 넣고 모두 비활성화상태로 대기
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }


		// 사용할 오브젝트가 필요할 때 호출
    public GameObject GetFromPool()
    {
        if (pool.Count > 0)
        {
		        // 큐에서 하나 꺼내고 활성화함
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }
        // 풀이 다 썼다면 새로 만들 수도 있음(옵션)
        GameObject newObj = Instantiate(prefab);
        return newObj;
    }

		// 오브젝트의 사용이 끝났을 때 호출
    public void ReturnToPool(GameObject obj)
    {
		    // 오브젝트 비활성화하고 큐에 넣음
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
