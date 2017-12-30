using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Basic
{
    /// <summary>
    /// 通过反射获取静态字段
    /// </summary>
    public sealed class StaticMemberDynamicWrapper: System.Dynamic.DynamicObject
    {
        /// <summary>
        /// 表示类型的信息
        /// </summary>
        private readonly TypeInfo m_type;
        public StaticMemberDynamicWrapper(Type type):base()
        {
            m_type = type.GetTypeInfo();
        }
        /// <summary>
        /// 查找是否存在一个静态的方法
        /// </summary>
        /// <param name="name">方法名</param>
        /// <param name="paramTypes">参数列表的类型</param>
        /// <returns></returns>
        private MethodInfo FindMethod(String name, Type[] paramTypes)
        {
            return m_type.DeclaredMethods.FirstOrDefault((MethodInfo m) =>
            {
                return m.IsStatic && m.IsPublic && m.Name.Equals(name) && ParametersMatch(m.GetParameters(), paramTypes);
            });
        }
        /// <summary>
        /// 参数是否匹配
        /// </summary>
        /// <param name="parameters">函数的参数信息</param>
        /// <param name="paramsTypes">指定的参数Type信息</param>
        /// <returns></returns>
        private Boolean ParametersMatch(ParameterInfo[] parameters, Type[] paramsTypes)
        {
            if (parameters.Length != paramsTypes.Length)
                return false;
            for(Int32 i =0;i<parameters.Length;++i)
            {
                if (parameters[i].ParameterType != paramsTypes[i])
                    return false;
            }
            return true;
        }
        /// <summary>
        /// 返回字段的信息
        /// </summary>
        /// <param name="name">字段名</param>
        /// <returns></returns>
        private FieldInfo FindField(String name)
        {
            return m_type.DeclaredFields.FirstOrDefault(fi => fi.IsPublic &&
            fi.IsStatic && fi.Name.Equals(name));
        }
        /// <summary>
        /// 获取属性的访问器的元信息
        /// </summary>
        /// <param name="name">属性名称</param>
        /// <param name="get">设置get或者set的元信息</param>
        /// <returns></returns>
        private PropertyInfo FindProperty(String name, Boolean get)
        {
            if(get)
                return m_type.DeclaredProperties.FirstOrDefault(pi=>pi.Name.Equals(name) &&
                pi.GetMethod !=null && pi.GetMethod.IsPublic &&
                pi.GetMethod.IsStatic);
            return m_type.DeclaredProperties.FirstOrDefault(pi => pi.Name.Equals(name) &&
                pi.SetMethod != null && pi.SetMethod.IsPublic &&
                pi.SetMethod.IsStatic);
        }
        public override IEnumerable<String> GetDynamicMemberNames()
        {
            return m_type.DeclaredMembers.Select(mi => mi.Name);
        }
        public override Boolean TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            var field = FindField(binder.Name);
            if(field !=null)
            {
                result = field.GetValue(null);//获取值
                return true;
            }
            var prop = FindProperty(binder.Name, true);
            if(prop !=null)
            {
                result = prop.GetValue(null, null);return true;
            }
            return false;

        }
        public override Boolean TrySetMember(SetMemberBinder binder, Object value)
        {
            var field = FindField(binder.Name);
            if (field != null)
            {
                field.SetValue(null, value);
                return true;
            }
            var prop = FindProperty(binder.Name, true);
            if (prop != null)
            {
                prop.SetValue(null, value, null); return true;
            }
            return false;
        }
        public override Boolean TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            MethodInfo method = FindMethod(binder.Name, args.Select(c => c.GetType()).ToArray());
            if(method == null)
            {
                result = null;return false;
            }
            result = method.Invoke(null, args);
            return true;
        }
        public static void Main()
        {
            CodeTime.CodeTimer.Time("反射的性能", 10000, () =>
            {
                dynamic stringType = new StaticMemberDynamicWrapper(typeof(String));
                var v = stringType.Concat("A", "B");
            });
            CodeTime.CodeTimer.Time("不使用反射的性能", 10000, () =>
            {
                //dynamic stringType = new StaticMemberDynamicWrapper(typeof(String));
                var v = String.Concat("A", "B");
            });
        }
    }
}
