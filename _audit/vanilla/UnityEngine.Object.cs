using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using UnityEngine.Bindings;
using UnityEngine.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using UnityEngineInternal;

namespace UnityEngine
{
	// Token: 0x02000267 RID: 615
	[NativeHeader("Runtime/GameCode/CloneObject.h")]
	[RequiredByNativeCode(GenerateProxy = true)]
	[NativeHeader("Runtime/Export/Scripting/UnityEngineObject.bindings.h")]
	[NativeHeader("Runtime/SceneManager/SceneManager.h")]
	[StructLayout(LayoutKind.Sequential)]
	public class Object
	{
		// Token: 0x060019A2 RID: 6562 RVA: 0x0002B214 File Offset: 0x00029414
		[SecuritySafeCritical]
		public unsafe int GetInstanceID()
		{
			bool flag = this.m_CachedPtr == IntPtr.Zero;
			int num;
			if (flag)
			{
				num = 0;
			}
			else
			{
				bool flag2 = Object.OffsetOfInstanceIDInCPlusPlusObject == -1;
				if (flag2)
				{
					Object.OffsetOfInstanceIDInCPlusPlusObject = Object.GetOffsetOfInstanceIDInCPlusPlusObject();
				}
				num = *(int*)(void*)new IntPtr(this.m_CachedPtr.ToInt64() + (long)Object.OffsetOfInstanceIDInCPlusPlusObject);
			}
			return num;
		}

		// Token: 0x060019A3 RID: 6563 RVA: 0x0002B274 File Offset: 0x00029474
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x060019A4 RID: 6564 RVA: 0x0002B28C File Offset: 0x0002948C
		public override bool Equals(object other)
		{
			Object @object = other as Object;
			bool flag = @object == null && other != null && !(other is Object);
			return !flag && Object.CompareBaseObjects(this, @object);
		}

		// Token: 0x060019A5 RID: 6565 RVA: 0x0002B2D0 File Offset: 0x000294D0
		public static implicit operator bool(Object exists)
		{
			return !Object.CompareBaseObjects(exists, null);
		}

		// Token: 0x060019A6 RID: 6566 RVA: 0x0002B2EC File Offset: 0x000294EC
		private static bool CompareBaseObjects(Object lhs, Object rhs)
		{
			bool flag = lhs == null;
			bool flag2 = rhs == null;
			bool flag3 = flag2 && flag;
			bool flag4;
			if (flag3)
			{
				flag4 = true;
			}
			else
			{
				bool flag5 = flag2;
				if (flag5)
				{
					flag4 = !Object.IsNativeObjectAlive(lhs);
				}
				else
				{
					bool flag6 = flag;
					if (flag6)
					{
						flag4 = !Object.IsNativeObjectAlive(rhs);
					}
					else
					{
						flag4 = lhs == rhs;
					}
				}
			}
			return flag4;
		}

		// Token: 0x060019A7 RID: 6567 RVA: 0x0002B340 File Offset: 0x00029540
		private void EnsureRunningOnMainThread()
		{
			bool flag = !Object.CurrentThreadIsMainThread();
			if (flag)
			{
				throw new InvalidOperationException("EnsureRunningOnMainThread can only be called from the main thread");
			}
		}

		// Token: 0x060019A8 RID: 6568 RVA: 0x0002B368 File Offset: 0x00029568
		private static bool IsNativeObjectAlive(Object o)
		{
			return o.GetCachedPtr() != IntPtr.Zero;
		}

		// Token: 0x060019A9 RID: 6569 RVA: 0x0002B38C File Offset: 0x0002958C
		private IntPtr GetCachedPtr()
		{
			return this.m_CachedPtr;
		}

		// Token: 0x170004E1 RID: 1249
		// (get) Token: 0x060019AA RID: 6570 RVA: 0x0002B3A4 File Offset: 0x000295A4
		// (set) Token: 0x060019AB RID: 6571 RVA: 0x0002B3BC File Offset: 0x000295BC
		public string name
		{
			get
			{
				return Object.GetName(this);
			}
			set
			{
				Object.SetName(this, value);
			}
		}

		// Token: 0x060019AC RID: 6572 RVA: 0x0002B3C8 File Offset: 0x000295C8
		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, null, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty);
		}

		// Token: 0x060019AD RID: 6573 RVA: 0x0002B3EC File Offset: 0x000295EC
		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, parent, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty);
		}

		// Token: 0x060019AE RID: 6574 RVA: 0x0002B410 File Offset: 0x00029610
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, null, new ReadOnlySpan<Vector3>((void*)(&position), 1), new ReadOnlySpan<Quaternion>((void*)(&rotation), 1));
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0002B440 File Offset: 0x00029640
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, Transform parent, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, 1, parent, new ReadOnlySpan<Vector3>((void*)(&position), 1), new ReadOnlySpan<Quaternion>((void*)(&rotation), 1));
		}

		// Token: 0x060019B0 RID: 6576 RVA: 0x0002B470 File Offset: 0x00029670
		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, null, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty);
		}

		// Token: 0x060019B1 RID: 6577 RVA: 0x0002B494 File Offset: 0x00029694
		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, parent, ReadOnlySpan<Vector3>.Empty, ReadOnlySpan<Quaternion>.Empty);
		}

		// Token: 0x060019B2 RID: 6578 RVA: 0x0002B4B8 File Offset: 0x000296B8
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, null, new ReadOnlySpan<Vector3>((void*)(&position), 1), new ReadOnlySpan<Quaternion>((void*)(&rotation), 1));
		}

		// Token: 0x060019B3 RID: 6579 RVA: 0x0002B4E8 File Offset: 0x000296E8
		public static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, null, positions, rotations);
		}

		// Token: 0x060019B4 RID: 6580 RVA: 0x0002B504 File Offset: 0x00029704
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, Vector3 position, Quaternion rotation) where T : Object
		{
			return Object.InstantiateAsync<T>(original, count, parent, new ReadOnlySpan<Vector3>((void*)(&position), 1), new ReadOnlySpan<Quaternion>((void*)(&rotation), 1));
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x0002B534 File Offset: 0x00029734
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static AsyncInstantiateOperation<T> InstantiateAsync<T>(T original, int count, Transform parent, ReadOnlySpan<Vector3> positions, ReadOnlySpan<Quaternion> rotations) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			bool flag = count <= 0;
			if (flag)
			{
				throw new ArgumentException("Cannot call instantiate multiple with count less or equal to zero");
			}
			fixed (Vector3* pinnableReference = positions.GetPinnableReference())
			{
				Vector3* ptr = pinnableReference;
				fixed (Quaternion* pinnableReference2 = rotations.GetPinnableReference())
				{
					Quaternion* ptr2 = pinnableReference2;
					AsyncInstantiateOperation asyncInstantiateOperation = Object.Internal_InstantiateAsyncWithParent(original, count, parent, (IntPtr)((void*)ptr), positions.Length, (IntPtr)((void*)ptr2), rotations.Length);
					return new AsyncInstantiateOperation<T>(asyncInstantiateOperation);
				}
			}
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x0002B5BC File Offset: 0x000297BC
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			bool flag = original is ScriptableObject;
			if (flag)
			{
				throw new ArgumentException("Cannot instantiate a ScriptableObject with a position and rotation");
			}
			Object @object = Object.Internal_InstantiateSingle(original, position, rotation);
			bool flag2 = @object == null;
			if (flag2)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x0002B614 File Offset: 0x00029814
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Vector3 position, Quaternion rotation, Transform parent)
		{
			bool flag = parent == null;
			Object @object;
			if (flag)
			{
				@object = Object.Instantiate(original, position, rotation);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_InstantiateSingleWithParent(original, parent, position, rotation);
				bool flag2 = object2 == null;
				if (flag2)
				{
					throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
				}
				@object = object2;
			}
			return @object;
		}

		// Token: 0x060019B8 RID: 6584 RVA: 0x0002B66C File Offset: 0x0002986C
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			Object @object = Object.Internal_CloneSingle(original);
			bool flag = @object == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0002B6A8 File Offset: 0x000298A8
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Scene scene)
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			Object @object = Object.Internal_CloneSingleWithScene(original, scene);
			bool flag = @object == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return @object;
		}

		// Token: 0x060019BA RID: 6586 RVA: 0x0002B6E8 File Offset: 0x000298E8
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent)
		{
			return Object.Instantiate(original, parent, false);
		}

		// Token: 0x060019BB RID: 6587 RVA: 0x0002B704 File Offset: 0x00029904
		[TypeInferenceRule(TypeInferenceRules.TypeOfFirstArgument)]
		public static Object Instantiate(Object original, Transform parent, bool instantiateInWorldSpace)
		{
			bool flag = parent == null;
			Object @object;
			if (flag)
			{
				@object = Object.Instantiate(original);
			}
			else
			{
				Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
				Object object2 = Object.Internal_CloneSingleWithParent(original, parent, instantiateInWorldSpace);
				bool flag2 = object2 == null;
				if (flag2)
				{
					throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
				}
				@object = object2;
			}
			return @object;
		}

		// Token: 0x060019BC RID: 6588 RVA: 0x0002B758 File Offset: 0x00029958
		public static T Instantiate<T>(T original) where T : Object
		{
			Object.CheckNullArgument(original, "The Object you want to instantiate is null.");
			T t = (T)((object)Object.Internal_CloneSingle(original));
			bool flag = t == null;
			if (flag)
			{
				throw new UnityException("Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.");
			}
			return t;
		}

		// Token: 0x060019BD RID: 6589 RVA: 0x0002B7A8 File Offset: 0x000299A8
		public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation) where T : Object
		{
			return (T)((object)Object.Instantiate(original, position, rotation));
		}

		// Token: 0x060019BE RID: 6590 RVA: 0x0002B7CC File Offset: 0x000299CC
		public static T Instantiate<T>(T original, Vector3 position, Quaternion rotation, Transform parent) where T : Object
		{
			return (T)((object)Object.Instantiate(original, position, rotation, parent));
		}

		// Token: 0x060019BF RID: 6591 RVA: 0x0002B7F4 File Offset: 0x000299F4
		public static T Instantiate<T>(T original, Transform parent) where T : Object
		{
			return Object.Instantiate<T>(original, parent, false);
		}

		// Token: 0x060019C0 RID: 6592 RVA: 0x0002B810 File Offset: 0x00029A10
		public static T Instantiate<T>(T original, Transform parent, bool worldPositionStays) where T : Object
		{
			return (T)((object)Object.Instantiate(original, parent, worldPositionStays));
		}

		// Token: 0x060019C1 RID: 6593
		[NativeMethod(Name = "Scripting::DestroyObjectFromScripting", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void Destroy(Object obj, [DefaultValue("0.0F")] float t);

		// Token: 0x060019C2 RID: 6594 RVA: 0x0002B834 File Offset: 0x00029A34
		[ExcludeFromDocs]
		public static void Destroy(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		// Token: 0x060019C3 RID: 6595
		[NativeMethod(Name = "Scripting::DestroyObjectFromScriptingImmediate", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DestroyImmediate(Object obj, [DefaultValue("false")] bool allowDestroyingAssets);

		// Token: 0x060019C4 RID: 6596 RVA: 0x0002B850 File Offset: 0x00029A50
		[ExcludeFromDocs]
		public static void DestroyImmediate(Object obj)
		{
			bool flag = false;
			Object.DestroyImmediate(obj, flag);
		}

		// Token: 0x060019C5 RID: 6597 RVA: 0x0002B868 File Offset: 0x00029A68
		public static Object[] FindObjectsOfType(Type type)
		{
			return Object.FindObjectsOfType(type, false);
		}

		// Token: 0x060019C6 RID: 6598
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfType")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfType(Type type, bool includeInactive);

		// Token: 0x060019C7 RID: 6599 RVA: 0x0002B884 File Offset: 0x00029A84
		public static Object[] FindObjectsByType(Type type, FindObjectsSortMode sortMode)
		{
			return Object.FindObjectsByType(type, FindObjectsInactive.Exclude, sortMode);
		}

		// Token: 0x060019C8 RID: 6600
		[FreeFunction("UnityEngineObjectBindings::FindObjectsByType")]
		[TypeInferenceRule(TypeInferenceRules.ArrayOfTypeReferencedByFirstArgument)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsByType(Type type, FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode);

		// Token: 0x060019C9 RID: 6601
		[FreeFunction("GetSceneManager().DontDestroyOnLoad", ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern void DontDestroyOnLoad([NotNull("NullExceptionObject")] Object target);

		// Token: 0x170004E2 RID: 1250
		// (get) Token: 0x060019CA RID: 6602
		// (set) Token: 0x060019CB RID: 6603
		public extern HideFlags hideFlags
		{
			[MethodImpl(MethodImplOptions.InternalCall)]
			get;
			[MethodImpl(MethodImplOptions.InternalCall)]
			set;
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x0002B89E File Offset: 0x00029A9E
		[Obsolete("use Object.Destroy instead.")]
		public static void DestroyObject(Object obj, [DefaultValue("0.0F")] float t)
		{
			Object.Destroy(obj, t);
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x0002B8AC File Offset: 0x00029AAC
		[ExcludeFromDocs]
		[Obsolete("use Object.Destroy instead.")]
		public static void DestroyObject(Object obj)
		{
			float num = 0f;
			Object.Destroy(obj, num);
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x0002B8C8 File Offset: 0x00029AC8
		[Obsolete("warning use Object.FindObjectsByType instead.")]
		public static Object[] FindSceneObjectsOfType(Type type)
		{
			return Object.FindObjectsOfType(type);
		}

		// Token: 0x060019CF RID: 6607
		[FreeFunction("UnityEngineObjectBindings::FindObjectsOfTypeIncludingAssets")]
		[Obsolete("use Resources.FindObjectsOfTypeAll instead.")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		public static extern Object[] FindObjectsOfTypeIncludingAssets(Type type);

		// Token: 0x060019D0 RID: 6608 RVA: 0x0002B8E0 File Offset: 0x00029AE0
		public static T[] FindObjectsOfType<T>() where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T), false));
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x0002B908 File Offset: 0x00029B08
		public static T[] FindObjectsByType<T>(FindObjectsSortMode sortMode) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof(T), FindObjectsInactive.Exclude, sortMode));
		}

		// Token: 0x060019D2 RID: 6610 RVA: 0x0002B930 File Offset: 0x00029B30
		public static T[] FindObjectsOfType<T>(bool includeInactive) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsOfType(typeof(T), includeInactive));
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x0002B958 File Offset: 0x00029B58
		public static T[] FindObjectsByType<T>(FindObjectsInactive findObjectsInactive, FindObjectsSortMode sortMode) where T : Object
		{
			return Resources.ConvertObjects<T>(Object.FindObjectsByType(typeof(T), findObjectsInactive, sortMode));
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x0002B980 File Offset: 0x00029B80
		public static T FindObjectOfType<T>() where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T), false));
		}

		// Token: 0x060019D5 RID: 6613 RVA: 0x0002B9A8 File Offset: 0x00029BA8
		public static T FindObjectOfType<T>(bool includeInactive) where T : Object
		{
			return (T)((object)Object.FindObjectOfType(typeof(T), includeInactive));
		}

		// Token: 0x060019D6 RID: 6614 RVA: 0x0002B9D0 File Offset: 0x00029BD0
		public static T FindFirstObjectByType<T>() where T : Object
		{
			return (T)((object)Object.FindFirstObjectByType(typeof(T), FindObjectsInactive.Exclude));
		}

		// Token: 0x060019D7 RID: 6615 RVA: 0x0002B9F8 File Offset: 0x00029BF8
		public static T FindAnyObjectByType<T>() where T : Object
		{
			return (T)((object)Object.FindAnyObjectByType(typeof(T), FindObjectsInactive.Exclude));
		}

		// Token: 0x060019D8 RID: 6616 RVA: 0x0002BA20 File Offset: 0x00029C20
		public static T FindFirstObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
		{
			return (T)((object)Object.FindFirstObjectByType(typeof(T), findObjectsInactive));
		}

		// Token: 0x060019D9 RID: 6617 RVA: 0x0002BA48 File Offset: 0x00029C48
		public static T FindAnyObjectByType<T>(FindObjectsInactive findObjectsInactive) where T : Object
		{
			return (T)((object)Object.FindAnyObjectByType(typeof(T), findObjectsInactive));
		}

		// Token: 0x060019DA RID: 6618 RVA: 0x0002BA70 File Offset: 0x00029C70
		[Obsolete("Please use Resources.FindObjectsOfTypeAll instead")]
		public static Object[] FindObjectsOfTypeAll(Type type)
		{
			return Resources.FindObjectsOfTypeAll(type);
		}

		// Token: 0x060019DB RID: 6619 RVA: 0x0002BA88 File Offset: 0x00029C88
		private static void CheckNullArgument(object arg, string message)
		{
			bool flag = arg == null;
			if (flag)
			{
				throw new ArgumentException(message);
			}
		}

		// Token: 0x060019DC RID: 6620 RVA: 0x0002BAA8 File Offset: 0x00029CA8
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public static Object FindObjectOfType(Type type)
		{
			Object[] array = Object.FindObjectsOfType(type, false);
			bool flag = array.Length != 0;
			Object @object;
			if (flag)
			{
				@object = array[0];
			}
			else
			{
				@object = null;
			}
			return @object;
		}

		// Token: 0x060019DD RID: 6621 RVA: 0x0002BAD4 File Offset: 0x00029CD4
		public static Object FindFirstObjectByType(Type type)
		{
			Object[] array = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
			return (array.Length != 0) ? array[0] : null;
		}

		// Token: 0x060019DE RID: 6622 RVA: 0x0002BAFC File Offset: 0x00029CFC
		public static Object FindAnyObjectByType(Type type)
		{
			Object[] array = Object.FindObjectsByType(type, FindObjectsInactive.Exclude, FindObjectsSortMode.None);
			return (array.Length != 0) ? array[0] : null;
		}

		// Token: 0x060019DF RID: 6623 RVA: 0x0002BB24 File Offset: 0x00029D24
		[TypeInferenceRule(TypeInferenceRules.TypeReferencedByFirstArgument)]
		public static Object FindObjectOfType(Type type, bool includeInactive)
		{
			Object[] array = Object.FindObjectsOfType(type, includeInactive);
			bool flag = array.Length != 0;
			Object @object;
			if (flag)
			{
				@object = array[0];
			}
			else
			{
				@object = null;
			}
			return @object;
		}

		// Token: 0x060019E0 RID: 6624 RVA: 0x0002BB50 File Offset: 0x00029D50
		public static Object FindFirstObjectByType(Type type, FindObjectsInactive findObjectsInactive)
		{
			Object[] array = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.InstanceID);
			return (array.Length != 0) ? array[0] : null;
		}

		// Token: 0x060019E1 RID: 6625 RVA: 0x0002BB78 File Offset: 0x00029D78
		public static Object FindAnyObjectByType(Type type, FindObjectsInactive findObjectsInactive)
		{
			Object[] array = Object.FindObjectsByType(type, findObjectsInactive, FindObjectsSortMode.None);
			return (array.Length != 0) ? array[0] : null;
		}

		// Token: 0x060019E2 RID: 6626 RVA: 0x0002BBA0 File Offset: 0x00029DA0
		public override string ToString()
		{
			return Object.ToString(this);
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x0002BBB8 File Offset: 0x00029DB8
		public static bool operator ==(Object x, Object y)
		{
			return Object.CompareBaseObjects(x, y);
		}

		// Token: 0x060019E4 RID: 6628 RVA: 0x0002BBD4 File Offset: 0x00029DD4
		public static bool operator !=(Object x, Object y)
		{
			return !Object.CompareBaseObjects(x, y);
		}

		// Token: 0x060019E5 RID: 6629
		[NativeMethod(Name = "Object::GetOffsetOfInstanceIdMember", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern int GetOffsetOfInstanceIDInCPlusPlusObject();

		// Token: 0x060019E6 RID: 6630
		[NativeMethod(Name = "CurrentThreadIsMainThread", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern bool CurrentThreadIsMainThread();

		// Token: 0x060019E7 RID: 6631
		[NativeMethod(Name = "CloneObject", IsFreeFunction = true, ThrowsException = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingle([NotNull("NullExceptionObject")] Object data);

		// Token: 0x060019E8 RID: 6632 RVA: 0x0002BBF0 File Offset: 0x00029DF0
		[FreeFunction("CloneObjectToScene")]
		private static Object Internal_CloneSingleWithScene([NotNull("ArgumentNullException")] Object data, Scene scene)
		{
			return Object.Internal_CloneSingleWithScene_Injected(data, ref scene);
		}

		// Token: 0x060019E9 RID: 6633
		[FreeFunction("CloneObject")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingleWithParent([NotNull("NullExceptionObject")] Object data, [NotNull("NullExceptionObject")] Transform parent, bool worldPositionStays);

		// Token: 0x060019EA RID: 6634
		[FreeFunction("InstantiateAsyncObjects")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern AsyncInstantiateOperation Internal_InstantiateAsyncWithParent([NotNull("NullExceptionObject")] Object original, int count, Transform parent, IntPtr positions, int positionsCount, IntPtr rotations, int rotationsCount);

		// Token: 0x060019EB RID: 6635 RVA: 0x0002BBFA File Offset: 0x00029DFA
		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingle([NotNull("NullExceptionObject")] Object data, Vector3 pos, Quaternion rot)
		{
			return Object.Internal_InstantiateSingle_Injected(data, ref pos, ref rot);
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x0002BC06 File Offset: 0x00029E06
		[FreeFunction("InstantiateObject")]
		private static Object Internal_InstantiateSingleWithParent([NotNull("NullExceptionObject")] Object data, [NotNull("NullExceptionObject")] Transform parent, Vector3 pos, Quaternion rot)
		{
			return Object.Internal_InstantiateSingleWithParent_Injected(data, parent, ref pos, ref rot);
		}

		// Token: 0x060019ED RID: 6637
		[FreeFunction("UnityEngineObjectBindings::ToString")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string ToString(Object obj);

		// Token: 0x060019EE RID: 6638
		[FreeFunction("UnityEngineObjectBindings::GetName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern string GetName([NotNull("NullExceptionObject")] Object obj);

		// Token: 0x060019EF RID: 6639
		[FreeFunction("UnityEngineObjectBindings::IsPersistent")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsPersistent([NotNull("NullExceptionObject")] Object obj);

		// Token: 0x060019F0 RID: 6640
		[FreeFunction("UnityEngineObjectBindings::SetName")]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void SetName([NotNull("NullExceptionObject")] Object obj, string name);

		// Token: 0x060019F1 RID: 6641
		[NativeMethod(Name = "UnityEngineObjectBindings::DoesObjectWithInstanceIDExist", IsFreeFunction = true, IsThreadSafe = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool DoesObjectWithInstanceIDExist(int instanceID);

		// Token: 0x060019F2 RID: 6642
		[FreeFunction("UnityEngineObjectBindings::FindObjectFromInstanceID")]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object FindObjectFromInstanceID(int instanceID);

		// Token: 0x060019F3 RID: 6643
		[FreeFunction("UnityEngineObjectBindings::ForceLoadFromInstanceID")]
		[VisibleToOtherModules]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern Object ForceLoadFromInstanceID(int instanceID);

		// Token: 0x060019F4 RID: 6644
		[FreeFunction("UnityEngineObjectBindings::MarkObjectDirty", HasExplicitThis = true)]
		[MethodImpl(MethodImplOptions.InternalCall)]
		internal extern void MarkDirty();

		// Token: 0x060019F7 RID: 6647
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_CloneSingleWithScene_Injected(Object data, ref Scene scene);

		// Token: 0x060019F8 RID: 6648
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_InstantiateSingle_Injected(Object data, ref Vector3 pos, ref Quaternion rot);

		// Token: 0x060019F9 RID: 6649
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Object Internal_InstantiateSingleWithParent_Injected(Object data, Transform parent, ref Vector3 pos, ref Quaternion rot);

		// Token: 0x040008F9 RID: 2297
		private IntPtr m_CachedPtr;

		// Token: 0x040008FA RID: 2298
		internal static int OffsetOfInstanceIDInCPlusPlusObject = -1;

		// Token: 0x040008FB RID: 2299
		private const string objectIsNullMessage = "The Object you want to instantiate is null.";

		// Token: 0x040008FC RID: 2300
		private const string cloneDestroyedMessage = "Instantiate failed because the clone was destroyed during creation. This can happen if DestroyImmediate is called in MonoBehaviour.Awake.";
	}
}
