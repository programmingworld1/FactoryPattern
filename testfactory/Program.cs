using System;
using System.Collections.Generic;

namespace testfactory
{
    class Program
    {
        static void Main(string[] args)
        {
            PersonFactory.Instance().RegisterType<Man>("Man");
            PersonFactory.Instance().RegisterType<Woman>("Woman");
            //PersonFactory.Instance().RegisterType<Child>("Child"); //Compile type error (as expected)

            Person m = PersonFactory.Instance().Create("Man");
            Person w = PersonFactory.Instance().Create("Woman");
            Person c = PersonFactory.Instance().Create("Child");

            Console.ReadKey();
        }
    }

    public interface Person { }
    public class Man : Person { }
    public class Woman : Person { }
    public class Child { }

    /// <summary>
    /// Creational class used to create specific instances of <seealso cref="Person"/>'s
    /// </summary>
    public class PersonFactory
    {
        private static PersonFactory _instance;
        private Dictionary<string, Type> _factories;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <remarks>protected because of Singleton pattern</remarks>
        protected PersonFactory()
        {
            _factories = new Dictionary<string, Type>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static PersonFactory Instance()
        {
            if (_instance == null)
                _instance = new PersonFactory();

            return _instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns>Null if the type isn't found, an instance of typeName otherwise</returns>
        public Person Create(string typeName)
        {
            Person retval = null;

            if (_factories.ContainsKey(typeName))
                retval = (Person)Activator.CreateInstance(_factories[typeName]); //FIXME: This cast is still unfortunate, but because of the constraint at RegisterType it is save

            return retval;
        }

        /// <summary>
        /// Register the given type by typeName
        /// </summary>
        /// <typeparam name="T">Constraint: Must be typeof <seealso cref="Person"/></typeparam>
        /// <param name="typeName">The name of the type to register</param>
        /// <returns>True if the type is registered. False otherwise</returns>
        public bool RegisterType<T>(string typeName) where T : Person
        {
            if (_factories.ContainsKey(typeName))
                return false;

            _factories.Add(typeName, typeof(T));
            return true;
        }
    }

    /// <summary>
    /// The 'Singleton' class
    /// </summary>
    class Singleton
    {
        private static Singleton _instance;
        public Dictionary<string, Type> _factories { get; set; }

        // Constructor is 'protected'
        protected Singleton()
        {
            _factories = new Dictionary<string, Type>();
        }

        public static Singleton Instance()
        {
            // Uses lazy initialization.
            // Note: this is not thread safe.
            if (_instance == null)
            {
                _instance = new Singleton();
            }
            return _instance;
        }
    }

    // create objects
    public class ManageObject<T> where T : Person
    {
        Singleton singleton;

        int bla = 10;

        public ManageObject()
        {
            singleton = Singleton.Instance();
        }

        public Person Create(string type)
        {
            foreach (var item in singleton._factories)
            {
                if (item.Key == type)
                {
                    return (T)Activator.CreateInstance(item.Value, bla);
                }
            }
            return null;
        }

        public bool RegisterType(string type, Type tym)
        {
            if (tym == typeof(Person))
            {
                if (singleton._factories.ContainsValue(tym))
                    return false;

                singleton._factories.Add(type, tym);
                return true;
            }
            return false;
        }
    }
}
