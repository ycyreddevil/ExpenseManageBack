using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace yuyu.Infrastructure
{
    public interface IUnitWork
    {
        yuyuDBContext GetDbContext();
        T FindSingle<T>(Expression<Func<T, bool>> exp = null) where T:class;
        bool IsExist<T>(Expression<Func<T, bool>> exp) where T:class;
        IQueryable<T> Find<T>(Expression<Func<T, bool>> exp = null) where T:class;

        IQueryable<T> Find<T>(int pageindex = 1, int pagesize = 10, string orderby = "",
            Expression<Func<T, bool>> exp = null) where T:class;

        int GetCount<T>(Expression<Func<T, bool>> exp = null) where T:class;

        void Add<T>(T entity) where T:class;

        void BatchAdd<T>(T[] entities) where T:class;

        /// <summary>
        /// 更新一个实体的所有属性
        /// </summary>
        void Update<T>(T entity) where T:class;

        void BatchUpdate<T>(T[] entities) where T : class;

        void Delete<T>(T entity) where T:class;

        /// <summary>
        /// 实现按需要只更新部分更新
        /// <para>如：Update<T>(u =>u.Id==1,u =>new User{Name="ok"}) where T:class;</para>
        /// </summary>
        /// <param name="where">更新条件</param>
        /// <param name="entity">更新后的实体</param>
        void Update<T>(Expression<Func<T, bool>> where, Expression<Func<T, T>> entity) where T:class;
        /// <summary>
        /// 批量删除
        /// </summary>
        void Delete<T>(Expression<Func<T, bool>> exp) where T:class;

        void Save();

        int ExecuteSql(string sql);

        DataTable ExecuteQuerySql(string sql);
    }
}