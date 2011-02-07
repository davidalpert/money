using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.UserTypes;
using NHibernate.Type;
using System.Reflection;
using System.Data;
using NHibernate.Engine;

namespace Money.Storage.NHibernate
{
    /// <summary>
    /// A generic base class for implementing IComponentUserType
    /// </summary>
    /// <typeparam name="T">The type of the custom component type</typeparam>
    /// <remarks>
    /// <para>
    /// src: Mike Nichols http://geekswithblogs.net/opiesblog/archive/2006/08/13/87880.aspx
    /// </para>
    /// <para>
    /// This can be implemented a couple of ways. the easiest is simply to 
    /// overload the constructor in your subclass of this generic class and 
    /// pass anonymous methods in for the two delegates that peform the 
    /// Getting and the DeepCopying. 
    /// </para>
    /// <para>
    /// If the code-readability bothers you (but really any usage of anonymous 
    /// methods is going to introduce a bit of syntactical confusion at first...
    /// that goes away pretty quick tho), then you can simply use the Decorator of 
    /// this generic class and map to the superclasses methods (implementing 
    /// ICompositeUserType on your own class of course). 
    /// </para>
    /// <para>
    /// The two methods that invoke the delegates are virtual so this can make 
    /// your subclass a little cleaner.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// In my previous blog I used a StreetTypes Value Object. To use my generic 
    /// NHibernate user type I just need to do this.
    /// </para>
    /// First, the HBM Mapping File
    /// </para>
    /// <code>
    /// <property name="StreetType" type="Cei.eMerge.Data.NHibernateImpl.Types.StreetTypesUserType,Cei.eMerge.Data">
    ///   <column name="StreetTypeName" sql-type="varchar(50)"/>
    ///   <column name="StreetTypeAbbreviation" sql-type="varchar(50)"/> 
    /// </property>
    /// </code>
    /// 
    /// <para>
    /// Now the StreetTypesUserType Class
    /// </para>
    /// <code>
    /// public class StreetTypesUserType : GenericCompositeUserType&lt;StreetTypes&gt;
    /// {
    ///    public StreetTypesUserType() : this(true, 
    ///                 new IType[2] { NHibernateUtil.String, NHibernateUtil.String },
    ///                 new string[2] { "Name", "Abbreviation" }, null,
    ///                 new DeepCopyMap(delegate(StreetTypes obj)
    ///                 {
    ///                     return Algorithms.FindFirstWhere&lt;StreetTypes&gt;(
    ///                               StreetTypes.LIST_ALL,
    ///                               delegate(StreetTypes s)
    ///                               {
    ///                                    return s.Name.Equals(obj.Name);
    ///                               });
    ///                 }))
    ///    {
    ///    }
    ///    
    ///    public StreetTypesUserType(bool isMutable, 
    ///                               IType[] propertyTypes, 
    ///                               string[] propertyNames, 
    ///                               GetMap getMap, 
    ///                               DeepCopyMap deepCopyMethod)
    ///                  : base(isMutable, 
    ///                         propertyTypes, 
    ///                         propertyNames, 
    ///                         getMap,
    ///                         deepCopyMethod)
    ///    {
    ///    }
    /// }
    /// </code>
    /// </example>
    public class GenericCompositeUserType<T> : ICompositeUserType
    {
        private bool _isMutable;
        private IType[] _propertyTypes;
        private string[] _propertyNames;

        public GenericCompositeUserType(bool isMutable, IType[] propertyTypes,
                                        string[] propertyNames,
                                        GetMap getMap,
                                        DeepCopyMap deepCopyMethod)
        {
            _isMutable = isMutable;
            _propertyTypes = propertyTypes;
            _propertyNames = propertyNames;
            _get = getMap;
            _deepCopyMethod = deepCopyMethod;
        }
        /// <summary>
        /// Delegate for mapping values being returned from record to the implementor's Type.
        /// </summary>
        /// <remarks>
        /// We can pass an anonymous method into the constructor of the subclass for this mapping method.
        /// Implementors must do two things during this method if it is used:
        /// <list type="bullet">
        /// <item>A new instance of the mapped type must be created and returned.</item>
        /// <item>The values passed in must hydrate this new instance.</item>
        /// </list>
        /// </remarks>
        public GetMap Get
        {
            get { return _get; }
        }

        private GetMap _get;
        private DeepCopyMap _deepCopyMethod;

        public DeepCopyMap DeepCopyMethod
        {
            get { return _deepCopyMethod; }
        }
        /// <summary>
        /// A method that can override the default behavior to map properties. Handy if some kind of lookup to match
        /// the set of input values is how a new type is returned.
        /// </summary>
        /// <param name="values">The set of values corresponding to the <see cref="PropertyNames"/> expectations.</param>
        /// <returns>A <b>new</b> instance of the mapped type with values hydrated.</returns>
        public delegate T GetMap(object[] values);
        /// <summary>
        /// Used during caching for supporting the Assemble/Disassemble functions. i don't fully get yet the best way
        /// to generalize this.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public delegate T DeepCopyMap(T obj);


        public object GetPropertyValue(object component, int property)
        {
            PropertyInfo prop = typeof(T).GetProperty(PropertyNames[property], BindingFlags.Public |
                                                                                    BindingFlags.NonPublic |
                                                                                    BindingFlags.Static |
                                                                                    BindingFlags.Instance);
            object target = component;
            //Accomodate static properties (get/set accessors can be static)
            if (prop.GetGetMethod(true).IsStatic)
            {
                //NULL the target for static property handling
                target = null;
            }

            return prop.GetValue(target, null);
        }

        public void SetPropertyValue(object component, int property, object value)
        {
            if (!IsMutable)
            {
                throw new ArgumentException(typeof(T).UnderlyingSystemType.ToString() +
                    " is an immutable object.SetPropertyValue isn't supported.");
            }

            PropertyInfo prop = typeof(T).GetProperty(PropertyNames[property], BindingFlags.Public |
                                                                                    BindingFlags.NonPublic |
                                                                                    BindingFlags.Static |
                                                                                    BindingFlags.Instance);
            object target = component;
            //Accomodate static properties (get/set accessors can be static)
            if (prop.GetGetMethod(true).IsStatic)
            {
                //NULL the target for static property handling
                target = null;
            }
            prop.SetValue(target, value, null);

        }

        public new bool Equals(object x, object y)
        {
            if (x == y) return true;
            if (x == null || y == null) return false;
            return x.Equals(y);
        }
        /// <summary>
        /// This first checks to see if an implementation of <see cref="GetMap"/> has been provided. If not,
        /// it attempts to create an instance of &lt;T&gt; with full constructor initialization. If this fails
        /// an error is thrown. <b>NOTE</b> that if your constructors are all <b>string</b> types, an error will not
        /// be thrown but a incomplete instance will be created. Don't rely on this and be sure that your type
        /// provides a full constructor that has the args that match those defined in the <see cref="PropertyNames"/>
        /// property.
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="names"></param>
        /// <param name="session"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public virtual object NullSafeGet(IDataReader dr, string[] names, ISessionImplementor session, object owner)
        {
            if (dr == null)
            {
                return null;
            }


            object[] vals = new object[PropertyNames.Length];
            for (int i = 0; i < PropertyNames.Length; i++)
            {
                vals[i] = PropertyTypes[i].NullSafeGet(dr, names[i], session, owner);

            }
            if (Get != null)
            {
                return Get(vals);
            }


            try
            {
                return (T)Activator.CreateInstance(typeof(T), BindingFlags.NonPublic | BindingFlags.Static, null, vals, null);
            }
            catch (Exception)
            {
                throw new MissingMethodException("Since a GetMap Method wasn't provided and a constructor with " +
                                                vals.Length.ToString() +
                                                " args couldn't be found NHibernate couldn't map the returning values" +
                                                " to create a new instance of type '" + typeof(T).ToString() + "'. ");
            }

        }
        /// <summary>
        /// Write an instance of the mapped class to the input 'prepared statement'.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="value">The class being typed. (&lt;T&gt;)</param>
        /// <param name="index">The index to start the property mapping from.</param>
        /// <param name="session"></param>
        public void NullSafeSet(IDbCommand cmd, object value, int index, ISessionImplementor session)
        {
            if (value == null)
                return;
            int propIndexer = index;

            object target = value;

            for (int i = 0; i < PropertyNames.Length; i++)
            {
                PropertyInfo prop = typeof(T).GetProperty(PropertyNames[i]);
                //Accomodate static properties (get/set accessors can be static)
                if (prop.GetGetMethod(true).IsStatic)
                {
                    target = null;
                }

                object propValue = prop.GetValue(target, null);

                PropertyTypes[i].NullSafeSet(cmd, propValue, propIndexer, session);
                propIndexer++;
            }
        }

        public virtual object DeepCopy(object value)
        {
            T input = (T)value;
            if (value == null)
            {
                return value;
            }
            return DeepCopyMethod(input);
        }

        public object Disassemble(object value, ISessionImplementor session)
        {
            return DeepCopy(value);
        }

        public object Assemble(object cached, ISessionImplementor session, object owner)
        {
            return DeepCopy(cached);
        }
        /// <summary>
        /// An array of property names to map to the &lt;T&gt;. This should correspond to the column structure left-to-right.
        /// </summary>
        public string[] PropertyNames
        {
            get { return _propertyNames; }
        }
        /// <summary>
        /// The <see cref="IType"/> correspondents to the properties being mapped. use <b>NHibernateUtil.<i>x</i></b> static types
        /// for these.
        /// </summary>
        public IType[] PropertyTypes
        {
            get { return _propertyTypes; }
        }
        /// <summary>
        /// The <see cref="Type"/> of the class being mapped.
        /// </summary>
        public Type ReturnedClass
        {
            get { return typeof(T); }
        }
        /// <summary>
        /// Obviously, Value Objects should be false.
        /// </summary>
        public bool IsMutable
        {
            get { return _isMutable; }
        }

    }
}
