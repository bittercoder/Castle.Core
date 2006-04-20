// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Collections;

	using Nullables;

	using Iesi.Collections;

	/// <summary>
	/// Contains utility methods for dealing with ActiveRecord objects
	/// and collections.
	/// Useful for external frameworks.
	/// </summary>
	public abstract class SupportingUtils
	{
		[Obsolete("Use ActiveRecordMediator instead")]
		public static IList FindAll(Type type)
		{
			return ActiveRecordMediator.FindAll(type);
		}

		[Obsolete("Use ActiveRecordMediator instead")]
		public static object FindByPK(Type type, object id)
		{
			return ActiveRecordMediator.FindByPrimaryKey(type, id);
		}

		[Obsolete("Use ActiveRecordMediator instead")]
		public static object FindByPK(Type type, object id, bool throwOnNotFound)
		{
			return ActiveRecordMediator.FindByPrimaryKey(type, id, throwOnNotFound);
		}

		#region BuildArray

		/// <summary>
		/// Converts the results stored in an <see cref="IEnumerable"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="type">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		public static Array BuildArray(Type type, IEnumerable list, bool distinct)
		{
			return BuildArray(type, list, null, distinct);
		}

		/// <summary>
		/// Converts the results stored in an <see cref="IEnumerable"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="type">The type of the new array</param>
		/// <param name="list">The source list</param>
		/// <param name="entityIndex">
		/// If the HQL clause selects more than one field, or a join is performed
		/// without using <c>fetch join</c>, the contents of the result list will
		/// be of type <c>object[]</c>. Specify which index in this array should be used to
		/// compose the new result array.
		/// </param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		public static Array BuildArray(Type type, IEnumerable list, NullableInt32 entityIndex, bool distinct)
		{
			// we only need to perform an additional processing if an
			// entityIndex was specified, or if distinct was chosen.
			if (distinct || entityIndex.HasValue)
			{
				Set set = (distinct ? new ListSet() : null);

				ICollection collection = list as ICollection;
				
				IList newList = collection != null ? new ArrayList(collection.Count) : new ArrayList();
				
				foreach (object item in list)
				{
					object el = (!entityIndex.HasValue ? item : ((object[]) item)[entityIndex.Value]);
					
					if (set == null || set.Add(el))
					{
						newList.Add(el);
					}
				}

				list = newList;
			}

			ICollection col = list as ICollection;
			
			if (col == null)
			{
				ArrayList newList = new ArrayList();
				
				foreach (object item in list)
				{
					newList.Add(item);
				}

				col = newList;
			}

			Array typeSafeArray = Array.CreateInstance(type, col.Count);
			
			col.CopyTo(typeSafeArray, 0);
			
			return typeSafeArray;
		}

		#endregion

		#region BuildObjectArray

		/// <summary>
		/// Converts the results stored in an <see cref="IEnumerable"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="type">
		/// The class of the object which will be created for each row contained in
		/// the supplied <paramref name="list" />.
		/// </param>
		/// <param name="list">The source list</param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		/// <remarks>A good alternative is to use the new <see cref="ImportAttribute"/></remarks>
		public static Array BuildObjectArray(Type type, IEnumerable list, bool distinct)
		{
			// we only need to perform an additional processing if 
			// distinct was chosen.
			Set set = (distinct ? new ListSet() : null);

			ICollection coll = list as ICollection;
			IList newList = coll != null ? new ArrayList(coll.Count) : new ArrayList();
			
			foreach (object item in list)
			{
				object[] p = (item is object[] ? (object[]) item : new object[] {item});
				object el = Activator.CreateInstance(type, p);
				
				if (set == null || set.Add(el))
				{
					newList.Add(el);
				}
			}

			Array a = Array.CreateInstance(type, newList.Count);
			newList.CopyTo(a, 0);
			return a;
		}

		#endregion

#if dotNet2

		#region BuildObjectArray

		/// <summary>
		/// Converts the results stored in an <see cref="IEnumerable"/> to an
		/// strongly-typed array.
		/// </summary>
		/// <param name="list">The source list</param>
		/// <param name="distinct">If true, only distinct results will be inserted in the array</param>
		/// <returns>The strongly-typed array</returns>
		/// <typeparam name="T">
		/// The class of the object which will be created for each row contained in
		/// the supplied <paramref name="list" />.
		/// </param>
		/// <remarks>A good alternative is to use the new <see cref="ImportAttribute"/></remarks>
		public static T[] BuildObjectArray<T>(IEnumerable list, bool distinct)
		{
			return (T[]) BuildObjectArray(typeof(T), list, distinct);
		}
		#endregion

		#region BuildArray
		public static T[] BuildArray<T>(IEnumerable list, bool distinct)
		{
			return (T[]) BuildArray(typeof(T), list, distinct);
		}

		public static T[] BuildArray<T>(IEnumerable list, int? entityIndex, bool distinct)
		{
			return (T[]) BuildArray(typeof(T), list, ConvertNullable(entityIndex), distinct);
		}
		#endregion

		#region Nullable conversions

		/// <summary>
		/// Converts an <see cref="INullableType"/> from the <c>Nullables</c>
		/// library into a .NET 2.0 <see cref="System.Nullable"/>.
		/// </summary>
		public static T? ConvertToDotNet2Nullable<T, U>(U value)
			where T : struct
			where U : struct, INullableType
		{
			T? r = null;
			if (value.HasValue)
				r = (T) value.Value;
			return r;
		}

		/// <summary>
		/// Converts a <see cref="NullableInt32"/> from the <c>Nullables</c>
		/// library into a .NET 2.0 <see cref="System.Nullable"/>.
		/// </summary>
		public static int? ConvertNullable(NullableInt32 value)
		{
			return ConvertToDotNet2Nullable<int, NullableInt32>(value);
		}

		/// <summary>
		/// Converts a .NET 2.0 <see cref="System.Nullable"/> into a
		/// <see cref="NullableInt32"/> from the <c>Nullables</c> library.
		/// </summary>
		public static NullableInt32 ConvertNullable(int? value)
		{
			NullableInt32 r = null;
			if (value.HasValue)
				r = value.Value;
			return r;
		}

		#endregion

#endif
	}
}