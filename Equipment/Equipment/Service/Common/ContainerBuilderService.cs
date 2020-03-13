using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Equipment.Service.Common
{
	public class ContainerBuilderService
	{
        /// <summary>
        /// 反射加载继承接口的类
        /// </summary>
        /// <param name="services"></param>
		public static void Load<T>(IServiceCollection services) where T : class
        {
            string path = Assembly.GetEntryAssembly().Location;
            string bin = Path.GetDirectoryName(path);
            if (bin == null)
                throw new Exception("查无dll文件");
            string[] assemblies = Directory.GetFiles(bin, "*.dll");
            foreach (string file in assemblies)
            {
                try
                {
                    if (File.Exists(file))
                    {
                        Assembly asm = Assembly.LoadFrom(file); //Assembly：是一个程序集
                        //寻找实现定义接口的类 
                        var query = from t in asm.GetTypes()
                                    where t.IsClass && t.GetInterface(typeof(T).FullName) != null
                                    select t;

                        // 添加泛型集合到启动任务列表
                        foreach (Type type in query)
                        {
                            services.AddTransient(type);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }

        }
    }
}
