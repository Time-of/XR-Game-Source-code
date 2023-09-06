using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/**
 * @author 이성수
 * @brief 구매 가능한 아이템 인터페이스입니다.
 * @since 2023-03-26
 */
public interface IPurchaseable
{
	public abstract uint GetPrice();
}
