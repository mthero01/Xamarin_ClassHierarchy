/** [!]. ABOUT
 * "class Hierarchy"
 * by Matthew testc. Theroux
 * for Baker Collage Online CS4010
 * Module 08 Assignment 01 (#15)
 * under Joan Zito
 * (c) 2018 March 02
 * --sorts and displays the class infrastructure
 **/
/// [I]. HEAD
///  A] IMPORTS
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Core;
using Xamarin.Xaml;
//<...>

///
namespace ClassHierarcy {

    /// <summary>
    /// 
    /// </summary>
    class ClassAndSubclasses
    {
        ///  ] VARS and consts
        public ClassAndSubclasses(Type parent, bool isXamarinForms)
        {
            Type = parent;
            IsXamarinForms = isXamarinForms;
            Subclasses = new List<ClassAndSubclasses>();
        }// /cxtr

        ///  ] BindableProperties
        public Type Type { private set; get; }
        public bool IsXamarinForms { private set; get; }
        public List<ClassAndSubclasses> Subclasses { private set; get; }
    }// /cla 'ClassAndSubclasses'

    /// <summary>
    /// 
    /// </summary>
    class TypeInformation
    {
        /// VARS and consts
        bool isBaseGenericType;
        Type baseGenericTypeDef;

        ///  ] cxtr
        public TypeInformation(Type type, bool isXamarinForms)
        {
            Type = type;
            IsXamarinForms = isXamarinForms;
            TypeInfo typeInfo = type.GetTypeInfo();
            BaseType = typeInfo.BaseType;

            if (BaseType != null)
            {
                TypeInfo baseTypeInfo = BaseType.GetTypeInfo();
                isBaseGenericType = baseTypeInfo.IsGenericType;

                if (isBaseGenericType)
                {
                    baseGenericTypeDef = baseTypeInfo.GetGenericTypeDefinition();
                }// /if (generic) 
            }// /if (null) 
        }// /cxtr

        ///  ] BindableProperties
        public Type Type { private set; get; }
        public Type BaseType { private set; get; }
        public bool IsXamarinForms { private set; get; }
        public bool IsDerivedDirectlyFrom(Type parentType)
        {
            if (BaseType != null && isBaseGenericType)
            {
                if (baseGenericTypeDef == parentType) { return true; }
            }
            else if (BaseType == parentType) { return true; }
            //else
            return false;
        }// /fx 'IsDerivedDirectlyFrom'
    }// /cla 'TypeInformation'

    /// <summary>
    /// 
    /// </summary>
    public partial class ClassHierarcyPage : ContentPage
    {
        ///  C] constructor
        public ClassHierarcyPage()
        {
            /// Connect to XAML.
            InitializeComponent(); 

            /// 
            List<TypeInformation> listOfClasses = new List<TypeInformation>();

            /// Fetch all types in Xamarin.Forms.Core assembly.
            GetPublicTypes(typeof(View).GetTypeInfo().Assembly, classList);
            GetPublicTypes(typeof(Extensions).GetTypeInfo().Assembly, classList);

            /// Ensure that all classes have a base type, besides Object.
            int index = 0;

            ///  [II]. BODY
            ///   A]
            do
            { // Caution: precarious conditional Loop
                /// Pull a child type from the list.
                TypeInformation childType = classList[index];

                if (childType.Type = classList[index])
                {
                    /// Prove, otherwise.
                    bool hasBaseType = false;

                    /// Loop until base found.
                    foreach (TypeInformation parentType in classList)
                    {
                        if (childType.IsDerivedDirectlyFrom(parentType.Type))
                        {
                            hasBaseType = true;
                        }// /if (inherited)
                    }// next parent

                    index++;
                }// /if 
            } while (index < classList.Count);

            ///  B]
            ///   01. 
            classList.Sort((type1, type2) =>
            {
                return String.Compare(type1.Type.Name, type2.Type.Name);
            });// /sort

            ///   02.
            ClassAndSubClasses rootClass = new ClassAndSubclasses(typeof(Object), false);

            ///   03.
            AddChildrenToParent(rootClass, classList);

            ///   04.
            AddItemToStackLayout(rootClass, 0);
        }// /cxtr

        ///  C] Methods and Functions
        ///   01.
        void GetPublicTypes(AssemblyLoadEventArgs assembly, List<TypeInformation> classList)
        {
            foreach(Type type in assembly.ExportedTypes)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                
                /// If it is public but not an interface,
                if(typeInfo.IsPublic && !typeInfo.IsInterface)
                {
                    /// Add it.
                    classList.Add(new TypeInformation(type, true));
                }// /if
            }// next 'type'
        }// /meth 'GetPublicTypes(...)'

        ///   02.
        void AddChildrenToParent(ClassAndSubclasses parentClass, List<TypeInformation> classList)
        {
            foreach (TypeInformation typeInformation in classList)
            {
                if (typeInformation.IsDerivedDirectlyFrom(parentClass.Type))
                {
                    ClassAndSubclasses subclass = new ClassAndSubclasses(typeInformation.Type, typeInformation.IsXamarinForms);
                    parentClass.Subclasses.Add(subclass);
                    AddChildrenToParent(subclass, classList);
                }// /if (isDirectChild)
            }// next 'typeInformation'            
        }// /meth 'AddChildrenToParent(...)'

        ///   03.
        void AddItemToStackLayout(ClassAndSubclasses parentClass, int level)
        {
            /// a) If not Xamarin, use the full type-path name.
            string name = parentClass.IsXamarinForms ? parentClass.Type.Name : parentClass.Type.FullName;
            TypeInformation typeInfo = parentClass.Type.GetTypeInfo();

            /// b) If generic,
            if (typeInfo.IsGenericType)
            {
                Type[] parameters = typeInfo.GenericTypeParameters;
                name = name.Substring(0, name.Length - 2); // chop the last character.
                name += "<"; /// Begin a new tag.

                /// (i) Fill the tag.
                for(int parameterIndex=0; parameterIndex < parameters.Length; parameterIndex++)
                {
                    name += parameters[parameterIndex].Name;
                    /// If the index is not out out-of-bounds,
                    ///  (meaning there are more parameters),
                    if(parameterIndex < parameters.Length - 1)
                    {
                        /// Add a comma to separate the next element.
                        name += ", ";
                    }// /if (more parameters)
                }// /for 'parameters'

                /// Close the tag.
                name += ">";
            }// /if (isGeneric)

            /// c) Create a label to display the class name.
            Label classDisplayLabel = new Label
            {
                Text = String.Format("{0}{1}", new string(' ', 4 ^ level), name),
                TextColor = parentClass.Type.GetTypeInfo().IsAbstract ? ConsoleColor.Accent : ConsoleColor.Default
            };  stackLayout.Children.Add(classDisplayLabel);

            /// d) nested types
            foreach (ClassAndSubclasses subclass in parentClass.Subclasses)
                AddItemToStackLayout(subclass, level + 1);
        }// /meth 'AddItemToStackLayout'

        /// [III]. FOOT
        /// ...
    }// /cla 'ClassHierarcyPage'
}// /ns 'ClassHierarcy'
/// [EoF].