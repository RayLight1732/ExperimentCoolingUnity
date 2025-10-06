using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class ArrayModifier : MonoBehaviour
{
    public GameObject prefab;
    public int count = 5;
    public Vector3 offset = new Vector3(2, 0, 0);

    private Transform holder;

    void OnValidate()
    {
#if UNITY_EDITOR
        // OnValidate直後に削除・生成する処理を予約
        EditorApplication.delayCall += RebuildArray;
#endif
    }

    private void RebuildArray()
    {
        if (this == null) return; // Destroyされていたら中止
        if (prefab == null) return;

        // 既存削除
        if (holder != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(holder.gameObject);
            else
                Destroy(holder.gameObject);
#else
            Destroy(holder.gameObject);
#endif
        }

        holder = transform.Find("ArrayHolder")?.transform;
        if (holder != null)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                DestroyImmediate(holder.gameObject);
            else
                Destroy(holder.gameObject);
#else
            Destroy(holder.gameObject);
#endif
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
#if UNITY_EDITOR
            if (!Application.isPlaying)
                obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab, holder);
            else
                obj = Instantiate(prefab, holder);
#else
            obj = Instantiate(prefab, holder);
#endif
            obj.transform.localPosition = offset * i;
        }
    }
}