using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 배치 미리보기 홀로그램 클래스입니다.
 * @since 2023-03-26
 */
public class PlacePreviewHologram : PoolableObject
{
	[SerializeField, ReadOnlyProperty]
	private List<Collider> OverlappedColliders = new List<Collider>();

	public bool bIsPlaceable { get; private set; }

	[SerializeField]
	private Material GreenMaterial;

	[SerializeField]
	private Material RedMaterial;

	private Renderer[] PreviewObjectRenderers;

	/// <summary>
	/// 실제로 배치할 오브젝트 프리팹
	/// </summary>
	public PoolableObject ObjectPrefabToPlace;



	private void Awake()
	{
		PreviewObjectRenderers = gameObject.GetComponentsInChildren<Renderer>();
	}



	public void PlaceObject()
	{
		GameplayHelperLibrary.SpawnObject(ObjectPrefabToPlace.InternalName, transform.position, transform.rotation);
		ReturnToPool();
	}



	public override void OnPreSpawnedFromPool()
	{
		base.OnPreSpawnedFromPool();

		bIsPlaceable = false;
		OverlappedColliders.Clear();
	}



	public override void OnPostSpawnedFromPool()
	{
		UpdateIsPlaceable();
	}



	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 1 << LayerMask.NameToLayer("Default")) return;

		OverlappedColliders.Add(other);

		if (other.gameObject.CompareTag("Friendly") || other.gameObject.CompareTag("Hostile"))
		{
			PoolableObject Poolable = other.gameObject.GetComponent<PoolableObject>();
			if (Poolable != null)
			{
				Poolable.OnReturnedToPoolEvent.AddListener(OnOverlappedObjectReturnedToPoolTriggerExitHandler);
			}
		}

		UpdateIsPlaceable();
	}



	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 1 << LayerMask.NameToLayer("Default")) return;

		OverlappedColliders.Remove(other);

		if (other.gameObject.CompareTag("Friendly") || other.gameObject.CompareTag("Hostile"))
		{
			PoolableObject Poolable = other.gameObject.GetComponent<PoolableObject>();
			if (Poolable != null)
			{
				Poolable.OnReturnedToPoolEvent.RemoveListener(OnOverlappedObjectReturnedToPoolTriggerExitHandler);
			}
		}
		
		UpdateIsPlaceable();
	}



	/// <summary>
	/// 오버랩된 풀링 가능한 오브젝트들이 풀로 돌아갈 때 명시적으로 OnTriggerExit를 발생시키도록 하는 기능
	/// </summary>
	/// <param name="Poolable"></param>
	private void OnOverlappedObjectReturnedToPoolTriggerExitHandler(PoolableObject Poolable)
	{
		OnTriggerExit(Poolable.GetComponent<Collider>());
	}



	private void UpdateIsPlaceable()
	{
		bIsPlaceable = OverlappedColliders.Count <= 0;

		foreach(Renderer renderer in PreviewObjectRenderers)
		{
			renderer.material = bIsPlaceable ? GreenMaterial : RedMaterial;
		}
	}
}
