/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System;
using System.Collections.Generic;
using UnityEngine;
using XLua;

namespace Main
{
	public partial class LuaInjectionData : MonoBehaviour
	{
		[SerializeField]
		private List<Injection4> m_InjectionList = new List<Injection4>();

		public LuaTable Data
		{
			get
			{
				LuaTable dataTable = LuaMain.LuaEnv.NewTable();
				foreach (Injection4 injection in m_InjectionList)
				{
					if (!string.IsNullOrEmpty(injection.Name))
					{
						object value = injection.Value;
						if (injection.Type == InjectionType.List || injection.Type == InjectionType.Dict)
						{
							value = Injection.ToValueTable(injection.Type, injection.Value);
						}
						dataTable.Set(injection.Name, value);
					}
				}
				return dataTable;
			}
		}
	}
}