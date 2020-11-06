using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Faker
{
    public class Faker : IFaker
    {

        public Faker()
        {
            _config = new FakerConfig();
            _generators = new Dictionary<Type, IGenerator>();
            _genericGeneratorFactory = new List<IGenericGeneratorFactory>();
            LoadGenerators();
            LoadGeneratorsFromDirectory();    
        }

        public Faker(FakerConfig config)
        {
            _config = config;
            _generators = new Dictionary<Type, IGenerator>();
            _genericGeneratorFactory = new List<IGenericGeneratorFactory>();
            LoadGenerators();
            LoadGeneratorsFromDirectory();
        }
        
        private readonly Dictionary<Type, IGenerator> _generators;
        private readonly List<IGenericGeneratorFactory> _genericGeneratorFactory;
        private readonly string _pluginPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        private readonly FakerConfig _config;

        private void LoadGenerators()
        {
            var currentAssembly = Assembly.GetExecutingAssembly();
            LoadGeneratorsFromAssembly(currentAssembly);
        }

        private void LoadGeneratorsFromAssembly(Assembly assembly)
        {
            var types = assembly.GetTypes().Where(type =>
                    typeof(IGenerator).IsAssignableFrom(type) || typeof(IGenericGeneratorFactory).IsAssignableFrom(type));
            foreach (var type in types)
            {
                if (type.FullName == null) continue;
                if (type.GetInterfaces().Contains(typeof(IGenericGenerator))) continue;
                if (!type.IsClass) continue;
                if (assembly.CreateInstance(type.FullName) is IGenerator generatorPlugin)
                {
                    var generatorType = generatorPlugin.GetGenerationType();
                    _generators.Add(generatorType, generatorPlugin);
                }
                else if (assembly.CreateInstance(type.FullName) is IGenericGeneratorFactory generatorFactoryPlugin)
                {
                    _genericGeneratorFactory.Add(generatorFactoryPlugin);
                }
            }
        }

        private void LoadGeneratorsFromDirectory()
        {
            if (_generators == null) return;
            
            var pluginDirectory = new DirectoryInfo(_pluginPath);
            if (!pluginDirectory.Exists)
            {
                pluginDirectory.Create();
                return;
            }

            var pluginFiles = Directory.GetFiles(pluginDirectory.FullName,"*.dll");

            foreach (var pluginFile in pluginFiles)
            {
                var assembly = Assembly.LoadFrom(pluginFile);
                LoadGeneratorsFromAssembly(assembly);
            }
        }

        private object Create(Type type)
        {
            if (type.IsPointer) return IntPtr.Zero;
            var method = typeof(Faker).GetMethod("Create");
            if (method == null) return null;
            var genericMethod = method.MakeGenericMethod(type);
            return genericMethod.Invoke(this, null);
        }

        public object Create<T>()
        {
            var type = typeof(T);
            if (_config.Excluded(type) || type.IsAbstract) return null;
            _config.ExcludeType(type);
            var instance = new object();
            if (_generators.TryGetValue(type, out var generator))
            {
                instance = (T) generator.Generate();
            }
            else if (type.IsGenericType || type.IsArray || type.IsEnum)
            {
                var generators = CreateGenericGenerators(type);
                if (type.IsArray || type.IsEnum)
                {
                    type = type.BaseType;
                }

                if (type != null)
                {
                    if (generators.TryGetValue(type, out var genericGenerator))
                    {
                        instance = (T) genericGenerator.Generate();
                    }
                    else
                    {
                        instance = default(T);
                    }
                }
            }
            else if (type.IsClass || type.IsValueType)
            {    
                try
                {
                    instance = InitializeWithConstructor(type);
                    if (instance != null)
                    {
                        FillObject(instance);
                    }
                    
                }
                catch (Exception)
                {
                    instance = default(T);
                }
            }
            _config.RemoveFromExcludedTypes(type);
            return instance;
        }
        
        private Dictionary<Type, IGenerator> CreateGenericGenerators(Type type)
        {
            var generators = new Dictionary<Type, IGenerator>();
            foreach (var generatorFactory in _genericGeneratorFactory)
            {
                IGenerator generator;
                if (type.IsGenericType)
                {
                    generator = generatorFactory.GetGenerator(type.GetGenericArguments());
                }
                else if(type.GetElementType() != null)
                {
                    generator = generatorFactory.GetGenerator(new []{ type.GetElementType() });
                }
                else
                {
                    generator = generatorFactory.GetGenerator(new[] { type });
                }
                var generatorType = generator.GetGenerationType();
                if (!generators.ContainsKey(generatorType))
                {
                    generators.Add(generatorType, generator);
                }
            }

            return generators;
        }

        private void FillObject(object instance)
        {
            var type = instance.GetType();

            var members = new List<MemberInfo>(type.GetMembers());

            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Field)
                {
                    var fieldInfo = member as FieldInfo;
                    if (fieldInfo == null) continue;
                    if (fieldInfo.IsLiteral) continue;
                    var generator = _config.GetGeneratorByMemberInfo(fieldInfo);
                    if (generator != null)
                    {
                        fieldInfo.SetValue(instance, generator.Generate());
                    }
                    else
                    {
                        var fieldType = fieldInfo.FieldType;
                        var value = Create(fieldType);
                        fieldInfo.SetValue(instance, value);
                    }
                }
                else if (member.MemberType == MemberTypes.Property)
                {
                    var propertyInfo = member as PropertyInfo;
                    if (propertyInfo == null) continue;
                    if (!propertyInfo.CanWrite) continue;
                    var generator = _config.GetGeneratorByMemberInfo(propertyInfo);
                    if (generator != null)
                    {
                        propertyInfo.SetValue(instance, generator.Generate());
                    }
                    else
                    {
                        var propertyType = propertyInfo.PropertyType;
                        var value = Create(propertyType);
                        propertyInfo.SetValue(instance, value);
                    }
                }
            }
        }

        private static ConstructorInfo GetConstructorWithMaxNumberOfParameters(ConstructorInfo[] constructors)
        {
            if (constructors == null || constructors.Length <= 0) return null;
            
            var constructorInfo = constructors[0];
            foreach (var constructor in constructors)
            {
                if (constructor.GetParameters().Length > constructorInfo.GetParameters().Length)
                {
                    constructorInfo = constructor;
                }
            }

            return constructorInfo;
        }

        private object InitializeWithConstructor(Type type)
        {
            var constructorInfos = type.GetConstructors();
            var constructorInfo = GetConstructorWithMaxNumberOfParameters(constructorInfos);
            if (constructorInfo == null) return null;
            
            var constructorParameters = new List<object>();
            var parametersInfo = constructorInfo.GetParameters();
            foreach (var parameterInfo in parametersInfo)
            {
                var parameterName = parameterInfo.Name;
                var generator = _config.GetGeneratorByName(parameterName);
                if (generator != null)
                {
                    constructorParameters.Add(generator.Generate());
                }
                else
                {
                    var parameterType = parameterInfo.ParameterType;
                    var parameter = Create(parameterType);
                    constructorParameters.Add(parameter);
                }
                
            }
            var instance = constructorInfo.Invoke(constructorParameters.ToArray());

            return instance;
        }
    }
}