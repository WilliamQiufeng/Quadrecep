using System.Collections;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.ObjectGraphVisitors;
namespace Quadrecep.Map
{
    /// <summary>
    /// This is a modification from https://stackoverflow.com/a/64737155/11225486. <br/>
    /// This class skips empty collections when serializing to save space.
    /// </summary>
    internal sealed class YamlIEnumerableSkipEmptyObjectGraphVisitor : ChainedObjectGraphVisitor
    {
        public YamlIEnumerableSkipEmptyObjectGraphVisitor(IObjectGraphVisitor<IEmitter> nextVisitor): base(nextVisitor)
        {
        }

        private bool IsEmptyCollection(IObjectDescriptor value)
        {
            return value.Value switch
            {
                null => true,
                IEnumerable enumerable => !enumerable.GetEnumerator().MoveNext(),
                _ => false
            };
        }

        public override bool Enter(IObjectDescriptor value, IEmitter context)
        {
            return !IsEmptyCollection(value) && base.Enter(value, context);
        }

        public override bool EnterMapping(IPropertyDescriptor key, IObjectDescriptor value, IEmitter context)
        {
            return !IsEmptyCollection(value) && base.EnterMapping(key, value, context);
        }
    }

}