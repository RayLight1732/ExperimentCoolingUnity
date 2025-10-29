using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CircleArrayModifier : MonoBehaviour
{
    public GameObject prefab;
    public int count = 5;
    public float radius;
    public float degree;
    public float degreeOffset;
    public bool placeEnd;
    private Transform holder;

    void OnValidate()
    {
#if UNITY_EDITOR
        // OnValidate直後に削除・生成する処理を予約
        EditorApplication.delayCall += RebuildArray;
#endif
    }

#if UNITY_EDITOR
    private void RebuildArray()
    {
        if (this == null) return; // Destroyされていたら中止
        if (prefab == null) return;

        // 既存削除
        if (holder != null)
        {
            if (!Application.isPlaying)
                DestroyImmediate(holder.gameObject);
            else
                Destroy(holder.gameObject);

        }

        holder = transform.Find("ArrayHolder")?.transform;
        if (holder != null)
        {
            if (!Application.isPlaying)
                DestroyImmediate(holder.gameObject);
            else
                Destroy(holder.gameObject);

        }

        // 新しいホルダー作成
        holder = new GameObject("ArrayHolder").transform;
        holder.SetParent(transform);
        holder.localPosition = Vector3.zero;
        holder.localRotation = Quaternion.identity;

        // 複製配置
        for (int i = 0; i < count; i++)
        {
            GameObject obj;

            if (!Application.isPlaying)
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, holder);
            else
                obj = Instantiate(prefab, holder);
            int c = count;
            if (placeEnd)
            {
                c = count - 1;
            }
            float rad = Mathf.Deg2Rad * i * degree / c;
            obj.transform.localPosition = new Vector3(radius*Mathf.Cos(rad), 0,radius*Mathf.Sin(rad));
            obj.transform.localRotation = Quaternion.Euler(0,-i*degree/c+degreeOffset,0);
            Debug.Log(rad);
        }
    }
#endif
}