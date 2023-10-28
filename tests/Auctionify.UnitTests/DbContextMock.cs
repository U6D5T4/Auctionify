using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using System.Linq.Expressions;

namespace Auctionify.UnitTests
{
    public class DbContextMock
    {
        public static Mock<TContext> GetMock<TData, TContext>(List<TData> lstData,
            Expression<Func<TContext, DbSet<TData>>> dbSetSelectionExpression,
            Mock<TContext>? context = null,
            List<string>? includes = null) where TData : class where TContext : DbContext
        {
            IQueryable<TData> lstDataQueryable = lstData.AsQueryable().BuildMockDbSet().Object;
            Mock<DbSet<TData>> dbSetMock = new Mock<DbSet<TData>>();
            Mock<TContext> dbContext = context ?? new Mock<TContext>();

            dbSetMock.As<IQueryable<TData>>().Setup(s => s.Provider).Returns(lstDataQueryable.Provider);
            dbSetMock.As<IQueryable<TData>>().Setup(s => s.Expression).Returns(lstDataQueryable.Expression);
            dbSetMock.As<IQueryable<TData>>().Setup(s => s.ElementType).Returns(lstDataQueryable.ElementType);
            dbSetMock.As<IQueryable<TData>>().Setup(s => s.GetEnumerator()).Returns(lstDataQueryable.GetEnumerator());
            dbSetMock.Setup(x => x.Add(It.IsAny<TData>())).Callback<TData>(lstData.Add);
            dbSetMock.Setup(x => x.AddRange(It.IsAny<IEnumerable<TData>>())).Callback<IEnumerable<TData>>(lstData.AddRange);
            dbSetMock.Setup(x => x.Remove(It.IsAny<TData>())).Callback<TData>(t => lstData.Remove(t));
            dbSetMock.Setup(x => x.RemoveRange(It.IsAny<IEnumerable<TData>>())).Callback<IEnumerable<TData>>(ts =>
            {
                foreach (var t in ts) { lstData.Remove(t); }
            });

            if (includes != null)
            {
                //foreach (var include in includes)
                //{
                //    dbSetMock.Setup(x => x.Inc).Returns(dbSetMock.Object);
                //}
            }

            dbContext.Setup(ctx => ctx.Set<TData>()).Returns(dbSetMock.Object);
            dbContext.Setup(dbSetSelectionExpression).Returns(dbSetMock.Object);

            return dbContext;
        }
    }
}
