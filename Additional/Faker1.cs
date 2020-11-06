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
