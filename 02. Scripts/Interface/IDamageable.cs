using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author �̼���
 * @brief ���ظ� ���� �� �ִ� �������̽� Ŭ�����Դϴ�.
 * @since 2023-03-12
 */
public interface IDamageable
{
	/// <summary>
	/// ���ظ� �޽��ϴ�.
	/// </summary>
	/// <param name="Damage">���ط�</param>
	/// <param name="Instigator">���ظ� �� ��ü (����ź�� ���, ����ź�� ���� ��ü)</param>
	/// <param name="DamageCauser">���ظ� �� �������� ��� (����ź�� ���, ����ź)</param>
	public abstract void TakeDamage(float Damage, GameObject Instigator, GameObject DamageCauser);
}
