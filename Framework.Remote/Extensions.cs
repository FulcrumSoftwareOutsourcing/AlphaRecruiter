/********************************************************************
 *  FulcrumWeb RAD Framework - Fulcrum of your business             *
 *  Copyright (c) 2002-2009 FulcrumWeb, ALL RIGHTS RESERVED         *
 *                                                                  *
 *  THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      *
 *  FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        *
 *  COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       *
 *  AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  *
 *  AND PERMISSION FROM FULCRUMWEB. CONSULT THE END USER LICENSE    *
 *  AGREEMENT FOR INFORMATION ON ADDITIONAL RESTRICTIONS.           *
 ********************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Framework.Remote
{
	public static class Extensions
	{
		public static T2[] ToArray<T1, T2>( this ICollection<T1> coll, Func<T1, T2> convert )
		{
			var ary = new T2[coll.Count];
			var idx = 0;
			foreach ( var item in coll )
				ary[idx++] = convert( item );
			return ary;
		}



		public static T2[] ToArray<T1, T2>( this IEnumerable<T1> seq, Func<T1, T2> convert )
		{
			return seq.ToLinkedList( convert ).ToArray();
		}

		public static IEnumerable<U> MapLazy<T, U>( this IEnumerable<T> seq, Func<T, U> convert )
		{
			foreach ( var x in seq )
				yield return convert( x );
		}

		public static IEnumerable<T> MapLazy<T>( this IEnumerable seq, Func<object, T> convert )
		{
			foreach ( var x in seq )
				yield return convert( x );
		}

		public static Type GetInterface<T>( this Type ty )
		{
			return ty.GetInterface( typeof( T ).FullName );
		}

		public static bool HasInterface<T>( this Type ty )
		{
			return ty.GetInterface<T>() != null;
		}

		public static bool HasInterface( this Type ty, Type interfaceType )
		{
			return ty.GetInterface( interfaceType.FullName ) != null;
		}

		public static bool EndsWith( this string s, params string[] ends )
		{
			for ( var i = 0; i < ends.Length; ++i )
			{
				if ( s.EndsWith( ends[i] ) )
					return true;
			}
			return false;
		}

		public static Dictionary<T1, T2> ToDictionary<T1, T2>( this KeyValuePair<T1, T2>[] ary )
		{
			var dic = new Dictionary<T1, T2>( ary.Length );
			for ( var i = 0; i < ary.Length; ++i )
				dic.Add( ary[i].Key, ary[i].Value );
			return dic;
		}

		public static Dictionary<K2, V> ToDictionary<K1, K2, V>( this KeyValuePair<K1, V>[] ary, Func<K1, K2> keyConvert )
		{
			var dic = new Dictionary<K2, V>( ary.Length );
			for ( var i = 0; i < ary.Length; ++i )
				dic.Add( keyConvert( ary[i].Key ), ary[i].Value );
			return dic;
		}

		public static bool HasAttribute<T>( this MemberInfo host )
		{
			return Attribute.IsDefined( host, typeof( T ) );
		}

		public static LinkedList<T> ToLinkedList<T>( this IEnumerable<T> seq )
		{
			var list = new LinkedList<T>();
			foreach ( var x in seq )
				list.AddLast( x );
			return list;
		}

		public static LinkedList<T2> ToLinkedList<T1, T2>( this IEnumerable<T1> seq, Func<T1, T2> convert )
		{
			var list = new LinkedList<T2>();
			foreach ( var x in seq )
				list.AddLast( convert( x ) );
			return list;
		}

		public static string JoinNonEmpty( this string s, string delimiter, params string[] parts )
		{
			var sb = new StringBuilder( s );
			var needDelimiter = !string.IsNullOrEmpty( s );
			for ( var i = 0; i < parts.Length; ++i )
			{
				if ( string.IsNullOrEmpty( parts[i] ) )
					continue;
				if ( needDelimiter )
					sb.Append( delimiter );
				sb.Append( parts[i] );
				needDelimiter = true;
			}
			return sb.ToString();
		}

		public static void ForEach<T>( this IEnumerable<T> seq, Action<T> func )
		{
			foreach ( var x in seq )
				func( x );
		}

		public static IDictionary<string, object> Rewrite( this IDictionary<string, object> dic, IDictionary src )
		{
			dic.Clear();
			var iter = src.GetEnumerator();
			while ( iter.MoveNext() )
				dic.Add( iter.Key.ToString(), iter.Value );
			return dic;
		}

		public static Dictionary<K, U> ToDictionary<K, V, U>( this IEnumerable<KeyValuePair<K, V>> seq, Func<V, U> convert )
		{
			var dic = new Dictionary<K, U>();
            foreach (var x in seq)
            {
                if (!dic.Keys.Contains(x.Key))
                {
                 

                    dic.Add(x.Key, convert(x.Value));
                   
                }
            }
		    return dic;
		}

		public static Dictionary<K, V> ToDictionary<K, V>( this IEnumerable<KeyValuePair<K, V>> seq )
		{
			var dic = new Dictionary<K, V>();
			foreach ( var x in seq )
				dic.Add( x.Key, x.Value );
			return dic;
		}

		public static LinkedList<string> TrimSplit( this string s, char separator, bool skipEmpty )
		{
			var list = new LinkedList<string>();
			var sb = new StringBuilder();
			foreach ( char c in s )
			{
				if ( Char.IsWhiteSpace( c ) )
					continue;
				if ( c == separator )
				{
					if ( !skipEmpty || sb.Length > 0 )
					{
						list.AddLast( sb.ToString() );
						sb.Remove( 0, sb.Length );
					}
					continue;
				}
				sb.Append( c );
			}
			if ( !skipEmpty || sb.Length > 0 )
				list.AddLast( sb.ToString() );
			return list;
		}

		public static bool IsBaseOf( this Type baseType, Type derivedType )
		{
			for ( var x = derivedType.BaseType; x != null; x = x.BaseType )
			{
				if ( x == baseType )
					return true;
			}
			return false;
		}

		public static MethodInfo GetMethod( this Type ty, string name, Type[] parameterTypes, Type returnType, MethodAttributes attrs )
		{
			var methods = ( from m in ty.GetMethods( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic )
							where m.Name == name
								&& m.ReturnType == returnType
								&& ( m.Attributes & attrs ) != 0
								&& m.GetParameters().MapLazy( _ => _.ParameterType ).SequenceEqual( parameterTypes )
							select m ).Take( 2 ).ToLinkedList();
			if ( methods.Count > 1 )
				throw new AmbiguousMatchException();
			return methods.First.Value;
		}

        public static bool Contains<T>(this IEnumerable<T> seq, Func<T, bool> filter)
        {
            foreach (var x in seq)
            {
                if (filter(x))
                    return true;
            }
            return false;
        }

		public static bool OneOf( this string self, params string[] list )
		{
			for ( var i = 0; i < list.Length; ++i )
			{
				if ( list[i] == self )
					return true;
			}
			return false;
		}
	}
	

	#region KeyValuePair
	public static class KeyValuePair
	{
		public static KeyValuePair<K, V> Create<K, V>( K key, V value )
		{
			return new KeyValuePair<K, V>( key, value );
		}
	}
	#endregion
}
