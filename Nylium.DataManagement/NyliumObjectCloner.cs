using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Nylium.DataManagement
{

    public delegate void BeforeCreateObjectEventHandler<T>(ref bool cancel, T existingObject) where T : class;



    public class NyliumObjectClonerFactory<T> where T : class
    {
        public event BeforeCreateObjectEventHandler<T> BeforeCreateObject;



        public void CloneObject(T originalObject, T newObject)
        {
            // Get all properties of the object (excluding those marked with [Key])
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                // Skip read-only and key properties (such as primary keys, identity fields)
                if (!property.CanWrite || property.GetCustomAttribute<System.ComponentModel.DataAnnotations.KeyAttribute>() != null)
                {
                    continue;
                }

                // Get the value from the original object
                var originalValue = property.GetValue(originalObject);

                // Handle reference types and collections separately if needed
                if (originalValue is System.Collections.IEnumerable && property.PropertyType != typeof(string))
                {
                    // Skip for now, unless you need to clone collections as well
                    continue;
                }

                // Copy the value to the new object
                property.SetValue(newObject, originalValue);
            }
        }


        public T CloneCreateObject(IObjectSpace objectSpace, T originalObject)
        {
            // Create a new instance of the object type

            bool cancel = false;
            T clonedObject = null;

            BeforeCreateObject?.Invoke(ref cancel, clonedObject);
            if (!cancel)
            {
                if (clonedObject == null)
                {
                    clonedObject = objectSpace.CreateObject<T>();
                }

            }

            if (clonedObject != null)
            {
                CloneObject(originalObject, clonedObject);
            }
            return clonedObject;
        }

        public void CloneObjectList(IList<T> originalList, IList<T> newList)
        {
            // Ensure that both lists have the same count
            if (originalList.Count != newList.Count)
            {
                throw new ArgumentException("Original and new list must have the same number of elements");
            }

            // Iterate through the list and clone each object
            for (int i = 0; i < originalList.Count; i++)
            {
                CloneObject(originalList[i], newList[i]);
            }
        }

        public IList<T> CloneCreateObjectList(IList<T> originalList, IObjectSpace objectSpace)
        {
            return CloneCreateObjectList(objectSpace, originalList);
        }

        public List<T> CloneCreateObjectList(IObjectSpace objectSpace, IList<T> originalList)
        {
            var clonedList = new List<T>();

            // Iterate through the list and clone each object
            foreach (T originalObject in originalList)
            {

                T clonedObject = CloneCreateObject(objectSpace, originalObject);
                clonedList.Add(clonedObject);
            }

            return clonedList;
        }
    }
}
