﻿using AutoMapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBCZ
{
    /// <summary>
    /// AutoMapper扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return MapTo<TDestination>(source, destination);
        }

        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        /// <typeparam name="TSource">源类型</typeparam>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <returns></returns>
        public static TDestination MapTo<TSource, TDestination>(this TSource source) where TDestination : new()
        {
            return MapTo(source, new TDestination());
        }

        /// <summary>
        /// 将源对象映射到目标对象
        /// </summary>
        /// <typeparam name="TDestination">目标类型</typeparam>
        /// <param name="source">源对象</param>
        /// <param name="destination">目标对象</param>
        /// <returns></returns>
        private static TDestination MapTo<TDestination>(object source, TDestination destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(source.GetType().Name);
            }
            if (destination == null)
            {
                throw new ArgumentNullException(destination.GetType().Name);
            }
            var sourceType = GetObjectType(source.GetType());
            var destinationType = GetObjectType(typeof(TDestination));
            try
            {
                var map = Mapper.Configuration.FindTypeMapFor(sourceType, destinationType);
                if (map != null)
                {
                    return Mapper.Map(source, destination);
                }
                var maps = Mapper.Configuration.GetAllTypeMaps();
                Mapper.Initialize(config =>
                {
                    foreach (var item in maps)
                    {
                        config.CreateMap(item.SourceType, item.DestinationType);
                    }
                    config.CreateMap(sourceType, destinationType);
                });

            }
            catch (InvalidOperationException)
            {
                Mapper.Initialize(config =>
                {
                    config.CreateMap(sourceType, destinationType);
                });
            }
            return Mapper.Map(source, destination);
        }

        /// <summary>
        /// 获取对象类型
        /// </summary>
        /// <param name="source">类型</param>
        /// <returns></returns>
        private static Type GetObjectType(Type source)
        {
            if (source.IsGenericType && typeof(IEnumerable).IsAssignableFrom(source))
            {
                var type = source.GetGenericArguments()[0];
                return GetObjectType(type);
            }
            return source;
        }
    }
}
