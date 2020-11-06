using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Faker
{
    public class FakerConfig
    {
        private readonly Dictionary<MemberInfo, IGenerator> _generators;

        private readonly List<Type> excludedTypes = new List<Type>();

        internal IGenerator GetGeneratorByName(string parameterName)
        {
            foreach (var member in _generators.Keys)
            {
                if (member.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase))
                {
                    _generators.TryGetValue(member, out var generator);
                    return generator;
                }
            }

            return null;
        }

        internal IGenerator GetGeneratorByMemberInfo(MemberInfo memberInfo)
        {
            _generators.TryGetValue(memberInfo, out var generator);
            return generator;
        }

        internal void ExcludeType(Type type)
        {
            excludedTypes.Add(type);
        }

        internal void RemoveFromExcludedTypes(Type type)
        {
            excludedTypes.Remove(type);
        }

        internal bool Excluded(Type type)
        {
            return excludedTypes.Contains(type);
        }

        public FakerConfig()
        {
            _generators = new Dictionary<MemberInfo, IGenerator>();
        }
        //
        // Expression<Func<TClass, object>>
        //
        public void Add<TClass, TMember, TGenerator>(Expression<Func<TClass, TMember>> expressionTree)
        where TGenerator: IGenerator
        {
            var generator = (IGenerator)Activator.CreateInstance<TGenerator>();

            var body = (MemberExpression)expressionTree.Body;
            var memberInfo = body.Member;
            _generators.Add(memberInfo, generator);
        }
    }
}