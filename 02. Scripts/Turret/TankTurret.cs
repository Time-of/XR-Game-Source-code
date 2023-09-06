using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 탱크 터렛
 * @since 2023-03-17
 */
public class TankTurret : TurretBase
{
	public Rigidbody RigidComp;



	private void Start()
	{
		RigidComp = GetComponent<Rigidbody>();
	}
}
